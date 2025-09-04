using CMS.ContentEngine;
using CMS.Websites;

using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Helpers;
using Migration.Tool.KX13;
using Migration.Tool.KX13.Context;
using Migration.Tool.KX13.Models;

namespace Migration.Tool.Core.KX13.Handlers;

// ReSharper disable once UnusedType.Global
public class MigrateSitesCommandHandler(
    ILogger<MigrateSitesCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IProtocol protocol,
    IImporter importer,
    ToolConfiguration toolConfiguration)
    : IRequestHandler<MigrateSitesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var migratedCultureCodes = new Dictionary<string, ContentLanguageInfo>(StringComparer.CurrentCultureIgnoreCase);
        var entityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<CmsSite>();
        var domainSanitizer = new WebsiteChannelDomainSanitizer(logger, WebsiteChannelInfo.Provider.Get());
        var existingChannels = ChannelInfo.Provider.Get();

        foreach (var kx13CmsSite in kx13Context.CmsSites.Include(s => s.Cultures))
        {
            protocol.FetchedSource(kx13CmsSite);
            logger.LogInformation("Migrating site {SiteName} with SiteGuid {SiteGuid}", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);
            if (existingChannels.Any(ch => ch.ChannelName == kx13CmsSite.SiteName))
            {
                logger.LogInformation("Site skipped. It already exists.");
                continue;
            }
            if (entityConfiguration.ExcludeCodeNames.Contains(kx13CmsSite.SiteName, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogInformation("Site excluded in settings");
                continue;
            }

            string defaultCultureCode = GetSiteCulture(kx13CmsSite);
            var migratedSiteCultures = kx13CmsSite.Cultures.ToList();
            if (!migratedSiteCultures.Any(x => x.CultureCode.Equals(defaultCultureCode, StringComparison.InvariantCultureIgnoreCase)))
            {
                await using var ctx = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
                if (ctx.CmsCultures.FirstOrDefault(c => c.CultureCode == defaultCultureCode) is { } defaultCulture)
                {
                    migratedSiteCultures.Add(defaultCulture);
                }
            }

            foreach (var cmsCulture in migratedSiteCultures)
            {
                var existing = ContentLanguageInfoProvider.ProviderObject.Get()
                    .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageCultureFormat), cmsCulture.CultureCode)
                    .FirstOrDefault();

                if (existing != null && existing.ContentLanguageGUID != cmsCulture.CultureGuid)
                {
                    existing.ContentLanguageGUID = cmsCulture.CultureGuid;
                    existing.Update();
                }

                if (migratedCultureCodes.ContainsKey(cmsCulture.CultureCode))
                {
                    continue;
                }

                var langResult = await importer.ImportAsync(new ContentLanguageModel
                {
                    ContentLanguageGUID = cmsCulture.CultureGuid,
                    ContentLanguageDisplayName = cmsCulture.CultureName,
                    ContentLanguageName = cmsCulture.CultureCode,
                    ContentLanguageIsDefault = string.Equals(cmsCulture.CultureCode, defaultCultureCode, StringComparison.InvariantCultureIgnoreCase),
                    ContentLanguageFallbackContentLanguageGuid = null,
                    ContentLanguageCultureFormat = cmsCulture.CultureCode
                });

                if (langResult is { Success: true, Imported: ContentLanguageInfo importedLanguage })
                {
                    migratedCultureCodes.TryAdd(cmsCulture.CultureCode, importedLanguage);
                    logger.LogTrace("Imported language {Language} from {Culture}", importedLanguage.ContentLanguageName, cmsCulture.CultureCode);
                }
            }


            string? homePagePath = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, SettingsKeys.CMSHomePagePath);
            int? cookieLevel = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, SettingsKeys.CMSDefaultCookieLevel) switch
            {
                "all" => CookieLevelConstants.ALL,
                "visitor" => CookieLevelConstants.VISITOR,
                "editor" => CookieLevelConstants.EDITOR,
                "system" => CookieLevelConstants.SYSTEM,
                "essential" => CookieLevelConstants.ESSENTIAL,
                _ => null
            };
            bool? storeFormerUrls = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, "CMSStoreFormerUrls") is string storeFormerUrlsStr
                ? bool.TryParse(storeFormerUrlsStr, out bool sfu) ? sfu : null
                : null;

            if (!domainSanitizer.GetCandidate(kx13CmsSite.SiteName, kx13CmsSite.SiteDomainName.Trim('/'), out var domainName))
            {
                continue;
            }

            var channelResult = await importer.ImportAsync(new ChannelModel { ChannelDisplayName = kx13CmsSite.SiteDisplayName, ChannelName = kx13CmsSite.SiteName, ChannelGUID = kx13CmsSite.SiteGuid, ChannelType = ChannelType.Website });

            var websiteChannelModel = new WebsiteChannelModel
            {
                WebsiteChannelGUID = kx13CmsSite.SiteGuid,
                WebsiteChannelChannelGuid = kx13CmsSite.SiteGuid,
                WebsiteChannelDomain = domainName,
                WebsiteChannelHomePage = homePagePath,
                WebsiteChannelPrimaryContentLanguageGuid = migratedCultureCodes[defaultCultureCode].ContentLanguageGUID,
                WebsiteChannelDefaultCookieLevel = cookieLevel,
                WebsiteChannelStoreFormerUrls = storeFormerUrls
            };
            var webSiteChannelResult = await importer.ImportAsync(websiteChannelModel);

            if (!webSiteChannelResult.Success)
            {
                if (webSiteChannelResult.ModelValidationResults != null)
                {
                    foreach (var mvr in webSiteChannelResult.ModelValidationResults)
                    {
                        logger.LogError("Invalid channel properties {Members}: {ErrorMessage}", string.Join(", ", mvr.MemberNames), mvr.ErrorMessage);
                    }
                }
                else
                {
                    logger.LogError(webSiteChannelResult.Exception, "Failed to migrate site");
                }

                return new CommandFailureResult();
            }

            if (webSiteChannelResult.Imported is WebsiteChannelInfo webSiteChannel)
            {
                domainSanitizer.CommitCandidate(domainName);
                string? cmsReCaptchaPublicKey = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, "CMSReCaptchaPublicKey");
                string? cmsReCaptchaPrivateKey = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, "CMSReCaptchaPrivateKey");

                WebsiteCaptchaSettingsInfo? reCaptchaSettings = null;
                string? cmsReCaptchaV3PrivateKey = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, "CMSReCaptchaV3PrivateKey");
                string? cmsRecaptchaV3PublicKey = KenticoHelper.GetSettingsKey(kx13ContextFactory, kx13CmsSite.SiteId, "CMSRecaptchaV3PublicKey");
                double? cmsRecaptchaV3Threshold = KenticoHelper.GetSettingsKey<double>(kx13ContextFactory, kx13CmsSite.SiteId, "CMSRecaptchaV3Threshold");

                if (!string.IsNullOrWhiteSpace(cmsReCaptchaV3PrivateKey) || !string.IsNullOrWhiteSpace(cmsRecaptchaV3PublicKey))
                {
                    reCaptchaSettings = new WebsiteCaptchaSettingsInfo
                    {
                        WebsiteCaptchaSettingsWebsiteChannelID = webSiteChannel.WebsiteChannelID,
                        WebsiteCaptchaSettingsReCaptchaSiteKey = cmsRecaptchaV3PublicKey,
                        WebsiteCaptchaSettingsReCaptchaSecretKey = cmsReCaptchaV3PrivateKey,
                        WebsiteCaptchaSettingsReCaptchaThreshold = cmsRecaptchaV3Threshold ?? 0.5d,
                        WebsiteCaptchaSettingsReCaptchaVersion = ReCaptchaVersion.ReCaptchaV3
                    };
                }

                if (!string.IsNullOrWhiteSpace(cmsReCaptchaPublicKey) || !string.IsNullOrWhiteSpace(cmsReCaptchaPrivateKey))
                {
                    if (reCaptchaSettings is not null)
                    {
                        logger.LogError("""
                                        Conflicting settings found, ReCaptchaV2 and ReCaptchaV3 is set simultaneously.
                                        Remove setting keys 'CMSReCaptchaPublicKey', 'CMSReCaptchaPrivateKey'
                                        or remove setting keys 'CMSReCaptchaV3PrivateKey', 'CMSRecaptchaV3PublicKey', 'CMSRecaptchaV3Threshold'.
                                        """);
                        throw new InvalidOperationException("Invalid ReCaptcha settings");
                    }

                    reCaptchaSettings = new WebsiteCaptchaSettingsInfo
                    {
                        WebsiteCaptchaSettingsWebsiteChannelID = webSiteChannel.WebsiteChannelID,
                        WebsiteCaptchaSettingsReCaptchaSiteKey = cmsReCaptchaPublicKey,
                        WebsiteCaptchaSettingsReCaptchaSecretKey = cmsReCaptchaPrivateKey,
                        WebsiteCaptchaSettingsReCaptchaVersion = ReCaptchaVersion.ReCaptchaV2
                    };
                }

                if (reCaptchaSettings != null)
                {
                    WebsiteCaptchaSettingsInfo.Provider.Set(reCaptchaSettings);
                }
            }
        }

        return new GenericCommandResult();
    }

    private string GetSiteCulture(CmsSite site)
    {
        // simplified logic from CMS.DocumentEngine.DefaultPreferredCultureEvaluator.Evaluate()
        // domain alias skipped, HttpContext logic skipped
        string? siteCulture = site.SiteDefaultVisitorCulture
                              ?? KenticoHelper.GetSettingsKey(kx13ContextFactory, site.SiteId, SettingsKeys.CMSDefaultCultureCode);

        return siteCulture
               ?? throw new InvalidOperationException("Unknown site culture");
    }
}
