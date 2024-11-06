using Microsoft.Extensions.Configuration;

namespace Migration.Tool.Common;

/// <summary>
///     Autofix enum
/// </summary>
/// <remarks>do not update value names, they are used in json configuration</remarks>
public enum AutofixEnum
{
    DiscardData,
    AttemptFix,
    Error
}

public class ToolConfiguration
{
    #region Path to CMS dir of source instance

    [ConfigurationKeyName(ConfigurationNames.KxCmsDirPath)]
    public string? KxCmsDirPath { get; set; }

    #endregion

    [ConfigurationKeyName(ConfigurationNames.EntityConfigurations)]
    public EntityConfigurations EntityConfigurations { get; set; } = [];

    [ConfigurationKeyName(ConfigurationNames.MigrateOnlyMediaFileInfo)]
    public bool? MigrateOnlyMediaFileInfo { get; set; } = false;

    [ConfigurationKeyName(ConfigurationNames.UseOmActivityNodeRelationAutofix)]
    public AutofixEnum? UseOmActivityNodeRelationAutofix { get; set; } = AutofixEnum.Error;

    [ConfigurationKeyName(ConfigurationNames.UseOmActivitySiteRelationAutofix)]
    public AutofixEnum? UseOmActivitySiteRelationAutofix { get; set; } = AutofixEnum.Error;

    [ConfigurationKeyName(ConfigurationNames.MigrationProtocolPath)]
    public string? MigrationProtocolPath { get; set; }

    [ConfigurationKeyName(ConfigurationNames.MemberIncludeUserSystemFields)]
    public string? MemberIncludeUserSystemFields { get; set; }

    [ConfigurationKeyName(ConfigurationNames.MigrateMediaToMediaLibrary)]
    public bool MigrateMediaToMediaLibrary { get; set; }

    [ConfigurationKeyName(ConfigurationNames.UseDeprecatedFolderPageType)]
    public bool? UseDeprecatedFolderPageType { get; set; }

    [ConfigurationKeyName(ConfigurationNames.CreateReusableFieldSchemaForClasses)]
    public string? CreateReusableFieldSchemaForClasses { get; set; }

    public IReadOnlySet<string> ClassNamesCreateReusableSchema => classNamesCreateReusableSchema ??= new HashSet<string>(
        (CreateReusableFieldSchemaForClasses?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) ?? []).Select(x => x.Trim()),
        StringComparer.InvariantCultureIgnoreCase
    );

    #region Opt-in features

    [ConfigurationKeyName(ConfigurationNames.OptInFeatures)]
    public OptInFeatures? OptInFeatures { get; set; }

    #endregion

    #region Connection string of source instance

    private string? kxConnectionString;

    [ConfigurationKeyName(ConfigurationNames.KxConnectionString)]
    public string KxConnectionString
    {
        get => kxConnectionString!;
        set => kxConnectionString = value;
    }

    #endregion

    #region Connection string of target instance

    [ConfigurationKeyName(ConfigurationNames.XbKConnectionString)]
    public string XbKConnectionString
    {
        get => xbKConnectionString!;
        set => xbKConnectionString = value;
    }

    public void SetXbKConnectionStringIfNotEmpty(string? connectionString)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            xbKConnectionString = connectionString;
        }
    }

    #endregion

    #region Path to root directory of target instance

    private HashSet<string>? classNamesCreateReusableSchema;
    private string? xbKConnectionString;

    [ConfigurationKeyName(ConfigurationNames.XbKDirPath)]
    public string? XbKDirPath { get; set; } = null;

    #endregion

    [ConfigurationKeyName(ConfigurationNames.UrlProtocol)]
    public string? UrlProtocol { get; set; }
}
