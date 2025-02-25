namespace Migration.Tool.Common;

public class ConfigurationNames
{
    public const string KxConnectionString = "KxConnectionString";
    public const string KxCmsDirPath = "KxCmsDirPath";

    [Obsolete("Not needed anymore, replaced by Settings:XbyKApiSettings:ConnectionStrings:CMSConnectionString")]
    public const string XbKConnectionString = "XbKConnectionString";

    [Obsolete("Replaced by XbyKDirPath")]
    public const string XbKDirPath = "XbKDirPath";
    public const string XbyKDirPath = "XbyKDirPath";

    public const string MigrateOnlyMediaFileInfo = "MigrateOnlyMediaFileInfo";
    public const string UseOmActivityNodeRelationAutofix = "UseOmActivityNodeRelationAutofix";
    public const string UseOmActivitySiteRelationAutofix = "UseOmActivitySiteRelationAutofix";
    public const string MigrationProtocolPath = "MigrationProtocolPath";
    public const string Enabled = "Enabled";
    public const string Connections = "Connections";
    public const string UrlProtocol = "UrlProtocol";

    public const string MemberIncludeUserSystemFields = "MemberIncludeUserSystemFields";

    public const string MigrateMediaToMediaLibrary = "MigrateMediaToMediaLibrary";
    public const string LegacyFlatAssetTree = "LegacyFlatAssetTree";
    public const string AssetRootFolders = "AssetRootFolders";

    public const string UseDeprecatedFolderPageType = "UseDeprecatedFolderPageType";

    public const string ExcludeCodeNames = "ExcludeCodeNames";
    public const string ConvertClassesToContentHub = "ConvertClassesToContentHub";
    public const string ExplicitPrimaryKeyMapping = "ExplicitPrimaryKeyMapping";

    public const string SiteName = "SiteName";
    public const string SourceInstanceUri = "SourceInstanceUri";
    public const string Secret = "Secret";

    public const string CreateReusableFieldSchemaForClasses = "CreateReusableFieldSchemaForClasses";

    public const string IncludeExtendedMetadata = "IncludeExtendedMetadata";

    public const string TodoPlaceholder = "[TODO]";

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
    public const string XbyKApiSettings = "XbyKApiSettings";

    #endregion
}
