using CMS.Websites;
using Microsoft.Extensions.Logging;

namespace Migration.Tool.Common.Helpers;
public class WebsiteChannelDomainSanitizer
{
    private readonly ILogger logger;
    private readonly HashSet<string> websiteChannelDomains = [];

    public WebsiteChannelDomainSanitizer(ILogger logger, IEnumerable<WebsiteChannelInfo> existingChannels)
    {
        this.logger = logger;

        foreach (var websiteChannelInfo in existingChannels)
        {
            websiteChannelDomains.Add(websiteChannelInfo.WebsiteChannelDomain);
        }
    }

    public bool GetCandidate(string siteName, string? domainName, out string? sanitizedName)
    {
        if (domainName is null)
        {
            sanitizedName = domainName;
            return true;
        }

        int fallbackDomainPort = 5000;
        var result = UriHelper.GetUniqueDomainCandidate(
            domainName,
            ref fallbackDomainPort,
            candidate => !websiteChannelDomains.Contains(candidate)
        );

        switch (result)
        {
            case (true, false, var candidate, null):
            {
                sanitizedName = candidate;
                break;
            }
            case (true, true, var candidate, null):
            {
                sanitizedName = candidate;
                logger.LogWarning("Domain '{Domain}' of site '{SiteName}' is not unique. '{Fallback}' is used instead", domainName, siteName, candidate);
                break;
            }
            case { Success: false, Fallback: { } fallback }:
            {
                sanitizedName = fallback;
                logger.LogWarning("Unable to use domain '{Domain}' of site '{SiteName}' as channel domain. Fallback '{Fallback}' is used", domainName, siteName, fallback);
                break;
            }
            default:
            {
                logger.LogError("Unable to use domain '{Domain}' of site '{SiteName}' as channel domain. No fallback available, skipping site", domainName, siteName);
                sanitizedName = null;
                return false;
            }
        }

        return true;
    }

    public void CommitCandidate(string? candidate)
    {
        if (candidate is not null)
        {
            websiteChannelDomains.Add(candidate);
        }
    }
}
