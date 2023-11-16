namespace Migration.Toolkit.Core.HandlersNG;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.Websites.Internal;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Providers;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;
using Newtonsoft.Json;

// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    private readonly ILogger<MigratePagesCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    // private readonly IEntityMapper<CmsTreeMapperSource, TreeNode> _nodeMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly IProtocol _protocol;
    private readonly KxpPageFacade _pageFacade;
    private readonly IImporter _importer;
    private readonly IUmtMapper<CmsTreeMapperSource> _mapper;

    // private readonly KxpSiteFacade _siteFacade;
    private readonly ContentItemNameProvider _contentItemNameProvider;

    public MigratePagesCommandHandler(
        ILogger<MigratePagesCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        IProtocol protocol,
        KxpPageFacade pageFacade,
        IImporter importer,
        IUmtMapper<CmsTreeMapperSource> mapper
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        // _nodeMapper = nodeMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _protocol = protocol;
        _pageFacade = pageFacade;
        _importer = importer;
        _mapper = mapper;
        // _siteFacade = siteFacade;
        _contentItemNameProvider = new ContentItemNameProvider(new Providers.ContentItemNameValidator());

    }

    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
    private record PageUrlPathKey(int PageUrlPathSiteId, string PageUrlPathUrlPathHash, string PageUrlPathCulture);

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
        // var siteMappings = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId);
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
                .Include(t => t.CmsDocuments)
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
                        originalCmsDocument.DocumentGuid = Guid.NewGuid();
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

                int? mappedParentNodeId;
                if (nodeClassClassName == CLASS_CMS_ROOT)
                {
                    // mappedParentNodeId = MapTargetRootNodeId(channelInfo, targetCultureCode);
                    //
                    // if (mappedParentNodeId == null)
                    // {
                    //     _logger.LogError("Unable to locate target instance page root node");
                    //     _protocol.Append(HandbookReferences
                    //         .MissingRequiredDependency<KXP.Models.CmsTree>(nameof(TreeNode.NodeParentID), mappedParentNodeId)
                    //         .NeedsManualAction()
                    //         .WithData(new
                    //         {
                    //             CLASS_CMS_ROOT,
                    //             targetSiteId,
                    //             sourceCultureCode,
                    //             targetCultureCode,
                    //             SourceCmsDocumentId = kx13CmsDocument.DocumentId,
                    //             SourceDocumentGuid = kx13CmsDocument.DocumentGuid,
                    //             SourcePageName = kx13CmsDocument.DocumentName,
                    //             SourceParentNodeId = kx13CmsTree.NodeParentId,
                    //         }));
                    //     return new CommandFailureResult();
                    // }
                    // _primaryKeyMappingContext.SetMapping<CmsTree>(
                    //     r => r.NodeId,
                    //     kx13CmsTree.NodeId,
                    //     mappedParentNodeId.Value
                    // );
                    _logger.LogInformation("Root node skipped, V27 has no support for root nodes");
                    continue;
                }

                var migratedDocuments = kx13CmsTree.CmsDocuments.Aggregate(new List<KX13M.CmsDocument>(), (list, document) =>
                {
                    var isPublished = _pageFacade.IsPublished(new IsPublishedArguments(
                        document.DocumentCanBePublished, document.DocumentWorkflowStepId,
                        document.DocumentIsArchived, document.DocumentCheckedOutVersionHistoryId,
                        document.DocumentPublishedVersionHistoryId, document.DocumentPublishFrom, document.DocumentPublishTo)
                    );

                    if (isPublished)
                    {
                        list.Add(document);
                    }
                    else
                    {
                        var handbookRef = HandbookReferences
                            .SourcePageIsNotPublished(document.DocumentGuid ?? Guid.Empty)
                            .WithIdentityPrint(document);

                        _logger.LogWarning("Source page {PageGuid} is not published => skipping ({HandbookRef})", document.DocumentGuid, handbookRef.ToString());
                        _protocol.Warning(handbookRef, document);
                    }

                    return list;
                });

                // mappedParentNodeId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsTree>(t => t.NodeId, kx13CmsTree.NodeParentId);
                // if (mappedParentNodeId == null)
                // {
                //     _protocol.Append(HandbookReferences
                //         .MissingRequiredDependency<KXP.Models.CmsTree>(nameof(TreeNode.NodeParentID), mappedParentNodeId)
                //         .NeedsManualAction()
                //         .WithData(new
                //         {
                //             SourceCmsDocumentId = kx13CmsDocument.DocumentId,
                //             SourceDocumentGuid = kx13CmsDocument.DocumentGuid,
                //             SourcePageName = kx13CmsDocument.DocumentName,
                //             SourceParentNodeId = kx13CmsTree.NodeParentId,
                //         }));
                //     _logger.LogWarning("Document with guid '{PageGuid}' has missing parent in target instance", kx13CmsDocument.DocumentGuid);
                //     continue;
                // }

                if (kx13CmsTreeOriginal is { NodeSkuid: not null })
                {
                    _logger.LogWarning("Page has SKU bound, SKU info will be discarded");
                    _protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                        .WithMessage("Page has SKU bound, SKU info will be discarded")
                        .WithIdentityPrint(kx13CmsTreeOriginal)
                        .WithData(new { NodeSKUID = kx13CmsTreeOriginal.NodeSkuid })
                    );
                }

                // var kxoTreeNodeParent = new DocumentQuery()
                //     .Where(nameof(TreeNode.NodeID), QueryOperator.Equals, mappedParentNodeId)
                //     .Culture(targetCultureCode)
                //     .Single();

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
                    foreach (var umtModel in results)
                    {
                        if (await _importer.ImportAsync(umtModel) is { Success: false } result)
                        {
                            if (result.Imported != null)
                            {

                            }
                            _logger.LogError("Failed to import: {Exception}, {ValidationResults}", result.Exception, JsonConvert.SerializeObject(result.ModelValidationResults));
                        }
                    }

                    // TODO tomas.krch: 2023-11-16 migrate url paths
                    // MigratePageUrlPaths(kx13CmsTree, kx13CmsDocument, treeNode);

                    // TODO tomas.krch: 2023-11-16 log success
                    // _protocol.Success(kx13CmsTree, treeNode, mapped);
                    // _logger.LogEntitySetAction(newInstance, treeNode);
                }

                catch (Exception ex)
                {
                    _protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<WebPageItemInfo>(ex)
                        .NeedsManualAction()
                        // .WithIdentityPrint(treeNode)
                    );
                    _logger.LogError("Failed to import content item: {Exception}", ex);
                    // TODO tomas.krch: 2023-11-16 log error
                    // _logger.LogEntitySetError(ex, false, results);
                }
            }
        }


        return new GenericCommandResult();
    }

    // private void MigratePageUrlPaths(CmsTree kx13CmsTree, CmsDocument kx13CmsDocument, TreeNode treeNode)
    // {
    //     using var kx13Context = _kx13ContextFactory.CreateDbContext();
    //     using var kxpContext = _kxpContextFactory.CreateDbContext();
    //
    //     var pageUrlPathsByHash =
    //         kxpContext.CmsPageUrlPaths.Include(x => x.PageUrlPathNode)
    //             .ToDictionary(x => new PageUrlPathKey(x.PageUrlPathSiteId, x.PageUrlPathUrlPathHash, x.PageUrlPathCulture));
    //
    //     var kx13PageUrlPaths = kx13Context
    //         .CmsPageUrlPaths.Where(p => p.PageUrlPathNodeId == kx13CmsTree.NodeId && p.PageUrlPathCulture == kx13CmsDocument.DocumentCulture);
    //
    //     foreach (var kx13PageUrlPath in kx13PageUrlPaths)
    //     {
    //         var pageUrlPathKey = new PageUrlPathKey(treeNode.NodeSiteID, kx13PageUrlPath.PageUrlPathUrlPathHash, treeNode.DocumentCulture);
    //
    //         var exists = PageUrlPathInfoProvider.ProviderObject.Get(kx13PageUrlPath.PageUrlPathGuid) != null;
    //         if (exists)
    //         {
    //             _logger.LogInformation("PageUrlPath skipped, already exists in target instance: {Info}", pageUrlPathKey);
    //             continue;
    //         }
    //
    //         var pageUrlPath = PageUrlPathInfo.New();
    //         pageUrlPath.PageUrlPathCulture = treeNode.DocumentCulture;
    //         pageUrlPath.PageUrlPathNodeID = treeNode.NodeID;
    //         pageUrlPath.PageUrlPathSiteID = treeNode.NodeSiteID;
    //         pageUrlPath.PageUrlPathGUID = kx13PageUrlPath.PageUrlPathGuid;
    //         pageUrlPath.PageUrlPathUrlPath = kx13PageUrlPath.PageUrlPathUrlPath;
    //         pageUrlPath.PageUrlPathUrlPathHash = kx13PageUrlPath.PageUrlPathUrlPathHash;
    //         pageUrlPath.PageUrlPathLastModified = kx13PageUrlPath.PageUrlPathLastModified;
    //         if (pageUrlPathsByHash.ContainsKey(pageUrlPathKey))
    //         {
    //             _logger.LogWarning("PageUrlPath skipped, already exists in target instance: {Info}", pageUrlPathKey);
    //             _protocol.Append(HandbookReferences
    //                 .DataAlreadyExistsInTargetInstance
    //                 .WithMessage($"PathUrlPathHash is already present, possibly old PageUrlPaths are still present?")
    //                 .WithIdentityPrint(kx13PageUrlPath)
    //             );
    //             continue;
    //         }
    //
    //         try
    //         {
    //             PageUrlPathInfoProvider.ProviderObject.Set(pageUrlPath);
    //             _logger.LogEntitySetAction(true, pageUrlPath);
    //         }
    //         // TODO tk: 2022-07-14 verify if check is needed when kentico api is used
    //         /*Violation in unique index or Violation in unique constraint */
    //         catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
    //         {
    //             _logger.LogEntitySetError(sqlException, true, pageUrlPath);
    //             _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, pageUrlPath)
    //                 .WithData(new { pageUrlPath.PageUrlPathUrlPathHash, pageUrlPath.PageUrlPathUrlPath })
    //                 .WithMessage($"Failed to migrate page, page url conflicts with other. PageNodeGuid: {kx13CmsTree.NodeGuid}. Needs manual migration.")
    //             );
    //         }
    //     }
    // }
}