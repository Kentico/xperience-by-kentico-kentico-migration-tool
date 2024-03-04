namespace Migration.Toolkit.Core.K11.Services.CmsClass;

using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;

public class ClassService(ILogger<ClassService> logger, IDbContextFactory<K11Context> k11ContextFactory)
{
    private readonly ILogger<ClassService> _logger = logger;

    private readonly ConcurrentDictionary<string, CmsFormUserControl?> _userControlsCache = new(StringComparer.InvariantCultureIgnoreCase);
    public CmsFormUserControl? GetFormControlDefinition(string userControlCodeName)
    {
        var k11Context = k11ContextFactory.CreateDbContext();
        return _userControlsCache.GetOrAdd(userControlCodeName, s =>
        {
            return k11Context.CmsFormUserControls.FirstOrDefault(x => x.UserControlCodeName == userControlCodeName);
        });
    }
}