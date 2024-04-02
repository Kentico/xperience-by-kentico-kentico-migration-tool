// namespace Migration.Toolkit.Core.KX12.Services.CmsClass;
//
// using System.Collections.Concurrent;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.KX12.Context;
//
// public class ClassService
// {
//     private readonly ILogger<ClassService> _logger;
//     private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
//
//     public ClassService(ILogger<ClassService> logger, IDbContextFactory<KX12Context> kx12ContextFactory)
//     {
//         _logger = logger;
//         _kx12ContextFactory = kx12ContextFactory;
//     }
//
//     private readonly ConcurrentDictionary<string, KX12M.CmsFormUserControl?> _userControlsCache = new(StringComparer.InvariantCultureIgnoreCase);
//     public KX12M.CmsFormUserControl? GetFormControlDefinition(string userControlCodeName)
//     {
//         var kx12Context = _kx12ContextFactory.CreateDbContext();
//         return _userControlsCache.GetOrAdd(userControlCodeName, s =>
//         {
//             return kx12Context.CmsFormUserControls.FirstOrDefault(x => x.UserControlCodeName == userControlCodeName);
//         });
//     }
// }