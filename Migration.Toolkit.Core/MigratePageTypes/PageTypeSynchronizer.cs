// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Configuration;
// using Migration.Toolkit.KX13.Context;
// using Migration.Toolkit.KXO.Context;
//
// namespace Migration.Toolkit.Core.MigratePageTypes;
//
// public class PageTypeSynchronizer: SynchronizerBase<Migration.Toolkit.KX13.Models.CmsClass, Migration.Toolkit.KXO.Models.CmsClass, string>, ISynchronizer<Migration.Toolkit.KX13.Models.CmsClass,Migration.Toolkit.KXO.Models.CmsClass>
// {
//     private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
//     private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
//
//     public PageTypeSynchronizer(
//         ILogger<PageTypeSynchronizer> logger,
//         IEntityMapper<KX13.Models.CmsClass, KXO.Models.CmsClass> mapper, 
//         // IDataEqualityComparer<KX13.Models.CmsClass, KXO.Models.CmsClass> comparer, 
//         EntityConfigurations entityConfigurations,
//         IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
//         IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory
//         ) : base(logger, mapper, entityConfigurations)
//     {
//         _kxoContextFactory = kxoContextFactory;
//         _kx13ContextFactory = kx13ContextFactory;
//     }
//
//     protected override IEnumerable<KX13.Models.CmsClass> GetSourceEnumerable()
//     {
//         using var kx13Context = _kx13ContextFactory.CreateDbContext();
//
//         return kx13Context.CmsClasses.Where(c => c.ClassIsDocumentType).OrderBy(x => x.ClassName);
//     }
//
//     protected override IEnumerable<KXO.Models.CmsClass> GetTargetEnumerable()
//     {
//         using var kxoContext = _kxoContextFactory.CreateDbContext();
//
//         return kxoContext.CmsClasses.Where(c => c.ClassIsDocumentType).OrderBy(x => x.ClassName);
//     }
//
//     protected override IEnumerable<string> GetAllKeysEnumerable()
//     {
//         using var kx13Context = _kx13ContextFactory.CreateDbContext();
//         using var kxoContext = _kxoContextFactory.CreateDbContext();
//
//         return kx13Context.CmsClasses.Where(c => c.ClassIsDocumentType).Select(x => x.ClassName).Union(
//             kxoContext.CmsClasses.Where(c => c.ClassIsDocumentType).Select(x => x.ClassName));
//     }
//
//     protected override string? SelectKey(KX13.Models.CmsClass? source)
//     {
//         return source?.ClassName;
//     }
//
//     protected override string? SelectKey(KXO.Models.CmsClass? target)
//     {
//         return target?.ClassName;
//     }
//
//     protected override void Insert(KXO.Models.CmsClass item)
//     {
//         // throw new NotImplementedException();
//     }
//
//     protected override void Update(KXO.Models.CmsClass item)
//     {
//         // throw new NotImplementedException();
//     }
//
//     protected override void Delete(KXO.Models.CmsClass item)
//     {
//         // throw new NotImplementedException();
//     }
//
//     protected override string Print(KX13.Models.CmsClass item) => $"{item.ClassName}: {item.ClassDisplayName}";
//
//     protected override string Print(KXO.Models.CmsClass item) => $"{item.ClassName}: {item.ClassDisplayName}";
// }