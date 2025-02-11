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
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.Core.K11.Helpers;
using Migration.Tool.Core.K11.Services;
using Migration.Tool.K11.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.K11.Handlers;

public class MigrateContactManagementCommandHandler(
    ILogger<MigrateContactManagementCommandHandler> logger,
    BulkDataCopyService bulkDataCopyService,
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KeyMappingContext keyMappingContext,
    CountryMigrator countryMigrator,
    KxpClassFacade kxpClassFacade,
    ISpoiledGuidContext spoiledGuidContext,
    IProtocol protocol)
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
            { nameof(OmContact.ContactId), nameof(ContactInfo.ContactID) },
            { nameof(OmContact.ContactFirstName), nameof(ContactInfo.ContactFirstName) },
            { nameof(OmContact.ContactMiddleName), nameof(ContactInfo.ContactMiddleName) },
            { nameof(OmContact.ContactLastName), nameof(ContactInfo.ContactLastName) },
            { nameof(OmContact.ContactJobTitle), nameof(ContactInfo.ContactJobTitle) },
            { nameof(OmContact.ContactAddress1), nameof(ContactInfo.ContactAddress1) },
            { nameof(OmContact.ContactCity), nameof(ContactInfo.ContactCity) },
            { nameof(OmContact.ContactZip), nameof(ContactInfo.ContactZIP) },
            { nameof(OmContact.ContactStateId), nameof(ContactInfo.ContactStateID) },
            { nameof(OmContact.ContactCountryId), nameof(ContactInfo.ContactCountryID) },
            { nameof(OmContact.ContactMobilePhone), nameof(ContactInfo.ContactMobilePhone) },
            { nameof(OmContact.ContactBusinessPhone), nameof(ContactInfo.ContactBusinessPhone) },
            { nameof(OmContact.ContactEmail), nameof(ContactInfo.ContactEmail) },
            // No support 2022-07-07  { nameof(OmContact.ContactBirthday), nameof(KXO.Models.OmContact.ContactBirthday) },
            { nameof(OmContact.ContactGender), nameof(ContactInfo.ContactGender) },
            // { nameof(OmContact.ContactStatusId), nameof(KXO.Models.OmContact.ContactStatusId) }, // No support 2022-07-07  but needs to be mapped because of constraint
            { nameof(OmContact.ContactNotes), nameof(ContactInfo.ContactNotes) },
            { nameof(OmContact.ContactOwnerUserId), nameof(ContactInfo.ContactOwnerUserID) },
            // No support 2022-07-07  { nameof(OmContact.ContactMonitored), nameof(KXO.Models.OmContact.ContactMonitored) },
            { nameof(OmContact.ContactGuid), nameof(ContactInfo.ContactGUID) },
            { nameof(OmContact.ContactLastModified), nameof(ContactInfo.ContactLastModified) },
            { nameof(OmContact.ContactCreated), nameof(ContactInfo.ContactCreated) },
            // No support 2022-07-07  { nameof(OmContact.ContactBounces), nameof(KXO.Models.OmContact.ContactBounces) },
            { nameof(OmContact.ContactCampaign), nameof(ContactInfo.ContactCampaign) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadId), nameof(KXO.Models.OmContact.ContactSalesForceLeadId) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDisabled), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDisabled) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDateTime) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationSuspensionDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationSuspensionDateTime) },
            { nameof(OmContact.ContactCompanyName), nameof(ContactInfo.ContactCompanyName) }
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationRequired), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationRequired) },
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

        primaryKeyMappingContext.PreloadDependencies<CmsUser>(u => u.UserId);
        primaryKeyMappingContext.PreloadDependencies<CmsState>(u => u.StateId);
        primaryKeyMappingContext.PreloadDependencies<CmsCountry>(u => u.CountryId);

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
            switch (primaryKeyMappingContext.MapSourceId<CmsUser>(u => u.UserId, sourceUserId))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    // try search member
                    if (keyMappingContext.MapSourceKey<CmsUser, MemberInfo, int?>(
                            s => s.UserId,
                            s => s.UserGuid,
                            sourceUserId,
                            t => t.MemberID,
                            MemberInfo.Provider.Get
                        ) is { Success: true, Mapped: { } memberId })
                    {
                        return ValueInterceptorResult.ReplaceValue(memberId);
                    }

                    protocol.Append(HandbookReferences.MissingRequiredDependency<CmsUser>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(ContactInfo.ContactStateID), StringComparison.InvariantCultureIgnoreCase) && value is int sourceStateId)
        {
            switch (primaryKeyMappingContext.MapSourceId<CmsState>(u => u.StateId, sourceStateId.NullIfZero()))
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
            switch (primaryKeyMappingContext.MapSourceId<CmsCountry>(u => u.CountryId, sourceCountryId.NullIfZero()))
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
            { nameof(OmActivity.ActivityId), nameof(ActivityInfo.ActivityID) },
            { nameof(OmActivity.ActivityContactId), nameof(ActivityInfo.ActivityContactID) },
            { nameof(OmActivity.ActivityCreated), nameof(ActivityInfo.ActivityCreated) },
            { nameof(OmActivity.ActivityType), nameof(ActivityInfo.ActivityType) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemID), nameof(KXO.Models.OmActivity.ActivityItemID) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemDetailID), nameof(KXO.Models.OmActivity.ActivityItemDetailID) },
            { nameof(OmActivity.ActivityValue), nameof(ActivityInfo.ActivityValue) },
            { nameof(OmActivity.ActivityUrl), nameof(ActivityInfo.ActivityURL) },
            { nameof(OmActivity.ActivityTitle), nameof(ActivityInfo.ActivityTitle) },
            { nameof(OmActivity.ActivitySiteId), nameof(ActivityInfo.ActivityChannelID) },
            { nameof(OmActivity.ActivityComment), nameof(ActivityInfo.ActivityComment) },
            // { nameof(OmActivity.ActivityCampaign), nameof(ActivityInfo.ActivityCampaign) }, // deprecated without replacement in v27
            { nameof(OmActivity.ActivityUrlreferrer), nameof(ActivityInfo.ActivityURLReferrer) },
            { nameof(OmActivity.ActivityCulture), nameof(ActivityInfo.ActivityLanguageID) },
            { nameof(OmActivity.ActivityNodeId), nameof(ActivityInfo.ActivityWebPageItemGUID) },
            { nameof(OmActivity.ActivityUtmsource), nameof(ActivityInfo.ActivityUTMSource) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityAbvariantName), nameof(KXO.Models.OmActivity.ActivityAbvariantName) },
            // OBSOLETE 26.0.0: { nameof(OmActivity.ActivityUrlhash), nameof(ActivityInfo.ActivityUrlhash) },
            { nameof(OmActivity.ActivityUtmcontent), nameof(ActivityInfo.ActivityUTMContent) }
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

        var bulkCopyRequest = new BulkCopyRequestExtended("OM_Activity",
            s => true, // s => s != "ActivityID",
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
        if (columnName.Equals(nameof(OmActivity.ActivitySiteId), StringComparison.InvariantCultureIgnoreCase) &&
            value is int sourceActivitySiteId)
        {
            var result = keyMappingContext.MapSourceKey<CmsSite, ChannelInfo, int?>(
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

        if (columnName.Equals(nameof(OmActivity.ActivityNodeId), StringComparison.InvariantCultureIgnoreCase) && value is int activityNodeId)
        {
            if (currentRow.TryGetValue(nameof(OmActivity.ActivitySiteId), out object? mSiteId) && mSiteId is int siteId)
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
