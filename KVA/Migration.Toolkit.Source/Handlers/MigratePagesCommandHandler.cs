namespace Migration.Toolkit.Source.Handlers;

using System.Diagnostics;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Websites;
using CMS.Websites.Internal;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Mappers;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Providers;
using Migration.Toolkit.Source.Services;
using Migration.Toolkit.Source.Services.Model;
using Mono.Cecil.Cil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler(
    ILogger<MigratePagesCommandHandler> logger,
    ToolkitConfiguration toolkitConfiguration,
    IProtocol protocol,
    IImporter importer,
    IUmtMapper<CmsTreeMapperSource> mapper,
    ModelFacade modelFacade,
    DeferredPathService deferredPathService
)
    : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    private readonly ContentItemNameProvider _contentItemNameProvider = new(new ContentItemNameValidator());

    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        var classEntityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        var cultureCodeToLanguageGuid = modelFacade.SelectAll<ICmsCulture>()
            .ToDictionary(c => c.CultureCode, c => c.CultureGUID, StringComparer.InvariantCultureIgnoreCase);

        var sites = modelFacade.SelectAll<ICmsSite>();
        foreach (var ksSite in sites)
        {
            var channelInfo = ChannelInfoProvider.ProviderObject.Get(ksSite.SiteGUID);
            if (channelInfo == null)
            {
                logger.LogError("Target channel for site '{SiteName}' not exists!", ksSite.SiteName);
                continue;
            }

            logger.LogInformation("Migrating pages for site '{SourceSiteName}' to target channel '{TargetChannelName}' as content items", ksSite.SiteName, channelInfo.ChannelName);

            var ksTrees = modelFacade.SelectWhere<ICmsTree>("NodeSiteId = @siteId", new SqlParameter("siteId", ksSite.SiteID));

            foreach (var ksTreeOriginal in ksTrees)
            {
                logger.LogDebug("Page '{NodeAliasPath}' migration", ksTreeOriginal.NodeAliasPath);

                protocol.FetchedSource(ksTreeOriginal);

                var ksNode = ksTreeOriginal;
                var nodeLinkedNode = modelFacade.SelectById<ICmsTree>(ksNode.NodeLinkedNodeID);
                var migratedDocuments = modelFacade
                    .SelectWhere<ICmsDocument>("DocumentNodeID = @nodeId", new SqlParameter("nodeId", ksNode.NodeID))
                    .ToList();

                if (nodeLinkedNode != null)
                {
                    if (nodeLinkedNode.NodeSiteID != ksNode.NodeSiteID)
                    {
                        // skip & write to protocol
                        logger.LogWarning("Linked node with NodeGuid {NodeGuid} is linked from different site - unable to migrate", ksTreeOriginal.NodeGUID);
                        protocol.Warning(HandbookReferences.CmsTreeTreeIsLinkFromDifferentSite, ksNode);
                        continue;
                    }

                    // materialize linked node & write to protocol
                    var linkedNode = modelFacade.SelectWhere<ICmsTree>("NodeSiteID = @nodeSiteID AND NodeGUID = @nodeGuid",
                        new SqlParameter("nodeSiteID", ksNode.NodeSiteID),
                        new SqlParameter("nodeGuid", nodeLinkedNode.NodeGUID)
                    ).SingleOrDefault();

                    Debug.Assert(ksNode != null, nameof(ksNode) + " != null");
                    Debug.Assert(linkedNode != null, nameof(linkedNode) + " != null");

                    migratedDocuments.Clear();

                    var linkedNodeDocuments = modelFacade
                        .SelectWhere<ICmsDocument>("DocumentNodeID = @nodeId", new SqlParameter("nodeId", linkedNode.NodeID))
                        .ToList();

                    for (var i = 0; i < linkedNodeDocuments.Count; i++)
                    {
                        var linkedDocument = linkedNodeDocuments[i];
                        var fixedDocumentGuid = GuidHelper.CreateDocumentGuid($"{linkedDocument.DocumentID}|{ksNode.NodeID}|{ksNode.NodeSiteID}"); //Guid.NewGuid();
                        if (ContentItemInfo.Provider.Get(ksNode.NodeGUID)?.ContentItemID is { } contentItemId)
                        {
                            if (cultureCodeToLanguageGuid.TryGetValue(linkedDocument.DocumentCulture, out var languageGuid) &&
                                ContentLanguageInfoProvider.ProviderObject.Get(languageGuid) is { } languageInfo)
                            {
                                if (ContentItemCommonDataInfo.Provider.Get()
                                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), contentItemId)
                                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentLanguageID), languageInfo.ContentLanguageID)
                                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataIsLatest), true)
                                        .FirstOrDefault() is { } contentItemCommonDataInfo)
                                {
                                    fixedDocumentGuid = contentItemCommonDataInfo.ContentItemCommonDataGUID;
                                    logger.LogTrace("Page '{NodeAliasPath}' is linked => ContentItemCommonDataGUID copy to DocumentGuid", ksNode.NodeAliasPath);
                                }
                            }
                        }

                        linkedNodeDocuments[i] = linkedDocument switch
                        {
                            CmsDocumentK11 doc => doc with { DocumentGUID = fixedDocumentGuid, DocumentID = 0 },
                            CmsDocumentK12 doc => doc with { DocumentGUID = fixedDocumentGuid, DocumentID = 0 },
                            CmsDocumentK13 doc => doc with { DocumentGUID = fixedDocumentGuid, DocumentID = 0 },
                            _ => linkedNodeDocuments[i]
                        };

                        migratedDocuments.Add(linkedNodeDocuments[i]);
                        ksNode = ksNode switch
                        {
                            CmsTreeK11 node => node with { NodeLinkedNodeID = null, NodeLinkedNodeSiteID = null },
                            CmsTreeK12 node => node with { NodeLinkedNodeID = null, NodeLinkedNodeSiteID = null },
                            CmsTreeK13 node => node with { NodeLinkedNodeID = null, NodeLinkedNodeSiteID = null },
                            _ => ksNode
                        };
                        logger.LogTrace("Linked node with NodeGuid {NodeGuid} was materialized", ksNode.NodeGUID);
                    }
                }

                var ksNodeClass = modelFacade.SelectById<ICmsClass>(ksNode.NodeClassID) ?? throw new InvalidOperationException($"Node with missing class, node id '{ksNode.NodeID}'");
                var nodeClassClassName = ksNodeClass.ClassName;
                if (classEntityConfiguration.ExcludeCodeNames.Contains(nodeClassClassName, StringComparer.InvariantCultureIgnoreCase))
                {
                    protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(nodeClassClassName, "PageType"), ksNode);
                    logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", nodeClassClassName);
                    continue;
                }

                if (nodeClassClassName == CLASS_CMS_ROOT)
                {
                    logger.LogInformation("Root node skipped, V27 has no support for root nodes");
                    continue;
                }

                Debug.Assert(migratedDocuments.Count > 0, "migratedDocuments.Count > 0");

                if (ksTreeOriginal is { NodeSKUID: not null })
                {
                    logger.LogWarning("Page '{NodeAliasPath}' has SKU bound, SKU info will be discarded", ksTreeOriginal.NodeAliasPath);
                    protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                        .WithMessage($"Page '{ksTreeOriginal.NodeAliasPath}' has SKU bound, SKU info will be discarded")
                        .WithIdentityPrint(ksTreeOriginal)
                        .WithData(new { NodeSKUID = ksTreeOriginal.NodeSKUID })
                    );
                }

                var safeNodeName = await _contentItemNameProvider.Get(ksNode.NodeName);
                var ksNodeParent = modelFacade.SelectById<ICmsTree>(ksNode.NodeParentID);
                var nodeParentGuid = ksNodeParent?.NodeAliasPath == "/" || ksNodeParent == null
                    ? (Guid?)null
                    : ksNodeParent?.NodeGUID;

                var targetClass = DataClassInfoProvider.ProviderObject.Get(ksNodeClass.ClassGUID);

                var results = mapper.Map(new CmsTreeMapperSource(
                    ksNode,
                    safeNodeName,
                    ksSite.SiteGUID,
                    nodeParentGuid,
                    cultureCodeToLanguageGuid,
                    targetClass.ClassFormDefinition,
                    ksNodeClass.ClassFormDefinition,
                    migratedDocuments
                ));
                try
                {
                    WebPageItemInfo? webPageItemInfo = null;
                    List<ContentItemCommonDataInfo> commonDataInfos = new List<ContentItemCommonDataInfo>();
                    foreach (var umtModel in results)
                    {
                        var result = await importer.ImportAsync(umtModel);
                        if (result is { Success: false })
                        {
                            logger.LogError("Failed to import: {Exception}, {ValidationResults}", result.Exception, JsonConvert.SerializeObject(result.ModelValidationResults));
                        }

                        switch (result)
                        {
                            case { Success: true, Imported: ContentItemCommonDataInfo ccid }:
                            {
                                commonDataInfos.Add(ccid);
                                Debug.Assert(ccid.ContentItemCommonDataContentLanguageID != 0, "ccid.ContentItemCommonDataContentLanguageID != 0");
                                break;
                            }
                            case { Success: true, Imported: ContentItemLanguageMetadataInfo cclm }:
                            {
                                Debug.Assert(cclm.ContentItemLanguageMetadataContentLanguageID != 0, "ccid.ContentItemCommonDataContentLanguageID != 0");
                                break;
                            }
                            case { Success: true, Imported: WebPageItemInfo wp }:
                            {
                                webPageItemInfo = wp;
                                break;
                            }
                        }
                    }

                    AsserVersionStatusRule(commonDataInfos);

                    if (webPageItemInfo != null)
                    {
                        foreach (var migratedDocument in migratedDocuments)
                        {
                            await MigratePageUrlPaths(webPageItemInfo.WebPageItemGUID,
                                webPageItemInfo.WebPageItemID, ksSite.SiteGUID,
                                cultureCodeToLanguageGuid[migratedDocument.DocumentCulture], commonDataInfos,
                                migratedDocument, ksNode
                            );
                        }
                    }
                    else
                    {
                        logger.LogTrace("No webpage item produced for '{NodeAliasPath}'", ksNode.NodeAliasPath);
                    }
                }

                catch (Exception ex)
                {
                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<WebPageItemInfo>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(ksNode)
                    );
                    logger.LogError("Failed to import content item: {Exception}", ex);
                }
            }
        }

        await ExecDeferredPageBuilderPatch();

        // check urls
        foreach (var wpi in WebPageItemInfo.Provider.Get())
        {
            var urls = WebPageUrlPathInfo.Provider.Get()
                .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), wpi.WebPageItemID);
            if (urls.Count < 1)
            {
                logger.LogWarning("No url for page {Page}", new {wpi.WebPageItemName, wpi.WebPageItemTreePath, wpi.WebPageItemGUID});
            }
        }

        return new GenericCommandResult();
    }

    [Conditional("DEBUG")]
    private static void AsserVersionStatusRule(List<ContentItemCommonDataInfo> commonDataInfos)
    {
        foreach (var contentItemCommonDataInfos in commonDataInfos.GroupBy(x => x.ContentItemCommonDataContentLanguageID))
        {
            VersionStatus? versionStatus = null;
            var onlyOneStatus = contentItemCommonDataInfos.Aggregate(true, (acc, i) =>
            {
                try
                {
                    if (versionStatus.HasValue)
                    {
                        return versionStatus != i.ContentItemCommonDataVersionStatus;
                    }

                    return true;
                }
                finally
                {
                    versionStatus = i.ContentItemCommonDataVersionStatus;
                }
            });
            Debug.Assert(onlyOneStatus);
        }
    }

    private async Task MigratePageUrlPaths(Guid webPageItemGuid, int webPageItemId, Guid webSiteChannelGuid, Guid languageGuid,
        List<ContentItemCommonDataInfo> contentItemCommonDataInfos, ICmsDocument ksDocument, ICmsTree ksTree)
    {
        // TODO tomas.krch 2024-03-27: we will need to create even missing ones.. WebPageUrlPathInfo
        // migration of cmspageurlpath is not available => fallback needed

        if (modelFacade.IsAvailable<ICmsPageUrlPath>())
        {
            var ksPaths = modelFacade.SelectWhere<CmsPageUrlPathK13>("PageUrlPathNodeId = @nodeId AND PageUrlPathCulture = @culture",
                new SqlParameter("nodeId", ksTree.NodeID),
                new SqlParameter("culture", ksDocument.DocumentCulture)
            ).ToList();

            var existingPaths = WebPageUrlPathInfo.Provider.Get()
                .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemId)
                .ToList();

            var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);

            var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);
            if (ksPaths.Count > 0)
            {
                foreach (var ksPath in ksPaths)
                {
                    logger.LogTrace("Page url path: C={Culture} S={Site} P={Path}", ksPath.PageUrlPathCulture, ksPath.PageUrlPathSiteID, ksPath.PageUrlPathUrlPath);

                    foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
                    {
                        logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID,
                            contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                        Debug.Assert(!string.IsNullOrWhiteSpace(ksPath.PageUrlPathUrlPath), "!string.IsNullOrWhiteSpace(kx13PageUrlPath.PageUrlPathUrlPath)");

                        var webPageUrlPath = new WebPageUrlPathModel
                        {
                            WebPageUrlPathGUID = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus == VersionStatus.Draft
                                ? Guid.NewGuid()
                                : ksPath.PageUrlPathGUID,
                            WebPageUrlPath = ksPath.PageUrlPathUrlPath,
                            WebPageUrlPathHash = ksPath.PageUrlPathUrlPathHash,
                            WebPageUrlPathWebPageItemGuid = webPageItemGuid,
                            WebPageUrlPathWebsiteChannelGuid = webSiteChannelGuid,
                            WebPageUrlPathContentLanguageGuid = languageGuid,
                            WebPageUrlPathIsLatest = contentItemCommonDataInfo.ContentItemCommonDataIsLatest,
                            WebPageUrlPathIsDraft = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus switch
                            {
                                VersionStatus.InitialDraft => false,
                                VersionStatus.Draft => true,
                                VersionStatus.Published => false,
                                VersionStatus.Archived => false,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                        };

                        var ep = existingPaths.FirstOrDefault(ep =>
                            ep.WebPageUrlPath == webPageUrlPath.WebPageUrlPath &&
                            ep.WebPageUrlPathContentLanguageID == languageInfo.ContentLanguageID &&
                            ep.WebPageUrlPathIsDraft == webPageUrlPath.WebPageUrlPathIsDraft &&
                            ep.WebPageUrlPathIsLatest == webPageUrlPath.WebPageUrlPathIsLatest &&
                            ep.WebPageUrlPathWebsiteChannelID == webSiteChannel.WebsiteChannelID
                        );

                        if (ep != null)
                        {
                            webPageUrlPath.WebPageUrlPathGUID = ep.WebPageUrlPathGUID;
                            logger.LogTrace("Existing page url path found for '{Path}', fixing GUID to '{Guid}'", ksPath.PageUrlPathUrlPath, webPageUrlPath.WebPageUrlPathGUID);
                        }

                        switch (await importer.ImportAsync(webPageUrlPath))
                        {
                            case { Success: true, Imported: WebPageUrlPathInfo imported }:
                            {
                                logger.LogInformation("Page url path imported '{Path}' '{Guid}'", imported.WebPageUrlPath, imported.WebPageUrlPathGUID);
                                break;
                            }
                            case { Success: false, Exception: { } exception }:
                            {
                                logger.LogError("Failed to import page url path: {Error}", exception.ToString());
                                break;
                            }
                            case { Success: false, ModelValidationResults: { } validation }:
                            {
                                foreach (var validationResult in validation)
                                {
                                    logger.LogError("Failed to import page url path {Members}: {Error}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                                }

                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
                {
                    logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id} - fallback",
                        contentItemCommonDataInfo.ContentItemCommonDataContentItemID, contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                    var webPageUrlPath = new WebPageUrlPathModel
                    {
                        WebPageUrlPathGUID = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus == VersionStatus.Draft
                            ? Guid.NewGuid()
                            : GuidHelper.CreateWebPageUrlPathGuid($"{ksDocument.DocumentGUID}|{ksDocument.DocumentCulture}|{ksTree.NodeAliasPath}"),
                        WebPageUrlPath = ksTree.NodeAliasPath,//ksPath.PageUrlPathUrlPath,
                        // WebPageUrlPathHash = ksPath.PageUrlPathUrlPathHash,
                        WebPageUrlPathWebPageItemGuid = webPageItemGuid,
                        WebPageUrlPathWebsiteChannelGuid = webSiteChannelGuid,
                        WebPageUrlPathContentLanguageGuid = languageGuid,
                        WebPageUrlPathIsLatest = contentItemCommonDataInfo.ContentItemCommonDataIsLatest,
                        WebPageUrlPathIsDraft = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus switch
                        {
                            VersionStatus.InitialDraft => false,
                            VersionStatus.Draft => true,
                            VersionStatus.Published => false,
                            VersionStatus.Archived => false,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                    };

                    var ep = existingPaths.FirstOrDefault(ep =>
                        ep.WebPageUrlPath == webPageUrlPath.WebPageUrlPath &&
                        ep.WebPageUrlPathContentLanguageID == languageInfo.ContentLanguageID &&
                        ep.WebPageUrlPathIsDraft == webPageUrlPath.WebPageUrlPathIsDraft &&
                        ep.WebPageUrlPathIsLatest == webPageUrlPath.WebPageUrlPathIsLatest &&
                        ep.WebPageUrlPathWebsiteChannelID == webSiteChannel.WebsiteChannelID
                    );

                    if (ep != null)
                    {
                        webPageUrlPath.WebPageUrlPathGUID = ep.WebPageUrlPathGUID;
                        logger.LogTrace("Existing page url path found for '{Path}', fixing GUID to '{Guid}'", webPageUrlPath.WebPageUrlPath, webPageUrlPath.WebPageUrlPathGUID);
                    }

                    switch (await importer.ImportAsync(webPageUrlPath))
                    {
                        case { Success: true, Imported: WebPageUrlPathInfo imported }:
                        {
                            logger.LogInformation("Page url path imported '{Path}' '{Guid}'", imported.WebPageUrlPath, imported.WebPageUrlPathGUID);
                            break;
                        }
                        case { Success: false, Exception: { } exception }:
                        {
                            logger.LogError("Failed to import page url path: {Error}", exception.ToString());
                            break;
                        }
                        case { Success: false, ModelValidationResults: { } validation }:
                        {
                            foreach (var validationResult in validation)
                            {
                                logger.LogError("Failed to import page url path {Members}: {Error}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                            }

                            break;
                        }
                    }
                }
            }
        }
        else
        {
            var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);

            var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);
            foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
            {
                logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID,
                    contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                var urlPath = (ksDocument switch
                {
                    CmsDocumentK11 doc => doc.DocumentUrlPath,
                    CmsDocumentK12 doc => doc.DocumentUrlPath,
                    _ => null
                }).NullIf(string.Empty) ?? ksTree.NodeAliasPath;

                var webPageUrlPath = new WebPageUrlPathModel
                {
                    WebPageUrlPathGUID = GuidHelper.CreateWebPageUrlPathGuid($"{urlPath}|{ksDocument.DocumentCulture}|{webSiteChannel.WebsiteChannelGUID}"),
                    WebPageUrlPath = urlPath,
                    // WebPageUrlPathHash = kx13PageUrlPath.PageUrlPathUrlPathHash,
                    WebPageUrlPathWebPageItemGuid = webPageItemGuid,
                    WebPageUrlPathWebsiteChannelGuid = webSiteChannelGuid,
                    WebPageUrlPathContentLanguageGuid = languageGuid,
                    WebPageUrlPathIsLatest = contentItemCommonDataInfo.ContentItemCommonDataIsLatest,
                    WebPageUrlPathIsDraft = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus switch
                    {
                        VersionStatus.InitialDraft => false,
                        VersionStatus.Draft => true,
                        VersionStatus.Published => false,
                        VersionStatus.Archived => false,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                };

                switch (await importer.ImportAsync(webPageUrlPath))
                {
                    case { Success: true, Imported: WebPageUrlPathInfo imported }:
                    {
                        logger.LogInformation("Page url path imported '{Path}' '{Guid}'", imported.WebPageUrlPath, imported.WebPageUrlPathGUID);
                        break;
                    }
                    case { Success: false, Exception: { } exception }:
                    {
                        logger.LogError("Failed to import page url path: {Error}", exception.ToString());
                        break;
                    }
                    case { Success: false, ModelValidationResults: { } validation }:
                    {
                        foreach (var validationResult in validation)
                        {
                            logger.LogError("Failed to import page url path {Members}: {Error}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                        }
                        break;
                    }
                }
            }
        }
    }


    #region Deffered patch

    private async Task ExecDeferredPageBuilderPatch()
    {
        logger.LogInformation("Executing TreePath patch");

        foreach (var (uniqueId, className, webSiteChannelId) in deferredPathService.GetWidgetsToPatch())
        {
            if (className == ContentItemCommonDataInfo.TYPEINFO.ObjectClassName)
            {
                var contentItemCommonDataInfo = await ContentItemCommonDataInfo.Provider.GetAsync(uniqueId);

                contentItemCommonDataInfo.ContentItemCommonDataPageBuilderWidgets = DeferredPatchPageBuilderWidgets(
                    contentItemCommonDataInfo.ContentItemCommonDataPageBuilderWidgets, webSiteChannelId, out var anythingChangedW);
                contentItemCommonDataInfo.ContentItemCommonDataPageTemplateConfiguration = DeferredPatchPageTemplateConfiguration(
                    contentItemCommonDataInfo.ContentItemCommonDataPageTemplateConfiguration, webSiteChannelId, out var anythingChangedC);

                if (anythingChangedC || anythingChangedW)
                {
                    contentItemCommonDataInfo.Update();
                }
            }
            else if (className == PageTemplateConfigurationInfo.TYPEINFO.ObjectClassName)
            {
                var pageTemplateConfigurationInfo = await PageTemplateConfigurationInfo.Provider.GetAsync(uniqueId);
                pageTemplateConfigurationInfo.PageTemplateConfigurationWidgets = DeferredPatchPageBuilderWidgets(
                    pageTemplateConfigurationInfo.PageTemplateConfigurationWidgets,
                    webSiteChannelId,
                    out var anythingChangedW
                );
                pageTemplateConfigurationInfo.PageTemplateConfigurationTemplate = DeferredPatchPageTemplateConfiguration(
                    pageTemplateConfigurationInfo.PageTemplateConfigurationTemplate,
                    webSiteChannelId,
                    out var anythingChangedC
                );
                if (anythingChangedW || anythingChangedC)
                {
                    PageTemplateConfigurationInfo.Provider.Set(pageTemplateConfigurationInfo);
                }
            }
        }
    }

    private string DeferredPatchPageTemplateConfiguration(string documentPageTemplateConfiguration, int webSiteChannelId, out bool anythingChanged)
    {
        if (!string.IsNullOrWhiteSpace(documentPageTemplateConfiguration))
        {
            var configuration = JObject.Parse(documentPageTemplateConfiguration);
            PageBuilderWidgetsPatcher.DeferredPatchProperties(configuration, TreePathConvertor.GetSiteConverter(webSiteChannelId), out anythingChanged);
            return JsonConvert.SerializeObject(configuration);
        }

        anythingChanged = false;
        return documentPageTemplateConfiguration;
    }

    private string DeferredPatchPageBuilderWidgets(string documentPageBuilderWidgets, int webSiteChannelId, out bool anythingChanged)
    {
        if (!string.IsNullOrWhiteSpace(documentPageBuilderWidgets))
        {
            var patched = PageBuilderWidgetsPatcher.DeferredPatchConfiguration(
                JsonConvert.DeserializeObject<EditableAreasConfiguration>(documentPageBuilderWidgets),
                TreePathConvertor.GetSiteConverter(webSiteChannelId),
                out anythingChanged
            );
            return JsonConvert.SerializeObject(patched);
        }
        else
        {
            anythingChanged = false;
            return documentPageBuilderWidgets;
        }
    }

    #endregion
}