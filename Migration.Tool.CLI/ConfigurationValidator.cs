using System.Diagnostics.Contracts;

using Microsoft.Extensions.Configuration;

using Migration.Tool.Common;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Core.KX13.Helpers;

namespace Migration.Tool.CLI;

public enum ValidationMessageType
{
    Error,
    Warning
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

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.KxConnectionString)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_SourceConnectionString_IsRequired);
        }

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.KxCmsDirPath)))
        {
            yield return new ValidationMessage(ValidationMessageType.Warning, Resources.ConfigurationValidator_GetValidationErrors_SourceCmsDirPath_IsRecommended);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        if (settings?.GetValue<string>(ConfigurationNames.XbKConnectionString) is not null)
        {
            yield return new ValidationMessage(ValidationMessageType.Warning, $"Configuration key '{ConfigurationNames.XbKConnectionString}' is deprecated, use 'Settings:XbyKApiSettings:ConnectionStrings:CMSConnectionString' instead");
        }

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.XbKDirPath)) && CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.XbyKDirPath)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                Resources.ConfigurationValidator_GetValidationErrors_TargetCmsDirPath_IsRequired);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        var targetKxpApiSettings = settings?.GetSectionWithFallback(ConfigurationNames.XbyKApiSettings, ConfigurationNames.XbKApiSettings);
        if (targetKxpApiSettings?.Exists() != true)
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
                bool? qsiEnabled = querySourceInstanceApi.GetValue<bool?>(ConfigurationNames.Enabled);
                if (qsiEnabled is true)
                {
                    var connections = querySourceInstanceApi.GetSection(ConfigurationNames.Connections).GetChildren();
                    foreach (var connection in connections)
                    {
                        string? siteUri = connection.GetValue<string>(ConfigurationNames.SourceInstanceUri);
                        string? secret = connection.GetValue<string>(ConfigurationNames.Secret);

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
                static ValidationMessage Required(int item, string fieldName) => new(
                        ValidationMessageType.Error,
                        $"Custom DataType migration at index [{item}] is missing value '{fieldName}', supply value or remove whole DataType migration."
                    );

                var fieldMigrations = customMigrationModel.FieldMigrations;

                for (int i = 0; i < fieldMigrations.Length; i++)
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

        #region Commerce configuration validation

        var commerceConfiguration = settings?.GetSection(ConfigurationNames.CommerceConfiguration);
        if (commerceConfiguration is not null)
        {
            var systemFieldPrefix = commerceConfiguration.GetValue<string?>(ConfigurationNames.SystemFieldPrefix);
            if (systemFieldPrefix is not null && string.IsNullOrWhiteSpace(systemFieldPrefix))
            {
                yield return new ValidationMessage(ValidationMessageType.Error,
                    $"'{ConfigurationNames.CommerceConfiguration}:{ConfigurationNames.SystemFieldPrefix}' cannot be empty or whitespace when specified. Either provide a valid prefix value or remove the configuration to use the default.");
            }
        }

        #endregion
    }

    #region "Helper methods"

    [Pure]
    private static bool StringIsNullOrFitsOneOf<TEnum>(string? s) where TEnum : Enum => s is null || Enum.TryParse(ReflectionHelper<TEnum>.CurrentType, s, out object? _);

    [Pure]
    private static bool CheckCfgValue(string? s) => string.IsNullOrWhiteSpace(s) || s == ConfigurationNames.TodoPlaceholder;

    #endregion
}
