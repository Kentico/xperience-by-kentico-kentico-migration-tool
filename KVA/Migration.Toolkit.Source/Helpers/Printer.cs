using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Modules;

using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Helpers;

public class Printer
{
    public static string PrintKxpModelInfo<T>(T model)
    {
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

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
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        string Fallback(object obj)
        {
            return printType
                ? $"{currentTypeName}({SerializationHelper.SerializeOnlyNonComplexProperties(obj)})"
                : $"{SerializationHelper.SerializeOnlyNonComplexProperties(obj)}";
        }

        string FormatModel(string inner)
        {
            return printType
                ? $"{currentTypeName}({inner})"
                : $"{inner}";
        }

        return model switch
        {
            MediaLibraryInfo item => FormatModel($"ID={item.LibraryID}, GUID={item.LibraryGUID}, Name={item.LibraryName}"),
            MediaFileInfo item => FormatModel($"ID={item.FileID}, GUID={item.FileGUID}, Name={item.FileName}"),
            DataClassInfo item => FormatModel($"ID={item.ClassID}, GUID={item.ClassGUID}, Name={item.ClassName}"),

            CountryInfo item => FormatModel($"ID={item.CountryID}, GUID={item.CountryGUID}, Name={item.CountryName}"),
            StateInfo item => FormatModel($"ID={item.StateID}, GUID={item.StateGUID}, Name={item.StateName}"),

            ResourceInfo item => FormatModel($"ID={item.ResourceID}, Guid={item.ResourceGUID} Name={item.ResourceName}"),
            AlternativeFormInfo item => FormatModel($"ID={item.FormID}, Guid={item.FormGUID} Name={item.FormName}"),
            UserInfo item => FormatModel($"ID={item.UserID}, Guid={item.UserGUID} Name={item.UserName}"),
            RoleInfo item => FormatModel($"ID={item.RoleID}, Guid={item.RoleGUID} Name={item.RoleName}"),
            MemberInfo item => FormatModel($"ID={item.MemberID}, Guid={item.MemberGuid} Name={item.MemberName}"),

            CmsForm item => FormatModel($"ID={item.FormId}, GUID={item.FormGuid}, Name={item.FormName}"),
            CmsUser item => FormatModel($"ID={item.UserId}, GUID={item.UserGuid}, Name={item.UserName}"),
            CmsConsent item => FormatModel($"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            CmsConsentArchive item => FormatModel($"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}"),
            CmsConsentAgreement item => FormatModel($"ID={item.ConsentAgreementId}, GUID={item.ConsentAgreementGuid}"),
            CmsSettingsKey item => FormatModel($"ID={item.KeyId}, GUID={item.KeyGuid}, Name={item.KeyName}"),

            CmsPageTemplateConfigurationK11 item => FormatModel($"Item not exists in K11 {item}"),
            CmsPageTemplateConfigurationK12 item => FormatModel($"ID={item.PageTemplateConfigurationID}, GUID={item.PageTemplateConfigurationGUID}, Name={item.PageTemplateConfigurationName}, SiteId={item.PageTemplateConfigurationSiteID}"),
            CmsPageTemplateConfigurationK13 item => FormatModel($"ID={item.PageTemplateConfigurationID}, GUID={item.PageTemplateConfigurationGUID}, Name={item.PageTemplateConfigurationName}, SiteId={item.PageTemplateConfigurationSiteID}"),
            ICmsRole item => FormatModel($"ID={item.RoleID}, GUID={item.RoleGUID}, Name={item.RoleName}, SiteId={item.SiteID}"),
            ICmsAttachment item => FormatModel($"ID={item.AttachmentID}, GUID={item.AttachmentGUID}, Name={item.AttachmentName}"),
            ICmsClass item => FormatModel($"ID={item.ClassID}, GUID={item.ClassGUID}, Name={item.ClassName}"),
            ICmsConsent item => FormatModel($"ID={item.ConsentID}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            ICmsConsentArchive item => FormatModel($"ID={item.ConsentArchiveID}, GUID={item.ConsentArchiveGuid}"),
            ICmsConsentAgreement item => FormatModel($"ID={item.ConsentAgreementID}, GUID={item.ConsentAgreementGuid}"),
            ICmsCountry item => FormatModel($"ID={item.CountryID}, GUID={item.CountryGUID}, Name={item.CountryName}"),
            ICmsState item => FormatModel($"ID={item.StateID}, GUID={item.StateGUID}, Name={item.StateName}"),
            ICmsTree item => FormatModel($"NodeID={item.NodeID}, NodeGUID={item.NodeGUID}, NodeName={item.NodeName}, NodeAliasPath={item.NodeAliasPath}"),
            ICmsDocument item => FormatModel($"NodeID={item.DocumentNodeID}, DocumentID={item.DocumentID}, DocumentGUID={item.DocumentGUID}, DocumentCulture={item.DocumentCulture}, DocumentName={item.DocumentName}"),
            ICmsResource item => FormatModel($"ID={item.ResourceID}, GUID={item.ResourceGUID}, Name={item.ResourceName}"),

            null => $"<null> ref of {currentTypeName}",
            _ => Fallback(model)
        };
    }

    public static string GetEntityIdentityPrints<T>(IEnumerable<T> models, string separator = "|") => string.Join(separator, models.Select(m => GetEntityIdentityPrint(m, false)));

    public static string PrintEnumValues<TEnum>(string separator) where TEnum : struct, Enum => string.Join(separator, Enum.GetValues<TEnum>());
}

public class PrintService : IPrintService
{
    public string PrintKxpModelInfo<T>(T model) => Printer.PrintKxpModelInfo(model);

    public string GetEntityIdentityPrint<T>(T model, bool printType = true) => Printer.GetEntityIdentityPrint(model, printType);

    public string GetEntityIdentityPrints<T>(IEnumerable<T> models, string separator = "|") => Printer.GetEntityIdentityPrints(models, separator);

    public string PrintEnumValues<TEnum>(string separator) where TEnum : struct, Enum => Printer.PrintEnumValues<TEnum>(separator);
}
