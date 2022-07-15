using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.MediaLibrary;
using Migration.Toolkit.Common.Helpers;

namespace Migration.Toolkit.Core.Helpers;

public static class Printer
{
    public static string PrintKxoModelInfo<T>(T model)
    {
        var currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        return model switch
        {
            KXO.Models.CmsSite site => $"{currentTypeName}: {nameof(site.SiteGuid)}={site.SiteGuid}",
            KXO.Models.MediaLibrary mediaLibrary => $"{currentTypeName}: {nameof(mediaLibrary.LibraryGuid)}={mediaLibrary.LibraryGuid}",
            KXO.Models.MediaFile mediaFile => $"{currentTypeName}: {nameof(mediaFile.FileGuid)}={mediaFile.FileGuid}",
            KXO.Models.CmsTree tree => $"{currentTypeName}: {nameof(tree.NodeGuid)}={tree.NodeGuid}",
            KXO.Models.CmsDocument document => $"{currentTypeName}: {nameof(document.DocumentGuid)}={document.DocumentGuid}",
            KXO.Models.CmsAcl acl => $"{currentTypeName}: {nameof(acl.Aclguid)}={acl.Aclguid}",
            KXO.Models.CmsRole role => $"{currentTypeName}: {nameof(role.RoleGuid)}={role.RoleGuid}, {nameof(role.RoleName)}={role.RoleName}",
            KXO.Models.CmsUser user => $"{currentTypeName}: {nameof(user.UserGuid)}={user.UserGuid}, {nameof(user.UserName)}={user.UserName}",
            KXO.Models.CmsResource resource => $"{currentTypeName}: {nameof(resource.ResourceGuid)}={resource.ResourceGuid}, {nameof(resource.ResourceName)}={resource.ResourceName}",
            KXO.Models.CmsSettingsCategory settingsCategory => $"{currentTypeName}: {nameof(settingsCategory.CategoryName)}={settingsCategory.CategoryName}",
            KXO.Models.CmsSettingsKey settingsKey => $"{currentTypeName}: {nameof(settingsKey.KeyGuid)}={settingsKey.KeyGuid}, {nameof(settingsKey.KeyName)}={settingsKey.KeyName}",
            KXO.Models.CmsForm form => $"{currentTypeName}: {nameof(form.FormGuid)}={form.FormGuid}, {nameof(form.FormName)}={form.FormName}",
            KXO.Models.CmsPageUrlPath pageUrlPath => $"{currentTypeName}: {nameof(pageUrlPath.PageUrlPathGuid)}={pageUrlPath.PageUrlPathGuid}, {nameof(pageUrlPath.PageUrlPathUrlPath)}={pageUrlPath.PageUrlPathUrlPath}",
            KXO.Models.OmContactGroup omContactGroup => $"{currentTypeName}: {nameof(omContactGroup.ContactGroupGuid)}={omContactGroup.ContactGroupGuid}, {nameof(omContactGroup.ContactGroupName)}={omContactGroup.ContactGroupName}",
            
            null => $"{currentTypeName}: <null>",
            _ => $"TODO: {typeof(T).FullName}"
        };
    }

    public static string GetEntityIdentityPrint<T>(T model)
    {
        var currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        return model switch
        {
            MediaLibraryInfo item => $"ID={item.LibraryID}, GUID={item.LibraryGUID}, Name={item.LibraryName}",
            MediaFileInfo item => $"ID={item.FileID}, GUID={item.FileGUID}, Name={item.FileName}",
            DataClassInfo item => $"ID={item.ClassID}, GUID={item.ClassGUID}, Name={item.ClassName}",
            PageUrlPathInfo item => $"ID={item.PageUrlPathID}, GUID={item.PageUrlPathGUID}, UrlPath={item.PageUrlPathUrlPath}",
            TreeNode item => $"NodeID={item.NodeID}, DocumentID={item.DocumentID}, NodeGUID={item.NodeGUID}, DocumentGUID={item.DocumentGUID}, DocumentCulture={item.DocumentCulture}, DocumentName={item.DocumentName}",
            
            KXO.Models.CmsForm item => $"ID={item.FormId}, GUID={item.FormGuid}, Name={item.FormName}",
            KXO.Models.CmsUser item => $"ID={item.UserId}, GUID={item.UserGuid}, Name={item.UserName}",
            KXO.Models.CmsConsent item => $"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}",
            KXO.Models.CmsConsentArchive item => $"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}",
            KXO.Models.CmsSettingsKey item => $"ID={item.KeyId}, GUID={item.KeyGuid}, Name={item.KeyName}, SiteId={item.SiteId}",
            KXO.Models.CmsSite item => $"ID={item.SiteId}, GUID={item.SiteGuid}, Name={item.SiteName}",
            KX13M.CmsRole item => $"ID={item.RoleId}, GUID={item.RoleGuid}, Name={item.RoleName}, SiteId={item.SiteId}",
            
            KX13M.CmsAttachment item => $"ID={item.AttachmentId}, GUID={item.AttachmentGuid}, Name={item.AttachmentName}",
            KX13M.CmsClass item => $"ID={item.ClassId}, GUID={item.ClassGuid}, Name={item.ClassName}",
            KX13M.CmsConsent item => $"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}",
            KX13M.CmsConsentArchive item => $"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}",
            
            null => $"<null> of {currentTypeName}",
            _ => $"TODO: {ReflectionHelper<T>.CurrentType.FullName}"
        };
        
        // throw new NotImplementedException($"No entity identity print defined for type '{ReflectionHelper<T>.CurrentType.FullName}'");
    }

    public static string PrintEnumValues<TEnum>(string separator) where TEnum: struct, Enum
    {
        return string.Join(separator, Enum.GetValues<TEnum>());
    }
}