namespace Migration.Toolkit.Core.Behaviors;

using System.Diagnostics;
using CMS.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;

public class CommandConstraintBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public const string CmsHotfixVersionKey = "CMSHotfixVersion";
    public const string CmsHotfixDataVersionKey = "CMSHotfixDataVersion";
    public const string CmsDbVersionKey = "CMSDBVersion";
    public const string CmsDataVersionKey = "CMSDataVersion";
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
        var criticalCheckPassed = true;
        const string supportedVersion = "13.0.64";
        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsDataVersionKey) is { } cmsDataVersion and not { KeyValue: "13.0" })
        {
            // NOK
            _logger.LogCritical("{Key} '{CMSDataVersion}' is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'.",
                CmsDataVersionKey, cmsDataVersion.KeyValue, supportedVersion);
            _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
            {
                cmsDataVersion = cmsDataVersion.KeyValue,
                supportedVersion
            }));
            criticalCheckPassed = false;
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsDbVersionKey) is { } cmsDbVersion and not { KeyValue: "13.0" })
        {
            // NOK
            _logger.LogCritical("{Key} '{CMSDBVersion}' is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'.",
                CmsDbVersionKey, cmsDbVersion.KeyValue, supportedVersion);
            _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
            {
                cmsDbVersion = cmsDbVersion.KeyValue,
                supportedVersion
            }));
            criticalCheckPassed = false;
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsHotfixDataVersionKey) is { } cmsHotfixDataVersion)
        {
            if (int.TryParse(cmsHotfixDataVersion.KeyValue, out var version))
            {
                if (version < 64)
                {
                    // NOK
                    _logger.LogCritical(
                        "{Key} '{CMSHotfixDataVersion}' is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'.",
                        CmsHotfixDataVersionKey, version, supportedVersion);
                    _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
                    {
                        cmsHotfixDataVersion = cmsHotfixDataVersion.KeyValue,
                        supportedVersion
                    }));
                    criticalCheckPassed = false;
                }
            }
            else
            {
                _logger.LogCritical("Unable to read {Key} '{CMSHotfixDataVersion}'. Upgrade Kentico to at least '{SupportedVersion}'.",
                    CmsHotfixDataVersionKey, cmsHotfixDataVersion.KeyValue, supportedVersion);
                _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
                {
                    cmsHotfixDataVersion = cmsHotfixDataVersion.KeyValue,
                    supportedVersion
                }));
                criticalCheckPassed = false;
            }
        }

        if (kx13Context.CmsSettingsKeys.FirstOrDefault(s => s.KeyName == CmsHotfixVersionKey) is { } cmsHotfixVersion)
        {
            if (int.TryParse(cmsHotfixVersion.KeyValue, out var version))
            {
                if (version < 64)
                {
                    // NOK
                    _logger.LogCritical(
                        "{Key} '{CMSHotfixVersion}' is not supported for migration. Upgrade Kentico to at least '{SupportedVersion}'.",
                        CmsHotfixVersionKey, version, supportedVersion);
                    _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
                    {
                        cmsHotfixVersion = cmsHotfixVersion.KeyValue,
                        supportedVersion
                    }));
                    criticalCheckPassed = false;
                }
            }
            else
            {
                _logger.LogCritical("Unable to read {Key} '{CMSHotfixVersion}'. Upgrade Kentico to at least '{SupportedVersion}'.",
                    CmsHotfixVersionKey, cmsHotfixVersion.KeyValue, supportedVersion);
                _protocol.Append(HandbookReferences.InvalidSourceCmsVersion().WithData(new
                {
                    cmsHotfixVersion = cmsHotfixVersion.KeyValue,
                    supportedVersion
                }));
                criticalCheckPassed = false;
            }
        }

        var sites = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId);
        var sourceSites = kx13Context.CmsSites
            .Include(s => s.Cultures)
            .ToList();

        foreach (var (sourceSiteId, targetSiteId) in sites)
        {
            criticalCheckPassed &= CheckSite(sourceSites, sourceSiteId);
        }

        if (request is ICultureReliantCommand cultureReliantCommand)
        {
            criticalCheckPassed &= CheckCulture(cultureReliantCommand, sourceSites, sites);
        }

        return criticalCheckPassed;
    }

    private bool CheckSite(List<CmsSite> sourceSites, int sourceSiteId)
    {
        bool criticalCheckPassed = true;
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

    private bool CheckCulture(ICultureReliantCommand cultureReliantCommand, List<CmsSite> sourceSites, Dictionary<int, int> sites)
    {
        var criticalCheckPassed = true;
        var cultureCode = cultureReliantCommand.CultureCode;
        var siteCultureLookup = sourceSites
            .ToDictionary(x => x.SiteId, x => x.Cultures.Select(x => x.CultureCode.ToLowerInvariant()));

        foreach (var (sourceSiteId, _) in sites)
        {
            if (siteCultureLookup.ContainsKey(sourceSiteId))
            {
                var siteCultures = siteCultureLookup[sourceSiteId].ToArray();
                if (!siteCultures.Contains(cultureCode.ToLowerInvariant()))
                {
                    var supportedCultures = string.Join(", ", siteCultures);
                    _logger.LogCritical(
                        "Unable to find culture '{Culture}' mapping to site '{SiteId}'. Check --culture parameter. Supported cultures for site: {SupportedCultures}",
                        cultureCode, sourceSiteId, supportedCultures);
                    _protocol.Append(HandbookReferences.CommandConstraintBroken("Culture is mapped to site")
                        .WithMessage("Check program argument '--culture'")
                        .WithData(new
                        {
                            cultureCode,
                            sourceSiteId,
                            SiteCultures = supportedCultures
                        }));
                    criticalCheckPassed = false;
                }
            }
        }

        return criticalCheckPassed;
    }
}