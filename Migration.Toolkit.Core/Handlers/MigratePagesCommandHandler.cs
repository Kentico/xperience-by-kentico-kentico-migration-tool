namespace Migration.Toolkit.Core.Handlers;

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
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.Providers;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Newtonsoft.Json;

// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    private readonly ILogger<MigratePagesCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly IProtocol _protocol;
    private readonly IImporter _importer;
    private readonly IUmtMapper<CmsTreeMapperSource> _mapper;
    private readonly ContentItemNameProvider _contentItemNameProvider;

    public MigratePagesCommandHandler(
        ILogger<MigratePagesCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        IProtocol protocol,
        IImporter importer,
        IUmtMapper<CmsTreeMapperSource> mapper
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _protocol = protocol;
        _importer = importer;
        _mapper = mapper;
        _contentItemNameProvider = new ContentItemNameProvider(new Providers.ContentItemNameValidator());

    }

    private CmsTree? GetFullSourceCmsTree(int siteId, Guid nodeGuid)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();

        return kx13Context.CmsTrees
            .Include(t => t.NodeParent)
            .Include(t => t.CmsDocuments)
            .Include(t => t.NodeClass)
            .Include(t => t.CmsPageUrlPaths)
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
        var classEntityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var cultureCodeToLanguageGuid = kx13Context.CmsCultures
            .ToDictionary(c => c.CultureCode, c => c.CultureGuid, StringComparer.InvariantCultureIgnoreCase);

        var sites = await kx13Context.CmsSites.ToListAsync(cancellationToken: cancellationToken);
        foreach (var kx13Site in sites)
        {
            var channelInfo = ChannelInfoProvider.ProviderObject.Get(kx13Site.SiteGuid);
            if (channelInfo == null)
            {
                _logger.LogError("Target channel for site '{SiteName}' not exists!", kx13Site.SiteName);
                continue;
            }

            _logger.LogInformation("Migrating pages for site '{SourceSiteName}' to target channel '{TargetChannelName}' as content items", kx13Site.SiteName, channelInfo.ChannelName);

            var kx13CmsTrees = kx13Context.CmsTrees
                .Include(t => t.NodeParent)
                .Include(t => t.CmsDocuments).ThenInclude(d=>d.DocumentCheckedOutVersionHistory)
                .Include(t => t.NodeClass)
                .Include(t => t.CmsPageUrlPaths)
                .Include(t => t.NodeLinkedNode)
                .Where(x => x.NodeSiteId == kx13Site.SiteId)
                .OrderBy(t => t.NodeLevel)
                .ThenBy(t => t.NodeOrder)
                .ThenBy(t => t.NodeParentId)
                .ThenBy(t => t.NodeId)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution();

            foreach (var kx13CmsTreeOriginal in kx13CmsTrees)
            {
                _logger.LogDebug("Page '{NodeAliasPath}' migration", kx13CmsTreeOriginal.NodeAliasPath);

                _protocol.FetchedSource(kx13CmsTreeOriginal);

                var kx13CmsTree = kx13CmsTreeOriginal;
                if (kx13CmsTree.NodeLinkedNode != null)
                {
                    if (kx13CmsTree.NodeLinkedNode.NodeSiteId != kx13CmsTree.NodeSiteId)
                    {
                        // skip & write to protocol
                        _logger.LogWarning("Linked node with NodeGuid {NodeGuid} is linked from different site - unable to migrate", kx13CmsTreeOriginal.NodeGuid);
                        _protocol.Warning(HandbookReferences.CmsTreeTreeIsLinkFromDifferentSite, kx13CmsTree);
                        continue;
                    }

                    // materialize linked node & write to protocol
                    var linkedNode = GetFullSourceCmsTree(kx13CmsTree.NodeSiteId, kx13CmsTree.NodeLinkedNode.NodeGuid);

                    Debug.Assert(kx13CmsTree != null, nameof(kx13CmsTree) + " != null");
                    Debug.Assert(linkedNode != null, nameof(linkedNode) + " != null");

                    kx13CmsTree.CmsDocuments.Clear();


                    foreach (var originalCmsDocument in linkedNode.CmsDocuments)
                    {
                        var fixedDocumentGuid = Guid.NewGuid();
                        if (ContentItemInfo.Provider.Get(kx13CmsTree.NodeGuid)?.ContentItemID is { } contentItemId)
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
                                    _logger.LogTrace("Page '{NodeAliasPath}' is linked => ContentItemCommonDataGUID copy to DocumentGuid", kx13CmsTree.NodeAliasPath);
                                }
                            }
                        }

                        originalCmsDocument.DocumentGuid = fixedDocumentGuid;
                        originalCmsDocument.DocumentId = 0;

                        kx13CmsTree.CmsDocuments.Add(originalCmsDocument);
                        kx13CmsTree.NodeLinkedNodeId = null;
                        kx13CmsTree.NodeLinkedNodeSiteId = null;
                        _logger.LogTrace("Linked node with NodeGuid {NodeGuid} was materialized", kx13CmsTree.NodeGuid);
                    }
                }

                var nodeClassClassName = kx13CmsTree.NodeClass.ClassName;
                if (classEntityConfiguration.ExcludeCodeNames.Contains(nodeClassClassName, StringComparer.InvariantCultureIgnoreCase))
                {
                    _protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(nodeClassClassName, "PageType"), kx13CmsTree);
                    _logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", nodeClassClassName);
                    continue;
                }

                if (nodeClassClassName == CLASS_CMS_ROOT)
                {
                    _logger.LogInformation("Root node skipped, V27 has no support for root nodes");
                    continue;
                }

                var migratedDocuments = kx13CmsTree.CmsDocuments.ToList();

                Debug.Assert(migratedDocuments.Count > 0, "migratedDocuments.Count > 0");

                if (kx13CmsTreeOriginal is { NodeSkuid: not null })
                {
                    _logger.LogWarning("Page has SKU bound, SKU info will be discarded");
                    _protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                        .WithMessage("Page has SKU bound, SKU info will be discarded")
                        .WithIdentityPrint(kx13CmsTreeOriginal)
                        .WithData(new { NodeSKUID = kx13CmsTreeOriginal.NodeSkuid })
                    );
                }

                var safeNodeName = await _contentItemNameProvider.Get(kx13CmsTree.NodeName);
                var nodeParentGuid = kx13CmsTree.NodeParent?.NodeAliasPath == "/" || kx13CmsTree.NodeParent == null
                    ? (Guid?)null
                    : kx13CmsTree.NodeParent?.NodeGuid;

                var targetClass = DataClassInfoProvider.ProviderObject.Get(kx13CmsTree.NodeClass.ClassGuid);

                var results = _mapper.Map(new CmsTreeMapperSource(
                    kx13CmsTree,
                    safeNodeName,
                    kx13Site.SiteGuid,
                    nodeParentGuid,
                    cultureCodeToLanguageGuid,
                    targetClass.ClassFormDefinition,
                    kx13CmsTree.NodeClass.ClassFormDefinition,
                    migratedDocuments
                ));
                try
                {
                    WebPageItemInfo? webPageItemInfo = null;
                    List<ContentItemCommonDataInfo> commonDataInfos = new List<ContentItemCommonDataInfo>();
                    foreach (var umtModel in results)
                    {
                        var result = await _importer.ImportAsync(umtModel);
                        if (result is { Success: false })
                        {
                            _logger.LogError("Failed to import: {Exception}, {ValidationResults}", result.Exception, JsonConvert.SerializeObject(result.ModelValidationResults));
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
                            await MigratePageUrlPaths(kx13CmsTree.NodeId, migratedDocument.DocumentCulture, webPageItemInfo.WebPageItemGUID, webPageItemInfo.WebPageItemID, kx13Site.SiteGuid,
                                cultureCodeToLanguageGuid[migratedDocument.DocumentCulture],
                                commonDataInfos
                            );
                        }
                    }
                    else
                    {
                        _logger.LogTrace("No webpage item produced for '{NodeAliasPath}'", kx13CmsTree.NodeAliasPath);
                    }
                }

                catch (Exception ex)
                {
                    _protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<WebPageItemInfo>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(kx13CmsTree)
                    );
                    _logger.LogError("Failed to import content item: {Exception}", ex);
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

    private async Task MigratePageUrlPaths(int nodeId, string documentCulture, Guid webPageItemGuid, int webPageItemId, Guid webSiteChannelGuid, Guid languageGuid, List<ContentItemCommonDataInfo> contentItemCommonDataInfos)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync();

        var kx13PageUrlPaths = kx13Context
            .CmsPageUrlPaths.Where(p => p.PageUrlPathNodeId == nodeId && p.PageUrlPathCulture == documentCulture);

        var existingPaths = WebPageUrlPathInfoProvider.ProviderObject.Get()
            .WhereEquals(nameof(WebPageUrlPathInfo.WebPageUrlPathWebPageItemID), webPageItemId)
            .ToList();

        var languageInfo = ContentLanguageInfoProvider.ProviderObject.Get(languageGuid);

        var webSiteChannel = WebsiteChannelInfoProvider.ProviderObject.Get(webSiteChannelGuid);

        foreach (var kx13PageUrlPath in kx13PageUrlPaths)
        {
            _logger.LogTrace("Page url path: C={Culture} S={Site} P={Path}", kx13PageUrlPath.PageUrlPathCulture, kx13PageUrlPath.PageUrlPathSiteId, kx13PageUrlPath.PageUrlPathUrlPath);

            foreach (var contentItemCommonDataInfo in contentItemCommonDataInfos.Where(x=>x.ContentItemCommonDataContentLanguageID == languageInfo.ContentLanguageID))
            {
                _logger.LogTrace("Page url path common data info: CIID={ContentItemId} CLID={Language} ID={Id}", contentItemCommonDataInfo.ContentItemCommonDataContentItemID, contentItemCommonDataInfo.ContentItemCommonDataContentLanguageID, contentItemCommonDataInfo.ContentItemCommonDataID);

                Debug.Assert(!string.IsNullOrWhiteSpace(kx13PageUrlPath.PageUrlPathUrlPath), "!string.IsNullOrWhiteSpace(kx13PageUrlPath.PageUrlPathUrlPath)");

                var webPageUrlPath = new WebPageUrlPathModel
                {
                    WebPageUrlPathGUID = Guid.NewGuid(),
                    WebPageUrlPath = kx13PageUrlPath.PageUrlPathUrlPath,
                    WebPageUrlPathHash = kx13PageUrlPath.PageUrlPathUrlPathHash,
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
                    _logger.LogTrace("Existing page url path found for '{Path}', fixing GUID to '{Guid}'", kx13PageUrlPath.PageUrlPathUrlPath, webPageUrlPath.WebPageUrlPathGUID);
                }

                switch (await _importer.ImportAsync(webPageUrlPath))
                {
                    case { Success: true, Imported: WebPageUrlPathInfo imported }:
                    {
                        _logger.LogInformation("Page url path imported '{Path}' '{Guid}'", imported.WebPageUrlPath, imported.WebPageUrlPathGUID);
                        break;
                    }
                    case { Success: false, Exception: {} exception }:
                    {
                        _logger.LogError("Failed to import page url path: {Error}", exception.ToString());
                        break;
                    }
                    case { Success: false, ModelValidationResults: { } validation }:
                    {
                        foreach (var validationResult in validation)
                        {
                            _logger.LogError("Failed to import page url path {Members}: {Error}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                        }
                        break;
                    }
                }
            }
        }
    }
}