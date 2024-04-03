namespace Migration.Toolkit.Source.Services;

using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Source.Model;

public class ClassService(ILogger<ClassService> logger, ModelFacade modelFacade)
{
    private readonly ConcurrentDictionary<string, ICmsFormUserControl?> _userControlsCache = new(StringComparer.InvariantCultureIgnoreCase);
    public ICmsFormUserControl? GetFormControlDefinition(string userControlCodeName)
    {
        return _userControlsCache.GetOrAdd(userControlCodeName, s =>
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
}