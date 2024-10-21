using System.Globalization;

using CMS.Helpers;

using Microsoft.EntityFrameworkCore;

using Migration.Tool.K11;

namespace Migration.Tool.Core.K11.Helpers;

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

    public static string? GetSettingsKey(IDbContextFactory<K11Context> ctxf, int? siteId, string keyName)
    {
        using var k11Context = ctxf.CreateDbContext();
        var keys = k11Context.CmsSettingsKeys.Where(x => x.KeyName == keyName);

        return (keys.FirstOrDefault(x => x.SiteId == siteId)
                ?? keys.FirstOrDefault(x => x.SiteId == null))?.KeyValue;
    }

    public static T? GetSettingsKey<T>(IDbContextFactory<K11Context> ctxf, int? siteId, string keyName) where T : struct, IParsable<T>
    {
        using var k11Context = ctxf.CreateDbContext();
        var keys = k11Context.CmsSettingsKeys.Where(x => x.KeyName == keyName);
        string? value = (keys.FirstOrDefault(x => x.SiteId == siteId)
                         ?? keys.FirstOrDefault(x => x.SiteId == null))?.KeyValue;


        return T.TryParse(value, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    public static bool? TryGetSettingsKey<T>(IDbContextFactory<K11Context> ctxf, int? siteId, string keyName, out T? result) where T : IParsable<T> => T.TryParse(GetSettingsKey(ctxf, siteId, keyName), CultureInfo.InvariantCulture, out result);
}
