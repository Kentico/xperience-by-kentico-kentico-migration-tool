
using Migration.Toolkit.Common.MigrationProtocol;

namespace Migration.Toolkit.Common.Abstractions;
public interface IModelMappingResult
{
    bool Success { get; }
}

public interface IModelMappingResult<TResult> : IModelMappingResult
{
    TResult? Item { get; }
    bool NewInstance { get; }
    HandbookReference? HandbookReference { get; }

    void Deconstruct(out TResult? item, out bool newInstance)
    {
        item = Item;
        newInstance = NewInstance;
    }
    void Deconstruct(out HandbookReference? handbookReference) => handbookReference = HandbookReference;
}

public record AggregatedResult<TResult>(IEnumerable<IModelMappingResult<TResult>> Results) : IModelMappingResult<TResult>
{
    public TResult? Item => default;

    public bool NewInstance => false;

    public HandbookReference? HandbookReference => throw new NotImplementedException();

    public bool Success => Results.All(x => x.Success);
}

public record MapperResult<TResult>(TResult? Item, bool NewInstance, bool Success, HandbookReference? HandbookReference) : IModelMappingResult<TResult>;
public record MapperResultSuccess<TResult>(TResult? Item, bool NewInstance) : MapperResult<TResult>(Item, NewInstance, true, null);
public record MapperResultFailure<TResult>(HandbookReference HandbookReference) : MapperResult<TResult>(default, false, false, HandbookReference);

public static class Extensions
{
    public static MapperResultFailure<TResult> AsFailure<TResult>(this HandbookReference reference) => new(reference);
}
