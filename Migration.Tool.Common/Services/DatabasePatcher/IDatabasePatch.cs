using Microsoft.Data.SqlClient;

namespace Migration.Tool.Common.Services.DatabasePatcher;
public interface IDatabasePatch
{
    public string Name { get; }
    public abstract void Apply(SqlConnection sqlConnection);
}
