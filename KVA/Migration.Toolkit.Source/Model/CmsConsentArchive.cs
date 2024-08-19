// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public partial interface ICmsConsentArchive : ISourceModel<ICmsConsentArchive>
{
    int ConsentArchiveID { get; }
    Guid ConsentArchiveGuid { get; }
    DateTime ConsentArchiveLastModified { get; }
    int ConsentArchiveConsentID { get; }
    string ConsentArchiveHash { get; }
    string ConsentArchiveContent { get; }

    static string ISourceModel<ICmsConsentArchive>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentArchiveK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsConsentArchiveK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsConsentArchiveK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsConsentArchive>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentArchiveK11.IsAvailable(version),
        { Major: 12 } => CmsConsentArchiveK12.IsAvailable(version),
        { Major: 13 } => CmsConsentArchiveK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsConsentArchive>.TableName => "CMS_ConsentArchive";
    static string ISourceModel<ICmsConsentArchive>.GuidColumnName => "ConsentArchiveGuid"; //assumtion, class Guid column doesn't change between versions
    static ICmsConsentArchive ISourceModel<ICmsConsentArchive>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentArchiveK11.FromReader(reader, version),
        { Major: 12 } => CmsConsentArchiveK12.FromReader(reader, version),
        { Major: 13 } => CmsConsentArchiveK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsConsentArchiveK11(int ConsentArchiveID, Guid ConsentArchiveGuid, DateTime ConsentArchiveLastModified, int ConsentArchiveConsentID, string ConsentArchiveHash, string ConsentArchiveContent) : ICmsConsentArchive, ISourceModel<CmsConsentArchiveK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentArchiveID";
    public static string TableName => "CMS_ConsentArchive";
    public static string GuidColumnName => "ConsentArchiveGuid";
    static CmsConsentArchiveK11 ISourceModel<CmsConsentArchiveK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentArchiveID"), reader.Unbox<Guid>("ConsentArchiveGuid"), reader.Unbox<DateTime>("ConsentArchiveLastModified"), reader.Unbox<int>("ConsentArchiveConsentID"), reader.Unbox<string>("ConsentArchiveHash"), reader.Unbox<string>("ConsentArchiveContent")
        );
    public static CmsConsentArchiveK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentArchiveID"), reader.Unbox<Guid>("ConsentArchiveGuid"), reader.Unbox<DateTime>("ConsentArchiveLastModified"), reader.Unbox<int>("ConsentArchiveConsentID"), reader.Unbox<string>("ConsentArchiveHash"), reader.Unbox<string>("ConsentArchiveContent")
        );
};
public partial record CmsConsentArchiveK12(int ConsentArchiveID, Guid ConsentArchiveGuid, DateTime ConsentArchiveLastModified, int ConsentArchiveConsentID, string ConsentArchiveHash, string ConsentArchiveContent) : ICmsConsentArchive, ISourceModel<CmsConsentArchiveK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentArchiveID";
    public static string TableName => "CMS_ConsentArchive";
    public static string GuidColumnName => "ConsentArchiveGuid";
    static CmsConsentArchiveK12 ISourceModel<CmsConsentArchiveK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentArchiveID"), reader.Unbox<Guid>("ConsentArchiveGuid"), reader.Unbox<DateTime>("ConsentArchiveLastModified"), reader.Unbox<int>("ConsentArchiveConsentID"), reader.Unbox<string>("ConsentArchiveHash"), reader.Unbox<string>("ConsentArchiveContent")
        );
    public static CmsConsentArchiveK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentArchiveID"), reader.Unbox<Guid>("ConsentArchiveGuid"), reader.Unbox<DateTime>("ConsentArchiveLastModified"), reader.Unbox<int>("ConsentArchiveConsentID"), reader.Unbox<string>("ConsentArchiveHash"), reader.Unbox<string>("ConsentArchiveContent")
        );
};
public partial record CmsConsentArchiveK13(int ConsentArchiveID, Guid ConsentArchiveGuid, DateTime ConsentArchiveLastModified, int ConsentArchiveConsentID, string ConsentArchiveHash, string ConsentArchiveContent) : ICmsConsentArchive, ISourceModel<CmsConsentArchiveK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentArchiveID";
    public static string TableName => "CMS_ConsentArchive";
    public static string GuidColumnName => "ConsentArchiveGuid";
    static CmsConsentArchiveK13 ISourceModel<CmsConsentArchiveK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentArchiveID"), reader.Unbox<Guid>("ConsentArchiveGuid"), reader.Unbox<DateTime>("ConsentArchiveLastModified"), reader.Unbox<int>("ConsentArchiveConsentID"), reader.Unbox<string>("ConsentArchiveHash"), reader.Unbox<string>("ConsentArchiveContent")
        );
    public static CmsConsentArchiveK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentArchiveID"), reader.Unbox<Guid>("ConsentArchiveGuid"), reader.Unbox<DateTime>("ConsentArchiveLastModified"), reader.Unbox<int>("ConsentArchiveConsentID"), reader.Unbox<string>("ConsentArchiveHash"), reader.Unbox<string>("ConsentArchiveContent")
        );
};

