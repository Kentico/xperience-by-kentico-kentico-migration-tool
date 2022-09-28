namespace Migration.Toolkit.Common;

using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
public class OptInFeatures
{
    [ConfigurationKeyName(ConfigurationNames.QuerySourceInstanceApi)]
    public AdvancedFeatureQuerySourceInstanceApi? QuerySourceInstanceApi { get; set; }
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