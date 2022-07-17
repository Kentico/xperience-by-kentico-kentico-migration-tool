using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Helpers;

namespace Migration.Toolkit.Core.Services;

public class TableReflectionService
{
    private readonly ILogger<TableReflectionService> _logger;
    private readonly Dictionary<string,Type> _tableNameLookup;

    public TableReflectionService(ILogger<TableReflectionService> logger)
    {
        _logger = logger;
        var (_, tableNameLookup) = typeof(KX13.Context.KX13Context).Assembly.GetTypes().Aggregate((
            nameLookup: new Dictionary<string, Type>(),
            tableNameLookup: new Dictionary<string, Type>()
        ), (lookups, type) =>
        {
            var rh = new ReflectionHelper(type);

            if (rh.GetFirstAttributeOrNull<TableAttribute>()?.Name is {} tableName && !string.IsNullOrWhiteSpace(tableName))
            {
                lookups.tableNameLookup[tableName] = type;
                lookups.nameLookup[type.Name] = type;
            }

            return lookups;
        });

        this._tableNameLookup = tableNameLookup;
    }

    public Type GetSourceTableTypeByTableName(string tableName)
    {
        if (!_tableNameLookup.ContainsKey(tableName))
        {
            var joinedKeys = string.Join(", ", _tableNameLookup.Keys);
            _logger.LogError("Invalid table name, use one of following: {TableNames}", joinedKeys);
            throw new KeyNotFoundException($"Invalid table name, use one of following: {joinedKeys}");
        }
        return _tableNameLookup[tableName];
    }
}