using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Behaviors;

public class CommandConstraintBehavior<TRequest, TResponse>(
    ILogger<CommandConstraintBehavior<TRequest, TResponse>> logger,
    IMigrationProtocol protocol,
    IDbContextFactory<K11Context> k11ContextFactory
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

            bool criticalCheckPassed = PerformChecks(request, k11Context);
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

    private bool PerformChecks(TRequest request, K11Context k11Context)
    {
        bool criticalCheckPassed = true;
        var sourceSites = k11Context.CmsSites
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

        return criticalCheckPassed;
    }

    private bool CheckSite(List<CmsSite> sourceSites, int sourceSiteId)
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

    private bool CheckCulture(ICultureReliantCommand cultureReliantCommand, List<CmsSite> sourceSites)
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
                    logger.LogCritical("Unable to find culture '{Culture}' mapping to site '{SiteId}'. Check --culture parameter. Supported cultures for site: {SupportedCultures}", cultureCode,
                        site.SiteId, supportedCultures);
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
