namespace Migration.Toolkit.Core.Configuration;

public class GlobalConfiguration
{
    /// <summary>
    /// Mapping source SiteID to target SiteID
    /// </summary>
    public Dictionary<int?, int?> SiteIdMapping { get; set; }
}