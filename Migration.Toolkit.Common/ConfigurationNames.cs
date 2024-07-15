namespace Migration.Toolkit.Common;

public class ConfigurationNames
{
    public const string KxConnectionString = "KxConnectionString";
    public const string KxCmsDirPath = "KxCmsDirPath";

    [Obsolete("not needed anymore, connection string from Kentico config section is used")]
    public const string XbKConnectionString = "XbKConnectionString";

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

    public const string SiteName = "SiteName";
    public const string SourceInstanceUri = "SourceInstanceUri";
    public const string Secret = "Secret";

    public const string CreateReusableFieldSchemaForClasses = "CreateReusableFieldSchemaForClasses";

    #region "Section names"

    public const string CmsConnectionString = "CMSConnectionString";
    public const string ConnectionStrings = "ConnectionStrings";
    public const string Settings = "Settings";
    public const string Logging = "Logging";
    public const string File = "File";
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

    public const string XbKApiSettings = "XbKApiSettings";

    #endregion

    public const string TodoPlaceholder = "[TODO]";
}