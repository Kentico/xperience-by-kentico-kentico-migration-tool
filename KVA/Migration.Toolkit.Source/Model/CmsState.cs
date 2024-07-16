// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public interface ICmsState : ISourceModel<ICmsState>
{
    int StateID { get; }
    string StateDisplayName { get; }
    string StateName { get; }
    string? StateCode { get; }
    int CountryID { get; }
    Guid StateGUID { get; }
    DateTime StateLastModified { get; }

    static string ISourceModel<ICmsState>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsStateK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsStateK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsStateK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsState>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsStateK11.IsAvailable(version),
        { Major: 12 } => CmsStateK12.IsAvailable(version),
        { Major: 13 } => CmsStateK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsState>.TableName => "CMS_State";
    static string ISourceModel<ICmsState>.GuidColumnName => "StateGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsState ISourceModel<ICmsState>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsStateK11.FromReader(reader, version),
        { Major: 12 } => CmsStateK12.FromReader(reader, version),
        { Major: 13 } => CmsStateK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsStateK11(int StateID, string StateDisplayName, string StateName, string? StateCode, int CountryID, Guid StateGUID, DateTime StateLastModified) : ICmsState, ISourceModel<CmsStateK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "StateID";
    public static string TableName => "CMS_State";
    public static string GuidColumnName => "StateGUID";
    static CmsStateK11 ISourceModel<CmsStateK11>.FromReader(IDataReader reader, SemanticVersion version) => new CmsStateK11(
            reader.Unbox<int>("StateID"), reader.Unbox<string>("StateDisplayName"), reader.Unbox<string>("StateName"), reader.Unbox<string?>("StateCode"), reader.Unbox<int>("CountryID"), reader.Unbox<Guid>("StateGUID"), reader.Unbox<DateTime>("StateLastModified")
        );
    public static CmsStateK11 FromReader(IDataReader reader, SemanticVersion version) => new CmsStateK11(
            reader.Unbox<int>("StateID"), reader.Unbox<string>("StateDisplayName"), reader.Unbox<string>("StateName"), reader.Unbox<string?>("StateCode"), reader.Unbox<int>("CountryID"), reader.Unbox<Guid>("StateGUID"), reader.Unbox<DateTime>("StateLastModified")
        );
};
public partial record CmsStateK12(int StateID, string StateDisplayName, string StateName, string? StateCode, int CountryID, Guid StateGUID, DateTime StateLastModified) : ICmsState, ISourceModel<CmsStateK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "StateID";
    public static string TableName => "CMS_State";
    public static string GuidColumnName => "StateGUID";
    static CmsStateK12 ISourceModel<CmsStateK12>.FromReader(IDataReader reader, SemanticVersion version) => new CmsStateK12(
            reader.Unbox<int>("StateID"), reader.Unbox<string>("StateDisplayName"), reader.Unbox<string>("StateName"), reader.Unbox<string?>("StateCode"), reader.Unbox<int>("CountryID"), reader.Unbox<Guid>("StateGUID"), reader.Unbox<DateTime>("StateLastModified")
        );
    public static CmsStateK12 FromReader(IDataReader reader, SemanticVersion version) => new CmsStateK12(
            reader.Unbox<int>("StateID"), reader.Unbox<string>("StateDisplayName"), reader.Unbox<string>("StateName"), reader.Unbox<string?>("StateCode"), reader.Unbox<int>("CountryID"), reader.Unbox<Guid>("StateGUID"), reader.Unbox<DateTime>("StateLastModified")
        );
};
public partial record CmsStateK13(int StateID, string StateDisplayName, string StateName, string? StateCode, int CountryID, Guid StateGUID, DateTime StateLastModified) : ICmsState, ISourceModel<CmsStateK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "StateID";
    public static string TableName => "CMS_State";
    public static string GuidColumnName => "StateGUID";
    static CmsStateK13 ISourceModel<CmsStateK13>.FromReader(IDataReader reader, SemanticVersion version) => new CmsStateK13(
            reader.Unbox<int>("StateID"), reader.Unbox<string>("StateDisplayName"), reader.Unbox<string>("StateName"), reader.Unbox<string?>("StateCode"), reader.Unbox<int>("CountryID"), reader.Unbox<Guid>("StateGUID"), reader.Unbox<DateTime>("StateLastModified")
        );
    public static CmsStateK13 FromReader(IDataReader reader, SemanticVersion version) => new CmsStateK13(
            reader.Unbox<int>("StateID"), reader.Unbox<string>("StateDisplayName"), reader.Unbox<string>("StateName"), reader.Unbox<string?>("StateCode"), reader.Unbox<int>("CountryID"), reader.Unbox<Guid>("StateGUID"), reader.Unbox<DateTime>("StateLastModified")
        );
};
