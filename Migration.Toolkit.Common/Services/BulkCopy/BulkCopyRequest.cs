namespace Migration.Toolkit.Common.Services.BulkCopy;

using System.Data;

/// <summary>
/// Request definition for bulk copy service
/// </summary>
/// <param name="TableName">Table name of table that will be copied</param>
/// <param name="ColumnFilter">Column exclusion</param>
/// <param name="DataFilter">Data exclusion - use for data validation and data exclusion when you know, that data will not satisfy target DB constraints</param>
/// <param name="BatchSize">Size of batch for bulk copy</param>
/// <param name="Columns">Columns used for bulk copy</param>
/// <param name="ValueInterceptor">Use for data validation and error reporting, avoid expensive operations</param>
/// <param name="SkippedRowCallback">Called when row is skipped for whatever reason</param>
/// <param name="OrderBy">SQL ORDER BY definition</param>
public record BulkCopyRequest(string TableName, Func<string,bool> ColumnFilter, Func<IDataReader,bool> DataFilter, int BatchSize, List<string>? Columns = null, ValueInterceptor? ValueInterceptor = null, ValueInterceptingReaderSkippedRow? SkippedRowCallback = null, string? OrderBy = null);

public record BulkCopyRequestExtended(string TableName, Func<string,bool> ColumnFilter, Func<IDataReader,bool> DataFilter, int BatchSize, Dictionary<string, string> ColumnsMapping, ValueInterceptor? ValueInterceptor = null, ValueInterceptingReaderSkippedRow? SkippedRowCallback = null, string? OrderBy = null): BulkCopyRequest(TableName, ColumnFilter, DataFilter, BatchSize, ColumnsMapping.Keys.ToList(), ValueInterceptor, SkippedRowCallback, OrderBy);