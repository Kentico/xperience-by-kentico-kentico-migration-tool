namespace Migration.Toolkit.Core.KX12.Services;

using CMS.DataEngine.Internal;
using CMS.Websites;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;

public class PageTemplateMigrator
{
    private readonly ILogger<PageTemplateMigrator> _logger;
    private readonly IEntityMapper<KX12M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo> _pageTemplateConfigurationMapper;
    private readonly IProtocol _protocol;

    public PageTemplateMigrator(
        ILogger<PageTemplateMigrator> logger,
        IEntityMapper<KX12M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo> pageTemplateConfigurationMapper,
        IProtocol protocol
    )
    {
        _logger = logger;
        _pageTemplateConfigurationMapper = pageTemplateConfigurationMapper;
        _protocol = protocol;
    }

    public Task MigratePageTemplateConfigurationAsync(KX12M.CmsPageTemplateConfiguration k12PageTemplateConfiguration)
    {
        var kxpProvider = Provider<PageTemplateConfigurationInfo>.Instance;
        var kxpPageTemplateConfiguration = kxpProvider.Get().WhereEquals("PageTemplateConfigurationGUID", k12PageTemplateConfiguration.PageTemplateConfigurationGuid).FirstOrDefault();

        var mapped = _pageTemplateConfigurationMapper.Map(k12PageTemplateConfiguration, kxpPageTemplateConfiguration);

        if (mapped is { Success: true, Item: { } result })
        {
            try
            {
                kxpProvider.Set(result);

                _protocol.Success(k12PageTemplateConfiguration, result, null);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create target instance of PageTemplateConfiguration");
                _protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<PageTemplateConfigurationInfo>(exception)
                    .NeedsManualAction()
                    .WithData(new
                    {
                        exception,
                        SourcePageTemplateConfigurationGuid = k12PageTemplateConfiguration.PageTemplateConfigurationGuid,
                        SourcePageTemplateConfigurationName = k12PageTemplateConfiguration.PageTemplateConfigurationName
                    })
                );
            }
        }

        return Task.CompletedTask;
    }
}