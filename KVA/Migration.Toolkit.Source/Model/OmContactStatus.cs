namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface IOmContactStatus : ISourceModel<IOmContactStatus>
{
    int ContactStatusID { get; }
    string ContactStatusName { get; }
    string ContactStatusDisplayName { get; }
    string? ContactStatusDescription { get; }

    static string ISourceModel<IOmContactStatus>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => OmContactStatusK11.GetPrimaryKeyName(version),
            { Major: 12 } => OmContactStatusK12.GetPrimaryKeyName(version),
            { Major: 13 } => OmContactStatusK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<IOmContactStatus>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => OmContactStatusK11.IsAvailable(version),
            { Major: 12 } => OmContactStatusK12.IsAvailable(version),
            { Major: 13 } => OmContactStatusK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<IOmContactStatus>.TableName => "OM_ContactStatus";
    static string ISourceModel<IOmContactStatus>.GuidColumnName => ""; //assumtion, class Guid column doesn't change between versions
    static IOmContactStatus ISourceModel<IOmContactStatus>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => OmContactStatusK11.FromReader(reader, version),
            { Major: 12 } => OmContactStatusK12.FromReader(reader, version),
            { Major: 13 } => OmContactStatusK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record OmContactStatusK11(int ContactStatusID, string ContactStatusName, string ContactStatusDisplayName, string? ContactStatusDescription) : IOmContactStatus, ISourceModel<OmContactStatusK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ContactStatusID";
    public static string TableName => "OM_ContactStatus";
    public static string GuidColumnName => "";
    static OmContactStatusK11 ISourceModel<OmContactStatusK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmContactStatusK11(
            reader.Unbox<int>("ContactStatusID"), reader.Unbox<string>("ContactStatusName"), reader.Unbox<string>("ContactStatusDisplayName"), reader.Unbox<string?>("ContactStatusDescription")
        );
    }
    public static OmContactStatusK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmContactStatusK11(
            reader.Unbox<int>("ContactStatusID"), reader.Unbox<string>("ContactStatusName"), reader.Unbox<string>("ContactStatusDisplayName"), reader.Unbox<string?>("ContactStatusDescription")
        );
    }
};
public partial record OmContactStatusK12(int ContactStatusID, string ContactStatusName, string ContactStatusDisplayName, string? ContactStatusDescription) : IOmContactStatus, ISourceModel<OmContactStatusK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ContactStatusID";
    public static string TableName => "OM_ContactStatus";
    public static string GuidColumnName => "";
    static OmContactStatusK12 ISourceModel<OmContactStatusK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmContactStatusK12(
            reader.Unbox<int>("ContactStatusID"), reader.Unbox<string>("ContactStatusName"), reader.Unbox<string>("ContactStatusDisplayName"), reader.Unbox<string?>("ContactStatusDescription")
        );
    }
    public static OmContactStatusK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmContactStatusK12(
            reader.Unbox<int>("ContactStatusID"), reader.Unbox<string>("ContactStatusName"), reader.Unbox<string>("ContactStatusDisplayName"), reader.Unbox<string?>("ContactStatusDescription")
        );
    }
};
public partial record OmContactStatusK13(int ContactStatusID, string ContactStatusName, string ContactStatusDisplayName, string? ContactStatusDescription) : IOmContactStatus, ISourceModel<OmContactStatusK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ContactStatusID";
    public static string TableName => "OM_ContactStatus";
    public static string GuidColumnName => "";
    static OmContactStatusK13 ISourceModel<OmContactStatusK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmContactStatusK13(
            reader.Unbox<int>("ContactStatusID"), reader.Unbox<string>("ContactStatusName"), reader.Unbox<string>("ContactStatusDisplayName"), reader.Unbox<string?>("ContactStatusDescription")
        );
    }
    public static OmContactStatusK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new OmContactStatusK13(
            reader.Unbox<int>("ContactStatusID"), reader.Unbox<string>("ContactStatusName"), reader.Unbox<string>("ContactStatusDisplayName"), reader.Unbox<string?>("ContactStatusDescription")
        );
    }
};