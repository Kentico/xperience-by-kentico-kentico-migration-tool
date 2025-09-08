using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Services.DatabasePatcher.Patches;

namespace Migration.Tool.Common.Services.DatabasePatcher;

public class DatabasePatcher(ToolConfiguration toolConfiguration, ILogger<DatabasePatcher> logger)
{
    private const string APPLIED_PATCHES_TABLE_SCHEMA = "dbo";
    private const string APPLIED_PATCHES_TABLE_NAME = "MT_Patches";

    private readonly IDatabasePatch[] availablePatches =
    {
        new DbPatch_001_ContentItemRelatedGuids()
    };

    /// <summary>
    /// Applies patches to the target XbyK database. This mechanism is used mainly for 
    /// fixing invalid data caused by previous runs of Migration Tool.
    /// </summary>
    public void Run()
    {
        using var connection = new SqlConnection(toolConfiguration.XbKConnectionString);
        connection.Open();

        EnsurePatchTableExists(connection);

        var applied = GetAppliedPatches(connection);

        var unapplied = availablePatches
            .Where(p => !applied.Contains(p.Name))
            .ToArray();

        if (unapplied.Length == 0)
        {
            logger.LogInformation("No DB patches to apply");
            return;
        }

        foreach (var patch in unapplied)
        {
            logger.LogInformation($"Applying DB patch: {patch.Name}");
            patch.Apply(connection);
            MarkPatchAsApplied(patch.Name, connection);
        }
    }

    private void EnsurePatchTableExists(SqlConnection connection)
    {
        var sql = $@"
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = '{APPLIED_PATCHES_TABLE_NAME}' AND s.name = '{APPLIED_PATCHES_TABLE_SCHEMA}')
BEGIN
    CREATE TABLE {APPLIED_PATCHES_TABLE_SCHEMA}.{APPLIED_PATCHES_TABLE_NAME} (
        Name NVARCHAR(200) NOT NULL PRIMARY KEY,
        AppliedOn DATETIME NOT NULL
    )
END";

        using var cmd = new SqlCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    private HashSet<string> GetAppliedPatches(SqlConnection connection)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var sql = $"SELECT Name FROM {APPLIED_PATCHES_TABLE_SCHEMA}.{APPLIED_PATCHES_TABLE_NAME}";
        using var cmd = new SqlCommand(sql, connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }

    private void MarkPatchAsApplied(string name, SqlConnection connection)
    {
        var sql = $"INSERT INTO {APPLIED_PATCHES_TABLE_SCHEMA}.{APPLIED_PATCHES_TABLE_NAME} (Name, AppliedOn) VALUES (@name, @appliedOn)";
        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@appliedOn", DateTime.UtcNow);
        cmd.ExecuteNonQuery();
    }
}
