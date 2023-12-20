namespace Migration.Toolkit.Core.Behaviors;

using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KX13.Context;

public class CommandConstraintBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    private const string CmsHotfixVersionKey = "CMSHotfixVersion";
    private const string CmsHotfixDataVersionKey = "CMSHotfixDataVersion";
    private const string CmsDbVersionKey = "CMSDBVersion";
    private const string CmsDataVersionKey = "CMSDataVersion";

    private readonly ILogger<CommandConstraintBehavior<TRequest, TResponse>> _logger;
    private readonly IMigrationProtocol _protocol;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    public CommandConstraintBehavior(
        ILogger<CommandConstraintBehavior<TRequest, TResponse>> logger,
        IMigrationProtocol protocol,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration
    )
    {
        _logger = logger;
        _protocol = protocol;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

            var criticalCheckPassed = PerformChecks(request, kx13Context);
            if (!criticalCheckPassed)
            {
                return (TResponse)(CommandResult)new CommandCheckFailedResult(criticalCheckPassed);
            }
        }
        catch (Exception ex)
        {
            _protocol.CommandError<TRequest, TResponse>(ex, request);
            _logger.LogCritical(ex, "Error occured while checking command constraints");
            return (TResponse)(CommandResult)new CommandCheckFailedResult(false);
        }

        return await next();
    }

    private bool PerformChecks(TRequest request, KX13Context kx13Context)
    {
        return true;
        var criticalCheckPassed = true;
        const string supportedVersion = "13.0.64";
        if (SemanticVersion.TryParse(supportedVersion, out var minimalVersion))
        {
            criticalCheckPassed &= CheckVersion(kx13Context, minimalVersion);
        }

        // var sites = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId);
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
        var criticalCheckPassed = true;

        #region Check conclusion methods

        void UnableToReadVersionKey(string keyName)
        {
            _logger.LogCritical("Unable to read CMS version (incorrect format) - SettingsKeyName '{Key}'. Ensure Kentico version is at least '{SupportedVersion}'", keyName, minimalVersion.ToString());
            _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
            {
                ErrorKind = "Settings key value incorrect format",
                SettingsKeyName = keyName,
                SupportedVersion = minimalVersion.ToString()
            }));
            criticalCheckPassed = false;
        }

        void VersionKeyNotFound(string keyName)
        {
            _logger.LogCritical("CMS version not found - SettingsKeyName '{Key}'. Ensure Kentico version is at least '{SupportedVersion}'", keyName, minimalVersion.ToString());
            _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
            {
                ErrorKind = "Settings key not found",
                SettingsKeyName = keyName,
                SupportedVersion = minimalVersion.ToString()
            }));
            criticalCheckPassed = false;
        }

        void UpgradeNeeded(string keyName, string currentVersion)
        {
            _logger.LogCritical("{Key} '{CurrentVersion}' is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'", keyName, currentVersion, minimalVersion.ToString());
            _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
            {
                CurrentVersion = currentVersion,
                SupportedVersion = minimalVersion.ToString()
            }));
            criticalCheckPassed = false;
        }

        void LowHotfix(string keyName, int currentHotfix)
        {
            _logger.LogCritical("{Key} '{CurrentVersion}' hotfix is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'", keyName, currentHotfix, minimalVersion.ToString());
            _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
            {
                CurrentHotfix = currentHotfix.ToString(),
                SupportedVersion = minimalVersion.ToString()
            }));
            criticalCheckPassed = false;
        }

        #endregion

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsDataVersionKey) is { } cmsDataVersion)
        {
            if (SemanticVersion.TryParse(cmsDataVersion.KeyValue, out var cmsDataVer))
            {
                if (cmsDataVer.IsLesserThan(minimalVersion))
                {
                    UpgradeNeeded(CmsDataVersionKey, cmsDataVer.ToString());
                }
            }
            else
            {
                UnableToReadVersionKey(CmsDataVersionKey);
            }
        }
        else
        {
            VersionKeyNotFound(CmsDataVersionKey);
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsDbVersionKey) is { } cmsDbVersion)
        {
            if (SemanticVersion.TryParse(cmsDbVersion.KeyValue, out var cmsDataVer))
            {
                if (cmsDataVer.IsLesserThan(minimalVersion))
                {
                    UpgradeNeeded(CmsDbVersionKey, cmsDataVer.ToString());
                }
            }
            else
            {
                UnableToReadVersionKey(CmsDbVersionKey);
            }
        }
        else
        {
            VersionKeyNotFound(CmsDbVersionKey);
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsHotfixDataVersionKey) is { } cmsHotfixDataVersion)
        {
            if (int.TryParse(cmsHotfixDataVersion.KeyValue, out var version))
            {
                if (version < minimalVersion.Hotfix)
                {
                    LowHotfix(CmsHotfixDataVersionKey, version);
                }
            }
            else
            {
                UnableToReadVersionKey(CmsHotfixDataVersionKey);
            }
        }
        else
        {
            VersionKeyNotFound(CmsHotfixDataVersionKey);
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsHotfixVersionKey) is { } cmsHotfixVersion)
        {
            if (int.TryParse(cmsHotfixVersion.KeyValue, out var version))
            {
                if (version < minimalVersion.Hotfix)
                {
                    LowHotfix(CmsHotfixVersionKey, version);
                }
            }
            else
            {
                UnableToReadVersionKey(CmsHotfixVersionKey);
            }
        }
        else
        {
            VersionKeyNotFound(CmsHotfixVersionKey);
        }

        return criticalCheckPassed;
    }

    private bool CheckSite(List<KX13M.CmsSite> sourceSites, int sourceSiteId)
    {
        var criticalCheckPassed = true;
        if (sourceSites.All(s => s.SiteId != sourceSiteId))
        {
            var supportedSites = sourceSites.Select(x => new
            {
                x.SiteName,
                x.SiteId
            }).ToArray();
            var supportedSitesStr = string.Join(", ", supportedSites.Select(x => x.ToString()));
            _logger.LogCritical("Unable to find site with ID '{SourceSiteId}'. Check --siteId parameter. Supported sites: {SupportedSites}", sourceSiteId,
                supportedSitesStr);
            _protocol.Append(HandbookReferences.CommandConstraintBroken("Site exists")
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

    private bool CheckCulture(ICultureReliantCommand cultureReliantCommand, List<KX13M.CmsSite> sourceSites)
    {
        var criticalCheckPassed = true;
        var cultureCode = cultureReliantCommand.CultureCode;
        var siteCultureLookup = sourceSites
            .ToDictionary(x => x.SiteId, x => x.Cultures.Select(s => s.CultureCode.ToLowerInvariant()));

        foreach (var site in sourceSites)
        {
            if (siteCultureLookup.TryGetValue(site.SiteId, out var value))
            {
                var siteCultures = value.ToArray();
                if (!siteCultures.Contains(cultureCode.ToLowerInvariant()))
                {
                    var supportedCultures = string.Join(", ", siteCultures);
                    _logger.LogCritical("Unable to find culture '{Culture}' mapping to site '{SiteId}'. Check --culture parameter. Supported cultures for site: {SupportedCultures}", cultureCode, site.SiteId, supportedCultures);
                    _protocol.Append(HandbookReferences.CommandConstraintBroken("Culture is mapped to site")
                        .WithMessage("Check program argument '--culture'")
                        .WithData(new
                        {
                            cultureCode,
                            site.SiteId,
                            SiteCultures = supportedCultures
                        }));
                    criticalCheckPassed = false;
                }
            }
        }

        return criticalCheckPassed;
    }

    // TODO tk: 2022-11-02 create global rule
    private bool CheckDbCollations()
    {
        var kxCollation = GetDbCollationName(_toolkitConfiguration.KxConnectionString ?? throw new InvalidOperationException("KxConnectionString is required"));
        var xbkCollation = GetDbCollationName(_toolkitConfiguration.XbKConnectionString ?? throw new InvalidOperationException("XbKConnectionString is required"));
        var collationAreSame = kxCollation == xbkCollation;
        if (!collationAreSame)
        {
            _logger.LogCritical("Source db collation '{SourceDbCollation}' is not same as target db collation {TargetDbCollation} => same collations are required", kxCollation, xbkCollation);
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