using System.Data;
using System.Text;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Services.BulkCopy;

public record SqlColumn(string ColumnName, int OrdinalPosition);

public class BulkDataCopyService(ToolConfiguration configuration, ILogger<BulkDataCopyService> logger)
{
    public bool CheckIfDataExistsInTargetTable(string tableName)
    {
        using var targetConnection = new SqlConnection(configuration.XbKConnectionString);
        using var command = targetConnection.CreateCommand();
        string query = $"SELECT COUNT(*) FROM {tableName}";
        command.CommandText = query;


        targetConnection.Open();
        return (int)command.ExecuteScalar() > 0;
    }

    public bool CheckForTableColumnsDifferences(string tableName, Dictionary<string, string>? checkedColumns, out List<(string? sourceColumn, string? targetColumn)> columnsWithFailedCheck)
    {
        bool anyFailedColumnCheck = false;
        var sourceTableColumns = GetSqlTableColumns(tableName, configuration.KxConnectionString)
            .Where(c => checkedColumns?.Keys.Any(k => k.Equals(c.ColumnName, StringComparison.InvariantCultureIgnoreCase)) ?? true)
            .Select(x => x.ColumnName).OrderBy(x => x);
        var targetTableColumns = GetSqlTableColumns(tableName, configuration.XbKConnectionString)
            .Where(c => checkedColumns?.Values.Any(k => k.Equals(c.ColumnName, StringComparison.InvariantCultureIgnoreCase)) ?? true)
            .Select(x => x.ColumnName).OrderBy(x => x);

        var aligner = EnumerableHelper.CreateAligner(
            sourceTableColumns,
            targetTableColumns,
            sourceTableColumns.Union(targetTableColumns).OrderBy(x => x),
            a => a,
            b => b,
            false
        );

        columnsWithFailedCheck = [];
        while (aligner.MoveNext())
        {
            switch (aligner.Current)
            {
                case SimpleAlignResultMatch<string, string, string> result:
                    logger.LogDebug("Table {Table} pairing source({SourceColumnName}) <> target({TargetColumnName}) success", tableName, result?.A, result?.B);
                    break;
                case { } result:
                    columnsWithFailedCheck.Add((result.A, result.B));
                    logger.LogError("Table {Table} pairing source({SourceColumnName}) <> target({TargetColumnName}) has failed", tableName, result?.A, result?.B);
                    anyFailedColumnCheck = true;
                    break;
                default:
                    break;
            }
        }

        return anyFailedColumnCheck;
    }

    public bool CheckForTableColumnsDifferences(string tableName, out List<(string sourceColumn, string targetColumn)> columnsWithFailedCheck)
        => CheckForTableColumnsDifferences(tableName, null, out columnsWithFailedCheck);

    public void CopyTableToTable(BulkCopyRequest request)
    {
        (string tableName, var columnFilter, var dataFilter, int batchSize, var columns, var valueInterceptor, var skippedRowCallback, string? orderBy) = request;

        logger.LogInformation("Copy of {TableName} started", tableName);

        var sourceColumns = GetSqlTableColumns(tableName, configuration.KxConnectionString, columns)
            .ToArray();
        var targetColumns = GetSqlTableColumns(tableName, configuration.XbKConnectionString, columns)
            .ToArray();
        var allTargetColumns = GetSqlTableColumns(tableName, configuration.XbKConnectionString)
            .ToArray();

        AssertColumnsMatch(request, sourceColumns, targetColumns);

        using var sourceConnection = new SqlConnection(configuration.KxConnectionString);
        using var sourceCommand = sourceConnection.CreateCommand();
        using var sqlBulkCopy = new SqlBulkCopy(configuration.XbKConnectionString, SqlBulkCopyOptions.KeepIdentity);

        sqlBulkCopy.BulkCopyTimeout = 1200;
        sqlBulkCopy.BatchSize = batchSize;
        // sqlBulkCopy.EnableStreaming
        // sqlBulkCopy.BulkCopyTimeout

        sqlBulkCopy.NotifyAfter = 100000;
        sqlBulkCopy.SqlRowsCopied += (sender, args) => logger.LogInformation("Copy '{TableName}': Rows copied={Rows}", tableName, args.RowsCopied);


        string selectQuery = BuildSelectQuery(tableName, sourceColumns, orderBy).ToString();
        logger.LogTrace("Select QUERY: {Query}", selectQuery);
        sqlBulkCopy.DestinationTableName = tableName;
        // foreach (var (columnName, ordinalPosition) in sourceColumns)
        if (request is BulkCopyRequestExtended extended)
        {
            foreach ((string sourceColumn, string targetColumn) in extended.ColumnsMapping)
            {
                if (!columnFilter(sourceColumn))
                {
                    continue;
                }

                var tc = allTargetColumns
                             .FirstOrDefault(tc => tc.ColumnName.Equals(targetColumn, StringComparison.InvariantCultureIgnoreCase))
                         ?? throw new InvalidOperationException($"Missing column '{targetColumn}'");
                sqlBulkCopy.ColumnMappings.Add(sourceColumn, tc.ColumnName);
                logger.LogTrace("Column mapping '{Source}' => '{Target}', {SC} {TC}", sourceColumn, tc.ColumnName,
                    sourceColumns.Any(s => s.ColumnName.Equals(sourceColumn, StringComparison.InvariantCultureIgnoreCase)),
                    targetColumns.Any(s => s.ColumnName.Equals(tc.ColumnName, StringComparison.InvariantCultureIgnoreCase))
                );
            }
        }
        else
        {
            foreach ((string columnName, int ordinalPosition) in sourceColumns)
            {
                if (!columnFilter(columnName))
                {
                    continue;
                }

                sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
                logger.LogTrace("Column mapping '{Source}' => '{Target}', {SC} {TC}", columnName, columnName,
                    sourceColumns.Any(s => s.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase)),
                    targetColumns.Any(s => s.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                );
            }
        }

        sourceCommand.CommandText = selectQuery;
        sourceCommand.CommandType = CommandType.Text;
        sourceConnection.Open();
        using var reader = sourceCommand.ExecuteReader();
        var filteredReader = new FilteredDbDataReader<SqlDataReader>(reader, dataFilter);
        IDataReader readerPipeline = filteredReader;
        if (valueInterceptor != null)
        {
            readerPipeline = new ValueInterceptingReader(readerPipeline, valueInterceptor, sourceColumns, skippedRowCallback);
        }

        sqlBulkCopy.WriteToServer(readerPipeline);

        logger.LogInformation("Copy of {TableName} finished! Total={Total}, TotalCopied={TotalCopied}", tableName, filteredReader.TotalItems, filteredReader.TotalNonFiltered);
    }

    private void AssertColumnsMatch(BulkCopyRequest request, SqlColumn[] sourceColumns, SqlColumn[] targetColumns)
    {
        foreach ((string columnName, int ordinalPosition) in targetColumns)
        {
            if (sourceColumns.All(x => x.ColumnName != columnName))
            {
                logger.LogWarning("{TableName} target column '{TargetColumn}' has no match", request.TableName, columnName);
            }
        }

        foreach ((string columnName, int ordinalPosition) in sourceColumns)
        {
            if (targetColumns.All(x => x.ColumnName != columnName))
            {
                logger.LogDebug("{TableName} source column '{SourceColumn}' has no match", request.TableName, columnName);
            }
        }
    }

    private static StringBuilder BuildSelectQuery(string tableName, SqlColumn[] sourceColumns, string? orderBy)
    {
        var selectBuilder = new StringBuilder();
        selectBuilder.Append("SELECT ");
        for (int i = 0; i < sourceColumns.Length; i++)
        {
            (string columnName, int _) = sourceColumns[i];
            selectBuilder.Append($"[{columnName}]");
            if (i < sourceColumns.Length - 1)
            {
                selectBuilder.Append(", ");
            }
        }

        selectBuilder.Append($" FROM {tableName}");
        if (orderBy != null)
        {
            selectBuilder.Append($" ORDER BY {orderBy}");
        }

        return selectBuilder;
    }

    public IEnumerable<SqlColumn> GetSqlTableColumns(string tableName, string? connectionString, IEnumerable<string>? allowedColumns = null)
    {
        using var sourceConnection = new SqlConnection(connectionString);
        using var cmd = sourceConnection.CreateCommand();

        cmd.CommandText = @"SELECT * FROM INFORMATION_SCHEMA.COLUMNS JOIN INFORMATION_SCHEMA.TABLES ON COLUMNS.TABLE_NAME = TABLES.TABLE_NAME
        WHERE TABLES.TABLE_NAME = @tableName";

        cmd.Parameters.AddWithValue("tableName", tableName);

        sourceConnection.Open();
        using var reader = cmd.ExecuteReader();

        int ordinal = 0;

        var allowedColumnsList = allowedColumns?.ToList();
        var columnFiler = allowedColumnsList != null && allowedColumnsList.Count != 0
            ? new Func<string, bool>(s => allowedColumnsList.Contains(s, StringComparer.InvariantCultureIgnoreCase))
            : _ => true;

        while (reader.Read())
        {
            string columnName = reader.GetString("COLUMN_NAME");
            if (columnFiler(columnName))
            {
                logger.LogTrace("[{Table}].{ColumnName} INCLUDED", tableName, columnName);
                yield return new SqlColumn(columnName, ordinal); // reader.GetInt32("ORDINAL_POSITION"));
                ordinal++;
            }
            else
            {
                logger.LogTrace("[{Table}].{ColumnName} SKIPPED", tableName, columnName);
            }
        }
    }
}
