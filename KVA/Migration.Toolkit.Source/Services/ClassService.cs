using System.Collections.Concurrent;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Services;

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
        catch (Exception ex)
        {
            logger.LogError("Error while retrieving FormUserControl with codename {CodeName}", s);
        }

        return null;
    });
}
