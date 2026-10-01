using System.Text.Json;
using System.Text.Json.Nodes;

namespace Migration.Tool.Common.Helpers;

/// <summary>
/// Collects per-channel domain configuration discovered during site migration and renders the
/// "WebsiteChannelDomains" appsettings.json section (DomainOverrides + LanguageDomains).
/// Xperience by Kentico keeps language-specific domains and domain overrides only in application
/// configuration (no database counterpart), so the migration emits a ready-to-paste snippet
/// instead of writing them to the target database.
/// </summary>
public class WebsiteChannelDomainsConfigCollector
{
    private readonly Dictionary<string, Dictionary<string, List<string>>> languageDomains = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<string>> domainOverrides = new(StringComparer.OrdinalIgnoreCase);

    public bool HasAny => languageDomains.Count > 0 || domainOverrides.Count > 0;

    /// <summary>
    /// Registers a domain serving <paramref name="languageCode"/> on channel <paramref name="channelName"/>.
    /// The first domain registered for a language becomes the canonical one; the rest are inbound-only aliases.
    /// </summary>
    public void AddLanguageDomain(string channelName, string languageCode, string domain)
    {
        if (!languageDomains.TryGetValue(channelName, out var languages))
        {
            languages = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            languageDomains[channelName] = languages;
        }

        if (!languages.TryGetValue(languageCode, out var domains))
        {
            domains = [];
            languages[languageCode] = domains;
        }

        domains.Add(domain);
    }

    /// <summary>
    /// Registers a domain override candidate for a path-prefix channel (extra domains the source site responded on).
    /// </summary>
    public void AddDomainOverride(string channelName, string domain)
    {
        if (!domainOverrides.TryGetValue(channelName, out var domains))
        {
            domains = [];
            domainOverrides[channelName] = domains;
        }

        domains.Add(domain);
    }

    /// <summary>
    /// Renders the collected domains as the "WebsiteChannelDomains" appsettings.json section
    /// (bound to WebsiteChannelDomainOptions by the target application).
    /// </summary>
    public string BuildJson()
    {
        var websiteChannelDomains = new JsonObject();

        if (domainOverrides.Count > 0)
        {
            var overridesNode = new JsonObject();
            foreach ((string channelName, var domains) in domainOverrides)
            {
                overridesNode[channelName] = new JsonObject { ["Domains"] = new JsonArray(domains.Distinct(StringComparer.OrdinalIgnoreCase).Select(d => (JsonNode)d).ToArray()) };
            }

            websiteChannelDomains["DomainOverrides"] = overridesNode;
        }

        if (languageDomains.Count > 0)
        {
            var languageDomainsNode = new JsonObject();
            foreach ((string channelName, var languages) in languageDomains)
            {
                var domainsNode = new JsonObject();
                foreach ((string languageCode, var domains) in languages)
                {
                    domainsNode[languageCode] = new JsonArray(domains.Distinct(StringComparer.OrdinalIgnoreCase).Select(d => (JsonNode)d).ToArray());
                }

                languageDomainsNode[channelName] = new JsonObject { ["Domains"] = domainsNode };
            }

            websiteChannelDomains["LanguageDomains"] = languageDomainsNode;
        }

        var root = new JsonObject { ["WebsiteChannelDomains"] = websiteChannelDomains };
        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }

}
