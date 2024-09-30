using System.Collections.Immutable;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Services;

public class MediaLinkServiceFactory(ModelFacade modelFacade)
{
    public MediaLinkService Create()
    {
        var sites = modelFacade.SelectAll<ICmsSite>().Select(x => x switch
        {
            CmsSiteK13 siteK13 => (x.SiteID, x.SiteName, siteK13.SitePresentationURL),
            CmsSiteK12 siteK12 => (x.SiteID, x.SiteName, siteK12.SitePresentationURL ?? siteK12.SiteDomainName),
            CmsSiteK11 siteK11 => (x.SiteID, x.SiteName, siteK11.SitePresentationURL ?? siteK11.SiteDomainName),
            _ => throw new NotImplementedException(),
        }).ToImmutableList();

        var cmsMediaLibrariesFolder = modelFacade
            .SelectWhere<ICmsSettingsKey>("KeyName = N'CMSMediaLibrariesFolder'").Select(x => (x.SiteID, x.KeyValue))
            .ToImmutableList();
        
        var cmsUseMediaLibrariesSiteFolder = modelFacade
            .SelectWhere<ICmsSettingsKey>("KeyName = N'CMSUseMediaLibrariesSiteFolder'").Select(x => (x.SiteID, x.KeyValue))
            .ToImmutableList();

        var mediaLibraries = modelFacade.SelectAll<IMediaLibrary>()
            .GroupBy(x => x.LibrarySiteID)
            .ToDictionary(x => x.Key, x => new HashSet<string>(x.Select(l => l.LibraryFolder)));
        
        return new MediaLinkService(
            sites,
            cmsMediaLibrariesFolder,
            cmsUseMediaLibrariesSiteFolder,
            mediaLibraries
        );;
    }
}
