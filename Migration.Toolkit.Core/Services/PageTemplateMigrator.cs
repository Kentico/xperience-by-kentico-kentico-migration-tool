// TODOV27 tomas.krch: 2023-09-05: template migrator replacement needed
// namespace Migration.Toolkit.Core.Services;
//
// // using CMS.DocumentEngine => obsolete;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.MigrationProtocol;
//
// public class PageTemplateMigrator
// {
//     private readonly ILogger<PageTemplateMigrator> _logger;
//     private readonly IEntityMapper<KX13M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo> _pageTemplateConfigurationMapper;
//     private readonly IProtocol _protocol;
//
//     public PageTemplateMigrator(
//         ILogger<PageTemplateMigrator> logger,
//         IEntityMapper<KX13M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo> pageTemplateConfigurationMapper,
//         IProtocol protocol
//     )
//     {
//         _logger = logger;
//         _pageTemplateConfigurationMapper = pageTemplateConfigurationMapper;
//         _protocol = protocol;
//     }
//
//     public Task MigratePageTemplateConfigurationAsync(KX13M.CmsPageTemplateConfiguration kx13PageTemplateConfiguration)
//     {
//         var kxpProvider = PageTemplateConfigurationInfoProvider.ProviderObject;
//         var kxpPageTemplateConfiguration = kxpProvider.Get().WhereEquals("PageTemplateConfigurationGUID", kx13PageTemplateConfiguration.PageTemplateConfigurationGuid).FirstOrDefault();
//
//         var mapped = _pageTemplateConfigurationMapper.Map(kx13PageTemplateConfiguration, kxpPageTemplateConfiguration);
//
//         if (mapped is (var pageTemplateConfigurationInfo, _) { Success: true })
//         {
//             try
//             {
//                 kxpProvider.Set(pageTemplateConfigurationInfo);
//
//                 _protocol.Success(kx13PageTemplateConfiguration, pageTemplateConfigurationInfo, null);
//             }
//             catch (Exception exception)
//             {
//                 _logger.LogError(exception, "Failed to create target instance of PageTemplateConfiguration");
//                 _protocol.Append(HandbookReferences
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