namespace Migration.Toolkit.Core.Services;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Services.CmsClass;

public class FieldMigrationService // shall be singleton to cache necessary data
{
    private readonly ILogger<FieldMigrationService> _logger;
    private readonly FieldMigration[] _userDefinedMigrations;

    public FieldMigrationService(ToolkitConfiguration configuration, ILogger<FieldMigrationService> logger)
    {
        _logger = logger;

        var allUserDefinedMigrations = configuration.OptInFeatures?.CustomMigration?.FieldMigrations?.Select(fm =>
            new FieldMigration(
                fm.SourceDataType!,
                fm.TargetDataType!,
                fm.SourceFormControl!,
                fm.TargetFormComponent!,
                fm.Actions,
                fm.FieldNameRegex != null ? new Regex(fm.FieldNameRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase) : null
            )
        ).ToArray() ?? Array.Empty<FieldMigration>();
        _userDefinedMigrations = allUserDefinedMigrations;
    }

    public FieldMigration? GetFieldMigration(string sourceDataType, string? sourceFormControl, string? fieldName)
    {
        if (sourceFormControl == null)
        {
            _logger.LogDebug("Source field has no control defined '{SourceDataType}', field '{FieldName}'", sourceDataType, fieldName);
            return null;
        }

        var userDefined = GetFieldMigrationInternal(_userDefinedMigrations, sourceDataType, sourceFormControl, fieldName);
        if (userDefined is { })
        {
            _logger.LogDebug("Field migration matched: '{MatchType}', {Migration}", "UserDefined", userDefined);
            return userDefined;
        }

        var preDefined = GetFieldMigrationInternal(FieldMappingInstance.BuiltInFieldMigrations, sourceDataType, sourceFormControl, fieldName);
        if (preDefined is { })
        {
            _logger.LogDebug("Field migration matched: '{MatchType}', {Migration}", "BuiltIn", preDefined);
            return preDefined;
        }

        throw new InvalidOperationException($"No migration found for combination of '{sourceDataType}' datatype and '{sourceFormControl}'");
    }

    private static FieldMigration? GetFieldMigrationInternal(FieldMigration[] migrations, string sourceDataType, string? sourceFormControl, string? fieldName)
    {
        var matchedByDtFc = migrations.Where(x =>
            string.Equals(x.SourceDataType, sourceDataType, StringComparison.InvariantCultureIgnoreCase) &&
            string.Equals(x.SourceFormControl, sourceFormControl, StringComparison.InvariantCultureIgnoreCase)
        ).ToArray();

        if (matchedByDtFc is { Length: > 0 })
        {
            if (matchedByDtFc.LastOrDefault(x => fieldName != null && (x.FieldNameRegex?.IsMatch(fieldName) ?? false)) is { } exactMatch)
            {
                return exactMatch;
            }

            if (matchedByDtFc.LastOrDefault() is { } looseMatch)
            {
                return looseMatch;
            }
        }

        var generalMatch = migrations.LastOrDefault(x =>
            string.Equals(x.SourceDataType, sourceDataType, StringComparison.InvariantCultureIgnoreCase) &&
            string.Equals(x.SourceFormControl, SfcDirective.CatchAnyNonMatching, StringComparison.InvariantCultureIgnoreCase)
        );
        if (generalMatch is { })
        {
            return generalMatch;
        }

        return null;
    }
}