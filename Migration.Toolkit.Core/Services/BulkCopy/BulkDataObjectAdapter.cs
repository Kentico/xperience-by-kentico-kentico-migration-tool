// using System.Data;
// using System.Data.SqlClient;
// using Migration.Toolkit.Common.Helpers;
//
// namespace Migration.Toolkit.Core.Services.BulkCopy;
//
// public static class BulkDataObjectAdapter<T> where T: class
// {
//     public static IDataReader Adapt(IEnumerable<T> source)
//     {
//         return new InternalDataWrapper(source);
//     }
//
//     private class InternalDataWrapper : IDataReader
//     {
//         private readonly Dictionary<int, ObjectPropertyGetterMap> _propertyByOrdinal;
//         private readonly Dictionary<string, ObjectPropertyGetterMap> _propertyByName;
//         private readonly IEnumerator<T> _enumerator;
//         private bool _disposed;
//
//         public InternalDataWrapper(IEnumerable<T> items)
//         {
//             _propertyByOrdinal = ReflectionHelper<T>.GetPropertyGetterMaps().ToDictionary(k => k.PropertyIndex + 1, v => v);
//             _propertyByName = ReflectionHelper<T>.GetPropertyGetterMaps().ToDictionary(k => k.PropertyName, v => v);
//             _enumerator = items.GetEnumerator();
//         }
//         
//         public bool GetBoolean(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public byte GetByte(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
//         {
//             throw new NotImplementedException();
//         }
//
//         public char GetChar(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
//         {
//             throw new NotImplementedException();
//         }
//
//         public IDataReader GetData(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string GetDataTypeName(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public DateTime GetDateTime(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public decimal GetDecimal(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public double GetDouble(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public Type GetFieldType(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public float GetFloat(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public Guid GetGuid(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public short GetInt16(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public int GetInt32(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public long GetInt64(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string GetName(int i)
//         {
//             return _propertyByOrdinal[i].PropertyName;
//         }
//
//         public int GetOrdinal(string name)
//         {
//             return _propertyByName[name].PropertyIndex + 1;
//         }
//
//         public string GetString(int i)
//         {
//             throw new NotImplementedException();
//         }
//
//         public object? GetValue(int i)
//         {
//             return _propertyByOrdinal[i].PropertyGetMethod?.Invoke(_enumerator.Current, Array.Empty<object>())!;
//         }
//
//         public int GetValues(object[] values)
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool IsDBNull(int i)
//         {
//             return this[i] == default;
//         }
//
//         public int FieldCount => _propertyByOrdinal.Count;
//
//         public object? this[int i] => GetValue(i);
//
//         public object? this[string name] => GetValue(GetOrdinal(name));
//
//         public void Dispose()
//         {
//             if (!_disposed)
//             {
//                 _enumerator.Dispose();
//                 _disposed = true;
//             }
//         }
//
//         public void Close()
//         {
//             Dispose();
//         }
//
//         public DataTable? GetSchemaTable()
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool NextResult()
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool Read()
//         {
//             return _enumerator.MoveNext();
//         }
//
//         public int Depth { get; }
//         public bool IsClosed => _disposed;
//         public int RecordsAffected { get; }
//     }
//
//     public static void UpdateColumnMappingsSameColumnNames(SqlBulkCopyColumnMappingCollection columnMappings, Func<string, bool> columnNameFilter)
//     {
//         foreach (var property in ReflectionHelper<T>.GetPropertyGetterMaps())
//         {
//             if (columnNameFilter(property.PropertyName))
//             {
//                 columnMappings.Add(property.PropertyIndex, property.PropertyIndex);    
//             }
//         }
//     }
// }