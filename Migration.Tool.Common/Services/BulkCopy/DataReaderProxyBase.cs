using System.Data;

namespace Migration.Tool.Common.Services.BulkCopy;

public class DataReaderProxyBase(IDataReader innerReader) : IDataReader
{
    protected readonly IDataReader InnerReader = innerReader;

    public virtual int FieldCount => InnerReader.FieldCount;
    public virtual int Depth => InnerReader.Depth;
    public virtual bool IsClosed => InnerReader.IsClosed;
    public virtual int RecordsAffected => InnerReader.RecordsAffected;

    public virtual bool GetBoolean(int i) => InnerReader.GetBoolean(i);
    public virtual byte GetByte(int i) => InnerReader.GetByte(i);

    public virtual long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) =>
        InnerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    public virtual char GetChar(int i) => InnerReader.GetChar(i);

    public virtual long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) =>
        InnerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    public virtual IDataReader GetData(int i) => InnerReader.GetData(i);
    public virtual string GetDataTypeName(int i) => InnerReader.GetDataTypeName(i);
    public virtual DateTime GetDateTime(int i) => InnerReader.GetDateTime(i);
    public virtual decimal GetDecimal(int i) => InnerReader.GetDecimal(i);
    public virtual double GetDouble(int i) => InnerReader.GetDouble(i);
    public virtual Type GetFieldType(int i) => InnerReader.GetFieldType(i);
    public virtual float GetFloat(int i) => InnerReader.GetFloat(i);
    public virtual Guid GetGuid(int i) => InnerReader.GetGuid(i);
    public virtual short GetInt16(int i) => InnerReader.GetInt16(i);
    public virtual int GetInt32(int i) => InnerReader.GetInt32(i);
    public virtual long GetInt64(int i) => InnerReader.GetInt64(i);
    public virtual string GetName(int i) => InnerReader.GetName(i);
    public virtual int GetOrdinal(string name) => InnerReader.GetOrdinal(name);
    public virtual string GetString(int i) => InnerReader.GetString(i);
    public virtual object GetValue(int i) => InnerReader.GetValue(i);
    public virtual int GetValues(object[] values) => InnerReader.GetValues(values);
    public virtual bool IsDBNull(int i) => InnerReader.IsDBNull(i);
    public virtual object this[int i] => InnerReader[i];
    public virtual object this[string name] => InnerReader[name];
    public virtual void Dispose() => InnerReader.Dispose();
    public virtual void Close() => InnerReader.Close();
    public virtual DataTable? GetSchemaTable() => InnerReader.GetSchemaTable();
    public virtual bool NextResult() => InnerReader.NextResult();
    public virtual bool Read() => InnerReader.Read();
}
