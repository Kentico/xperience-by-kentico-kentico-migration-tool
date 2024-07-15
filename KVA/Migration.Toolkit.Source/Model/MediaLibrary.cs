namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface IMediaLibrary : ISourceModel<IMediaLibrary>
{
    int LibraryID { get; }
    string LibraryName { get; }
    string LibraryDisplayName { get; }
    string? LibraryDescription { get; }
    string LibraryFolder { get; }
    int? LibraryAccess { get; }
    int LibrarySiteID { get; }
    Guid? LibraryGUID { get; }
    DateTime? LibraryLastModified { get; }
    string? LibraryTeaserPath { get; }
    Guid? LibraryTeaserGUID { get; }

    static string ISourceModel<IMediaLibrary>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => MediaLibraryK11.GetPrimaryKeyName(version),
            { Major: 12 } => MediaLibraryK12.GetPrimaryKeyName(version),
            { Major: 13 } => MediaLibraryK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<IMediaLibrary>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => MediaLibraryK11.IsAvailable(version),
            { Major: 12 } => MediaLibraryK12.IsAvailable(version),
            { Major: 13 } => MediaLibraryK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<IMediaLibrary>.TableName => "Media_Library";
    static string ISourceModel<IMediaLibrary>.GuidColumnName => "LibraryGUID"; //assumtion, class Guid column doesn't change between versions
    static IMediaLibrary ISourceModel<IMediaLibrary>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => MediaLibraryK11.FromReader(reader, version),
            { Major: 12 } => MediaLibraryK12.FromReader(reader, version),
            { Major: 13 } => MediaLibraryK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record MediaLibraryK11(int LibraryID, string LibraryName, string LibraryDisplayName, string? LibraryDescription, string LibraryFolder, int? LibraryAccess, int? LibraryGroupID, int LibrarySiteID, Guid? LibraryGUID, DateTime? LibraryLastModified, string? LibraryTeaserPath, Guid? LibraryTeaserGUID) : IMediaLibrary, ISourceModel<MediaLibraryK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "LibraryID";
    public static string TableName => "Media_Library";
    public static string GuidColumnName => "LibraryTeaserGUID";
    static MediaLibraryK11 ISourceModel<MediaLibraryK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new MediaLibraryK11(
            reader.Unbox<int>("LibraryID"), reader.Unbox<string>("LibraryName"), reader.Unbox<string>("LibraryDisplayName"), reader.Unbox<string?>("LibraryDescription"), reader.Unbox<string>("LibraryFolder"), reader.Unbox<int?>("LibraryAccess"), reader.Unbox<int?>("LibraryGroupID"), reader.Unbox<int>("LibrarySiteID"), reader.Unbox<Guid?>("LibraryGUID"), reader.Unbox<DateTime?>("LibraryLastModified"), reader.Unbox<string?>("LibraryTeaserPath"), reader.Unbox<Guid?>("LibraryTeaserGUID")
        );
    }
    public static MediaLibraryK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new MediaLibraryK11(
            reader.Unbox<int>("LibraryID"), reader.Unbox<string>("LibraryName"), reader.Unbox<string>("LibraryDisplayName"), reader.Unbox<string?>("LibraryDescription"), reader.Unbox<string>("LibraryFolder"), reader.Unbox<int?>("LibraryAccess"), reader.Unbox<int?>("LibraryGroupID"), reader.Unbox<int>("LibrarySiteID"), reader.Unbox<Guid?>("LibraryGUID"), reader.Unbox<DateTime?>("LibraryLastModified"), reader.Unbox<string?>("LibraryTeaserPath"), reader.Unbox<Guid?>("LibraryTeaserGUID")
        );
    }
};
public partial record MediaLibraryK12(int LibraryID, string LibraryName, string LibraryDisplayName, string? LibraryDescription, string LibraryFolder, int? LibraryAccess, int? LibraryGroupID, int LibrarySiteID, Guid? LibraryGUID, DateTime? LibraryLastModified, string? LibraryTeaserPath, Guid? LibraryTeaserGUID) : IMediaLibrary, ISourceModel<MediaLibraryK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "LibraryID";
    public static string TableName => "Media_Library";
    public static string GuidColumnName => "LibraryTeaserGUID";
    static MediaLibraryK12 ISourceModel<MediaLibraryK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new MediaLibraryK12(
            reader.Unbox<int>("LibraryID"), reader.Unbox<string>("LibraryName"), reader.Unbox<string>("LibraryDisplayName"), reader.Unbox<string?>("LibraryDescription"), reader.Unbox<string>("LibraryFolder"), reader.Unbox<int?>("LibraryAccess"), reader.Unbox<int?>("LibraryGroupID"), reader.Unbox<int>("LibrarySiteID"), reader.Unbox<Guid?>("LibraryGUID"), reader.Unbox<DateTime?>("LibraryLastModified"), reader.Unbox<string?>("LibraryTeaserPath"), reader.Unbox<Guid?>("LibraryTeaserGUID")
        );
    }
    public static MediaLibraryK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new MediaLibraryK12(
            reader.Unbox<int>("LibraryID"), reader.Unbox<string>("LibraryName"), reader.Unbox<string>("LibraryDisplayName"), reader.Unbox<string?>("LibraryDescription"), reader.Unbox<string>("LibraryFolder"), reader.Unbox<int?>("LibraryAccess"), reader.Unbox<int?>("LibraryGroupID"), reader.Unbox<int>("LibrarySiteID"), reader.Unbox<Guid?>("LibraryGUID"), reader.Unbox<DateTime?>("LibraryLastModified"), reader.Unbox<string?>("LibraryTeaserPath"), reader.Unbox<Guid?>("LibraryTeaserGUID")
        );
    }
};
public partial record MediaLibraryK13(int LibraryID, string LibraryName, string LibraryDisplayName, string? LibraryDescription, string LibraryFolder, int? LibraryAccess, int LibrarySiteID, Guid? LibraryGUID, DateTime? LibraryLastModified, string? LibraryTeaserPath, Guid? LibraryTeaserGUID, bool? LibraryUseDirectPathForContent) : IMediaLibrary, ISourceModel<MediaLibraryK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "LibraryID";
    public static string TableName => "Media_Library";
    public static string GuidColumnName => "LibraryTeaserGUID";
    static MediaLibraryK13 ISourceModel<MediaLibraryK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new MediaLibraryK13(
            reader.Unbox<int>("LibraryID"), reader.Unbox<string>("LibraryName"), reader.Unbox<string>("LibraryDisplayName"), reader.Unbox<string?>("LibraryDescription"), reader.Unbox<string>("LibraryFolder"), reader.Unbox<int?>("LibraryAccess"), reader.Unbox<int>("LibrarySiteID"), reader.Unbox<Guid?>("LibraryGUID"), reader.Unbox<DateTime?>("LibraryLastModified"), reader.Unbox<string?>("LibraryTeaserPath"), reader.Unbox<Guid?>("LibraryTeaserGUID"), reader.Unbox<bool?>("LibraryUseDirectPathForContent")
        );
    }
    public static MediaLibraryK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new MediaLibraryK13(
            reader.Unbox<int>("LibraryID"), reader.Unbox<string>("LibraryName"), reader.Unbox<string>("LibraryDisplayName"), reader.Unbox<string?>("LibraryDescription"), reader.Unbox<string>("LibraryFolder"), reader.Unbox<int?>("LibraryAccess"), reader.Unbox<int>("LibrarySiteID"), reader.Unbox<Guid?>("LibraryGUID"), reader.Unbox<DateTime?>("LibraryLastModified"), reader.Unbox<string?>("LibraryTeaserPath"), reader.Unbox<Guid?>("LibraryTeaserGUID"), reader.Unbox<bool?>("LibraryUseDirectPathForContent")
        );
    }
};