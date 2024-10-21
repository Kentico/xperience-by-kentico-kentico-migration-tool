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
using Migration.Tool.Core.K11.Helpers;
using Migration.Tool.K11;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Handlers;

// ReSharper disable once UnusedType.Global
public class MigrateSitesCommandHandler(
    ILogger<MigrateSitesCommandHandler> logger,
    IDbContextFactory<K11Context> k11ContextFactory,
    IProtocol protocol,
    IImporter importer)
    : IRequestHandler<MigrateSitesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);
        var migratedCultureCodes = new Dictionary<string, ContentLanguageInfo>(StringComparer.CurrentCultureIgnoreCase);
        int fallbackDomainPort = 5000;
        var migratedDomains = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        foreach (var k11CmsSite in k11Context.CmsSites.Include(s => s.Cultures).Include(cmsSite => cmsSite.CmsSiteDomainAliases))
        {
            protocol.FetchedSource(k11CmsSite);
            logger.LogTrace("Migrating site {SiteName} with SiteGuid {SiteGuid}", k11CmsSite.SiteName, k11CmsSite.SiteGuid);

            string defaultCultureCode = GetSiteCulture(k11CmsSite);
            var migratedSiteCultures = k11CmsSite.Cultures.ToList();
            if (!migratedSiteCultures.Any(x => x.CultureCode.Equals(defaultCultureCode, StringComparison.InvariantCultureIgnoreCase)))
            {
                await using var ctx = await k11ContextFactory.CreateDbContextAsync(cancellationToken);
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
                    ContentLanguageIsDefault = true,
                    ContentLanguageFallbackContentLanguageGuid = null,
                    ContentLanguageCultureFormat = cmsCulture.CultureCode
                });

                if (langResult is { Success: true, Imported: ContentLanguageInfo importedLanguage })
                {
                    migratedCultureCodes.TryAdd(cmsCulture.CultureCode, importedLanguage);
                    logger.LogTrace("Imported language {Language} from {Culture}", importedLanguage.ContentLanguageName, cmsCulture.CultureCode);
                }
            }

            // TODO tomas.krch 2024-02-23: treepath migration when upgrade to recent XbyK occurs
            string? homePageNodeAliasPath = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, SettingsKeys.CMSDefaultAliasPath);
            int? cookieLevel = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, SettingsKeys.CMSDefaultCookieLevel) switch
            {
                "all" => CookieLevelConstants.ALL,
                "visitor" => CookieLevelConstants.VISITOR,
                "editor" => CookieLevelConstants.EDITOR,
                "system" => CookieLevelConstants.SYSTEM,
                "essential" => CookieLevelConstants.ESSENTIAL,
                _ => null
            };
            bool? storeFormerUrls = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, "CMSStoreFormerUrls") is { } storeFormerUrlsStr
                ? bool.TryParse(storeFormerUrlsStr, out bool sfu) ? sfu : null
                : true;

            var result = UriHelper.GetUniqueDomainCandidate(
                k11CmsSite.SiteDomainName,
                ref fallbackDomainPort,
                candidate => !migratedDomains.Contains(candidate)
            );

            string webSiteChannelDomain;
            switch (result)
            {
                case (true, false, var candidate, null):
                {
                    webSiteChannelDomain = candidate;
                    break;
                }
                case (true, true, var candidate, null):
                {
                    webSiteChannelDomain = candidate;
                    logger.LogWarning("Domain '{Domain}' of site '{SiteName}' is not unique. '{Fallback}' is used instead", k11CmsSite.SiteDomainName, k11CmsSite.SiteName, candidate);
                    protocol.Warning(HandbookReferences
                        .InvalidSourceData<CmsSite>()
                        .WithMessage($"Domain '{k11CmsSite.SiteDomainName}' of site '{k11CmsSite.SiteName}' is not unique. '{candidate}' is used instead"), k11CmsSite);
                    break;
                }
                case { Success: false, Fallback: { } fallback }:
                {
                    webSiteChannelDomain = fallback;
                    logger.LogWarning("Unable to use domain '{Domain}' of site '{SiteName}' as channel domain. Fallback '{Fallback}' is used", k11CmsSite.SiteDomainName, k11CmsSite.SiteName, fallback);
                    protocol.Warning(HandbookReferences
                        .InvalidSourceData<CmsSite>()
                        .WithMessage($"Non-unique domain name '{k11CmsSite.SiteDomainName}', fallback '{fallback}' used"), k11CmsSite);
                    break;
                }
                default:
                {
                    logger.LogError("Unable to use domain '{Domain}' of site '{SiteName}' as channel domain. No fallback available, skipping site", k11CmsSite.SiteDomainName, k11CmsSite.SiteName);
                    protocol.Warning(HandbookReferences
                        .InvalidSourceData<CmsSite>()
                        .WithMessage($"Invalid domain name for migration '{k11CmsSite.SiteDomainName}'"), k11CmsSite);
                    continue;
                }
            }

            await importer.ImportAsync(new ChannelModel { ChannelDisplayName = k11CmsSite.SiteDisplayName, ChannelName = k11CmsSite.SiteName, ChannelGUID = k11CmsSite.SiteGuid, ChannelType = ChannelType.Website });

            var webSiteChannelResult = await importer.ImportAsync(new WebsiteChannelModel
            {
                WebsiteChannelGUID = k11CmsSite.SiteGuid,
                WebsiteChannelChannelGuid = k11CmsSite.SiteGuid,
                WebsiteChannelDomain = webSiteChannelDomain,
                WebsiteChannelHomePage = homePageNodeAliasPath ?? "/",
                WebsiteChannelPrimaryContentLanguageGuid = migratedCultureCodes[defaultCultureCode].ContentLanguageGUID,
                WebsiteChannelDefaultCookieLevel = cookieLevel,
                WebsiteChannelStoreFormerUrls = storeFormerUrls
            });

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
                migratedDomains.Add(webSiteChannelDomain);

                string? cmsReCaptchaPublicKey = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, "CMSReCaptchaPublicKey");
                string? cmsReCaptchaPrivateKey = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, "CMSReCaptchaPrivateKey");

                WebsiteCaptchaSettingsInfo? reCaptchaSettings = null;
                string? cmsReCaptchaV3PrivateKey = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, "CMSReCaptchaV3PrivateKey");
                string? cmsRecaptchaV3PublicKey = KenticoHelper.GetSettingsKey(k11ContextFactory, k11CmsSite.SiteId, "CMSRecaptchaV3PublicKey");
                double? cmsRecaptchaV3Threshold = KenticoHelper.GetSettingsKey<double>(k11ContextFactory, k11CmsSite.SiteId, "CMSRecaptchaV3Threshold");

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
        string? siteCulture = site.SiteDefaultVisitorCulture.NullIf(string.Empty)
                              ?? KenticoHelper.GetSettingsKey(k11ContextFactory, site.SiteId, SettingsKeys.CMSDefaultCultureCode);

        return siteCulture
               ?? throw new InvalidOperationException("Unknown site culture");
    }
}
