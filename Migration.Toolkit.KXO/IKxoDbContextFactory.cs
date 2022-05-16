using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.KXO;

public interface IKxoDbContextFactory
{
    KxoContext Create();
}