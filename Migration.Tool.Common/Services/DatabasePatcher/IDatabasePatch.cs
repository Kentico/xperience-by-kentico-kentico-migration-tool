using Microsoft.Data.SqlClient;

namespace Migration.Tool.Common.Services.DatabasePatcher;

/// <summary>
/// Interface for classes implementing a patch that is to be automatically and idempotently applied to target project database
/// </summary>
public interface IDatabasePatch
{
    /// <summary>
    /// Unique name (in set of all <see cref="IDatabasePatch"/> patches, used in metatable to mark that the patch has been already applied
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Business logic of the patch
    /// </summary>
    /// <param name="sqlConnection">An open DB connection the patch can use to perform its operations</param>
    public void Apply(SqlConnection sqlConnection);
}
