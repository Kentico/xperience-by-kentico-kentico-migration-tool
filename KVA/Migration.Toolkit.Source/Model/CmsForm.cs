// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;

public interface ICmsForm : ISourceModel<ICmsForm>
{
    int FormID { get; }
    string FormDisplayName { get; }
    string FormName { get; }
    string? FormSendToEmail { get; }
    string? FormSendFromEmail { get; }
    string? FormEmailSubject { get; }
    string? FormEmailTemplate { get; }
    bool? FormEmailAttachUploadedDocs { get; }
    int FormClassID { get; }
    int FormItems { get; }
    string? FormReportFields { get; }
    string? FormRedirectToUrl { get; }
    string? FormDisplayText { get; }
    bool FormClearAfterSave { get; }
    string? FormSubmitButtonText { get; }
    int FormSiteID { get; }
    string? FormConfirmationEmailField { get; }
    string? FormConfirmationTemplate { get; }
    string? FormConfirmationSendFromEmail { get; }
    string? FormConfirmationEmailSubject { get; }
    int? FormAccess { get; }
    string? FormSubmitButtonImage { get; }
    Guid FormGUID { get; }
    DateTime FormLastModified { get; }

    static string ISourceModel<ICmsForm>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsFormK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsFormK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsFormK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static bool ISourceModel<ICmsForm>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsFormK11.IsAvailable(version),
        { Major: 12 } => CmsFormK12.IsAvailable(version),
        { Major: 13 } => CmsFormK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static string ISourceModel<ICmsForm>.TableName => "CMS_Form";
    static string ISourceModel<ICmsForm>.GuidColumnName => "FormGUID"; //assumtion, class Guid column doesn't change between versions

    static ICmsForm ISourceModel<ICmsForm>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsFormK11.FromReader(reader, version),
        { Major: 12 } => CmsFormK12.FromReader(reader, version),
        { Major: 13 } => CmsFormK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}

public record CmsFormK11(
    int FormID,
    string FormDisplayName,
    string FormName,
    string? FormSendToEmail,
    string? FormSendFromEmail,
    string? FormEmailSubject,
    string? FormEmailTemplate,
    bool? FormEmailAttachUploadedDocs,
    int FormClassID,
    int FormItems,
    string? FormReportFields,
    string? FormRedirectToUrl,
    string? FormDisplayText,
    bool FormClearAfterSave,
    string? FormSubmitButtonText,
    int FormSiteID,
    string? FormConfirmationEmailField,
    string? FormConfirmationTemplate,
    string? FormConfirmationSendFromEmail,
    string? FormConfirmationEmailSubject,
    int? FormAccess,
    string? FormSubmitButtonImage,
    Guid FormGUID,
    DateTime FormLastModified,
    bool? FormLogActivity) : ICmsForm, ISourceModel<CmsFormK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FormID";
    public static string TableName => "CMS_Form";
    public static string GuidColumnName => "FormGUID";

    static CmsFormK11 ISourceModel<CmsFormK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<string?>("FormSendToEmail"), reader.Unbox<string?>("FormSendFromEmail"), reader.Unbox<string?>("FormEmailSubject"),
        reader.Unbox<string?>("FormEmailTemplate"), reader.Unbox<bool?>("FormEmailAttachUploadedDocs"), reader.Unbox<int>("FormClassID"), reader.Unbox<int>("FormItems"), reader.Unbox<string?>("FormReportFields"),
        reader.Unbox<string?>("FormRedirectToUrl"), reader.Unbox<string?>("FormDisplayText"), reader.Unbox<bool>("FormClearAfterSave"), reader.Unbox<string?>("FormSubmitButtonText"), reader.Unbox<int>("FormSiteID"),
        reader.Unbox<string?>("FormConfirmationEmailField"), reader.Unbox<string?>("FormConfirmationTemplate"), reader.Unbox<string?>("FormConfirmationSendFromEmail"), reader.Unbox<string?>("FormConfirmationEmailSubject"),
        reader.Unbox<int?>("FormAccess"), reader.Unbox<string?>("FormSubmitButtonImage"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<bool?>("FormLogActivity")
    );

    public static CmsFormK11 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<string?>("FormSendToEmail"), reader.Unbox<string?>("FormSendFromEmail"), reader.Unbox<string?>("FormEmailSubject"),
        reader.Unbox<string?>("FormEmailTemplate"), reader.Unbox<bool?>("FormEmailAttachUploadedDocs"), reader.Unbox<int>("FormClassID"), reader.Unbox<int>("FormItems"), reader.Unbox<string?>("FormReportFields"),
        reader.Unbox<string?>("FormRedirectToUrl"), reader.Unbox<string?>("FormDisplayText"), reader.Unbox<bool>("FormClearAfterSave"), reader.Unbox<string?>("FormSubmitButtonText"), reader.Unbox<int>("FormSiteID"),
        reader.Unbox<string?>("FormConfirmationEmailField"), reader.Unbox<string?>("FormConfirmationTemplate"), reader.Unbox<string?>("FormConfirmationSendFromEmail"), reader.Unbox<string?>("FormConfirmationEmailSubject"),
        reader.Unbox<int?>("FormAccess"), reader.Unbox<string?>("FormSubmitButtonImage"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<bool?>("FormLogActivity")
    );
}

public record CmsFormK12(
    int FormID,
    string FormDisplayName,
    string FormName,
    string? FormSendToEmail,
    string? FormSendFromEmail,
    string? FormEmailSubject,
    string? FormEmailTemplate,
    bool? FormEmailAttachUploadedDocs,
    int FormClassID,
    int FormItems,
    string? FormReportFields,
    string? FormRedirectToUrl,
    string? FormDisplayText,
    bool FormClearAfterSave,
    string? FormSubmitButtonText,
    int FormSiteID,
    string? FormConfirmationEmailField,
    string? FormConfirmationTemplate,
    string? FormConfirmationSendFromEmail,
    string? FormConfirmationEmailSubject,
    int? FormAccess,
    string? FormSubmitButtonImage,
    Guid FormGUID,
    DateTime FormLastModified,
    bool? FormLogActivity,
    int FormDevelopmentModel,
    string? FormBuilderLayout) : ICmsForm, ISourceModel<CmsFormK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FormID";
    public static string TableName => "CMS_Form";
    public static string GuidColumnName => "FormGUID";

    static CmsFormK12 ISourceModel<CmsFormK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<string?>("FormSendToEmail"), reader.Unbox<string?>("FormSendFromEmail"), reader.Unbox<string?>("FormEmailSubject"),
        reader.Unbox<string?>("FormEmailTemplate"), reader.Unbox<bool?>("FormEmailAttachUploadedDocs"), reader.Unbox<int>("FormClassID"), reader.Unbox<int>("FormItems"), reader.Unbox<string?>("FormReportFields"),
        reader.Unbox<string?>("FormRedirectToUrl"), reader.Unbox<string?>("FormDisplayText"), reader.Unbox<bool>("FormClearAfterSave"), reader.Unbox<string?>("FormSubmitButtonText"), reader.Unbox<int>("FormSiteID"),
        reader.Unbox<string?>("FormConfirmationEmailField"), reader.Unbox<string?>("FormConfirmationTemplate"), reader.Unbox<string?>("FormConfirmationSendFromEmail"), reader.Unbox<string?>("FormConfirmationEmailSubject"),
        reader.Unbox<int?>("FormAccess"), reader.Unbox<string?>("FormSubmitButtonImage"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<bool?>("FormLogActivity"), reader.Unbox<int>("FormDevelopmentModel"),
        reader.Unbox<string?>("FormBuilderLayout")
    );

    public static CmsFormK12 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<string?>("FormSendToEmail"), reader.Unbox<string?>("FormSendFromEmail"), reader.Unbox<string?>("FormEmailSubject"),
        reader.Unbox<string?>("FormEmailTemplate"), reader.Unbox<bool?>("FormEmailAttachUploadedDocs"), reader.Unbox<int>("FormClassID"), reader.Unbox<int>("FormItems"), reader.Unbox<string?>("FormReportFields"),
        reader.Unbox<string?>("FormRedirectToUrl"), reader.Unbox<string?>("FormDisplayText"), reader.Unbox<bool>("FormClearAfterSave"), reader.Unbox<string?>("FormSubmitButtonText"), reader.Unbox<int>("FormSiteID"),
        reader.Unbox<string?>("FormConfirmationEmailField"), reader.Unbox<string?>("FormConfirmationTemplate"), reader.Unbox<string?>("FormConfirmationSendFromEmail"), reader.Unbox<string?>("FormConfirmationEmailSubject"),
        reader.Unbox<int?>("FormAccess"), reader.Unbox<string?>("FormSubmitButtonImage"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<bool?>("FormLogActivity"), reader.Unbox<int>("FormDevelopmentModel"),
        reader.Unbox<string?>("FormBuilderLayout")
    );
}

public record CmsFormK13(
    int FormID,
    string FormDisplayName,
    string FormName,
    string? FormSendToEmail,
    string? FormSendFromEmail,
    string? FormEmailSubject,
    string? FormEmailTemplate,
    bool? FormEmailAttachUploadedDocs,
    int FormClassID,
    int FormItems,
    string? FormReportFields,
    string? FormRedirectToUrl,
    string? FormDisplayText,
    bool FormClearAfterSave,
    string? FormSubmitButtonText,
    int FormSiteID,
    string? FormConfirmationEmailField,
    string? FormConfirmationTemplate,
    string? FormConfirmationSendFromEmail,
    string? FormConfirmationEmailSubject,
    int? FormAccess,
    string? FormSubmitButtonImage,
    Guid FormGUID,
    DateTime FormLastModified,
    bool FormLogActivity,
    string? FormBuilderLayout) : ICmsForm, ISourceModel<CmsFormK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "FormID";
    public static string TableName => "CMS_Form";
    public static string GuidColumnName => "FormGUID";

    static CmsFormK13 ISourceModel<CmsFormK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<string?>("FormSendToEmail"), reader.Unbox<string?>("FormSendFromEmail"), reader.Unbox<string?>("FormEmailSubject"),
        reader.Unbox<string?>("FormEmailTemplate"), reader.Unbox<bool?>("FormEmailAttachUploadedDocs"), reader.Unbox<int>("FormClassID"), reader.Unbox<int>("FormItems"), reader.Unbox<string?>("FormReportFields"),
        reader.Unbox<string?>("FormRedirectToUrl"), reader.Unbox<string?>("FormDisplayText"), reader.Unbox<bool>("FormClearAfterSave"), reader.Unbox<string?>("FormSubmitButtonText"), reader.Unbox<int>("FormSiteID"),
        reader.Unbox<string?>("FormConfirmationEmailField"), reader.Unbox<string?>("FormConfirmationTemplate"), reader.Unbox<string?>("FormConfirmationSendFromEmail"), reader.Unbox<string?>("FormConfirmationEmailSubject"),
        reader.Unbox<int?>("FormAccess"), reader.Unbox<string?>("FormSubmitButtonImage"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<bool>("FormLogActivity"), reader.Unbox<string?>("FormBuilderLayout")
    );

    public static CmsFormK13 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("FormID"), reader.Unbox<string>("FormDisplayName"), reader.Unbox<string>("FormName"), reader.Unbox<string?>("FormSendToEmail"), reader.Unbox<string?>("FormSendFromEmail"), reader.Unbox<string?>("FormEmailSubject"),
        reader.Unbox<string?>("FormEmailTemplate"), reader.Unbox<bool?>("FormEmailAttachUploadedDocs"), reader.Unbox<int>("FormClassID"), reader.Unbox<int>("FormItems"), reader.Unbox<string?>("FormReportFields"),
        reader.Unbox<string?>("FormRedirectToUrl"), reader.Unbox<string?>("FormDisplayText"), reader.Unbox<bool>("FormClearAfterSave"), reader.Unbox<string?>("FormSubmitButtonText"), reader.Unbox<int>("FormSiteID"),
        reader.Unbox<string?>("FormConfirmationEmailField"), reader.Unbox<string?>("FormConfirmationTemplate"), reader.Unbox<string?>("FormConfirmationSendFromEmail"), reader.Unbox<string?>("FormConfirmationEmailSubject"),
        reader.Unbox<int?>("FormAccess"), reader.Unbox<string?>("FormSubmitButtonImage"), reader.Unbox<Guid>("FormGUID"), reader.Unbox<DateTime>("FormLastModified"), reader.Unbox<bool>("FormLogActivity"), reader.Unbox<string?>("FormBuilderLayout")
    );
}
