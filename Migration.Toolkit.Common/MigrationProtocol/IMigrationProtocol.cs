namespace Migration.Toolkit.Common.MigrationProtocol;

using MediatR;
using Migration.Toolkit.Common.Abstractions;

public interface IMigrationProtocol
{
    void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped);
    void FetchedTarget<TTarget>(TTarget? target);
    void FetchedSource<TSource>(TSource? source);
    void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped);
    void Warning<T>(HandbookReference handbookRef, T? entity);

    void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>;
    void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult;
    void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse>;

    void Append(HandbookReference? handbookReference);
}