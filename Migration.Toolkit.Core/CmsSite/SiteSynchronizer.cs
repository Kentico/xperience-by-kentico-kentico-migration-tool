// using System.ComponentModel.DataAnnotations.Schema;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Common;
// using Migration.Toolkit.Common.Helpers;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Configuration;
// using Migration.Toolkit.KX13.Context;
// using Migration.Toolkit.KXO.Context;
// using K13M = Migration.Toolkit.KX13.Models;
// using K14M = Migration.Toolkit.KXO.Models;
//
// namespace Migration.Toolkit.Core.CmsSite;
//
// public class SiteSynchronizer : ISynchronizer<K13M.CmsSite, K14M.CmsSite>
// {
//     protected readonly ILogger<SiteSynchronizer> Logger;
//     protected readonly GlobalConfiguration GlobalConfiguration;
//     
//     private readonly IEntityMapper<K13M.CmsSite, Migration.Toolkit.KXO.Models.CmsSite> _mapper;
//     private readonly IDataEqualityComparer<K13M.CmsSite, K14M.CmsSite> _comparer;
//     private readonly EntityConfigurations _entityConfigurations;
//     
//
//     public SiteSynchronizer(ILogger<SiteSynchronizer> logger, IEntityMapper<K13M.CmsSite, K14M.CmsSite> mapper,
//         IDataEqualityComparer<K13M.CmsSite, K14M.CmsSite> comparer, EntityConfigurations entityConfigurations, GlobalConfiguration globalConfiguration)
//     {
//         Logger = logger;
//         GlobalConfiguration = globalConfiguration;
//         _mapper = mapper;
//         _comparer = comparer;
//         _entityConfigurations = entityConfigurations;
//     }
//
//     public async Task StartAsync()
//     {
//         await using var k13Context = new KX13Context();
//         await using var k14Context = new KxoContext();
//
//         var allKeys =
//             k13Context.CmsSites.Select(x => x.SiteGuid).OrderBy(x => x).ToList()
//                 .Union(k14Context.CmsSites.Select(x => x.SiteGuid).OrderBy(x => x)).Distinct().OrderBy(x => x).ToList();
//
//         using var aligner = SimpleAligner<K13M.CmsSite, K14M.CmsSite, Guid>
//             .Create(
//                 k13Context.CmsSites.OrderBy(x => x.SiteGuid).ToList().OrderBy(x => x.SiteGuid).GetEnumerator(),
//                 k14Context.CmsSites.OrderBy(x => x.SiteGuid).ToList().OrderBy(x => x.SiteGuid).GetEnumerator(),
//                 allKeys.GetEnumerator(),
//                 x => x.SiteGuid,
//                 x => x.SiteGuid,
//                 true
//             );
//
//         var tableName = ReflectionHelper<K14M.CmsSite>.GetFirstAttributeOrNull<TableAttribute>()?.Name;
//         var configuration = _entityConfigurations.GetEntityConfiguration(tableName);
//         
//         // TODO tk: 2022-05-06 create buffer for bulk updates / inserts
//         foreach (var item in aligner)
//         {
//          item.   
//         }
//         while (aligner.MoveNext())
//         {
//             var current = aligner.Current;
//
//             switch (current)
//             {
//                 case AlignResultMatchSame<K13M.CmsSite, K14M.CmsSite, Guid, ModelMappingResult<K14M.CmsSite>?>(_, var b, _, _):
//                     Logger.LogTrace("Updating NOT {item} (item data is equal)", b);
//                     break;
//                 case AlignResultOnlyA<K13M.CmsSite, K14M.CmsSite, Guid, ModelMappingResult<K14M.CmsSite>?>(var cmsSite, _, var result) when result! is { Success: true }:
//                     // insert
//                     await k14Context.CmsSites.AddAsync(result.Item!);
//                     Logger.LogInformation("Inserting {item}", cmsSite);
//                     break;
//                 case AlignResultOnlyB<K13M.CmsSite, K14M.CmsSite, Guid, ModelMappingResult<K14M.CmsSite>?>(var target, _, _):
//                     if (configuration.DeleteTargetIfSourceNotExists)
//                     {
//                         k14Context.CmsSites.Remove(target);
//                         Logger.LogInformation("Deleting {item}", target);
//                     }
//                     else
//                     {
//                         Logger.LogInformation("Not deleting {item}", target);
//                     }
//                     break;
//                 case AlignResultMatchMapped<K13M.CmsSite, K14M.CmsSite, Guid, ModelMappingResult<K14M.CmsSite>?>(var cmsSite, _, _, var result) when result! is { Success: true }:
//                     switch (result)
//                     {
//                         case null:
//                             // skip
//                             Logger.LogWarning("Result of mapping was not created");
//                             break;
//                         case ModelMappingFailed<K14M.CmsSite>(var message):
//                             // skip
//                             Logger.LogWarning(message);
//                             break;
//                         case ModelMappingFailedKeyMismatch<K14M.CmsSite>(_, _, var message, _):
//                             // skip
//                             Logger.LogWarning(message);
//                             break;
//                         case ModelMappingFailedSourceNotDefined<K14M.CmsSite>(_, _, var message, _):
//                             // skip
//                             Logger.LogWarning(message);
//                             break;
//                         case ModelMappingSuccess<K14M.CmsSite>(var mappingResult, var newInstance):
//                             if (newInstance)
//                             {
//                                 // insert
//                                 await k14Context.CmsSites.AddAsync(result.Item!);
//                                 Logger.LogInformation("Inserting {item}", cmsSite);
//                             }
//                             else
//                             {
//                                 // update
//                                 k14Context.CmsSites.Update(mappingResult!); 
//                                 Logger.LogInformation("Updating {item}", cmsSite);    
//                             }
//                             break;
//                         default:
//                             throw new ArgumentOutOfRangeException(nameof(result));
//                     }
//                     break;
//                 case AlignFatalNoMatch<K13M.CmsSite, K14M.CmsSite, Guid, ModelMappingResult<K14M.CmsSite>?>(_, _, var key, var errorDescription):
//                     // skip
//                     Logger.LogError("Failed to match {key} with error {errorDescription}", key, errorDescription);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(current));
//             }
//         }
//     }
// }