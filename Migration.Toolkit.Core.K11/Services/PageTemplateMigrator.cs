// namespace Migration.Toolkit.Core.K11.Services;
//
// using CMS.DataEngine.Internal;
// using CMS.Websites;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Common.Abstractions;
// using Migration.Toolkit.Common.MigrationProtocol;
// using Migration.Toolkit.K11.Models;
//
// public class PageTemplateMigrator(ILogger<PageTemplateMigrator> logger,
//     IEntityMapper<CmsPageTemplateConfiguration, PageTemplateConfigurationInfo> pageTemplateConfigurationMapper,
//     IProtocol protocol)
// {
//     public Task MigratePageTemplateConfigurationAsync(CmsPageTemplateConfiguration k11PageTemplateConfiguration)
//     {
//         var kxpProvider = Provider<PageTemplateConfigurationInfo>.Instance;
//         var kxpPageTemplateConfiguration = kxpProvider.Get().WhereEquals("PageTemplateConfigurationGUID", k11PageTemplateConfiguration.PageTemplateConfigurationGuid).FirstOrDefault();
//
//         var mapped = pageTemplateConfigurationMapper.Map(k11PageTemplateConfiguration, kxpPageTemplateConfiguration);
//
//         if (mapped is { Success: true, Item: { } result })
//         {
//             try
//             {
//                 kxpProvider.Set(result);
//
//                 protocol.Success(k11PageTemplateConfiguration, result, null);
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
//                         SourcePageTemplateConfigurationGuid = k11PageTemplateConfiguration.PageTemplateConfigurationGuid,
//                         SourcePageTemplateConfigurationName = k11PageTemplateConfiguration.PageTemplateConfigurationName
//                     })
//                 );
//             }
//         }
//
//         return Task.CompletedTask;
//     }
// }