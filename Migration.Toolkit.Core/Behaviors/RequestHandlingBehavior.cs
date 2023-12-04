using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.Core.Behaviors;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;

public class RequestHandlingBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse: CommandResult
{
    private readonly ILogger<RequestHandlingBehavior<TRequest, TResponse>> _logger;
    private readonly IMigrationProtocol _protocol;

    public RequestHandlingBehavior(
        ILogger<RequestHandlingBehavior<TRequest, TResponse>> logger,
        IMigrationProtocol protocol
        )
    {
        _logger = logger;
        _protocol = protocol;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Handling {CommandName}", typeof(TRequest).Name);
        try
        {
            _protocol.CommandRequest<TRequest, TResponse>(request);
            var response = await next();
            _protocol.CommandFinished(request, response);
            return response;
        }
        catch (Exception ex)
        {
            _protocol.CommandError<TRequest, TResponse>(ex, request);
            _logger.LogError(ex, "Error occured");
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled {CommandName} in elapsed: {Elapsed}", typeof(TRequest).Name, sw.Elapsed);    
        }
    }
}