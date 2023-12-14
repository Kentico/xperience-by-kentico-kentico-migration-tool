namespace Migration.Toolkit.Core.Services.CmsClass;

using System.Text.RegularExpressions;
using CMS.DataEngine;
using Migration.Toolkit.KX13.Auxiliary;
using Migration.Toolkit.KXP.Api.Auxiliary;
using FcText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForText;
using FcLongText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForLongText;

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
        new(Kx13FieldDataType.ALL, FieldDataType.ALL, SfcDirective.CatchAnyNonMatching, null, new[] { TfcDirective.Clear }),
        new(Kx13FieldDataType.Unknown, FieldDataType.Unknown, SfcDirective.CatchAnyNonMatching, null, new[] { TfcDirective.Clear }),
        new(Kx13FieldDataType.Text, FieldDataType.Text, FcText.TextBoxControl, FormComponents.AdminTextInputComponent),
        new(Kx13FieldDataType.Text, FieldDataType.Text, FcText.DropDownListControl, FormComponents.AdminDropDownComponent),
        new(Kx13FieldDataType.Text, FieldDataType.Text, FcText.IconSelector, FormComponents.AdminIconSelectorComponent),
        new(Kx13FieldDataType.Text, FieldDataType.Text, FcText.Password, FormComponents.AdminPasswordComponent),
        new(Kx13FieldDataType.Text, FieldDataType.Text, FcText.RadioButtonsControl, FormComponents.AdminRadioGroupComponent),
        new(Kx13FieldDataType.Text, FieldDataType.Text, FcText.TextAreaControl, FormComponents.AdminTextAreaComponent),
        new(Kx13FieldDataType.Text, FieldDataType.Text, SfcDirective.CatchAnyNonMatching, FormComponents.AdminTextInputComponent),
        new(Kx13FieldDataType.LongText, FieldDataType.LongText, FcLongText.HtmlAreaControl, FormComponents.AdminRichTextEditorComponent),
        new(Kx13FieldDataType.LongText, FieldDataType.LongText, FcLongText.TextBoxControl, FormComponents.AdminTextInputComponent),
        new(Kx13FieldDataType.LongText, FieldDataType.LongText, FcLongText.DropDownListControl, FormComponents.AdminDropDownComponent),
        new(Kx13FieldDataType.LongText, FieldDataType.LongText, FcLongText.TextAreaControl, FormComponents.AdminTextAreaComponent),
        new(Kx13FieldDataType.LongText, FieldDataType.LongText, SfcDirective.CatchAnyNonMatching, FormComponents.AdminRichTextEditorComponent),
        new(Kx13FieldDataType.Integer, FieldDataType.Integer, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberInputComponent),
        new(Kx13FieldDataType.LongInteger, FieldDataType.LongInteger, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, new[] { TfcDirective.Clear }),//FormComponents.AdminNumberInputComponent),
        new(Kx13FieldDataType.Double, FieldDataType.Double, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear, new[] { TfcDirective.Clear }),// FormComponents.AdminNumberInputComponent),
        new(Kx13FieldDataType.Decimal, FieldDataType.Decimal, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDecimalNumberInputComponent),
        new(Kx13FieldDataType.DateTime, FieldDataType.DateTime, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateTimeInputComponent),
        new(Kx13FieldDataType.Date, FieldDataType.Date, SfcDirective.CatchAnyNonMatching, FormComponents.AdminDateInputComponent),
        new(Kx13FieldDataType.TimeSpan, FieldDataType.TimeSpan, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(Kx13FieldDataType.Boolean, FieldDataType.Boolean, SfcDirective.CatchAnyNonMatching, FormComponents.AdminCheckBoxComponent),
        new(Kx13FieldDataType.DocAttachments, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, new[] { TcaDirective.ConvertToAsset }),
        new(Kx13FieldDataType.File, FieldDataType.Assets, SfcDirective.CatchAnyNonMatching, FormComponents.AdminAssetSelectorComponent, new[] { TcaDirective.ConvertToAsset }),
        new(Kx13FieldDataType.Guid, FieldDataType.LongText, "RelatedDocuments", FormComponents.AdminPageSelectorComponent, new [] { TcaDirective.ConvertToPages }),
        new(Kx13FieldDataType.Guid, FieldDataType.Guid, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(Kx13FieldDataType.Binary, FieldDataType.Binary, SfcDirective.CatchAnyNonMatching, TfcDirective.Clear),
        new(Kx13FieldDataType.Xml, FieldDataType.Xml, SfcDirective.CatchAnyNonMatching, FormComponents.AdminNumberWithLabelComponent),
        new(Kx13FieldDataType.DocRelationships, FieldDataType.Pages, SfcDirective.CatchAnyNonMatching, FormComponents.AdminPageSelectorComponent, new[] { TcaDirective.ConvertToPages }),
    };

    public static DataTypeMigrationModel BuiltInModel => new(
        BuiltInFieldMigrations,
        new FormComponentReplacement[]
        {
            new(Kx13FormComponents.Kentico_AttachmentSelector, FormComponents.AdminAssetSelectorComponent), new(Kx13FormComponents.Kentico_PageSelector, FormComponents.AdminPageSelectorComponent)
        },
        new string[]
        {
            // Legacy mode is no longer supported
            Kx13FormComponents.Kentico_BoolFieldValueTypeSelector, Kx13FormComponents.Kentico_CheckBox, Kx13FormComponents.Kentico_CompareToFieldSelector, Kx13FormComponents.Kentico_ConsentAgreement,
            Kx13FormComponents.Kentico_ConsentSelector, Kx13FormComponents.Kentico_DropDown, Kx13FormComponents.Kentico_EmailInput, Kx13FormComponents.Kentico_FileUploader,
            Kx13FormComponents.Kentico_HiddenGuidInput, Kx13FormComponents.Kentico_IntInput, Kx13FormComponents.Kentico_MultipleChoice, Kx13FormComponents.Kentico_Name,
            Kx13FormComponents.Kentico_NumericFieldComparisonTypeSelector, Kx13FormComponents.Kentico_RadioButtons, Kx13FormComponents.Kentico_Recaptcha,
            Kx13FormComponents.Kentico_StringFieldComparisonTypeSelector, Kx13FormComponents.Kentico_TextArea, Kx13FormComponents.Kentico_TextInput, Kx13FormComponents.Kentico_USPhone,
            Kx13FormComponents.Kentico_Invalid, Kx13FormComponents.Kentico_RichText,
            // Kentico_AttachmentSelector,
            Kx13FormComponents.Kentico_MediaFilesSelector, Kx13FormComponents.Kentico_GeneralSelector, Kx13FormComponents.Kentico_ObjectSelector,
            // Kentico_PageSelector,
            Kx13FormComponents.Kentico_PathSelector, Kx13FormComponents.Kentico_UrlSelector,
        });
}