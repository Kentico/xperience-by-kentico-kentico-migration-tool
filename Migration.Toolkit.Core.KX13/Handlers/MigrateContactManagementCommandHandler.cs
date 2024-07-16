using CMS.Activities;
using CMS.ContactManagement;
using CMS.ContentEngine;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Core.KX13.Contexts;
using Migration.Toolkit.Core.KX13.Helpers;
using Migration.Toolkit.Core.KX13.Services;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.KX13.Handlers;

public class MigrateContactManagementCommandHandler(
    ILogger<MigrateContactManagementCommandHandler> logger,
    IDbContextFactory<KxpContext> kxpContextFactory,
    BulkDataCopyService bulkDataCopyService,
    ToolkitConfiguration toolkitConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KeyMappingContext keyMappingContext,
    CountryMigrator countryMigrator,
    KxpClassFacade kxpClassFacade,
    ISpoiledGuidContext spoiledGuidContext,
    IProtocol protocol)
    : IRequestHandler<MigrateContactManagementCommand, CommandResult>, IDisposable
{
    private readonly KxpContext _kxpContext = kxpContextFactory.CreateDbContext();

    public void Dispose() => _kxpContext.Dispose();

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
            { nameof(KX13M.OmContact.ContactId), nameof(OmContact.ContactId) },
            { nameof(KX13M.OmContact.ContactFirstName), nameof(OmContact.ContactFirstName) },
            { nameof(KX13M.OmContact.ContactMiddleName), nameof(OmContact.ContactMiddleName) },
            { nameof(KX13M.OmContact.ContactLastName), nameof(OmContact.ContactLastName) },
            { nameof(KX13M.OmContact.ContactJobTitle), nameof(OmContact.ContactJobTitle) },
            { nameof(KX13M.OmContact.ContactAddress1), nameof(OmContact.ContactAddress1) },
            { nameof(KX13M.OmContact.ContactCity), nameof(OmContact.ContactCity) },
            { nameof(KX13M.OmContact.ContactZip), nameof(OmContact.ContactZip) },
            { nameof(KX13M.OmContact.ContactStateId), nameof(OmContact.ContactStateId) },
            { nameof(KX13M.OmContact.ContactCountryId), nameof(OmContact.ContactCountryId) },
            { nameof(KX13M.OmContact.ContactMobilePhone), nameof(OmContact.ContactMobilePhone) },
            { nameof(KX13M.OmContact.ContactBusinessPhone), nameof(OmContact.ContactBusinessPhone) },
            { nameof(KX13M.OmContact.ContactEmail), nameof(OmContact.ContactEmail) },
            // No support 2022-07-07  { nameof(OmContact.ContactBirthday), nameof(KXO.Models.OmContact.ContactBirthday) },
            { nameof(KX13M.OmContact.ContactGender), nameof(OmContact.ContactGender) },
            // { nameof(OmContact.ContactStatusId), nameof(KXO.Models.OmContact.ContactStatusId) }, // No support 2022-07-07  but needs to be mapped because of constraint
            { nameof(KX13M.OmContact.ContactNotes), nameof(OmContact.ContactNotes) },
            { nameof(KX13M.OmContact.ContactOwnerUserId), nameof(OmContact.ContactOwnerUserId) },
            // No support 2022-07-07  { nameof(OmContact.ContactMonitored), nameof(KXO.Models.OmContact.ContactMonitored) },
            { nameof(KX13M.OmContact.ContactGuid), nameof(OmContact.ContactGuid) },
            { nameof(KX13M.OmContact.ContactLastModified), nameof(OmContact.ContactLastModified) },
            { nameof(KX13M.OmContact.ContactCreated), nameof(OmContact.ContactCreated) },
            // No support 2022-07-07  { nameof(OmContact.ContactBounces), nameof(KXO.Models.OmContact.ContactBounces) },
            { nameof(KX13M.OmContact.ContactCampaign), nameof(OmContact.ContactCampaign) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadId), nameof(KXO.Models.OmContact.ContactSalesForceLeadId) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDisabled), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDisabled) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDateTime) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationSuspensionDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationSuspensionDateTime) },
            { nameof(KX13M.OmContact.ContactCompanyName), nameof(OmContact.ContactCompanyName) }
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

        primaryKeyMappingContext.PreloadDependencies<KX13M.CmsUser>(u => u.UserId);
        primaryKeyMappingContext.PreloadDependencies<KX13M.CmsState>(u => u.StateId);
        primaryKeyMappingContext.PreloadDependencies<KX13M.CmsCountry>(u => u.CountryId);

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
            switch (primaryKeyMappingContext.MapSourceId<KX13M.CmsUser>(u => u.UserId, sourceUserId))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    // try search member
                    if (keyMappingContext.MapSourceKey<KX13M.CmsUser, CmsMember, int?>(
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
            }
        }

        if (columnName.Equals(nameof(OmContact.ContactStateId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceStateId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX13M.CmsState>(u => u.StateId, sourceStateId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<CmsState>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }
            }
        }

        if (columnName.Equals(nameof(OmContact.ContactCountryId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceCountryId)
        {
            switch (primaryKeyMappingContext.MapSourceId<KX13M.CmsCountry>(u => u.CountryId, sourceCountryId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    protocol.Append(HandbookReferences.MissingRequiredDependency<CmsCountry>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }
            }
        }


        return ValueInterceptorResult.DoNothing;
    }

    #endregion

    #region "Migrate contact activities"

    private CommandResult? MigrateContactActivities() //(List<int> migratedSiteIds)
    {
        var requiredColumnsForContactMigration = new Dictionary<string, string>
        {
            { nameof(KX13M.OmActivity.ActivityId), nameof(OmActivity.ActivityId) },
            { nameof(KX13M.OmActivity.ActivityContactId), nameof(OmActivity.ActivityContactId) },
            { nameof(KX13M.OmActivity.ActivityCreated), nameof(OmActivity.ActivityCreated) },
            { nameof(KX13M.OmActivity.ActivityType), nameof(OmActivity.ActivityType) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemId), nameof(KXO.Models.OmActivity.ActivityItemId) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemDetailId), nameof(KXO.Models.OmActivity.ActivityItemDetailId) },
            { nameof(KX13M.OmActivity.ActivityValue), nameof(OmActivity.ActivityValue) },
            { nameof(KX13M.OmActivity.ActivityUrl), nameof(OmActivity.ActivityUrl) },
            { nameof(KX13M.OmActivity.ActivityTitle), nameof(OmActivity.ActivityTitle) },
            { nameof(KX13M.OmActivity.ActivitySiteId), nameof(OmActivity.ActivityChannelId) },
            { nameof(KX13M.OmActivity.ActivityComment), nameof(OmActivity.ActivityComment) },
            // { nameof(OmActivity.ActivityCampaign), nameof(KXP.Models.OmActivity.ActivityCampaign) }, // deprecated without replacement in v27
            { nameof(KX13M.OmActivity.ActivityUrlreferrer), nameof(OmActivity.ActivityUrlreferrer) },
            { nameof(KX13M.OmActivity.ActivityCulture), nameof(OmActivity.ActivityLanguageId) },
            { nameof(KX13M.OmActivity.ActivityNodeId), nameof(OmActivity.ActivityWebPageItemGuid) },
            { nameof(KX13M.OmActivity.ActivityUtmsource), nameof(OmActivity.ActivityUtmsource) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityAbvariantName), nameof(KXO.Models.OmActivity.ActivityAbvariantName) },
            // OBSOLETE 26.0.0: { nameof(OmActivity.ActivityUrlhash), nameof(KXP.Models.OmActivity.ActivityUrlhash) },
            { nameof(KX13M.OmActivity.ActivityUtmcontent), nameof(OmActivity.ActivityUtmcontent) }
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
            s => true, // s => s != "ActivityID",
            reader => true, // migratedSiteIds.Contains(reader.GetInt32(reader.GetOrdinal("ActivitySiteID"))), // TODO tk: 2022-07-07 move condition to source query
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
        if (columnName.Equals(nameof(KX13M.OmActivity.ActivitySiteId), StringComparison.InvariantCultureIgnoreCase) &&
            value is int sourceActivitySiteId)
        {
            var result = keyMappingContext.MapSourceKey<KX13M.CmsSite, CmsChannel, int?>(
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
                    switch (toolkitConfiguration.UseOmActivitySiteRelationAutofix ?? AutofixEnum.Error)
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
            }
        }

        if (columnName.Equals(nameof(KX13M.OmActivity.ActivityNodeId), StringComparison.InvariantCultureIgnoreCase) && value is int activityNodeId)
        {
            if (currentRow.TryGetValue(nameof(KX13M.OmActivity.ActivitySiteId), out object? mSiteId) && mSiteId is int siteId)
            {
                if (spoiledGuidContext.GetNodeGuid(siteId, activityNodeId) is { } nodeGuid)
                {
                    return ValueInterceptorResult.ReplaceValue(nodeGuid);
                }
            }

            switch (toolkitConfiguration.UseOmActivityNodeRelationAutofix ?? AutofixEnum.Error)
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

        if (columnName.Equals(nameof(KX13M.OmActivity.ActivityCulture), StringComparison.InvariantCultureIgnoreCase) && value is string cultureCode)
        {
            return ValueInterceptorResult.ReplaceValue(ContentLanguageInfoProvider.ProviderObject.Get(cultureCode)?.ContentLanguageID);
        }

        return ValueInterceptorResult.DoNothing;
    }

    #endregion
}
