namespace Migration.Toolkit.KXP.Api.Services.CmsClass;

using System.Text.RegularExpressions;
using CMS.DataEngine;
using CMS.OnlineForms;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.KXP.Api.Auxiliary;
using FcText = Common.Enumerations.Kx13FormControls.UserControlForText;
using FcLongText = Common.Enumerations.Kx13FormControls.UserControlForLongText;

public record FormComponentReplacement(string OldFormComponent, string NewFormComponent);

public record DataTypeMigrationModel(
    FieldMigration[] FieldMigrations,
    FormComponentReplacement[] NotSupportedInKxpLegacyMode,
    [property: Obsolete("Legacy mode is no longer supported")]
    string[] SupportedInKxpLegacyMode
);
public record FieldMigration(string SourceDataType, string TargetDataType, string SourceFormControl, string? TargetFormComponent, string[]? Actions = null, Regex? FieldNameRegex = null);

/// <summary>
/// Tca = target control action
/// </summary>
public static partial class TcaDirective
{
    public const string ClearSettings = "clear settings";
    public const string ClearMacroTable = "clear hashtable";
    public const string ConvertToAsset = "convert to asset";
    public const string ConvertToPages = "convert to pages";
}

/// <summary>
/// Tfc = Target form component
/// </summary>
public static class TfcDirective
{
    public const string CopySourceControl = "#copy-source-control#";
    public const string DoNothing = "#nothing#";
    public const string Clear = "#clear#";
}

/// <summary>
/// Sfc = source form control
/// </summary>
public static class SfcDirective
{
    public const string CatchAnyNonMatching = "#any#";
}

public static class FieldMappingInstance
{
    public static FieldMigration[] BuiltInFieldMigrations =>
    [
        new(KsFieldDataType.ALL, FieldDataType.ALL, SfcDirective.CatchAnyNonMatching, null, [TfcDirective.Clear]),
        new(KsFieldDataType.Unknown, FieldDataType.Unknown, SfcDirective.CatchAnyNonMatching, null, [TfcDirective.Clear]),
        new(KsFieldDataType.Text, FieldDataType.Text, FcText.TextBoxControl, FormComponents.AdminTextInputComponent),
        new(KsFieldDataType.Text, FieldDataType.Text, FcText.DropDownListControl, FormComponents.AdminDropDownComponent),
        new(KsFieldDataType.Text, FieldDataType.Text, FcText.IconSelector, FormComponents.AdminIconSelectorComponent),
        new(KsFieldDataType.Text, FieldDataType.Text, FcText.Password, FormComponents.AdminPasswordComponent),
        new(KsFieldDataType.Text, FieldDataType.Text, FcText.RadioButtonsControl, FormComponents.AdminRadioGroupComponent),
        new(KsFieldDataType.Text, FieldDataType.Text, FcText.TextAreaControl, FormComponents.AdminTextAreaComponent),
        new(KsFieldDataType.Text, FieldDataType.Text, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent),
        new(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.HtmlAreaControl, FormComponents.AdminRichTextEditorComponent),
        new(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.TextBoxControl, FormComponents.AdminTextInputComponent),
        new(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.DropDownListControl, FormComponents.AdminDropDownComponent),
        new(KsFieldDataType.LongText, FieldDataType.LongText, FcLongText.TextAreaControl, FormComponents.AdminTextAreaComponent),
        new(KsFieldDataType.LongText, FieldDataType.LongText, SfcDirective.CatchAnyNonMatching, FormComponents.AdminRichTextEditorComponent),
        new(KsFieldDataType.Integer, FieldDataType.Integer, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberInputComponent),
        new(KsFieldDataType.LongInteger, FieldDataType.LongInteger, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, [TfcDirective.Clear]),//FormComponents.AdminNumberInputComponent),
        new(KsFieldDataType.Double, FieldDataType.Double, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, [TfcDirective.Clear]),// FormComponents.AdminNumberInputComponent),
        new(KsFieldDataType.Decimal, FieldDataType.Decimal, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDecimalNumberInputComponent),
        new(KsFieldDataType.DateTime, FieldDataType.DateTime, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateTimeInputComponent),
        new(KsFieldDataType.Date, FieldDataType.Date, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateInputComponent),
        new(KsFieldDataType.TimeSpan, FieldDataType.TimeSpan, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent),
        new(KsFieldDataType.Boolean, FieldDataType.Boolean, SfcDirective.CatchAnyNonMatching, FormComponents.AdminCheckBoxComponent),
        new(KsFieldDataType.DocAttachments, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, [TcaDirective.ConvertToAsset]),
        new(KsFieldDataType.File, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, [TcaDirective.ConvertToAsset]),
        new(KsFieldDataType.Guid, FieldDataType.LongText, "RelatedDocuments", FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent, [TcaDirective.ConvertToPages]),
        new(KsFieldDataType.Guid, FieldDataType.Guid, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(KsFieldDataType.Binary, FieldDataType.Binary, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(KsFieldDataType.Xml, FieldDataType.Xml, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberWithLabelComponent),
        new(KsFieldDataType.DocRelationships, FieldDataType.WebPages, SfcDirective.CatchAnyNonMatching, FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent, [TcaDirective.ConvertToPages]),

        new(KsFieldDataType.TimeSpan, FieldDataType.TimeSpan, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent, [TcaDirective.ConvertToPages]),
        new(KsFieldDataType.BizFormFile, BizFormUploadFile.DATATYPE_FORMFILE, SfcDirective.CatchAnyNonMatching, FormComponents.MvcFileUploaderComponent, []),
    ];

    public static DataTypeMigrationModel BuiltInModel => new(
        BuiltInFieldMigrations,
        [
            new(Kx13FormComponents.Kentico_AttachmentSelector, FormComponents.AdminAssetSelectorComponent),
            new(Kx13FormComponents.Kentico_PageSelector, FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent),
            // new(Kx13FormComponents.Kentico_PathSelector, FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent)
        ],
        [] // legacy mode is no more
    );
}