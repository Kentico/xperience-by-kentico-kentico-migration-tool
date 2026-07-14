namespace Migration.Tool.Extensions.MetadataSchema;

/// <summary>
/// Immutable record representing a single row from the
/// <c>Migration_EffectiveMetadata</c> auxiliary table in the source database.
/// </summary>
public sealed record EffectiveMetadataRecord(
    int DocumentID,
    int NodeID,
    int NodeSiteID,
    Guid DocumentGUID,
    string ClassName,
    string? EffectiveTitle,
    bool TitleInherited,
    string? EffectiveKeywords,
    bool KeywordsInherited,
    string? EffectiveDescription,
    bool DescriptionInherited
);
