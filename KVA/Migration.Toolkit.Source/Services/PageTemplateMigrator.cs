namespace Migration.Toolkit.Source.Services;

using CMS.DataEngine;
using CMS.Websites;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Source.Model;

public class PageTemplateMigrator(
    ILogger<PageTemplateMigrator> logger,
    IEntityMapper<ICmsPageTemplateConfiguration, PageTemplateConfigurationInfo> pageTemplateConfigurationMapper,
    IProtocol protocol)
{
    public Task MigratePageTemplateConfigurationAsync(ICmsPageTemplateConfiguration ksPageTemplateConfiguration)
    {
        if (ksPageTemplateConfiguration is ICmsPageTemplateConfigurationK12K13 pageTemplateConfiguration)
        {
            var kxpProvider = Provider<PageTemplateConfigurationInfo>.Instance;
            var kxpPageTemplateConfiguration = kxpProvider.Get().WhereEquals("PageTemplateConfigurationGUID", pageTemplateConfiguration.PageTemplateConfigurationGUID).FirstOrDefault();

            var mapped = pageTemplateConfigurationMapper.Map(ksPageTemplateConfiguration, kxpPageTemplateConfiguration);

            if (mapped is { Success: true, Item: { } result })
            {
                try
                {
                    kxpProvider.Set(result);

                    protocol.Success(ksPageTemplateConfiguration, result, null);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Failed to create target instance of PageTemplateConfiguration");
                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<PageTemplateConfigurationInfo>(exception)
                        .NeedsManualAction()
                        .WithData(new
                        {
                            exception,
                            SourcePageTemplateConfigurationGuid = pageTemplateConfiguration.PageTemplateConfigurationGUID,
                            SourcePageTemplateConfigurationName = pageTemplateConfiguration.PageTemplateConfigurationName
                        })
                    );
                }
            }
            return Task.CompletedTask;
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}