// TODOV27 tomas.krch: 2023-09-05: replace implementation => obsolete
// namespace Migration.Toolkit.KXP.Api;
//
// using CMS.Base;
// // using CMS.SiteProvider; => obsolete
// using Microsoft.Extensions.Logging;
//
// // public record SiteInfoResult(SiteInfo SiteInfo, string CultureCode);
//
// public class KxpSiteFacade
// {
//     private readonly ILogger<KxpSiteFacade> _logger;
//
//     public KxpSiteFacade(ILogger<KxpSiteFacade> logger, KxpApiInitializer kxpApiInitializer)
//     {
//         _logger = logger;
//         kxpApiInitializer.EnsureApiIsInitialized();
//     }
//
//     // public SiteInfoResult GetSiteInfo(int siteId, string prefreredCulture)
//     // {
//     //     // var siteInfo = SiteInfoProvider.ProviderObject.Get(siteId);
//     //     // // assumption: current release supports only 1 culture
//     //     // var cultureCodes = CultureSiteInfoProvider.GetSiteCultureCodes(siteInfo.SiteName);
//     //     // var siteCulture = cultureCodes.SingleOrDefault(x => x.Equals(prefreredCulture, StringComparison.InvariantCultureIgnoreCase)) ?? cultureCodes.First();
//     //     //
//     //     // return new SiteInfoResult(siteInfo, siteCulture);
//     //     var c = 1;
//     //     throw new NotImplementedException("Obsolete feature, needs to be replaced");
//     // }
// }