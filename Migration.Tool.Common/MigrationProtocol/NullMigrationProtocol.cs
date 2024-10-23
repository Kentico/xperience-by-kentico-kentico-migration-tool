using MediatR;

using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Common.MigrationProtocol;

public class NullMigrationProtocol : IMigrationProtocol
{
    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
    }

    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>
    {
    }

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult
    {
    }

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse>
    {
    }

    public void Append(HandbookReference? handbookReference)
    {
    }

    public void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped)
    {
    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {
    }

    public void FetchedSource<TSource>(TSource? source)
    {
    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped)
    {
    }
}
