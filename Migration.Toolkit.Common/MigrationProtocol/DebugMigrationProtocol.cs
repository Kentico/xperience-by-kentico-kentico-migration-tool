namespace Migration.Toolkit.Common.MigrationProtocol;

using MediatR;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;

public class DebugMigrationProtocol: IMigrationProtocol
{
    private readonly ILogger<DebugMigrationProtocol> _logger;

    public DebugMigrationProtocol(ILogger<DebugMigrationProtocol> logger)
    {
        _logger = logger;
    }

    public void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped)
    {
        
    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {
        _logger.LogDebug("FetchedTarget: {Type}: {Source}", typeof(TTarget).FullName, target);
    }

    public void FetchedSource<TSource>(TSource? source)
    {
        _logger.LogDebug("FetchedSource: {Type}: {Source}", typeof(TSource).FullName, source);
    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped)
    {
        
    }
    
    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
        
    }

    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>
    {
        _logger.LogDebug("CommandRequest {Request}", request);
    }

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult
    {
        _logger.LogDebug("CommandFinished {Request} => {Response}", request, response);
    }

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse>
    {
        _logger.LogDebug("CommandError {Request} => {Exception}", request, exception);
    }

    public void Append(HandbookReference? handbookReference)
    {
        _logger.LogDebug(handbookReference?.ToString());
    }
}