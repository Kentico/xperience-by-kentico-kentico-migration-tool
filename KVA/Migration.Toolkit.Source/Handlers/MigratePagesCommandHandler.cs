using System.Collections.Concurrent;
using System.Diagnostics;

using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.DataEngine.Query;
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Toolkit.Source.Handlers;
// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler(
    ILogger<MigratePagesCommandHandler> logger,
    ToolkitConfiguration toolkitConfiguration,
    IProtocol protocol,
    IImporter importer,
    IUmtMapper<CmsTreeMapperSource> mapper,
    ModelFacade modelFacade,
    DeferredPathService deferredPathService,
    SpoiledGuidContext spoiledGuidContext
)
    : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string ClassCmsRoot = "CMS.Root";

    private readonly ContentItemNameProvider contentItemNameProvider = new(new ContentItemNameValidator());

    private readonly ConcurrentDictionary<string, ContentLanguageInfo> languages = new(StringComparer.InvariantCultureIgnoreCase);

    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        var classEntityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        var cultureCodeToLanguageGuid = modelFacade.SelectAll<ICmsCulture>()
            .ToDictionary(c => c.CultureCode, c => c.CultureGUID, StringComparer.InvariantCultureIgnoreCase);

        var allTargetLanguages = ContentLanguageInfoProvider.ProviderObject.Get().ToList();

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

            var ksTrees = modelFacade.Select<ICmsTree>(
                "NodeSiteId = @siteId",
                "NodeLevel, NodeOrder",
                new SqlParameter("siteId", ksSite.SiteID)
            );

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

                    for (int i = 0; i < linkedNodeDocuments.Count; i++)
                    {
                        var linkedDocument = linkedNodeDocuments[i];
                        var fixedDocumentGuid = GuidHelper.CreateDocumentGuid($"{linkedDocument.DocumentID}|{ksNode.NodeID}|{ksNode.NodeSiteID}"); //Guid.NewGuid();
                        var patchedNodeGuid = spoiledGuidContext.EnsureNodeGuid(ksNode.NodeGUID, ksNode.NodeSiteID, ksNode.NodeID);
                        if (ContentItemInfo.Provider.Get(patchedNodeGuid)?.ContentItemID is { } contentItemId)
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
                string nodeClassClassName = ksNodeClass.ClassName;
                if (classEntityConfiguration.ExcludeCodeNames.Contains(nodeClassClassName, StringComparer.InvariantCultureIgnoreCase))
                {
                    protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(nodeClassClassName, "PageType"), ksNode);
                    logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", nodeClassClassName);
                    continue;
                }

                if (nodeClassClassName == ClassCmsRoot)
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
                        .WithData(new { ksTreeOriginal.NodeSKUID })
                    );
                }

                string safeNodeName = await contentItemNameProvider.Get(ksNode.NodeName);
                var ksNodeParent = modelFacade.SelectById<ICmsTree>(ksNode.NodeParentID);
                var nodeParentGuid = ksNodeParent?.NodeAliasPath == "/" || ksNodeParent == null
                    ? (Guid?)null
                    : spoiledGuidContext.EnsureNodeGuid(ksNodeParent);

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
                    var commonDataInfos = new List<ContentItemCommonDataInfo>();
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

                            default:
                                break;
                        }
                    }

                    AsserVersionStatusRule(commonDataInfos);

                    if (webPageItemInfo != null && targetClass is { ClassWebPageHasUrl: true })
                    {
                        var existingDocumentLanguages = new HashSet<Guid>();
                        foreach (var migratedDocument in migratedDocuments)
                        {
                            var languageGuid = cultureCodeToLanguageGuid[migratedDocument.DocumentCulture];

                            await MigratePageUrlPaths(
                                webPageItemInfo.WebPageItemGUID,
                                webPageItemInfo.WebPageItemID, ksSite.SiteGUID,
                                languageGuid,
                                commonDataInfos,
                                migratedDocument,
                                ksNode,
                                migratedDocument.DocumentCulture
                            );

                            existingDocumentLanguages.Add(languageGuid);
                        }

                        var channelCulturesWithoutPage = allTargetLanguages.Where(x => !existingDocumentLanguages.Any(y => y == x.ContentLanguageGUID));

                        foreach (var culture in channelCulturesWithoutPage)
                        {
                            await MigratePageUrlPaths(
                                webPageItemInfo.WebPageItemGUID,
                                webPageItemInfo.WebPageItemID, ksSite.SiteGUID,
                                culture.ContentLanguageGUID,
                                commonDataInfos,
                                null,
                                ksNode,
                                culture.ContentLanguageName
                            );
                        }

                        MigrateFormerUrls(ksNode, webPageItemInfo);

                        var urls = WebPageUrlPathInfo.Provider.Get()
                            .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemInfo.WebPageItemID);

                        if (urls.Count < 1)
                        {
                            logger.LogWarning("No url for page {Page}", new { webPageItemInfo.WebPageItemName, webPageItemInfo.WebPageItemTreePath, webPageItemInfo.WebPageItemGUID });
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

        return new GenericCommandResult();
    }

    [Conditional("DEBUG")]
    private static void AsserVersionStatusRule(List<ContentItemCommonDataInfo> commonDataInfos)
    {
        foreach (var contentItemCommonDataInfos in commonDataInfos.GroupBy(x => x.ContentItemCommonDataContentLanguageID))
        {
            VersionStatus? versionStatus = null;
            bool onlyOneStatus = contentItemCommonDataInfos.Aggregate(true, (acc, i) =>
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
        List<ContentItemCommonDataInfo> contentItemCommonDataInfos, ICmsDocument? ksDocument, ICmsTree ksTree, string documentCulture)
    {
        var existingPaths = WebPageUrlPathInfo.Provider.Get()
                .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemId)
                .ToList();

        if (modelFacade.IsAvailable<ICmsPageUrlPath>())
        {
            var ksPaths = modelFacade.SelectWhere<CmsPageUrlPathK13>("PageUrlPathNodeId = @nodeId AND PageUrlPathCulture = @culture",
                new SqlParameter("nodeId", ksTree.NodeID),
                new SqlParameter("culture", documentCulture)
            ).ToList();

            var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);
            var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);

            if (ksDocument is null)
            {
                await CreateDefaultPageUrlAsync(webPageItemGuid, webSiteChannelGuid, languageGuid, ksTree, documentCulture, languageInfo, webSiteChannel, existingPaths);
            }

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
                            }
                        };

                        CheckPathAlreadyExists(existingPaths, webPageUrlPath, languageInfo, webSiteChannel);

                        var importResult = await importer.ImportAsync(webPageUrlPath);

                        LogImportResult(importResult);
                    }
                }
            }
            else
            {
                if (ksDocument is null)
                {
                    await CreateDefaultPageUrlAsync(webPageItemGuid, webSiteChannelGuid, languageGuid, ksTree, documentCulture, languageInfo, webSiteChannel, existingPaths);
                }
                foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
                {
                    logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id} - fallback",
                        contentItemCommonDataInfo.ContentItemCommonDataContentItemID, contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID,
                        contentItemCommonDataInfo.ContentItemCommonDataID);

                    var webPageUrlPath = new WebPageUrlPathModel
                    {
                        WebPageUrlPathGUID = contentItemCommonDataInfo.ContentItemCommonDataVersionStatus == VersionStatus.Draft
                            ? GuidHelper.CreateWebPageUrlPathGuid($"{ksDocument!.DocumentGUID}|{documentCulture}|{ksTree.NodeAliasPath}|DRAFT")
                            : GuidHelper.CreateWebPageUrlPathGuid($"{ksDocument!.DocumentGUID}|{ksTree.NodeAliasPath}"),
                        WebPageUrlPath = ksTree.NodeAliasPath, //ksPath.PageUrlPathUrlPath,
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
                            VersionStatus.Unpublished => false,
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    };

                    CheckPathAlreadyExists(existingPaths, webPageUrlPath, languageInfo, webSiteChannel);

                    var importResult = await importer.ImportAsync(webPageUrlPath);

                    LogImportResult(importResult);
                }
            }
        }
        else
        {
            var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);

            var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);

            if (ksDocument is null)
            {
                await CreateDefaultPageUrlAsync(webPageItemGuid, webSiteChannelGuid, languageGuid, ksTree, documentCulture, languageInfo, webSiteChannel, existingPaths);
            }

            foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
            {
                logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID,
                    contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                string urlPath = (ksDocument switch
                {
                    CmsDocumentK11 doc => doc.DocumentUrlPath,
                    CmsDocumentK12 doc => doc.DocumentUrlPath,
                    null => $"{languageInfo.ContentLanguageName}{ksTree.NodeAliasPath}",
                    _ => null
                }).NullIf(string.Empty) ?? ksTree.NodeAliasPath;

                var webPageUrlPath = new WebPageUrlPathModel
                {
                    WebPageUrlPathGUID = GuidHelper.CreateWebPageUrlPathGuid($"{urlPath}|{documentCulture}|{webSiteChannel.WebsiteChannelGUID}"),
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
                    }
                };

                var importResult = await importer.ImportAsync(webPageUrlPath);

                LogImportResult(importResult);
            }
        }
    }

    private async Task CreateDefaultPageUrlAsync(Guid webPageItemGuid, Guid webSiteChannelGuid, Guid languageGuid,
        ICmsTree ksTree, string documentCulture, ContentLanguageInfo languageInfo, WebsiteChannelInfo webSiteChannel, List<WebPageUrlPathInfo> existingPaths)
    {
        string urlPath = $"{languageInfo.ContentLanguageName}{ksTree.NodeAliasPath}";

        var webPageUrlPath = new WebPageUrlPathModel
        {
            WebPageUrlPathGUID = GuidHelper.CreateWebPageUrlPathGuid($"{urlPath}|{documentCulture}|{webSiteChannel.WebsiteChannelGUID}"),
            WebPageUrlPath = urlPath,
            WebPageUrlPathWebPageItemGuid = webPageItemGuid,
            WebPageUrlPathWebsiteChannelGuid = webSiteChannelGuid,
            WebPageUrlPathContentLanguageGuid = languageGuid,
            WebPageUrlPathIsLatest = true,
            WebPageUrlPathIsDraft = false
        };

        CheckPathAlreadyExists(existingPaths, webPageUrlPath, languageInfo, webSiteChannel);

        var importResult = await importer.ImportAsync(webPageUrlPath);

        LogDefaultCreationImportResult(importResult);
    }

    private void CheckPathAlreadyExists(List<WebPageUrlPathInfo> existingPaths, WebPageUrlPathModel webPageUrlPath,
        ContentLanguageInfo languageInfo,
        WebsiteChannelInfo webSiteChannel)
    {
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
    }

    private void LogImportResult(IImportResult importResult)
    {
        switch (importResult)
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

            default:
                break;
        }
    }

    private void LogDefaultCreationImportResult(IImportResult importResult)
    {
        switch (importResult)
        {
            case { Success: true, Imported: WebPageUrlPathInfo imported }:
            {
                logger.LogTrace("Page url default path created '{Path}' '{Guid}'", imported.WebPageUrlPath, imported.WebPageUrlPathGUID);
                break;
            }
            case { Success: false, Exception: { } exception }:
            {
                logger.LogError("Failed to create default page url path: {Error}", exception.ToString());
                break;
            }
            case { Success: false, ModelValidationResults: { } validation }:
            {
                foreach (var validationResult in validation)
                {
                    logger.LogError("Failed to create default url path {Members}: {Error}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                }

                break;
            }

            default:
                break;
        }
    }

    private void MigrateFormerUrls(ICmsTree ksNode, WebPageItemInfo targetPage)
    {
        if (modelFacade.IsAvailable<ICmsPageFormerUrlPath>())
        {
            var formerUrlPaths = modelFacade.SelectWhere<ICmsPageFormerUrlPath>(
                "PageFormerUrlPathSiteID = @siteId AND PageFormerUrlPathNodeID = @nodeId",
                new SqlParameter("siteId", ksNode.NodeSiteID),
                new SqlParameter("nodeId", ksNode.NodeID)
            );
            foreach (var cmsPageFormerUrlPath in formerUrlPaths)
            {
                logger.LogDebug("PageFormerUrlPath migration '{PageFormerUrlPath}' ", cmsPageFormerUrlPath);
                protocol.FetchedSource(cmsPageFormerUrlPath);

                switch (cmsPageFormerUrlPath)
                {
                    case CmsPageFormerUrlPathK11:
                    case CmsPageFormerUrlPathK12:
                    {
                        logger.LogError("Unexpected type '{Type}'", cmsPageFormerUrlPath.GetType().FullName);
                        break;
                    }
                    case CmsPageFormerUrlPathK13 pfup:
                    {
                        try
                        {
                            var languageInfo = languages.GetOrAdd(
                                pfup.PageFormerUrlPathCulture,
                                s => ContentLanguageInfoProvider.ProviderObject.Get().WhereEquals(nameof(ContentLanguageInfo.ContentLanguageName), s).SingleOrDefault() ?? throw new InvalidOperationException($"Missing content language '{s}'")
                            );

                            var ktPath = WebPageFormerUrlPathInfo.Provider.Get()
                                .WhereEquals(nameof(WebPageFormerUrlPathInfo.WebPageFormerUrlPathHash), GetWebPageUrlPathHashQueryExpression(pfup.PageFormerUrlPathUrlPath))
                                .WhereEquals(nameof(WebPageFormerUrlPathInfo.WebPageFormerUrlPathWebsiteChannelID), targetPage.WebPageItemWebsiteChannelID)
                                .WhereEquals(nameof(WebPageFormerUrlPathInfo.WebPageFormerUrlPathContentLanguageID), languageInfo.ContentLanguageID)
                                .SingleOrDefault();

                            if (ktPath != null)
                            {
                                protocol.FetchedTarget(ktPath);
                            }

                            var webPageFormerUrlPathInfo = ktPath ?? new WebPageFormerUrlPathInfo();
                            webPageFormerUrlPathInfo.WebPageFormerUrlPath = pfup.PageFormerUrlPathUrlPath;
                            webPageFormerUrlPathInfo.WebPageFormerUrlPathHash = modelFacade.HashPath(pfup.PageFormerUrlPathUrlPath);
                            webPageFormerUrlPathInfo.WebPageFormerUrlPathWebPageItemID = targetPage.WebPageItemID;
                            webPageFormerUrlPathInfo.WebPageFormerUrlPathWebsiteChannelID = targetPage.WebPageItemWebsiteChannelID;
                            webPageFormerUrlPathInfo.WebPageFormerUrlPathContentLanguageID = languageInfo.ContentLanguageID;
                            webPageFormerUrlPathInfo.WebPageFormerUrlPathLastModified = pfup.PageFormerUrlPathLastModified;

                            WebPageFormerUrlPathInfo.Provider.Set(webPageFormerUrlPathInfo);
                            logger.LogInformation("Former page url path imported '{Path}'", webPageFormerUrlPathInfo.WebPageFormerUrlPath);
                        }
                        catch (Exception ex)
                        {
                            protocol.Append(HandbookReferences
                                .ErrorCreatingTargetInstance<WebPageFormerUrlPathInfo>(ex)
                                .NeedsManualAction()
                                .WithIdentityPrint(pfup)
                            );
                            logger.LogError("Failed to import page former url path: {Exception}", ex);
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(cmsPageFormerUrlPath));
                }
            }
        }
        else
        {
            logger.LogDebug("CmsPageFormerUrlPath not supported in source instance");
        }
    }

    internal static QueryExpression GetWebPageUrlPathHashQueryExpression(string urlPath) => $"CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', LOWER(N'{SqlHelper.EscapeQuotes(urlPath)}')), 2)".AsExpression();

    // private async Task MigrateAlternativeUrls()
    // {
    //     if (modelFacade.IsAvailable<ICmsAlternativeUrl>())
    //     {
    //     }
    //     else
    //     {
    //     }
    // }

    #region Deffered patch

    private async Task ExecDeferredPageBuilderPatch()
    {
        logger.LogInformation("Executing TreePath patch");

        foreach ((var uniqueId, string className, int webSiteChannelId) in deferredPathService.GetWidgetsToPatch())
        {
            if (className == ContentItemCommonDataInfo.TYPEINFO.ObjectClassName)
            {
                var contentItemCommonDataInfo = await ContentItemCommonDataInfo.Provider.GetAsync(uniqueId);

                contentItemCommonDataInfo.ContentItemCommonDataPageBuilderWidgets = DeferredPatchPageBuilderWidgets(
                    contentItemCommonDataInfo.ContentItemCommonDataPageBuilderWidgets, webSiteChannelId, out bool anythingChangedW);
                contentItemCommonDataInfo.ContentItemCommonDataPageTemplateConfiguration = DeferredPatchPageTemplateConfiguration(
                    contentItemCommonDataInfo.ContentItemCommonDataPageTemplateConfiguration, webSiteChannelId, out bool anythingChangedC);

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
                    out bool anythingChangedW
                );
                pageTemplateConfigurationInfo.PageTemplateConfigurationTemplate = DeferredPatchPageTemplateConfiguration(
                    pageTemplateConfigurationInfo.PageTemplateConfigurationTemplate,
                    webSiteChannelId,
                    out bool anythingChangedC
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

        anythingChanged = false;
        return documentPageBuilderWidgets;
    }

    #endregion
}
