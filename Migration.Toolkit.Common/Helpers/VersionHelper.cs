using Microsoft.Data.SqlClient;

namespace Migration.Toolkit.Common.Helpers;

public static class VersionHelper
{
    public static SemanticVersion? GetInstanceVersion(SqlConnection connection)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = N'CMSDBVersion'";
        string? result = cmd.ExecuteScalar() as string;

        return SemanticVersion.TryParse(result, out var semanticVersion)
            ? semanticVersion
            : null;
    }
}
