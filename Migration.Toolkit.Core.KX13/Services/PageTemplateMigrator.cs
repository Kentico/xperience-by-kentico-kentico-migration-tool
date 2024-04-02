// namespace Migration.Toolkit.Core.KX13.Services;
//
// using CMS.DataEngine;
// using CMS.Websites;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Common.Abstractions;
// using Migration.Toolkit.Common.MigrationProtocol;
//
// public class PageTemplateMigrator(
//     ILogger<PageTemplateMigrator> logger,
//     IEntityMapper<KX13M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo> pageTemplateConfigurationMapper,
//     IProtocol protocol)
// {
//     public Task MigratePageTemplateConfigurationAsync(KX13M.CmsPageTemplateConfiguration kx13PageTemplateConfiguration)
//     {
//         var kxpProvider = Provider<PageTemplateConfigurationInfo>.Instance;
//         var kxpPageTemplateConfiguration = kxpProvider.Get().WhereEquals("PageTemplateConfigurationGUID", kx13PageTemplateConfiguration.PageTemplateConfigurationGuid).FirstOrDefault();
//
//         var mapped = pageTemplateConfigurationMapper.Map(kx13PageTemplateConfiguration, kxpPageTemplateConfiguration);
//
//         if (mapped is { Success: true, Item: { } result })
//         {
//             try
//             {
//                 kxpProvider.Set(result);
//
//                 protocol.Success(kx13PageTemplateConfiguration, result, null);
//             }
//             catch (Exception exception)
//             {
//                 logger.LogError(exception, "Failed to create target instance of PageTemplateConfiguration");
//                 protocol.Append(HandbookReferences
//                     .ErrorCreatingTargetInstance<PageTemplateConfigurationInfo>(exception)
//                     .NeedsManualAction()
//                     .WithData(new
//                     {
//                         exception,
//                         SourcePageTemplateConfigurationGuid = kx13PageTemplateConfiguration.PageTemplateConfigurationGuid,
//                         SourcePageTemplateConfigurationName = kx13PageTemplateConfiguration.PageTemplateConfigurationName
//                     })
//                 );
//             }
//         }
//
//         return Task.CompletedTask;
//     }
// }