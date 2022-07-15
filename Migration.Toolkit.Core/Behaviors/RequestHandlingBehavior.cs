using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Behaviors;

public class RequestHandlingBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse: CommandResult
{
    private readonly ILogger<RequestHandlingBehavior<TRequest, TResponse>> _logger;
    private readonly IMigrationProtocol _migrationProtocol;

    public RequestHandlingBehavior(
        ILogger<RequestHandlingBehavior<TRequest, TResponse>> logger,
        IMigrationProtocol migrationProtocol
        )
    {
        _logger = logger;
        _migrationProtocol = migrationProtocol;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Handling {CommandName}", typeof(TRequest).Name);
        try
        {
            _migrationProtocol.CommandRequest<TRequest, TResponse>(request);
            var response = await next();
            _migrationProtocol.CommandFinished(request, response);
            return response;
        }
        catch (Exception ex)
        {
            _migrationProtocol.CommandError<TRequest, TResponse>(ex, request);
            _logger.LogError(ex, "Error occured");
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled {CommandName} in elapsed: {Elapsed}", typeof(TResponse).Name, sw.Elapsed);    
        }
    }
}