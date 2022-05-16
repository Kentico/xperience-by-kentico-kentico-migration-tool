namespace Migration.Toolkit.Core.Abstractions;

public interface IEntityMapper<TSourceEntity, TTargetEntity>
{
    ModelMappingResult<TTargetEntity> Map(TSourceEntity? source, TTargetEntity? target);
}