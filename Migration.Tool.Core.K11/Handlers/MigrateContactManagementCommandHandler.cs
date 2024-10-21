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
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.Core.K11.Helpers;
using Migration.Tool.Core.K11.Services;
using Migration.Tool.K11.Models;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Context;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.K11.Handlers;

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
    IProtocol protocol)
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
            { nameof(Tool.K11.Models.OmContact.ContactId), nameof(KXP.Models.OmContact.ContactId) },
            { nameof(Tool.K11.Models.OmContact.ContactFirstName), nameof(KXP.Models.OmContact.ContactFirstName) },
            { nameof(Tool.K11.Models.OmContact.ContactMiddleName), nameof(KXP.Models.OmContact.ContactMiddleName) },
            { nameof(Tool.K11.Models.OmContact.ContactLastName), nameof(KXP.Models.OmContact.ContactLastName) },
            { nameof(Tool.K11.Models.OmContact.ContactJobTitle), nameof(KXP.Models.OmContact.ContactJobTitle) },
            { nameof(Tool.K11.Models.OmContact.ContactAddress1), nameof(KXP.Models.OmContact.ContactAddress1) },
            { nameof(Tool.K11.Models.OmContact.ContactCity), nameof(KXP.Models.OmContact.ContactCity) },
            { nameof(Tool.K11.Models.OmContact.ContactZip), nameof(KXP.Models.OmContact.ContactZip) },
            { nameof(Tool.K11.Models.OmContact.ContactStateId), nameof(KXP.Models.OmContact.ContactStateId) },
            { nameof(Tool.K11.Models.OmContact.ContactCountryId), nameof(KXP.Models.OmContact.ContactCountryId) },
            { nameof(Tool.K11.Models.OmContact.ContactMobilePhone), nameof(KXP.Models.OmContact.ContactMobilePhone) },
            { nameof(Tool.K11.Models.OmContact.ContactBusinessPhone), nameof(KXP.Models.OmContact.ContactBusinessPhone) },
            { nameof(Tool.K11.Models.OmContact.ContactEmail), nameof(KXP.Models.OmContact.ContactEmail) },
            // No support 2022-07-07  { nameof(OmContact.ContactBirthday), nameof(KXO.Models.OmContact.ContactBirthday) },
            { nameof(Tool.K11.Models.OmContact.ContactGender), nameof(KXP.Models.OmContact.ContactGender) },
            // { nameof(OmContact.ContactStatusId), nameof(KXO.Models.OmContact.ContactStatusId) }, // No support 2022-07-07  but needs to be mapped because of constraint
            { nameof(Tool.K11.Models.OmContact.ContactNotes), nameof(KXP.Models.OmContact.ContactNotes) },
            { nameof(Tool.K11.Models.OmContact.ContactOwnerUserId), nameof(KXP.Models.OmContact.ContactOwnerUserId) },
            // No support 2022-07-07  { nameof(OmContact.ContactMonitored), nameof(KXO.Models.OmContact.ContactMonitored) },
            { nameof(Tool.K11.Models.OmContact.ContactGuid), nameof(KXP.Models.OmContact.ContactGuid) },
            { nameof(Tool.K11.Models.OmContact.ContactLastModified), nameof(KXP.Models.OmContact.ContactLastModified) },
            { nameof(Tool.K11.Models.OmContact.ContactCreated), nameof(KXP.Models.OmContact.ContactCreated) },
            // No support 2022-07-07  { nameof(OmContact.ContactBounces), nameof(KXO.Models.OmContact.ContactBounces) },
            { nameof(Tool.K11.Models.OmContact.ContactCampaign), nameof(KXP.Models.OmContact.ContactCampaign) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadId), nameof(KXO.Models.OmContact.ContactSalesForceLeadId) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDisabled), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDisabled) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDateTime) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationSuspensionDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationSuspensionDateTime) },
            { nameof(Tool.K11.Models.OmContact.ContactCompanyName), nameof(KXP.Models.OmContact.ContactCompanyName) }
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

        primaryKeyMappingContext.PreloadDependencies<Tool.K11.Models.CmsUser>(u => u.UserId);
        primaryKeyMappingContext.PreloadDependencies<Tool.K11.Models.CmsState>(u => u.StateId);
        primaryKeyMappingContext.PreloadDependencies<Tool.K11.Models.CmsCountry>(u => u.CountryId);

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
        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactCompanyName), StringComparison.InvariantCultureIgnoreCase))
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

        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactOwnerUserId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceUserId)
        {
            switch (primaryKeyMappingContext.MapSourceId<Tool.K11.Models.CmsUser>(u => u.UserId, sourceUserId))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    // try search member
                    if (keyMappingContext.MapSourceKey<Tool.K11.Models.CmsUser, CmsMember, int?>(
                            s => s.UserId,
                            s => s.UserGuid,
                            sourceUserId,
                            t => t.MemberId,
                            t => t.MemberGuid
                        ) is { Success: true, Mapped: { } memberId })
                    {
                        return ValueInterceptorResult.ReplaceValue(memberId);
                    }

                    protocol.Append(HandbookReferences.MissingRequiredDependency<KXP.Models.CmsUser>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactStateId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceStateId)
        {
            switch (primaryKeyMappingContext.MapSourceId<Tool.K11.Models.CmsState>(u => u.StateId, sourceStateId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<KXP.Models.CmsState>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }

                default:
                    break;
            }
        }

        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactCountryId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceCountryId)
        {
            switch (primaryKeyMappingContext.MapSourceId<Tool.K11.Models.CmsCountry>(u => u.CountryId, sourceCountryId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<KXP.Models.CmsCountry>(columnName, value)
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
            { nameof(Tool.K11.Models.OmActivity.ActivityId), nameof(KXP.Models.OmActivity.ActivityId) },
            { nameof(Tool.K11.Models.OmActivity.ActivityContactId), nameof(KXP.Models.OmActivity.ActivityContactId) },
            { nameof(Tool.K11.Models.OmActivity.ActivityCreated), nameof(KXP.Models.OmActivity.ActivityCreated) },
            { nameof(Tool.K11.Models.OmActivity.ActivityType), nameof(KXP.Models.OmActivity.ActivityType) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemId), nameof(KXO.Models.OmActivity.ActivityItemId) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemDetailId), nameof(KXO.Models.OmActivity.ActivityItemDetailId) },
            { nameof(Tool.K11.Models.OmActivity.ActivityValue), nameof(KXP.Models.OmActivity.ActivityValue) },
            { nameof(Tool.K11.Models.OmActivity.ActivityUrl), nameof(KXP.Models.OmActivity.ActivityUrl) },
            { nameof(Tool.K11.Models.OmActivity.ActivityTitle), nameof(KXP.Models.OmActivity.ActivityTitle) },
            { nameof(Tool.K11.Models.OmActivity.ActivitySiteId), nameof(KXP.Models.OmActivity.ActivityChannelId) },
            { nameof(Tool.K11.Models.OmActivity.ActivityComment), nameof(KXP.Models.OmActivity.ActivityComment) },
            // { nameof(OmActivity.ActivityCampaign), nameof(KXP.Models.OmActivity.ActivityCampaign) }, // deprecated without replacement in v27
            { nameof(Tool.K11.Models.OmActivity.ActivityUrlreferrer), nameof(KXP.Models.OmActivity.ActivityUrlreferrer) },
            { nameof(Tool.K11.Models.OmActivity.ActivityCulture), nameof(KXP.Models.OmActivity.ActivityLanguageId) },
            { nameof(Tool.K11.Models.OmActivity.ActivityNodeId), nameof(KXP.Models.OmActivity.ActivityWebPageItemGuid) },
            { nameof(Tool.K11.Models.OmActivity.ActivityUtmsource), nameof(KXP.Models.OmActivity.ActivityUtmsource) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityAbvariantName), nameof(KXO.Models.OmActivity.ActivityAbvariantName) },
            // OBSOLETE 26.0.0: { nameof(OmActivity.ActivityUrlhash), nameof(KXP.Models.OmActivity.ActivityUrlhash) },
            { nameof(Tool.K11.Models.OmActivity.ActivityUtmcontent), nameof(KXP.Models.OmActivity.ActivityUtmcontent) }
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
        if (columnName.Equals(nameof(Tool.K11.Models.OmActivity.ActivitySiteId), StringComparison.InvariantCultureIgnoreCase) &&
            value is int sourceActivitySiteId)
        {
            var result = keyMappingContext.MapSourceKey<CmsSite, CmsChannel, int?>(
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

        if (columnName.Equals(nameof(Tool.K11.Models.OmActivity.ActivityNodeId), StringComparison.InvariantCultureIgnoreCase) && value is int activityNodeId)
        {
            if (currentRow.TryGetValue(nameof(Tool.K11.Models.OmActivity.ActivitySiteId), out object? mSiteId) && mSiteId is int siteId)
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

        if (columnName.Equals(nameof(KXP.Models.OmActivity.ActivityLanguageId), StringComparison.InvariantCultureIgnoreCase) && value is string cultureCode)
        {
            return ValueInterceptorResult.ReplaceValue(ContentLanguageInfoProvider.ProviderObject.Get(cultureCode)?.ContentLanguageID);
        }

        return ValueInterceptorResult.DoNothing;
    }

    #endregion
}
