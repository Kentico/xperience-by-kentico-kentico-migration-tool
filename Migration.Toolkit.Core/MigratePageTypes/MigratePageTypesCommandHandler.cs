﻿using System.Diagnostics;
using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXO.Api;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigratePageTypes;

public class MigratePageTypesCommandHandler: IRequestHandler<MigratePageTypesCommand, MigratePageTypesResult>
{
    private readonly ILogger<MigratePageTypesCommandHandler> _logger;
    private readonly IEntityMapper<CmsClass, KXO.Models.CmsClass> _mapper;
    private readonly IEntityMapper<CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KxoClassFacade _kxoClassFacade;
    private readonly IMigrationProtocol _migrationProtocol;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    public MigratePageTypesCommandHandler(
        ILogger<MigratePageTypesCommandHandler> logger,
        IEntityMapper<KX13.Models.CmsClass, KXO.Models.CmsClass> mapper,
        IEntityMapper<KX13.Models.CmsClass, DataClassInfo> dataClassMapper,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxoClassFacade kxoClassFacade,
        IMigrationProtocol migrationProtocol,
        ToolkitConfiguration toolkitConfiguration
        )
    {
        _logger = logger;
        _mapper = mapper;
        _dataClassMapper = dataClassMapper;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _kxoClassFacade = kxoClassFacade;
        _migrationProtocol = migrationProtocol;
        _toolkitConfiguration = toolkitConfiguration;
    }

    public async Task<MigratePageTypesResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogInformation("Selecting source CMS_Classes");
        var cmsClassesDocumentTypes = kx13Context.CmsClasses.Where(x => x.ClassIsDocumentType).OrderBy(x => x.ClassId).AsEnumerable();
        _logger.LogInformation("Selected source CMS_Classes, took: {took}", sw.Elapsed);
        
        foreach (var kx13Class in cmsClassesDocumentTypes)
        {
            _migrationProtocol.FetchedSource(kx13Class);
            
            if (kx13Class.ClassName == "CMS.Root")
            {
                _migrationProtocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, kx13Class);
                _logger.LogWarning($"CmsClass: {kx13Class.ClassName} was skipped => CMS.Root cannot be migrated.");
                continue;
            }

            if (kx13Class.ClassConnectionString != _toolkitConfiguration.SourceConnectionString && string.IsNullOrWhiteSpace(kx13Class.ClassConnectionString))
            {
                _migrationProtocol.Warning(HandbookReferences.CmsClassClassConnectionStringIsDifferent, kx13Class);
                _logger.LogWarning($"CmsClass: {kx13Class.ClassName} => ClassConnectionString is different from source connection string needs attention!");
            }
            
            var kxoDataClass = _kxoClassFacade.GetClass(kx13Class.ClassGuid);
            _migrationProtocol.FetchedTarget(kxoDataClass);
            
            SaveUsingKxoApi(kx13Class, kxoDataClass);
            // await SaveUsingEntityFramework(cancellationToken, kx13CmsClassesDocumentType, kxoCmsClass, kxoContext);
        }
        
        _logger.LogInformation("Finished: {took}", sw.Elapsed);

        return new MigratePageTypesResult();
    }

    private void SaveUsingKxoApi(CmsClass kx13Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(kx13Class, kxoDataClass);
        _migrationProtocol.MappedTarget(mapped);
        mapped.LogResult(_logger);

        switch (mapped)
        {
            case ModelMappingSuccess<DataClassInfo>(var dataClassInfo, var newInstance):
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                _kxoClassFacade.SetClass(dataClassInfo);

                _migrationProtocol.Success(kx13Class, dataClassInfo, mapped);

                _logger.LogInformation(newInstance
                    ? $"CmsClass: {dataClassInfo.ClassName} was inserted."
                    : $"CmsClass: {dataClassInfo.ClassName} was updated.");

                _primaryKeyMappingContext.SetMapping<KX13.Models.CmsClass>(
                    r => r.ClassId,
                    kx13Class.ClassId,
                    dataClassInfo.ClassID
                );

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mapped));
        }
    }

    private async Task SaveUsingEntityFramework(CancellationToken cancellationToken, CmsClass kx13CmsClassesDocumentType, KXO.Models.CmsClass? kxoCmsClass, KxoContext kxoContext)
    {
        var mapped = _mapper.Map(kx13CmsClassesDocumentType, kxoCmsClass);
        _migrationProtocol.MappedTarget(mapped);
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

                _migrationProtocol.Success(kx13CmsClassesDocumentType, kxoCmsClass, mapped);

                _logger.LogInformation(newInstance
                    ? $"CmsClass: {cmsClass.ClassName} was inserted."
                    : $"CmsClass: {cmsClass.ClassName} was updated.");

                // kxoContext.SaveChangesWithIdentityInsert<KXO.Models.CmsClass>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mapped));
        }
    }
}