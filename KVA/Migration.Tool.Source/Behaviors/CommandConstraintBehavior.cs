using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Behaviors;

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
            bool criticalCheckPassed = PerformChecks(request);
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
        bool criticalCheckPassed = true;

        var sourceSites = modelFacade.SelectAll<ICmsSite>()
            .ToList();

        foreach (var site in sourceSites)
        {
            criticalCheckPassed &= CheckSite(sourceSites, site.SiteID);
        }

        return criticalCheckPassed;
    }

    private bool CheckSite(List<ICmsSite> sourceSites, int sourceSiteId)
    {
        bool criticalCheckPassed = true;
        if (sourceSites.All(s => s.SiteID != sourceSiteId))
        {
            var supportedSites = sourceSites.Select(x => new { x.SiteName, x.SiteID }).ToArray();
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
}
