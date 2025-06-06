using System.Collections.Concurrent;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class ClassService(ILogger<ClassService> logger, ModelFacade modelFacade)
{
    private readonly ConcurrentDictionary<string, ICmsFormUserControl?> userControlsCache = new(StringComparer.InvariantCultureIgnoreCase);

    public ICmsFormUserControl? GetFormControlDefinition(string userControlCodeName) => userControlsCache.GetOrAdd(userControlCodeName, s =>
    {
        try
        {
            var cmsFormUserControl = modelFacade.SelectWhere<ICmsFormUserControl>(
                "UserControlCodeName = @userControlCodeName",
                new SqlParameter("userControlCodeName", userControlCodeName)
            ).SingleOrDefault();

            return cmsFormUserControl;
        }
        catch
        {
            logger.LogError("Error while retrieving FormUserControl with codename {CodeName}", s);
        }

        return null;
    });
}
