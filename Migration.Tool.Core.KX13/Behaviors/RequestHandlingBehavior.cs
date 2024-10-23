using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;

namespace Migration.Tool.Core.KX13.Behaviors;

public class RequestHandlingBehavior<TRequest, TResponse>(
    ILogger<RequestHandlingBehavior<TRequest, TResponse>> logger,
    IMigrationProtocol protocol) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("Handling {CommandName}", typeof(TRequest).Name);
        try
        {
            protocol.CommandRequest<TRequest, TResponse>(request);
            var response = await next();
            protocol.CommandFinished(request, response);
            return response;
        }
        catch (Exception ex)
        {
            protocol.CommandError<TRequest, TResponse>(ex, request);
            logger.LogError(ex, "Error occured");
            throw;
        }
        finally
        {
            logger.LogInformation("Handled {CommandName} in elapsed: {Elapsed}", typeof(TRequest).Name, sw.Elapsed);
        }
    }
}
