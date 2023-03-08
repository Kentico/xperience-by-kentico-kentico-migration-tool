namespace Migration.Toolkit.Common;

using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
public class OptInFeatures
{
    [ConfigurationKeyName(ConfigurationNames.QuerySourceInstanceApi)]
    public AdvancedFeatureQuerySourceInstanceApi? QuerySourceInstanceApi { get; set; }

    [ConfigurationKeyName(ConfigurationNames.CustomMigration)]
    public CustomMigrationModel? CustomMigration { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class AdvancedFeatureQuerySourceInstanceApi
{
    [ConfigurationKeyName(ConfigurationNames.Enabled)]
    public bool Enabled { get; set; } = false;

    [ConfigurationKeyName(ConfigurationNames.Connections)]
    public List<SourceInstanceInfo> Connections { get; set; } = new();
}

// ReSharper disable once ClassNeverInstantiated.Global
public class SourceInstanceInfo
{
    [ConfigurationKeyName(ConfigurationNames.SourceInstanceUri)]
    public Uri? SourceInstanceUri { get; set; } = null!;

    [ConfigurationKeyName(ConfigurationNames.Secret)]
    public string Secret { get; set; } = null!;
}

// ReSharper disable once ClassNeverInstantiated.Global
public class FieldMigrationSerializable
{
    [ConfigurationKeyName(ConfigurationNames.SourceDataType)]
    public string? SourceDataType { get; set; }
    [ConfigurationKeyName(ConfigurationNames.TargetDataType)]
    public string? TargetDataType { get; set; }
    [ConfigurationKeyName(ConfigurationNames.SourceFormControl)]
    public string? SourceFormControl { get; set; }
    [ConfigurationKeyName(ConfigurationNames.TargetFormComponent)]
    public string? TargetFormComponent { get; set; }
    [ConfigurationKeyName(ConfigurationNames.Actions)]
    public string[]? Actions { get; set; }
    [ConfigurationKeyName(ConfigurationNames.FieldNameRegex)]
    public string? FieldNameRegex { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomMigrationModel
{
    public FieldMigrationSerializable[]? FieldMigrations { get; set; }
}