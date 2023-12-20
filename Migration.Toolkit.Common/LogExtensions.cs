namespace Migration.Toolkit.Common;

using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services;

public static class LogExtensions
{
    public static IPrintService PrintService = default;

    public static IModelMappingResult<TResult> Log<TResult>(this IModelMappingResult<TResult> mappingResult, ILogger logger, IMigrationProtocol protocol)
    {
        switch (mappingResult)
        {
            case { Success: false } result:
            {
                if (result is AggregatedResult<TResult> aggregatedResult)
                {
                    foreach (var r in aggregatedResult.Results)
                    {
                        protocol.Append(r.HandbookReference);
                        logger.LogError(r.HandbookReference?.ToString());
                    }
                }
                else
                {
                    protocol.Append(result.HandbookReference);
                    logger.LogError(result.HandbookReference?.ToString());
                }

                break;
            }
            case { Success: true } result:
            {
                logger.LogTrace("Success - {model}", PrintService.PrintKxpModelInfo(result.Item));
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(mappingResult));
            }
        }

        return mappingResult;
    }

    public static ILogger<T> LogEntitySetAction<T, TEntity>(this ILogger<T> logger, bool newInstance, TEntity entity)
    {
        var entityIdentityPrint = PrintService.GetEntityIdentityPrint(entity);
        logger.LogDebug("Command {Command}: {Action} {EntityIdentityPrint}", ReflectionHelper<T>.CurrentType.Name, newInstance ? "inserted" : "updated", entityIdentityPrint);
        return logger;
    }

    public static ILogger<T> LogEntitySetError<T, TEntity>(this ILogger<T> logger, Exception exception, bool newInstance, TEntity entity)
    {
        var entityIdentityPrint = PrintService.GetEntityIdentityPrint(entity);
        logger.LogError(exception, "Command {Command}: failed during {Action}, {EntityIdentityPrint}", ReflectionHelper<T>.CurrentType.Name, newInstance ? "insert" : "update", entityIdentityPrint);
        return logger;
    }

    public static ILogger<T> LogEntitiesSetError<T, TEntity>(this ILogger<T> logger, Exception exception, bool newInstance, IEnumerable<TEntity> entities)
    {
        var entityIdentityPrint = PrintService.GetEntityIdentityPrints(entities);
        logger.LogError(exception, "Command {Command}: failed during {Action}, {EntityIdentityPrint}", ReflectionHelper<T>.CurrentType.Name, newInstance ? "insert" : "update", entityIdentityPrint);
        return logger;
    }

    public static ILogger<T> LogErrorMissingDependency<T, TEntity>(this ILogger<T> logger, TEntity entity, string dependencyName, object dependencyValue, Type dependencyType)
    {
        var entityIdentityPrint = PrintService.GetEntityIdentityPrint(entity);
        logger.LogError("Command {Command}: {EntityIdentityPrint} is missing dependency {FieldName}={Value} of type {DependencyType}", ReflectionHelper<T>.CurrentType.Name, entityIdentityPrint, dependencyName, dependencyValue, dependencyType.Name);
        return logger;
    }
}