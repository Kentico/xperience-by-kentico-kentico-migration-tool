namespace Migration.Toolkit.Source.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Source.Model;

public class CommandConstraintBehavior<TRequest, TResponse>(
    ILogger<CommandConstraintBehavior<TRequest, TResponse>> logger,
    IMigrationProtocol protocol,
    ModelFacade modelFacade)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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

        return criticalCheckPassed;
    }

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
}