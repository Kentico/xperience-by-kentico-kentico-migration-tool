using System.Data;

namespace Migration.Toolkit.Common.Services.BulkCopy;

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
    private readonly Dictionary<int, string> _columnOrdinals;
    private readonly ValueInterceptingReaderSkippedRow? _skippedValueCallback;
    private readonly ValueInterceptor _valueInterceptor;

    private Dictionary<int, object?> _overwrittenValues = [];

    public ValueInterceptingReader(IDataReader innerReader, ValueInterceptor valueInterceptor, SqlColumn[] columnOrdinals, ValueInterceptingReaderSkippedRow? skippedValueCallback) : base(innerReader)
    {
        _valueInterceptor = valueInterceptor;
        _skippedValueCallback = skippedValueCallback;
        _columnOrdinals = columnOrdinals.ToDictionary(x => x.OrdinalPosition, x => x.ColumnName);
    }

    public override object? GetValue(int i) => _overwrittenValues.ContainsKey(i) ? _overwrittenValues[i] : base.GetValue(i);

    public override bool Read()
    {
        while (base.Read())
        {
            _overwrittenValues = [];

            bool skipCurrentDataRow = false;
            var currentRow = _columnOrdinals.ToDictionary(k => k.Value, v => base.GetValue(v.Key));
            foreach ((int columnOrdinal, string columnName) in _columnOrdinals)
            {
                (object? newValue, bool overwriteValue, bool skipDataRow) = _valueInterceptor.Invoke(columnOrdinal, columnName, base.GetValue(columnOrdinal), currentRow);
                if (skipDataRow)
                {
                    skipCurrentDataRow = true;
                    break;
                }

                if (overwriteValue)
                {
                    _overwrittenValues[columnOrdinal] = newValue;
                }
            }

            if (skipCurrentDataRow)
            {
                _skippedValueCallback?.Invoke(currentRow);
                continue;
            }

            return true;
        }

        return false;
    }
}
