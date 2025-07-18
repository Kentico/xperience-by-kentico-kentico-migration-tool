namespace Migration.Tool.Source;
public static class FieldConstants
{
    /// <summary>
    /// Value for <see cref="Kentico.Xperience.UMT.Model.FormField.ColumnSize"/> equivalent to NVARCHAR(MAX) in database. 
    /// Works only if <see cref="Kentico.Xperience.UMT.Model.FormField.ColumnType"/> is "longtext"
    /// </summary>
    public const int LongTextMaxColumnSize = 0;

    /// <summary>
    /// Maximum number of characters in a column that contains URL. The value is recommended as highest practical limit.
    /// </summary>
    public const int TextUrlColumnSize = 2083;

    public const int ContentItemLanguageMetadataDisplayNameColumnSize = 100;
}
