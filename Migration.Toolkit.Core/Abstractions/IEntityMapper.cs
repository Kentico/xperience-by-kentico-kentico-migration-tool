namespace Migration.Toolkit.Core.Abstractions;

using Kentico.Xperience.UMT.Model;

public interface IEntityMapper<TSourceEntity, TTargetEntity>
{
    IModelMappingResult<TTargetEntity> Map(TSourceEntity? source, TTargetEntity? target);
}

public interface IUmtMapper<in TSourceEntity>
{
    IEnumerable<IUmtModel> Map(TSourceEntity source);
}