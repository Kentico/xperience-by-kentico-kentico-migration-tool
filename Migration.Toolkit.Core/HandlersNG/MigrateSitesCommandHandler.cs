namespace Migration.Toolkit.Core.HandlersNG;

using CMS.ContentEngine;
using CMS.Websites;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Helpers;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;

// ReSharper disable once UnusedType.Global
public class MigrateSitesCommandHandlerNG : IRequestHandler<MigrateSitesCommand, CommandResult>
{
    private readonly ILogger<MigrateSitesCommandHandlerNG> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IProtocol _protocol;
    private readonly IImporter _importer;

    public MigrateSitesCommandHandlerNG(
        ILogger<MigrateSitesCommandHandlerNG> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IProtocol protocol,
        IImporter importer
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _protocol = protocol;
        _importer = importer;
    }

    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var migratedCultureCodes = new Dictionary<string, ContentLanguageInfo>(StringComparer.CurrentCultureIgnoreCase);
        foreach (var kx13CmsSite in kx13Context.CmsSites.Include(s => s.Cultures))
        {
            _protocol.FetchedSource(kx13CmsSite);
            _logger.LogTrace("Migrating site {SiteName} with SiteGuid {SiteGuid}", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);

            // not needed enymore - verify
            // var targetSiteId = _primaryKeyMappingContext.MapFromSourceOrNull<CmsSite>(s => s.SiteId, kx13CmsSite.SiteId);
            // if (targetSiteId is null)
            // {
            //     // TODO tk: 2022-05-26 add site guid mapping
            //     _logger.LogWarning("Site '{SiteName}' with Guid {Guid} migration skipped", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);
            //     continue;
            // }

            var defaultCultureCode = GetSiteCulture(kx13CmsSite);
            var migratedSiteCultures = kx13CmsSite.Cultures.ToList();
            if (!migratedSiteCultures.Any(x => x.CultureCode.Equals(defaultCultureCode, StringComparison.InvariantCultureIgnoreCase)))
            {
                await using var ctx = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
                if (ctx.CmsCultures.FirstOrDefault(c => c.CultureCode == defaultCultureCode) is { } defaultCulture)
                {
                    migratedSiteCultures.Add(defaultCulture);
                }
            }

            foreach (var cmsCulture in migratedSiteCultures)
            {
                if (migratedCultureCodes.ContainsKey(cmsCulture.CultureCode)) continue;
                var langResult = await _importer.ImportAsync(new ContentLanguageModel
                {
                    ContentLanguageGUID = cmsCulture.CultureGuid,
                    ContentLanguageDisplayName = cmsCulture.CultureName,
                    ContentLanguageName = cmsCulture.CultureCode,
                    ContentLanguageIsDefault = true,
                    ContentLanguageFallbackContentLanguageGuid = null,
                    ContentLanguageCultureFormat = cmsCulture.CultureCode
                });

                if (langResult is { Success: true, Imported: ContentLanguageInfo importedLanguage })
                {
                    migratedCultureCodes.TryAdd(cmsCulture.CultureCode, importedLanguage);
                }
                else
                {
                    // TODO tomas.krch: 2023-11-08 define error
                    throw new Exception("");
                }
            }


            var homePagePath = KenticoHelper.GetSettingsKey(_kx13ContextFactory, kx13CmsSite.SiteId, SettingsKeys.CMSHomePagePath);
            var cookieLevel = KenticoHelper.GetSettingsKey(_kx13ContextFactory, kx13CmsSite.SiteId, SettingsKeys.CMSDefaultCookieLevel) switch
            {
                "all" => CookieLevelConstants.ALL,
                "visitor" => CookieLevelConstants.VISITOR,
                "editor" => CookieLevelConstants.EDITOR,
                "system" => CookieLevelConstants.SYSTEM,
                "essential" => CookieLevelConstants.ESSENTIAL,
                _ => (int?)null
            };
            var storeFormerUrls = KenticoHelper.GetSettingsKey(_kx13ContextFactory, kx13CmsSite.SiteId, "CMSStoreFormerUrls") is string storeFormerUrlsStr
                ? bool.TryParse(storeFormerUrlsStr, out var sfu) ? (bool?)sfu : null
                : null;

            var channelResult = await _importer.ImportAsync(new ChannelModel
            {
                ChannelDisplayName = kx13CmsSite.SiteDisplayName,
                ChannelName = kx13CmsSite.SiteName, // TODO tomas.krch: 2023-11-08 check codename validity
                ChannelGUID = kx13CmsSite.SiteGuid,
                ChannelType = ChannelType.Website
            });

            if (channelResult.Imported is ChannelInfo chi)
            {
                // bug workaround - NEEDS CHECKUP IN UMT
                if (chi.ChannelType != ChannelType.Website)
                {
                    chi.ChannelType = ChannelType.Website;
                    chi.Update();
                }
            }

            var webSiteChannelResult = await _importer.ImportAsync(new WebsiteChannelModel
            {
                WebsiteChannelGUID = kx13CmsSite.SiteGuid,
                WebsiteChannelChannelGuid = kx13CmsSite.SiteGuid,
                WebsiteChannelDomain = kx13CmsSite.SiteDomainName,
                WebsiteChannelHomePage = homePagePath,
                WebsiteChannelPrimaryContentLanguageGuid = migratedCultureCodes[defaultCultureCode].ContentLanguageGUID,
                WebsiteChannelDefaultCookieLevel = cookieLevel,
                WebsiteChannelStoreFormerUrls = storeFormerUrls
            });

            // _primaryKeyMappingContext.SetMapping<CmsSite>(r => r.SiteId, kx13CmsSite.SiteId, channelInfo.ChannelID);

            // var channelInfo = ChannelInfoProvider.ProviderObject.Get(kx13CmsSite.SiteGuid);
            // var websiteChannelInfo = WebsiteChannelInfoProvider.ProviderObject.Get(kx13CmsSite.SiteGuid);
            // var xbkTarget = new SiteMapperResult(channelInfo, websiteChannelInfo);
            // _protocol.FetchedTarget(xbkTarget);
            //
            // var mapped = _channelMapper.Map(kx13CmsSite, xbkTarget);
            // _protocol.MappedTarget(mapped);
            //
            // // TODO tomas.krch: 2023-10-30 refactor needed,
            // // var observer = _importService.StartImport(new UmtModel[]
            // // {
            // //     // TODO tomas.krch: 2023-10-30 channel import model
            // //     new DataClassModel
            // //     {
            // //
            // //     }
            // // }.AsEnumerable(), new ImporterContext("[to be removed]", "[to be removed]"), null);
            // // await observer.ImportCompletedTask;
            //
            // if (mapped is { Success : true } result)
            // {
            //     var (webSiteChannelMappingResult, newInstance) = result;
            //     ArgumentNullException.ThrowIfNull(webSiteChannelMappingResult, nameof(webSiteChannelMappingResult));
            //
            //     (channelInfo, websiteChannelInfo) = webSiteChannelMappingResult;
            //
            //     ChannelInfoProvider.ProviderObject.Set(channelInfo);
            //     WebsiteChannelInfoProvider.ProviderObject.Set(websiteChannelInfo);
            //
            //     _protocol.Success(kx13CmsSite, webSiteChannelMappingResult, mapped);
            //     _logger.LogEntitySetAction(newInstance, webSiteChannelMappingResult);
            //
            //     // TODO tomas.krch: 2023-10-30 might be needed to set reference to web site channel id
            //     _primaryKeyMappingContext.SetMapping<CmsSite>(r => r.SiteId, kx13CmsSite.SiteId, channelInfo.ChannelID);
            // }
        }

        return new GenericCommandResult();
    }

    // private record MigratedLanguage(ContentLanguageInfo ContentLanguageInfo, List<int> SiteIds);
    // private async Task<MigratedLanguage[]> MigrateContentLanguagesAsync(CancellationToken cancellationToken)
    // {
    //     await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
    //
    //     string? siteDefaultCulture = null;
    //     var culturesMigrated = new Dictionary<string, KX13M.CmsCulture>(StringComparer.InvariantCultureIgnoreCase);
    //     string? defaultCulture = KenticoHelper.GetSettingsKey(_kx13ContextFactory, null, SettingsKeys.CMSDefaultCultureCode);
    //
    //     foreach (var kx13CmsSite in kx13Context.CmsSites.Include(s=> s.Cultures))
    //     {
    //         var siteCulture = KenticoHelper.GetSettingsKey(_kx13ContextFactory, kx13CmsSite.SiteId, SettingsKeys.CMSDefaultCultureCode);
    //         foreach (var cmsCulture in kx13CmsSite.Cultures)
    //         {
    //             culturesMigrated.TryAdd(cmsCulture.CultureCode, cmsCulture);
    //         }
    //     }
    //
    //     if (defaultCulture == null)
    //     {
    //         defaultCulture = "en-US";
    //         var fallback =
    //             await kx13Context.CmsCultures.FirstOrDefaultAsync(c => c.CultureCode == "en-US", cancellationToken: cancellationToken) ??
    //             await kx13Context.CmsCultures.FirstOrDefaultAsync(cancellationToken: cancellationToken) ??
    //             new CmsCulture
    //             {
    //                 CultureName = "English", CultureCode = "en-US", CultureShortName = "English", CultureGuid = new Guid("2C2FE26A-600B-4197-A1B6-A75E2DBCC337"),
    //             };
    //
    //         culturesMigrated.TryAdd(fallback.CultureCode, fallback);
    //     }
    //
    //     foreach (var (key, value) in culturesMigrated)
    //     {
    //         await _importer.ImportAsync(new ContentLanguageModel
    //         {
    //             ContentLanguageGUID = value.CultureGuid,
    //             ContentLanguageDisplayName = value.CultureName,
    //             ContentLanguageName = value.CultureCode,
    //             ContentLanguageIsDefault = string.Equals(siteDefaultCulture, value.CultureCode, StringComparison.InvariantCultureIgnoreCase),
    //             ContentLanguageFallbackContentLanguageGuid = null,
    //             ContentLanguageCultureFormat = value.CultureCode
    //         });
    //     }
    // }

    private string GetSiteCulture(CmsSite site)
    {
        // simplified logic from CMS.DocumentEngine.DefaultPreferredCultureEvaluator.Evaluate()
        // domain alias skipped, HttpContext logic skipped
        var siteCulture = site.SiteDefaultVisitorCulture
                          ?? KenticoHelper.GetSettingsKey(_kx13ContextFactory, site.SiteId, SettingsKeys.CMSDefaultCultureCode);

        return siteCulture
               ?? throw new InvalidOperationException("Unknown site culture");
    }
}