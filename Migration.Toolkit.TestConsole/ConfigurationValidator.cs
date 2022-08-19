namespace Migration.Toolkit.TestConsole;

using System.Diagnostics.Contracts;
using Microsoft.Extensions.Configuration;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Core.Helpers;

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
        var settings = root.GetSection("Settings");

        if (settings is null)
        {
            yield return new ValidationMessage(ValidationMessageType.Error, "Section 'Settings' is required");
        }
        
        if (CheckCfgValue(settings?.GetValue<string>("SourceConnectionString")))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                "Configuration value in path 'Settings.SourceConnectionString' is required");
        }

        if (CheckCfgValue(settings?.GetValue<string>("SourceCmsDirPath")))
        {
            yield return new ValidationMessage(ValidationMessageType.Warning,
                "Configuration value in path 'Settings.SourceCmsDirPath' is empty, it is recommended to set source instance filesystem path");
        }

        if (CheckCfgValue(settings?.GetValue<string>("TargetConnectionString")))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                "Configuration value in path 'Settings.TargetConnectionString' is required");
        }

        if (CheckCfgValue(settings?.GetValue<string>("TargetCmsDirPath")))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                "Configuration value in path 'Settings.TargetCmsDirPath' is required");
        }

        var targetKxoApiSettings = settings?.GetSection("TargetKxoApiSettings");
        if (targetKxoApiSettings is null)
        {
            yield return new ValidationMessage(ValidationMessageType.Error, "Section 'Settings.TargetKxoApiSettings' is required");
        }

        var connectionStrings = targetKxoApiSettings?.GetSection("ConnectionStrings");
        if (connectionStrings is null)
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                "Section 'Settings.TargetKxoApiSettings.ConnectionStrings' is required");
        }

        if (CheckCfgValue(connectionStrings?.GetValue<string>("CMSConnectionString")))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                "Configuration value in path  'Settings.TargetKxoApiSettings.ConnectionStrings.CMSConnectionString' is required");
        }

        if (!StringIsNullOrFitsOneOf<AutofixEnum>(connectionStrings?.GetValue<string>("UseOmActivityNodeRelationAutofix")))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                $"Configuration value in path 'Settings.UseOmActivityNodeRelationAutofix' must fit one of: {Printer.PrintEnumValues<AutofixEnum>(", ")}"
            );
        }

        if (!StringIsNullOrFitsOneOf<AutofixEnum>(connectionStrings?.GetValue<string>("UseOmActivitySiteRelationAutofix")))
        {
            yield return new ValidationMessage(ValidationMessageType.Error,
                $"Configuration value in path 'Settings.UseOmActivitySiteRelationAutofix' must fit one of: {Printer.PrintEnumValues<AutofixEnum>(", ")}"
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
        return string.IsNullOrWhiteSpace(s) || s == "[TODO]";
    }

    #endregion
}