namespace Migration.Toolkit.Common;

public class ConfigurationNames
{
    public const string SourceConnectionString = "SourceConnectionString";
    public const string SourceCmsDirPath = "SourceCmsDirPath";
    public const string TargetConnectionString = "TargetConnectionString";
    public const string TargetCmsDirPath = "TargetCmsDirPath";
    public const string TargetAttachmentMediaLibraryName = "TargetAttachmentMediaLibraryName";
    public const string MigrateOnlyMediaFileInfo = "MigrateOnlyMediaFileInfo";
    public const string UseOmActivityNodeRelationAutofix = "UseOmActivityNodeRelationAutofix";
    public const string UseOmActivitySiteRelationAutofix = "UseOmActivitySiteRelationAutofix";
    public const string MigrationProtocolPath = "MigrationProtocolPath";

    public const string ExcludeCodeNames = "ExcludeCodeNames";
    public const string ExplicitPrimaryKeyMapping = "ExplicitPrimaryKeyMapping";

    #region "Section names"

    public const string CmsConnectionString = "CMSConnectionString";
    public const string ConnectionStrings = "ConnectionStrings";
    public const string Settings = "Settings";
    public const string Logging = "Logging";
    public const string EntityConfigurations = "EntityConfigurations";

    [Obsolete("Use TargetKxpApiSettings const instead")]
    public const string TargetKxoApiSettings = "TargetKxoApiSettings";

    public const string TargetKxpApiSettings = "TargetKxpApiSettings";

    #endregion
    
    public const string TodoPlaceholder = "[TODO]";
}