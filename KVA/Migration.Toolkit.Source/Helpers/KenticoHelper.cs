using System.Globalization;
using CMS.Helpers;
using Microsoft.Data.SqlClient;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Helpers;

public static class KenticoHelper
{
    public static void CopyCustomData(ContainerCustomData target, string? sourceXml)
    {
        var customNodeData = new ContainerCustomData();
        customNodeData.LoadData(sourceXml);
        foreach (string? columnName in customNodeData.ColumnNames)
        {
            target.SetValue(columnName, customNodeData.GetValue(columnName));
        }
    }
    
    public static string? GetSettingsKey(ModelFacade facade, int? siteId, string keyName)
    {
        var keys = facade.Select<ICmsSettingsKey>("KeyName = @keyName", "SiteID", new SqlParameter("keyName", keyName)).ToList();
        return (keys.FirstOrDefault(x => x.SiteID == siteId)
                ?? keys.FirstOrDefault(x => x.SiteID == null))?.KeyValue;
    }

    public static T? GetSettingsKey<T>(ModelFacade facade, int? siteId, string keyName) where T : struct, IParsable<T>
    {
        var keys = facade.Select<ICmsSettingsKey>("KeyName = @keyName", "SiteID", new SqlParameter("keyName", keyName)).ToList();
        string? value = (keys.FirstOrDefault(x => x.SiteID == siteId)
                         ?? keys.FirstOrDefault(x => x.SiteID == null))?.KeyValue;


        return T.TryParse(value, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }
}
