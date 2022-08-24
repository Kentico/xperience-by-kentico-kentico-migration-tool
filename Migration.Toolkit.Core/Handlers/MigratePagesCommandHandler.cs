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
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

// ReSharper disable once UnusedType.Global
public class MigratePagesCommandHandler : IRequestHandler<MigratePagesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";
    
    private readonly ILogger<MigratePagesCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IEntityMapper<CmsTreeMapperSource, TreeNode> _nodeMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;
    private readonly KxpPageFacade _pageFacade;
    private readonly KxpSiteFacade _siteFacade;

    public MigratePagesCommandHandler(
        ILogger<MigratePagesCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IEntityMapper<CmsTreeMapperSource, TreeNode> nodeMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        KxpPageFacade pageFacade,
        KxpSiteFacade siteFacade
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _kxpContextFactory = kxpContextFactory;
        _nodeMapper = nodeMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _pageFacade = pageFacade;
        _siteFacade = siteFacade;
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
            .AsSplitQuery()
            .SingleOrDefault();
    }

    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        var sourceCultureCode = request.CultureCode;

        var siteMappings = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId);
        var classEntityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        foreach (var (sourceSiteId, targetSiteId) in siteMappings)
        {
            _logger.LogInformation("Migrating pages for site {SourceSiteId} to target site {TargetSiteId}", sourceSiteId, targetSiteId);
            
            var (targetSiteInfo, targetCultureCode) = _siteFacade.GetSiteInfo(targetSiteId);
            
            await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

            var kx13CmsTrees = kx13Context.CmsTrees
                .Include(t => t.NodeParent)
                .Include(t => t.CmsDocuments.Where(x => x.DocumentCulture == sourceCultureCode))
                .Include(t => t.NodeClass)
                .Include(t => t.CmsPageUrlPaths)
                .Include(t => t.NodeLinkedNode)
                .Where(x => x.NodeSiteId == sourceSiteId)
                .OrderBy(t => t.NodeLevel)
                .ThenBy(t => t.NodeParentId)
                .ThenBy(t => t.NodeId)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution();
            
            foreach (var kx13CmsTreeOriginal in kx13CmsTrees)
            {
                _protocol.FetchedSource(kx13CmsTreeOriginal);
                
                var kx13CmsTree = kx13CmsTreeOriginal;
                var migrationOfLinkedNode = false;
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
                    var linkedNode = GetFullSourceCmsTree(kx13CmsTree.NodeSiteId, sourceCultureCode, kx13CmsTree.NodeLinkedNode.NodeGuid);

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
                    _protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(kx13CmsTree.NodeClass.ClassName, "PageType"), kx13CmsTree);
                    _logger.LogWarning("Page: page of class {ClassName} was skipped => it is explicitly excluded in configuration", kx13CmsTree.NodeClass.ClassName);
                    continue;    
                }
                
                int? mappedParentNodeId;
                if (kx13CmsTree.NodeClass.ClassName == CLASS_CMS_ROOT)
                {
                    mappedParentNodeId = MapTargetRootNodeId(targetSiteId, targetCultureCode);
                    
                    if (mappedParentNodeId == null)
                    {
                        _logger.LogError("Unable to locate target instance page root node");
                        _protocol.Append(HandbookReferences
                            .MissingRequiredDependency<KXP.Models.CmsTree>(nameof(TreeNode.NodeParentID), mappedParentNodeId)
                            .NeedsManualAction()
                            .WithData(new
                            {
                                CLASS_CMS_ROOT,
                                targetSiteId,
                                sourceCultureCode,
                                targetCultureCode,
                                SourceCmsDocumentId = kx13CmsDocument.DocumentId,
                                SourceDocumentGuid = kx13CmsDocument.DocumentGuid,
                                SourcePageName = kx13CmsDocument.DocumentName,
                                SourceParentNodeId = kx13CmsTree.NodeParentId,
                            }));
                        return new CommandFailureResult();
                    }

                    _primaryKeyMappingContext.SetMapping<CmsTree>(
                        r => r.NodeId,
                        kx13CmsTree.NodeId,
                        mappedParentNodeId.Value
                    );

                    continue;
                }

                var isPublished = _pageFacade.IsPublished(new IsPublishedArguments(
                    kx13CmsDocument.DocumentCanBePublished, kx13CmsDocument.DocumentWorkflowStepId,
                    kx13CmsDocument.DocumentIsArchived, kx13CmsDocument.DocumentCheckedOutVersionHistoryId,
                    kx13CmsDocument.DocumentPublishedVersionHistoryId, kx13CmsDocument.DocumentPublishFrom, kx13CmsDocument.DocumentPublishTo)
                );

                if (!isPublished)
                {
                    _logger.LogWarning("Source page {PageGuid} is not published => skipping", kx13CmsDocument.DocumentGuid);
                    _protocol.Warning(HandbookReferences.SourcePageIsNotPublished(kx13CmsDocument.DocumentGuid ?? Guid.Empty), kx13CmsDocument);
                    continue;
                }
                
                mappedParentNodeId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsTree>(t => t.NodeId, kx13CmsTree.NodeParentId);
                if (mappedParentNodeId == null)
                {
                    _protocol.Append(HandbookReferences
                        .MissingRequiredDependency<KXP.Models.CmsTree>(nameof(TreeNode.NodeParentID), mappedParentNodeId)
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

                _protocol.FetchedTarget(kxoTreeNode);

                if (migrationOfLinkedNode && kxoTreeNode != null)
                {
                    _logger.LogWarning("Linked node is already materialized in target instance, if you want to migrate again delete it in target instance");
                    _protocol.Append(HandbookReferences.LinkedDataAlreadyMaterializedInTargetInstance.WithData(new { kx13CmsTree.NodeGuid, kx13CmsTree.NodeAliasPath }));
                    continue;
                }

                var kxoTreeNodeParent = new DocumentQuery()
                    .Where(nameof(TreeNode.NodeID), QueryOperator.Equals, mappedParentNodeId)
                    .Culture(targetCultureCode)
                    .Single();
                
                var source = new CmsTreeMapperSource(kx13CmsTree, sourceCultureCode, targetCultureCode);
                var mapped = _nodeMapper.Map(source, kxoTreeNode);
                _protocol.MappedTarget(mapped);

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
                            treeNode.Insert(kxoTreeNodeParent, true);
                        }
                        else
                        {
                            var vh = VersionManager.GetInstance(treeProvider);
                            vh.RemoveWorkflow(treeNode);
                            treeNode.Update(true);
                        }

                        MigratePageUrlPaths(kx13CmsTree, kx13CmsDocument, treeNode);

                        treeNode.Publish();

                        _protocol.Success(kx13CmsTree, treeNode, mapped);
                        _logger.LogEntitySetAction(newInstance, treeNode);
                        
                        _primaryKeyMappingContext.SetMapping<CmsTree>(
                            r => r.NodeId,
                            kx13CmsTree.NodeId,
                            treeNode.NodeID
                        );
                    }
                    
                    catch (Exception ex)
                    {
                        _protocol.Append(HandbookReferences
                            .ErrorCreatingTargetInstance<TreeNode>(ex)
                            .NeedsManualAction()
                            .WithIdentityPrint(treeNode)
                        );
                        _logger.LogEntitySetError(ex, newInstance, treeNode);
                    }
                }
            }    
        }
        

        return new GenericCommandResult();
    }

    private int? MapTargetRootNodeId(int targetSiteId, string cultureCode)
    {
        try
        {
            return new DocumentQuery(CLASS_CMS_ROOT)
                .OnSite(new SiteInfoIdentifier(targetSiteId))
                .Culture(cultureCode)
                .SingleOrDefault()
                ?.NodeID;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to locate target instance CMS.Root node");
            return null;
        }
    }

    private void MigratePageUrlPaths(CmsTree kx13CmsTree, CmsDocument kx13CmsDocument, TreeNode treeNode)
    {
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        using var kxpContext = _kxpContextFactory.CreateDbContext();
        
        var pageUrlPathsByHash =
            kxpContext.CmsPageUrlPaths.Include(x => x.PageUrlPathNode)
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
                _protocol.Append(HandbookReferences
                    .DataAlreadyExistsInTargetInstance
                    .WithMessage($"PathUrlPathHash is already present, possibly old PageUrlPaths are still present?")
                    .WithIdentityPrint(kx13PageUrlPath)
                );
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
                _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, pageUrlPath)
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