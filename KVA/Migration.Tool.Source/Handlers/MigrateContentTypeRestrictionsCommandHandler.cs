using AngleSharp.Text;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Websites;
using CMS.Websites.Internal;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.KXP.Api;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Handlers;

public class MigrateContentTypeRestrictionsCommandHandler(
    ILogger<MigratePageTypesCommandHandler> logger,
    KxpClassFacade kxpClassFacade,
    ToolConfiguration toolConfiguration,
    ModelFacade modelFacade,
    IImporter importer
    )
    : IRequestHandler<MigrateContentTypeRestrictionsCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateContentTypeRestrictionsCommand request, CancellationToken cancellationToken)
    {
        await MigrateAllowedChildrenTypes();
        await MigratePageScopes();

        return new GenericCommandResult();
    }

    private async Task MigrateAllowedChildrenTypes()
    {
        // Delete allowed child content type bypass, created by MigratePageTypesCommand for the case user doesn't want to use restrictions
        AllowedChildContentTypeInfo.Provider.BulkDelete(new WhereCondition());

        var ksRecords = modelFacade.SelectAll<ICmsAllowedChildClasses>();
        var ksClasses = modelFacade.SelectAll<ICmsClass>().ToDictionary(x => x.ClassID);

        foreach (var ksRecord in ksRecords)
        {
            var parentClass = ksClasses.GetValueOrDefault(ksRecord.ParentClassID);
            if (parentClass is null)
            {
                logger.LogError($"Cannot migrate allowed child content type ({nameof(ICmsAllowedChildClasses.ParentClassID)}={{ParentClassID}},{nameof(ICmsAllowedChildClasses.ChildClassID)}={{ChildClassID}}). Parent class not found by ID.", ksRecord.ParentClassID, ksRecord.ChildClassID);
                continue;
            }

            if (new[] { "cms.root", "cms.folder" }.Any(x => parentClass.ClassName.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Ignore. XbyK doesn't have equivalent classes. To limit types under root or folder, web page scopes should be used
                logger.LogInformation("Skipping allowed child type entry with parent class CMS.Root/CMS.Folder");
                continue;
            }

            var childClass = ksClasses.GetValueOrDefault(ksRecord.ChildClassID);
            if (childClass is null)
            {
                logger.LogError($"Cannot migrate allowed child content type ({nameof(ICmsAllowedChildClasses.ParentClassID)}={{ParentClassID}},{nameof(ICmsAllowedChildClasses.ChildClassID)}={{ChildClassID}}). Child class not found by ID.", ksRecord.ParentClassID, ksRecord.ChildClassID);
                continue;
            }

            if (childClass.ClassName.Equals("cms.folder", StringComparison.InvariantCultureIgnoreCase))
            {
                // Ignore. We allow folders to be parented by any content type
                logger.LogInformation("Skipping allowed child type entry with child class CMS.Folder");
                continue;
            }

            var importResult = await importer.ImportAsync(new AllowedChildContentTypeModel
            {
                AllowedChildContentTypeParentGuid = parentClass.ClassGUID,
                AllowedChildContentTypeChildGuid = childClass.ClassGUID,
            });

            LogImportResult<AllowedChildContentTypeInfo>(importResult, x => $"Parent='{parentClass.ClassName}' ({parentClass.ClassGUID}), Child='{childClass.ClassName}' ({childClass.ClassGUID})");
        }
    }

    private void LogImportResult<T>(IImportResult importResult, Func<T, string> importedObjectDescription)
    {
        switch (importResult)
        {
            case { Success: true, Imported: T imported }:
            {
                logger.LogInformation("Imported {ImportedObjectType} {ImportedObjectDescription}", typeof(T).Name, importedObjectDescription(imported));
                break;
            }
            case { Success: false, Exception: { } exception }:
            {
                logger.LogError("Failed to import allowed child content type {ImportedObjectDescription}: {Error}", importedObjectDescription, exception.ToString());
                break;
            }
            case { Success: false, ModelValidationResults: { } validation }:
            {
                foreach (var validationResult in validation)
                {
                    logger.LogError("Failed to import allowed child content type {ImportedObjectDescription} {Members}: {Error}", importedObjectDescription, string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                }
                break;
            }
            default:
                break;
        }
    }

    private async Task MigratePageScopes()
    {
        var ksScopes = modelFacade.SelectAll<ICmsDocumentTypeScope>();
        var ksSitesById = modelFacade.SelectAll<ICmsSite>().ToDictionary(x => x.SiteID);
        var ksScopesWithoutUnmigratedSites = ksScopes.Where(x => x.ScopeSiteID is null || !toolConfiguration.EntityConfigurations.GetEntityConfiguration<CmsSiteK13>().ExcludeCodeNames.Contains(ksSitesById[x.ScopeSiteID.Value].SiteName, StringComparison.OrdinalIgnoreCase));
        var ksScopeClasses = modelFacade.SelectAll<ICmsDocumentTypeScopeClass>();
        var ksClassesById = modelFacade.SelectAll<ICmsClass>().ToDictionary(x => x.ClassID);

        foreach (var ksScope in ksScopesWithoutUnmigratedSites)
        {
            if (ksScope.ScopeSiteID is null)
            {
                logger.LogError("Scope GUID='{ScopeGUID}' not migrated. ScopeSiteID is null", ksScope.ScopeGUID);
                continue;
            }
            var ksSite = ksSitesById.GetValueOrDefault(ksScope.ScopeSiteID!.Value);
            if (ksSite is null)
            {
                logger.LogError("Scope GUID='{ScopeGUID}' not migrated. Scope site of ID='{ScopeSiteID}' not found in source instance", ksScope.ScopeGUID, ksScope.ScopeSiteID);
                continue;
            }
            var kxWebsiteChannel = WebsiteChannelInfo.Provider.Get(ksSite.SiteGUID);
            if (kxWebsiteChannel is null)
            {
                logger.LogError("Scope GUID='{ScopeGUID}' not migrated. Website channel of GUID='{WebsiteChannelGUID}', matching the source scope site, not found", ksScope.ScopeGUID, ksSite.SiteGUID);
                continue;
            }

            Guid? kxWebPageItemGuid;
            if (ksScope.ScopePath.Equals("/", StringComparison.InvariantCultureIgnoreCase))
            {
                kxWebPageItemGuid = null;
            }
            else
            {
                kxWebPageItemGuid = WebPageItemInfo.Provider.Get()
                    .And().WhereEquals(nameof(WebPageItemInfo.WebPageItemWebsiteChannelID), kxWebsiteChannel.WebsiteChannelID)
                    .WhereEquals(nameof(WebPageItemInfo.WebPageItemTreePath), ksScope.ScopePath)
                    .Columns(nameof(WebPageItemInfo.WebPageItemGUID)).FirstOrDefault()?.WebPageItemGUID;
                if (kxWebPageItemGuid is null)
                {
                    logger.LogWarning("Cannot migrate scope GUID='{ScopeGUID}'. No WebPageItem of tree path '{ScopePath}' in website channel {WebsiteChannelGUID}", ksScope.ScopeGUID, ksScope.ScopePath, kxWebsiteChannel.WebsiteChannelGUID);
                    continue;
                }
            }

            var scopeImportResult = await importer.ImportAsync(new WebPageScopeModel
            {
                WebPageScopeGUID = ksScope.ScopeGUID,
                WebPageScopeIncludeChildren = ksScope.ScopeIncludeChildren,
                WebPageScopeWebsiteChannelGuid = kxWebsiteChannel.WebsiteChannelGUID,
                WebPageScopeWebPageItemGuid = kxWebPageItemGuid
            });
            LogImportResult<WebPageScopeModel>(scopeImportResult, x => $"GUID='{x.WebPageScopeGUID}'");

            if (scopeImportResult is { Success: true, Imported: WebPageScopeInfo importedScope })
            {
                foreach (var ksScopeClass in ksScopeClasses.Where(x => x.ScopeID == ksScope.ScopeID))
                {
                    var ksClass = ksClassesById.GetValueOrDefault(ksScopeClass.ClassID);
                    if (ksClass is null)
                    {
                        logger.LogError("Allowed class ID={ClassID} of scope GUID='{ScopeGUID}' could not be migrated. Class with this ID not found in source instance", ksScopeClass.ClassID, ksScope.ScopeGUID);
                        continue;
                    }
                    if (new[] { "cms.root", "cms.folder" }.Any(x => x.Equals(ksClass.ClassName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        logger.LogInformation("Skipping page scope class CMS.Root/CMS.Folder");
                        continue;
                    }
                    var kxClass = kxpClassFacade.GetClass(ksClass.ClassGUID);
                    if (kxClass is null)
                    {
                        logger.LogError("Allowed class GUID={ClassGUID} of scope GUID='{ScopeGUID}' could not be migrated. Class with this GUID not found in target instance", ksClass!.ClassGUID, ksScope.ScopeGUID);
                        continue;
                    }

                    var scopeClassImportResult = await importer.ImportAsync(new WebPageScopeContentTypeModel
                    {
                        WebPageScopeContentTypeWebPageScopeGuid = importedScope.WebPageScopeGUID,
                        WebPageScopeContentTypeContentTypeGuid = kxClass.ClassGUID
                    });
                    LogImportResult<WebPageScopeContentTypeInfo>(scopeClassImportResult, x => $"ScopeGUID='{importedScope.WebPageScopeGUID}', ContentTypeGUID='{kxClass.ClassGUID}'");
                }
            }
        }
    }
}
