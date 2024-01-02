namespace Migration.Toolkit.Core.Handlers;

using CMS.ContentEngine;
using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;

public class MigratePageTypesCommandHandler : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    private readonly ILogger<MigratePageTypesCommandHandler> _logger;
    private readonly IEntityMapper<CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly IProtocol _protocol;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    private readonly KeyMappingContext _keyMappingContext;
    private readonly PageTemplateMigrator _pageTemplateMigrator;

    public MigratePageTypesCommandHandler(
        ILogger<MigratePageTypesCommandHandler> logger,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IDbContextFactory<KX13Context> kx13ContextFactory,
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
        _kx13ContextFactory = kx13ContextFactory;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _kxpClassFacade = kxpClassFacade;
        _protocol = protocol;
        _toolkitConfiguration = toolkitConfiguration;
        _keyMappingContext = keyMappingContext;
        _pageTemplateMigrator = pageTemplateMigrator;
    }

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        var cmsClassesDocumentTypes = kx13Context.CmsClasses
                .Include(c => c.Sites)
                .Where(x => x.ClassIsDocumentType)
                .OrderBy(x => x.ClassId)
                .AsEnumerable()
            ;

        using var kx13Classes = EnumerableHelper.CreateDeferrableItemWrapper(cmsClassesDocumentTypes);

        while(kx13Classes.GetNext(out var di))
        {
            var (_, kx13Class) = di;

            if (kx13Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !_primaryKeyMappingContext.HasMapping<CmsClass>(c=> c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (kx13Classes.TryDeferItem(di))
                {
                    _logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(kx13Class), di.Recurrence);
                }
                else
                {
                    _logger.LogErrorMissingDependency(kx13Class, nameof(kx13Class.ClassInheritsFromClassId), kx13Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    _protocol.Append(HandbookReferences
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            _protocol.FetchedSource(kx13Class);

            if (entityConfiguration.ExcludeCodeNames.Contains(kx13Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                _protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(kx13Class.ClassName, "PageType"), kx13Class);
                _logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", kx13Class.ClassName);
                continue;
            }

            if (string.Equals(kx13Class.ClassName, CLASS_CMS_ROOT, StringComparison.InvariantCultureIgnoreCase))
            {
                _protocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, kx13Class);
                _logger.LogInformation("CmsClass: {ClassName} was skipped => CMS.Root cannot be migrated", kx13Class.ClassName);
                continue;
            }

            // kx13Class.ClassConnectionString check is not necessary

            var kxoDataClass = _kxpClassFacade.GetClass(kx13Class.ClassGuid);
            _protocol.FetchedTarget(kxoDataClass);

            if (SaveUsingKxoApi(kx13Class, kxoDataClass) is { } targetClassId)
            {
                foreach (var sourceSite in kx13Class.Sites)
                {
                    if (_keyMappingContext.MapSourceKey<KX13M.CmsSite, KXP.Models.CmsChannel, int?>(
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
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13PageTemplateConfigurations = kx13Context.CmsPageTemplateConfigurations;
        foreach (var kx13PageTemplateConfiguration in kx13PageTemplateConfigurations)
        {
            await _pageTemplateMigrator.MigratePageTemplateConfigurationAsync(kx13PageTemplateConfiguration);
        }
    }

    private int? SaveUsingKxoApi(CmsClass kx13Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(kx13Class, kxoDataClass);
        _protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                _kxpClassFacade.SetClass(dataClassInfo);

                _protocol.Success(kx13Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    kx13Class.ClassId,
                    dataClassInfo.ClassID
                );

                return dataClassInfo.ClassID;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving page type {ClassName}", kx13Class.ClassName);
        }

        return null;
    }
}