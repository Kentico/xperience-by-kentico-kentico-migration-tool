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
    IImporter importer,
    ToolConfiguration toolConfiguration)
    : IRequestHandler<MigrateSitesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateSitesCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);
        var migratedCultureCodes = new Dictionary<string, ContentLanguageInfo>(StringComparer.CurrentCultureIgnoreCase);
        var migratedDomains = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        var entityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<CmsSite>();
        var domainSanitizer = new WebsiteChannelDomainSanitizer(logger, WebsiteChannelInfo.Provider.Get());
        var existingChannels = ChannelInfo.Provider.Get();

        foreach (var k11CmsSite in k11Context.CmsSites.Include(s => s.Cultures).Include(cmsSite => cmsSite.CmsSiteDomainAliases))
        {
            protocol.FetchedSource(k11CmsSite);
            logger.LogInformation("Migrating site {SiteName} with SiteGuid {SiteGuid}", k11CmsSite.SiteName, k11CmsSite.SiteGuid);
            if (existingChannels.Any(ch => ch.ChannelName == k11CmsSite.SiteName))
            {
                logger.LogInformation("Site skipped. It already exists.");
                continue;
            }
            if (entityConfiguration.ExcludeCodeNames.Contains(k11CmsSite.SiteName, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogInformation("Site excluded in settings");
                continue;
            }

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
                if (migratedCultureCodes.ContainsKey(cmsCulture.CultureCode))
                {
                    continue;
                }

                var existing = ContentLanguageInfoProvider.ProviderObject.Get()
                    .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageCultureFormat), cmsCulture.CultureCode)
                    .FirstOrDefault();

                if (existing != null)
                {
                    if (existing.ContentLanguageGUID != cmsCulture.CultureGuid)
                    {
                        existing.ContentLanguageGUID = cmsCulture.CultureGuid;
                        existing.Update();
                    }
                    migratedCultureCodes.TryAdd(cmsCulture.CultureCode, existing);
                }
                else
                {
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

            if (!domainSanitizer.GetCandidate(k11CmsSite.SiteName, k11CmsSite.SiteDomainName, out var domainName))
            {
                continue;
            }

            await importer.ImportAsync(new ChannelModel { ChannelDisplayName = k11CmsSite.SiteDisplayName, ChannelName = k11CmsSite.SiteName, ChannelGUID = k11CmsSite.SiteGuid, ChannelType = ChannelType.Website });

            var webSiteChannelResult = await importer.ImportAsync(new WebsiteChannelModel
            {
                WebsiteChannelGUID = k11CmsSite.SiteGuid,
                WebsiteChannelChannelGuid = k11CmsSite.SiteGuid,
                WebsiteChannelDomain = domainName,
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
                domainSanitizer.CommitCandidate(domainName);
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
