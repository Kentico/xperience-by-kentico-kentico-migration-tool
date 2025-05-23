using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;

namespace Migration.Tool.KXP.Api.Services.CmsClass;

public class FieldMigrationService // shall be singleton to cache necessary data
    : IFieldMigrationService
{
    private readonly ILogger<FieldMigrationService> logger;
    private readonly IList<IFieldMigration> fieldMigrations;
    private readonly FieldMigration[] userDefinedMigrations;

    public FieldMigrationService(ToolConfiguration configuration, ILogger<FieldMigrationService> logger, IEnumerable<IFieldMigration> fieldMigrations)
    {
        this.logger = logger;
        this.fieldMigrations = fieldMigrations.OrderBy(x => x.Rank).ToList();

        var allUserDefinedMigrations = configuration.OptInFeatures?.CustomMigration?.FieldMigrations?.Select(fm =>
            new FieldMigration(
                fm.SourceDataType!,
                fm.TargetDataType!,
                fm.SourceFormControl!,
                fm.TargetFormComponent!,
                fm.Actions,
                fm.FieldNameRegex != null ? new Regex(fm.FieldNameRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase) : null
            )
        ).ToArray() ?? [];
        userDefinedMigrations = allUserDefinedMigrations;
    }

    public IFieldMigration? GetFieldMigration(FieldMigrationContext fieldMigrationContext, bool allowNullSourceFormControl = false)
    {
        foreach (var fieldMigrator in fieldMigrations)
        {
            if (fieldMigrator.ShallMigrate(fieldMigrationContext))
            {
                return fieldMigrator;
            }
        }

        (string? sourceDataType, string? sourceFormControl, string? fieldName, _) = fieldMigrationContext;
        if (!allowNullSourceFormControl && sourceFormControl == null)
        {
            logger.LogDebug("Source field has no control defined '{SourceDataType}', field '{FieldName}'", sourceDataType, fieldName);
            return null;
        }

        var userDefined = GetFieldMigrationInternal(userDefinedMigrations, sourceDataType, sourceFormControl, fieldName);
        if (userDefined is not null)
        {
            logger.LogDebug("Field migration matched: '{MatchType}', {Migration}", "UserDefined", userDefined);
            return userDefined;
        }

        var preDefined = GetFieldMigrationInternal(FieldMappingInstance.BuiltInFieldMigrations, sourceDataType, sourceFormControl, fieldName, allowNullSourceFormControl);
        if (preDefined is not null)
        {
            logger.LogDebug("Field migration matched: '{MatchType}', {Migration}", "BuiltIn", preDefined);
            return preDefined;
        }

        throw new InvalidOperationException($"No migration found for combination of '{sourceDataType}' datatype and '{sourceFormControl}'");
    }

    private static FieldMigration? GetFieldMigrationInternal(FieldMigration[] migrations, string sourceDataType, string? sourceFormControl, string? fieldName, bool allowNullSourceFormControl = false)
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
        if (generalMatch is not null)
        {
            return generalMatch;
        }

        if (allowNullSourceFormControl)
        {
            var typeOnlyMatch = migrations.LastOrDefault(x =>
                string.Equals(x.SourceDataType, sourceDataType, StringComparison.InvariantCultureIgnoreCase)
            );
            if (typeOnlyMatch is not null)
            {
                return typeOnlyMatch;
            }
        }

        return null;
    }
}
