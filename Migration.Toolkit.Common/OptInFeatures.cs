namespace Migration.Toolkit.Common;

using System.Text.Json.Serialization;

// ReSharper disable once ClassNeverInstantiated.Global
public class OptInFeatures
{
    [JsonPropertyName(ConfigurationNames.QuerySourceInstanceApi)]
    public AdvancedFeatureQuerySourceInstanceApi? QuerySourceInstanceApi { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class AdvancedFeatureQuerySourceInstanceApi
{
    [JsonPropertyName(ConfigurationNames.Enabled)]
    public bool Enabled { get; set; } = false;

    [JsonPropertyName(ConfigurationNames.Connections)]
    public List<SourceInstanceInfo> Connections { get; set; } = new();
}

// ReSharper disable once ClassNeverInstantiated.Global
public class SourceInstanceInfo
{
    [JsonPropertyName(ConfigurationNames.SourceInstanceUri)]
    public Uri? SourceInstanceUri { get; set; } = null!;

    [JsonPropertyName(ConfigurationNames.Secret)]
    public string Secret { get; set; } = null!;
}