namespace Migration.Toolkit.Core.Services.CmsClass;

using CMS.DataEngine;
using Migration.Toolkit.KX13.Auxiliary;
using Migration.Toolkit.KXP.Api.Auxiliary;
using FcText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForText;
using FcLongText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForLongText;

public record FormComponentReplacement(string OldFormComponent, string NewFormComponent);

public record DataTypeConversionModel(Dictionary<string, DataTypeModel> DataTypeMappings, FormComponentReplacement[] NotSupportedInKxpLegacyMode,
    string[] SupportedInKxpLegacyMode);

public record DataTypeModel(string TargetDataType, Dictionary<string, TargetComponent> FormComponents);

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

    // public const string ClearFieldControl = "#clear-field-control#"; not acceptable
    public const string DoNothing = "#leave-it-as-it-is#";
}

/// <summary>
/// Sfc = source form control 
/// </summary>
public static class SfcDirective
{
    public const string CatchAnyNonMatching = "#any-not-matched-by-control-mapping-list#";
}

public record TargetComponent(string TargetFormComponent, params string[] Actions);

public static class FieldMappingInstance
{
    public static DataTypeConversionModel Default => new(new Dictionary<string, DataTypeModel>
        {
            {
                Kx13FieldDataType.ALL, new DataTypeModel(FieldDataType.ALL, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.CopySourceControl) }
                })
            },
            {
                Kx13FieldDataType.Unknown, new DataTypeModel(FieldDataType.Unknown, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.CopySourceControl) }
                })
            },
            {
                Kx13FieldDataType.Text, new DataTypeModel(FieldDataType.Text, new()
                {
                    { FcText.TextBoxControl, new TargetComponent(FormComponents.AdminTextInputComponent) },
                    { FcText.DropDownListControl, new TargetComponent(FormComponents.AdminDropDownComponent) },
                    { FcText.IconSelector, new TargetComponent(FormComponents.AdminIconSelectorComponent) },
                    { FcText.Password, new TargetComponent(FormComponents.AdminPasswordComponent) },
                    { FcText.RadioButtonsControl, new TargetComponent(FormComponents.AdminRadioGroupComponent) },
                    { FcText.TextAreaControl, new TargetComponent(FormComponents.AdminTextAreaComponent) },
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminTextInputComponent) }
                })
            },
            {
                Kx13FieldDataType.LongText, new DataTypeModel(FieldDataType.LongText, new()
                {
                    { FcLongText.HtmlAreaControl, new TargetComponent(FormComponents.AdminRichTextEditorComponent) },
                    { FcLongText.TextBoxControl, new TargetComponent(FormComponents.AdminTextInputComponent) },
                    { FcLongText.DropDownListControl, new TargetComponent(FormComponents.AdminDropDownComponent) },
                    { FcLongText.TextAreaControl, new TargetComponent(FormComponents.AdminTextAreaComponent) },
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminRichTextEditorComponent) }
                })
            },
            {
                Kx13FieldDataType.Integer, new DataTypeModel(FieldDataType.Integer, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent) }
                })
            },
            {
                Kx13FieldDataType.LongInteger, new DataTypeModel(FieldDataType.LongInteger, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent) }
                })
            },
            {
                Kx13FieldDataType.Double, new DataTypeModel(FieldDataType.Double, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent) }
                })
            },
            {
                Kx13FieldDataType.Decimal, new DataTypeModel(FieldDataType.Decimal, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminDecimalNumberInputComponent) }
                })
            },
            {
                Kx13FieldDataType.DateTime, new DataTypeModel(FieldDataType.DateTime, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminDateTimeInputComponent) }
                })
            },
            {
                Kx13FieldDataType.Date, new DataTypeModel(FieldDataType.Date, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminDateInputComponent) }
                })
            },
            {
                Kx13FieldDataType.TimeSpan, new DataTypeModel(FieldDataType.TimeSpan, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.CopySourceControl) }
                })
            },
            {
                Kx13FieldDataType.Boolean, new DataTypeModel(FieldDataType.Boolean, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminCheckBoxComponent) }
                })
            },
            {
                Kx13FieldDataType.DocAttachments, new DataTypeModel(FieldDataType.Assets, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminAssetSelectorComponent, TcaDirective.ConvertToAsset) }
                })
            },
            {
                Kx13FieldDataType.File, new DataTypeModel(FieldDataType.Assets, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminAssetSelectorComponent, TcaDirective.ConvertToAsset) }
                })
            },
            {
                Kx13FieldDataType.Guid, new DataTypeModel(FieldDataType.Guid, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.CopySourceControl) }
                })
            },
            {
                Kx13FieldDataType.Binary, new DataTypeModel(FieldDataType.Binary, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.DoNothing) }
                })
            },
            {
                Kx13FieldDataType.Xml, new DataTypeModel(FieldDataType.Xml, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.DoNothing) }
                })
            },
            {
                Kx13FieldDataType.DocRelationships, new DataTypeModel(FieldDataType.Pages, new()
                {
                    { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminPageSelectorComponent, TcaDirective.ConvertToPages) }
                })
            }
        },
        new FormComponentReplacement[]
        {
            new(Kx13FormComponents.Kentico_AttachmentSelector, FormComponents.AdminAssetSelectorComponent),
            new(Kx13FormComponents.Kentico_PageSelector, FormComponents.AdminPageSelectorComponent)
        },
        new[]
        {
            Kx13FormComponents.Kentico_BoolFieldValueTypeSelector,
            Kx13FormComponents.Kentico_CheckBox,
            Kx13FormComponents.Kentico_CompareToFieldSelector,
            Kx13FormComponents.Kentico_ConsentAgreement,
            Kx13FormComponents.Kentico_ConsentSelector,
            Kx13FormComponents.Kentico_DropDown,
            Kx13FormComponents.Kentico_EmailInput,
            Kx13FormComponents.Kentico_FileUploader,
            Kx13FormComponents.Kentico_HiddenGuidInput,
            Kx13FormComponents.Kentico_IntInput,
            Kx13FormComponents.Kentico_MultipleChoice,
            Kx13FormComponents.Kentico_Name,
            Kx13FormComponents.Kentico_NumericFieldComparisonTypeSelector,
            Kx13FormComponents.Kentico_RadioButtons,
            Kx13FormComponents.Kentico_Recaptcha,
            Kx13FormComponents.Kentico_StringFieldComparisonTypeSelector,
            Kx13FormComponents.Kentico_TextArea,
            Kx13FormComponents.Kentico_TextInput,
            Kx13FormComponents.Kentico_USPhone,
            Kx13FormComponents.Kentico_Invalid,
            Kx13FormComponents.Kentico_RichText,
            // Kentico_AttachmentSelector,
            Kx13FormComponents.Kentico_MediaFilesSelector,
            Kx13FormComponents.Kentico_GeneralSelector,
            Kx13FormComponents.Kentico_ObjectSelector,
            // Kentico_PageSelector,
            Kx13FormComponents.Kentico_PathSelector,
            Kx13FormComponents.Kentico_UrlSelector,
        });
}