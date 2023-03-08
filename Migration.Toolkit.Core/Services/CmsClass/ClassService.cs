using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.KX13.Context;

namespace Migration.Toolkit.Core.Services.CmsClass;

public class ClassService
{
    private readonly ILogger<ClassService> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;

    public ClassService(ILogger<ClassService> logger, IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory)
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
    }

    private readonly ConcurrentDictionary<string, KX13M.CmsFormUserControl?> _userControlsCache = new(StringComparer.InvariantCultureIgnoreCase);
    public KX13M.CmsFormUserControl? GetFormControlDefinition(string userControlCodeName)
    {
        var kx13Context = _kx13ContextFactory.CreateDbContext();
        return _userControlsCache.GetOrAdd(userControlCodeName, s =>
        {
            return kx13Context.CmsFormUserControls.FirstOrDefault(x => x.UserControlCodeName == userControlCodeName);
        });
    }
}