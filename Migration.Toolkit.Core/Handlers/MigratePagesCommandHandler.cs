namespace Migration.Toolkit.Core.Handlers;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CMS.DataEngine;
using CMS.DocumentEngine;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Api;
using Migration.Toolkit.KXO.Context;

// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private readonly ILogger<MigratePagesCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IEntityMapper<CmsTreeMapperSource, TreeNode> _nodeMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IMigrationProtocol _migrationProtocol;
    private readonly KxoPageFacade _pageFacade;

    public MigratePagesCommandHandler(
        ILogger<MigratePagesCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IDbContextFactory<KxoContext> kxoContextFactory,
        IEntityMapper<CmsTreeMapperSource, TreeNode> nodeMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IMigrationProtocol migrationProtocol,
        KxoPageFacade pageFacade
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _kxoContextFactory = kxoContextFactory;
        _nodeMapper = nodeMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _migrationProtocol = migrationProtocol;
        _pageFacade = pageFacade;
    }

    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local")]
    private record PageUrlPathKey(int PageUrlPathSiteId, string PageUrlPathUrlPathHash, string PageUrlPathCulture);
    
    private CmsTree? GetFullSourceCmsTree(int siteId, string cultureCode, Guid nodeGuid)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();

        return kx13Context.CmsTrees
            .Include(t => t.NodeParent)
            .Include(t => t.CmsDocuments.Where(x => x.DocumentCulture == cultureCode))
            .Include(t => t.NodeClass)
            .Include(t => t.CmsPageUrlPaths)
            .Include(t => t.NodeLinkedNode)
            .Where(x => x.NodeSiteId == siteId && x.NodeGuid == nodeGuid)
            .OrderBy(t => t.NodeLevel)
            .ThenBy(t => t.NodeParentId)
            .ThenBy(t => t.NodeId)
            .AsNoTracking()
            .SingleOrDefault();
    }

    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        var cultureCode = request.CultureCode;

        var siteMappings = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId);
        var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId).Keys.ToList();
        var classEntityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsTrees = kx13Context.CmsTrees
            .Include(t => t.NodeParent)
            .Include(t => t.CmsDocuments.Where(x => x.DocumentCulture == cultureCode))
            .Include(t => t.NodeClass)
            .Include(t => t.CmsPageUrlPaths)
            .Include(t => t.NodeLinkedNode)
            .Where(x => migratedSiteIds.Contains(x.NodeSiteId))
            .OrderBy(t => t.NodeLevel)
            .ThenBy(t => t.NodeParentId)
            .ThenBy(t => t.NodeId)
            .AsNoTrackingWithIdentityResolution();

        foreach (var kx13CmsTreeOriginal in kx13CmsTrees)
        {
            _migrationProtocol.FetchedSource(kx13CmsTreeOriginal);
            
            var kx13CmsTree = kx13CmsTreeOriginal;
            var migrationOfLinkedNode = false;
            if (kx13CmsTree.NodeLinkedNode != null)
            {
                if (kx13CmsTree.NodeLinkedNode.NodeSiteId != kx13CmsTree.NodeSiteId)
                {
                    // skip & write to protocol
                    _logger.LogWarning("Linked node with NodeGuid {NodeGuid} is linked from different site - unable to migrate", kx13CmsTreeOriginal.NodeGuid);
                    _migrationProtocol.Warning(HandbookReferences.CmsTreeTreeIsLinkFromDifferentSite, kx13CmsTree);
                    continue;
                }

                // materialize linked node & write to protocol
                var linkedNode = GetFullSourceCmsTree(kx13CmsTree.NodeSiteId, cultureCode, kx13CmsTree.NodeLinkedNode.NodeGuid);

                Debug.Assert(kx13CmsTree != null, nameof(kx13CmsTree) + " != null");
                Debug.Assert(linkedNode != null, nameof(linkedNode) + " != null");

                var originalCmsDocument = linkedNode.CmsDocuments.Single();
                originalCmsDocument.DocumentGuid = Guid.NewGuid();
                originalCmsDocument.DocumentId = 0;

                kx13CmsTree.CmsDocuments.Add(originalCmsDocument);
                kx13CmsTree.NodeLinkedNodeId = null;
                kx13CmsTree.NodeLinkedNodeSiteId = null;
                migrationOfLinkedNode = true;
                
                _logger.LogTrace("Linked node with NodeGuid {NodeGuid} was materialized", kx13CmsTree.NodeGuid);
            }
            
            var kx13CmsDocument = kx13CmsTree.CmsDocuments.SingleOrDefault();
            Debug.Assert(kx13CmsDocument != null, nameof(kx13CmsDocument) + " != null");
            
            if (classEntityConfiguration.ExcludeCodeNames.Contains(kx13CmsTree.NodeClass.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                _migrationProtocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(kx13CmsTree.NodeClass.ClassName, "PageType"), kx13CmsTree);
                _logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", kx13CmsTree.NodeClass.ClassName);
                continue;    
            }
            
            int? mappedParentNodeId;
            if (kx13CmsTree.NodeClass.ClassName == "CMS.Root")
            {
                if(!siteMappings.TryGetValue(kx13CmsTree.NodeSiteId, out var targetSiteId))
                {
                    continue;
                }

                // TODO tk: 2022-07-15 cache parent
                mappedParentNodeId = new DocumentQuery("CMS.Root")
                    .OnSite(new SiteInfoIdentifier(targetSiteId))
                    .Culture(cultureCode)
                    .SingleOrDefault()
                    ?.NodeID;

                if (mappedParentNodeId == null)
                {
                    _logger.LogError("Unable to find target instance page root node");
                    _migrationProtocol.Append(HandbookReferences
                        .MissingRequiredDependency<KXO.Models.CmsTree>(nameof(TreeNode.NodeParentID), mappedParentNodeId)
                        .NeedsManualAction()
                        .WithData(new
                        {
                            SourceCmsDocumentId = kx13CmsDocument.DocumentId,
                            SourceDocumentGuid = kx13CmsDocument.DocumentGuid,
                            SourcePageName = kx13CmsDocument.DocumentName,
                            SourceParentNodeId = kx13CmsTree.NodeParentId,
                        }));
                    return new CommandFailureResult();
                }

                _migrationProtocol.Warning(HandbookReferences.CmsTreeTreeRootSkip, kx13CmsTree);
                _primaryKeyMappingContext.SetMapping<CmsTree>(
                    r => r.NodeId,
                    kx13CmsTree.NodeId,
                    mappedParentNodeId.Value
                );

                continue;
            }

            // TODO tk: 2022-07-12 check if page class is supported / migrated
            
            var isPublished = _pageFacade.IsPublished(new IsPublishedArgument(
                kx13CmsDocument.DocumentCanBePublished, kx13CmsDocument.DocumentWorkflowStepId,
                kx13CmsDocument.DocumentIsArchived, kx13CmsDocument.DocumentCheckedOutVersionHistoryId,
                kx13CmsDocument.DocumentPublishedVersionHistoryId, kx13CmsDocument.DocumentPublishFrom, kx13CmsDocument.DocumentPublishTo)
            );

            if (!isPublished)
            {
                _logger.LogWarning("Source page {PageGuid} is not published => skipping", kx13CmsDocument.DocumentGuid);
                _migrationProtocol.Warning(HandbookReferences.SourcePageIsNotPublished(kx13CmsDocument.DocumentGuid ?? Guid.Empty), kx13CmsDocument);
                continue;
            }
            
            mappedParentNodeId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsTree>(t => t.NodeId, kx13CmsTree.NodeParentId);
            if (mappedParentNodeId == null)
            {
                _migrationProtocol.Append(HandbookReferences
                    .MissingRequiredDependency<KXO.Models.CmsTree>(nameof(TreeNode.NodeParentID), mappedParentNodeId)
                    .NeedsManualAction()
                    .WithData(new
                    {
                        SourceCmsDocumentId = kx13CmsDocument.DocumentId,
                        SourceDocumentGuid = kx13CmsDocument.DocumentGuid,
                        SourcePageName = kx13CmsDocument.DocumentName,
                        SourceParentNodeId = kx13CmsTree.NodeParentId,
                    }));
                _logger.LogWarning("Document with guid '{PageGuid}' has missing parent in target instance", kx13CmsDocument.DocumentGuid);
                continue;
            }

            var kxoTreeNode = new DocumentQuery(kx13CmsTree.NodeClass.ClassName)
                .WithGuid(kx13CmsTree.CmsDocuments.Single().DocumentGuid.GetValueOrDefault())
                .SingleOrDefault();

            _migrationProtocol.FetchedTarget(kxoTreeNode);

            if (migrationOfLinkedNode && kxoTreeNode != null)
            {
                _logger.LogWarning("Linked node is already materialized in target instance, if you want to migrate again delete it in target instance");
                _migrationProtocol.Append(HandbookReferences.LinkedDataAlreadyMaterializedInTargetInstance.WithData(new { kx13CmsTree.NodeGuid, kx13CmsTree.NodeAliasPath }));
                continue;
            }

            var kxoTreeNodeParent = new DocumentQuery()
                .Where("NodeID", QueryOperator.Equals, mappedParentNodeId)
                .Culture(cultureCode)
                .Single();
            
            var source = new CmsTreeMapperSource(kx13CmsTree, cultureCode);
            var mapped = _nodeMapper.Map(source, kxoTreeNode);
            _migrationProtocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (treeNode, newInstance) = result;

                ArgumentNullException.ThrowIfNull(treeNode, nameof(treeNode));

                try
                {
                    var treeProvider = new TreeProvider
                    {
                        UpdateUser = false,
                        UpdateTimeStamps = false,
                        LogEvents = false,
                        LogIntegration = false,
                        UpdatePaths = false,
                    };
                    treeNode.TreeProvider = treeProvider;
                    if (newInstance)
                    {
                        treeNode.Insert(kxoTreeNodeParent, false);
                    }
                    else
                    {
                        treeNode.Update(false);
                    }

                    MigratePageUrlPaths(kx13CmsTree, kx13CmsDocument, treeNode);

                    treeNode.Publish();

                    _migrationProtocol.Success(kx13CmsTree, treeNode, mapped);
                    _logger.LogEntitySetAction(newInstance, treeNode);
                    
                    _primaryKeyMappingContext.SetMapping<CmsTree>(
                        r => r.NodeId,
                        kx13CmsTree.NodeId,
                        treeNode.NodeID
                    );
                }
                
                catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
                {
                    _migrationProtocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<TreeNode>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(treeNode)
                    );
                    _logger.LogEntitySetError(ex, newInstance, treeNode);
                }
            }
        }

        return new GenericCommandResult();
    }

    private void MigratePageUrlPaths(CmsTree kx13CmsTree, CmsDocument kx13CmsDocument, TreeNode treeNode)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        using var kxoContext = _kxoContextFactory.CreateDbContext();
        
        var pageUrlPathsByHash =
            kxoContext.CmsPageUrlPaths.Include(x => x.PageUrlPathNode)
                .ToDictionary(x => new PageUrlPathKey(x.PageUrlPathSiteId, x.PageUrlPathUrlPathHash, x.PageUrlPathCulture));

        var kx13PageUrlPaths = kx13Context
            .CmsPageUrlPaths.Where(p => p.PageUrlPathNodeId == kx13CmsTree.NodeId && p.PageUrlPathCulture == kx13CmsDocument.DocumentCulture);

        foreach (var kx13PageUrlPath in kx13PageUrlPaths)
        {
            var pageUrlPathKey = new PageUrlPathKey(treeNode.NodeSiteID, kx13PageUrlPath.PageUrlPathUrlPathHash, treeNode.DocumentCulture);
            
            var exists = PageUrlPathInfoProvider.ProviderObject.Get(kx13PageUrlPath.PageUrlPathGuid) != null;
            if (exists)
            {
                _logger.LogWarning("PageUrlPath skipped, already exists in target instance: {Info}", pageUrlPathKey);
                // TODO tk: 2022-07-08 report
                continue;
            }
            
            var pageUrlPath = PageUrlPathInfo.New();
            pageUrlPath.PageUrlPathCulture = treeNode.DocumentCulture;
            pageUrlPath.PageUrlPathNodeID = treeNode.NodeID;
            pageUrlPath.PageUrlPathSiteID = treeNode.NodeSiteID;
            pageUrlPath.PageUrlPathGUID = kx13PageUrlPath.PageUrlPathGuid;
            pageUrlPath.PageUrlPathUrlPath = kx13PageUrlPath.PageUrlPathUrlPath;
            pageUrlPath.PageUrlPathUrlPathHash = kx13PageUrlPath.PageUrlPathUrlPathHash;
            pageUrlPath.PageUrlPathLastModified = kx13PageUrlPath.PageUrlPathLastModified;
            if (pageUrlPathsByHash.ContainsKey(pageUrlPathKey))
            {
                _logger.LogWarning("PageUrlPath skipped, already exists in target instance: {Info}", pageUrlPathKey);
                // TODO tk: 2022-07-08 report
                continue;
            }

            try
            {
                PageUrlPathInfoProvider.ProviderObject.Set(pageUrlPath);
                _logger.LogEntitySetAction(true, pageUrlPath);
            }
            // TODO tk: 2022-07-14 verify if check is needed when kentico api is used
            /*Violation in unique index or Violation in unique constraint */ 
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                _logger.LogEntitySetError(sqlException, true, pageUrlPath);
                _migrationProtocol.Append(HandbookReferences.DbConstraintBroken(sqlException, pageUrlPath)
                    .WithData(new
                    {
                        pageUrlPath.PageUrlPathUrlPathHash,
                        pageUrlPath.PageUrlPathUrlPath
                    })
                    .WithMessage($"Failed to migrate page, page url conflicts with other. PageNodeGuid: {kx13CmsTree.NodeGuid}. Needs manual migration.")
                );
            }

            
        }
    }
}