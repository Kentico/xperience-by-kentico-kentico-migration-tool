// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Tool.Common;

namespace Migration.Tool.Source.Model;
public partial interface ICmsConsentAgreement : ISourceModel<ICmsConsentAgreement>
{
    int ConsentAgreementID { get; }
    Guid ConsentAgreementGuid { get; }
    bool ConsentAgreementRevoked { get; }
    int ConsentAgreementContactID { get; }
    int ConsentAgreementConsentID { get; }
    string? ConsentAgreementConsentHash { get; }
    DateTime ConsentAgreementTime { get; }

    static string ISourceModel<ICmsConsentAgreement>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentAgreementK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsConsentAgreementK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsConsentAgreementK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsConsentAgreement>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentAgreementK11.IsAvailable(version),
        { Major: 12 } => CmsConsentAgreementK12.IsAvailable(version),
        { Major: 13 } => CmsConsentAgreementK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsConsentAgreement>.TableName => "CMS_ConsentAgreement";
    static string ISourceModel<ICmsConsentAgreement>.GuidColumnName => "ConsentAgreementGuid"; //assumtion, class Guid column doesn't change between versions
    static ICmsConsentAgreement ISourceModel<ICmsConsentAgreement>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsConsentAgreementK11.FromReader(reader, version),
        { Major: 12 } => CmsConsentAgreementK12.FromReader(reader, version),
        { Major: 13 } => CmsConsentAgreementK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsConsentAgreementK11(int ConsentAgreementID, Guid ConsentAgreementGuid, bool ConsentAgreementRevoked, int ConsentAgreementContactID, int ConsentAgreementConsentID, string? ConsentAgreementConsentHash, DateTime ConsentAgreementTime) : ICmsConsentAgreement, ISourceModel<CmsConsentAgreementK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentAgreementID";
    public static string TableName => "CMS_ConsentAgreement";
    public static string GuidColumnName => "ConsentAgreementGuid";
    static CmsConsentAgreementK11 ISourceModel<CmsConsentAgreementK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentAgreementID"), reader.Unbox<Guid>("ConsentAgreementGuid"), reader.Unbox<bool>("ConsentAgreementRevoked"), reader.Unbox<int>("ConsentAgreementContactID"), reader.Unbox<int>("ConsentAgreementConsentID"), reader.Unbox<string?>("ConsentAgreementConsentHash"), reader.Unbox<DateTime>("ConsentAgreementTime")
        );
    public static CmsConsentAgreementK11 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentAgreementID"), reader.Unbox<Guid>("ConsentAgreementGuid"), reader.Unbox<bool>("ConsentAgreementRevoked"), reader.Unbox<int>("ConsentAgreementContactID"), reader.Unbox<int>("ConsentAgreementConsentID"), reader.Unbox<string?>("ConsentAgreementConsentHash"), reader.Unbox<DateTime>("ConsentAgreementTime")
        );
};
public partial record CmsConsentAgreementK12(int ConsentAgreementID, Guid ConsentAgreementGuid, bool ConsentAgreementRevoked, int ConsentAgreementContactID, int ConsentAgreementConsentID, string? ConsentAgreementConsentHash, DateTime ConsentAgreementTime) : ICmsConsentAgreement, ISourceModel<CmsConsentAgreementK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentAgreementID";
    public static string TableName => "CMS_ConsentAgreement";
    public static string GuidColumnName => "ConsentAgreementGuid";
    static CmsConsentAgreementK12 ISourceModel<CmsConsentAgreementK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentAgreementID"), reader.Unbox<Guid>("ConsentAgreementGuid"), reader.Unbox<bool>("ConsentAgreementRevoked"), reader.Unbox<int>("ConsentAgreementContactID"), reader.Unbox<int>("ConsentAgreementConsentID"), reader.Unbox<string?>("ConsentAgreementConsentHash"), reader.Unbox<DateTime>("ConsentAgreementTime")
        );
    public static CmsConsentAgreementK12 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentAgreementID"), reader.Unbox<Guid>("ConsentAgreementGuid"), reader.Unbox<bool>("ConsentAgreementRevoked"), reader.Unbox<int>("ConsentAgreementContactID"), reader.Unbox<int>("ConsentAgreementConsentID"), reader.Unbox<string?>("ConsentAgreementConsentHash"), reader.Unbox<DateTime>("ConsentAgreementTime")
        );
};
public partial record CmsConsentAgreementK13(int ConsentAgreementID, Guid ConsentAgreementGuid, bool ConsentAgreementRevoked, int ConsentAgreementContactID, int ConsentAgreementConsentID, string? ConsentAgreementConsentHash, DateTime ConsentAgreementTime) : ICmsConsentAgreement, ISourceModel<CmsConsentAgreementK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ConsentAgreementID";
    public static string TableName => "CMS_ConsentAgreement";
    public static string GuidColumnName => "ConsentAgreementGuid";
    static CmsConsentAgreementK13 ISourceModel<CmsConsentAgreementK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentAgreementID"), reader.Unbox<Guid>("ConsentAgreementGuid"), reader.Unbox<bool>("ConsentAgreementRevoked"), reader.Unbox<int>("ConsentAgreementContactID"), reader.Unbox<int>("ConsentAgreementConsentID"), reader.Unbox<string?>("ConsentAgreementConsentHash"), reader.Unbox<DateTime>("ConsentAgreementTime")
        );
    public static CmsConsentAgreementK13 FromReader(IDataReader reader, SemanticVersion version) => new(
            reader.Unbox<int>("ConsentAgreementID"), reader.Unbox<Guid>("ConsentAgreementGuid"), reader.Unbox<bool>("ConsentAgreementRevoked"), reader.Unbox<int>("ConsentAgreementContactID"), reader.Unbox<int>("ConsentAgreementConsentID"), reader.Unbox<string?>("ConsentAgreementConsentHash"), reader.Unbox<DateTime>("ConsentAgreementTime")
        );
};

