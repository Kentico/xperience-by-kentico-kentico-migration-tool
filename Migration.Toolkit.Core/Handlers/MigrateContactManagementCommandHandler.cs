namespace Migration.Toolkit.Core.Handlers;

using CMS.Activities;
using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.Websites.Internal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

public class MigrateContactManagementCommandHandler : IRequestHandler<MigrateContactManagementCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateContactManagementCommandHandler> _logger;
    private readonly BulkDataCopyService _bulkDataCopyService;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KeyMappingContext _keyMappingContext;
    private readonly CountryMigrator _countryMigrator;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly IProtocol _protocol;
    private readonly KxpContext _kxpContext;

    public MigrateContactManagementCommandHandler(
        ILogger<MigrateContactManagementCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        BulkDataCopyService bulkDataCopyService,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KeyMappingContext keyMappingContext,
        CountryMigrator countryMigrator,
        KxpClassFacade kxpClassFacade,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContext = kxpContextFactory.CreateDbContext();
        _bulkDataCopyService = bulkDataCopyService;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _keyMappingContext = keyMappingContext;
        _countryMigrator = countryMigrator;
        _kxpClassFacade = kxpClassFacade;
        _protocol = protocol;
    }

    public Task<CommandResult> Handle(MigrateContactManagementCommand request, CancellationToken cancellationToken)
    {
        _countryMigrator.MigrateCountriesAndStates();

        if (MigrateContacts() is { } ccr) return Task.FromResult(ccr);
        if (MigrateContactActivities() is { } acr) return Task.FromResult(acr);

        return Task.FromResult<CommandResult>(new GenericCommandResult());
    }

    #region "Migrate contacts"

    private CommandResult? MigrateContacts()
    {
        var requiredColumnsForContactMigration = new Dictionary<string, string>
        {
            { nameof(OmContact.ContactId), nameof(KXP.Models.OmContact.ContactId) },
            { nameof(OmContact.ContactFirstName), nameof(KXP.Models.OmContact.ContactFirstName) },
            { nameof(OmContact.ContactMiddleName), nameof(KXP.Models.OmContact.ContactMiddleName) },
            { nameof(OmContact.ContactLastName), nameof(KXP.Models.OmContact.ContactLastName) },
            { nameof(OmContact.ContactJobTitle), nameof(KXP.Models.OmContact.ContactJobTitle) },
            { nameof(OmContact.ContactAddress1), nameof(KXP.Models.OmContact.ContactAddress1) },
            { nameof(OmContact.ContactCity), nameof(KXP.Models.OmContact.ContactCity) },
            { nameof(OmContact.ContactZip), nameof(KXP.Models.OmContact.ContactZip) },
            { nameof(OmContact.ContactStateId), nameof(KXP.Models.OmContact.ContactStateId) },
            { nameof(OmContact.ContactCountryId), nameof(KXP.Models.OmContact.ContactCountryId) },
            { nameof(OmContact.ContactMobilePhone), nameof(KXP.Models.OmContact.ContactMobilePhone) },
            { nameof(OmContact.ContactBusinessPhone), nameof(KXP.Models.OmContact.ContactBusinessPhone) },
            { nameof(OmContact.ContactEmail), nameof(KXP.Models.OmContact.ContactEmail) },
            // No support 2022-07-07  { nameof(OmContact.ContactBirthday), nameof(KXO.Models.OmContact.ContactBirthday) },
            { nameof(OmContact.ContactGender), nameof(KXP.Models.OmContact.ContactGender) },
            // { nameof(OmContact.ContactStatusId), nameof(KXO.Models.OmContact.ContactStatusId) }, // No support 2022-07-07  but needs to be mapped because of constraint
            { nameof(OmContact.ContactNotes), nameof(KXP.Models.OmContact.ContactNotes) },
            { nameof(OmContact.ContactOwnerUserId), nameof(KXP.Models.OmContact.ContactOwnerUserId) },
            // No support 2022-07-07  { nameof(OmContact.ContactMonitored), nameof(KXO.Models.OmContact.ContactMonitored) },
            { nameof(OmContact.ContactGuid), nameof(KXP.Models.OmContact.ContactGuid) },
            { nameof(OmContact.ContactLastModified), nameof(KXP.Models.OmContact.ContactLastModified) },
            { nameof(OmContact.ContactCreated), nameof(KXP.Models.OmContact.ContactCreated) },
            // No support 2022-07-07  { nameof(OmContact.ContactBounces), nameof(KXO.Models.OmContact.ContactBounces) },
            { nameof(OmContact.ContactCampaign), nameof(KXP.Models.OmContact.ContactCampaign) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadId), nameof(KXO.Models.OmContact.ContactSalesForceLeadId) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDisabled), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDisabled) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationDateTime) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationSuspensionDateTime), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationSuspensionDateTime) },
            { nameof(OmContact.ContactCompanyName), nameof(KXP.Models.OmContact.ContactCompanyName) },
            // No support 2022-07-07  { nameof(OmContact.ContactSalesForceLeadReplicationRequired), nameof(KXO.Models.OmContact.ContactSalesForceLeadReplicationRequired) },
        };

        foreach (var cfi in _kxpClassFacade.GetCustomizedFieldInfos(ContactInfo.TYPEINFO.ObjectClassName))
        {
            requiredColumnsForContactMigration.Add(cfi.FieldName, cfi.FieldName);
        }

        if (_bulkDataCopyService.CheckIfDataExistsInTargetTable("OM_Contact"))
        {
            _protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable("OM_Contact"));
            _logger.LogError("Data must not exist in target instance table, remove data before proceeding");
            return new CommandFailureResult();
        }

        if (_bulkDataCopyService.CheckForTableColumnsDifferences("OM_Contact", requiredColumnsForContactMigration, out var differences))
        {
            _protocol.Append(HandbookReferences
                .BulkCopyColumnMismatch("OM_Contact")
                .NeedsManualAction()
                .WithData(differences)
            );
            _logger.LogError("Table {TableName} columns do not match, fix columns before proceeding", "OM_Contact");
            {
                return new CommandFailureResult();
            }
        }

        _primaryKeyMappingContext.PreloadDependencies<CmsUser>(u => u.UserId);
        _primaryKeyMappingContext.PreloadDependencies<CmsState>(u => u.StateId);
        _primaryKeyMappingContext.PreloadDependencies<CmsCountry>(u => u.CountryId);

        var bulkCopyRequest = new BulkCopyRequest("OM_Contact",
            s => true,// s => s != "ContactID",
            _ => true,
            50000,
            requiredColumnsForContactMigration.Keys.ToList(),
            ContactValueInterceptor,
            current => { _logger.LogError("Contact skipped due error, contact: {Contact}", PrintHelper.PrintDictionary(current)); },
            "ContactID"
        );

        _logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
        try
        {
            _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to migrate contacts");
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
                _protocol.Append(HandbookReferences.ValueTruncationSkip("OM_Contact")
                    .WithData(new { value, maxLength = 100, s.Length, columnName, contact = PrintHelper.PrintDictionary(currentRow) })
                );
                return ValueInterceptorResult.SkipRow;
            }
        }

        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactOwnerUserId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceUserId)
        {
            switch (_primaryKeyMappingContext.MapSourceId<CmsUser>(u => u.UserId, sourceUserId))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    // try search member
                    if (_keyMappingContext.MapSourceKey<CmsUser, KXP.Models.CmsMember, int?>(
                            s => s.UserId,
                            s => s.UserGuid,
                            sourceUserId,
                            t => t.MemberId,
                            t => t.MemberGuid
                        ) is { Success:true, Mapped: {} memberId })
                    {
                        return ValueInterceptorResult.ReplaceValue(memberId);
                    }
                    _protocol.Append(HandbookReferences.MissingRequiredDependency<KXP.Models.CmsUser>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }
            }
        }

        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactStateId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceStateId)
        {
            switch (_primaryKeyMappingContext.MapSourceId<CmsState>(u => u.StateId, sourceStateId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    _protocol.Append(HandbookReferences.MissingRequiredDependency<KXP.Models.CmsState>(columnName, value)
                        .WithData(currentRow));
                    return ValueInterceptorResult.SkipRow;
                }
            }
        }

        if (columnName.Equals(nameof(KXP.Models.OmContact.ContactCountryId), StringComparison.InvariantCultureIgnoreCase) && value is int sourceCountryId)
        {
            switch (_primaryKeyMappingContext.MapSourceId<CmsCountry>(u => u.CountryId, sourceCountryId.NullIfZero()))
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id);
                case { Success: false }:
                {
                    _protocol.Append(HandbookReferences.MissingRequiredDependency<KXP.Models.CmsCountry>(columnName, value)
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
            { nameof(OmActivity.ActivityId), nameof(KXP.Models.OmActivity.ActivityId) },
            { nameof(OmActivity.ActivityContactId), nameof(KXP.Models.OmActivity.ActivityContactId) },
            { nameof(OmActivity.ActivityCreated), nameof(KXP.Models.OmActivity.ActivityCreated) },
            { nameof(OmActivity.ActivityType), nameof(KXP.Models.OmActivity.ActivityType) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemId), nameof(KXO.Models.OmActivity.ActivityItemId) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityItemDetailId), nameof(KXO.Models.OmActivity.ActivityItemDetailId) },
            { nameof(OmActivity.ActivityValue), nameof(KXP.Models.OmActivity.ActivityValue) },
            { nameof(OmActivity.ActivityUrl), nameof(KXP.Models.OmActivity.ActivityUrl) },
            { nameof(OmActivity.ActivityTitle), nameof(KXP.Models.OmActivity.ActivityTitle) },
            { nameof(OmActivity.ActivitySiteId), nameof(KXP.Models.OmActivity.ActivityChannelId) },
            { nameof(OmActivity.ActivityComment), nameof(KXP.Models.OmActivity.ActivityComment) },
            // { nameof(OmActivity.ActivityCampaign), nameof(KXP.Models.OmActivity.ActivityCampaign) }, // deprecated without replacement in v27
            { nameof(OmActivity.ActivityUrlreferrer), nameof(KXP.Models.OmActivity.ActivityUrlreferrer) },
            { nameof(OmActivity.ActivityCulture), nameof(KXP.Models.OmActivity.ActivityLanguageId) },
            { nameof(OmActivity.ActivityNodeId), nameof(KXP.Models.OmActivity.ActivityWebPageItemGuid) },
            { nameof(OmActivity.ActivityUtmsource), nameof(KXP.Models.OmActivity.ActivityUtmsource) },
            // No support 2022-07-07  { nameof(OmActivity.ActivityAbvariantName), nameof(KXO.Models.OmActivity.ActivityAbvariantName) },
            // OBSOLETE 26.0.0: { nameof(OmActivity.ActivityUrlhash), nameof(KXP.Models.OmActivity.ActivityUrlhash) },
            { nameof(OmActivity.ActivityUtmcontent), nameof(KXP.Models.OmActivity.ActivityUtmcontent) },
        };

        foreach (var cfi in _kxpClassFacade.GetCustomizedFieldInfos(ActivityInfo.TYPEINFO.ObjectClassName))
        {
            requiredColumnsForContactMigration.Add(cfi.FieldName, cfi.FieldName);
        }

        if (_bulkDataCopyService.CheckIfDataExistsInTargetTable("OM_Activity"))
        {
            _protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable("OM_Activity"));
            _logger.LogError("Data must not exist in target instance table, remove data before proceeding");
            return new CommandFailureResult();
        }

        // _primaryKeyMappingContext.PreloadDependencies<CmsTree>(u => u.NodeId);
        // no need to preload contact, ID should stay same
        // _primaryKeyMappingContext.PreloadDependencies<OmContact>(u => u.ContactId);

        var bulkCopyRequest = new BulkCopyRequestExtended("OM_Activity",
            s => true,// s => s != "ActivityID",
            reader => true, // migratedSiteIds.Contains(reader.GetInt32(reader.GetOrdinal("ActivitySiteID"))), // TODO tk: 2022-07-07 move condition to source query
            50000,
            requiredColumnsForContactMigration,
            ActivityValueInterceptor,
            current => { _logger.LogError("Contact activity skipped due error, activity: {Activity}", PrintHelper.PrintDictionary(current)); },
            "ActivityID"
        );

        _logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);

        try
        {
            _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to migrate activities");
            return new CommandFailureResult();
        }
        return null;
    }

    private ValueInterceptorResult ActivityValueInterceptor(int columnOrdinal, string columnName, object value, Dictionary<string, object?> currentRow)
    {
        if (columnName.Equals(nameof(KXP.Models.OmActivity.ActivityChannelId), StringComparison.InvariantCultureIgnoreCase) &&
            value is int sourceActivitySiteId)
        {
            var result = _keyMappingContext.MapSourceKey<KX13M.CmsTree, ChannelInfo, int?>(
                s => s.NodeId,
                s => s.NodeGuid,
                sourceActivitySiteId.NullIfZero(),
                t => t.ChannelID,
                t => t.ChannelGUID
            );
            switch(result)
            {
                case (true, var id):
                    return ValueInterceptorResult.ReplaceValue(id ?? 0);
                case { Success: false }:
                {
                    switch (_toolkitConfiguration.UseOmActivitySiteRelationAutofix ?? AutofixEnum.Error)
                    {
                        case AutofixEnum.DiscardData:
                            _logger.LogTrace("Autofix (ActivitySiteId={ActivitySiteId} not exists) => discard data", sourceActivitySiteId);
                            return ValueInterceptorResult.SkipRow;
                        case AutofixEnum.AttemptFix:
                            _logger.LogTrace("Autofix (ActivitySiteId={ActivitySiteId} not exists) => ActivityNodeId=0", sourceActivitySiteId);
                            return ValueInterceptorResult.ReplaceValue(0);
                        case AutofixEnum.Error:
                        default: //error
                            _protocol.Append(HandbookReferences
                                .MissingRequiredDependency<KXP.Models.CmsChannel>(columnName, value)
                                .WithData(currentRow)
                            );
                            return ValueInterceptorResult.SkipRow;
                    }
                }
            }
        }

        if (columnName.Equals(nameof(KXP.Models.OmActivity.ActivityWebPageItemGuid), StringComparison.InvariantCultureIgnoreCase) && value is int activityNodeId)
        {
            var result = _keyMappingContext.MapSourceKey<KX13M.CmsTree, WebPageItemInfo, Guid?>(
                s => s.NodeId,
                s => s.NodeGuid,
                activityNodeId.NullIfZero(), t => t.WebPageItemGUID, t => t.WebPageItemGUID);
            switch(result)
            {
                case (true, var guid):
                    return ValueInterceptorResult.ReplaceValue(guid);
                case { Success: false }:
                {
                    switch (_toolkitConfiguration.UseOmActivityNodeRelationAutofix ?? AutofixEnum.Error)
                    {
                        case AutofixEnum.DiscardData:
                            _logger.LogTrace("Autofix (ActivitySiteId={NodeId} not exists) => discard data", activityNodeId);
                            return ValueInterceptorResult.SkipRow;
                        case AutofixEnum.AttemptFix:
                            _logger.LogTrace("Autofix (ActivityNodeId={NodeId} not exists) => ActivityNodeId=0", activityNodeId);
                            return ValueInterceptorResult.ReplaceValue(null);
                        case AutofixEnum.Error:
                        default: //error
                            _protocol.Append(HandbookReferences
                                .MissingRequiredDependency<KXP.Models.CmsWebPageItem>(columnName, value)
                                .WithData(currentRow)
                            );
                            return ValueInterceptorResult.SkipRow;
                    }
                }
            }
        }

        if (columnName.Equals( nameof(KXP.Models.OmActivity.ActivityLanguageId), StringComparison.InvariantCultureIgnoreCase) && value is string cultureCode)
        {
            return ValueInterceptorResult.ReplaceValue(ContentLanguageInfoProvider.ProviderObject.Get(cultureCode)?.ContentLanguageID);
        }

        return ValueInterceptorResult.DoNothing;
    }

    #endregion

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}