using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineForms;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.Services;

namespace Migration.Tool.Core.KX13.Helpers;

public class Printer
{
    public static string PrintKxpModelInfo<T>(T model)
    {
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        return model switch
        {
#pragma warning disable CS0618 // Type or member is obsolete
            MediaLibraryInfo mediaLibrary => $"{currentTypeName}: {nameof(mediaLibrary.LibraryGUID)}={mediaLibrary.LibraryGUID}",
            MediaFileInfo mediaFile => $"{currentTypeName}: {nameof(mediaFile.FileGUID)}={mediaFile.FileGUID}",
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning disable CS0618 // Type or member is obsolete
            MediaLibraryInfo item => FormatModel($"ID={item.LibraryID}, GUID={item.LibraryGUID}, Name={item.LibraryName}"),
            MediaFileInfo item => FormatModel($"ID={item.FileID}, GUID={item.FileGUID}, Name={item.FileName}"),
#pragma warning restore CS0618 // Type or member is obsolete
            DataClassInfo item => FormatModel($"ID={item.ClassID}, GUID={item.ClassGUID}, Name={item.ClassName}"),

            CountryInfo item => FormatModel($"ID={item.CountryID}, GUID={item.CountryGUID}, Name={item.CountryName}"),
            StateInfo item => FormatModel($"ID={item.StateID}, GUID={item.StateGUID}, Name={item.StateName}"),

            ResourceInfo item => FormatModel($"ID={item.ResourceID}, Guid={item.ResourceGUID} Name={item.ResourceName}"),
            AlternativeFormInfo item => FormatModel($"ID={item.FormID}, Guid={item.FormGUID} Name={item.FormName}"),
            UserInfo item => FormatModel($"ID={item.UserID}, Guid={item.UserGUID} Name={item.UserName}"),
            RoleInfo item => FormatModel($"ID={item.RoleID}, Guid={item.RoleGUID} Name={item.RoleName}"),
            MemberInfo item => FormatModel($"ID={item.MemberID}, Guid={item.MemberGuid} Name={item.MemberName}"),

            BizFormInfo item => FormatModel($"ID={item.FormID}, GUID={item.FormGUID}, Name={item.FormName}"),
            ConsentInfo item => FormatModel($"ID={item.ConsentID}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            ConsentArchiveInfo item => FormatModel($"ID={item.ConsentArchiveID}, GUID={item.ConsentArchiveGuid}"),
            ConsentAgreementInfo item => FormatModel($"ID={item.ConsentAgreementID}, GUID={item.ConsentAgreementGuid}"),
            SettingsKeyInfo item => FormatModel($"ID={item.KeyID}, GUID={item.KeyGUID}, Name={item.KeyName}"),

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
