using CMS.Activities;
using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.Globalization;
using CMS.Membership;
using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services;
using Migration.Tool.Common.Services.BulkCopy;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.Core.KX12.Helpers;
using Migration.Tool.Core.KX12.Services;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX12.Handlers;

public class MigrateContactManagementCommandHandler(
    ILogger<MigrateContactManagementCommandHandler> logger,
    BulkDataCopyService bulkDataCopyService,
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KeyMappingContext keyMappingContext,
    CountryMigrator countryMigrator,
    KxpClassFacade kxpClassFacade,
    ISpoiledGuidContext spoiledGuidContext,
    IProtocol protocol
)
    : IRequestHandler<MigrateContactManagementCommand, CommandResult>
{
    public Task<CommandResult> Handle(MigrateContactManagementCommand request, CancellationToken cancellationToken)
    {
        countryMigrator.MigrateCountriesAndStates();

        if (MigrateContacts() is { } ccr)
        {
            return Task.FromResult(ccr);
        }

        if (MigrateContactActivities() is { } acr)
        {
            return Task.FromResult(acr);
        }

        return Task.FromResult<CommandResult>(new GenericCommandResult());
    }

    #region "Migrate contacts"

    private CommandResult? MigrateContacts()
    {
        var requiredColumnsForContactMigration = new Dictionary<string, string>
        {
            { nameof(KX12M.OmContact.ContactId), nameof(ContactInfo.ContactID) },
            { nameof(KX12M.OmContact.ContactFirstName), nameof(ContactInfo.ContactFirstName) },
            { nameof(KX12M.OmContact.ContactMiddleName), nameof(ContactInfo.ContactMiddleName) },
            { nameof(KX12M.OmContact.ContactLastName), nameof(ContactInfo.ContactLastName) },
            { nameof(KX12M.OmContact.ContactJobTitle), nameof(ContactInfo.ContactJobTitle) },
            { nameof(KX12M.OmContact.ContactAddress1), nameof(ContactInfo.ContactAddress1) },
            { nameof(KX12M.OmContact.ContactCity), nameof(ContactInfo.ContactCity) },
            { nameof(KX12M.OmContact.ContactZip), nameof(ContactInfo.ContactZIP) },
            { nameof(KX12M.OmContact.ContactStateId), nameof(ContactInfo.ContactStateID) },
            { nameof(KX12M.OmContact.ContactCountryId), nameof(ContactInfo.ContactCountryID) },
            { nameof(KX12M.OmContact.ContactMobilePhone), nameof(ContactInfo.ContactMobilePhone) },
            { nameof(KX12M.OmContact.ContactBusinessPhone), nameof(ContactInfo.ContactBusinessPhone) },
            { nameof(KX12M.OmContact.ContactEmail), nameof(ContactInfo.ContactEmail) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactBirthday), nameof(KXO.Models.OmContact.ContactBirthday) },
            { nameof(KX12M.OmContact.ContactGender), nameof(ContactInfo.ContactGender) },
            // { nameof(ContactInfo.ContactStatusId), nameof(KXO.Models.OmContact.ContactStatusId) }, // No support 2022-07-07  but needs to be mapped because of constraint
            { nameof(KX12M.OmContact.ContactNotes), nameof(ContactInfo.ContactNotes) },
            { nameof(KX12M.OmContact.ContactOwnerUserId), nameof(ContactInfo.ContactOwnerUserID) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactMonitored), nameof(KXO.Models.OmContact.ContactMonitored) },
            { nameof(KX12M.OmContact.ContactGuid), nameof(ContactInfo.ContactGUID) },
            { nameof(KX12M.OmContact.ContactLastModified), nameof(ContactInfo.ContactLastModified) },
            { nameof(KX12M.OmContact.ContactCreated), nameof(ContactInfo.ContactCreated) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactBounces), nameof(KXO.Models.OmContact.ContactBounces) },
            { nameof(KX12M.OmContact.ContactCampaign), nameof(ContactInfo.ContactCampaign) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactSalesForceLeadId), nameof(KXO.Models.OmContact.ContactSalesForceLeadId) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactSalesForceLeadReplicationDisabled), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDisabled) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactSalesForceLeadReplicationDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDateTime) },
            // No support 2022-07-07  { nameof(ContactInfo.ContactSalesForceLeadReplicationSuspensionDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationSuspensionDateTime) },
            { nameof(KX12M.OmContact.ContactCompanyName), nameof(ContactInfo.ContactCompanyName) }
            // No support 2022-07-07  { nameof(ContactInfo.ContactSalesForceLeadReplicationRequired), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationRequired) },
        };

        foreach (var cfi in kxpClassFacade.GetCustomizedFieldInfos(ContactInfo.TYPEINFO.ObjectClassName))
        {
            requiredColumnsForContactMigration.Add(cfi.FieldName, cfi.FieldName);
        }

        if (bulkDataCopyService.CheckIfDataExistsInTargetTable("OM_Contact"))
        {
            protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable("OM_Contact"));
            logger.LogError("Data must not exist in target instance table, remove data before proceeding");
            return new CommandFailureResult();
        }

        if (bulkDataCopyService.CheckForTableColumnsDifferences("OM_Contact", requiredColumnsForContactMigration, out var differences))
        {
            protocol.Append(HandbookReferences
                .BulkCopyColumnMismatch("OM_Contact")
                .NeedsManualAction()
                .WithData(differences)
            );
            logger.LogError("Table {TableName} columns do not match, fix columns before proceeding", "OM_Contact");
            {
                return new CommandFailureResult();
            }
        }

        primaryKeyMappingContext.PreloadDependencies<KX12M.CmsUser>(u => u.UserId);
        primaryKeyMappingContext.PreloadDependencies<KX12M.CmsState>(u => u.StateId);
        primaryKeyMappingContext.PreloadDependencies<KX12M.CmsCountry>(u => u.CountryId);

        var bulkCopyRequest = new BulkCopyRequest("OM_Contact",
            s => true, // s => s != "ContactID",
            _ => true,
            50000,
            requiredColumnsForContactMigration.Keys.ToList(),
            ContactValueInterceptor,
            current => logger.LogError("Contact skipped due error, contact: {Contact}", PrintHelper.PrintDictionary(current)),
            "ContactID"
        );

        logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
        try
        {
            bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to migrate contacts");
            return new CommandFailureResult();
        }

        return null;
    }

    private ValueInterceptorResult ContactValueInterceptor(int ordinal, string columnName, object value, Dictionary<string, object?> currentRow)
    {
        if (columnName.Equals(nameof(ContactInfo.ContactCompanyName), StringComparison.InvariantCultureIgnoreCase))
        {
            // autofix removed in favor of error report and data consistency
            // var truncatedValue = SqlDataTypeHelper.TruncateString(value, 100);
            // return new ValueInterceptorResult(truncatedValue, true, false);

            if (value is string { Length: > 100 } s)
            {
                protocol.Append(HandbookReferences.ValueTruncationSkip("OM_Contact")
                    .WithData(new
                    {
                        value,
                        maxLength = 100,
                        s.Length,
                        columnName,
                        contact = PrintHelper.PrintDictionary(currentRow)
                    })
                );
                return ValueInterceptorResult.SkipRow;
            }
        }

        if (columnName.Equals(nameof(ContactInfo.ContactOwnerUserID), StringComparison.InvariantCultureIgnoreCase) && value is int sourceUserId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX12M.CmsUser>(u => u.UserId, sourceUserId))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    // try search member
                    if (keyMappingContext.MapSourceKey<KX12M.CmsUser, MemberInfo, int?>(
                            s => s.UserId,
                            s => s.UserGuid,
                            sourceUserId,
                            t => t.MemberID,
                            MemberInfo.Provider.Get
                        ) is { Success: true, Mapped: { } memberId })
                    {
                        return ValueInterceptorResult.ReplaceValue(memberId);
                    }

                    protocol.Append(HandbookReferences.MissingRequiredDependency<UserInfo>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(ContactInfo.ContactStateID), StringComparison.InvariantCultureIgnoreCase) && value is int sourceStateId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX12M.CmsState>(u => u.StateId, sourceStateId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<StateInfo>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(ContactInfo.ContactCountryID), StringComparison.InvariantCultureIgnoreCase) && value is int sourceCountryId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX12M.CmsCountry>(u => u.CountryId, sourceCountryId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<CountryInfo>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }


        return ValueInterceptorResult.DoNothing;
    }

    #endregion

    #region "Migrate contact activities"

    private CommandResult? MigrateContactActivities()
    {
        var requiredColumnsForContactMigration = new Dictionary<string, string>
        {
            { nameof(KX12M.OmActivity.ActivityId), nameof(ActivityInfo.ActivityID) },
            { nameof(KX12M.OmActivity.ActivityContactId), nameof(ActivityInfo.ActivityContactID) },
            { nameof(KX12M.OmActivity.ActivityCreated), nameof(ActivityInfo.ActivityCreated) },
            { nameof(KX12M.OmActivity.ActivityType), nameof(ActivityInfo.ActivityType) },
            // No support 2022-07-07  { nameof(ActivityInfo.ActivityItemId), nameof(KXO.Models.OmActivity.ActivityItemId) },
            // No support 2022-07-07  { nameof(ActivityInfo.ActivityItemDetailId), nameof(KXO.Models.OmActivity.ActivityItemDetailId) },
            { nameof(KX12M.OmActivity.ActivityValue), nameof(ActivityInfo.ActivityValue) },
            { nameof(KX12M.OmActivity.ActivityUrl), nameof(ActivityInfo.ActivityURL) },
            { nameof(KX12M.OmActivity.ActivityTitle), nameof(ActivityInfo.ActivityTitle) },
            { nameof(KX12M.OmActivity.ActivitySiteId), nameof(ActivityInfo.ActivityChannelID) },
            { nameof(KX12M.OmActivity.ActivityComment), nameof(ActivityInfo.ActivityComment) },
            // { nameof(ActivityInfo.ActivityCampaign), nameof(KXP.Models.OmActivity.ActivityCampaign) }, // deprecated without replacement in v27
            { nameof(KX12M.OmActivity.ActivityUrlreferrer), nameof(ActivityInfo.ActivityURLReferrer) },
            { nameof(KX12M.OmActivity.ActivityCulture), nameof(ActivityInfo.ActivityLanguageID) },
            { nameof(KX12M.OmActivity.ActivityNodeId), nameof(ActivityInfo.ActivityWebPageItemGUID) },
            { nameof(KX12M.OmActivity.ActivityUtmsource), nameof(ActivityInfo.ActivityUTMSource) },
            // No support 2022-07-07  { nameof(ActivityInfo.ActivityAbvariantName), nameof(KXO.Models.OmActivity.ActivityAbvariantName) },
            // OBSOLETE 26.0.0: { nameof(ActivityInfo.ActivityUrlhash), nameof(KXP.Models.OmActivity.ActivityUrlhash) },
            { nameof(KX12M.OmActivity.ActivityUtmcontent), nameof(ActivityInfo.ActivityUTMContent) }
        };

        foreach (var cfi in kxpClassFacade.GetCustomizedFieldInfos(ActivityInfo.TYPEINFO.ObjectClassName))
        {
            requiredColumnsForContactMigration.Add(cfi.FieldName, cfi.FieldName);
        }

        if (bulkDataCopyService.CheckIfDataExistsInTargetTable("OM_Activity"))
        {
            protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable("OM_Activity"));
            logger.LogError("Data must not exist in target instance table, remove data before proceeding");
            return new CommandFailureResult();
        }

        // _primaryKeyMappingContext.PreloadDependencies<CmsTree>(u => u.NodeId);
        // no need to preload contact, ID should stay same
        // _primaryKeyMappingContext.PreloadDependencies<OmContact>(u => u.ContactId);

        var bulkCopyRequest = new BulkCopyRequestExtended("OM_Activity",
            s => true,
            reader => true,
            50000,
            requiredColumnsForContactMigration,
            ActivityValueInterceptor,
            current => logger.LogError("Contact activity skipped due error, activity: {Activity}", PrintHelper.PrintDictionary(current)),
            "ActivityID"
        );

        logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);

        try
        {
            bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to migrate activities");
            return new CommandFailureResult();
        }

        return null;
    }

    private ValueInterceptorResult ActivityValueInterceptor(int columnOrdinal, string columnName, object value, Dictionary<string, object?> currentRow)
    {
        if (columnName.Equals(nameof(KX12M.OmActivity.ActivitySiteId), StringComparison.InvariantCultureIgnoreCase) &&
            value is int sourceActivitySiteId)
        {
            var result = keyMappingContext.MapSourceKey<KX12M.CmsSite, ChannelInfo, int?>(
                s => s.SiteId,
                s => s.SiteGuid,
                sourceActivitySiteId.NullIfZero(),
                t => t.ChannelID,
                guid => ChannelInfo.Provider.Get().WhereEquals(nameof(ChannelInfo.ChannelGUID), guid).SingleOrDefault()
            );
            switch (result)
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id ?? 0);
                case { Success: false }:
                {
                    switch (toolConfiguration.UseOmActivitySiteRelationAutofix ?? AutofixEnum.Error)
                    {
                        case AutofixEnum.DiscardData:
                            logger.LogTrace("Autofix (ActivitySiteId={ActivitySiteId} not exists) => discard data", sourceActivitySiteId);
                            return ValueInterceptorResult.SkipRow;
                        case AutofixEnum.AttemptFix:
                            logger.LogTrace("Autofix (ActivitySiteId={ActivitySiteId} not exists) => ActivityNodeId=0", sourceActivitySiteId);
                            return ValueInterceptorResult.ReplaceValue(0);
                        case AutofixEnum.Error:
                        default: //error
                            protocol.Append(HandbookReferences
                                .MissingRequiredDependency<ChannelInfo>(columnName, value)
                                .WithData(currentRow)
                            );
                            return ValueInterceptorResult.SkipRow;
                    }
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(KX12M.OmActivity.ActivityNodeId), StringComparison.InvariantCultureIgnoreCase) && value is int activityNodeId)
        {
            if (currentRow.TryGetValue(nameof(KX12M.OmActivity.ActivitySiteId), out object? mSiteId) && mSiteId is int siteId)
            {
                if (spoiledGuidContext.GetNodeGuid(siteId, activityNodeId) is { } nodeGuid)
                {
                    return ValueInterceptorResult.ReplaceValue(nodeGuid);
                }
            }

            switch (toolConfiguration.UseOmActivityNodeRelationAutofix ?? AutofixEnum.Error)
            {
                case AutofixEnum.DiscardData:
                    logger.LogTrace("Autofix (ActivitySiteId={NodeId} not exists) => discard data", activityNodeId);
                    return ValueInterceptorResult.SkipRow;
                case AutofixEnum.AttemptFix:
                    logger.LogTrace("Autofix (ActivityNodeId={NodeId} not exists) => ActivityNodeId=0", activityNodeId);
                    return ValueInterceptorResult.ReplaceValue(null);
                case AutofixEnum.Error:
                default: //error
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<CMS.Websites.Internal.WebPageItemInfo>(columnName, value)
                        .WithData(currentRow)
                    );
                    return ValueInterceptorResult.SkipRow;
            }
        }

        if (columnName.Equals(nameof(ActivityInfo.ActivityLanguageID), StringComparison.InvariantCultureIgnoreCase) && value is string cultureCode)
        {
            return ValueInterceptorResult.ReplaceValue(ContentLanguageInfoProvider.ProviderObject.Get(cultureCode)?.ContentLanguageID);
        }

        return ValueInterceptorResult.DoNothing;
    }

    #endregion
}
