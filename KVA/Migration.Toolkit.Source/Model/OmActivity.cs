namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface IOmActivity: ISourceModel<IOmActivity>
{
    int ActivityID { get; }
    int ActivityContactID { get; }
    DateTime? ActivityCreated { get; }
    string ActivityType { get; }
    int? ActivityItemID { get; }
    int? ActivityItemDetailID { get; }
    string? ActivityValue { get; }
    string? ActivityURL { get; }
    string? ActivityTitle { get; }
    int ActivitySiteID { get; }
    string? ActivityComment { get; }
    string? ActivityCampaign { get; }
    string? ActivityURLReferrer { get; }
    string? ActivityCulture { get; }
    int? ActivityNodeID { get; }
    string? ActivityUTMSource { get; }
    string? ActivityABVariantName { get; }
    long ActivityURLHash { get; }
    string? ActivityUTMContent { get; }    

    static string ISourceModel<IOmActivity>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => OmActivityK11.GetPrimaryKeyName(version),
            { Major: 12 } => OmActivityK12.GetPrimaryKeyName(version),
            { Major: 13 } => OmActivityK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<IOmActivity>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => OmActivityK11.IsAvailable(version),
            { Major: 12 } => OmActivityK12.IsAvailable(version),
            { Major: 13 } => OmActivityK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<IOmActivity>.TableName => "OM_Activity";
    static string ISourceModel<IOmActivity>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static IOmActivity ISourceModel<IOmActivity>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => OmActivityK11.FromReader(reader, version),
            { Major: 12 } => OmActivityK12.FromReader(reader, version),
            { Major: 13 } => OmActivityK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record OmActivityK11(int ActivityID, int ActivityContactID, DateTime? ActivityCreated, string ActivityType, int? ActivityItemID, int? ActivityItemDetailID, string? ActivityValue, string? ActivityURL, string? ActivityTitle, int ActivitySiteID, string? ActivityComment, string? ActivityCampaign, string? ActivityURLReferrer, string? ActivityCulture, int? ActivityNodeID, string? ActivityUTMSource, string? ActivityABVariantName, string? ActivityMVTCombinationName, long ActivityURLHash, string? ActivityUTMContent): IOmActivity, ISourceModel<OmActivityK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ActivityID";   
    public static string TableName => "OM_Activity";
    public static string GuidColumnName => "";
    static OmActivityK11 ISourceModel<OmActivityK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmActivityK11(
            reader.Unbox<int>("ActivityID"), reader.Unbox<int>("ActivityContactID"), reader.Unbox<DateTime?>("ActivityCreated"), reader.Unbox<string>("ActivityType"), reader.Unbox<int?>("ActivityItemID"), reader.Unbox<int?>("ActivityItemDetailID"), reader.Unbox<string?>("ActivityValue"), reader.Unbox<string?>("ActivityURL"), reader.Unbox<string?>("ActivityTitle"), reader.Unbox<int>("ActivitySiteID"), reader.Unbox<string?>("ActivityComment"), reader.Unbox<string?>("ActivityCampaign"), reader.Unbox<string?>("ActivityURLReferrer"), reader.Unbox<string?>("ActivityCulture"), reader.Unbox<int?>("ActivityNodeID"), reader.Unbox<string?>("ActivityUTMSource"), reader.Unbox<string?>("ActivityABVariantName"), reader.Unbox<string?>("ActivityMVTCombinationName"), reader.Unbox<long>("ActivityURLHash"), reader.Unbox<string?>("ActivityUTMContent")                
        );
    }
    public static OmActivityK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmActivityK11(
            reader.Unbox<int>("ActivityID"), reader.Unbox<int>("ActivityContactID"), reader.Unbox<DateTime?>("ActivityCreated"), reader.Unbox<string>("ActivityType"), reader.Unbox<int?>("ActivityItemID"), reader.Unbox<int?>("ActivityItemDetailID"), reader.Unbox<string?>("ActivityValue"), reader.Unbox<string?>("ActivityURL"), reader.Unbox<string?>("ActivityTitle"), reader.Unbox<int>("ActivitySiteID"), reader.Unbox<string?>("ActivityComment"), reader.Unbox<string?>("ActivityCampaign"), reader.Unbox<string?>("ActivityURLReferrer"), reader.Unbox<string?>("ActivityCulture"), reader.Unbox<int?>("ActivityNodeID"), reader.Unbox<string?>("ActivityUTMSource"), reader.Unbox<string?>("ActivityABVariantName"), reader.Unbox<string?>("ActivityMVTCombinationName"), reader.Unbox<long>("ActivityURLHash"), reader.Unbox<string?>("ActivityUTMContent")                
        );
    }
};
public partial record OmActivityK12(int ActivityID, int ActivityContactID, DateTime? ActivityCreated, string ActivityType, int? ActivityItemID, int? ActivityItemDetailID, string? ActivityValue, string? ActivityURL, string? ActivityTitle, int ActivitySiteID, string? ActivityComment, string? ActivityCampaign, string? ActivityURLReferrer, string? ActivityCulture, int? ActivityNodeID, string? ActivityUTMSource, string? ActivityABVariantName, string? ActivityMVTCombinationName, long ActivityURLHash, string? ActivityUTMContent): IOmActivity, ISourceModel<OmActivityK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ActivityID";   
    public static string TableName => "OM_Activity";
    public static string GuidColumnName => "";
    static OmActivityK12 ISourceModel<OmActivityK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmActivityK12(
            reader.Unbox<int>("ActivityID"), reader.Unbox<int>("ActivityContactID"), reader.Unbox<DateTime?>("ActivityCreated"), reader.Unbox<string>("ActivityType"), reader.Unbox<int?>("ActivityItemID"), reader.Unbox<int?>("ActivityItemDetailID"), reader.Unbox<string?>("ActivityValue"), reader.Unbox<string?>("ActivityURL"), reader.Unbox<string?>("ActivityTitle"), reader.Unbox<int>("ActivitySiteID"), reader.Unbox<string?>("ActivityComment"), reader.Unbox<string?>("ActivityCampaign"), reader.Unbox<string?>("ActivityURLReferrer"), reader.Unbox<string?>("ActivityCulture"), reader.Unbox<int?>("ActivityNodeID"), reader.Unbox<string?>("ActivityUTMSource"), reader.Unbox<string?>("ActivityABVariantName"), reader.Unbox<string?>("ActivityMVTCombinationName"), reader.Unbox<long>("ActivityURLHash"), reader.Unbox<string?>("ActivityUTMContent")                
        );
    }
    public static OmActivityK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmActivityK12(
            reader.Unbox<int>("ActivityID"), reader.Unbox<int>("ActivityContactID"), reader.Unbox<DateTime?>("ActivityCreated"), reader.Unbox<string>("ActivityType"), reader.Unbox<int?>("ActivityItemID"), reader.Unbox<int?>("ActivityItemDetailID"), reader.Unbox<string?>("ActivityValue"), reader.Unbox<string?>("ActivityURL"), reader.Unbox<string?>("ActivityTitle"), reader.Unbox<int>("ActivitySiteID"), reader.Unbox<string?>("ActivityComment"), reader.Unbox<string?>("ActivityCampaign"), reader.Unbox<string?>("ActivityURLReferrer"), reader.Unbox<string?>("ActivityCulture"), reader.Unbox<int?>("ActivityNodeID"), reader.Unbox<string?>("ActivityUTMSource"), reader.Unbox<string?>("ActivityABVariantName"), reader.Unbox<string?>("ActivityMVTCombinationName"), reader.Unbox<long>("ActivityURLHash"), reader.Unbox<string?>("ActivityUTMContent")                
        );
    }
};
public partial record OmActivityK13(int ActivityID, int ActivityContactID, DateTime? ActivityCreated, string ActivityType, int? ActivityItemID, int? ActivityItemDetailID, string? ActivityValue, string? ActivityURL, string? ActivityTitle, int ActivitySiteID, string? ActivityComment, string? ActivityCampaign, string? ActivityURLReferrer, string? ActivityCulture, int? ActivityNodeID, string? ActivityUTMSource, string? ActivityABVariantName, long ActivityURLHash, string? ActivityUTMContent): IOmActivity, ISourceModel<OmActivityK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ActivityID";   
    public static string TableName => "OM_Activity";
    public static string GuidColumnName => "";
    static OmActivityK13 ISourceModel<OmActivityK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmActivityK13(
            reader.Unbox<int>("ActivityID"), reader.Unbox<int>("ActivityContactID"), reader.Unbox<DateTime?>("ActivityCreated"), reader.Unbox<string>("ActivityType"), reader.Unbox<int?>("ActivityItemID"), reader.Unbox<int?>("ActivityItemDetailID"), reader.Unbox<string?>("ActivityValue"), reader.Unbox<string?>("ActivityURL"), reader.Unbox<string?>("ActivityTitle"), reader.Unbox<int>("ActivitySiteID"), reader.Unbox<string?>("ActivityComment"), reader.Unbox<string?>("ActivityCampaign"), reader.Unbox<string?>("ActivityURLReferrer"), reader.Unbox<string?>("ActivityCulture"), reader.Unbox<int?>("ActivityNodeID"), reader.Unbox<string?>("ActivityUTMSource"), reader.Unbox<string?>("ActivityABVariantName"), reader.Unbox<long>("ActivityURLHash"), reader.Unbox<string?>("ActivityUTMContent")                
        );
    }
    public static OmActivityK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmActivityK13(
            reader.Unbox<int>("ActivityID"), reader.Unbox<int>("ActivityContactID"), reader.Unbox<DateTime?>("ActivityCreated"), reader.Unbox<string>("ActivityType"), reader.Unbox<int?>("ActivityItemID"), reader.Unbox<int?>("ActivityItemDetailID"), reader.Unbox<string?>("ActivityValue"), reader.Unbox<string?>("ActivityURL"), reader.Unbox<string?>("ActivityTitle"), reader.Unbox<int>("ActivitySiteID"), reader.Unbox<string?>("ActivityComment"), reader.Unbox<string?>("ActivityCampaign"), reader.Unbox<string?>("ActivityURLReferrer"), reader.Unbox<string?>("ActivityCulture"), reader.Unbox<int?>("ActivityNodeID"), reader.Unbox<string?>("ActivityUTMSource"), reader.Unbox<string?>("ActivityABVariantName"), reader.Unbox<long>("ActivityURLHash"), reader.Unbox<string?>("ActivityUTMContent")                
        );
    }
};

