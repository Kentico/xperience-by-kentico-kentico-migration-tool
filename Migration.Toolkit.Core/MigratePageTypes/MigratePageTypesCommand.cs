using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Configuration;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigratePageTypes;

public class MigratePageTypesCommandHandler: IRequestHandler<Commands.MigratePageTypesCommand, MigratePageTypesResult>
{
    private readonly ILogger<MigratePageTypesCommandHandler> _logger;
    private readonly IEntityMapper<CmsClass, KXO.Models.CmsClass> _mapper;
    private readonly EntityConfigurations _entityConfigurations;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;

    public MigratePageTypesCommandHandler(
        ILogger<MigratePageTypesCommandHandler> logger,
        IEntityMapper<KX13.Models.CmsClass, KXO.Models.CmsClass> mapper,
        EntityConfigurations entityConfigurations,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory)
    {
        _logger = logger;
        _mapper = mapper;
        _entityConfigurations = entityConfigurations;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
    }

    public async Task<MigratePageTypesResult> Handle(Commands.MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        
        using var kx13Context = _kx13ContextFactory.CreateDbContext();
        using var kxoContext = _kxoContextFactory.CreateDbContext();
        
        _logger.LogInformation("Selecting source CMS_Classes");
        var cmsClassesDocumentTypes = kx13Context.CmsClasses.Where(x => x.ClassIsDocumentType).AsEnumerable();
        _logger.LogInformation("Selected source CMS_Classes, took: {took}", sw.Elapsed);
        
        foreach (var cmsClassesDocumentType in cmsClassesDocumentTypes)
        {
            if (cmsClassesDocumentType.ClassName == "CMS.Root") continue;
            // TODO tk: 2022-05-16 verify target schema
            var target = kxoContext.CmsClasses.FirstOrDefault(c => c.ClassName == cmsClassesDocumentType.ClassName && c.ClassIsDocumentType == true);
            var mapped = _mapper.Map(cmsClassesDocumentType, target);

            mapped.LogResult(_logger);
            
            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsClass>(var cmsClass, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsClass, nameof(cmsClass));
                    
                    if (newInstance)
                    {
                        kxoContext.CmsClasses.Add(cmsClass);
                        _logger.LogInformation($"CmsClass: {cmsClass.ClassName} was inserted.");
                    }
                    else
                    {
                        kxoContext.CmsClasses.Update(cmsClass);
                        _logger.LogInformation($"CmsClass: {cmsClass.ClassName} was updated.");
                    }

                    kxoContext.SaveChanges();
                    // kxoContext.SaveChangesWithIdentityInsert<KXO.Models.CmsClass>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapped));
            }
        }
        
        _logger.LogInformation("Finished: {took}", sw.Elapsed);

        return new MigratePageTypesResult();
    }
}