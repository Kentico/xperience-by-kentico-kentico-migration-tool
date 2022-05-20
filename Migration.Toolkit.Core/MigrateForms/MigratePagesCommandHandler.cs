using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigrateForms;

public class MigrateFormsCommandHandler : IRequestHandler<MigrateFormsCommand, GenericCommandResult>, IDisposable
{
    private readonly ILogger<MigrateFormsCommandHandler> _logger;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<CmsForm, KXO.Models.CmsForm> _cmsFormsMapper;
    // private readonly IEntityMapper<KX13.Models.CmsAcl, KXO.Models.CmsAcl> _aclMapper;
    // private readonly IEntityMapper<KX13.Models.CmsPageUrlPath, KXO.Models.CmsPageUrlPath> _pageUrlPathMapper;
    private readonly GlobalConfiguration _globalConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IMigrationProtocol _migrationProtocol;

    private KxoContext _kxoContext;

    public MigrateFormsCommandHandler(
        ILogger<MigrateFormsCommandHandler> logger,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        IEntityMapper<KX13.Models.CmsForm, KXO.Models.CmsForm> cmsFormsMapper,
        // IEntityMapper<KX13.Models.CmsAcl, KXO.Models.CmsAcl> aclMapper,
        // IEntityMapper<KX13.Models.CmsPageUrlPath, KXO.Models.CmsPageUrlPath> pageUrlPathMapper,
        GlobalConfiguration globalConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IMigrationProtocol migrationProtocol
    )
    {
        _logger = logger;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _cmsFormsMapper = cmsFormsMapper;
        // _aclMapper = aclMapper;
        // _pageUrlPathMapper = pageUrlPathMapper;
        _globalConfiguration = globalConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _migrationProtocol = migrationProtocol;
        _kxoContext = kxoContextFactory.CreateDbContext();
    }

    public async Task<GenericCommandResult> Handle(MigrateFormsCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        // TODO tk: 2022-05-19 reorder method arguments
        // await RequireMigratedCmsAcls(cancellationToken, kx13Context);

        var kx13CmsForms = kx13Context.CmsForms
                .Where(x => _globalConfiguration.SiteIdMapping.Keys.Contains(x.FormSiteId))
                .OrderBy(t => t.FormId)
            ;

        foreach (var kx13CmsForm in kx13CmsForms)
        {
            _migrationProtocol.FetchedSource(kx13CmsForm);

            var kxoCmsForm = await _kxoContext.CmsForms
                // .Include(t => t.CmsDocuments.Where(x => x.DocumentCulture == cultureCode))
                .FirstOrDefaultAsync(x => x.FormGuid == kx13CmsForm.FormGuid, cancellationToken: cancellationToken);

            _migrationProtocol.FetchedTarget(kxoCmsForm);

            // TODO tk: 2022-05-20 any reasons why form shouldn't be migrated?

            var mapped = _cmsFormsMapper.Map(kx13CmsForm, kxoCmsForm);
            _migrationProtocol.MappedTarget(mapped);
            mapped.LogResult(_logger);

            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsForm>(var cmsForm, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsForm, nameof(cmsForm));
                    
                    if (newInstance)
                    {
                        _kxoContext.CmsForms.Add(cmsForm);
                    }
                    else
                    {
                        _kxoContext.CmsForms.Update(cmsForm);
                    }

                    try
                    {
                        await _kxoContext.SaveChangesAsync(cancellationToken);
                        
                        _migrationProtocol.Success(kx13CmsForm, cmsForm, mapped);
                        _logger.LogInformation(newInstance
                            ? $"CmsForm: {cmsForm.FormName} with NodeGuid '{cmsForm.FormGuid}' was inserted."
                            : $"CmsForm: {cmsForm.FormName} with NodeGuid '{cmsForm.FormGuid}' was updated.");
                    }
                    catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
                    {
                        throw;
                    }
                    
                    // TODO tk: 2022-05-20 migrate coupled data here!

                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsForm>(
                        r => r.FormId,
                        kx13CmsForm.FormId,
                        cmsForm.FormId
                    );

                    // foreach (var kx13CmsDocument in kx13CmsForm.CmsDocuments)
                    // {
                    //     var kxoCmdDocument = cmsForm.CmsDocuments.FirstOrDefault(x => x.DocumentGuid == kx13CmsDocument.DocumentGuid);
                    //     if (kxoCmdDocument == null)
                    //     {
                    //         // TODO tk: 2022-05-18 report inconsistency
                    //         _logger.LogWarning("Inconsistency: new cmsDocument should be present, but it isn't. NodeGuid={nodeGuid}", cmsForm.NodeGuid);
                    //         continue;
                    //     }
                    //     
                    //     _primaryKeyMappingContext.SetMapping<KX13.Models.CmsDocument>(
                    //         r => r.DocumentId,
                    //         kx13CmsDocument.DocumentId,
                    //         kxoCmdDocument.DocumentId
                    //     );    
                    // }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }

        return new GenericCommandResult();
    }

    private async Task RequireMigratedCmsClass(CancellationToken cancellationToken, KX13Context kx13Context)
    {
        // TODO tk: 2022-05-20 class migration - shared component
        // var kx13CmsPageUrlPaths = kx13Context.CmsPageUrlPaths
        //     .Where(x => _globalConfiguration.SiteIdMapping.Keys.Contains(x.PageUrlPathSiteId));
        //
        // foreach (var kx13CmsPageUrlPath in kx13CmsPageUrlPaths)
        // {
        //     _migrationProtocol.FetchedSource(kx13CmsPageUrlPath);
        //
        //     var kxoCmsPageUrlPaths = await _kxoContext.CmsPageUrlPaths
        //         .FirstOrDefaultAsync(x => x.PageUrlPathGuid == kx13CmsPageUrlPath.PageUrlPathGuid, cancellationToken: cancellationToken);
        //
        //     _migrationProtocol.FetchedTarget(kxoCmsPageUrlPaths);
        //
        //     var mapped = _pageUrlPathMapper.Map(kx13CmsPageUrlPath, kxoCmsPageUrlPaths);
        //     _migrationProtocol.MappedTarget(mapped);
        //     mapped.LogResult(_logger);
        //
        //     switch (mapped)
        //     {
        //         case ModelMappingSuccess<KXO.Models.CmsPageUrlPath>(var cmsPageUrlPath, var newInstance):
        //             ArgumentNullException.ThrowIfNull(cmsPageUrlPath, nameof(cmsPageUrlPath));
        //
        //             if (newInstance)
        //             {
        //                 _kxoContext.CmsPageUrlPaths.Add(cmsPageUrlPath);
        //             }
        //             else
        //             {
        //                 _kxoContext.CmsPageUrlPaths.Update(cmsPageUrlPath);
        //             }
        //
        //             try
        //             {
        //                 await _kxoContext.SaveChangesAsync(cancellationToken);
        //
        //                 _migrationProtocol.Success(kx13CmsPageUrlPath, cmsPageUrlPath, mapped);
        //                 _logger.LogInformation(newInstance
        //                     ? $"CmsPageUrlPath: {cmsPageUrlPath.PageUrlPathGuid} was inserted."
        //                     : $"CmsPageUrlPath: {cmsPageUrlPath.PageUrlPathGuid} was updated.");
        //             }
        //             catch (Exception ex) // TODO tk: 2022-05-18 handle exceptions
        //             {
        //                 throw;
        //             }
        //
        //             _primaryKeyMappingContext.SetMapping<KX13.Models.CmsPageUrlPath>(
        //                 r => r.PageUrlPathId,
        //                 kx13CmsPageUrlPath.PageUrlPathId,
        //                 cmsPageUrlPath.PageUrlPathId
        //             );
        //
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(mapped));
        //     }
        // }
    }

    public void Dispose()
    {
        _kxoContext.Dispose();
    }
}