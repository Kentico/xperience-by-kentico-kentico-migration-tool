namespace Migration.Toolkit.Core.MigrateSettingKeys;

public record CmsSettingsKeyKey(string KeyName, int? SiteId, Guid KeyGuid)
{
    public override string ToString()
    {
        return $"KN={KeyName.ToString().PadLeft(60,' ')} SID={SiteId} G={KeyGuid}";
    }

    public static CmsSettingsKeyKey? From(Migration.Toolkit.KX13.Models.CmsSettingsKey? cmsSettingsKey) =>
        cmsSettingsKey == null ? null : new(cmsSettingsKey.KeyName, cmsSettingsKey.SiteId, cmsSettingsKey.KeyGuid);

    public static CmsSettingsKeyKey? From(Migration.Toolkit.KXO.Models.CmsSettingsKey? cmsSettingsKey) =>
        cmsSettingsKey == null ? null : new(cmsSettingsKey.KeyName, cmsSettingsKey.SiteId, cmsSettingsKey.KeyGuid);
    
    public static CmsSettingsKeyKey From(string? keyName, int? siteId, Guid keyGuid)
    {
        ArgumentNullException.ThrowIfNull(keyName);
        
        return new(keyName, siteId, keyGuid);
    }
}