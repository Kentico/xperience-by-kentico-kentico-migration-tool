namespace Migration.Toolkit.Source;

using System.Runtime.CompilerServices;
using CMS.DataEngine;
using Microsoft.Data.SqlClient;
using Migration.Toolkit.Common;

public class ModelFacade(ToolkitConfiguration configuration)
{
    private SemanticVersion? _version;

    public async IAsyncEnumerable<T> SelectAllAsync<T>([EnumeratorCancellation] CancellationToken cancellationToken) where T : ISourceModel<T>
    {
        _version ??= SelectVersion();

        await using var conn = GetConnection();
        await conn.OpenAsync(cancellationToken);
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName}";
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return T.FromReader(reader, _version);
        }
    }

    public IEnumerable<T> SelectAll<T>() where T : ISourceModel<T>
    {
        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName}";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return T.FromReader(reader, _version);
        }
    }

    public IEnumerable<T> SelectWhere<T>(string where, params SqlParameter[] parameters) where T : ISourceModel<T>
    {
        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName} WHERE {where}";
        cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return T.FromReader(reader, _version);
        }
    }

    public bool IsAvailable<T>() where T : ISourceModel<T>
    {
        _version ??= SelectVersion();
        return T.IsAvailable(_version);
    }

    public IEnumerable<T> Select<T>(string where, string orderBy, params SqlParameter[] parameters) where T : ISourceModel<T>
    {
        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName} WHERE {where} ORDER BY {orderBy}";
        cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return T.FromReader(reader, _version);
        }
    }

    public IEnumerable<TResult> Select<TResult>(string query, Func<SqlDataReader, SemanticVersion, TResult> convertor, params SqlParameter[] parameters)
    {
        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddRange(parameters);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return convertor(reader, _version);
        }
    }

    public T? SelectById<T>(int? id) where T : ISourceModel<T>
    {
        if (!id.HasValue) return default;
        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {T.TableName} WHERE {T.GetPrimaryKeyName(_version)}={id}";
        using var reader = cmd.ExecuteReader();
        reader.Read();
        var result = T.FromReader(reader, _version);
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

        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT {T.GuidColumnName} FROM {T.TableName} WHERE {T.GetPrimaryKeyName(_version)}={id}";
        var ret = cmd.ExecuteScalar();
        if (ret is Guid guid)
        {
            objectGuid = guid;
            return true;
        }
        else if (ret is DBNull)
        {
            objectGuid = null;
            return true;
        }
        else
        {
            throw new InvalidOperationException($"Unexpected return value: '{ret}'");
        }
    }

    public SemanticVersion SelectVersion()
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName LIKE 'CMSDataVersion'";

        var cmsVersion = "";
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

    private SqlConnection GetConnection()
    {
        return new SqlConnection(configuration.KxConnectionString);
    }

    public string HashPath(string path)
    {
        _version ??= SelectVersion();
        using var conn = GetConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', LOWER(@path)), 2)";
        cmd.Parameters.AddWithValue("path", path);
        if (cmd.ExecuteScalar() is string s)
        {
            return s;
        }

        throw new InvalidOperationException($"Unable to hash path '{path}'");
    }
}