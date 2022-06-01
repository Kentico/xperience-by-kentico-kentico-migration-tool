using System.Data.Common;

namespace Migration.Toolkit.Core.Services.BulkCopy;

public record BulkCopyRequest(string TableName, Func<string,bool> ColumnFilter, Func<DbDataReader,bool> DataFilter, int BatchSize);