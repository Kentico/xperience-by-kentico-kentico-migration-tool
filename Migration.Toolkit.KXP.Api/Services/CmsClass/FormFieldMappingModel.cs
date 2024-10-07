using System.Text.RegularExpressions;
using System.Xml.Linq;
using CMS.DataEngine;
using CMS.OnlineForms;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.KXP.Api.Auxiliary;
using FcLongText = Migration.Toolkit.Common.Enumerations.Kx13FormControls.UserControlForLongText;
using FcText = Migration.Toolkit.Common.Enumerations.Kx13FormControls.UserControlForText;

namespace Migration.Toolkit.KXP.Api.Services.CmsClass;

public record FormComponentReplacement(string OldFormComponent, string NewFormComponent);

public record DataTypeMigrationModel(
    FieldMigration[] FieldMigrations,
    FormComponentReplacement[] NotSupportedInKxpLegacyMode,
    [property: Obsolete("Legacy mode is no longer supported")] string[] SupportedInKxpLegacyMode
);

public interface ISourceObjectContext;

public record EmptySourceObjectContext : ISourceObjectContext;
public record FieldMigrationContext(string SourceDataType, string? SourceFormControl, string? FieldName, ISourceObjectContext SourceObjectContext);
public record FieldMigrationResult(bool Success, object? MigratedValue);
public interface IFieldMigration
{
    /// <summary>
    /// custom migrations are sorted by this number, first encountered migration wins. Values higher than 100 000 are set to default migrations, set number bellow 100 000 for custom migrations
    /// </summary>
    int Rank { get; }

    /// <summary>
    /// Methods determines if this migration is usable in context 
    /// </summary>
    /// <param name="context">Expect multiple context types: <see cref="FieldMigrationContext"/> for pages, <see cref="EmptySourceObjectContext"/> for data class</param>
    /// <returns></returns>
    bool ShallMigrate(FieldMigrationContext context);

    /// <summary>
    /// Performs migration of FormField, result is mutated property field
    /// </summary>
    /// <param name="formDefinitionPatcher">Helper class for execution of common functionalities</param>
    /// <param name="field">Field for migration</param>
    /// <param name="columnTypeAttr">field type - in xml "columntype"</param>
    /// <param name="fieldDescriptor">field name or field GUID if field name is not specified</param>
    void MigrateFieldDefinition(FormDefinitionPatcher formDefinitionPatcher, XElement field, XAttribute? columnTypeAttr, string fieldDescriptor);
    /// <summary>
    /// Performs migration of field value
    /// </summary>
    /// <param name="sourceValue">Value from source instance for migration directly from database reader (DBNull may be encountered)</param>
    /// <param name="context">Context <see cref="FieldMigrationContext"/> for pages</param>
    /// <returns>If migration of field succeeds, returns success and migrated value. If not, returns false as success and null reference as value</returns>
    Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context);
}

public record FieldMigration(string SourceDataType, string TargetDataType, string SourceFormControl, string? TargetFormComponent, string[]? Actions = null, Regex? FieldNameRegex = null) : IFieldMigration
{
    public int Rank => 100_000;
    public bool ShallMigrate(FieldMigrationContext context) => throw new NotImplementedException();
    public Task<FieldMigrationResult> MigrateValue(object? sourceValue, FieldMigrationContext context) => throw new NotImplementedException();
    public void MigrateFieldDefinition(FormDefinitionPatcher formDefinitionPatcher, XElement field, XAttribute? columnTypeAttr, string fieldDescriptor) => throw new NotImplementedException();
}

/// <summary>
///     Tca = target control action
/// </summary>
public static class TcaDirective
{
    public const string ClearSettings = "clear settings";
    public const string ClearMacroTable = "clear hashtable";
    public const string ConvertToAsset = "convert to asset";
    public const string ConvertToPages = "convert to pages";
    public const string ConvertToRichText = "convert to richtext";
}

/// <summary>
///     Tfc = Target form component
/// </summary>
public static class TfcDirective
{
    public const string CopySourceControl = "#copy-source-control#";
    public const string DoNothing = "#nothing#";
    public const string Clear = "#clear#";
}

/// <summary>
///     Sfc = source form control
/// </summary>
public static class SfcDirective
{
    public const string CatchAnyNonMatching = "#any#";
}

public static class FieldMappingInstance
{
    public static void PrepareFieldMigrations(ToolkitConfiguration configuration)
    {
        var m = new List<FieldMigration>();
        m.AddRange([
            new FieldMigration(KsFieldDataType.ALL, FieldDataType.ALL, SfcDirective.CatchAnyNonMatching, null, [TfcDirective.Clear]),
            new FieldMigration(KsFieldDataType.Unknown, FieldDataType.Unknown, SfcDirective.CatchAnyNonMatching, null, [TfcDirective.Clear]),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, FcText.TextBoxControl, FormComponents.AdminTextInputComponent),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, FcText.DropDownListControl, FormComponents.AdminDropDownComponent),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, FcText.IconSelector, FormComponents.AdminIconSelectorComponent),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, FcText.Password, FormComponents.AdminPasswordComponent),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, FcText.RadioButtonsControl, FormComponents.AdminRadioGroupComponent),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, FcText.TextAreaControl, FormComponents.AdminTextAreaComponent),
            new FieldMigration(KsFieldDataType.Text, FieldDataType.Text, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent),
            new FieldMigration(KsFieldDataType.LongText, FieldDataType.RichTextHTML, FcLongText.HtmlAreaControl, FormComponents.AdminRichTextEditorComponent, [TcaDirective.ConvertToRichText]),
            new FieldMigration(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.TextBoxControl, FormComponents.AdminTextInputComponent),
            new FieldMigration(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.DropDownListControl, FormComponents.AdminDropDownComponent),
            new FieldMigration(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.TextAreaControl, FormComponents.AdminTextAreaComponent),
            new FieldMigration(KsFieldDataType.LongText, FieldDataType.LongText, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextAreaComponent),
            new FieldMigration(KsFieldDataType.Integer, FieldDataType.Integer, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberInputComponent),
            new FieldMigration(KsFieldDataType.LongInteger, FieldDataType.LongInteger, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, [TfcDirective.Clear]), //FormComponents.AdminNumberInputComponent),
            new FieldMigration(KsFieldDataType.Double, FieldDataType.Double, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, [TfcDirective.Clear]), // FormComponents.AdminNumberInputComponent),
            new FieldMigration(KsFieldDataType.Decimal, FieldDataType.Decimal, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDecimalNumberInputComponent),
            new FieldMigration(KsFieldDataType.DateTime, FieldDataType.DateTime, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateTimeInputComponent),
            new FieldMigration(KsFieldDataType.Date, FieldDataType.Date, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateInputComponent),
            new FieldMigration(KsFieldDataType.TimeSpan, FieldDataType.TimeSpan, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent),
            new FieldMigration(KsFieldDataType.Boolean, FieldDataType.Boolean, SfcDirective.CatchAnyNonMatching, FormComponents.AdminCheckBoxComponent),
            new FieldMigration(KsFieldDataType.Guid, FieldDataType.LongText, "RelatedDocuments", FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent, [TcaDirective.ConvertToPages]),
            new FieldMigration(KsFieldDataType.Guid, FieldDataType.Guid, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
            new FieldMigration(KsFieldDataType.Binary, FieldDataType.Binary, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
            new FieldMigration(KsFieldDataType.Xml, FieldDataType.Xml, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberWithLabelComponent),
            new FieldMigration(KsFieldDataType.DocRelationships, FieldDataType.WebPages, SfcDirective.CatchAnyNonMatching, FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent, [TcaDirective.ConvertToPages]),

            new FieldMigration(KsFieldDataType.TimeSpan, FieldDataType.TimeSpan, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent, []),
            new FieldMigration(KsFieldDataType.BizFormFile, BizFormUploadFile.DATATYPE_FORMFILE, SfcDirective.CatchAnyNonMatching, FormComponents.MvcFileUploaderComponent, [])
        ]);

        if (configuration.MigrateMediaToMediaLibrary)
        {
            m.AddRange([
                new FieldMigration(KsFieldDataType.DocAttachments, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, [TcaDirective.ConvertToAsset]),
                new FieldMigration(KsFieldDataType.File, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, [TcaDirective.ConvertToAsset]),
            ]);
        }
        else
        {
            m.AddRange([
                new FieldMigration(KsFieldDataType.DocAttachments, FieldDataType.ContentItemReference, SfcDirective.CatchAnyNonMatching, FormComponents.AdminContentItemSelectorComponent, [TcaDirective.ConvertToAsset]),
                new FieldMigration(KsFieldDataType.File, FieldDataType.ContentItemReference, SfcDirective.CatchAnyNonMatching, FormComponents.AdminContentItemSelectorComponent, [TcaDirective.ConvertToAsset]),
            ]);
        }

        BuiltInFieldMigrations = [.. m];
    }

    public static FieldMigration[] BuiltInFieldMigrations { get; private set; } = null!;

    public static DataTypeMigrationModel BuiltInModel => new(
        BuiltInFieldMigrations,
        [
            new FormComponentReplacement(Kx13FormComponents.Kentico_AttachmentSelector, FormComponents.AdminAssetSelectorComponent),
            new FormComponentReplacement(Kx13FormComponents.Kentico_PageSelector, FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent),
            new FormComponentReplacement(Kx13FormComponents.Kentico_PathSelector, FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent)
        ],
        [] // legacy mode is no more
    );
}
