// using Microsoft.EntityFrameworkCore;
//
// namespace Migration.Toolkit.Core;
//
// public static class EfCoreHelper
// {
//     public static Task EnableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: true);
//     public static Task DisableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: false);
//
//     private static Task SetIdentityInsert<T>(DbContext context, bool enable)
//     {
//         var entityType = context.Model.FindEntityType(typeof(T));
//         var value = enable ? "ON" : "OFF";
//         return context.Database.ExecuteSqlRawAsync(
//             $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
//     }
//
//     public static void SaveChangesWithIdentityInsert<T>(this DbContext context)
//     {
//         using var transaction = context.Database.BeginTransaction();
//         context.EnableIdentityInsert<T>();
//         context.SaveChanges();
//         context.DisableIdentityInsert<T>();
//         transaction.Commit();
//     }
// }