using Microsoft.Extensions.Configuration;

namespace Migration.Tool.CLI;
public static class ConfigurationExtensions
{
    public static IConfigurationSection GetSectionWithFallback(this IConfigurationSection section, string key, params string[] fallbackKeys)
    {
        var resolvedSection = section.GetSection(key);
        if (!resolvedSection.Exists())
        {
            foreach (string fallbackKey in fallbackKeys)
            {
                resolvedSection = section.GetSection(fallbackKey);
                if (resolvedSection.Exists())
                {
                    break;
                }
            }
        }
        return resolvedSection;
    }
}
