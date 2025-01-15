using System.Runtime.CompilerServices;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;

namespace Migration.Tool.Source;

public class ModelFacade(ILogger<ModelFacade> logger, ToolConfiguration configuration)
{
    private SemanticVersion? semanticVersion;

    public async IAsyncEnumerable<T> SelectAllAsync<T>([EnumeratorCancellation] CancellationToken cancellationToken) where T : ISourceModel<T>
    {
        semanticVersion ??= SelectVersion();

        await using var conn = GetConnection();
        await conn.OpenAsync(cancellationToken);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName}";
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return T.FromReader(reader, semanticVersion);
        }
    }

    public IEnumerable<T> SelectAll<T>(string? orderBy = null) where T : ISourceModel<T>
    {
        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName}";
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            cmd.CommandText += orderBy;
        }

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return T.FromReader(reader, semanticVersion);
        }
    }

    public IEnumerable<Dictionary<string, object?>> SelectAllAsDictionary(string tableName, string? orderBy = null)
    {
        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {tableName}";
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            cmd.CommandText += orderBy;
        }

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var r = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var dbColumn in reader.GetColumnSchema())
            {
                if (dbColumn.ColumnOrdinal is { } ordinal)
                {
                    object val = reader.GetValue(ordinal);

                    if (DBNull.Value.Equals(val))
                    {
                        r.Add(dbColumn.ColumnName, null);
                    }
                    else
                    {
                        r.Add(dbColumn.ColumnName, val);
                    }
                }
                else
                {
                    logger.LogError("Column '{Column}' of '{Table}' has no ordinal! This might introduce invalid data mapping", dbColumn.ColumnName, tableName);
                }
            }

            yield return r;
        }
    }

    public IEnumerable<T> SelectWhere<T>(string where, params SqlParameter[] parameters) where T : ISourceModel<T>
    {
        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName} WHERE {where}";
        cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return T.FromReader(reader, semanticVersion);
        }
    }

    public bool IsAvailable<T>() where T : ISourceModel<T>
    {
        semanticVersion ??= SelectVersion();
        return T.IsAvailable(semanticVersion);
    }

    public IEnumerable<T> Select<T>(string where, string orderBy, params SqlParameter[] parameters) where T : ISourceModel<T>
    {
        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName} WHERE {where} ORDER BY {orderBy}";
        cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return T.FromReader(reader, semanticVersion);
        }
    }

    public IEnumerable<TResult> Select<TResult>(string query, Func<SqlDataReader, SemanticVersion, TResult> convertor, params SqlParameter[] parameters)
    {
        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return convertor(reader, semanticVersion);
        }
    }

    public T? SelectById<T>(int? id) where T : ISourceModel<T>
    {
        if (!id.HasValue)
        {
            return default;
        }

        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName} WHERE {T.GetPrimaryKeyName(semanticVersion)}={id}";
        using var reader = cmd.ExecuteReader();
        T? result = default;
        if (reader.Read())
        {
            result = T.FromReader(reader, semanticVersion);
        }

        if (reader.Read())
        {
            throw new InvalidOperationException("Multiple items were found by ID");
        }

        return result;
    }

    public bool TrySelectGuid<T>(int? id, out Guid? objectGuid) where T : ISourceModel<T>
    {
        if (!id.HasValue)
        {
            Unsafe.SkipInit(out objectGuid);
            return false;
        }

        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT {T.GuidColumnName} FROM {T.TableName} WHERE {T.GetPrimaryKeyName(semanticVersion)}={id}";
        object ret = cmd.ExecuteScalar();
        if (ret is Guid guid)
        {
            objectGuid = guid;
            return true;
        }

        if (ret is DBNull)
        {
            objectGuid = null;
            return true;
        }

        throw new InvalidOperationException($"Unexpected return value: '{ret}'");
    }

    public SemanticVersion SelectVersion()
    {
        if (semanticVersion != null)
        {
            return semanticVersion;
        }

        using var conn = GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE 'CMSDataVersion'";

        string cmsVersion = "";
        if (cmd.ExecuteScalar() is string cmsDbVersion)
        {
            cmsVersion += cmsDbVersion;
        }
        else
        {
            throw new InvalidOperationException("Unable to determine source instance version");
        }

        cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE 'CMSHotfixVersion'";
        if (cmd.ExecuteScalar() is string cmsHotfixVersion)
        {
            cmsVersion += $".{cmsHotfixVersion}";
        }
        else
        {
            throw new InvalidOperationException("Unable to determine source instance hotfix");
        }

        return SemanticVersion.TryParse(cmsVersion, out var version)
            ? version
            : throw new InvalidOperationException("Unable to determine source instance version");
    }

    private SqlConnection GetConnection() => new(configuration.KxConnectionString);

    public string HashPath(string path)
    {
        semanticVersion ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', LOWER(@path)), 2)";
        cmd.Parameters.AddWithValue("path", path);
        if (cmd.ExecuteScalar() is string s)
        {
            return s;
        }

        throw new InvalidOperationException($"Unable to hash path '{path}'");
    }
}
