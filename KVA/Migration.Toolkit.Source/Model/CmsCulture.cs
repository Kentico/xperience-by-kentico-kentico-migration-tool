// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public partial interface ICmsCulture : ISourceModel<ICmsCulture>
{
    int CultureID { get; }
    string CultureName { get; }
    string CultureCode { get; }
    string CultureShortName { get; }
    Guid CultureGUID { get; }
    DateTime CultureLastModified { get; }
    string? CultureAlias { get; }
    bool? CultureIsUICulture { get; }

    static string ISourceModel<ICmsCulture>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsCultureK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsCultureK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsCultureK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsCulture>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsCultureK11.IsAvailable(version),
        { Major: 12 } => CmsCultureK12.IsAvailable(version),
        { Major: 13 } => CmsCultureK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsCulture>.TableName => "CMS_Culture";
    static string ISourceModel<ICmsCulture>.GuidColumnName => "CultureGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsCulture ISourceModel<ICmsCulture>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsCultureK11.FromReader(reader, version),
        { Major: 12 } => CmsCultureK12.FromReader(reader, version),
        { Major: 13 } => CmsCultureK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsCultureK11(int CultureID, string CultureName, string CultureCode, string CultureShortName, Guid CultureGUID, DateTime CultureLastModified, string? CultureAlias, bool? CultureIsUICulture) : ICmsCulture, ISourceModel<CmsCultureK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CultureID";
    public static string TableName => "CMS_Culture";
    public static string GuidColumnName => "CultureGUID";
    static CmsCultureK11 ISourceModel<CmsCultureK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("CultureID"), reader.Unbox<string>("CultureName"), reader.Unbox<string>("CultureCode"), reader.Unbox<string>("CultureShortName"), reader.Unbox<Guid>("CultureGUID"), reader.Unbox<DateTime>("CultureLastModified"), reader.Unbox<string?>("CultureAlias"), reader.Unbox<bool?>("CultureIsUICulture")
        );
    public static CmsCultureK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("CultureID"), reader.Unbox<string>("CultureName"), reader.Unbox<string>("CultureCode"), reader.Unbox<string>("CultureShortName"), reader.Unbox<Guid>("CultureGUID"), reader.Unbox<DateTime>("CultureLastModified"), reader.Unbox<string?>("CultureAlias"), reader.Unbox<bool?>("CultureIsUICulture")
        );
};
public partial record CmsCultureK12(int CultureID, string CultureName, string CultureCode, string CultureShortName, Guid CultureGUID, DateTime CultureLastModified, string? CultureAlias, bool? CultureIsUICulture) : ICmsCulture, ISourceModel<CmsCultureK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CultureID";
    public static string TableName => "CMS_Culture";
    public static string GuidColumnName => "CultureGUID";
    static CmsCultureK12 ISourceModel<CmsCultureK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("CultureID"), reader.Unbox<string>("CultureName"), reader.Unbox<string>("CultureCode"), reader.Unbox<string>("CultureShortName"), reader.Unbox<Guid>("CultureGUID"), reader.Unbox<DateTime>("CultureLastModified"), reader.Unbox<string?>("CultureAlias"), reader.Unbox<bool?>("CultureIsUICulture")
        );
    public static CmsCultureK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("CultureID"), reader.Unbox<string>("CultureName"), reader.Unbox<string>("CultureCode"), reader.Unbox<string>("CultureShortName"), reader.Unbox<Guid>("CultureGUID"), reader.Unbox<DateTime>("CultureLastModified"), reader.Unbox<string?>("CultureAlias"), reader.Unbox<bool?>("CultureIsUICulture")
        );
};
public partial record CmsCultureK13(int CultureID, string CultureName, string CultureCode, string CultureShortName, Guid CultureGUID, DateTime CultureLastModified, string? CultureAlias, bool? CultureIsUICulture) : ICmsCulture, ISourceModel<CmsCultureK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CultureID";
    public static string TableName => "CMS_Culture";
    public static string GuidColumnName => "CultureGUID";
    static CmsCultureK13 ISourceModel<CmsCultureK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("CultureID"), reader.Unbox<string>("CultureName"), reader.Unbox<string>("CultureCode"), reader.Unbox<string>("CultureShortName"), reader.Unbox<Guid>("CultureGUID"), reader.Unbox<DateTime>("CultureLastModified"), reader.Unbox<string?>("CultureAlias"), reader.Unbox<bool?>("CultureIsUICulture")
        );
    public static CmsCultureK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("CultureID"), reader.Unbox<string>("CultureName"), reader.Unbox<string>("CultureCode"), reader.Unbox<string>("CultureShortName"), reader.Unbox<Guid>("CultureGUID"), reader.Unbox<DateTime>("CultureLastModified"), reader.Unbox<string?>("CultureAlias"), reader.Unbox<bool?>("CultureIsUICulture")
        );
};

