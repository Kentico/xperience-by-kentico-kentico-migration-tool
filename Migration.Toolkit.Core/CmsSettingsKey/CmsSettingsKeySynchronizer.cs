// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Configuration;
// using Migration.Toolkit.Core.MigrateSettingKeys;
// using Migration.Toolkit.Core.Services;
// using Migration.Toolkit.KX13.Context;
// using Migration.Toolkit.KXO.Context;
//
// namespace Migration.Toolkit.Core.CmsSettingsKey;
//
// // TODO tk: 2022-05-17: remove, 1:1 sync not needed.
// public class CmsSettingsKeySynchronizer: SynchronizerBase<Migration.Toolkit.KX13.Models.CmsSettingsKey,Migration.Toolkit.KXO.Models.CmsSettingsKey, CmsSettingsKeyKey>, ISynchronizer<Migration.Toolkit.KX13.Models.CmsSettingsKey,Migration.Toolkit.KXO.Models.CmsSettingsKey>
// {
//     private readonly GlobalConfiguration _globalConfiguration;
//     private readonly IPkMappingService _pkMappingService;
//     private readonly IDbContextFactory<KxoContext> _targetContextFactory;
//     private readonly IDbContextFactory<KX13Context> _sourceContextFactory;
//
//     public CmsSettingsKeySynchronizer(
//         ILogger<CmsSettingsKeySynchronizer> logger,
//         IEntityMapper<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey> mapper,
//         // IDataEqualityComparer<Migration.Toolkit.KX13.Models.CmsSettingsKey, Migration.Toolkit.KXO.Models.CmsSettingsKey> comparer,
//         EntityConfigurations entityConfigurations,
//         GlobalConfiguration globalConfiguration,
//         IPkMappingService pkMappingService,
//         IDbContextFactory<KxoContext> targetContextFactory,
//         IDbContextFactory<KX13Context> sourceContextFactory
//     ) :
//         base(logger, mapper, entityConfigurations)
//     {
//         _globalConfiguration = globalConfiguration;
//         _pkMappingService = pkMappingService;
//         _targetContextFactory = targetContextFactory;
//         _sourceContextFactory = sourceContextFactory;
//     }
//
//     protected override IEnumerable<Migration.Toolkit.KX13.Models.CmsSettingsKey> GetSourceEnumerable()
//     {
//         using var dbContext = _sourceContextFactory.CreateDbContext();
//         
//         // TODO tk: 2022-05-08 optimization possible, remove eager .ToList and fix key merge
//         var query = dbContext.CmsSettingsKeys.ToList().OrderBy(x => x.KeyName, StringComparer.InvariantCulture).ThenBy(x => x.SiteId);
//         foreach (var item in query)
//         {
//             if (_pkMappingService.TryMapSiteId(item.SiteId, out var mappedSiteId))
//             {
//                 // TODO tk: 2022-05-08 refactor - this needs to be verified by selecting correct target id
//                 item.SiteId = mappedSiteId;
//                 yield return item;
//             }
//         }
//     }
//
//     protected override IEnumerable<Migration.Toolkit.KXO.Models.CmsSettingsKey> GetTargetEnumerable()
//     {
//         using var dbContext = _targetContextFactory.CreateDbContext();
//         
//         var query = dbContext.CmsSettingsKeys.ToList().OrderBy(x => x.KeyName, StringComparer.InvariantCulture).ThenBy(x => x.SiteId);
//         foreach (var item in query)
//         {
//             yield return item;
//         }
//     }
//
//     protected override IEnumerable<CmsSettingsKeyKey> GetAllKeysEnumerable()
//     {
//         using var kxoContext = _targetContextFactory.CreateDbContext();
//         using var kx13Context = _sourceContextFactory.CreateDbContext();
//
//         var sourceSiteIds = _globalConfiguration.SiteIdMapping.Keys.ToList();
//         var targetSiteIds = _globalConfiguration.SiteIdMapping.Values.ToList();
//
//         var query13 = kx13Context.CmsSettingsKeys.Select(x => new { x.KeyName, x.SiteId, x.KeyGuid }).Where(x => sourceSiteIds.Contains(x.SiteId) || x.SiteId == null);
//         var query14 = kxoContext.CmsSettingsKeys.Select(x => new { x.KeyName, x.SiteId, x.KeyGuid }).Where(x => targetSiteIds.Contains(x.SiteId) || x.SiteId == null);
//
//         var keys = new HashSet<CmsSettingsKeyKey>(query13.AsEnumerable().Aggregate(new List<CmsSettingsKeyKey>(), (list, cmsSettingsKey) =>
//         {
//             if (_pkMappingService.TryMapSiteId(cmsSettingsKey.SiteId, out var mappedSiteId))
//             {
//                 list.Add(CmsSettingsKeyKey.From(cmsSettingsKey.KeyName, mappedSiteId, cmsSettingsKey.KeyGuid));
//             }
//             
//             return list;
//         }));
//
//         keys.UnionWith(query14.Select(x => CmsSettingsKeyKey.From(x.KeyName, x.SiteId, x.KeyGuid)));
//         return keys.OrderBy(x => x.KeyName, StringComparer.InvariantCulture).ThenBy(x => x.SiteId);
//     }
//     
//     protected override CmsSettingsKeyKey? SelectKey(Migration.Toolkit.KX13.Models.CmsSettingsKey? source) => CmsSettingsKeyKey.From(source);
//
//     protected override CmsSettingsKeyKey? SelectKey(Migration.Toolkit.KXO.Models.CmsSettingsKey? target) => CmsSettingsKeyKey.From(target);
//
//     protected override void Insert(Migration.Toolkit.KXO.Models.CmsSettingsKey target)
//     {
//         Logger.LogInformation("INSERTED {target}", Print(target));
//     }
//
//     protected override void Update(Migration.Toolkit.KXO.Models.CmsSettingsKey target)
//     {
//         Logger.LogInformation("UPDATED {target}", Print(target));
//     }
//
//     protected override void Delete(Migration.Toolkit.KXO.Models.CmsSettingsKey target)
//     {
//         Logger.LogInformation("DELETED {target}", Print(target));
//     }
//
//     protected override string Print(Migration.Toolkit.KX13.Models.CmsSettingsKey source)
//     {
//         return $"{source.KeyName} on siteId {source.SiteId}";
//     }
//
//     protected override string Print(Migration.Toolkit.KXO.Models.CmsSettingsKey target)
//     {
//         return $"{target.KeyName} on siteId {target.SiteId}";
//     }
// }