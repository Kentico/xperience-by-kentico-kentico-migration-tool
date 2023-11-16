namespace Migration.Toolkit.Core.MappersNG;

using CMS.ContentEngine;
using CMS.Websites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;

public record SiteMapperResult(ChannelInfo ChannelInfo, WebsiteChannelInfo WebsiteChannelInfo);
public class SiteMapper: EntityMapperBase<KX13M.CmsSite, SiteMapperResult>
{
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;

    public SiteMapper(
        ILogger<SiteMapper> logger,
        PrimaryKeyMappingContext pkContext,
        IProtocol protocol,
        IDbContextFactory<KX13Context> kx13ContextFactory
    ) : base(logger, pkContext, protocol)
    {
        _kx13ContextFactory = kx13ContextFactory;
    }

    protected override SiteMapperResult CreateNewInstance(KX13.Models.CmsSite tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new(new ChannelInfo(), new WebsiteChannelInfo());

    protected override SiteMapperResult MapInternal(KX13.Models.CmsSite source, SiteMapperResult target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (channelInfo, websiteChannelInfo) = target;

        channelInfo.ChannelType = ChannelType.Website;
        channelInfo.ChannelName = source.SiteName;
        channelInfo.ChannelDisplayName = source.SiteDisplayName;
        channelInfo.ChannelGUID = source.SiteGuid;

        var sitePresentationUrl = new Uri(source.SitePresentationUrl);
        var homePagePath = KenticoHelper.GetSettingsKey(_kx13ContextFactory, source.SiteId, "CMSHomePagePath");
        var cookieLevel = KenticoHelper.GetSettingsKey(_kx13ContextFactory, source.SiteId, "CMSDefaultCookieLevel") switch
        {
            "all" => CookieLevelConstants.ALL,
            "visitor" => CookieLevelConstants.VISITOR,
            "editor" => CookieLevelConstants.EDITOR,
            "system" => CookieLevelConstants.SYSTEM,
            "essential" => CookieLevelConstants.ESSENTIAL,
            _ => (int?)null
        };
        var storeFormerUrls = KenticoHelper.GetSettingsKey(_kx13ContextFactory, source.SiteId, "CMSStoreFormerUrls") is { } storeFormerUrlsStr
            ? bool.TryParse(storeFormerUrlsStr, out var sfu) ? (bool?)sfu : null
            : null;

        // TODO tomas.krch: 2023-11-01 search content language by culture
        // source.SiteDefaultVisitorCulture

        websiteChannelInfo.WebsiteChannelDefaultCookieLevel = cookieLevel ?? CookieLevelConstants.ALL;
        websiteChannelInfo.WebsiteChannelGUID = source.SiteGuid;
        websiteChannelInfo.WebsiteChannelDomain = sitePresentationUrl.Host;
        websiteChannelInfo.WebsiteChannelHomePage = homePagePath;
        // TODO tomas.krch: 2023-11-01 this needs to be done late - it will be done by UMT
        websiteChannelInfo.WebsiteChannelPrimaryContentLanguageID = 0;
        // TODO tomas.krch: 2023-11-01 this needs to be done late - it will be done by UMT
        websiteChannelInfo.WebsiteChannelChannelID = channelInfo.ChannelID;
        websiteChannelInfo.WebsiteChannelStoreFormerUrls = storeFormerUrls ?? true;
        // websiteChannelInfo.WebsiteChannelID = 0;

        // target.SiteName = source.SiteName;
        // target.SiteDisplayName = source.SiteDisplayName;
        // target.SiteDescription = source.SiteDescription;
        // target.SiteStatus = source.SiteStatus;

        // var sitePresentationUrl = new Uri(source.SitePresentationUrl);
        // target.SiteDomainName = sitePresentationUrl.Host;
        // target.SiteDomainName = source.SiteDomainName;
        // target.SiteDefaultVisitorCulture = source.SiteDefaultVisitorCulture;
        // target.SiteGuid = source.SiteGuid; // do not rewrite, instead add siteguid to mapping if you need it
        // target.SiteLastModified = source.SiteLastModified;

        return target;
    }
}