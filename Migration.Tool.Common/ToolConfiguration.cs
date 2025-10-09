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

    [ConfigurationKeyName(ConfigurationNames.IncludeExtendedMetadata)]
    public bool? IncludeExtendedMetadata { get; set; } = false;

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

    [ConfigurationKeyName(ConfigurationNames.LegacyFlatAssetTree)]
    public bool? LegacyFlatAssetTree { get; set; }

    [ConfigurationKeyName(ConfigurationNames.LegacyPermissiveMediaLibrarySubfolders)]
    public bool? LegacyPermissiveMediaLibrarySubfolders { get; set; }

    [ConfigurationKeyName(ConfigurationNames.AssetRootFolders)]
    public Dictionary<string, string>? AssetRootFolders { get; set; }

    [ConfigurationKeyName(ConfigurationNames.UseDeprecatedFolderPageType)]
    public bool? UseDeprecatedFolderPageType { get; set; }

    [ConfigurationKeyName(ConfigurationNames.CreateReusableFieldSchemaForClasses)]
    public string? CreateReusableFieldSchemaForClasses { get; set; }

    [ConfigurationKeyName(ConfigurationNames.ConvertClassesToContentHub)]
    public string? ConvertClassesToContentHub { get; set; }

    [ConfigurationKeyName(ConfigurationNames.CustomModuleClassDisplayNamePatterns)]
    public Dictionary<string, string>? CustomModuleClassDisplayNamePatterns { get; set; }

    public IReadOnlySet<string> ClassNamesCreateReusableSchema => classNamesCreateReusableSchema ??= new HashSet<string>(
        (CreateReusableFieldSchemaForClasses?.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries) ?? []).Select(x => x.Trim()),
        StringComparer.InvariantCultureIgnoreCase
    );

    public IReadOnlySet<string> ClassNamesConvertToContentHub => classNamesConvertToContentHub ??= new HashSet<string>(
        (ConvertClassesToContentHub?.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries) ?? []).Select(x => x.Trim()),
        StringComparer.InvariantCultureIgnoreCase
    );

    [ConfigurationKeyName(ConfigurationNames.TargetWorkspaceName)]
    public string? TargetWorkspaceName { get; set; }

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

#pragma warning disable CS0618 // Type or member is obsolete
    [ConfigurationKeyName(ConfigurationNames.XbKConnectionString)]
#pragma warning restore CS0618 // Type or member is obsolete
    public string XbKConnectionString
    {
        get => xbkConnectionString!;
        set => xbkConnectionString = value;
    }

    public void SetXbKConnectionStringIfNotEmpty(string? connectionString)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            xbkConnectionString = connectionString;
        }
    }

    #endregion

    #region Path to root directory of target instance

    private HashSet<string>? classNamesConvertToContentHub;
    private HashSet<string>? classNamesCreateReusableSchema;
    private string? xbkConnectionString;

#pragma warning disable CS0618 // Type or member is obsolete
    [ConfigurationKeyName(ConfigurationNames.XbKDirPath)]
#pragma warning restore CS0618 // Type or member is obsolete
    public string? XbKDirPath { get; set; } = null;

    [ConfigurationKeyName(ConfigurationNames.XbyKDirPath)]
    public string? XbyKDirPath { get; set; } = null;

    #endregion

    [ConfigurationKeyName(ConfigurationNames.UrlProtocol)]
    public string? UrlProtocol { get; set; }
}
