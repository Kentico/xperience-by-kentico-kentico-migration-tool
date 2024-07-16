using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Content.Web.Mvc;
using Kentico.Forms.Web.Mvc;
using Kentico.Forms.Web.Mvc.Internal;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.DigitalMarketing.Forms;
using Kentico.Xperience.Admin.Headless.Forms;
using Kentico.Xperience.Admin.Websites;
using Kentico.Xperience.Admin.Websites.Forms;

namespace Migration.Toolkit.KXP.Api.Auxiliary;

public class FormComponents
{
    ///<summary>Form component value type: Kentico.Forms.Web.Mvc.BoolFieldValueTypes</summary>
    public static string MvcBoolComparisonTypeSelectorComponent => BoolComparisonTypeSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Boolean</summary>
    public static string MvcCheckBoxComponent => Kentico.Forms.Web.Mvc.CheckBoxComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Guid</summary>
    public static string MvcCompareToFieldSelectorComponent => CompareToFieldSelectorComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.Guid, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string MvcConsentAgreementComponent => ConsentAgreementComponent.IDENTIFIER;

    public static string MvcConsentSelectorComponent => ConsentSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Forms.Web.Mvc.DropDownProperties</summary>
    public static string MvcDropDownComponent => Kentico.Forms.Web.Mvc.DropDownComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcEmailInputComponent => EmailInputComponent.IDENTIFIER;

    ///<summary>Form component value type: CMS.OnlineForms.BizFormUploadFile</summary>
    public static string MvcFileUploaderComponent => FileUploaderComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Guid</summary>
    public static string MvcHiddenGuidInputComponent => HiddenGuidInputComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.Int32, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string MvcIntInputComponent => IntInputComponent.IDENTIFIER;

    public static string MvcNameComponent => NameComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Forms.Web.Mvc.NumericFieldComparisonTypes</summary>
    public static string MvcNumericComparisonTypeSelectorComponent => NumericComparisonTypeSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Forms.Web.Mvc.RadioButtonsProperties</summary>
    public static string MvcRadioButtonsComponent => RadioButtonsComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcRecaptchaComponent => RecaptchaComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Forms.Web.Mvc.StringFieldComparisonTypes</summary>
    public static string MvcStringComparisonTypeSelectorComponent => StringComparisonTypeSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcTextAreaComponent => Kentico.Forms.Web.Mvc.TextAreaComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcTextInputComponent => Kentico.Forms.Web.Mvc.TextInputComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcUSPhoneComponent => USPhoneComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.Double, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string MvcInternal_DoubleInputComponent => DoubleInputComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Content.Web.Mvc.MultiSelectorProperties</summary>
    public static string Kentico_Content_Web_Mvc_MultipleChoiceComponent => MultipleChoiceComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcFcRichTextComponent => RichTextComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type:
    ///     System.Collections.Generic.IEnumerable`1[[Kentico.Components.Web.Mvc.FormComponents.MediaFilesSelectorItem,
    ///     Kentico.Content.Web.Mvc, Version=28.4.1.0, Culture=neutral, PublicKeyToken=834b12a258f213f9]]
    /// </summary>
    public static string MvcFcMediaFilesSelector => MediaFilesSelector.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Components.Web.Mvc.FormComponents.GeneralSelectorItem</summary>
    public static string MvcFcGeneralSelector => GeneralSelector.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Components.Web.Mvc.FormComponents.ObjectSelectorItem</summary>
    public static string MvcFcObjectSelector => ObjectSelector.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Components.Web.Mvc.FormComponents.PageSelectorItem</summary>
    public static string MvcFcPageSelector => PageSelector.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Components.Web.Mvc.FormComponents.PathSelectorItem</summary>
    public static string MvcFcPathSelector => PathSelector.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string MvcFcUrlSelector => UrlSelector.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Collections.Generic.IEnumerable`1[[CMS.MediaLibrary.AssetRelatedItem,
    ///     CMS.MediaLibrary, Version=28.4.1.0, Culture=neutral, PublicKeyToken=834b12a258f213f9]]
    /// </summary>
    public static string AdminAssetSelectorComponent => AssetSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Boolean</summary>
    public static string AdminCheckBoxComponent => Kentico.Xperience.Admin.Base.Forms.CheckBoxComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminCodeEditorComponent => CodeEditorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminConditionBuilderComponent => ConditionBuilderComponent.IDENTIFIER;

    ///<summary>Form component value type: CMS.ContentEngine.ContentItemAssetMetadata</summary>
    public static string AdminContentItemAssetUploaderComponent => ContentItemAssetUploaderComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Xperience.Admin.Base.Forms.ContentItemSelectorProperties</summary>
    public static string AdminContentItemSelectorComponent => ContentItemSelectorComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.DateTime, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string AdminDateInputComponent => DateInputComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.DateTime, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string AdminDateTimeInputComponent => DateTimeInputComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.Decimal, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string AdminDecimalNumberInputComponent => DecimalNumberInputComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminDropDownComponent => Kentico.Xperience.Admin.Base.Forms.DropDownComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminExtensionSelectorComponent => ExtensionSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminIconSelectorComponent => IconSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminLinkComponent => LinkComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.Int32, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string AdminNumberInputComponent => NumberInputComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Int32</summary>
    public static string AdminNumberWithLabelComponent => NumberWithLabelComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminObjectCodeNameSelectorComponent => ObjectCodeNameSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Guid</summary>
    public static string AdminObjectGuidSelectorComponent => ObjectGuidSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.Int32</summary>
    public static string AdminObjectIdSelectorComponent => ObjectIdSelectorComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Nullable`1[[System.Int32, System.Private.CoreLib, Version=8.0.0.0,
    ///     Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
    /// </summary>
    public static string AdminSingleObjectIdSelectorComponent => SingleObjectIdSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminPasswordComponent => PasswordComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminRadioGroupComponent => RadioGroupComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminRichTextEditorComponent => RichTextEditorComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Collections.Generic.IEnumerable`1[[CMS.ContentEngine.TagReference,
    ///     CMS.ContentEngine, Version=28.4.1.0, Culture=neutral, PublicKeyToken=834b12a258f213f9]]
    /// </summary>
    public static string AdminTagSelectorComponent => TagSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminTextAreaComponent => Kentico.Xperience.Admin.Base.Forms.TextAreaComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminTextInputComponent => Kentico.Xperience.Admin.Base.Forms.TextInputComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string AdminTextWithLabelComponent => TextWithLabelComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Xperience.Admin.Base.Forms.TileSelectorItem</summary>
    public static string AdminTileSelectorComponent => TileSelectorComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Collections.Generic.IEnumerable`1[[CMS.EmailLibrary.EmailRelatedItem,
    ///     CMS.EmailMarketing, Version=28.4.1.0, Culture=neutral, PublicKeyToken=834b12a258f213f9]]
    /// </summary>
    public static string Kentico_Xperience_Admin_DigitalMarketing_Forms_EmailSelectorComponent => EmailSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: Kentico.Xperience.Admin.Headless.Forms.HeadlessItemSelectorProperties</summary>
    public static string Kentico_Xperience_Admin_Headless_Forms_HeadlessItemSelectorComponent => HeadlessItemSelectorComponent.IDENTIFIER;

    /// <summary>
    ///     Form component value type: System.Collections.Generic.IEnumerable`1[[CMS.Websites.WebPageRelatedItem,
    ///     CMS.Websites, Version=28.4.1.0, Culture=neutral, PublicKeyToken=834b12a258f213f9]]
    /// </summary>
    public static string Kentico_Xperience_Admin_Websites_WebPageSelectorComponent => WebPageSelectorComponent.IDENTIFIER;

    ///<summary>Form component value type: System.String</summary>
    public static string Kentico_Xperience_Admin_Websites_Forms_UrlSelectorComponent => UrlSelectorComponent.IDENTIFIER;


    // Abstract control: Kentico.Forms.Web.Mvc.ComparisonTypeSelectorComponent`1
    // Abstract control: Kentico.Forms.Web.Mvc.SelectorFormComponent`1
    // No identity prop control: Kentico.Forms.Web.Mvc.InvalidComponent
    // Abstract control: Kentico.Content.Web.Mvc.MultiSelectorFormComponent`1
    // Abstract control: Kentico.Components.Web.Mvc.Selectors.Internal.ObjectSelectorComponentBase`2
    // Abstract control: Kentico.Components.Web.Mvc.FormComponents.Internal.PageSelectorBase`2
    // Internal control: Kentico.Xperience.Admin.Base.Forms.AdditionalActionListComponent
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.FormComponentWithNestedComponents`3
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.FormComponent`2
    // Internal control: Kentico.Xperience.Admin.Base.Forms.CodeNameComponent
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.ContentItemSelectorComponentBase`1
    // Internal control: Kentico.Xperience.Admin.Base.Forms.DateRangeInputComponent
    // Internal control: Kentico.Xperience.Admin.Base.Forms.DoubleNumberInputComponent
    // No identity prop control: Kentico.Xperience.Admin.Base.Forms.GeneralSelectorComponent
    // No identity prop control: Kentico.Xperience.Admin.Base.Forms.SingleGeneralSelectorComponent
    // Internal control: Kentico.Xperience.Admin.Base.Forms.MacroWrapperComponent
    // Internal control: Kentico.Xperience.Admin.Base.Forms.NamespaceCodeNameComponent
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.MultipleObjectSelectorBase`2
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.ObjectSelectorBase`4
    // No identity prop control: Kentico.Xperience.Admin.Base.Forms.ObjectSelectorComponent
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.SingleObjectSelectorBase`2
    // Internal control: Kentico.Xperience.Admin.Base.Forms.ValidationRuleListComponent
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.Internal.DateInputComponentBase`3
    // Abstract control: Kentico.Xperience.Admin.Base.Forms.Internal.GeneralSelectorComponentBase`3
    // No identity prop control: Kentico.Xperience.Admin.DigitalMarketing.Forms.FormSelectorComponent
}
