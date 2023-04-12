namespace Migration.Toolkit.Common;

public class ConfigurationNames
{
    [Obsolete("Use 'XbKConnectionString' instead.")]
    public const string SourceConnectionString = "SourceConnectionString";
    public const string KxConnectionString = "KxConnectionString";
    [Obsolete("Use 'XbKConnectionString' instead.")]
    public const string SourceCmsDirPath = "SourceCmsDirPath";
    public const string KxCmsDirPath = "KxCmsDirPath";

    [Obsolete("Use 'XbKConnectionString' instead.")]
    public const string TargetConnectionString = "TargetConnectionString";
    public const string XbKConnectionString = "XbKConnectionString";
    [Obsolete("Use 'XbKDirPath' instead.")]
    public const string TargetCmsDirPath = "TargetCmsDirPath";
    public const string XbKDirPath = "XbKDirPath";
    public const string MigrateOnlyMediaFileInfo = "MigrateOnlyMediaFileInfo";
    public const string UseOmActivityNodeRelationAutofix = "UseOmActivityNodeRelationAutofix";
    public const string UseOmActivitySiteRelationAutofix = "UseOmActivitySiteRelationAutofix";
    public const string MigrationProtocolPath = "MigrationProtocolPath";
    public const string Enabled = "Enabled";
    public const string Connections = "Connections";

    public const string MemberIncludeUserSystemFields = "MemberIncludeUserSystemFields";

    public const string ExcludeCodeNames = "ExcludeCodeNames";
    public const string ExplicitPrimaryKeyMapping = "ExplicitPrimaryKeyMapping";

    public const string SourceInstanceUri = "SourceInstanceUri";
    public const string Secret = "Secret";

    #region "Section names"

    public const string CmsConnectionString = "CMSConnectionString";
    public const string ConnectionStrings = "ConnectionStrings";
    public const string Settings = "Settings";
    public const string Logging = "Logging";
    public const string EntityConfigurations = "EntityConfigurations";

    public const string OptInFeatures = "OptInFeatures";
    public const string QuerySourceInstanceApi = "QuerySourceInstanceApi";

    public const string CustomMigration = "CustomMigration";

    public const string SourceDataType = "SourceDataType";
    public const string TargetDataType = "TargetDataType";
    public const string SourceFormControl = "SourceFormControl";
    public const string TargetFormComponent = "TargetFormComponent";
    public const string Actions = "Actions";
    public const string FieldNameRegex = "FieldNameRegex";

    [Obsolete("Use TargetKxpApiSettings const instead")]
    public const string TargetKxoApiSettings = "TargetKxoApiSettings";
    [Obsolete("Use XbKApiSettings const instead")]
    public const string TargetKxpApiSettings = "TargetKxpApiSettings";
    public const string XbKApiSettings = "XbKApiSettings";

    #endregion

    public const string TodoPlaceholder = "[TODO]";
}