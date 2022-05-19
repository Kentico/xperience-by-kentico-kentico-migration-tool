using System.Linq.Expressions;

namespace Migration.Toolkit.Core.Services;

public interface IPrimaryKeyLocatorService
{
    bool TryLocate<T>(Expression<Func<T,object>> keyNameSelector, int sourceId, out int targetId);
}