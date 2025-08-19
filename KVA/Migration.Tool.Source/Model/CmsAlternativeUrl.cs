// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsAlternativeUrl : ISourceModel<ICmsAlternativeUrl>
{
    public int AlternativeUrlID { get; }
    public Guid AlternativeUrlGUID { get; }
    public int AlternativeUrlDocumentID { get; }
    public int AlternativeUrlSiteID { get; }
    public string AlternativeUrlUrl { get; }
    public DateTime AlternativeUrlLastModified { get; }

    static string ISourceModel<ICmsAlternativeUrl>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAlternativeUrlK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsAlternativeUrlK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsAlternativeUrlK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsAlternativeUrl>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAlternativeUrlK11.IsAvailable(version),
        { Major: 12 } => CmsAlternativeUrlK12.IsAvailable(version),
        { Major: 13 } => CmsAlternativeUrlK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsAlternativeUrl>.TableName => "CMS_AlternativeUrl";
    static string ISourceModel<ICmsAlternativeUrl>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static ICmsAlternativeUrl ISourceModel<ICmsAlternativeUrl>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsAlternativeUrlK11.FromReader(reader, version),
        { Major: 12 } => CmsAlternativeUrlK12.FromReader(reader, version),
        { Major: 13 } => CmsAlternativeUrlK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsAlternativeUrlK11() : ICmsAlternativeUrl, ISourceModel<CmsAlternativeUrlK11>
{
    public static bool IsAvailable(SemanticVersion version) => false;
    public static string GetPrimaryKeyName(SemanticVersion version) => "";
    public static string TableName => "CMS_AlternativeUrl";
    public static string GuidColumnName => "";

    public int AlternativeUrlID => throw new NotImplementedException();

    public Guid AlternativeUrlGUID => throw new NotImplementedException();

    public int AlternativeUrlDocumentID => throw new NotImplementedException();

    public int AlternativeUrlSiteID => throw new NotImplementedException();

    public string AlternativeUrlUrl => throw new NotImplementedException();

    public DateTime AlternativeUrlLastModified => throw new NotImplementedException();

    static CmsAlternativeUrlK11 ISourceModel<CmsAlternativeUrlK11>.FromReader(IDataReader reader, SemanticVersion version) => new(

        );
    public static CmsAlternativeUrlK11 FromReader(IDataReader reader, SemanticVersion version) => new(

        );
};
public partial record CmsAlternativeUrlK12(int AlternativeUrlID, Guid AlternativeUrlGUID, int AlternativeUrlDocumentID, int AlternativeUrlSiteID, string AlternativeUrlUrl, DateTime AlternativeUrlLastModified) : ICmsAlternativeUrl, ISourceModel<CmsAlternativeUrlK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "AlternativeUrlID";
    public static string TableName => "CMS_AlternativeUrl";
    public static string GuidColumnName => "AlternativeUrlGUID";
    static CmsAlternativeUrlK12 ISourceModel<CmsAlternativeUrlK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("AlternativeUrlID"), reader.Unbox<Guid>("AlternativeUrlGUID"), reader.Unbox<int>("AlternativeUrlDocumentID"), reader.Unbox<int>("AlternativeUrlSiteID"), reader.Unbox<string>("AlternativeUrlUrl"), reader.Unbox<DateTime>("AlternativeUrlLastModified")
        );
    public static CmsAlternativeUrlK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("AlternativeUrlID"), reader.Unbox<Guid>("AlternativeUrlGUID"), reader.Unbox<int>("AlternativeUrlDocumentID"), reader.Unbox<int>("AlternativeUrlSiteID"), reader.Unbox<string>("AlternativeUrlUrl"), reader.Unbox<DateTime>("AlternativeUrlLastModified")
        );
};
public partial record CmsAlternativeUrlK13(int AlternativeUrlID, Guid AlternativeUrlGUID, int AlternativeUrlDocumentID, int AlternativeUrlSiteID, string AlternativeUrlUrl, DateTime AlternativeUrlLastModified) : ICmsAlternativeUrl, ISourceModel<CmsAlternativeUrlK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "AlternativeUrlID";
    public static string TableName => "CMS_AlternativeUrl";
    public static string GuidColumnName => "AlternativeUrlGUID";
    static CmsAlternativeUrlK13 ISourceModel<CmsAlternativeUrlK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("AlternativeUrlID"), reader.Unbox<Guid>("AlternativeUrlGUID"), reader.Unbox<int>("AlternativeUrlDocumentID"), reader.Unbox<int>("AlternativeUrlSiteID"), reader.Unbox<string>("AlternativeUrlUrl"), reader.Unbox<DateTime>("AlternativeUrlLastModified")
        );
    public static CmsAlternativeUrlK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("AlternativeUrlID"), reader.Unbox<Guid>("AlternativeUrlGUID"), reader.Unbox<int>("AlternativeUrlDocumentID"), reader.Unbox<int>("AlternativeUrlSiteID"), reader.Unbox<string>("AlternativeUrlUrl"), reader.Unbox<DateTime>("AlternativeUrlLastModified")
        );
};

