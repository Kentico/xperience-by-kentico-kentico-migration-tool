using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core;

public static class LogExtensions
{
    public static void LogResult<T, TResult>(this ModelMappingResult<TResult> mappingResult, ILogger<T> logger)
    {
        switch (mappingResult)
        {
            case ModelMappingFailed<TResult>(var message):
            {
                logger.LogError(message);
                break;
            }
            case ModelMappingFailedKeyMismatch<TResult>(var tResult, var success, var message, var newInstance):
            {
                logger.LogError(message);
                break;
            }
            case ModelMappingFailedSourceNotDefined<TResult>(var tResult, var success, var message, var newInstance):
            {
                logger.LogError(message);
                break;
            }
            case ModelMappingSuccess<TResult>(var tResult, var newInstance):
            {
                logger.LogTrace($"Model mapped successfully");
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(mappingResult));
            }
        }
    }
}