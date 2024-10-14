using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Source.Contexts;

public class PrimaryKeyMappingContext(
    ILogger<PrimaryKeyMappingContext> logger,
    IPrimaryKeyLocatorService primaryKeyLocatorService,
    ToolConfiguration toolConfiguration)
    : IPrimaryKeyMappingContext
{
    private readonly Dictionary<string, int> mappingCache = new(StringComparer.OrdinalIgnoreCase);

    public void SetMapping(Type type, string keyName, int sourceId, int targetId)
    {
        Debug.Assert(sourceId > 0, "sourceId > 0");
        Debug.Assert(targetId > 0, "targetId > 0");

        var foundProp = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(p => p.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));

        Debug.Assert(foundProp != null, "foundProp != null");

        string fullKeyName = $"{type.FullName}.{foundProp.Name}.{sourceId}";

        mappingCache[fullKeyName] = targetId;
        logger.LogTrace("Primary key for {FullKeyName} stored. {SourceId} maps to {TargetId}", fullKeyName, sourceId, targetId);
    }

    public void SetMapping<T>(Expression<Func<T, object>> keyNameSelector, int sourceId, int targetId)
    {
        string fullKeyName = CreateKey(keyNameSelector, sourceId);
        mappingCache[fullKeyName] = targetId;
        logger.LogTrace("{Key}: {SourceValue}=>{TargetValue}", fullKeyName, sourceId, targetId);
    }

    public int RequireMapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int sourceId)
    {
        string memberName = keyNameSelector.GetMemberName();
        string fullKeyName = CreateKey(keyNameSelector, sourceId);
        if (sourceId == 0)
        {
            throw new MappingFailureException(fullKeyName, $"Cannot satisfy required mapping {fullKeyName} - source Id cannot be 0.");
        }

        if (GetExplicitMappingOrNull<T>(memberName, sourceId) is { } explicitlyMappedId)
        {
            logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return explicitlyMappedId;
        }

        if (mappingCache.TryGetValue(fullKeyName, out int resultId))
        {
            logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return resultId;
        }

        logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (primaryKeyLocatorService.TryLocate(keyNameSelector, sourceId, out int targetId))
        {
            SetMapping(keyNameSelector, sourceId, targetId); // cache id
            logger.LogTrace("{Key} located as {Value}", fullKeyName, resultId);
            return targetId;
        }

        throw new MappingFailureException(fullKeyName, "Target entity is missing");
    }

    public bool TryRequireMapFromSource<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId, out int targetIdResult)
    {
        targetIdResult = -1;
        if (sourceId is not int sid)
        {
            return false;
        }

        string memberName = keyNameSelector.GetMemberName();
        string fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            throw new MappingFailureException(fullKeyName, $"Cannot satisfy required mapping {fullKeyName} - source Id cannot be 0.");
        }

        if (GetExplicitMappingOrNull<T>(memberName, sourceId) is { } explicitlyMappedId)
        {
            logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            targetIdResult = explicitlyMappedId;
            return true;
        }

        if (mappingCache.TryGetValue(fullKeyName, out int resultId))
        {
            logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            targetIdResult = resultId;
            return true;
        }

        logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out int targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
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

        string memberName = keyNameSelector.GetMemberName();
        string fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            logger.LogWarning("{Key} Key locator invalid argument, cannot supply 0 as argument", fullKeyName);
            return null;
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { } explicitlyMappedId)
        {
            logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return explicitlyMappedId;
        }

        if (mappingCache.TryGetValue(fullKeyName, out int resultId))
        {
            logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return resultId;
        }

        logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out int targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
            return targetId;
        }

        throw new MappingFailureException(fullKeyName, "Target entity is missing");
    }

    public int? MapFromSourceOrNull<T>(Expression<Func<T, object>> keyNameSelector, int? sourceId)
    {
        if (sourceId is not { } sid)
        {
            return null;
        }

        string memberName = keyNameSelector.GetMemberName();
        string fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            logger.LogWarning("{Key} Key locator invalid argument, cannot supply 0 as argument", fullKeyName);
            return null;
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { } explicitlyMappedId)
        {
            logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return explicitlyMappedId;
        }

        if (mappingCache.TryGetValue(fullKeyName, out int resultId))
        {
            logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return resultId;
        }

        logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out int targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
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

        string memberName = keyNameSelector.GetMemberName();
        string fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            logger.LogWarning("{Key} Key locator invalid argument, cannot supply 0 as argument", fullKeyName);
            return new MapSourceIdResult(true, null);
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is { } explicitlyMappedId)
        {
            logger.LogTrace("{Key} resolved as {Value} from explicit mapping", fullKeyName, explicitlyMappedId);
            return new MapSourceIdResult(true, explicitlyMappedId);
        }

        if (mappingCache.TryGetValue(fullKeyName, out int resultId))
        {
            logger.LogTrace("{Key} resolved as {Value}", fullKeyName, resultId);
            return new MapSourceIdResult(true, resultId);
        }

        logger.LogTrace("TryLocate {Key}", fullKeyName);
        if (useLocator && primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out int targetId))
        {
            SetMapping(keyNameSelector, sid, targetId); // cache id
            logger.LogTrace("{Key} located as {Value}", fullKeyName, targetId);
            return new MapSourceIdResult(true, targetId);
        }

        return new MapSourceIdResult(false, null);
    }

    public void PreloadDependencies<T>(Expression<Func<T, object>> keyNameSelector)
    {
        foreach ((int sourceId, int targetId) in primaryKeyLocatorService.SelectAll(keyNameSelector))
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

        string memberName = keyNameSelector.GetMemberName();
        string fullKeyName = CreateKey(keyNameSelector, sid);
        if (sid == 0)
        {
            return true;
        }

        if (GetExplicitMappingOrNull<T>(memberName, sid) is not null)
        {
            return true;
        }

        if (mappingCache.TryGetValue(fullKeyName, out _))
        {
            return true;
        }

        if (useLocator && primaryKeyLocatorService.TryLocate(keyNameSelector, sid, out _))
        {
            return true;
        }

        return false;
    }

    private int? GetExplicitMappingOrNull<T>(string memberName, int? sourceId)
    {
        if (sourceId == null)
        {
            return null;
        }

        var mappings = toolConfiguration.EntityConfigurations?.GetEntityConfiguration<T>().ExplicitPrimaryKeyMapping;
        if (mappings?.TryGetValue(memberName, out var memberMappings) ?? false)
        {
            return memberMappings.GetValueOrDefault($"{sourceId}");
        }

        return null;
    }

    private static string CreateKey<T>(Expression<Func<T, object>> keyNameSelector, int sourceId) => $"{typeof(T).FullName}.{keyNameSelector.GetMemberName()}.{sourceId}";
}
