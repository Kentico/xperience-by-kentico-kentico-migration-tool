using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Services;

namespace Migration.Toolkit.Core.Contexts;

using Migration.Toolkit.Common.Services;

public class PrimaryKeyMappingContext : IPrimaryKeyMappingContext
{
    private readonly Dictionary<string, int> _mappings = new(StringComparer.OrdinalIgnoreCase);

    private readonly ILogger<PrimaryKeyMappingContext> _logger;
    private readonly IPrimaryKeyLocatorService _primaryKeyLocatorService;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    public PrimaryKeyMappingContext(
        ILogger<PrimaryKeyMappingContext> logger,
        IPrimaryKeyLocatorService primaryKeyLocatorService,
        ToolkitConfiguration toolkitConfiguration
    )
    {
        _logger = logger;
        _primaryKeyLocatorService = primaryKeyLocatorService;
        _toolkitConfiguration = toolkitConfiguration;
    }

    private int? GetExplicitMappingOrNull<T>(string memberName, int? sourceId)
    {
        if (sourceId == null) return null;

        var mappings = _toolkitConfiguration.EntityConfigurations?.GetEntityConfiguration<T>().ExplicitPrimaryKeyMapping;
        if (mappings?.TryGetValue(memberName, out var memberMappings) ?? false)
        {
            return memberMappings.TryGetValue($"{sourceId}", out var mappedId) ? mappedId : null;
        }

        return null;
    }

    private static string CreateKey<T>(Expression<Func<T, object>> keyNameSelector, int sourceId)
    {
        return $"{typeof(T).FullName}.{keyNameSelector.GetMemberName()}.{sourceId}";
    }

    public void SetMapping(Type type, string keyName, int sourceId, int targetId)
    {
        Debug.Assert(sourceId > 0, "sourceId > 0");
        Debug.Assert(targetId > 0, "targetId > 0");

        var foundProp = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(p => p.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));

        Debug.Assert(foundProp != null, "foundProp != null");

        var fullKeyName = $"{type.FullName}.{foundProp.Name}.{sourceId}";

        _mappings[fullKeyName] = targetId;
        _logger.LogTrace("Primary key for {FullKeyName} stored. {SourceId} maps to {TargetId}", fullKeyName, sourceId, targetId);
    }

    public void SetMapping<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, int targetId)
    {
        var fullKeyName = CreateKey(keyNameSelector, sourceId);
        _mappings[fullKeyName] = targetId;
        _logger.LogTrace("{Key}: {SourceValue}=>{TargetValue}", fullKeyName, sourceId, targetId);
    }

    public int RequireMapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int sourceId)
    {
        var memberName = keyNameSelector.GetMemberName();
        var fullKeyName = CreateKey(keyNameSelector, sourceId);
        if (sourceId == 0)
        {
            throw new MappingFailureException(fullKeyName, $"Cannot satisfy required mapping {fullKeyName} - source Id cannot be 0.");
        }

        if (GetExplicitMappingOrNull<T>(memberName, sourceId) is { } explicitlyMappedId)
        {
            _logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return explicitlyMappedId;
        }

        if (_mappings.TryGetValue(fullKeyName, out var resultId))
        {
            _logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return resultId;
        }

        _logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (_primaryKeyLocatorService.TryLocate(keyNameSelector, sourceId, out var targetId))
        {
            SetMapping(keyNameSelector, sourceId, targetId); // cache id
            _logger.LogTrace("{Key} located as {Value}", fullKeyName, resultId);
            return targetId;
        }

        throw new MappingFailureException(fullKeyName, "Target entity is missing");
    }

    public bool TryRequireMapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, out int targetIdResult)
    {
        targetIdResult = -1;
        if (!(sourceId is int sid))
        {
            return false;
        }

        var memberName = keyNameSelector.GetMemberName();
        var fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            throw new MappingFailureException(fullKeyName, $"Cannot satisfy required mapping {fullKeyName} - source Id cannot be 0.");
        }

        if (GetExplicitMappingOrNull<T>(memberName, sourceId) is { } explicitlyMappedId)
        {
            _logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            targetIdResult = explicitlyMappedId;
            return true;
        }

        if (_mappings.TryGetValue(fullKeyName, out var resultId))
        {
            _logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            targetIdResult = resultId;
            return true;
        }

        _logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (_primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out var targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            _logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
            targetIdResult = targetId;
            return true;
        }

        return false;
    }

    public int? MapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId)
    {
        if (sourceId is not { } sid)
        {
            return null;
        }

        var memberName = keyNameSelector.GetMemberName();
        var fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            _logger.LogWarning("{Key} Key locator invalid argument, cannot supply 0 as argument", fullKeyName);
            return null;
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { } explicitlyMappedId)
        {
            _logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return explicitlyMappedId;
        }

        if (_mappings.TryGetValue(fullKeyName, out var resultId))
        {
            _logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return resultId;
        }



        _logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (_primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out var targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            _logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
            return targetId;
        }

        throw new MappingFailureException(fullKeyName, $"Target entity is missing");
    }

    public int? MapFromSourceOrNull<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId)
    {
        if (sourceId is not { } sid)
        {
            return null;
        }

        var memberName = keyNameSelector.GetMemberName();
        var fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            _logger.LogWarning("{Key} Key locator invalid argument, cannot supply 0 as argument", fullKeyName);
            return null;
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { } explicitlyMappedId)
        {
            _logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return explicitlyMappedId;
        }

        if (_mappings.TryGetValue(fullKeyName, out var resultId))
        {
            _logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return resultId;
        }



        _logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (_primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out var targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            _logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
            return targetId;
        }

        return null;
    }

    public MapSourceIdResult MapSourceId<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, bool useLocator = true)
    {
        if (sourceId is not { } sid)
        {
            return new MapSourceIdResult(true, null);
        }

        var memberName = keyNameSelector.GetMemberName();
        var fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            _logger.LogWarning("{Key} Key locator invalid argument, cannot supply 0 as argument", fullKeyName);
            return new MapSourceIdResult(true, null);
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { } explicitlyMappedId)
        {
            _logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return new MapSourceIdResult(true, explicitlyMappedId);
        }

        if (_mappings.TryGetValue(fullKeyName, out var resultId))
        {
            _logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return new MapSourceIdResult(true, resultId);
        }



        _logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (useLocator && _primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out var targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            _logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
            return new MapSourceIdResult(true, targetId);
        }

        return new MapSourceIdResult(false, null);
    }

    public void PreloadDependencies<T>(Expression<Func<T, object>> keyNameSelector)
    {
        foreach (var (sourceId, targetId) in _primaryKeyLocatorService.SelectAll(keyNameSelector))
        {
            SetMapping(keyNameSelector, sourceId, targetId);
        }
    }

    public bool HasMapping<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, bool useLocator = true)
    {
        if (sourceId is not { } sid)
        {
            return true;
        }

        var memberName = keyNameSelector.GetMemberName();
        var fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            return true;
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { })
        {
            return true;
        }

        if (_mappings.TryGetValue(fullKeyName, out _))
        {
            return true;
        }

        if (useLocator && _primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out _))
        {
            return true;
        }

        return false;
    }
}