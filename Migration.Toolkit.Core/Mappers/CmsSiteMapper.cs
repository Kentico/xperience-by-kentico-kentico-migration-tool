// using Microsoft.Extensions.Logging;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Contexts;
// using Migration.Toolkit.Core.MigrationProtocol;
//
// namespace Migration.Toolkit.Core.Mappers;
//
// using Migration.Toolkit.KXP.Models;
//
// public class CmsSiteMapper : EntityMapperBase<KX13.Models.CmsSite, CmsSite>
// {
//     public CmsSiteMapper(
//         ILogger<CmsSiteMapper> logger,
//         PrimaryKeyMappingContext pkContext,
//         IProtocol protocol
//     ) : base(logger, pkContext, protocol)
//     {
//
//     }
//
//     protected override CmsSite? CreateNewInstance(KX13.Models.CmsSite tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();
//
//     protected override CmsSite MapInternal(KX13.Models.CmsSite source, CmsSite target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
//     {
//         target.SiteName = source.SiteName;
//         target.SiteDisplayName = source.SiteDisplayName;
//         target.SiteDescription = source.SiteDescription;
//         target.SiteStatus = source.SiteStatus;
//
//         var sitePresentationUrl = new Uri(source.SitePresentationUrl);
//         target.SiteDomainName = sitePresentationUrl.Host;
//         // target.SiteDomainName = source.SiteDomainName;
//         target.SiteDefaultVisitorCulture = source.SiteDefaultVisitorCulture;
//         // target.SiteGuid = source.SiteGuid; // do not rewrite, instead add siteguid to mapping if you need it
//         target.SiteLastModified = source.SiteLastModified;
//
//         return target;
//     }
// }