using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.KX13;
using Migration.Tool.KX13.Context;

namespace Migration.Tool.Core.KX13.Behaviors;

public class CommandConstraintBehavior<TRequest, TResponse>(
    ILogger<CommandConstraintBehavior<TRequest, TResponse>> logger,
    IMigrationProtocol protocol,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    ToolConfiguration toolConfiguration)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

            bool criticalCheckPassed = PerformChecks(request, kx13Context);
            if (!criticalCheckPassed)
            {
                return (TResponse)(CommandResult)new CommandCheckFailedResult(criticalCheckPassed);
            }
        }
        catch (Exception ex)
        {
            protocol.CommandError<TRequest, TResponse>(ex, request);
            logger.LogCritical(ex, "Error occured while checking command constraints");
            return (TResponse)(CommandResult)new CommandCheckFailedResult(false);
        }

        return await next();
    }

    private bool PerformChecks(TRequest request, KX13Context kx13Context)
    {
        bool criticalCheckPassed = true;
        // const string supportedVersion = "13.0.64";
        const string supportedVersion = "13.0.0";
        if (SemanticVersion.TryParse(supportedVersion, out var minimalVersion))
        {
            criticalCheckPassed &= CheckVersion(kx13Context, minimalVersion);
        }

        var sourceSites = kx13Context.CmsSites
            .Include(s => s.Cultures)
            .ToList();

        foreach (var site in sourceSites)
        {
            criticalCheckPassed &= CheckSite(sourceSites, site.SiteId);
        }

        if (request is ICultureReliantCommand cultureReliantCommand)
        {
            criticalCheckPassed &= CheckCulture(cultureReliantCommand, sourceSites);
        }

        // criticalCheckPassed &= CheckDbCollations();

        return criticalCheckPassed;
    }

    private bool CheckVersion(KX13Context kx13Context, SemanticVersion minimalVersion)
    {
        bool criticalCheckPassed = true;

        #region Check conclusion methods

        void UnableToReadVersionKey(string keyName)
        {
            logger.LogCritical("Unable to read CMS version (incorrect format) - SettingsKeyName '{Key}'. Ensure Kentico Xperience 13 version is at least '{SupportedVersion}'", keyName, minimalVersion.ToString());
            protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new { ErrorKind = "Settings key value incorrect format", SettingsKeyName = keyName, SupportedVersion = minimalVersion.ToString() }));
            criticalCheckPassed = false;
        }

        void VersionKeyNotFound(string keyName)
        {
            logger.LogCritical("CMS version not found - SettingsKeyName '{Key}'. Ensure Kentico Xperience 13 version is at least '{SupportedVersion}'", keyName, minimalVersion.ToString());
            protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new { ErrorKind = "Settings key not found", SettingsKeyName = keyName, SupportedVersion = minimalVersion.ToString() }));
            criticalCheckPassed = false;
        }

        void UpgradeNeeded(string keyName, string currentVersion)
        {
            logger.LogCritical("{Key} '{CurrentVersion}' is not supported for migration. Upgrade Kentico Xperience 13 to at least '{SupportedVersion}'", keyName, currentVersion, minimalVersion.ToString());
            protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new { CurrentVersion = currentVersion, SupportedVersion = minimalVersion.ToString() }));
            criticalCheckPassed = false;
        }

        void LowHotfix(string keyName, int currentHotfix)
        {
            logger.LogCritical("{Key} '{CurrentVersion}' hotfix is not supported for migration. Upgrade Kentico Xperience 13 to at least '{SupportedVersion}'", keyName, currentHotfix, minimalVersion.ToString());
            protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new { CurrentHotfix = currentHotfix.ToString(), SupportedVersion = minimalVersion.ToString() }));
            criticalCheckPassed = false;
        }

        #endregion

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSDataVersion) is { } cmsDataVersion)
        {
            if (SemanticVersion.TryParse(cmsDataVersion.KeyValue, out var cmsDataVer))
            {
                if (cmsDataVer.IsLesserThan(minimalVersion))
                {
                    UpgradeNeeded(SettingsKeys.CMSDataVersion, cmsDataVer.ToString());
                }
            }
            else
            {
                UnableToReadVersionKey(SettingsKeys.CMSDataVersion);
            }
        }
        else
        {
            VersionKeyNotFound(SettingsKeys.CMSDataVersion);
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSDBVersion) is { } cmsDbVersion)
        {
            if (SemanticVersion.TryParse(cmsDbVersion.KeyValue, out var cmsDataVer))
            {
                if (cmsDataVer.IsLesserThan(minimalVersion))
                {
                    UpgradeNeeded(SettingsKeys.CMSDBVersion, cmsDataVer.ToString());
                }
            }
            else
            {
                UnableToReadVersionKey(SettingsKeys.CMSDBVersion);
            }
        }
        else
        {
            VersionKeyNotFound(SettingsKeys.CMSDBVersion);
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSHotfixDataVersion) is { } cmsHotfixDataVersion)
        {
            if (int.TryParse(cmsHotfixDataVersion.KeyValue, out int version))
            {
                if (version < minimalVersion.Hotfix)
                {
                    LowHotfix(SettingsKeys.CMSHotfixDataVersion, version);
                }
            }
            else
            {
                UnableToReadVersionKey(SettingsKeys.CMSHotfixDataVersion);
            }
        }
        else
        {
            VersionKeyNotFound(SettingsKeys.CMSHotfixDataVersion);
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSHotfixVersion) is { } cmsHotfixVersion)
        {
            if (int.TryParse(cmsHotfixVersion.KeyValue, out int version))
            {
                if (version < minimalVersion.Hotfix)
                {
                    LowHotfix(SettingsKeys.CMSHotfixVersion, version);
                }
            }
            else
            {
                UnableToReadVersionKey(SettingsKeys.CMSHotfixVersion);
            }
        }
        else
        {
            VersionKeyNotFound(SettingsKeys.CMSHotfixVersion);
        }

        return criticalCheckPassed;
    }

    private bool CheckSite(List<KX13M.CmsSite> sourceSites, int sourceSiteId)
    {
        bool criticalCheckPassed = true;
        if (sourceSites.All(s => s.SiteId != sourceSiteId))
        {
            var supportedSites = sourceSites.Select(x => new { x.SiteName, x.SiteId }).ToArray();
            string supportedSitesStr = string.Join(", ", supportedSites.Select(x => x.ToString()));
            logger.LogCritical("Unable to find site with ID '{SourceSiteId}'. Check --siteId parameter. Supported sites: {SupportedSites}", sourceSiteId,
                supportedSitesStr);
            protocol.Append(HandbookReferences.CommandConstraintBroken("Site exists")
                .WithMessage("Check program argument '--siteId'")
                .WithData(new { sourceSiteId, AvailableSites = supportedSites }));
            criticalCheckPassed = false;
        }

        return criticalCheckPassed;
    }

    private bool CheckCulture(ICultureReliantCommand cultureReliantCommand, List<KX13M.CmsSite> sourceSites)
    {
        bool criticalCheckPassed = true;
        string cultureCode = cultureReliantCommand.CultureCode;
        var siteCultureLookup = sourceSites
            .ToDictionary(x => x.SiteId, x => x.Cultures.Select(s => s.CultureCode.ToLowerInvariant()));

        foreach (var site in sourceSites)
        {
            if (siteCultureLookup.TryGetValue(site.SiteId, out var value))
            {
                string[] siteCultures = value.ToArray();
                if (!siteCultures.Contains(cultureCode.ToLowerInvariant()))
                {
                    string supportedCultures = string.Join(", ", siteCultures);
                    logger.LogCritical("Unable to find culture '{Culture}' mapping to site '{SiteId}'. Check --culture parameter. Supported cultures for site: {SupportedCultures}", cultureCode, site.SiteId, supportedCultures);
                    protocol.Append(HandbookReferences.CommandConstraintBroken("Culture is mapped to site")
                        .WithMessage("Check program argument '--culture'")
                        .WithData(new { cultureCode, site.SiteId, SiteCultures = supportedCultures }));
                    criticalCheckPassed = false;
                }
            }
        }

        return criticalCheckPassed;
    }
}
