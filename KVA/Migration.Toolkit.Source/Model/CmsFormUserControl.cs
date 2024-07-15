namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsFormUserControl : ISourceModel<ICmsFormUserControl>
{
    int UserControlID { get; }
    string UserControlDisplayName { get; }
    string UserControlCodeName { get; }
    string UserControlFileName { get; }
    bool UserControlForText { get; }
    bool UserControlForLongText { get; }
    bool UserControlForInteger { get; }
    bool UserControlForDecimal { get; }
    bool UserControlForDateTime { get; }
    bool UserControlForBoolean { get; }
    bool UserControlForFile { get; }
    bool? UserControlShowInDocumentTypes { get; }
    bool? UserControlShowInSystemTables { get; }
    bool? UserControlShowInWebParts { get; }
    bool? UserControlShowInReports { get; }
    Guid UserControlGUID { get; }
    DateTime UserControlLastModified { get; }
    bool UserControlForGuid { get; }
    bool? UserControlShowInCustomTables { get; }
    string? UserControlParameters { get; }
    bool UserControlForDocAttachments { get; }
    int? UserControlResourceID { get; }
    int? UserControlParentID { get; }
    string? UserControlDescription { get; }
    int? UserControlPriority { get; }
    bool? UserControlIsSystem { get; }
    bool UserControlForBinary { get; }
    bool UserControlForDocRelationships { get; }
    string? UserControlAssemblyName { get; }
    string? UserControlClassName { get; }

    static string ISourceModel<ICmsFormUserControl>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsFormUserControlK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsFormUserControlK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsFormUserControlK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsFormUserControl>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsFormUserControlK11.IsAvailable(version),
            { Major: 12 } => CmsFormUserControlK12.IsAvailable(version),
            { Major: 13 } => CmsFormUserControlK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsFormUserControl>.TableName => "CMS_FormUserControl";
    static string ISourceModel<ICmsFormUserControl>.GuidColumnName => "UserControlGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsFormUserControl ISourceModel<ICmsFormUserControl>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsFormUserControlK11.FromReader(reader, version),
            { Major: 12 } => CmsFormUserControlK12.FromReader(reader, version),
            { Major: 13 } => CmsFormUserControlK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsFormUserControlK11(int UserControlID, string UserControlDisplayName, string UserControlCodeName, string UserControlFileName, bool UserControlForText, bool UserControlForLongText, bool UserControlForInteger, bool UserControlForDecimal, bool UserControlForDateTime, bool UserControlForBoolean, bool UserControlForFile, bool UserControlShowInBizForms, string UserControlDefaultDataType, int? UserControlDefaultDataTypeSize, bool? UserControlShowInDocumentTypes, bool? UserControlShowInSystemTables, bool? UserControlShowInWebParts, bool? UserControlShowInReports, Guid UserControlGUID, DateTime UserControlLastModified, bool UserControlForGuid, bool? UserControlShowInCustomTables, bool UserControlForVisibility, string? UserControlParameters, bool UserControlForDocAttachments, int? UserControlResourceID, int? UserControlType, int? UserControlParentID, string? UserControlDescription, Guid? UserControlThumbnailGUID, int? UserControlPriority, bool? UserControlIsSystem, bool UserControlForBinary, bool UserControlForDocRelationships, string? UserControlAssemblyName, string? UserControlClassName) : ICmsFormUserControl, ISourceModel<CmsFormUserControlK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserControlID";
    public static string TableName => "CMS_FormUserControl";
    public static string GuidColumnName => "UserControlGUID";
    static CmsFormUserControlK11 ISourceModel<CmsFormUserControlK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsFormUserControlK11(
            reader.Unbox<int>("UserControlID"), reader.Unbox<string>("UserControlDisplayName"), reader.Unbox<string>("UserControlCodeName"), reader.Unbox<string>("UserControlFileName"), reader.Unbox<bool>("UserControlForText"), reader.Unbox<bool>("UserControlForLongText"), reader.Unbox<bool>("UserControlForInteger"), reader.Unbox<bool>("UserControlForDecimal"), reader.Unbox<bool>("UserControlForDateTime"), reader.Unbox<bool>("UserControlForBoolean"), reader.Unbox<bool>("UserControlForFile"), reader.Unbox<bool>("UserControlShowInBizForms"), reader.Unbox<string>("UserControlDefaultDataType"), reader.Unbox<int?>("UserControlDefaultDataTypeSize"), reader.Unbox<bool?>("UserControlShowInDocumentTypes"), reader.Unbox<bool?>("UserControlShowInSystemTables"), reader.Unbox<bool?>("UserControlShowInWebParts"), reader.Unbox<bool?>("UserControlShowInReports"), reader.Unbox<Guid>("UserControlGUID"), reader.Unbox<DateTime>("UserControlLastModified"), reader.Unbox<bool>("UserControlForGuid"), reader.Unbox<bool?>("UserControlShowInCustomTables"), reader.Unbox<bool>("UserControlForVisibility"), reader.Unbox<string?>("UserControlParameters"), reader.Unbox<bool>("UserControlForDocAttachments"), reader.Unbox<int?>("UserControlResourceID"), reader.Unbox<int?>("UserControlType"), reader.Unbox<int?>("UserControlParentID"), reader.Unbox<string?>("UserControlDescription"), reader.Unbox<Guid?>("UserControlThumbnailGUID"), reader.Unbox<int?>("UserControlPriority"), reader.Unbox<bool?>("UserControlIsSystem"), reader.Unbox<bool>("UserControlForBinary"), reader.Unbox<bool>("UserControlForDocRelationships"), reader.Unbox<string?>("UserControlAssemblyName"), reader.Unbox<string?>("UserControlClassName")
        );
    }
    public static CmsFormUserControlK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsFormUserControlK11(
            reader.Unbox<int>("UserControlID"), reader.Unbox<string>("UserControlDisplayName"), reader.Unbox<string>("UserControlCodeName"), reader.Unbox<string>("UserControlFileName"), reader.Unbox<bool>("UserControlForText"), reader.Unbox<bool>("UserControlForLongText"), reader.Unbox<bool>("UserControlForInteger"), reader.Unbox<bool>("UserControlForDecimal"), reader.Unbox<bool>("UserControlForDateTime"), reader.Unbox<bool>("UserControlForBoolean"), reader.Unbox<bool>("UserControlForFile"), reader.Unbox<bool>("UserControlShowInBizForms"), reader.Unbox<string>("UserControlDefaultDataType"), reader.Unbox<int?>("UserControlDefaultDataTypeSize"), reader.Unbox<bool?>("UserControlShowInDocumentTypes"), reader.Unbox<bool?>("UserControlShowInSystemTables"), reader.Unbox<bool?>("UserControlShowInWebParts"), reader.Unbox<bool?>("UserControlShowInReports"), reader.Unbox<Guid>("UserControlGUID"), reader.Unbox<DateTime>("UserControlLastModified"), reader.Unbox<bool>("UserControlForGuid"), reader.Unbox<bool?>("UserControlShowInCustomTables"), reader.Unbox<bool>("UserControlForVisibility"), reader.Unbox<string?>("UserControlParameters"), reader.Unbox<bool>("UserControlForDocAttachments"), reader.Unbox<int?>("UserControlResourceID"), reader.Unbox<int?>("UserControlType"), reader.Unbox<int?>("UserControlParentID"), reader.Unbox<string?>("UserControlDescription"), reader.Unbox<Guid?>("UserControlThumbnailGUID"), reader.Unbox<int?>("UserControlPriority"), reader.Unbox<bool?>("UserControlIsSystem"), reader.Unbox<bool>("UserControlForBinary"), reader.Unbox<bool>("UserControlForDocRelationships"), reader.Unbox<string?>("UserControlAssemblyName"), reader.Unbox<string?>("UserControlClassName")
        );
    }
};
public partial record CmsFormUserControlK12(int UserControlID, string UserControlDisplayName, string UserControlCodeName, string UserControlFileName, bool UserControlForText, bool UserControlForLongText, bool UserControlForInteger, bool UserControlForDecimal, bool UserControlForDateTime, bool UserControlForBoolean, bool UserControlForFile, bool UserControlShowInBizForms, string UserControlDefaultDataType, int? UserControlDefaultDataTypeSize, bool? UserControlShowInDocumentTypes, bool? UserControlShowInSystemTables, bool? UserControlShowInWebParts, bool? UserControlShowInReports, Guid UserControlGUID, DateTime UserControlLastModified, bool UserControlForGuid, bool? UserControlShowInCustomTables, bool UserControlForVisibility, string? UserControlParameters, bool UserControlForDocAttachments, int? UserControlResourceID, int? UserControlType, int? UserControlParentID, string? UserControlDescription, Guid? UserControlThumbnailGUID, int? UserControlPriority, bool? UserControlIsSystem, bool UserControlForBinary, bool UserControlForDocRelationships, string? UserControlAssemblyName, string? UserControlClassName) : ICmsFormUserControl, ISourceModel<CmsFormUserControlK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserControlID";
    public static string TableName => "CMS_FormUserControl";
    public static string GuidColumnName => "UserControlGUID";
    static CmsFormUserControlK12 ISourceModel<CmsFormUserControlK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsFormUserControlK12(
            reader.Unbox<int>("UserControlID"), reader.Unbox<string>("UserControlDisplayName"), reader.Unbox<string>("UserControlCodeName"), reader.Unbox<string>("UserControlFileName"), reader.Unbox<bool>("UserControlForText"), reader.Unbox<bool>("UserControlForLongText"), reader.Unbox<bool>("UserControlForInteger"), reader.Unbox<bool>("UserControlForDecimal"), reader.Unbox<bool>("UserControlForDateTime"), reader.Unbox<bool>("UserControlForBoolean"), reader.Unbox<bool>("UserControlForFile"), reader.Unbox<bool>("UserControlShowInBizForms"), reader.Unbox<string>("UserControlDefaultDataType"), reader.Unbox<int?>("UserControlDefaultDataTypeSize"), reader.Unbox<bool?>("UserControlShowInDocumentTypes"), reader.Unbox<bool?>("UserControlShowInSystemTables"), reader.Unbox<bool?>("UserControlShowInWebParts"), reader.Unbox<bool?>("UserControlShowInReports"), reader.Unbox<Guid>("UserControlGUID"), reader.Unbox<DateTime>("UserControlLastModified"), reader.Unbox<bool>("UserControlForGuid"), reader.Unbox<bool?>("UserControlShowInCustomTables"), reader.Unbox<bool>("UserControlForVisibility"), reader.Unbox<string?>("UserControlParameters"), reader.Unbox<bool>("UserControlForDocAttachments"), reader.Unbox<int?>("UserControlResourceID"), reader.Unbox<int?>("UserControlType"), reader.Unbox<int?>("UserControlParentID"), reader.Unbox<string?>("UserControlDescription"), reader.Unbox<Guid?>("UserControlThumbnailGUID"), reader.Unbox<int?>("UserControlPriority"), reader.Unbox<bool?>("UserControlIsSystem"), reader.Unbox<bool>("UserControlForBinary"), reader.Unbox<bool>("UserControlForDocRelationships"), reader.Unbox<string?>("UserControlAssemblyName"), reader.Unbox<string?>("UserControlClassName")
        );
    }
    public static CmsFormUserControlK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsFormUserControlK12(
            reader.Unbox<int>("UserControlID"), reader.Unbox<string>("UserControlDisplayName"), reader.Unbox<string>("UserControlCodeName"), reader.Unbox<string>("UserControlFileName"), reader.Unbox<bool>("UserControlForText"), reader.Unbox<bool>("UserControlForLongText"), reader.Unbox<bool>("UserControlForInteger"), reader.Unbox<bool>("UserControlForDecimal"), reader.Unbox<bool>("UserControlForDateTime"), reader.Unbox<bool>("UserControlForBoolean"), reader.Unbox<bool>("UserControlForFile"), reader.Unbox<bool>("UserControlShowInBizForms"), reader.Unbox<string>("UserControlDefaultDataType"), reader.Unbox<int?>("UserControlDefaultDataTypeSize"), reader.Unbox<bool?>("UserControlShowInDocumentTypes"), reader.Unbox<bool?>("UserControlShowInSystemTables"), reader.Unbox<bool?>("UserControlShowInWebParts"), reader.Unbox<bool?>("UserControlShowInReports"), reader.Unbox<Guid>("UserControlGUID"), reader.Unbox<DateTime>("UserControlLastModified"), reader.Unbox<bool>("UserControlForGuid"), reader.Unbox<bool?>("UserControlShowInCustomTables"), reader.Unbox<bool>("UserControlForVisibility"), reader.Unbox<string?>("UserControlParameters"), reader.Unbox<bool>("UserControlForDocAttachments"), reader.Unbox<int?>("UserControlResourceID"), reader.Unbox<int?>("UserControlType"), reader.Unbox<int?>("UserControlParentID"), reader.Unbox<string?>("UserControlDescription"), reader.Unbox<Guid?>("UserControlThumbnailGUID"), reader.Unbox<int?>("UserControlPriority"), reader.Unbox<bool?>("UserControlIsSystem"), reader.Unbox<bool>("UserControlForBinary"), reader.Unbox<bool>("UserControlForDocRelationships"), reader.Unbox<string?>("UserControlAssemblyName"), reader.Unbox<string?>("UserControlClassName")
        );
    }
};
public partial record CmsFormUserControlK13(int UserControlID, string UserControlDisplayName, string UserControlCodeName, string UserControlFileName, bool UserControlForText, bool UserControlForLongText, bool UserControlForInteger, bool UserControlForDecimal, bool UserControlForDateTime, bool UserControlForBoolean, bool UserControlForFile, bool? UserControlShowInDocumentTypes, bool? UserControlShowInSystemTables, bool? UserControlShowInWebParts, bool? UserControlShowInReports, Guid UserControlGUID, DateTime UserControlLastModified, bool UserControlForGuid, bool? UserControlShowInCustomTables, string? UserControlParameters, bool UserControlForDocAttachments, int? UserControlResourceID, int? UserControlParentID, string? UserControlDescription, int? UserControlPriority, bool? UserControlIsSystem, bool UserControlForBinary, bool UserControlForDocRelationships, string? UserControlAssemblyName, string? UserControlClassName) : ICmsFormUserControl, ISourceModel<CmsFormUserControlK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserControlID";
    public static string TableName => "CMS_FormUserControl";
    public static string GuidColumnName => "UserControlGUID";
    static CmsFormUserControlK13 ISourceModel<CmsFormUserControlK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsFormUserControlK13(
            reader.Unbox<int>("UserControlID"), reader.Unbox<string>("UserControlDisplayName"), reader.Unbox<string>("UserControlCodeName"), reader.Unbox<string>("UserControlFileName"), reader.Unbox<bool>("UserControlForText"), reader.Unbox<bool>("UserControlForLongText"), reader.Unbox<bool>("UserControlForInteger"), reader.Unbox<bool>("UserControlForDecimal"), reader.Unbox<bool>("UserControlForDateTime"), reader.Unbox<bool>("UserControlForBoolean"), reader.Unbox<bool>("UserControlForFile"), reader.Unbox<bool?>("UserControlShowInDocumentTypes"), reader.Unbox<bool?>("UserControlShowInSystemTables"), reader.Unbox<bool?>("UserControlShowInWebParts"), reader.Unbox<bool?>("UserControlShowInReports"), reader.Unbox<Guid>("UserControlGUID"), reader.Unbox<DateTime>("UserControlLastModified"), reader.Unbox<bool>("UserControlForGuid"), reader.Unbox<bool?>("UserControlShowInCustomTables"), reader.Unbox<string?>("UserControlParameters"), reader.Unbox<bool>("UserControlForDocAttachments"), reader.Unbox<int?>("UserControlResourceID"), reader.Unbox<int?>("UserControlParentID"), reader.Unbox<string?>("UserControlDescription"), reader.Unbox<int?>("UserControlPriority"), reader.Unbox<bool?>("UserControlIsSystem"), reader.Unbox<bool>("UserControlForBinary"), reader.Unbox<bool>("UserControlForDocRelationships"), reader.Unbox<string?>("UserControlAssemblyName"), reader.Unbox<string?>("UserControlClassName")
        );
    }
    public static CmsFormUserControlK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsFormUserControlK13(
            reader.Unbox<int>("UserControlID"), reader.Unbox<string>("UserControlDisplayName"), reader.Unbox<string>("UserControlCodeName"), reader.Unbox<string>("UserControlFileName"), reader.Unbox<bool>("UserControlForText"), reader.Unbox<bool>("UserControlForLongText"), reader.Unbox<bool>("UserControlForInteger"), reader.Unbox<bool>("UserControlForDecimal"), reader.Unbox<bool>("UserControlForDateTime"), reader.Unbox<bool>("UserControlForBoolean"), reader.Unbox<bool>("UserControlForFile"), reader.Unbox<bool?>("UserControlShowInDocumentTypes"), reader.Unbox<bool?>("UserControlShowInSystemTables"), reader.Unbox<bool?>("UserControlShowInWebParts"), reader.Unbox<bool?>("UserControlShowInReports"), reader.Unbox<Guid>("UserControlGUID"), reader.Unbox<DateTime>("UserControlLastModified"), reader.Unbox<bool>("UserControlForGuid"), reader.Unbox<bool?>("UserControlShowInCustomTables"), reader.Unbox<string?>("UserControlParameters"), reader.Unbox<bool>("UserControlForDocAttachments"), reader.Unbox<int?>("UserControlResourceID"), reader.Unbox<int?>("UserControlParentID"), reader.Unbox<string?>("UserControlDescription"), reader.Unbox<int?>("UserControlPriority"), reader.Unbox<bool?>("UserControlIsSystem"), reader.Unbox<bool>("UserControlForBinary"), reader.Unbox<bool>("UserControlForDocRelationships"), reader.Unbox<string?>("UserControlAssemblyName"), reader.Unbox<string?>("UserControlClassName")
        );
    }
};