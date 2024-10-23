using Microsoft.Data.SqlClient;

namespace Migration.Tool.Common.Services;

public class CoupledDataService(ToolConfiguration configuration)
{
    public Dictionary<string, object>? GetSourceCoupledDataRow(string tableName, string primaryKeyColumn, int? coupledDataId)
    {
        ArgumentNullException.ThrowIfNull(tableName);
        ArgumentNullException.ThrowIfNull(primaryKeyColumn);
        ArgumentNullException.ThrowIfNull(coupledDataId);

        using var targetConnection = new SqlConnection(configuration.KxConnectionString);
        using var command = targetConnection.CreateCommand();
        string query = $"SELECT * FROM {tableName} WHERE {primaryKeyColumn} = @{primaryKeyColumn}";
        command.CommandText = query;
        command.Parameters.AddWithValue(primaryKeyColumn, coupledDataId);

        targetConnection.Open();
        using var reader = command.ExecuteReader();

        var result = new Dictionary<string, object>();
        if (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                result.Add(reader.GetName(i), reader.GetValue(i));
            }

            return result;
        }

        return null;
    }
}
