using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;

/// <summary>
/// Defines mapping of a legacy metadata field of CMS_Document
/// </summary>
/// <param name="LegacyFieldName"></param>
/// <param name="TargetCaption"></param>
/// <param name="TargetSize">Varchar length of the target field. Use -1 for MAX</param>
/// <param name="AllowEmpty"></param>
public record LegacyDocumentMetadataFieldMapping(string LegacyFieldName, string TargetCaption, int TargetSize, bool AllowEmpty, Func<ICmsDocument, string?> GetValue);
