namespace Migration.Toolkit.Common.Helpers;

using Microsoft.Data.SqlClient;

public static class VersionHelper
{
    public static SemanticVersion? GetInstanceVersion(SqlConnection connection)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = N'CMSDBVersion'";
        var result = cmd.ExecuteScalar() as string;

        return SemanticVersion.TryParse(result, out var semanticVersion)
            ? semanticVersion
            : null;
    }
}