namespace Migration.Toolkit.Common;

public class ConfigurationNames
{
    public const string SourceConnectionString = "SourceConnectionString";
    public const string SourceCmsDirPath = "SourceCmsDirPath";
    public const string TargetConnectionString = "TargetConnectionString";
    public const string TargetCmsDirPath = "TargetCmsDirPath";
    public const string MigrateOnlyMediaFileInfo = "MigrateOnlyMediaFileInfo";
    public const string UseOmActivityNodeRelationAutofix = "UseOmActivityNodeRelationAutofix";
    public const string UseOmActivitySiteRelationAutofix = "UseOmActivitySiteRelationAutofix";
    public const string MigrationProtocolPath = "MigrationProtocolPath";
    public const string Enabled = "Enabled";
    public const string Connections = "Connections";

    public const string ExcludeCodeNames = "ExcludeCodeNames";
    public const string ExplicitPrimaryKeyMapping = "ExplicitPrimaryKeyMapping";
    
    public const string SourceInstanceUri = "SourceInstanceUri";
    public const string SourceInstanceSiteName = "SourceInstanceSiteName";
    public const string Secret = "Secret"; 

    #region "Section names"

    public const string CmsConnectionString = "CMSConnectionString";
    public const string ConnectionStrings = "ConnectionStrings";
    public const string Settings = "Settings";
    public const string Logging = "Logging";
    public const string EntityConfigurations = "EntityConfigurations";
    
    public const string OptInFeatures = "OptInFeatures";
    public const string QuerySourceInstanceApi = "QuerySourceInstanceApi";

    [Obsolete("Use TargetKxpApiSettings const instead")]
    public const string TargetKxoApiSettings = "TargetKxoApiSettings";

    public const string TargetKxpApiSettings = "TargetKxpApiSettings";

    #endregion
    
    public const string TodoPlaceholder = "[TODO]";
}