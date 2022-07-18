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
        
        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.SourceConnectionString)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_SourceConnectionString_IsRequired);
        }

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.SourceCmsDirPath)))
        {
            yield return new ValidationMessage(ValidationMessageType.Warning, Resources.ConfigurationValidator_GetValidationErrors_SourceCmsDirPath_IsRecommended);
        }

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.TargetConnectionString)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_TargetConnectionString_IsRequired);
        }

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.TargetCmsDirPath)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error, Resources.ConfigurationValidator_GetValidationErrors_TargetCmsDirPath_IsRequired);
        }

        var targetKxpApiSettings = settings?.GetSection(ConfigurationNames.TargetKxpApiSettings);
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
            yield return new ValidationMessage(ValidationMessageType.Error, string.Format(Resources.ConfigurationValidator_GetValidationErrors_UseOmActivitySiteRelationAutofix_MustFit, Printer.PrintEnumValues<AutofixEnum>(", "))
            );
        }

        if (CheckCfgValue(settings?.GetValue<string>(ConfigurationNames.TargetAttachmentMediaLibraryName)))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                Resources.ConfigurationValidator_GetValidationErrors_TargetAttachmentMediaLibraryName_IsRequired,
                Resources.ConfigurationValidator_GetValidationErrors_TargetAttachmentMediaLibraryName_RecommendedFix
            );
        }
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