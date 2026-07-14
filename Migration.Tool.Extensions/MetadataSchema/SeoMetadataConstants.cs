namespace Migration.Tool.Extensions.MetadataSchema;

/// <summary>
/// Constants for the SEO metadata reusable field schema and the source-DB auxiliary table.
/// </summary>
public static class SeoMetadataConstants
{
    /// <summary>
    /// Name of the auxiliary table created in the source (KX13) database.
    /// </summary>
    public const string AuxTableName = "Migration_EffectiveMetadata";

    /// <summary>
    /// The XbyK reusable field schema name (also used as the identifier for the schema).
    /// </summary>
    public const string SchemaName = "SEO.Metadata";

    /// <summary>
    /// Human-readable display name shown in the XbyK administration.
    /// </summary>
    public const string SchemaDisplayName = "SEO Metadata";

    /// <summary>
    /// Optional description shown in the XbyK administration.
    /// </summary>
    public const string SchemaDescription = "Effective SEO metadata fields.";

    /// <summary>
    /// Effective value of DocumentPageTitle.
    /// </summary>
    public const string FieldMetaTitle = "MetaTitle";

    /// <summary>
    /// Effective value of DocumentPageDescription.
    /// </summary>
    public const string FieldMetaDescription = "MetaDescription";

    /// <summary>
    /// Effective value of DocumentPageKeyWords.
    /// </summary>
    public const string FieldMetaKeywords = "MetaKeywords";

    /// <summary>
    /// True when MetaTitle was inherited from an ancestor page.
    /// </summary>
    public const string FieldMetaTitleInherited = "MetaTitleInherited";

    /// <summary>
    /// True when MetaDescription was inherited from an ancestor page.
    /// </summary>
    public const string FieldMetaDescriptionInherited = "MetaDescriptionInherited";

    /// <summary>
    /// True when MetaKeywords was inherited from an ancestor page.
    /// </summary>
    public const string FieldMetaKeywordsInherited = "MetaKeywordsInherited";
}
