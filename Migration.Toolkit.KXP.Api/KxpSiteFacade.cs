namespace Migration.Toolkit.KXP.Api;

using CMS.Base;
using CMS.SiteProvider;
using Microsoft.Extensions.Logging;

public record SiteInfoResult(SiteInfo SiteInfo, string CultureCode);

public class KxpSiteFacade
{
    private readonly ILogger<KxpSiteFacade> _logger;

    public KxpSiteFacade(ILogger<KxpSiteFacade> logger, KxpApiInitializer kxpApiInitializer)
    {
        _logger = logger;
        kxpApiInitializer.EnsureApiIsInitialized();
    }

    public SiteInfoResult GetSiteInfo(int siteId)
    {
        var siteInfo = SiteInfoProvider.ProviderObject.Get(siteId);
        // assumption: current release supports only 1 culture
        var siteCulture = CultureSiteInfoProvider.GetSiteCultureCodes(siteInfo.SiteName).Single();

        return new SiteInfoResult(siteInfo, siteCulture);
    } 
}