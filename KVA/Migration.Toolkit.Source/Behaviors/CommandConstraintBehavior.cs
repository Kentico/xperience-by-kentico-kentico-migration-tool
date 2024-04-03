namespace Migration.Toolkit.Source.Behaviors;

using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Source.Model;

public class CommandConstraintBehavior<TRequest, TResponse>(
    ILogger<CommandConstraintBehavior<TRequest, TResponse>> logger,
    IMigrationProtocol protocol,
    ToolkitConfiguration toolkitConfiguration,
    ModelFacade modelFacade)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            var criticalCheckPassed = PerformChecks(request);
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

    private bool PerformChecks(TRequest request)
    {
        var criticalCheckPassed = true;
        // const string supportedVersion = "13.0.64";
        // const string supportedVersion = "13.0.0";
        // if (SemanticVersion.TryParse(supportedVersion, out var minimalVersion))
        // {
        //     criticalCheckPassed &= CheckVersion(minimalVersion);
        // }

        // var sites = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId);
        var sourceSites = modelFacade.SelectAll<ICmsSite>()
            .ToList();

        foreach (var site in sourceSites)
        {
            criticalCheckPassed &= CheckSite(sourceSites, site.SiteID);
        }


        if (request is ICultureReliantCommand cultureReliantCommand)
        {
            var cultures = modelFacade.SelectAll<ICmsCulture>().ToList();
            var siteCultures = modelFacade.SelectAll<ICmsSiteCulture>().ToList();
            criticalCheckPassed &= CheckCulture(cultureReliantCommand, sourceSites, cultures, siteCultures);
        }

        // criticalCheckPassed &= CheckDbCollations();

        return criticalCheckPassed;
    }

    // private bool CheckVersion(KX13Context kx13Context, SemanticVersion minimalVersion)
    // {
    //     var criticalCheckPassed = true;
    //
    //     #region Check conclusion methods
    //
    //     void UnableToReadVersionKey(string keyName)
    //     {
    //         logger.LogCritical("Unable to read CMS version (incorrect format) - SettingsKeyName '{Key}'. Ensure Kentico version is at least '{SupportedVersion}'", keyName, minimalVersion.ToString());
    //         protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
    //         {
    //             ErrorKind = "Settings key value incorrect format",
    //             SettingsKeyName = keyName,
    //             SupportedVersion = minimalVersion.ToString()
    //         }));
    //         criticalCheckPassed = false;
    //     }
    //
    //     void VersionKeyNotFound(string keyName)
    //     {
    //         logger.LogCritical("CMS version not found - SettingsKeyName '{Key}'. Ensure Kentico version is at least '{SupportedVersion}'", keyName, minimalVersion.ToString());
    //         protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
    //         {
    //             ErrorKind = "Settings key not found",
    //             SettingsKeyName = keyName,
    //             SupportedVersion = minimalVersion.ToString()
    //         }));
    //         criticalCheckPassed = false;
    //     }
    //
    //     void UpgradeNeeded(string keyName, string currentVersion)
    //     {
    //         logger.LogCritical("{Key} '{CurrentVersion}' is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'", keyName, currentVersion, minimalVersion.ToString());
    //         protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
    //         {
    //             CurrentVersion = currentVersion,
    //             SupportedVersion = minimalVersion.ToString()
    //         }));
    //         criticalCheckPassed = false;
    //     }
    //
    //     void LowHotfix(string keyName, int currentHotfix)
    //     {
    //         logger.LogCritical("{Key} '{CurrentVersion}' hotfix is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'", keyName, currentHotfix, minimalVersion.ToString());
    //         protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
    //         {
    //             CurrentHotfix = currentHotfix.ToString(),
    //             SupportedVersion = minimalVersion.ToString()
    //         }));
    //         criticalCheckPassed = false;
    //     }
    //
    //     #endregion
    //
    //     if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSDataVersion) is { } cmsDataVersion)
    //     {
    //         if (SemanticVersion.TryParse(cmsDataVersion.KeyValue, out var cmsDataVer))
    //         {
    //             if (cmsDataVer.IsLesserThan(minimalVersion))
    //             {
    //                 UpgradeNeeded(SettingsKeys.CMSDataVersion, cmsDataVer.ToString());
    //             }
    //         }
    //         else
    //         {
    //             UnableToReadVersionKey(SettingsKeys.CMSDataVersion);
    //         }
    //     }
    //     else
    //     {
    //         VersionKeyNotFound(SettingsKeys.CMSDataVersion);
    //     }
    //
    //     if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSDBVersion) is { } cmsDbVersion)
    //     {
    //         if (SemanticVersion.TryParse(cmsDbVersion.KeyValue, out var cmsDataVer))
    //         {
    //             if (cmsDataVer.IsLesserThan(minimalVersion))
    //             {
    //                 UpgradeNeeded(SettingsKeys.CMSDBVersion, cmsDataVer.ToString());
    //             }
    //         }
    //         else
    //         {
    //             UnableToReadVersionKey(SettingsKeys.CMSDBVersion);
    //         }
    //     }
    //     else
    //     {
    //         VersionKeyNotFound(SettingsKeys.CMSDBVersion);
    //     }
    //
    //     if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSHotfixDataVersion) is { } cmsHotfixDataVersion)
    //     {
    //         if (int.TryParse(cmsHotfixDataVersion.KeyValue, out var version))
    //         {
    //             if (version < minimalVersion.Hotfix)
    //             {
    //                 LowHotfix(SettingsKeys.CMSHotfixDataVersion, version);
    //             }
    //         }
    //         else
    //         {
    //             UnableToReadVersionKey(SettingsKeys.CMSHotfixDataVersion);
    //         }
    //     }
    //     else
    //     {
    //         VersionKeyNotFound(SettingsKeys.CMSHotfixDataVersion);
    //     }
    //
    //     if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == SettingsKeys.CMSHotfixVersion) is { } cmsHotfixVersion)
    //     {
    //         if (int.TryParse(cmsHotfixVersion.KeyValue, out var version))
    //         {
    //             if (version < minimalVersion.Hotfix)
    //             {
    //                 LowHotfix(SettingsKeys.CMSHotfixVersion, version);
    //             }
    //         }
    //         else
    //         {
    //             UnableToReadVersionKey(SettingsKeys.CMSHotfixVersion);
    //         }
    //     }
    //     else
    //     {
    //         VersionKeyNotFound(SettingsKeys.CMSHotfixVersion);
    //     }
    //
    //     return criticalCheckPassed;
    // }

    private bool CheckSite(List<ICmsSite> sourceSites, int sourceSiteId)
    {
        var criticalCheckPassed = true;
        if (sourceSites.All(s => s.SiteID != sourceSiteId))
        {
            var supportedSites = sourceSites.Select(x => new
            {
                x.SiteName,
                x.SiteID
            }).ToArray();
            var supportedSitesStr = string.Join(", ", supportedSites.Select(x => x.ToString()));
            logger.LogCritical("Unable to find site with ID '{SourceSiteId}'. Check --siteId parameter. Supported sites: {SupportedSites}", sourceSiteId,
                supportedSitesStr);
            protocol.Append(HandbookReferences.CommandConstraintBroken("Site exists")
                .WithMessage("Check program argument '--siteId'")
                .WithData(new
                {
                    sourceSiteId,
                    AvailableSites = supportedSites
                }));
            criticalCheckPassed = false;
        }

        return criticalCheckPassed;
    }

    private bool CheckCulture(ICultureReliantCommand cultureReliantCommand, List<ICmsSite> sourceSites, List<ICmsCulture> cultures, List<ICmsSiteCulture> cmsSiteCultures)
    {

        var criticalCheckPassed = true;
        var cultureCode = cultureReliantCommand.CultureCode;

        foreach (var site in sourceSites)
        {
            var siteCultures = cmsSiteCultures
                .Where(x => x.SiteID == site.SiteID)
                .Select(x => cultures.FirstOrDefault(c => c.CultureID == x.CultureID)?.CultureCode)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (!siteCultures.Contains(cultureCode.ToLowerInvariant()))
            {
                var supportedCultures = string.Join(", ", siteCultures);
                logger.LogCritical("Unable to find culture '{Culture}' mapping to site '{SiteId}'. Check --culture parameter. Supported cultures for site: {SupportedCultures}", cultureCode,
                    site.SiteID, supportedCultures);
                protocol.Append(HandbookReferences.CommandConstraintBroken("Culture is mapped to site")
                    .WithMessage("Check program argument '--culture'")
                    .WithData(new { cultureCode, site.SiteID, SiteCultures = supportedCultures }));
                criticalCheckPassed = false;
            }
        }

        return criticalCheckPassed;
    }

    // TODO tk: 2022-11-02 create global rule
    private bool CheckDbCollations()
    {
        var kxCollation = GetDbCollationName(toolkitConfiguration.KxConnectionString ?? throw new InvalidOperationException("KxConnectionString is required"));
        var xbkCollation = GetDbCollationName(toolkitConfiguration.XbKConnectionString ?? throw new InvalidOperationException("XbKConnectionString is required"));
        var collationAreSame = kxCollation == xbkCollation;
        if (!collationAreSame)
        {
            logger.LogCritical("Source db collation '{SourceDbCollation}' is not same as target db collation {TargetDbCollation} => same collations are required", kxCollation, xbkCollation);
        }

        return collationAreSame;
    }

    private string? GetDbCollationName(string connectionString)
    {
        using var sqlConnection = new SqlConnection(connectionString);
        using var sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "SELECT DATABASEPROPERTYEX(DB_NAME(), 'Collation')";

        sqlConnection.Open();
        return sqlCommand.ExecuteScalar() as string;
    }
}