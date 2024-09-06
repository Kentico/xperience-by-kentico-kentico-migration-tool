// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public partial interface ICmsConsent : ISourceModel<ICmsConsent>
{
    int ConsentID { get; }
    string ConsentDisplayName { get; }
    string ConsentName { get; }
    string ConsentContent { get; }
    Guid ConsentGuid { get; }
    DateTime ConsentLastModified { get; }
    string ConsentHash { get; }

    static string ISourceModel<ICmsConsent>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsConsentK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsConsentK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsConsent>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentK11.IsAvailable(version),
        { Major: 12 } => CmsConsentK12.IsAvailable(version),
        { Major: 13 } => CmsConsentK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsConsent>.TableName => "CMS_Consent";
    static string ISourceModel<ICmsConsent>.GuidColumnName => "ConsentGuid"; //assumtion, class Guid column doesn't change between versions
    static ICmsConsent ISourceModel<ICmsConsent>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentK11.FromReader(reader, version),
        { Major: 12 } => CmsConsentK12.FromReader(reader, version),
        { Major: 13 } => CmsConsentK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsConsentK11(int ConsentID, string ConsentDisplayName, string ConsentName, string ConsentContent, Guid ConsentGuid, DateTime ConsentLastModified, string ConsentHash) : ICmsConsent, ISourceModel<CmsConsentK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentID";
    public static string TableName => "CMS_Consent";
    public static string GuidColumnName => "ConsentGuid";
    static CmsConsentK11 ISourceModel<CmsConsentK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentID"), reader.Unbox<string>("ConsentDisplayName"), reader.Unbox<string>("ConsentName"), reader.Unbox<string>("ConsentContent"), reader.Unbox<Guid>("ConsentGuid"), reader.Unbox<DateTime>("ConsentLastModified"), reader.Unbox<string>("ConsentHash")
        );
    public static CmsConsentK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentID"), reader.Unbox<string>("ConsentDisplayName"), reader.Unbox<string>("ConsentName"), reader.Unbox<string>("ConsentContent"), reader.Unbox<Guid>("ConsentGuid"), reader.Unbox<DateTime>("ConsentLastModified"), reader.Unbox<string>("ConsentHash")
        );
};
public partial record CmsConsentK12(int ConsentID, string ConsentDisplayName, string ConsentName, string ConsentContent, Guid ConsentGuid, DateTime ConsentLastModified, string ConsentHash) : ICmsConsent, ISourceModel<CmsConsentK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentID";
    public static string TableName => "CMS_Consent";
    public static string GuidColumnName => "ConsentGuid";
    static CmsConsentK12 ISourceModel<CmsConsentK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentID"), reader.Unbox<string>("ConsentDisplayName"), reader.Unbox<string>("ConsentName"), reader.Unbox<string>("ConsentContent"), reader.Unbox<Guid>("ConsentGuid"), reader.Unbox<DateTime>("ConsentLastModified"), reader.Unbox<string>("ConsentHash")
        );
    public static CmsConsentK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentID"), reader.Unbox<string>("ConsentDisplayName"), reader.Unbox<string>("ConsentName"), reader.Unbox<string>("ConsentContent"), reader.Unbox<Guid>("ConsentGuid"), reader.Unbox<DateTime>("ConsentLastModified"), reader.Unbox<string>("ConsentHash")
        );
};
public partial record CmsConsentK13(int ConsentID, string ConsentDisplayName, string ConsentName, string ConsentContent, Guid ConsentGuid, DateTime ConsentLastModified, string ConsentHash) : ICmsConsent, ISourceModel<CmsConsentK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentID";
    public static string TableName => "CMS_Consent";
    public static string GuidColumnName => "ConsentGuid";
    static CmsConsentK13 ISourceModel<CmsConsentK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentID"), reader.Unbox<string>("ConsentDisplayName"), reader.Unbox<string>("ConsentName"), reader.Unbox<string>("ConsentContent"), reader.Unbox<Guid>("ConsentGuid"), reader.Unbox<DateTime>("ConsentLastModified"), reader.Unbox<string>("ConsentHash")
        );
    public static CmsConsentK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentID"), reader.Unbox<string>("ConsentDisplayName"), reader.Unbox<string>("ConsentName"), reader.Unbox<string>("ConsentContent"), reader.Unbox<Guid>("ConsentGuid"), reader.Unbox<DateTime>("ConsentLastModified"), reader.Unbox<string>("ConsentHash")
        );
};

