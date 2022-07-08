namespace Migration.Toolkit.Common;

public interface IDataEqualityComparer<TSource, TTarget>
{
    bool DataEquals(TSource? source, TTarget? target);
}