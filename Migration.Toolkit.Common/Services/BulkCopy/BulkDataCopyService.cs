namespace Migration.Toolkit.Common.Services.BulkCopy;

using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Helpers;

public record SqlColumn(string ColumnName, int OrdinalPosition);

public class BulkDataCopyService
{
    private readonly ToolkitConfiguration _configuration;
    private readonly ILogger<BulkDataCopyService> _logger;

    public BulkDataCopyService(ToolkitConfiguration configuration, ILogger<BulkDataCopyService> logger)
    {
        this._configuration = configuration;
        this._logger = logger;
    }

    public bool CheckIfDataExistsInTargetTable(string tableName)
    {
        using var targetConnection = new SqlConnection(_configuration.XbKConnectionString);
        using var command = targetConnection.CreateCommand();
        var query = $"SELECT COUNT(*) FROM {tableName}";
        command.CommandText = query;


        targetConnection.Open();
        return ((int)command.ExecuteScalar()) > 0;
    }

    public bool CheckForTableColumnsDifferences(string tableName, Dictionary<string, string>? checkedColumns, out List<(string? sourceColumn, string? targetColumn)> columnsWithFailedCheck)
    {
        var anyFailedColumnCheck = false;
        var sourceTableColumns = GetSqlTableColumns(tableName, _configuration.KxConnectionString)
            .Where(c => checkedColumns?.Keys.Any(k=> k.Equals(c.ColumnName, StringComparison.InvariantCultureIgnoreCase)) ?? true)
            .Select(x => x.ColumnName).OrderBy(x => x);
        var targetTableColumns = GetSqlTableColumns(tableName, _configuration.XbKConnectionString)
            .Where(c => checkedColumns?.Values.Any(k=> k.Equals(c.ColumnName, StringComparison.InvariantCultureIgnoreCase)) ?? true)
            .Select(x => x.ColumnName).OrderBy(x => x);

        var aligner = EnumerableHelper.CreateAligner(
            sourceTableColumns,
            targetTableColumns,
            sourceTableColumns.Union(targetTableColumns).OrderBy(x => x),
            a => a,
            b => b,
            false
        );

        columnsWithFailedCheck = new List<(string sourceColumn, string targetColumn)>();
        while (aligner.MoveNext())
        {
            switch (aligner.Current)
            {
                case SimpleAlignResultMatch<string, string, string> result:
                    _logger.LogDebug("Table {Table} pairing source({SourceColumnName}) <> target({TargetColumnName}) success", tableName, result?.A, result?.B);
                    break;
                case { } result:
                    columnsWithFailedCheck.Add((result.A, result.B));
                    _logger.LogError("Table {Table} pairing source({SourceColumnName}) <> target({TargetColumnName}) has failed", tableName, result?.A, result?.B);
                    anyFailedColumnCheck = true;
                    break;
            }
        }

        return anyFailedColumnCheck;
    }

    public bool CheckForTableColumnsDifferences(string tableName, out List<(string sourceColumn, string targetColumn)> columnsWithFailedCheck)
        => CheckForTableColumnsDifferences(tableName, null, out columnsWithFailedCheck);

    public void CopyTableToTable(BulkCopyRequest request)
    {
        var (tableName, columnFilter, dataFilter, batchSize, columns, valueInterceptor, skippedRowCallback, orderBy) = request;

        _logger.LogInformation("Copy of {TableName} started", tableName);

        var sourceColumns = GetSqlTableColumns(tableName, _configuration.KxConnectionString, columns)
            .ToArray();
        var targetColumns = GetSqlTableColumns(tableName, _configuration.XbKConnectionString, columns)
            .ToArray();

        AssertColumnsMatch(request, sourceColumns, targetColumns);

        using var sourceConnection = new SqlConnection(_configuration.KxConnectionString);
        using var sourceCommand = sourceConnection.CreateCommand();
        using var sqlBulkCopy = new SqlBulkCopy(_configuration.XbKConnectionString, SqlBulkCopyOptions.KeepIdentity);

        sqlBulkCopy.BulkCopyTimeout = 1200;
        sqlBulkCopy.BatchSize = batchSize;
        // TODO tk: 2022-05-31  sqlBulkCopy.EnableStreaming
        // TODO tk: 2022-05-31  sqlBulkCopy.BulkCopyTimeout

        sqlBulkCopy.NotifyAfter = 100000;
        sqlBulkCopy.SqlRowsCopied += (sender, args) =>
        {
            _logger.LogInformation("Copy '{TableName}': Rows copied={Rows}", tableName, args.RowsCopied);
        };


        var selectQuery = BuildSelectQuery(tableName, sourceColumns, orderBy).ToString();

        sqlBulkCopy.DestinationTableName = tableName;
        // foreach (var (columnName, ordinalPosition) in sourceColumns)
        if (request is BulkCopyRequestExtended extended)
        {
            foreach (var (sourceColumn, targetColumn) in extended.ColumnsMapping)
            {
                if (!columnFilter(sourceColumn))
                {
                    continue;
                }
                sqlBulkCopy.ColumnMappings.Add(sourceColumn, targetColumn);
            }
        }
        else
        {
            foreach (var (columnName, ordinalPosition) in sourceColumns)
            {
                if (!columnFilter(columnName))
                {
                    continue;
                }
                sqlBulkCopy.ColumnMappings.Add(columnName, columnName);
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

        try
        {
            sqlBulkCopy.WriteToServer(readerPipeline);
        }
        catch(Exception ex)
        {
            throw;
        }

        _logger.LogInformation("Copy of {TableName} finished! Total={Total}, TotalCopied={TotalCopied}", tableName, filteredReader.TotalItems, filteredReader.TotalNonFiltered);
    }

    private void AssertColumnsMatch(BulkCopyRequest request, SqlColumn[] sourceColumns, SqlColumn[] targetColumns)
    {
        foreach (var (columnName, ordinalPosition) in targetColumns)
        {
            if (sourceColumns.All(x => x.ColumnName != columnName))
            {
                _logger.LogWarning("{TableName} target column '{TargetColumn}' has no match", request.TableName, columnName);
            }
        }

        foreach (var (columnName, ordinalPosition) in sourceColumns)
        {
            if (targetColumns.All(x => x.ColumnName != columnName))
            {
                _logger.LogDebug("{TableName} source column '{SourceColumn}' has no match", request.TableName, columnName);
            }
        }
    }

    private static StringBuilder BuildSelectQuery(string tableName, SqlColumn[] sourceColumns, string? orderBy)
    {
        StringBuilder selectBuilder = new StringBuilder();
        selectBuilder.Append("SELECT ");
        for (var i = 0; i < sourceColumns.Length; i++)
        {
            var (columnName, ordinalPosition) = sourceColumns[i];
            selectBuilder.Append(columnName);
            if (i < sourceColumns.Length - 1)
            {
                selectBuilder.Append(", ");
            }
        }

        selectBuilder.Append($" FROM {tableName}");
        if (orderBy != null) selectBuilder.Append($" ORDER BY {orderBy}");
        return selectBuilder;
    }

    // TODO tk: 2022-06-30 assert column type is compatible
    public IEnumerable<SqlColumn> GetSqlTableColumns(string tableName, string? connectionString, IEnumerable<string>? allowedColumns = null)
    {
        using var sourceConnection = new SqlConnection(connectionString);
        using var cmd = sourceConnection.CreateCommand();

        cmd.CommandText = @"SELECT * FROM INFORMATION_SCHEMA.COLUMNS JOIN INFORMATION_SCHEMA.TABLES ON COLUMNS.TABLE_NAME = TABLES.TABLE_NAME
        WHERE TABLES.TABLE_NAME = @tableName";

        cmd.Parameters.AddWithValue("tableName", tableName);

        sourceConnection.Open();
        using var reader = cmd.ExecuteReader();

        var ordinal = 0;

        var allowedColumnsList = allowedColumns?.ToList();
        var columnFiler =allowedColumnsList != null && allowedColumnsList.Count != 0
            ?  new Func<string, bool>(s => allowedColumnsList.Contains(s, StringComparer.InvariantCultureIgnoreCase))
            :  _ => true;

        while (reader.Read())
        {
            var columnName = reader.GetString("COLUMN_NAME");
            if (columnFiler(columnName))
            {
                yield return new(columnName, ordinal);// reader.GetInt32("ORDINAL_POSITION"));
                ordinal++;
            }

            // TODO tk: 2022-05-31 IS_NULLABLE, DATA_TYPE, ... check column compatibility

        }
    }
}