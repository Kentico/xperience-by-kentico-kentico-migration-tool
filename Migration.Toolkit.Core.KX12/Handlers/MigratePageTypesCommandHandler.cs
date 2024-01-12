namespace Migration.Toolkit.Core.KX12.Handlers;

using CMS.ContentEngine;
using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.Core.KX12.Helpers;
using Migration.Toolkit.Core.KX12.Services;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Migration.Toolkit.KXP.Api;

public class MigratePageTypesCommandHandler : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    private readonly ILogger<MigratePageTypesCommandHandler> _logger;
    private readonly IEntityMapper<CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly IProtocol _protocol;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    private readonly KeyMappingContext _keyMappingContext;
    private readonly PageTemplateMigrator _pageTemplateMigrator;

    public MigratePageTypesCommandHandler(
        ILogger<MigratePageTypesCommandHandler> logger,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxpClassFacade kxpClassFacade,
        IProtocol protocol,
        ToolkitConfiguration toolkitConfiguration,
        KeyMappingContext keyMappingContext,
        PageTemplateMigrator pageTemplateMigrator
    )
    {
        _logger = logger;
        _dataClassMapper = dataClassMapper;
        _kx12ContextFactory = kx12ContextFactory;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _kxpClassFacade = kxpClassFacade;
        _protocol = protocol;
        _toolkitConfiguration = toolkitConfiguration;
        _keyMappingContext = keyMappingContext;
        _pageTemplateMigrator = pageTemplateMigrator;
    }

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        var cmsClassesDocumentTypes = kx12Context.CmsClasses
                .Include(c => c.Sites)
                .Where(x => x.ClassIsDocumentType)
                .OrderBy(x => x.ClassId)
                .AsEnumerable()
            ;

        using var k12Classes = EnumerableHelper.CreateDeferrableItemWrapper(cmsClassesDocumentTypes);

        while(k12Classes.GetNext(out var di))
        {
            var (_, k12Class) = di;

            if (k12Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !_primaryKeyMappingContext.HasMapping<CmsClass>(c=> c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (k12Classes.TryDeferItem(di))
                {
                    _logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(k12Class), di.Recurrence);
                }
                else
                {
                    _logger.LogErrorMissingDependency(k12Class, nameof(k12Class.ClassInheritsFromClassId), k12Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    _protocol.Append(HandbookReferences
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            _protocol.FetchedSource(k12Class);

            if (entityConfiguration.ExcludeCodeNames.Contains(k12Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                _protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(k12Class.ClassName, "PageType"), k12Class);
                _logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", k12Class.ClassName);
                continue;
            }

            if (string.Equals(k12Class.ClassName, CLASS_CMS_ROOT, StringComparison.InvariantCultureIgnoreCase))
            {
                _protocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, k12Class);
                _logger.LogInformation("CmsClass: {ClassName} was skipped => CMS.Root cannot be migrated", k12Class.ClassName);
                continue;
            }

            if (string.Equals(k12Class.ClassName, "cms.site", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            var kxoDataClass = _kxpClassFacade.GetClass(k12Class.ClassGuid);
            _protocol.FetchedTarget(kxoDataClass);

            if (SaveUsingKxoApi(k12Class, kxoDataClass) is { } targetClassId)
            {
                foreach (var sourceSite in k12Class.Sites)
                {
                    if (_keyMappingContext.MapSourceKey<KX12M.CmsSite, KXP.Models.CmsChannel, int?>(
                            s => s.SiteId,
                            s => s.SiteGuid,
                            sourceSite.SiteId,
                            t => t.ChannelId,
                            t => t.ChannelGuid
                        ) is {Success:true, Mapped: {} channelId})
                    {
                        var info = new ContentTypeChannelInfo
                        {
                            ContentTypeChannelChannelID = channelId,
                            ContentTypeChannelContentTypeID = targetClassId
                        };
                        ContentTypeChannelInfoProvider.ProviderObject.Set(info);
                    }
                    else
                    {
                        _logger.LogWarning("Channel for site '{SiteName}' not found", sourceSite.SiteName);
                    }
                }
            }
        }

        await MigratePageTemplateConfigurations(cancellationToken);

        return new GenericCommandResult();
    }

    private async Task MigratePageTemplateConfigurations(CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12PageTemplateConfigurations = kx12Context.CmsPageTemplateConfigurations;
        foreach (var k12PageTemplateConfiguration in k12PageTemplateConfigurations)
        {
            await _pageTemplateMigrator.MigratePageTemplateConfigurationAsync(k12PageTemplateConfiguration);
        }
    }

    private int? SaveUsingKxoApi(CmsClass k12Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(k12Class, kxoDataClass);
        _protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                _kxpClassFacade.SetClass(dataClassInfo);

                _protocol.Success(k12Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    k12Class.ClassId,
                    dataClassInfo.ClassID
                );

                return dataClassInfo.ClassID;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving page type {ClassName}", k12Class.ClassName);
        }

        return null;
    }
}