namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsAttachment : ISourceModel<ICmsAttachment>
{
    int AttachmentID { get; }
    string AttachmentName { get; }
    string AttachmentExtension { get; }
    int AttachmentSize { get; }
    string AttachmentMimeType { get; }
    byte[]? AttachmentBinary { get; }
    int? AttachmentImageWidth { get; }
    int? AttachmentImageHeight { get; }
    int? AttachmentDocumentID { get; }
    Guid AttachmentGUID { get; }
    int AttachmentSiteID { get; }
    DateTime AttachmentLastModified { get; }
    bool? AttachmentIsUnsorted { get; }
    int? AttachmentOrder { get; }
    Guid? AttachmentGroupGUID { get; }
    Guid? AttachmentFormGUID { get; }
    string? AttachmentHash { get; }
    string? AttachmentTitle { get; }
    string? AttachmentDescription { get; }
    string? AttachmentCustomData { get; }
    string? AttachmentSearchContent { get; }
    string? AttachmentVariantDefinitionIdentifier { get; }
    int? AttachmentVariantParentID { get; }

    static string ISourceModel<ICmsAttachment>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsAttachmentK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsAttachmentK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsAttachmentK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsAttachment>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsAttachmentK11.IsAvailable(version),
            { Major: 12 } => CmsAttachmentK12.IsAvailable(version),
            { Major: 13 } => CmsAttachmentK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsAttachment>.TableName => "CMS_Attachment";
    static string ISourceModel<ICmsAttachment>.GuidColumnName => "AttachmentGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsAttachment ISourceModel<ICmsAttachment>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsAttachmentK11.FromReader(reader, version),
            { Major: 12 } => CmsAttachmentK12.FromReader(reader, version),
            { Major: 13 } => CmsAttachmentK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsAttachmentK11(int AttachmentID, string AttachmentName, string AttachmentExtension, int AttachmentSize, string AttachmentMimeType, byte[]? AttachmentBinary, int? AttachmentImageWidth, int? AttachmentImageHeight, int? AttachmentDocumentID, Guid AttachmentGUID, int AttachmentSiteID, DateTime AttachmentLastModified, bool? AttachmentIsUnsorted, int? AttachmentOrder, Guid? AttachmentGroupGUID, Guid? AttachmentFormGUID, string? AttachmentHash, string? AttachmentTitle, string? AttachmentDescription, string? AttachmentCustomData, string? AttachmentSearchContent, string? AttachmentVariantDefinitionIdentifier, int? AttachmentVariantParentID) : ICmsAttachment, ISourceModel<CmsAttachmentK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "AttachmentID";
    public static string TableName => "CMS_Attachment";
    public static string GuidColumnName => "AttachmentGUID";
    static CmsAttachmentK11 ISourceModel<CmsAttachmentK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsAttachmentK11(
            reader.Unbox<int>("AttachmentID"), reader.Unbox<string>("AttachmentName"), reader.Unbox<string>("AttachmentExtension"), reader.Unbox<int>("AttachmentSize"), reader.Unbox<string>("AttachmentMimeType"), reader.Unbox<byte[]?>("AttachmentBinary"), reader.Unbox<int?>("AttachmentImageWidth"), reader.Unbox<int?>("AttachmentImageHeight"), reader.Unbox<int?>("AttachmentDocumentID"), reader.Unbox<Guid>("AttachmentGUID"), reader.Unbox<int>("AttachmentSiteID"), reader.Unbox<DateTime>("AttachmentLastModified"), reader.Unbox<bool?>("AttachmentIsUnsorted"), reader.Unbox<int?>("AttachmentOrder"), reader.Unbox<Guid?>("AttachmentGroupGUID"), reader.Unbox<Guid?>("AttachmentFormGUID"), reader.Unbox<string?>("AttachmentHash"), reader.Unbox<string?>("AttachmentTitle"), reader.Unbox<string?>("AttachmentDescription"), reader.Unbox<string?>("AttachmentCustomData"), reader.Unbox<string?>("AttachmentSearchContent"), reader.Unbox<string?>("AttachmentVariantDefinitionIdentifier"), reader.Unbox<int?>("AttachmentVariantParentID")
        );
    }
    public static CmsAttachmentK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsAttachmentK11(
            reader.Unbox<int>("AttachmentID"), reader.Unbox<string>("AttachmentName"), reader.Unbox<string>("AttachmentExtension"), reader.Unbox<int>("AttachmentSize"), reader.Unbox<string>("AttachmentMimeType"), reader.Unbox<byte[]?>("AttachmentBinary"), reader.Unbox<int?>("AttachmentImageWidth"), reader.Unbox<int?>("AttachmentImageHeight"), reader.Unbox<int?>("AttachmentDocumentID"), reader.Unbox<Guid>("AttachmentGUID"), reader.Unbox<int>("AttachmentSiteID"), reader.Unbox<DateTime>("AttachmentLastModified"), reader.Unbox<bool?>("AttachmentIsUnsorted"), reader.Unbox<int?>("AttachmentOrder"), reader.Unbox<Guid?>("AttachmentGroupGUID"), reader.Unbox<Guid?>("AttachmentFormGUID"), reader.Unbox<string?>("AttachmentHash"), reader.Unbox<string?>("AttachmentTitle"), reader.Unbox<string?>("AttachmentDescription"), reader.Unbox<string?>("AttachmentCustomData"), reader.Unbox<string?>("AttachmentSearchContent"), reader.Unbox<string?>("AttachmentVariantDefinitionIdentifier"), reader.Unbox<int?>("AttachmentVariantParentID")
        );
    }
};
public partial record CmsAttachmentK12(int AttachmentID, string AttachmentName, string AttachmentExtension, int AttachmentSize, string AttachmentMimeType, byte[]? AttachmentBinary, int? AttachmentImageWidth, int? AttachmentImageHeight, int? AttachmentDocumentID, Guid AttachmentGUID, int AttachmentSiteID, DateTime AttachmentLastModified, bool? AttachmentIsUnsorted, int? AttachmentOrder, Guid? AttachmentGroupGUID, Guid? AttachmentFormGUID, string? AttachmentHash, string? AttachmentTitle, string? AttachmentDescription, string? AttachmentCustomData, string? AttachmentSearchContent, string? AttachmentVariantDefinitionIdentifier, int? AttachmentVariantParentID) : ICmsAttachment, ISourceModel<CmsAttachmentK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "AttachmentID";
    public static string TableName => "CMS_Attachment";
    public static string GuidColumnName => "AttachmentGUID";
    static CmsAttachmentK12 ISourceModel<CmsAttachmentK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsAttachmentK12(
            reader.Unbox<int>("AttachmentID"), reader.Unbox<string>("AttachmentName"), reader.Unbox<string>("AttachmentExtension"), reader.Unbox<int>("AttachmentSize"), reader.Unbox<string>("AttachmentMimeType"), reader.Unbox<byte[]?>("AttachmentBinary"), reader.Unbox<int?>("AttachmentImageWidth"), reader.Unbox<int?>("AttachmentImageHeight"), reader.Unbox<int?>("AttachmentDocumentID"), reader.Unbox<Guid>("AttachmentGUID"), reader.Unbox<int>("AttachmentSiteID"), reader.Unbox<DateTime>("AttachmentLastModified"), reader.Unbox<bool?>("AttachmentIsUnsorted"), reader.Unbox<int?>("AttachmentOrder"), reader.Unbox<Guid?>("AttachmentGroupGUID"), reader.Unbox<Guid?>("AttachmentFormGUID"), reader.Unbox<string?>("AttachmentHash"), reader.Unbox<string?>("AttachmentTitle"), reader.Unbox<string?>("AttachmentDescription"), reader.Unbox<string?>("AttachmentCustomData"), reader.Unbox<string?>("AttachmentSearchContent"), reader.Unbox<string?>("AttachmentVariantDefinitionIdentifier"), reader.Unbox<int?>("AttachmentVariantParentID")
        );
    }
    public static CmsAttachmentK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsAttachmentK12(
            reader.Unbox<int>("AttachmentID"), reader.Unbox<string>("AttachmentName"), reader.Unbox<string>("AttachmentExtension"), reader.Unbox<int>("AttachmentSize"), reader.Unbox<string>("AttachmentMimeType"), reader.Unbox<byte[]?>("AttachmentBinary"), reader.Unbox<int?>("AttachmentImageWidth"), reader.Unbox<int?>("AttachmentImageHeight"), reader.Unbox<int?>("AttachmentDocumentID"), reader.Unbox<Guid>("AttachmentGUID"), reader.Unbox<int>("AttachmentSiteID"), reader.Unbox<DateTime>("AttachmentLastModified"), reader.Unbox<bool?>("AttachmentIsUnsorted"), reader.Unbox<int?>("AttachmentOrder"), reader.Unbox<Guid?>("AttachmentGroupGUID"), reader.Unbox<Guid?>("AttachmentFormGUID"), reader.Unbox<string?>("AttachmentHash"), reader.Unbox<string?>("AttachmentTitle"), reader.Unbox<string?>("AttachmentDescription"), reader.Unbox<string?>("AttachmentCustomData"), reader.Unbox<string?>("AttachmentSearchContent"), reader.Unbox<string?>("AttachmentVariantDefinitionIdentifier"), reader.Unbox<int?>("AttachmentVariantParentID")
        );
    }
};
public partial record CmsAttachmentK13(int AttachmentID, string AttachmentName, string AttachmentExtension, int AttachmentSize, string AttachmentMimeType, byte[]? AttachmentBinary, int? AttachmentImageWidth, int? AttachmentImageHeight, int? AttachmentDocumentID, Guid AttachmentGUID, int AttachmentSiteID, DateTime AttachmentLastModified, bool? AttachmentIsUnsorted, int? AttachmentOrder, Guid? AttachmentGroupGUID, Guid? AttachmentFormGUID, string? AttachmentHash, string? AttachmentTitle, string? AttachmentDescription, string? AttachmentCustomData, string? AttachmentSearchContent, string? AttachmentVariantDefinitionIdentifier, int? AttachmentVariantParentID) : ICmsAttachment, ISourceModel<CmsAttachmentK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "AttachmentID";
    public static string TableName => "CMS_Attachment";
    public static string GuidColumnName => "AttachmentGUID";
    static CmsAttachmentK13 ISourceModel<CmsAttachmentK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsAttachmentK13(
            reader.Unbox<int>("AttachmentID"), reader.Unbox<string>("AttachmentName"), reader.Unbox<string>("AttachmentExtension"), reader.Unbox<int>("AttachmentSize"), reader.Unbox<string>("AttachmentMimeType"), reader.Unbox<byte[]?>("AttachmentBinary"), reader.Unbox<int?>("AttachmentImageWidth"), reader.Unbox<int?>("AttachmentImageHeight"), reader.Unbox<int?>("AttachmentDocumentID"), reader.Unbox<Guid>("AttachmentGUID"), reader.Unbox<int>("AttachmentSiteID"), reader.Unbox<DateTime>("AttachmentLastModified"), reader.Unbox<bool?>("AttachmentIsUnsorted"), reader.Unbox<int?>("AttachmentOrder"), reader.Unbox<Guid?>("AttachmentGroupGUID"), reader.Unbox<Guid?>("AttachmentFormGUID"), reader.Unbox<string?>("AttachmentHash"), reader.Unbox<string?>("AttachmentTitle"), reader.Unbox<string?>("AttachmentDescription"), reader.Unbox<string?>("AttachmentCustomData"), reader.Unbox<string?>("AttachmentSearchContent"), reader.Unbox<string?>("AttachmentVariantDefinitionIdentifier"), reader.Unbox<int?>("AttachmentVariantParentID")
        );
    }
    public static CmsAttachmentK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsAttachmentK13(
            reader.Unbox<int>("AttachmentID"), reader.Unbox<string>("AttachmentName"), reader.Unbox<string>("AttachmentExtension"), reader.Unbox<int>("AttachmentSize"), reader.Unbox<string>("AttachmentMimeType"), reader.Unbox<byte[]?>("AttachmentBinary"), reader.Unbox<int?>("AttachmentImageWidth"), reader.Unbox<int?>("AttachmentImageHeight"), reader.Unbox<int?>("AttachmentDocumentID"), reader.Unbox<Guid>("AttachmentGUID"), reader.Unbox<int>("AttachmentSiteID"), reader.Unbox<DateTime>("AttachmentLastModified"), reader.Unbox<bool?>("AttachmentIsUnsorted"), reader.Unbox<int?>("AttachmentOrder"), reader.Unbox<Guid?>("AttachmentGroupGUID"), reader.Unbox<Guid?>("AttachmentFormGUID"), reader.Unbox<string?>("AttachmentHash"), reader.Unbox<string?>("AttachmentTitle"), reader.Unbox<string?>("AttachmentDescription"), reader.Unbox<string?>("AttachmentCustomData"), reader.Unbox<string?>("AttachmentSearchContent"), reader.Unbox<string?>("AttachmentVariantDefinitionIdentifier"), reader.Unbox<int?>("AttachmentVariantParentID")
        );
    }
};