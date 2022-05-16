using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Core.Contexts;

public class PkMappingContext
{
    private Dictionary<string, int> _mappings = new(); 
    private readonly ILogger<PkMappingContext> _logger;

    public PkMappingContext(ILogger<PkMappingContext> logger)
    {
        _logger = logger;
    }

    public void SetMapping<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, int targetId)
    {
        Debug.Assert(sourceId > 0, "sourceId > 0");
        Debug.Assert(targetId > 0, "targetId > 0");
        
        var fullKeyName = $"{typeof(T).FullName}.{keyNameSelector.GetMemberName()}.{sourceId}";
        
        _mappings[fullKeyName] = targetId;
        _logger.LogTrace("Primary key for {fullKeyName} stored. {sourceId} maps to {targetId}", fullKeyName, sourceId, targetId);
    }
    
    public int MapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int sourceId)
    {
        Debug.Assert(sourceId > 0, "sourceId > 0");
        
        var fullKeyName = $"{typeof(T).FullName}.{keyNameSelector.GetMemberName()}.{sourceId}";

        return _mappings[fullKeyName];
    }
    
    public int? MapFromSourceNonRequired<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId)
    {
        if (sourceId == null) return null;
        
        Debug.Assert(sourceId > 0, "sourceId > 0");
        
        var fullKeyName = $"{typeof(T).FullName}.{keyNameSelector.GetMemberName()}.{sourceId}";
        if (_mappings.TryGetValue(fullKeyName, out var resultId))
        {
            return resultId;    
        }

        throw new InvalidOperationException($"Mapping with key {fullKeyName} is not present!");
    }
}