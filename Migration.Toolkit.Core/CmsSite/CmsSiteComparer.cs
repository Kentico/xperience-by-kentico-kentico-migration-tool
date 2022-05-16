using Migration.Toolkit.Common;

namespace Migration.Toolkit.Core.CmsSite;

public class CmsSiteEqualityComparer: IDataEqualityComparer<Migration.Toolkit.KX13.Models.CmsSite, Migration.Toolkit.KXO.Models.CmsSite>
{
    public bool DataEquals(Migration.Toolkit.KX13.Models.CmsSite? source, Migration.Toolkit.KXO.Models.CmsSite? target)
    {
        if (source is null && target is null) return true;
        if (source is null) return false;
        if (target is null) return false;

        return source.SiteId == target.SiteId &&
               source.SiteName == target.SiteName &&
               source.SiteDisplayName == target.SiteDisplayName &&
               source.SiteDescription == target.SiteDescription &&
               source.SiteStatus == target.SiteStatus &&
               source.SiteDomainName == target.SiteDomainName &&
               source.SiteDefaultVisitorCulture == target.SiteDefaultVisitorCulture &&
               source.SiteGuid == target.SiteGuid &&
               source.SiteLastModified == target.SiteLastModified; //&&
        // QUESTION tomas.krch: 2022-05-07  SitePresentationUrl is missing in source
        // source.SitePresentationUrl == target.SitePresentationUrl;
    }
}