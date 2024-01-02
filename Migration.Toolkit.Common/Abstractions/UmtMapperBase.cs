namespace Migration.Toolkit.Common.Abstractions;

using Kentico.Xperience.UMT.Model;

public abstract class UmtMapperBase<TSourceEntity>: IUmtMapper<TSourceEntity>
{
    public IEnumerable<IUmtModel> Map(TSourceEntity source)
    {
        return MapInternal(source);
    }

    protected abstract IEnumerable<IUmtModel> MapInternal(TSourceEntity source);
}