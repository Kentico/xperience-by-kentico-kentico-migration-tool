// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public interface ICmsSettingsKey : ISourceModel<ICmsSettingsKey>
{
    int KeyID { get; }
    string KeyName { get; }
    string KeyDisplayName { get; }
    string? KeyDescription { get; }
    string? KeyValue { get; }
    string KeyType { get; }
    int? KeyCategoryID { get; }
    int? SiteID { get; }
    Guid KeyGUID { get; }
    DateTime KeyLastModified { get; }
    int? KeyOrder { get; }
    string? KeyDefaultValue { get; }
    string? KeyValidation { get; }
    string? KeyEditingControlPath { get; }
    bool? KeyIsGlobal { get; }
    bool? KeyIsCustom { get; }
    bool? KeyIsHidden { get; }
    string? KeyFormControlSettings { get; }
    string? KeyExplanationText { get; }

    static string ISourceModel<ICmsSettingsKey>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSettingsKeyK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsSettingsKeyK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsSettingsKeyK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsSettingsKey>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSettingsKeyK11.IsAvailable(version),
        { Major: 12 } => CmsSettingsKeyK12.IsAvailable(version),
        { Major: 13 } => CmsSettingsKeyK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsSettingsKey>.TableName => "CMS_SettingsKey";
    static string ISourceModel<ICmsSettingsKey>.GuidColumnName => "KeyGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsSettingsKey ISourceModel<ICmsSettingsKey>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSettingsKeyK11.FromReader(reader, version),
        { Major: 12 } => CmsSettingsKeyK12.FromReader(reader, version),
        { Major: 13 } => CmsSettingsKeyK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsSettingsKeyK11(int KeyID, string KeyName, string KeyDisplayName, string? KeyDescription, string? KeyValue, string KeyType, int? KeyCategoryID, int? SiteID, Guid KeyGUID, DateTime KeyLastModified, int? KeyOrder, string? KeyDefaultValue, string? KeyValidation, string? KeyEditingControlPath, bool? KeyIsGlobal, bool? KeyIsCustom, bool? KeyIsHidden, string? KeyFormControlSettings, string? KeyExplanationText) : ICmsSettingsKey, ISourceModel<CmsSettingsKeyK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "KeyID";
    public static string TableName => "CMS_SettingsKey";
    public static string GuidColumnName => "KeyGUID";
    static CmsSettingsKeyK11 ISourceModel<CmsSettingsKeyK11>.FromReader(IDataReader reader, SemanticVersion version) => new CmsSettingsKeyK11(
            reader.Unbox<int>("KeyID"), reader.Unbox<string>("KeyName"), reader.Unbox<string>("KeyDisplayName"), reader.Unbox<string?>("KeyDescription"), reader.Unbox<string?>("KeyValue"), reader.Unbox<string>("KeyType"), reader.Unbox<int?>("KeyCategoryID"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("KeyGUID"), reader.Unbox<DateTime>("KeyLastModified"), reader.Unbox<int?>("KeyOrder"), reader.Unbox<string?>("KeyDefaultValue"), reader.Unbox<string?>("KeyValidation"), reader.Unbox<string?>("KeyEditingControlPath"), reader.Unbox<bool?>("KeyIsGlobal"), reader.Unbox<bool?>("KeyIsCustom"), reader.Unbox<bool?>("KeyIsHidden"), reader.Unbox<string?>("KeyFormControlSettings"), reader.Unbox<string?>("KeyExplanationText")
        );
    public static CmsSettingsKeyK11 FromReader(IDataReader reader, SemanticVersion version) => new CmsSettingsKeyK11(
            reader.Unbox<int>("KeyID"), reader.Unbox<string>("KeyName"), reader.Unbox<string>("KeyDisplayName"), reader.Unbox<string?>("KeyDescription"), reader.Unbox<string?>("KeyValue"), reader.Unbox<string>("KeyType"), reader.Unbox<int?>("KeyCategoryID"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("KeyGUID"), reader.Unbox<DateTime>("KeyLastModified"), reader.Unbox<int?>("KeyOrder"), reader.Unbox<string?>("KeyDefaultValue"), reader.Unbox<string?>("KeyValidation"), reader.Unbox<string?>("KeyEditingControlPath"), reader.Unbox<bool?>("KeyIsGlobal"), reader.Unbox<bool?>("KeyIsCustom"), reader.Unbox<bool?>("KeyIsHidden"), reader.Unbox<string?>("KeyFormControlSettings"), reader.Unbox<string?>("KeyExplanationText")
        );
};
public partial record CmsSettingsKeyK12(int KeyID, string KeyName, string KeyDisplayName, string? KeyDescription, string? KeyValue, string KeyType, int? KeyCategoryID, int? SiteID, Guid KeyGUID, DateTime KeyLastModified, int? KeyOrder, string? KeyDefaultValue, string? KeyValidation, string? KeyEditingControlPath, bool? KeyIsGlobal, bool? KeyIsCustom, bool? KeyIsHidden, string? KeyFormControlSettings, string? KeyExplanationText) : ICmsSettingsKey, ISourceModel<CmsSettingsKeyK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "KeyID";
    public static string TableName => "CMS_SettingsKey";
    public static string GuidColumnName => "KeyGUID";
    static CmsSettingsKeyK12 ISourceModel<CmsSettingsKeyK12>.FromReader(IDataReader reader, SemanticVersion version) => new CmsSettingsKeyK12(
            reader.Unbox<int>("KeyID"), reader.Unbox<string>("KeyName"), reader.Unbox<string>("KeyDisplayName"), reader.Unbox<string?>("KeyDescription"), reader.Unbox<string?>("KeyValue"), reader.Unbox<string>("KeyType"), reader.Unbox<int?>("KeyCategoryID"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("KeyGUID"), reader.Unbox<DateTime>("KeyLastModified"), reader.Unbox<int?>("KeyOrder"), reader.Unbox<string?>("KeyDefaultValue"), reader.Unbox<string?>("KeyValidation"), reader.Unbox<string?>("KeyEditingControlPath"), reader.Unbox<bool?>("KeyIsGlobal"), reader.Unbox<bool?>("KeyIsCustom"), reader.Unbox<bool?>("KeyIsHidden"), reader.Unbox<string?>("KeyFormControlSettings"), reader.Unbox<string?>("KeyExplanationText")
        );
    public static CmsSettingsKeyK12 FromReader(IDataReader reader, SemanticVersion version) => new CmsSettingsKeyK12(
            reader.Unbox<int>("KeyID"), reader.Unbox<string>("KeyName"), reader.Unbox<string>("KeyDisplayName"), reader.Unbox<string?>("KeyDescription"), reader.Unbox<string?>("KeyValue"), reader.Unbox<string>("KeyType"), reader.Unbox<int?>("KeyCategoryID"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("KeyGUID"), reader.Unbox<DateTime>("KeyLastModified"), reader.Unbox<int?>("KeyOrder"), reader.Unbox<string?>("KeyDefaultValue"), reader.Unbox<string?>("KeyValidation"), reader.Unbox<string?>("KeyEditingControlPath"), reader.Unbox<bool?>("KeyIsGlobal"), reader.Unbox<bool?>("KeyIsCustom"), reader.Unbox<bool?>("KeyIsHidden"), reader.Unbox<string?>("KeyFormControlSettings"), reader.Unbox<string?>("KeyExplanationText")
        );
};
public partial record CmsSettingsKeyK13(int KeyID, string KeyName, string KeyDisplayName, string? KeyDescription, string? KeyValue, string KeyType, int? KeyCategoryID, int? SiteID, Guid KeyGUID, DateTime KeyLastModified, int? KeyOrder, string? KeyDefaultValue, string? KeyValidation, string? KeyEditingControlPath, bool? KeyIsGlobal, bool? KeyIsCustom, bool? KeyIsHidden, string? KeyFormControlSettings, string? KeyExplanationText) : ICmsSettingsKey, ISourceModel<CmsSettingsKeyK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "KeyID";
    public static string TableName => "CMS_SettingsKey";
    public static string GuidColumnName => "KeyGUID";
    static CmsSettingsKeyK13 ISourceModel<CmsSettingsKeyK13>.FromReader(IDataReader reader, SemanticVersion version) => new CmsSettingsKeyK13(
            reader.Unbox<int>("KeyID"), reader.Unbox<string>("KeyName"), reader.Unbox<string>("KeyDisplayName"), reader.Unbox<string?>("KeyDescription"), reader.Unbox<string?>("KeyValue"), reader.Unbox<string>("KeyType"), reader.Unbox<int?>("KeyCategoryID"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("KeyGUID"), reader.Unbox<DateTime>("KeyLastModified"), reader.Unbox<int?>("KeyOrder"), reader.Unbox<string?>("KeyDefaultValue"), reader.Unbox<string?>("KeyValidation"), reader.Unbox<string?>("KeyEditingControlPath"), reader.Unbox<bool?>("KeyIsGlobal"), reader.Unbox<bool?>("KeyIsCustom"), reader.Unbox<bool?>("KeyIsHidden"), reader.Unbox<string?>("KeyFormControlSettings"), reader.Unbox<string?>("KeyExplanationText")
        );
    public static CmsSettingsKeyK13 FromReader(IDataReader reader, SemanticVersion version) => new CmsSettingsKeyK13(
            reader.Unbox<int>("KeyID"), reader.Unbox<string>("KeyName"), reader.Unbox<string>("KeyDisplayName"), reader.Unbox<string?>("KeyDescription"), reader.Unbox<string?>("KeyValue"), reader.Unbox<string>("KeyType"), reader.Unbox<int?>("KeyCategoryID"), reader.Unbox<int?>("SiteID"), reader.Unbox<Guid>("KeyGUID"), reader.Unbox<DateTime>("KeyLastModified"), reader.Unbox<int?>("KeyOrder"), reader.Unbox<string?>("KeyDefaultValue"), reader.Unbox<string?>("KeyValidation"), reader.Unbox<string?>("KeyEditingControlPath"), reader.Unbox<bool?>("KeyIsGlobal"), reader.Unbox<bool?>("KeyIsCustom"), reader.Unbox<bool?>("KeyIsHidden"), reader.Unbox<string?>("KeyFormControlSettings"), reader.Unbox<string?>("KeyExplanationText")
        );
};
