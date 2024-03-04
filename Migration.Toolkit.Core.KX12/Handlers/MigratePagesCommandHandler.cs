namespace Migration.Toolkit.Core.KX12.Handlers;

using System.Diagnostics;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Websites;
using CMS.Websites.Internal;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Mappers;
using Migration.Toolkit.Core.KX12.Providers;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Newtonsoft.Json;

// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler(ILogger<MigratePagesCommandHandler> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        IProtocol protocol,
        IImporter importer,
        IUmtMapper<CmsTreeMapperSource> mapper)
    : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    private readonly ContentItemNameProvider _contentItemNameProvider = new(new Providers.ContentItemNameValidator());

    private CmsTree? GetFullSourceCmsTree(int siteId, Guid nodeGuid)
    {
        using var kx12Context = kx12ContextFactory.CreateDbContext();

        return kx12Context.CmsTrees
            .Include(t => t.NodeParent)
            .Include(t => t.CmsDocuments)
            .Include(t => t.NodeClass)
            .Include(t => t.NodeLinkedNode)
            .Where(x => x.NodeSiteId == siteId && x.NodeGuid == nodeGuid)
            .OrderBy(t => t.NodeLevel)
            .ThenBy(t => t.NodeParentId)
            .ThenBy(t => t.NodeId)
            .AsNoTracking()
            .AsSplitQuery()
            .SingleOrDefault();
    }

    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        var classEntityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        await using var KX12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var cultureCodeToLanguageGuid = KX12Context.CmsCultures
            .ToDictionary(c => c.CultureCode, c => c.CultureGuid, StringComparer.InvariantCultureIgnoreCase);

        var sites = await KX12Context.CmsSites.ToListAsync(cancellationToken: cancellationToken);
        foreach (var k12Site in sites)
        {
            var channelInfo = ChannelInfoProvider.ProviderObject.Get(k12Site.SiteGuid);
            if (channelInfo == null)
            {
                logger.LogError("Target channel for site '{SiteName}' not exists!", k12Site.SiteName);
                continue;
            }

            logger.LogInformation("Migrating pages for site '{SourceSiteName}' to target channel '{TargetChannelName}' as content items", k12Site.SiteName, channelInfo.ChannelName);

            var k12CmsTrees = KX12Context.CmsTrees
                .Include(t => t.NodeParent)
                .Include(t => t.CmsDocuments).ThenInclude(d=>d.DocumentCheckedOutVersionHistory)
                .Include(t => t.NodeClass)
                //.Include(t => t.CmsPageUrlPaths) // TODO tomas.krch: 2023-12-14 KX12
                .Include(t => t.NodeLinkedNode)
                .Where(x => x.NodeSiteId == k12Site.SiteId)
                .OrderBy(t => t.NodeLevel)
                .ThenBy(t => t.NodeOrder)
                .ThenBy(t => t.NodeParentId)
                .ThenBy(t => t.NodeId)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution();

            foreach (var k12CmsTreeOriginal in k12CmsTrees)
            {
                logger.LogDebug("Page '{NodeAliasPath}' migration", k12CmsTreeOriginal.NodeAliasPath);

                protocol.FetchedSource(k12CmsTreeOriginal);

                var k12CmsTree = k12CmsTreeOriginal;
                if (k12CmsTree.NodeLinkedNode != null)
                {
                    if (k12CmsTree.NodeLinkedNode.NodeSiteId != k12CmsTree.NodeSiteId)
                    {
                        // skip & write to protocol
                        logger.LogWarning("Linked node with NodeGuid {NodeGuid} is linked from different site - unable to migrate", k12CmsTreeOriginal.NodeGuid);
                        protocol.Warning(HandbookReferences.CmsTreeTreeIsLinkFromDifferentSite, k12CmsTree);
                        continue;
                    }

                    // materialize linked node & write to protocol
                    var linkedNode = GetFullSourceCmsTree(k12CmsTree.NodeSiteId, k12CmsTree.NodeLinkedNode.NodeGuid);

                    Debug.Assert(k12CmsTree != null, nameof(k12CmsTree) + " != null");
                    Debug.Assert(linkedNode != null, nameof(linkedNode) + " != null");

                    k12CmsTree.CmsDocuments.Clear();


                    foreach (var originalCmsDocument in linkedNode.CmsDocuments)
                    {
                        var fixedDocumentGuid = Guid.NewGuid();
                        if (ContentItemInfo.Provider.Get(k12CmsTree.NodeGuid)?.ContentItemID is { } contentItemId)
                        {
                            if(cultureCodeToLanguageGuid.TryGetValue(originalCmsDocument.DocumentCulture, out var languageGuid) &&
                               ContentLanguageInfoProvider.ProviderObject.Get(languageGuid) is {} languageInfo)
                            {
                                if (ContentItemCommonDataInfo.Provider.Get()
                                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentItemID), contentItemId)
                                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataContentLanguageID), languageInfo.ContentLanguageID)
                                        .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataIsLatest), true)
                                        .FirstOrDefault() is { } contentItemCommonDataInfo)
                                {
                                    fixedDocumentGuid = contentItemCommonDataInfo.ContentItemCommonDataGUID;
                                    logger.LogTrace("Page '{NodeAliasPath}' is linked => ContentItemCommonDataGUID copy to DocumentGuid", k12CmsTree.NodeAliasPath);
                                }
                            }
                        }

                        originalCmsDocument.DocumentGuid = fixedDocumentGuid;
                        originalCmsDocument.DocumentId = 0;

                        k12CmsTree.CmsDocuments.Add(originalCmsDocument);
                        k12CmsTree.NodeLinkedNodeId = null;
                        k12CmsTree.NodeLinkedNodeSiteId = null;
                        logger.LogTrace("Linked node with NodeGuid {NodeGuid} was materialized", k12CmsTree.NodeGuid);
                    }
                }

                var nodeClassClassName = k12CmsTree.NodeClass.ClassName;
                if (classEntityConfiguration.ExcludeCodeNames.Contains(nodeClassClassName, StringComparer.InvariantCultureIgnoreCase))
                {
                    protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(nodeClassClassName, "PageType"), k12CmsTree);
                    logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", nodeClassClassName);
                    continue;
                }

                if (nodeClassClassName == CLASS_CMS_ROOT)
                {
                    logger.LogInformation("Root node skipped, V27 has no support for root nodes");
                    continue;
                }

                var migratedDocuments = k12CmsTree.CmsDocuments.ToList();

                Debug.Assert(migratedDocuments.Count > 0, "migratedDocuments.Count > 0");

                if (k12CmsTreeOriginal is { NodeSkuid: not null })
                {
                    logger.LogWarning("Page '{NodeAliasPath}' has SKU bound, SKU info will be discarded", k12CmsTreeOriginal.NodeAliasPath);
                    protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                        .WithMessage($"Page '{k12CmsTreeOriginal.NodeAliasPath}' has SKU bound, SKU info will be discarded")
                        .WithIdentityPrint(k12CmsTreeOriginal)
                        .WithData(new { NodeSKUID = k12CmsTreeOriginal.NodeSkuid })
                    );
                }

                var safeNodeName = await _contentItemNameProvider.Get(k12CmsTree.NodeName);
                var nodeParentGuid = k12CmsTree.NodeParent?.NodeAliasPath == "/" || k12CmsTree.NodeParent == null
                    ? (Guid?)null
                    : k12CmsTree.NodeParent?.NodeGuid;

                var targetClass = DataClassInfoProvider.ProviderObject.Get(k12CmsTree.NodeClass.ClassGuid);

                var results = mapper.Map(new CmsTreeMapperSource(
                    k12CmsTree,
                    safeNodeName,
                    k12Site.SiteGuid,
                    nodeParentGuid,
                    cultureCodeToLanguageGuid,
                    targetClass.ClassFormDefinition,
                    k12CmsTree.NodeClass.ClassFormDefinition,
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
                            await MigratePageUrlPaths(k12CmsTree, migratedDocument, webPageItemInfo.WebPageItemGUID, webPageItemInfo.WebPageItemID, k12Site.SiteGuid,
                                cultureCodeToLanguageGuid[migratedDocument.DocumentCulture],
                                commonDataInfos
                            );
                        }
                    }
                }

                catch (Exception ex)
                {
                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<WebPageItemInfo>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(k12CmsTree)
                    );
                    logger.LogError("Failed to import content item: {Exception}", ex);
                }
            }
        }


        return new GenericCommandResult();
    }

    [Conditional("DEBUG")]
    private static void AsserVersionStatusRule(List<ContentItemCommonDataInfo> commonDataInfos)
    {
        foreach (var contentItemCommonDataInfos in commonDataInfos.GroupBy(x=>x.ContentItemCommonDataContentLanguageID))
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

   private async Task MigratePageUrlPaths(CmsTree k12CmsTree, CmsDocument k11CmsDocument, Guid webPageItemGuid, int webPageItemId, Guid webSiteChannelGuid, Guid languageGuid,
        List<ContentItemCommonDataInfo> contentItemCommonDataInfos)
    {
        await using var kx11Context = await kx12ContextFactory.CreateDbContextAsync();

        var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);

        var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);
        foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x => x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
        {
            logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID,
                contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

            var urlPath = k11CmsDocument.DocumentUrlPath.NullIf(string.Empty) ?? k12CmsTree.NodeAliasPath;
            var webPageUrlPath = new WebPageUrlPathModel
            {
                WebPageUrlPathGUID = GuidHelper.CreateWebPageUrlPathGuid($"{urlPath}|{k11CmsDocument.DocumentCulture}|{webSiteChannel.WebsiteChannelGUID}"),
                WebPageUrlPath = urlPath,
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