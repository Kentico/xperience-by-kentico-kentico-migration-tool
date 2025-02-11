// using System.ComponentModel.DataAnnotations.Schema;
// using Microsoft.Extensions.Logging;
// using Migration.Tool.Common.Helpers;
// using Migration.Tool.K11;
//
// namespace Migration.Tool.Core.K11.Services;
//
// public class TableReflectionService
// {
//     private readonly ILogger<TableReflectionService> logger;
//     private readonly Dictionary<string, Type> tableNameLookup;
//
//     public TableReflectionService(ILogger<TableReflectionService> logger)
//     {
//         this.logger = logger;
//         var (_, tnl) = typeof(K11Context).Assembly.GetTypes().Aggregate((
//             nameLookup: new Dictionary<string, Type>(),
//             tableNameLookup: new Dictionary<string, Type>()
//         ), (lookups, type) =>
//         {
//             var rh = new ReflectionHelper(type);
//
//             if (rh.GetFirstAttributeOrNull<TableAttribute>()?.Name is { } tableName && !string.IsNullOrWhiteSpace(tableName))
//             {
//                 lookups.tableNameLookup[tableName] = type;
//                 lookups.nameLookup[type.Name] = type;
//             }
//
//             return lookups;
//         });
//
//         tableNameLookup = tnl;
//     }
//
//     public Type GetSourceTableTypeByTableName(string tableName)
//     {
//         if (!tableNameLookup.TryGetValue(tableName, out var name))
//         {
//             string joinedKeys = string.Join(", ", tableNameLookup.Keys);
//             logger.LogError("Invalid table name, use one of following: {TableNames}", joinedKeys);
//             throw new KeyNotFoundException($"Invalid table name, use one of following: {joinedKeys}");
//         }
//
//         return name;
//     }
// }
