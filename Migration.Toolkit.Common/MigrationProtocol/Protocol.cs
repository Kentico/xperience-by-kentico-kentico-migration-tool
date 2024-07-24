using MediatR;

using Migration.Toolkit.Common.Abstractions;

namespace Migration.Toolkit.Common.MigrationProtocol;

public class Protocol(IEnumerable<IMigrationProtocol> protocols) : IProtocol
{
    public void MappedTarget<TTarget>(IModelMappingResult<TTarget> mapped)
    {
        foreach (var protocol in protocols)
        {
            protocol.MappedTarget(mapped);
        }
    }

    public void FetchedTarget<TTarget>(TTarget? target)
    {
        foreach (var protocol in protocols)
        {
            protocol.FetchedTarget(target);
        }
    }

    public void FetchedSource<TSource>(TSource? source)
    {
        foreach (var protocol in protocols)
        {
            protocol.FetchedSource(source);
        }
    }

    public void Success<TSource, TTarget>(TSource source, TTarget target, IModelMappingResult<TTarget>? mapped)
    {
        foreach (var protocol in protocols)
        {
            protocol.Success(source, target, mapped);
        }
    }

    public void Warning<T>(HandbookReference handbookRef, T? entity)
    {
        foreach (var protocol in protocols)
        {
            protocol.Warning(handbookRef, entity);
        }
    }

    public void CommandRequest<TRequest, TResponse>(TRequest request) where TRequest : IRequest<TResponse>
    {
        foreach (var protocol in protocols)
        {
            protocol.CommandRequest<TRequest, TResponse>(request);
        }
    }

    public void CommandFinished<TRequest, TResponse>(TRequest request, TResponse response) where TRequest : IRequest<TResponse> where TResponse : CommandResult
    {
        foreach (var protocol in protocols)
        {
            protocol.CommandFinished(request, response);
        }
    }

    public void CommandError<TRequest, TResponse>(Exception exception, TRequest request) where TRequest : IRequest<TResponse>
    {
        foreach (var protocol in protocols)
        {
            protocol.CommandError<TRequest, TResponse>(exception, request);
        }
    }

    public void Append(HandbookReference? handbookReference)
    {
        foreach (var protocol in protocols)
        {
            protocol.Append(handbookReference);
        }
    }
}
