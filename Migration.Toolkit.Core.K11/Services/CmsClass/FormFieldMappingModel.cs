namespace Migration.Toolkit.Core.K11.Services.CmsClass;

using System.Text.RegularExpressions;
using CMS.DataEngine;
using Migration.Toolkit.K11.Auxiliary;
using Migration.Toolkit.KXP.Api.Auxiliary;
using FcText = Toolkit.K11.Auxiliary.Kx12FormControls.UserControlForText;
using FcLongText = Toolkit.K11.Auxiliary.Kx12FormControls.UserControlForLongText;

public record FormComponentReplacement(string OldFormComponent, string NewFormComponent);

public record DataTypeMigrationModel(FieldMigration[] FieldMigrations, FormComponentReplacement[] NotSupportedInKxpLegacyMode, string[] SupportedInKxpLegacyMode);
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
    public static FieldMigration[] BuiltInFieldMigrations => new FieldMigration[]
    {
        new(Kx12FieldDataType.ALL, FieldDataType.ALL, SfcDirective.CatchAnyNonMatching, null, new[] { TfcDirective.Clear }),
        new(Kx12FieldDataType.Unknown, FieldDataType.Unknown, SfcDirective.CatchAnyNonMatching, null, new[] { TfcDirective.Clear }),
        new(Kx12FieldDataType.Text, FieldDataType.Text, FcText.TextBoxControl, FormComponents.AdminTextInputComponent),
        new(Kx12FieldDataType.Text, FieldDataType.Text, FcText.DropDownListControl, FormComponents.AdminDropDownComponent),
        new(Kx12FieldDataType.Text, FieldDataType.Text, FcText.IconSelector, FormComponents.AdminIconSelectorComponent),
        new(Kx12FieldDataType.Text, FieldDataType.Text, FcText.Password, FormComponents.AdminPasswordComponent),
        new(Kx12FieldDataType.Text, FieldDataType.Text, FcText.RadioButtonsControl, FormComponents.AdminRadioGroupComponent),
        new(Kx12FieldDataType.Text, FieldDataType.Text, FcText.TextAreaControl, FormComponents.AdminTextAreaComponent),
        new(Kx12FieldDataType.Text, FieldDataType.Text, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent),
        new(Kx12FieldDataType.LongText, FieldDataType.LongText, FcLongText.HtmlAreaControl, FormComponents.AdminRichTextEditorComponent),
        new(Kx12FieldDataType.LongText, FieldDataType.LongText, FcLongText.TextBoxControl, FormComponents.AdminTextInputComponent),
        new(Kx12FieldDataType.LongText, FieldDataType.LongText, FcLongText.DropDownListControl, FormComponents.AdminDropDownComponent),
        new(Kx12FieldDataType.LongText, FieldDataType.LongText, FcLongText.TextAreaControl, FormComponents.AdminTextAreaComponent),
        new(Kx12FieldDataType.LongText, FieldDataType.LongText, SfcDirective.CatchAnyNonMatching, FormComponents.AdminRichTextEditorComponent),
        new(Kx12FieldDataType.Integer, FieldDataType.Integer, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberInputComponent),
        new(Kx12FieldDataType.LongInteger, FieldDataType.LongInteger, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, new[] { TfcDirective.Clear }),//FormComponents.AdminNumberInputComponent),
        new(Kx12FieldDataType.Double, FieldDataType.Double, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, new[] { TfcDirective.Clear }),// FormComponents.AdminNumberInputComponent),
        new(Kx12FieldDataType.Decimal, FieldDataType.Decimal, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDecimalNumberInputComponent),
        new(Kx12FieldDataType.DateTime, FieldDataType.DateTime, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateTimeInputComponent),
        new(Kx12FieldDataType.Date, FieldDataType.Date, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateInputComponent),
        new(Kx12FieldDataType.TimeSpan, FieldDataType.TimeSpan, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(Kx12FieldDataType.Boolean, FieldDataType.Boolean, SfcDirective.CatchAnyNonMatching, FormComponents.AdminCheckBoxComponent),
        new(Kx12FieldDataType.DocAttachments, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, new[] { TcaDirective.ConvertToAsset }),
        new(Kx12FieldDataType.File, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, new[] { TcaDirective.ConvertToAsset }),
        new(Kx12FieldDataType.Guid, FieldDataType.LongText, "RelatedDocuments", FormComponents.AdminPageSelectorComponent, new [] { TcaDirective.ConvertToPages }),
        new(Kx12FieldDataType.Guid, FieldDataType.Guid, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(Kx12FieldDataType.Binary, FieldDataType.Binary, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(Kx12FieldDataType.Xml, FieldDataType.Xml, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberWithLabelComponent),
        new(Kx12FieldDataType.DocRelationships, FieldDataType.Pages, SfcDirective.CatchAnyNonMatching, FormComponents.AdminPageSelectorComponent, new[] { TcaDirective.ConvertToPages }),
    };

    public static DataTypeMigrationModel BuiltInModel => new(
        BuiltInFieldMigrations,
        new FormComponentReplacement[]
        {
            new(Kx12FormComponents.Kentico_AttachmentSelector, FormComponents.AdminAssetSelectorComponent), new(Kx12FormComponents.Kentico_PageSelector, FormComponents.AdminPageSelectorComponent)
        },
        new[]
        {
            Kx12FormComponents.Kentico_BoolFieldValueTypeSelector, Kx12FormComponents.Kentico_CheckBox, Kx12FormComponents.Kentico_CompareToFieldSelector, Kx12FormComponents.Kentico_ConsentAgreement,
            Kx12FormComponents.Kentico_ConsentSelector, Kx12FormComponents.Kentico_DropDown, Kx12FormComponents.Kentico_EmailInput, Kx12FormComponents.Kentico_FileUploader,
            Kx12FormComponents.Kentico_HiddenGuidInput, Kx12FormComponents.Kentico_IntInput, Kx12FormComponents.Kentico_MultipleChoice, Kx12FormComponents.Kentico_Name,
            Kx12FormComponents.Kentico_NumericFieldComparisonTypeSelector, Kx12FormComponents.Kentico_RadioButtons, Kx12FormComponents.Kentico_Recaptcha,
            Kx12FormComponents.Kentico_StringFieldComparisonTypeSelector, Kx12FormComponents.Kentico_TextArea, Kx12FormComponents.Kentico_TextInput, Kx12FormComponents.Kentico_USPhone,
            Kx12FormComponents.Kentico_Invalid, Kx12FormComponents.Kentico_RichText,
            // Kentico_AttachmentSelector,
            Kx12FormComponents.Kentico_MediaFilesSelector, Kx12FormComponents.Kentico_GeneralSelector, Kx12FormComponents.Kentico_ObjectSelector,
            // Kentico_PageSelector,
            Kx12FormComponents.Kentico_PathSelector, Kx12FormComponents.Kentico_UrlSelector,
        });
}