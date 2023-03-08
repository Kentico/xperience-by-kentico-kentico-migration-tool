using System.Diagnostics.Contracts;
using Microsoft.Extensions.Configuration;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Core.Helpers;

namespace Migration.Toolkit.CLI;

public enum ValidationMessageType
{
    Error,
    Warning,
}

public record ValidationMessage(ValidationMessageType Type, string Message, string? RecommendedFix = null);

public static class ConfigurationValidator
{
    [Pure]
    public static IEnumerable<ValidationMessage> GetValidationErrors(IConfigurationRoot root)
    {
        var settings = root.GetSection(ConfigurationNames.Settings);

        if (settings is null)
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_Settings_IsRequired);
        }

        if (
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.SourceConnectionString)) &&
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.KxConnectionString))
        )
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_SourceConnectionString_IsRequired);
        }

        if (
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.SourceCmsDirPath)) &&
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.KxCmsDirPath))
        )
        {
            yield return new ValidationMessage(ValidationMessageType.Warning,
                Resources.ConfigurationValidator_GetValidationErrors_SourceCmsDirPath_IsRecommended);
        }

        if (
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.TargetConnectionString)) &&
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.XbKConnectionString))
        )
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_TargetConnectionString_IsRequired);
        }

        if (
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.TargetCmsDirPath)) &&
            CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.XbKDirPath))
           )
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                Resources.ConfigurationValidator_GetValidationErrors_TargetCmsDirPath_IsRequired);
        }

        var targetKxpApiSettings =
                settings?.GetSection(ConfigurationNames.XbKApiSettings) ??
                settings?.GetSection(ConfigurationNames.TargetKxpApiSettings)
            ;

        if (targetKxpApiSettings is null)
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_TargetKxpApiSettings_IsRequired);
        }

        var connectionStrings = targetKxpApiSettings?.GetSection(ConfigurationNames.ConnectionStrings);
        if (connectionStrings is null)
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_ConnectionStrings_IsRequired);
        }

        if (CheckCfgValue(connectionStrings?.GetValue<string>(ConfigurationNames.CmsConnectionString)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_CmsConnectionString_IsRequired);
        }

        if (!StringIsNullOrFitsOneOf<AutofixEnum>(connectionStrings?.GetValue<string>(ConfigurationNames.UseOmActivityNodeRelationAutofix)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, string.Format(Resources.ConfigurationValidator_GetValidationErrors_UseOmActivityNodeRelationAutofix_MustFit, Printer.PrintEnumValues<AutofixEnum>(", ")));
        }

        if (!StringIsNullOrFitsOneOf<AutofixEnum>(connectionStrings?.GetValue<string>(ConfigurationNames.UseOmActivitySiteRelationAutofix)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, string.Format(Resources.ConfigurationValidator_GetValidationErrors_UseOmActivitySiteRelationAutofix_MustFit, Printer.PrintEnumValues<AutofixEnum>(", ")));
        }

        #region Opt-in features validation

        var optInFeatures = settings?.GetSection(ConfigurationNames.OptInFeatures);
        if (optInFeatures is not null)
        {
            var querySourceInstanceApi = optInFeatures?.GetSection(ConfigurationNames.QuerySourceInstanceApi);
            if (querySourceInstanceApi is not null)
            {
                var qsiEnabled = querySourceInstanceApi.GetValue<bool?>(ConfigurationNames.Enabled);
                if (qsiEnabled is true)
                {
                    var connections = querySourceInstanceApi.GetSection(ConfigurationNames.Connections).GetChildren();
                    foreach (var connection in connections)
                    {
                        var siteUri = connection.GetValue<string>(ConfigurationNames.SourceInstanceUri);
                        var secret = connection.GetValue<string>(ConfigurationNames.Secret);

                        if (Uri.TryCreate(siteUri, UriKind.Absolute, out var sourceSiteUri))
                        {
                            if (!sourceSiteUri.IsLoopback)
                            {
                                yield return new ValidationMessage(ValidationMessageType.Error,
                                    "Source instance Uri invalid format, following formats are supported: http://localhost:5531, https://localhost/MySite");
                            }

                            // OK
                        }
                        else
                        {
                            yield return new ValidationMessage(ValidationMessageType.Error,
                                "Source instance Uri invalid format, following formats are supported: http://localhost:5531, https://localhost/MySite");
                        }

                        if (string.IsNullOrWhiteSpace(secret))
                        {
                            yield return new ValidationMessage(ValidationMessageType.Error,
                                "Source instance secret cannot be null or whitespace, set it to random hardly guessed string.");
                        }
                    }
                }
            }

            var customMigrationModel = optInFeatures?.GetValue<CustomMigrationModel>(ConfigurationNames.CustomMigration);
            if (customMigrationModel is { FieldMigrations.Length: > 0 })
            {
                ValidationMessage Required(int item, string fieldName)
                {
                    return new ValidationMessage(
                        ValidationMessageType.Error,
                        $"Custom DataType migration at index [{item}] is missing value '{fieldName}', supply value or remove whole DataType migration."
                    );
                }

                var fieldMigrations = customMigrationModel.FieldMigrations;

                for (var i = 0; i < fieldMigrations.Length; i++)
                {
                    var current = fieldMigrations[i];
                    if (string.IsNullOrWhiteSpace(current.SourceDataType))
                    {
                        yield return Required(i, nameof(ConfigurationNames.SourceDataType));
                    }
                    if (string.IsNullOrWhiteSpace(current.TargetDataType))
                    {
                        yield return Required(i, nameof(ConfigurationNames.TargetDataType));
                    }
                    if (string.IsNullOrWhiteSpace(current.SourceFormControl))
                    {
                        yield return Required(i, nameof(ConfigurationNames.SourceFormControl));
                    }
                    if (string.IsNullOrWhiteSpace(current.TargetFormComponent))
                    {
                        yield return Required(i, nameof(ConfigurationNames.TargetFormComponent));
                    }
                }
            }
        }

        #endregion
    }

    #region "Helper methods"

    [Pure]
    private static bool StringIsNullOrFitsOneOf<TEnum>(string? s) where TEnum: Enum
    {
        return s is null || Enum.TryParse(ReflectionHelper<TEnum>.CurrentType, s, out var _);
    }

    [Pure]
    private static bool CheckCfgValue(string? s)
    {
        return string.IsNullOrWhiteSpace(s) || s == ConfigurationNames.TodoPlaceholder;
    }

    #endregion
}