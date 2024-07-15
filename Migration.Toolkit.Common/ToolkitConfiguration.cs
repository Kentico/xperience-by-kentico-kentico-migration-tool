using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Migration.Toolkit.Common;

using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Autofix enum
/// </summary>
/// <remarks>do not update value names, they are used in json configuration</remarks>
public enum AutofixEnum
{
    DiscardData,
    AttemptFix,
    Error,
}

public class ToolkitConfiguration
{
    #region Connection string of source instance

    private string? _kxConnectionString;

    [ConfigurationKeyName(ConfigurationNames.KxConnectionString)]
    public string KxConnectionString
    {
        get => _kxConnectionString!;
        set => _kxConnectionString = value;
    }

    #endregion

    #region Path to CMS dir of source instance

    [ConfigurationKeyName(ConfigurationNames.KxCmsDirPath)]
    public string? KxCmsDirPath { get; set; }

    #endregion

    #region Connection string of target instance

    [ConfigurationKeyName(ConfigurationNames.XbKConnectionString)]
    public string XbKConnectionString
    {
        get => _xbKConnectionString!;
        set => _xbKConnectionString = value;
    }

    public void SetXbKConnectionStringIfNotEmpty(string? connectionString)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            _xbKConnectionString = connectionString;
        }
    }

    #endregion

    #region Path to root directory of target instance

    private HashSet<string>? _classNamesCreateReusableSchema;
    private string? _xbKConnectionString;

    [ConfigurationKeyName(ConfigurationNames.XbKDirPath)]
    public string? XbKDirPath { get; set; } = null;

    #endregion

    [ConfigurationKeyName(ConfigurationNames.EntityConfigurations)]
    public EntityConfigurations EntityConfigurations { get; set; } = new();

    [ConfigurationKeyName(ConfigurationNames.MigrateOnlyMediaFileInfo)]
    public bool? MigrateOnlyMediaFileInfo { get; set; } = true;

    [ConfigurationKeyName(ConfigurationNames.UseOmActivityNodeRelationAutofix)]
    public AutofixEnum? UseOmActivityNodeRelationAutofix { get; set; } = AutofixEnum.Error;

    [ConfigurationKeyName(ConfigurationNames.UseOmActivitySiteRelationAutofix)]
    public AutofixEnum? UseOmActivitySiteRelationAutofix { get; set; } = AutofixEnum.Error;

    [ConfigurationKeyName(ConfigurationNames.MigrationProtocolPath)]
    public string? MigrationProtocolPath { get; set; }

    [ConfigurationKeyName(ConfigurationNames.MemberIncludeUserSystemFields)]
    public string? MemberIncludeUserSystemFields { get; set; }

    [ConfigurationKeyName(ConfigurationNames.CreateReusableFieldSchemaForClasses)]
    public string? CreateReusableFieldSchemaForClasses { get; set; }


    public IReadOnlySet<string> ClassNamesCreateReusableSchema => _classNamesCreateReusableSchema ??= new HashSet<string>(
        (CreateReusableFieldSchemaForClasses?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) ?? []).Select(x => x.Trim()),
        StringComparer.InvariantCultureIgnoreCase
    );

    #region Opt-in features

    [ConfigurationKeyName(ConfigurationNames.OptInFeatures)]
    public OptInFeatures? OptInFeatures { get; set; }

    #endregion
}