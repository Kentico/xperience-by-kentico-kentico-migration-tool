using CMS.Activities;
using CMS.ContactManagement;
using CMS.ContentEngine;

using MediatR;

using Microsoft.EntityFrameworkCore;
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
using Migration.Tool.KXP.Context;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX12.Handlers;

public class MigrateContactManagementCommandHandler(
    ILogger<MigrateContactManagementCommandHandler> logger,
    IDbContextFactory<KxpContext> kxpContextFactory,
    BulkDataCopyService bulkDataCopyService,
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KeyMappingContext keyMappingContext,
    CountryMigrator countryMigrator,
    KxpClassFacade kxpClassFacade,
    ISpoiledGuidContext spoiledGuidContext,
    IProtocol protocol
)
    : IRequestHandler<MigrateContactManagementCommand, CommandResult>, IDisposable
{
    private readonly KxpContext kxpContext = kxpContextFactory.CreateDbContext();

    public void Dispose() => kxpContext.Dispose();

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
            { nameof(KX12M.OmContact.ContactId), nameof(OmContact.ContactId) },
            { nameof(KX12M.OmContact.ContactFirstName), nameof(OmContact.ContactFirstName) },
            { nameof(KX12M.OmContact.ContactMiddleName), nameof(OmContact.ContactMiddleName) },
            { nameof(KX12M.OmContact.ContactLastName), nameof(OmContact.ContactLastName) },
            { nameof(KX12M.OmContact.ContactJobTitle), nameof(OmContact.ContactJobTitle) },
            { nameof(KX12M.OmContact.ContactAddress1), nameof(OmContact.ContactAddress1) },
            { nameof(KX12M.OmContact.ContactCity), nameof(OmContact.ContactCity) },
            { nameof(KX12M.OmContact.ContactZip), nameof(OmContact.ContactZip) },
            { nameof(KX12M.OmContact.ContactStateId), nameof(OmContact.ContactStateId) },
            { nameof(KX12M.OmContact.ContactCountryId), nameof(OmContact.ContactCountryId) },
            { nameof(KX12M.OmContact.ContactMobilePhone), nameof(OmContact.ContactMobilePhone) },
            { nameof(KX12M.OmContact.ContactBusinessPhone), nameof(OmContact.ContactBusinessPhone) },
            { nameof(KX12M.OmContact.ContactEmail), nameof(OmContact.ContactEmail) },
            // No support 2022-07-07  { nameof(OmContact.ContactBirthday), nameof(KXO.Models.OmContact.ContactBirthday) },
            { nameof(KX12M.OmContact.ContactGender), nameof(OmContact.ContactGender) },
            // { nameof(OmContact.ContactStatusId), nameof(KXO.Models.OmContact.ContactStatusId) }, // No support 2022-07-07  but needs to be mapped because of constraint
            { nameof(KX12M.OmContact.ContactNotes), nameof(OmContact.ContactNotes) },
            { nameof(KX12M.OmContact.ContactOwnerUserId), nameof(OmContact.ContactOwnerUserId) },
            // No support 2022-07-07  { nameof(OmContact.ContactMonitored), nameof(KXO.Models.OmContact.ContactMonitored) },
            { nameof(KX12M.OmContact.ContactGuid), nameof(OmContact.ContactGuid) },
            { nameof(KX12M.OmContact.ContactLastModified), nameof(OmContact.ContactLastModified) },
            { nameof(KX12M.OmContact.ContactCreated), nameof(OmContact.ContactCreated) },
            // No support 2022-07-07  { nameof(OmContact.ContactBounces), nameof(KXO.Models.OmContact.ContactBounces) },
            { nameof(KX12M.OmContact.ContactCampaign), nameof(OmContact.ContactCampaign) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadId), nameof(KXO.Models.OmContact.ContactSalesForceLeadId) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDisabled), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDisabled) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDateTime) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationSuspensionDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationSuspensionDateTime) },
            { nameof(KX12M.OmContact.ContactCompanyName), nameof(OmContact.ContactCompanyName) }
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
        if (columnName.Equals(nameof(OmContact.ContactCompanyName), StringComparison.InvariantCultureIgnoreCase))
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

        if (columnName.Equals(nameof(OmContact.ContactOwnerUserId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceUserId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX12M.CmsUser>(u => u.UserId, sourceUserId))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    // try search member
                    if (keyMappingContext.MapSourceKey<KX12M.CmsUser, CmsMember, int?>(
                            s => s.UserId,
                            s => s.UserGuid,
                            sourceUserId,
                            t => t.MemberId,
                            t => t.MemberGuid
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

        if (columnName.Equals(nameof(OmContact.ContactStateId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceStateId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX12M.CmsState>(u => u.StateId, sourceStateId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<CmsState>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(OmContact.ContactCountryId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceCountryId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX12M.CmsCountry>(u => u.CountryId, sourceCountryId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<CmsCountry>(columnName, value)
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
            { nameof(KX12M.OmActivity.ActivityId), nameof(OmActivity.ActivityId) },
            { nameof(KX12M.OmActivity.ActivityContactId), nameof(OmActivity.ActivityContactId) },
            { nameof(KX12M.OmActivity.ActivityCreated), nameof(OmActivity.ActivityCreated) },
            { nameof(KX12M.OmActivity.ActivityType), nameof(OmActivity.ActivityType) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemId), nameof(KXO.Models.OmActivity.ActivityItemId) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemDetailId), nameof(KXO.Models.OmActivity.ActivityItemDetailId) },
            { nameof(KX12M.OmActivity.ActivityValue), nameof(OmActivity.ActivityValue) },
            { nameof(KX12M.OmActivity.ActivityUrl), nameof(OmActivity.ActivityUrl) },
            { nameof(KX12M.OmActivity.ActivityTitle), nameof(OmActivity.ActivityTitle) },
            { nameof(KX12M.OmActivity.ActivitySiteId), nameof(OmActivity.ActivityChannelId) },
            { nameof(KX12M.OmActivity.ActivityComment), nameof(OmActivity.ActivityComment) },
            // { nameof(OmActivity.ActivityCampaign), nameof(KXP.Models.OmActivity.ActivityCampaign) }, // deprecated without replacement in v27
            { nameof(KX12M.OmActivity.ActivityUrlreferrer), nameof(OmActivity.ActivityUrlreferrer) },
            { nameof(KX12M.OmActivity.ActivityCulture), nameof(OmActivity.ActivityLanguageId) },
            { nameof(KX12M.OmActivity.ActivityNodeId), nameof(OmActivity.ActivityWebPageItemGuid) },
            { nameof(KX12M.OmActivity.ActivityUtmsource), nameof(OmActivity.ActivityUtmsource) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityAbvariantName), nameof(KXO.Models.OmActivity.ActivityAbvariantName) },
            // OBSOLETE 26.0.0: { nameof(OmActivity.ActivityUrlhash), nameof(KXP.Models.OmActivity.ActivityUrlhash) },
            { nameof(KX12M.OmActivity.ActivityUtmcontent), nameof(OmActivity.ActivityUtmcontent) }
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
            var result = keyMappingContext.MapSourceKey<KX12M.CmsSite, CmsChannel, int?>(
                s => s.SiteId,
                s => s.SiteGuid,
                sourceActivitySiteId.NullIfZero(),
                t => t.ChannelId,
                t => t.ChannelGuid
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
                                .MissingRequiredDependency<CmsChannel>(columnName, value)
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
                        .MissingRequiredDependency<CmsWebPageItem>(columnName, value)
                        .WithData(currentRow)
                    );
                    return ValueInterceptorResult.SkipRow;
            }
        }

        if (columnName.Equals(nameof(OmActivity.ActivityLanguageId), StringComparison.InvariantCultureIgnoreCase) && value is string cultureCode)
        {
            return ValueInterceptorResult.ReplaceValue(ContentLanguageInfoProvider.ProviderObject.Get(cultureCode)?.ContentLanguageID);
        }

        return ValueInterceptorResult.DoNothing;
    }

    #endregion
}
