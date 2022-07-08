namespace Migration.Toolkit.Core.Abstractions;

public interface ISynchronizer<TSourceEntity, TTargetEntity>
{
    Task StartAsync();
}