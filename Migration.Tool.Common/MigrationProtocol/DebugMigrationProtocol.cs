using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common.MigrationProtocol;

public class DebugMigrationProtocol(ILogger<DebugMigrationProtocol> logger) : IMigrationProtocol
{
    public void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped)
    {
    }

    public void FetchedTarget<TTarget>(TTarget? target) => logger.LogDebug("FetchedTarget: {Type}: {Source}", typeof(TTarget).FullName, target);

    public void FetchedSource<TSource>(TSource? source) => logger.LogDebug("FetchedSource: {Type}: {Source}", typeof(TSource).FullName, source);

    public void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped)
    {
    }

    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
    }

    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse> => logger.LogDebug("CommandRequest {Request}", request);

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult => logger.LogDebug("CommandFinished {Request} => {Response}", request, response);

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse> => logger.LogDebug("CommandError {Request} => {Exception}", request, exception);

    public void Append(HandbookReference? handbookReference) => logger.LogDebug(handbookReference?.ToString());
}
