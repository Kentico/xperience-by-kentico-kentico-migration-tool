// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;

public interface IMediaFile : ISourceModel<IMediaFile>
{
    int FileID { get; }
    string FileName { get; }
    string FileTitle { get; }
    string FileDescription { get; }
    string FileExtension { get; }
    string FileMimeType { get; }
    string FilePath { get; }
    long FileSize { get; }
    int? FileImageWidth { get; }
    int? FileImageHeight { get; }
    Guid FileGUID { get; }
    int FileLibraryID { get; }
    int FileSiteID { get; }
    int? FileCreatedByUserID { get; }
    DateTime FileCreatedWhen { get; }
    int? FileModifiedByUserID { get; }
    DateTime FileModifiedWhen { get; }
    string? FileCustomData { get; }

    static string ISourceModel<IMediaFile>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => MediaFileK11.GetPrimaryKeyName(version),
        { Major: 12 } => MediaFileK12.GetPrimaryKeyName(version),
        { Major: 13 } => MediaFileK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static bool ISourceModel<IMediaFile>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => MediaFileK11.IsAvailable(version),
        { Major: 12 } => MediaFileK12.IsAvailable(version),
        { Major: 13 } => MediaFileK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static string ISourceModel<IMediaFile>.TableName => "Media_File";
    static string ISourceModel<IMediaFile>.GuidColumnName => "FileGUID"; //assumtion, class Guid column doesn't change between versions

    static IMediaFile ISourceModel<IMediaFile>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => MediaFileK11.FromReader(reader, version),
        { Major: 12 } => MediaFileK12.FromReader(reader, version),
        { Major: 13 } => MediaFileK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}

public record MediaFileK11(
    int FileID,
    string FileName,
    string FileTitle,
    string FileDescription,
    string FileExtension,
    string FileMimeType,
    string FilePath,
    long FileSize,
    int? FileImageWidth,
    int? FileImageHeight,
    Guid FileGUID,
    int FileLibraryID,
    int FileSiteID,
    int? FileCreatedByUserID,
    DateTime FileCreatedWhen,
    int? FileModifiedByUserID,
    DateTime FileModifiedWhen,
    string? FileCustomData) : IMediaFile, ISourceModel<MediaFileK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FileID";
    public static string TableName => "Media_File";
    public static string GuidColumnName => "FileGUID";

    static MediaFileK11 ISourceModel<MediaFileK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FileID"), reader.Unbox<string>("FileName"), reader.Unbox<string>("FileTitle"), reader.Unbox<string>("FileDescription"), reader.Unbox<string>("FileExtension"), reader.Unbox<string>("FileMimeType"),
        reader.Unbox<string>("FilePath"), reader.Unbox<long>("FileSize"), reader.Unbox<int?>("FileImageWidth"), reader.Unbox<int?>("FileImageHeight"), reader.Unbox<Guid>("FileGUID"), reader.Unbox<int>("FileLibraryID"),
        reader.Unbox<int>("FileSiteID"), reader.Unbox<int?>("FileCreatedByUserID"), reader.Unbox<DateTime>("FileCreatedWhen"), reader.Unbox<int?>("FileModifiedByUserID"), reader.Unbox<DateTime>("FileModifiedWhen"),
        reader.Unbox<string?>("FileCustomData")
    );

    public static MediaFileK11 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FileID"), reader.Unbox<string>("FileName"), reader.Unbox<string>("FileTitle"), reader.Unbox<string>("FileDescription"), reader.Unbox<string>("FileExtension"), reader.Unbox<string>("FileMimeType"),
        reader.Unbox<string>("FilePath"), reader.Unbox<long>("FileSize"), reader.Unbox<int?>("FileImageWidth"), reader.Unbox<int?>("FileImageHeight"), reader.Unbox<Guid>("FileGUID"), reader.Unbox<int>("FileLibraryID"),
        reader.Unbox<int>("FileSiteID"), reader.Unbox<int?>("FileCreatedByUserID"), reader.Unbox<DateTime>("FileCreatedWhen"), reader.Unbox<int?>("FileModifiedByUserID"), reader.Unbox<DateTime>("FileModifiedWhen"),
        reader.Unbox<string?>("FileCustomData")
    );
}

public record MediaFileK12(
    int FileID,
    string FileName,
    string FileTitle,
    string FileDescription,
    string FileExtension,
    string FileMimeType,
    string FilePath,
    long FileSize,
    int? FileImageWidth,
    int? FileImageHeight,
    Guid FileGUID,
    int FileLibraryID,
    int FileSiteID,
    int? FileCreatedByUserID,
    DateTime FileCreatedWhen,
    int? FileModifiedByUserID,
    DateTime FileModifiedWhen,
    string? FileCustomData) : IMediaFile, ISourceModel<MediaFileK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FileID";
    public static string TableName => "Media_File";
    public static string GuidColumnName => "FileGUID";

    static MediaFileK12 ISourceModel<MediaFileK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FileID"), reader.Unbox<string>("FileName"), reader.Unbox<string>("FileTitle"), reader.Unbox<string>("FileDescription"), reader.Unbox<string>("FileExtension"), reader.Unbox<string>("FileMimeType"),
        reader.Unbox<string>("FilePath"), reader.Unbox<long>("FileSize"), reader.Unbox<int?>("FileImageWidth"), reader.Unbox<int?>("FileImageHeight"), reader.Unbox<Guid>("FileGUID"), reader.Unbox<int>("FileLibraryID"),
        reader.Unbox<int>("FileSiteID"), reader.Unbox<int?>("FileCreatedByUserID"), reader.Unbox<DateTime>("FileCreatedWhen"), reader.Unbox<int?>("FileModifiedByUserID"), reader.Unbox<DateTime>("FileModifiedWhen"),
        reader.Unbox<string?>("FileCustomData")
    );

    public static MediaFileK12 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FileID"), reader.Unbox<string>("FileName"), reader.Unbox<string>("FileTitle"), reader.Unbox<string>("FileDescription"), reader.Unbox<string>("FileExtension"), reader.Unbox<string>("FileMimeType"),
        reader.Unbox<string>("FilePath"), reader.Unbox<long>("FileSize"), reader.Unbox<int?>("FileImageWidth"), reader.Unbox<int?>("FileImageHeight"), reader.Unbox<Guid>("FileGUID"), reader.Unbox<int>("FileLibraryID"),
        reader.Unbox<int>("FileSiteID"), reader.Unbox<int?>("FileCreatedByUserID"), reader.Unbox<DateTime>("FileCreatedWhen"), reader.Unbox<int?>("FileModifiedByUserID"), reader.Unbox<DateTime>("FileModifiedWhen"),
        reader.Unbox<string?>("FileCustomData")
    );
}

public record MediaFileK13(
    int FileID,
    string FileName,
    string FileTitle,
    string FileDescription,
    string FileExtension,
    string FileMimeType,
    string FilePath,
    long FileSize,
    int? FileImageWidth,
    int? FileImageHeight,
    Guid FileGUID,
    int FileLibraryID,
    int FileSiteID,
    int? FileCreatedByUserID,
    DateTime FileCreatedWhen,
    int? FileModifiedByUserID,
    DateTime FileModifiedWhen,
    string? FileCustomData) : IMediaFile, ISourceModel<MediaFileK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FileID";
    public static string TableName => "Media_File";
    public static string GuidColumnName => "FileGUID";

    static MediaFileK13 ISourceModel<MediaFileK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FileID"), reader.Unbox<string>("FileName"), reader.Unbox<string>("FileTitle"), reader.Unbox<string>("FileDescription"), reader.Unbox<string>("FileExtension"), reader.Unbox<string>("FileMimeType"),
        reader.Unbox<string>("FilePath"), reader.Unbox<long>("FileSize"), reader.Unbox<int?>("FileImageWidth"), reader.Unbox<int?>("FileImageHeight"), reader.Unbox<Guid>("FileGUID"), reader.Unbox<int>("FileLibraryID"),
        reader.Unbox<int>("FileSiteID"), reader.Unbox<int?>("FileCreatedByUserID"), reader.Unbox<DateTime>("FileCreatedWhen"), reader.Unbox<int?>("FileModifiedByUserID"), reader.Unbox<DateTime>("FileModifiedWhen"),
        reader.Unbox<string?>("FileCustomData")
    );

    public static MediaFileK13 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FileID"), reader.Unbox<string>("FileName"), reader.Unbox<string>("FileTitle"), reader.Unbox<string>("FileDescription"), reader.Unbox<string>("FileExtension"), reader.Unbox<string>("FileMimeType"),
        reader.Unbox<string>("FilePath"), reader.Unbox<long>("FileSize"), reader.Unbox<int?>("FileImageWidth"), reader.Unbox<int?>("FileImageHeight"), reader.Unbox<Guid>("FileGUID"), reader.Unbox<int>("FileLibraryID"),
        reader.Unbox<int>("FileSiteID"), reader.Unbox<int?>("FileCreatedByUserID"), reader.Unbox<DateTime>("FileCreatedWhen"), reader.Unbox<int?>("FileModifiedByUserID"), reader.Unbox<DateTime>("FileModifiedWhen"),
        reader.Unbox<string?>("FileCustomData")
    );
}
