namespace Migration.Toolkit.Core.Services.CmsClass;

using CMS.DataEngine;
using Migration.Toolkit.KX13.Auxiliary;
using Migration.Toolkit.KXP.Api.Auxiliary;
using FcText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForText;
using FcLongText = Migration.Toolkit.KX13.Auxiliary.Kx13FormControls.UserControlForLongText;

public record DataTypeModel(string TargetDataType, Dictionary<string, TargetComponent> FormComponents)
{
};

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
    public const string ClearFieldControl = "#clear-field-control#";
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
    public static Dictionary<string, DataTypeModel> Default => new()
    {
        {
            Kx13FieldDataType.ALL, new DataTypeModel(FieldDataType.ALL, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.ClearFieldControl)}
            })
        },
        {
            Kx13FieldDataType.Unknown, new DataTypeModel(FieldDataType.Unknown, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.ClearFieldControl)}
            })
        },
        {
            Kx13FieldDataType.Text, new DataTypeModel(FieldDataType.Text, new()
            {
                { FcText.TextBoxControl, new TargetComponent(FormComponents.AdminTextInputComponent)},
                { FcText.DropDownListControl, new TargetComponent(FormComponents.AdminDropDownComponent)},
                { FcText.IconSelector, new TargetComponent(FormComponents.AdminIconSelectorComponent)},
                { FcText.Password, new TargetComponent(FormComponents.AdminPasswordComponent)},
                { FcText.RadioButtonsControl, new TargetComponent(FormComponents.AdminRadioGroupComponent)},
                { FcText.TextAreaControl, new TargetComponent(FormComponents.AdminTextAreaComponent)},
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminTextInputComponent)}
            })
        },
        {
            Kx13FieldDataType.LongText, new DataTypeModel(FieldDataType.LongText, new()
            {
                { FcLongText.HtmlAreaControl, new TargetComponent(FormComponents.AdminTextInputComponent)},
                { FcLongText.TextBoxControl, new TargetComponent(FormComponents.AdminTextInputComponent)},
                { FcLongText.DropDownListControl, new TargetComponent(FormComponents.AdminDropDownComponent)},
                { FcLongText.TextAreaControl, new TargetComponent(FormComponents.AdminTextAreaComponent)},
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminRichTextEditorComponent)}
            })
        },
        {
            Kx13FieldDataType.Integer, new DataTypeModel(FieldDataType.Integer, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent)}
            })
        },
        {
            Kx13FieldDataType.LongInteger, new DataTypeModel(FieldDataType.LongInteger, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent)}
            })
        },
        {
            Kx13FieldDataType.Double, new DataTypeModel(FieldDataType.Double, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent)}
            })
        },
        {
            Kx13FieldDataType.Decimal, new DataTypeModel(FieldDataType.Decimal, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminNumberInputComponent)}
            })
        },
        {
            Kx13FieldDataType.DateTime, new DataTypeModel(FieldDataType.DateTime, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminDateTimeInputComponent)}
            })
        },
        {
            Kx13FieldDataType.Date, new DataTypeModel(FieldDataType.Date, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminDateInputComponent)}
            })
        },
        {
            Kx13FieldDataType.TimeSpan, new DataTypeModel(FieldDataType.TimeSpan, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.DoNothing)}
            })
        },
        {
            Kx13FieldDataType.Boolean, new DataTypeModel(FieldDataType.Boolean, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminCheckBoxComponent)}
            })
        },
        {
            Kx13FieldDataType.DocAttachments, new DataTypeModel(FieldDataType.Assets, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminAssetSelectorComponent, TcaDirective.ConvertToAsset)}
            })
        },
        {
            Kx13FieldDataType.File, new DataTypeModel(FieldDataType.Assets, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminAssetSelectorComponent, TcaDirective.ConvertToAsset)}
            })
        },
        {
            Kx13FieldDataType.Guid, new DataTypeModel(FieldDataType.Guid, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.ClearFieldControl)}
            })
        },
        {
            Kx13FieldDataType.Binary, new DataTypeModel(FieldDataType.Binary, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.DoNothing)}
            })
        },
        {
            Kx13FieldDataType.Xml, new DataTypeModel(FieldDataType.Xml, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(TfcDirective.DoNothing)}
            })
        },
        {
            Kx13FieldDataType.DocRelationships, new DataTypeModel(FieldDataType.Pages, new()
            {
                { SfcDirective.CatchAnyNonMatching, new TargetComponent(FormComponents.AdminPageSelectorComponent, TcaDirective.ConvertToPages)}
            })
        }
    };
}