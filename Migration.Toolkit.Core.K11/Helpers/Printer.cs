
using CMS.DataEngine;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.Membership;
using CMS.Modules;

using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.Services;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Helpers;
public class Printer
{
    public static string PrintKxpModelInfo<T>(T model)
    {
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

        return model switch
        {
            KXP.Models.MediaLibrary mediaLibrary => $"{currentTypeName}: {nameof(mediaLibrary.LibraryGuid)}={mediaLibrary.LibraryGuid}",
            KXP.Models.MediaFile mediaFile => $"{currentTypeName}: {nameof(mediaFile.FileGuid)}={mediaFile.FileGuid}",
            KXP.Models.CmsRole role => $"{currentTypeName}: {nameof(role.RoleGuid)}={role.RoleGuid}, {nameof(role.RoleName)}={role.RoleName}",
            KXP.Models.CmsUser user => $"{currentTypeName}: {nameof(user.UserGuid)}={user.UserGuid}, {nameof(user.UserName)}={user.UserName}",
            KXP.Models.CmsResource resource => $"{currentTypeName}: {nameof(resource.ResourceGuid)}={resource.ResourceGuid}, {nameof(resource.ResourceName)}={resource.ResourceName}",
            KXP.Models.CmsSettingsCategory settingsCategory => $"{currentTypeName}: {nameof(settingsCategory.CategoryName)}={settingsCategory.CategoryName}",
            KXP.Models.CmsSettingsKey settingsKey => $"{currentTypeName}: {nameof(settingsKey.KeyGuid)}={settingsKey.KeyGuid}, {nameof(settingsKey.KeyName)}={settingsKey.KeyName}",
            KXP.Models.CmsForm form => $"{currentTypeName}: {nameof(form.FormGuid)}={form.FormGuid}, {nameof(form.FormName)}={form.FormName}",
            KXP.Models.OmContactGroup omContactGroup => $"{currentTypeName}: {nameof(omContactGroup.ContactGroupGuid)}={omContactGroup.ContactGroupGuid}, {nameof(omContactGroup.ContactGroupName)}={omContactGroup.ContactGroupName}",

            null => $"{currentTypeName}: <null>",
            _ => $"TODO: {typeof(T).FullName}"
        };
    }

    public static string GetEntityIdentityPrint<T>(T model, bool printType = true)
    {
        string currentTypeName = ReflectionHelper<T>.CurrentType.Name;

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

            KXP.Models.CmsForm item => FormatModel($"ID={item.FormId}, GUID={item.FormGuid}, Name={item.FormName}"),
            KXP.Models.CmsUser item => FormatModel($"ID={item.UserId}, GUID={item.UserGuid}, Name={item.UserName}"),
            KXP.Models.CmsConsent item => FormatModel($"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            KXP.Models.CmsConsentArchive item => FormatModel($"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}"),
            KXP.Models.CmsConsentAgreement item => FormatModel($"ID={item.ConsentAgreementId}, GUID={item.ConsentAgreementGuid}"),
            KXP.Models.CmsSettingsKey item => FormatModel($"ID={item.KeyId}, GUID={item.KeyGuid}, Name={item.KeyName}"),

            CmsRole item => FormatModel($"ID={item.RoleId}, GUID={item.RoleGuid}, Name={item.RoleName}, SiteId={item.SiteId}"),
            CmsAttachment item => FormatModel($"ID={item.AttachmentId}, GUID={item.AttachmentGuid}, Name={item.AttachmentName}"),
            CmsClass item => FormatModel($"ID={item.ClassId}, GUID={item.ClassGuid}, Name={item.ClassName}"),
            CmsConsent item => FormatModel($"ID={item.ConsentId}, GUID={item.ConsentGuid}, Name={item.ConsentName}"),
            CmsConsentArchive item => FormatModel($"ID={item.ConsentArchiveId}, GUID={item.ConsentArchiveGuid}"),
            CmsConsentAgreement item => FormatModel($"ID={item.ConsentAgreementId}, GUID={item.ConsentAgreementGuid}"),
            CmsCountry item => FormatModel($"ID={item.CountryId}, GUID={item.CountryGuid}, Name={item.CountryName}"),
            CmsState item => FormatModel($"ID={item.StateId}, GUID={item.StateGuid}, Name={item.StateName}"),
            CmsTree item => FormatModel($"NodeID={item.NodeId}, NodeGUID={item.NodeGuid}, NodeName={item.NodeName}, NodeAliasPath={item.NodeAliasPath}"),
            CmsDocument item => FormatModel($"NodeID={item.DocumentNodeId}, DocumentID={item.DocumentId}, DocumentGUID={item.DocumentGuid}, DocumentCulture={item.DocumentCulture}, DocumentName={item.DocumentName}"),
            CmsResource item => FormatModel($"ID={item.ResourceId}, GUID={item.ResourceGuid}, Name={item.ResourceName}"),

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
