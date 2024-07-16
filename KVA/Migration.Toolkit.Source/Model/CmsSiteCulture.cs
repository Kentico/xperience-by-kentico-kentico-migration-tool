// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public interface ICmsSiteCulture : ISourceModel<ICmsSiteCulture>
{
    int SiteID { get; }
    int CultureID { get; }

    static string ISourceModel<ICmsSiteCulture>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteCultureK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsSiteCultureK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsSiteCultureK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsSiteCulture>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteCultureK11.IsAvailable(version),
        { Major: 12 } => CmsSiteCultureK12.IsAvailable(version),
        { Major: 13 } => CmsSiteCultureK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsSiteCulture>.TableName => "CMS_SiteCulture";
    static string ISourceModel<ICmsSiteCulture>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsSiteCulture ISourceModel<ICmsSiteCulture>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsSiteCultureK11.FromReader(reader, version),
        { Major: 12 } => CmsSiteCultureK12.FromReader(reader, version),
        { Major: 13 } => CmsSiteCultureK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsSiteCultureK11(int SiteID, int CultureID) : ICmsSiteCulture, ISourceModel<CmsSiteCultureK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CultureID";
    public static string TableName => "CMS_SiteCulture";
    public static string GuidColumnName => "";
    static CmsSiteCultureK11 ISourceModel<CmsSiteCultureK11>.FromReader(IDataReader reader, SemanticVersion version) => new CmsSiteCultureK11(
            reader.Unbox<int>("SiteID"), reader.Unbox<int>("CultureID")
        );
    public static CmsSiteCultureK11 FromReader(IDataReader reader, SemanticVersion version) => new CmsSiteCultureK11(
            reader.Unbox<int>("SiteID"), reader.Unbox<int>("CultureID")
        );
};
public partial record CmsSiteCultureK12(int SiteID, int CultureID) : ICmsSiteCulture, ISourceModel<CmsSiteCultureK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CultureID";
    public static string TableName => "CMS_SiteCulture";
    public static string GuidColumnName => "";
    static CmsSiteCultureK12 ISourceModel<CmsSiteCultureK12>.FromReader(IDataReader reader, SemanticVersion version) => new CmsSiteCultureK12(
            reader.Unbox<int>("SiteID"), reader.Unbox<int>("CultureID")
        );
    public static CmsSiteCultureK12 FromReader(IDataReader reader, SemanticVersion version) => new CmsSiteCultureK12(
            reader.Unbox<int>("SiteID"), reader.Unbox<int>("CultureID")
        );
};
public partial record CmsSiteCultureK13(int SiteID, int CultureID) : ICmsSiteCulture, ISourceModel<CmsSiteCultureK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CultureID";
    public static string TableName => "CMS_SiteCulture";
    public static string GuidColumnName => "";
    static CmsSiteCultureK13 ISourceModel<CmsSiteCultureK13>.FromReader(IDataReader reader, SemanticVersion version) => new CmsSiteCultureK13(
            reader.Unbox<int>("SiteID"), reader.Unbox<int>("CultureID")
        );
    public static CmsSiteCultureK13 FromReader(IDataReader reader, SemanticVersion version) => new CmsSiteCultureK13(
            reader.Unbox<int>("SiteID"), reader.Unbox<int>("CultureID")
        );
};
