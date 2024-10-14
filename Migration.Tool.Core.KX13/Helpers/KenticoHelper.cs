using System.Globalization;

using CMS.Helpers;

using Microsoft.EntityFrameworkCore;

using Migration.Tool.KX13.Context;

namespace Migration.Tool.Core.KX13.Helpers;

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

    public static string? GetSettingsKey(IDbContextFactory<KX13Context> ctxf, int? siteId, string keyName)
    {
        using var kx13Context = ctxf.CreateDbContext();
        var keys = kx13Context.CmsSettingsKeys.Where(x => x.KeyName == keyName);

        return (keys.FirstOrDefault(x => x.SiteId == siteId)
                ?? keys.FirstOrDefault(x => x.SiteId == null))?.KeyValue;
    }

    public static T? GetSettingsKey<T>(IDbContextFactory<KX13Context> ctxf, int? siteId, string keyName) where T : struct, IParsable<T>
    {
        using var kx13Context = ctxf.CreateDbContext();
        var keys = kx13Context.CmsSettingsKeys.Where(x => x.KeyName == keyName);
        string? value = (keys.FirstOrDefault(x => x.SiteId == siteId)
                         ?? keys.FirstOrDefault(x => x.SiteId == null))?.KeyValue;


        return T.TryParse(value, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    public static bool? TryGetSettingsKey<T>(IDbContextFactory<KX13Context> ctxf, int? siteId, string keyName, out T? result) where T : IParsable<T> => T.TryParse(GetSettingsKey(ctxf, siteId, keyName), CultureInfo.InvariantCulture, out result);
}
