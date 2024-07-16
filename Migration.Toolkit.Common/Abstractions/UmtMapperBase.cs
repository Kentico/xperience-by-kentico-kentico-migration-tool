using Kentico.Xperience.UMT.Model;

namespace Migration.Toolkit.Common.Abstractions;

public abstract class UmtMapperBase<TSourceEntity> : IUmtMapper<TSourceEntity>
{
    public IEnumerable<IUmtModel> Map(TSourceEntity source) => MapInternal(source);

    protected abstract IEnumerable<IUmtModel> MapInternal(TSourceEntity source);
}
