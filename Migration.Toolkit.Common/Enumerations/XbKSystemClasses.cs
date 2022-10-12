// ReSharper disable InconsistentNaming

namespace Migration.Toolkit.Common.Enumerations;

public class XbKSystemClasses
{
    public const string cms_culture = "cms.culture";
    public const string cms_site = "cms.site";
    public const string cms_Role = "cms.Role";
    public const string cms_user = "cms.user";
    public const string cms_UserRole = "cms.UserRole";
    public const string cms_emailtemplate = "cms.emailtemplate";
    public const string cms_permission = "cms.permission";
    public const string cms_resource = "cms.resource";
    public const string CMS_EventLog = "CMS.EventLog";
    public const string cms_tree = "cms.tree";
    public const string cms_document = "cms.document";
    public const string cms_class = "cms.class";
    public const string cms_query = "cms.query";
    public const string cms_transformation = "cms.transformation";
    public const string cms_workflow = "cms.workflow";
    public const string cms_workflowstep = "cms.workflowstep";
    public const string cms_workflowscope = "cms.workflowscope";
    public const string cms_versionhistory = "cms.versionhistory";
    public const string cms_sitedomainalias = "cms.sitedomainalias";
    public const string cms_form = "cms.form";
    public const string cms_LicenseKey = "cms.LicenseKey";
    public const string cms_WebFarmServer = "cms.WebFarmServer";
    public const string cms_country = "cms.country";
    public const string cms_state = "cms.state";
    public const string staging_synchronization = "staging.synchronization";
    public const string staging_server = "staging.server";
    public const string CMS_SettingsKey = "CMS.SettingsKey";
    public const string export_history = "export.history";
    public const string CMS_ResourceSite = "CMS.ResourceSite";
    public const string CMS_CultureSite = "CMS.CultureSite";
    public const string CMS_UserSite = "CMS.UserSite";
    public const string CMS_WorkflowStepRole = "CMS.WorkflowStepRole";
    public const string CMS_ClassSite = "CMS.ClassSite";
    public const string cms_FormRole = "cms.FormRole";
    public const string cms_rolepermission = "cms.rolepermission";
    public const string cms_settingscategory = "cms.settingscategory";
    public const string cms_AlternativeForm = "cms.AlternativeForm";
    public const string cms_timezone = "cms.timezone";
    public const string cms_email = "cms.email";
    public const string cms_attachmentforemail = "cms.attachmentforemail";
    public const string media_library = "media.library";
    public const string media_file = "media.file";
    public const string media_libraryrolepermission = "media.libraryrolepermission";
    public const string cms_SearchIndex = "cms.SearchIndex";
    public const string cms_SearchIndexSite = "cms.SearchIndexSite";
    public const string cms_SearchIndexCulture = "cms.SearchIndexCulture";
    public const string CMS_SearchTask = "CMS.SearchTask";
    public const string cms_userculture = "cms.userculture";
    public const string OM_ABTest = "OM.ABTest";
    public const string CMS_Membership = "CMS.Membership";
    public const string CMS_MembershipRole = "CMS.MembershipRole";
    public const string CMS_MembershipUser = "CMS.MembershipUser";
    public const string OM_Account = "OM.Account";
    public const string OM_AccountStatus = "OM.AccountStatus";
    public const string OM_Contact = "OM.Contact";
    public const string OM_ContactStatus = "OM.ContactStatus";
    public const string OM_ContactRole = "OM.ContactRole";
    public const string OM_AccountContact = "OM.AccountContact";
    public const string OM_ContactGroup = "OM.ContactGroup";
    public const string OM_ContactGroupMember = "OM.ContactGroupMember";
    public const string OM_Activity = "OM.Activity";
    public const string OM_ActivityType = "OM.ActivityType";
    public const string Integration_Connector = "Integration.Connector";
    public const string Integration_Synchronization = "Integration.Synchronization";
    public const string CMS_WorkflowTransition = "CMS.WorkflowTransition";
    public const string CMS_MacroRule = "CMS.MacroRule";
    public const string CMS_WorkflowStepUser = "CMS.WorkflowStepUser";
    public const string CMS_WorkflowUser = "CMS.WorkflowUser";
    public const string cms_workflowaction = "cms.workflowaction";
    public const string cms_webfarmservertask = "cms.webfarmservertask";
    public const string OM_VisitorToContact = "OM.VisitorToContact";
    public const string CMS_MacroIdentity = "CMS.MacroIdentity";
    public const string CMS_UserMacroIdentity = "CMS.UserMacroIdentity";
    public const string CMS_SearchTaskAzure = "CMS.SearchTaskAzure";
    public const string CMS_Consent = "CMS.Consent";
    public const string CMS_ConsentAgreement = "CMS.ConsentAgreement";
    public const string CMS_ConsentArchive = "CMS.ConsentArchive";
    public const string Temp_PageBuilderWidgets = "Temp.PageBuilderWidgets";
    public const string CMS_AlternativeUrl = "CMS.AlternativeUrl";
    public const string CMS_PageTemplateConfiguration = "CMS.PageTemplateConfiguration";
    public const string OM_ABVariantData = "OM.ABVariantData";
    public const string CMS_AutomationTemplate = "CMS.AutomationTemplate";
    public const string CMS_PageUrlPath = "CMS.PageUrlPath";
    public const string CMS_PageFormerUrlPath = "CMS.PageFormerUrlPath";
    public const string CMS_FormFeaturedField = "CMS.FormFeaturedField";
    public const string CMS_MacroRuleCategory = "CMS.MacroRuleCategory";
    public const string CMS_MacroRuleMacroRuleCategory = "CMS.MacroRuleMacroRuleCategory";
    public const string OM_TrackedWebsite = "OM.TrackedWebsite";
    public const string EmailLibrary_EmailConfiguration = "EmailLibrary.EmailConfiguration";
    public const string EmailLibrary_EmailTemplate = "EmailLibrary.EmailTemplate";
    public const string cms_contentrelationship = "cms.contentrelationship";
    public const string cms_contentrelationshipitem = "cms.contentrelationshipitem";

    public static HashSet<string> All = new(
        new[]
        {
            cms_culture, cms_site, cms_Role, cms_user, cms_UserRole, cms_emailtemplate, cms_permission, cms_resource, CMS_EventLog, cms_tree, cms_document, cms_class, cms_query,
            cms_transformation, cms_workflow, cms_workflowstep, cms_workflowscope, cms_versionhistory, cms_sitedomainalias, cms_form, cms_LicenseKey, cms_WebFarmServer, cms_country, cms_state,
            CMS_SettingsKey, CMS_ResourceSite, CMS_CultureSite, CMS_UserSite, CMS_WorkflowStepRole, CMS_ClassSite, cms_FormRole, cms_rolepermission, cms_settingscategory, cms_AlternativeForm,
            cms_timezone, cms_email, cms_attachmentforemail, cms_SearchIndex, cms_SearchIndexSite, cms_SearchIndexCulture, CMS_SearchTask, cms_userculture, CMS_Membership, CMS_MembershipRole,
            CMS_MembershipUser, CMS_WorkflowTransition, CMS_MacroRule, CMS_WorkflowStepUser, CMS_WorkflowUser, cms_workflowaction, cms_webfarmservertask, CMS_MacroIdentity, CMS_UserMacroIdentity,
            CMS_SearchTaskAzure, CMS_Consent, CMS_ConsentAgreement, CMS_ConsentArchive, CMS_AlternativeUrl, CMS_PageTemplateConfiguration, CMS_AutomationTemplate, CMS_PageUrlPath,
            CMS_PageFormerUrlPath, CMS_FormFeaturedField, CMS_MacroRuleCategory, CMS_MacroRuleMacroRuleCategory, cms_contentrelationship, cms_contentrelationshipitem,
        }, StringComparer.InvariantCultureIgnoreCase);
}