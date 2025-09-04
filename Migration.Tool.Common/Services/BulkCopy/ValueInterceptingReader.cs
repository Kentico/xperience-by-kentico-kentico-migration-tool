using System.Data;

namespace Migration.Tool.Common.Services.BulkCopy;

public record ValueInterceptorResult(object? Value = null, bool OverwriteValue = false, bool SkipDataRow = false)
{
    public static ValueInterceptorResult DoNothing => new();
    public static ValueInterceptorResult SkipRow => new(null, false, true);
    public static ValueInterceptorResult ReplaceValue(object? value) => new(value, true);
}

public delegate ValueInterceptorResult ValueInterceptor(int columnOrdinal, string columnName, object value, Dictionary<string, object?> currentRow);

public delegate void ValueInterceptingReaderSkippedRow(Dictionary<string, object?> current);

public class ValueInterceptingReader : DataReaderProxyBase
{
    private readonly Dictionary<int, string> columnOrdinals;
    private readonly ValueInterceptingReaderSkippedRow? skippedValueCallback;
    private readonly ValueInterceptor valueInterceptor;

    private Dictionary<int, object?> overwrittenValues = [];

    public ValueInterceptingReader(IDataReader innerReader, ValueInterceptor valueInterceptor, SqlColumn[] columnOrdinals, ValueInterceptingReaderSkippedRow? skippedValueCallback) : base(innerReader)
    {
        this.valueInterceptor = valueInterceptor;
        this.skippedValueCallback = skippedValueCallback;
        this.columnOrdinals = columnOrdinals.ToDictionary(x => x.OrdinalPosition, x => x.ColumnName);
    }

#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    public override object? GetValue(int i) => overwrittenValues.ContainsKey(i) ? overwrittenValues[i] : base.GetValue(i);
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

    public override bool Read()
    {
        while (base.Read())
        {
            overwrittenValues = [];

            bool skipCurrentDataRow = false;
            var currentRow = columnOrdinals.ToDictionary(k => k.Value, v => (object?)base.GetValue(v.Key));
            foreach ((int columnOrdinal, string columnName) in columnOrdinals)
            {
                (object? newValue, bool overwriteValue, bool skipDataRow) = valueInterceptor.Invoke(columnOrdinal, columnName, base.GetValue(columnOrdinal), currentRow);
                if (skipDataRow)
                {
                    skipCurrentDataRow = true;
                    break;
                }

                if (overwriteValue)
                {
                    overwrittenValues[columnOrdinal] = newValue;
                }
            }

            if (skipCurrentDataRow)
            {
                skippedValueCallback?.Invoke(currentRow);
                continue;
            }

            return true;
        }

        return false;
    }
}
