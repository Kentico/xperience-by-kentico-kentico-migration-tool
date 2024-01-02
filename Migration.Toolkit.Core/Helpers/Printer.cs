using CMS.DataEngine;
using CMS.MediaLibrary;
using Migration.Toolkit.Common.Helpers;
using CMS.Globalization;
using CMS.Membership;
using CMS.Modules;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.Helpers;

using Migration.Toolkit.Common.Services;

public class Printer
{
    public static string PrintKxpModelInfo<T>(T model)
    {
        var currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        return model switch
        {
            MediaLibrary mediaLibrary => $"{currentTypeName}: {nameof(mediaLibrary.LibraryGuid)}={mediaLibrary.LibraryGuid}",
            MediaFile mediaFile => $"{currentTypeName}: {nameof(mediaFile.FileGuid)}={mediaFile.FileGuid}",
            CmsRole role => $"{currentTypeName}: {nameof(role.RoleGuid)}={role.RoleGuid}, {nameof(role.RoleName)}={role.RoleName}",
            CmsUser user => $"{currentTypeName}: {nameof(user.UserGuid)}={user.UserGuid}, {nameof(user.UserName)}={user.UserName}",
            CmsResource resource => $"{currentTypeName}: {nameof(resource.ResourceGuid)}={resource.ResourceGuid}, {nameof(resource.ResourceName)}={resource.ResourceName}",
            CmsSettingsCategory settingsCategory => $"{currentTypeName}: {nameof(settingsCategory.CategoryName)}={settingsCategory.CategoryName}",
            CmsSettingsKey settingsKey => $"{currentTypeName}: {nameof(settingsKey.KeyGuid)}={settingsKey.KeyGuid}, {nameof(settingsKey.KeyName)}={settingsKey.KeyName}",
            CmsForm form => $"{currentTypeName}: {nameof(form.FormGuid)}={form.FormGuid}, {nameof(form.FormName)}={form.FormName}",
            OmContactGroup omContactGroup => $"{currentTypeName}: {nameof(omContactGroup.ContactGroupGuid)}={omContactGroup.ContactGroupGuid}, {nameof(omContactGroup.ContactGroupName)}={omContactGroup.ContactGroupName}",

            null => $"{currentTypeName}: <null>",
            _ => $"TODO: {typeof(T).FullName}"
        };
    }

    public static string GetEntityIdentityPrint<T>(T model, bool printType = true)
    {
        var currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        string Fallback(object obj) => printType
            ? $"{currentTypeName}({SerializationHelper.SerializeOnlyNonComplexProperties(obj)})"
            : $"{SerializationHelper.SerializeOnlyNonComplexProperties(obj)}"
        ;

        string FormatModel(string inner) => printType
            ? $"{currentTypeName}({inner})"
            : $"{inner}"
        ;

        return model switch
        {
            MediaLibraryInfo item => FormatModel($"ID={item.LibraryID}, GUID={item.LibraryGUID}, Name={item.LibraryName}"),
            MediaFileInfo item => FormatModel($"ID={item.FileID}, GUID={item.FileGUID}, Name={item.FileName}"),
            DataClassInfo item => FormatModel($"ID={item.ClassID}, GUID={item.ClassGUID}, Name={item.ClassName}"),

            CountryInfo item => FormatModel($"ID={item.CountryID}, GUID={item.CountryGUID}, Name={item.CountryName}"),
            StateInfo item => FormatModel($"ID={item.StateID}, GUID={item.StateGUID}, Name={item.StateName}"),

            ResourceInfo item => FormatModel($"ID={item.ResourceID}, Guid={item.ResourceGUID} Name={item.ResourceName}"),
            CMS.FormEngine.AlternativeFormInfo item => FormatModel($"ID={item.FormID}, Guid={item.FormGUID} Name={item.FormName}"),
            UserInfo item => FormatModel($"ID={item.UserID}, Guid={item.UserGUID} Name={item.UserName}"),
            RoleInfo item => FormatModel($"ID={item.RoleID}, Guid={item.RoleGUID} Name={item.RoleName}"),
            MemberInfo item => FormatModel($"ID={item.MemberID}, Guid={item.MemberGuid} Name={item.MemberName}"),

            CmsForm item => FormatModel($"ID={item.FormId}, GUID={item.FormGuid}, Name={item.FormName}"),
            CmsUser item => FormatModel($"ID={item.UserId}, GUID={item.UserGuid}, Name={item.UserName}"),
            CmsConsent item => FormatModel($"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            CmsConsentArchive item => FormatModel($"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}"),
            CmsConsentAgreement item => FormatModel($"ID={item.ConsentAgreementId}, GUID={item.ConsentAgreementGuid}"),
            CmsSettingsKey item => FormatModel($"ID={item.KeyId}, GUID={item.KeyGuid}, Name={item.KeyName}"),

            KX13M.CmsPageTemplateConfiguration item => FormatModel($"ID={item.PageTemplateConfigurationId}, GUID={item.PageTemplateConfigurationGuid}, Name={item.PageTemplateConfigurationName}, SiteId={item.PageTemplateConfigurationSiteId}"),
            KX13M.CmsRole item => FormatModel($"ID={item.RoleId}, GUID={item.RoleGuid}, Name={item.RoleName}, SiteId={item.SiteId}"),
            KX13M.CmsAttachment item => FormatModel($"ID={item.AttachmentId}, GUID={item.AttachmentGuid}, Name={item.AttachmentName}"),
            KX13M.CmsClass item => FormatModel($"ID={item.ClassId}, GUID={item.ClassGuid}, Name={item.ClassName}"),
            KX13M.CmsConsent item => FormatModel($"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            KX13M.CmsConsentArchive item => FormatModel($"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}"),
            KX13M.CmsConsentAgreement item => FormatModel($"ID={item.ConsentAgreementId}, GUID={item.ConsentAgreementGuid}"),
            KX13M.CmsCountry item => FormatModel($"ID={item.CountryId}, GUID={item.CountryGuid}, Name={item.CountryName}"),
            KX13M.CmsState item => FormatModel($"ID={item.StateId}, GUID={item.StateGuid}, Name={item.StateName}"),
            KX13M.CmsTree item => FormatModel($"NodeID={item.NodeId}, NodeGUID={item.NodeGuid}, NodeName={item.NodeName}, NodeAliasPath={item.NodeAliasPath}"),
            KX13M.CmsDocument item => FormatModel($"NodeID={item.DocumentNodeId}, DocumentID={item.DocumentId}, DocumentGUID={item.DocumentGuid}, DocumentCulture={item.DocumentCulture}, DocumentName={item.DocumentName}"),
            KX13M.CmsResource item => FormatModel($"ID={item.ResourceId}, GUID={item.ResourceGuid}, Name={item.ResourceName}"),

            null => $"<null> ref of {currentTypeName}",
            _ => Fallback(model)
        };
    }

    public static string GetEntityIdentityPrints<T>(IEnumerable<T> models, string separator = "|")
    {
        return string.Join(separator, models.Select(m => GetEntityIdentityPrint(m, false)));
    }

    public static string PrintEnumValues<TEnum>(string separator) where TEnum: struct, Enum
    {
        return string.Join(separator, Enum.GetValues<TEnum>());
    }
}

public class PrintService: IPrintService {
    public string PrintKxpModelInfo<T>(T model)
    {
        return Printer.PrintKxpModelInfo(model);
    }

    public string GetEntityIdentityPrint<T>(T model, bool printType = true)
    {
        return Printer.GetEntityIdentityPrint<T>(model, printType);
    }

    public string GetEntityIdentityPrints<T>(IEnumerable<T> models, string separator = "|")
    {
        return Printer.GetEntityIdentityPrints(models, separator);
    }

    public string PrintEnumValues<TEnum>(string separator) where TEnum : struct, Enum
    {
        return Printer.PrintEnumValues<TEnum>(separator);
    }
}