namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsCountry : ISourceModel<ICmsCountry>
{
    int CountryID { get; }
    string CountryDisplayName { get; }
    string CountryName { get; }
    Guid CountryGUID { get; }
    DateTime CountryLastModified { get; }
    string? CountryTwoLetterCode { get; }
    string? CountryThreeLetterCode { get; }

    static string ISourceModel<ICmsCountry>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsCountryK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsCountryK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsCountryK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsCountry>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsCountryK11.IsAvailable(version),
            { Major: 12 } => CmsCountryK12.IsAvailable(version),
            { Major: 13 } => CmsCountryK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsCountry>.TableName => "CMS_Country";
    static string ISourceModel<ICmsCountry>.GuidColumnName => "CountryGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsCountry ISourceModel<ICmsCountry>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsCountryK11.FromReader(reader, version),
            { Major: 12 } => CmsCountryK12.FromReader(reader, version),
            { Major: 13 } => CmsCountryK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsCountryK11(int CountryID, string CountryDisplayName, string CountryName, Guid CountryGUID, DateTime CountryLastModified, string? CountryTwoLetterCode, string? CountryThreeLetterCode) : ICmsCountry, ISourceModel<CmsCountryK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CountryID";
    public static string TableName => "CMS_Country";
    public static string GuidColumnName => "CountryGUID";
    static CmsCountryK11 ISourceModel<CmsCountryK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCountryK11(
            reader.Unbox<int>("CountryID"), reader.Unbox<string>("CountryDisplayName"), reader.Unbox<string>("CountryName"), reader.Unbox<Guid>("CountryGUID"), reader.Unbox<DateTime>("CountryLastModified"), reader.Unbox<string?>("CountryTwoLetterCode"), reader.Unbox<string?>("CountryThreeLetterCode")
        );
    }
    public static CmsCountryK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCountryK11(
            reader.Unbox<int>("CountryID"), reader.Unbox<string>("CountryDisplayName"), reader.Unbox<string>("CountryName"), reader.Unbox<Guid>("CountryGUID"), reader.Unbox<DateTime>("CountryLastModified"), reader.Unbox<string?>("CountryTwoLetterCode"), reader.Unbox<string?>("CountryThreeLetterCode")
        );
    }
};
public partial record CmsCountryK12(int CountryID, string CountryDisplayName, string CountryName, Guid CountryGUID, DateTime CountryLastModified, string? CountryTwoLetterCode, string? CountryThreeLetterCode) : ICmsCountry, ISourceModel<CmsCountryK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CountryID";
    public static string TableName => "CMS_Country";
    public static string GuidColumnName => "CountryGUID";
    static CmsCountryK12 ISourceModel<CmsCountryK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCountryK12(
            reader.Unbox<int>("CountryID"), reader.Unbox<string>("CountryDisplayName"), reader.Unbox<string>("CountryName"), reader.Unbox<Guid>("CountryGUID"), reader.Unbox<DateTime>("CountryLastModified"), reader.Unbox<string?>("CountryTwoLetterCode"), reader.Unbox<string?>("CountryThreeLetterCode")
        );
    }
    public static CmsCountryK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCountryK12(
            reader.Unbox<int>("CountryID"), reader.Unbox<string>("CountryDisplayName"), reader.Unbox<string>("CountryName"), reader.Unbox<Guid>("CountryGUID"), reader.Unbox<DateTime>("CountryLastModified"), reader.Unbox<string?>("CountryTwoLetterCode"), reader.Unbox<string?>("CountryThreeLetterCode")
        );
    }
};
public partial record CmsCountryK13(int CountryID, string CountryDisplayName, string CountryName, Guid CountryGUID, DateTime CountryLastModified, string? CountryTwoLetterCode, string? CountryThreeLetterCode) : ICmsCountry, ISourceModel<CmsCountryK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "CountryID";
    public static string TableName => "CMS_Country";
    public static string GuidColumnName => "CountryGUID";
    static CmsCountryK13 ISourceModel<CmsCountryK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCountryK13(
            reader.Unbox<int>("CountryID"), reader.Unbox<string>("CountryDisplayName"), reader.Unbox<string>("CountryName"), reader.Unbox<Guid>("CountryGUID"), reader.Unbox<DateTime>("CountryLastModified"), reader.Unbox<string?>("CountryTwoLetterCode"), reader.Unbox<string?>("CountryThreeLetterCode")
        );
    }
    public static CmsCountryK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsCountryK13(
            reader.Unbox<int>("CountryID"), reader.Unbox<string>("CountryDisplayName"), reader.Unbox<string>("CountryName"), reader.Unbox<Guid>("CountryGUID"), reader.Unbox<DateTime>("CountryLastModified"), reader.Unbox<string?>("CountryTwoLetterCode"), reader.Unbox<string?>("CountryThreeLetterCode")
        );
    }
};