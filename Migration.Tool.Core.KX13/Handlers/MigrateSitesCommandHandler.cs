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
        var domainsConfigCollector = new WebsiteChannelDomainsConfigCollector();
        var existingChannels = ChannelInfo.Provider.Get();

        foreach (var kx13CmsSite in kx13Context.CmsSites.Include(s => s.Cultures).Include(s => s.CmsSiteDomainAliases))
        {
            protocol.FetchedSource(kx13CmsSite);
            logger.LogInformation("Migrating site {SiteName} with SiteGuid {SiteGuid}", kx13CmsSite.SiteName, kx13CmsSite.SiteGuid);
            if (existingChannels.FirstOrDefault(ch => ch.ChannelName == kx13CmsSite.SiteName) is { } existingChannel)
            {
                logger.LogInformation("Site skipped. It already exists.");
                string existingSiteDefaultCulture = GetSiteCulture(kx13CmsSite);
                var (existingCultureBoundAliases, existingPlainAliases) = AnalyzeDomainAliases(kx13CmsSite, existingSiteDefaultCulture);
                bool sourceUsesLanguageDomains = existingCultureBoundAliases.Count > 0;
                var existingWebsiteChannel = WebsiteChannelInfo.Provider.Get()
                    .WhereEquals(nameof(WebsiteChannelInfo.WebsiteChannelChannelID), existingChannel.ChannelID)
                    .FirstOrDefault();
                WarnOnLanguageRoutingModeMismatch(existingChannel, existingWebsiteChannel, sourceUsesLanguageDomains);

                // regenerate the suggestion for existing sites too, reusing the stored (already sanitized) channel domain
                CollectDomainConfiguration(
                    domainsConfigCollector,
                    kx13CmsSite,
                    sourceUsesLanguageDomains,
                    existingCultureBoundAliases,
                    existingPlainAliases,
                    existingWebsiteChannel?.WebsiteChannelDomain.NullIf(string.Empty),
                    existingSiteDefaultCulture,
                    migratedCultureCodes);
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
                if (migratedCultureCodes.ContainsKey(cmsCulture.CultureCode))
                {
                    continue;
                }

                var existing = ContentLanguageInfo.Provider.Get()
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

            var (cultureBoundAliases, plainAliases) = AnalyzeDomainAliases(kx13CmsSite, defaultCultureCode);

            // a live site domain alias with its own default visitor culture means the source site served languages on
            // dedicated domains => the channel is created in the language-domains routing mode (LSD)
            bool useLanguageDomains = cultureBoundAliases.Count > 0;
            string? domainName = null;
            if (useLanguageDomains)
            {
                // a language-domains channel stores no domain, so the (administration) site domain name is not sanitized nor reserved
                logger.LogInformation(
                    "Site '{SiteName}' uses culture-specific live site domain aliases ({Aliases}) - the website channel will be created in the language-domains routing mode (requires Xperience by Kentico 31.9.0 or newer)",
                    kx13CmsSite.SiteName,
                    string.Join(", ", cultureBoundAliases.Select(a => $"{a.SiteDomainPresentationUrl}={a.SiteDefaultVisitorCulture}")));
            }
            else if (!domainSanitizer.GetCandidate(kx13CmsSite.SiteName, kx13CmsSite.SiteDomainName.Trim('/'), out domainName))
            {
                continue;
            }

            // UMT upserts the channel by GUID - remember whether it existed, so a failed website channel import never removes a pre-existing channel
            bool channelExistedBefore = ChannelInfo.Provider.Get()
                .WhereEquals(nameof(ChannelInfo.ChannelGUID), kx13CmsSite.SiteGuid)
                .Any();

            var channelResult = await importer.ImportAsync(new ChannelModel { ChannelDisplayName = kx13CmsSite.SiteDisplayName, ChannelName = kx13CmsSite.SiteName, ChannelGUID = kx13CmsSite.SiteGuid, ChannelType = ChannelType.Website });

            var websiteChannelModel = new WebsiteChannelModel
            {
                WebsiteChannelGUID = kx13CmsSite.SiteGuid,
                WebsiteChannelChannelGuid = kx13CmsSite.SiteGuid,
                WebsiteChannelHomePage = homePagePath,
                WebsiteChannelDefaultCookieLevel = cookieLevel,
                WebsiteChannelStoreFormerUrls = storeFormerUrls
            };
            if (useLanguageDomains)
            {
                // a language-domains channel must not carry a stored domain or a primary language (UMT 4.5.0 validation):
                // per-language domains live in the application's WebsiteChannelDomainOptions and the language comes from the host
                websiteChannelModel.WebsiteChannelLanguageRoutingMode = WebsiteChannelLanguageRoutingMode.LanguageDomains;
            }
            else
            {
                websiteChannelModel.WebsiteChannelDomain = domainName;
                websiteChannelModel.WebsiteChannelPrimaryContentLanguageGuid = migratedCultureCodes[defaultCultureCode].ContentLanguageGUID;
            }
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

                // a channel without its website channel breaks the Channel management UI - remove the channel
                // imported a moment ago so a failed run does not leave the target in a broken state
                if (!channelExistedBefore && channelResult.Imported is ChannelInfo orphanedChannel)
                {
                    try
                    {
                        ChannelInfo.Provider.Delete(orphanedChannel);
                        logger.LogWarning("Channel '{ChannelName}' was removed because the import of its website channel failed", orphanedChannel.ChannelName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to remove channel '{ChannelName}' after its website channel import failed - remove it manually, otherwise the administration fails when loading the channel", orphanedChannel.ChannelName);
                    }
                }

                return new CommandFailureResult();
            }

            if (webSiteChannelResult.Imported is WebsiteChannelInfo webSiteChannel)
            {
                domainSanitizer.CommitCandidate(domainName);

                CollectDomainConfiguration(domainsConfigCollector, kx13CmsSite, useLanguageDomains, cultureBoundAliases, plainAliases, domainName, defaultCultureCode, migratedCultureCodes);

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

        await EmitDomainsConfigSuggestion(domainsConfigCollector, cancellationToken);

        return new GenericCommandResult();
    }

    private void CollectDomainConfiguration(
        WebsiteChannelDomainsConfigCollector collector,
        CmsSite site,
        bool useLanguageDomains,
        IReadOnlyCollection<CmsSiteDomainAlias> cultureBoundAliases,
        IReadOnlyCollection<CmsSiteDomainAlias> plainAliases,
        string? storedChannelDomain,
        string defaultCultureCode,
        IReadOnlyDictionary<string, ContentLanguageInfo> migratedCultureCodes)
    {
        // keys of the generated configuration must be target content language code names (per XbyK docs);
        // when the site was skipped as already migrated, the per-run dictionary is empty - resolve against the target
        string? TryResolveLanguageName(string cultureCode) =>
            migratedCultureCodes.TryGetValue(cultureCode, out var language)
                ? language.ContentLanguageName
                : ContentLanguageInfo.Provider.Get()
                    .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageCultureFormat), cultureCode)
                    .FirstOrDefault()?.ContentLanguageName;

        if (useLanguageDomains)
        {
            string defaultLanguageName = TryResolveLanguageName(defaultCultureCode) ?? defaultCultureCode;

            // the site presentation URL is the live site domain of the default language - KX13 live sites run only on
            // presentation URLs, the site domain name is the administration domain and is never used as a live site domain.
            // Plain live site aliases become inbound-only aliases.
            if (UriHelper.TryNormalizeDomain(site.SitePresentationUrl, out string sitePresentationDomain))
            {
                collector.AddLanguageDomain(site.SiteName, defaultLanguageName, sitePresentationDomain);
            }
            else
            {
                logger.LogWarning(
                    "Site '{SiteName}' has no valid presentation URL ('{PresentationUrl}'), so the generated configuration contains no main domain for the default language '{Language}'. Add the domain to the configuration manually",
                    site.SiteName, site.SitePresentationUrl, defaultLanguageName);
            }
            foreach (var alias in plainAliases)
            {
                if (TryGetAliasDomain(alias, out string normalized))
                {
                    collector.AddLanguageDomain(site.SiteName, defaultLanguageName, normalized);
                }
            }

            foreach (var alias in cultureBoundAliases)
            {
                string cultureCode = alias.SiteDefaultVisitorCulture!;
                string? languageName = TryResolveLanguageName(cultureCode);
                if (languageName is null)
                {
                    logger.LogWarning(
                        "Domain alias '{Alias}' of site '{SiteName}' is bound to culture '{Culture}' which matches no content language in the target instance. The domain is included in the generated configuration, but Xperience by Kentico will report it as unknown until such content language exists",
                        alias.SiteDomainPresentationUrl, site.SiteName, cultureCode);
                }

                if (TryGetAliasDomain(alias, out string normalized))
                {
                    collector.AddLanguageDomain(site.SiteName, languageName ?? cultureCode, normalized);
                }
            }
        }
        else if (plainAliases.Count > 0)
        {
            // domain aliases have no database counterpart in Xperience by Kentico - offer them as configuration-based domain overrides
            // the stored channel domain stays the main domain - DomainOverrides take precedence over the stored domain
            if (storedChannelDomain is not null)
            {
                collector.AddDomainOverride(site.SiteName, storedChannelDomain);
            }
            foreach (var alias in plainAliases)
            {
                if (TryGetAliasDomain(alias, out string normalized))
                {
                    collector.AddDomainOverride(site.SiteName, normalized);
                }
            }
        }
    }

    /// <summary>
    /// Resolves the live-site domain of a KX13 live site domain alias - its presentation URL.
    /// </summary>
    private static bool TryGetAliasDomain(CmsSiteDomainAlias alias, out string domain) =>
        UriHelper.TryNormalizeDomain(alias.SiteDomainPresentationUrl, out domain);

    private async Task EmitDomainsConfigSuggestion(WebsiteChannelDomainsConfigCollector collector, CancellationToken cancellationToken)
    {
        // the CLI switches its working directory to XbyKDirPath on startup, so the file lands next to the target project's appsettings.json
        string filePath = Path.GetFullPath("WebsiteChannelDomains.suggested.json");
        if (!collector.HasAny)
        {
            // the file is derived output - a leftover from a previous run would present removed domains as current
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                logger.LogInformation("Stale '{FilePath}' from a previous run was removed - the current source configuration yields no domain suggestions", filePath);
            }

            return;
        }

        string json = collector.BuildJson();
        await File.WriteAllTextAsync(filePath, json, cancellationToken);

        logger.LogWarning(
            """
            Migrated sites use domain aliases. Xperience by Kentico stores language-specific domains and domain overrides only in application configuration, so they cannot be written to the target database.
            A suggested "WebsiteChannelDomains" configuration section was written to '{FilePath}'.
            Merge it into the target application's appsettings.json (per environment) and bind it in the application startup: builder.Services.Configure<WebsiteChannelDomainOptions>(builder.Configuration.GetSection("WebsiteChannelDomains"));
            The first domain of each language is used as the canonical domain for generated absolute URLs; remaining domains are inbound-only aliases.
            {Json}
            """,
            filePath, json);
    }

    /// <summary>
    /// Classifies the site's live site domain aliases into culture-bound ones (bound to a non-default visitor culture - these
    /// switch the channel to the language-domains routing mode) and plain ones, warning about aliases without a valid
    /// presentation URL. Administration domain aliases are ignored. Aliases are ordered by ID, so the first domain of
    /// each language is stable across runs.
    /// </summary>
    private (List<CmsSiteDomainAlias> CultureBoundAliases, List<CmsSiteDomainAlias> PlainAliases) AnalyzeDomainAliases(CmsSite kx13CmsSite, string defaultCultureCode)
    {
        var liveSiteAliases = kx13CmsSite.CmsSiteDomainAliases
            .Where(a => a.SiteDomainAliasType == 1)    // live site aliases; 0 = administration aliases
            .ToList();

        int administrationAliasCount = kx13CmsSite.CmsSiteDomainAliases.Count - liveSiteAliases.Count;
        if (administrationAliasCount > 0)
        {
            logger.LogDebug("{Count} administration domain alias(es) of site '{SiteName}' skipped - only live site domain aliases are migrated", administrationAliasCount, kx13CmsSite.SiteName);
        }

        foreach (var unusable in liveSiteAliases.Where(a => !TryGetAliasDomain(a, out _)))
        {
            logger.LogWarning(
                "Live site domain alias (ID {AliasId}, visitor culture '{VisitorCulture}') of site '{SiteName}' has no valid presentation URL ('{PresentationUrl}') and was skipped - a culture-bound alias would otherwise switch the channel to the language-domains routing mode",
                unusable.SiteDomainAliasId, unusable.SiteDefaultVisitorCulture, kx13CmsSite.SiteName, unusable.SiteDomainPresentationUrl);
        }

        var domainAliases = liveSiteAliases
            .Where(a => TryGetAliasDomain(a, out _))
            .OrderBy(a => a.SiteDomainAliasId)
            .ToList();
        var cultureBoundAliases = domainAliases
            .Where(a => !string.IsNullOrWhiteSpace(a.SiteDefaultVisitorCulture)
                        && !string.Equals(a.SiteDefaultVisitorCulture, defaultCultureCode, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        var plainAliases = domainAliases.Except(cultureBoundAliases).ToList();
        return (cultureBoundAliases, plainAliases);
    }

    /// <summary>
    /// An existing channel is never updated, so a routing mode decided by a previous run cannot be corrected by re-running
    /// the migration. Without this warning a mismatch (e.g. culture-bound aliases added or fixed in the source after the
    /// first run) would go unnoticed - pages migration follows the channel's stored mode, not the source configuration.
    /// </summary>
    private void WarnOnLanguageRoutingModeMismatch(ChannelInfo existingChannel, WebsiteChannelInfo? websiteChannel, bool sourceUsesLanguageDomains)
    {
        if (websiteChannel is null)
        {
            return;
        }

        bool targetUsesLanguageDomains = websiteChannel.WebsiteChannelLanguageRoutingMode == WebsiteChannelLanguageRoutingMode.LanguageDomains;
        if (sourceUsesLanguageDomains != targetUsesLanguageDomains)
        {
            logger.LogWarning(
                "Existing website channel '{ChannelName}' uses language routing mode '{TargetMode}', but the source site configuration implies '{ExpectedMode}'. " +
                "Existing channels are not updated by the migration - to get the expected routing mode, migrate the site into an empty target instance, " +
                "or align the channel manually (CMS_WebsiteChannel.WebsiteChannelLanguageRoutingMode) together with its stored URL paths.",
                existingChannel.ChannelName,
                targetUsesLanguageDomains ? WebsiteChannelLanguageRoutingMode.LanguageDomains : WebsiteChannelLanguageRoutingMode.PathPrefix,
                sourceUsesLanguageDomains ? WebsiteChannelLanguageRoutingMode.LanguageDomains : WebsiteChannelLanguageRoutingMode.PathPrefix);
        }
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
