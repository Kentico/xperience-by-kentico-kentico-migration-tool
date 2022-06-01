﻿using System.Collections.Immutable;
using System.Data;
using System.Xml.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.Core.Services.BulkCopy;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigratePages;

public class MigratePagesCommandHandler : IRequestHandler<MigratePagesCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigratePagesCommandHandler> _logger;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<KX13.Models.CmsTree, KXO.Models.CmsTree> _treeMapper;
    private readonly IEntityMapper<KX13.Models.CmsAcl, KXO.Models.CmsAcl> _aclMapper;
    private readonly IEntityMapper<KX13.Models.CmsPageUrlPath, KXO.Models.CmsPageUrlPath> _pageUrlPathMapper;
    private readonly BulkDataCopyService _bulkDataCopyService;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IMigrationProtocol _migrationProtocol;

    private KxoContext _kxoContext;

    public MigratePagesCommandHandler(
        ILogger<MigratePagesCommandHandler> logger,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        IEntityMapper<KX13.Models.CmsTree, KXO.Models.CmsTree> treeMapper,
        IEntityMapper<KX13.Models.CmsAcl, KXO.Models.CmsAcl> aclMapper,
        IEntityMapper<KX13.Models.CmsPageUrlPath, KXO.Models.CmsPageUrlPath> pageUrlPathMapper,
        BulkDataCopyService bulkDataCopyService,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IMigrationProtocol migrationProtocol
    )
    {
        _logger = logger;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _treeMapper = treeMapper;
        _aclMapper = aclMapper;
        _pageUrlPathMapper = pageUrlPathMapper;
        _bulkDataCopyService = bulkDataCopyService;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _migrationProtocol = migrationProtocol;
        _kxoContext = kxoContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigratePagesCommand request, CancellationToken cancellationToken)
    {
        var cultureCode = request.CultureCode;

        var explicitSiteIdMapping = _toolkitConfiguration.RequireSiteIdExplicitMapping<KX13.Models.CmsSite>(s => s.SiteId).Keys.ToList();
        
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogTrace("Selecting existing documents");
        // assuming coupled data were migrated too in previous attempts
        var alreadyExistingDocuments = _kxoContext.CmsDocuments
            .Include(d => d.DocumentNode)
            .ThenInclude(t => t.NodeClass)
            .Select(x => new { x.DocumentForeignKeyValue, x.DocumentNode.NodeClass.ClassId, x.DocumentNode.NodeClass.ClassGuid })
            .ToLookup(k => k.ClassGuid, v => v.DocumentForeignKeyValue);
        

        _logger.LogTrace("Selecting classes to migrate");
        var classesToMigrate = kx13Context.CmsClasses
            .Where(c => c.ClassIsDocumentType && c.ClassIsCoupledClass)
            .Select(x => new {x.ClassXmlSchema, x.ClassTableName, x.ClassId, x.ClassGuid})
            ;

        XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
        XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
        var coupledDataToMigrate = classesToMigrate.AsEnumerable().Select(x =>
        {
            var xDoc = XDocument.Parse(x.ClassXmlSchema);
            var autoIncrementColumns = xDoc.Descendants(nsSchema + "element").Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                .Select(x => x.Attribute("name").Value).ToImmutableHashSet();


            var result = (x.ClassTableName, x.ClassGuid, autoIncrementColumns);
            _logger.LogTrace("Class '{classGuild}' Resolved as: {result}", x.ClassGuid, result);
            
            return result;
        });

        // check if data is present in target tables
        var anyDataPresent = false;
        foreach (var (tableName, classGuid, autoIncrementColumns) in coupledDataToMigrate)
        {
            if (_bulkDataCopyService.CheckIfDataExistsInTargetTable(tableName))
            {
                _logger.LogError("Data exists in target coupled data table '{tableName}' - cannot migrate.", tableName);
                anyDataPresent = true;
            }
        }

        if (anyDataPresent)
        {
            // TODO tk: 2022-06-01 command fatal
            _logger.LogError("Command failed.");
            return new CommandFailureResult();
        }

        foreach (var (tableName, classGuid, autoIncrementColumns) in coupledDataToMigrate)
        {
            var lookup = alreadyExistingDocuments[classGuid];
            var bulkCopyRequest = new BulkCopyRequest(
                tableName, s => !autoIncrementColumns.Contains(s), reader => lookup.Contains(reader.GetInt32(autoIncrementColumns.Single())), 1500
            );
            if (_bulkDataCopyService.CheckIfDataExistsInTargetTable(tableName))
            {
                _logger.LogError("Data exists in target coupled data table '{tableName}' - cannot migrate.", tableName);
                
                continue;
            }
            
            _logger.LogTrace("Bulk data copy request: {request}", bulkCopyRequest);
            _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
        }
        
        await RequireMigratedCmsAcls(kx13Context, explicitSiteIdMapping, cancellationToken);
        
        var kx13CmsTrees = kx13Context.CmsTrees
                .Include(t => t.CmsDocuments.Where(x => x.DocumentCulture == cultureCode))
                .Include(t => t.NodeClass)
                .Where(x => explicitSiteIdMapping.Contains(x.NodeSiteId))
                .OrderBy(t => t.NodeLevel)
                .ThenBy(t => t.NodeParentId)
                .ThenBy(t => t.NodeId)
            // TODO tk: 2022-05-18  .Where(x=>x.IsPublished)
            ;

        foreach (var kx13CmsTree in kx13CmsTrees)
        {
            _migrationProtocol.FetchedSource(kx13CmsTree);

            var kxoCmsTree = await _kxoContext.CmsTrees
                .Include(t => t.CmsDocuments.Where(x => x.DocumentCulture == cultureCode))
                .FirstOrDefaultAsync(x => x.NodeGuid == kx13CmsTree.NodeGuid, cancellationToken: cancellationToken);

            _migrationProtocol.FetchedTarget(kxoCmsTree);

            if (kx13CmsTree.NodeAliasPath == "/" && kx13CmsTree.NodeClass.ClassName == "CMS.Root")
            {
                if (kxoCmsTree == null)
                {
                    _migrationProtocol.Fatal(HandbookReferences.CmsTreeTreeRootIsMissing, kx13CmsTree);
                    throw new Exception("Target tree root node is missing");
                }

                _migrationProtocol.Warning(HandbookReferences.CmsTreeTreeRootSkip, kx13CmsTree);
                _primaryKeyMappingContext.SetMapping<KX13.Models.CmsTree>(
                    r => r.NodeId,
                    kx13CmsTree.NodeId,
                    kxoCmsTree.NodeId
                );
                continue;
            }

            var mapped = _treeMapper.Map(kx13CmsTree, kxoCmsTree);
            _migrationProtocol.MappedTarget(mapped);
            mapped.LogResult(_logger);

            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsTree>(var cmsTree, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsTree, nameof(cmsTree));
                    
                    if (newInstance)
                    {
                        _kxoContext.CmsTrees.Add(cmsTree);
                    }
                    else
                    {
                        _kxoContext.CmsTrees.Update(cmsTree);
                    }

                    try
                    {
                        await _kxoContext.SaveChangesAsync(cancellationToken);

                        // self reference satisfaction
                        if (kx13CmsTree.NodeOriginalNodeId == kx13CmsTree.NodeId)
                        {
                            cmsTree.NodeOriginalNodeId = cmsTree.NodeId;
                            await _kxoContext.SaveChangesAsync(cancellationToken);
                        }
                        
                        _migrationProtocol.Success(kx13CmsTree, cmsTree, mapped);
                        _logger.LogInformation(newInstance
                            ? $"CmsTree: {cmsTree.NodeName} with NodeGuid '{cmsTree.NodeGuid}' was inserted."
                            : $"CmsTree: {cmsTree.NodeName} with NodeGuid '{cmsTree.NodeGuid}' was updated.");
                    }
                    catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
                    {
                        throw;
                    }

                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsTree>(
                        r => r.NodeId,
                        kx13CmsTree.NodeId,
                        cmsTree.NodeId
                    );

                    foreach (var kx13CmsDocument in kx13CmsTree.CmsDocuments)
                    {
                        var kxoCmdDocument = cmsTree.CmsDocuments.FirstOrDefault(x => x.DocumentGuid == kx13CmsDocument.DocumentGuid);
                        if (kxoCmdDocument == null)
                        {
                            // TODO tk: 2022-05-18 report inconsistency
                            _logger.LogWarning("Inconsistency: new cmsDocument should be present, but it isn't. NodeGuid={nodeGuid}", cmsTree.NodeGuid);
                            continue;
                        }
                        
                        _primaryKeyMappingContext.SetMapping<KX13.Models.CmsDocument>(
                            r => r.DocumentId,
                            kx13CmsDocument.DocumentId,
                            kxoCmdDocument.DocumentId
                        );    
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
        
        // TODO tk: 2022-05-19 reorder method arguments
        await RequireMigratedCmsPageUrlPaths(cancellationToken, kx13Context, explicitSiteIdMapping);
        
        return new GenericCommandResult();
    }

    private async Task RequireMigratedCmsAcls(KX13Context kx13Context, List<int?> explicitSiteIdMapping, CancellationToken cancellationToken)
    {
        var kx13CmsAcls = kx13Context.CmsAcls
            .Where(x => explicitSiteIdMapping.Contains(x.AclsiteId));

        foreach (var kx13CmsAcl in kx13CmsAcls)
        {
            _migrationProtocol.FetchedSource(kx13CmsAcl);

            var kxoCmsAcl = await _kxoContext.CmsAcls
                .FirstOrDefaultAsync(x => x.Aclguid == kx13CmsAcl.Aclguid, cancellationToken: cancellationToken);

            _migrationProtocol.FetchedTarget(kxoCmsAcl);

            var mapped = _aclMapper.Map(kx13CmsAcl, kxoCmsAcl);
            _migrationProtocol.MappedTarget(mapped);
            mapped.LogResult(_logger);

            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsAcl>(var cmsAcl, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsAcl, nameof(cmsAcl));

                    if (newInstance)
                    {
                        _kxoContext.CmsAcls.Add(cmsAcl);
                    }
                    else
                    {
                        _kxoContext.CmsAcls.Update(cmsAcl);
                    }

                    try
                    {
                        await _kxoContext.SaveChangesAsync(cancellationToken);

                        _migrationProtocol.Success(kx13CmsAcl, cmsAcl, mapped);
                        _logger.LogInformation(newInstance
                            ? $"CmsAcl: {cmsAcl.Aclguid} was inserted."
                            : $"CmsAcl: {cmsAcl.Aclguid} was updated.");
                    }
                    catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
                    {
                        throw;
                    }

                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsAcl>(
                        r => r.Aclid,
                        kx13CmsAcl.Aclid,
                        cmsAcl.Aclid
                    );

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
    }

    // TODO tk: 2022-05-19 might be better to migrate after each cmsTree in case migration get interrupted
    private async Task RequireMigratedCmsPageUrlPaths(CancellationToken cancellationToken, KX13Context kx13Context, List<int?> explicitSiteIdMapping)
    {
        var kx13CmsPageUrlPaths = kx13Context.CmsPageUrlPaths
            .Where(x => explicitSiteIdMapping.Contains(x.PageUrlPathSiteId));

        foreach (var kx13CmsPageUrlPath in kx13CmsPageUrlPaths)
        {
            _migrationProtocol.FetchedSource(kx13CmsPageUrlPath);

            var kxoCmsPageUrlPaths = await _kxoContext.CmsPageUrlPaths
                .FirstOrDefaultAsync(x => x.PageUrlPathGuid == kx13CmsPageUrlPath.PageUrlPathGuid, cancellationToken: cancellationToken);

            _migrationProtocol.FetchedTarget(kxoCmsPageUrlPaths);

            var mapped = _pageUrlPathMapper.Map(kx13CmsPageUrlPath, kxoCmsPageUrlPaths);
            _migrationProtocol.MappedTarget(mapped);
            mapped.LogResult(_logger);

            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsPageUrlPath>(var cmsPageUrlPath, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsPageUrlPath, nameof(cmsPageUrlPath));

                    if (newInstance)
                    {
                        _kxoContext.CmsPageUrlPaths.Add(cmsPageUrlPath);
                    }
                    else
                    {
                        _kxoContext.CmsPageUrlPaths.Update(cmsPageUrlPath);
                    }

                    try
                    {
                        await _kxoContext.SaveChangesAsync(cancellationToken);

                        _migrationProtocol.Success(kx13CmsPageUrlPath, cmsPageUrlPath, mapped);
                        _logger.LogInformation(newInstance
                            ? $"CmsPageUrlPath: {cmsPageUrlPath.PageUrlPathUrlPath} with Guid {cmsPageUrlPath.PageUrlPathGuid} was inserted."
                            : $"CmsPageUrlPath: {cmsPageUrlPath.PageUrlPathUrlPath} with Guid {cmsPageUrlPath.PageUrlPathGuid} was updated.");
                    }
                    catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
                    {
                        throw;
                    }

                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsPageUrlPath>(
                        r => r.PageUrlPathId,
                        kx13CmsPageUrlPath.PageUrlPathId,
                        cmsPageUrlPath.PageUrlPathId
                    );

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
    }

    public void Dispose()
    {
        _kxoContext.Dispose();
    }
}