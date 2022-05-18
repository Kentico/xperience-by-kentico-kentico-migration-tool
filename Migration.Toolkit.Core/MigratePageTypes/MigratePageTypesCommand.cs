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

public class MigratePageTypesCommandHandler: IRequestHandler<MigratePageTypesCommand, MigratePageTypesResult>
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

    public async Task<MigratePageTypesResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        await using var kxoContext = await _kxoContextFactory.CreateDbContextAsync(cancellationToken); // TODO tk: 2022-05-18 context needs to be disposed/recreated after error
        
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
                    }
                    else
                    {
                        kxoContext.CmsClasses.Update(cmsClass);
                    }

                    await kxoContext.SaveChangesAsync(cancellationToken); // TODO tk: 2022-05-18 context needs to be disposed/recreated after error

                    _logger.LogInformation(newInstance
                        ? $"CmsClass: {cmsClass.ClassName} was inserted."
                        : $"CmsClass: {cmsClass.ClassName} was updated.");

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