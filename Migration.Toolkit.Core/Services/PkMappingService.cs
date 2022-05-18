// using Migration.Toolkit.Core.Configuration;
//
// namespace Migration.Toolkit.Core.Services;
//
// public class PkMappingService: IPkMappingService
// {
//     private readonly GlobalConfiguration _globalConfiguration;
//
//     public PkMappingService(GlobalConfiguration globalConfiguration)
//     {
//         _globalConfiguration = globalConfiguration;
//     }
//     
//     public bool TryMapSiteId(int? sourceSiteId, out int? mappedSiteId)
//     {
//         mappedSiteId = null;
//         return sourceSiteId == null || _globalConfiguration.SiteIdMapping.TryGetValue(sourceSiteId, out mappedSiteId);
//     }
// }