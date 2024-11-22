using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineForms;
using CMS.Websites;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Services;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Helpers;

public class Printer
{
    public static string PrintKxpModelInfo<T>(T model)
    {
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        return model switch
        {
            MediaLibraryInfo mediaLibrary => $"{currentTypeName}: {nameof(mediaLibrary.LibraryGUID)}={mediaLibrary.LibraryGUID}",
            MediaFileInfo mediaFile => $"{currentTypeName}: {nameof(mediaFile.FileGUID)}={mediaFile.FileGUID}",
            RoleInfo role => $"{currentTypeName}: {nameof(role.RoleGUID)}={role.RoleGUID}, {nameof(role.RoleName)}={role.RoleName}",
            UserInfo user => $"{currentTypeName}: {nameof(user.UserGUID)}={user.UserGUID}, {nameof(user.UserName)}={user.UserName}",
            ResourceInfo resource => $"{currentTypeName}: {nameof(resource.ResourceGUID)}={resource.ResourceGUID}, {nameof(resource.ResourceName)}={resource.ResourceName}",
            SettingsCategoryInfo settingsCategory => $"{currentTypeName}: {nameof(settingsCategory.CategoryName)}={settingsCategory.CategoryName}",
            SettingsKeyInfo settingsKey => $"{currentTypeName}: {nameof(settingsKey.KeyGUID)}={settingsKey.KeyGUID}, {nameof(settingsKey.KeyName)}={settingsKey.KeyName}",
            BizFormInfo form => $"{currentTypeName}: {nameof(form.FormGUID)}={form.FormGUID}, {nameof(form.FormName)}={form.FormName}",
            ContactGroupInfo omContactGroup => $"{currentTypeName}: {nameof(omContactGroup.ContactGroupGUID)}={omContactGroup.ContactGroupGUID}, {nameof(omContactGroup.ContactGroupName)}={omContactGroup.ContactGroupName}",

            null => $"{currentTypeName}: <null>",
            _ => $"TODO: {typeof(T).FullName}"
        };
    }

    public static string GetEntityIdentityPrint<T>(T model, bool printType = true)
    {
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        string Fallback(object obj) => printType
                ? $"{currentTypeName}({SerializationHelper.SerializeOnlyNonComplexProperties(obj)})"
                : $"{SerializationHelper.SerializeOnlyNonComplexProperties(obj)}";

        string FormatModel(string inner) => printType
                ? $"{currentTypeName}({inner})"
                : $"{inner}";

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
            WebPageFormerUrlPathInfo item => FormatModel($"ID={item.WebPageFormerUrlPathID}, Guid=N/A Name={item.WebPageFormerUrlPath}"),

            BizFormInfo item => FormatModel($"ID={item.FormID}, GUID={item.FormGUID}, Name={item.FormName}"),
            ConsentInfo item => FormatModel($"ID={item.ConsentID}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            ConsentArchiveInfo item => FormatModel($"ID={item.ConsentArchiveID}, GUID={item.ConsentArchiveGuid}"),
            ConsentAgreementInfo item => FormatModel($"ID={item.ConsentAgreementID}, GUID={item.ConsentAgreementGuid}"),
            SettingsKeyInfo item => FormatModel($"ID={item.KeyID}, GUID={item.KeyGUID}, Name={item.KeyName}"),

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
