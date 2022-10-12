// ReSharper disable InconsistentNaming

namespace Migration.Toolkit.Common.Enumerations;

public static class Kx13SystemResource
{
    public const string CMS = "CMS";
    public const string CMS_ABTest = "CMS.ABTest";
    public const string CMS_Activities = "CMS.Activities";
    public const string CMS_Badges = "CMS.Badges";
    public const string CMS_Categories = "CMS.Categories";
    public const string CMS_ContactManagement = "CMS.ContactManagement";
    public const string CMS_Content = "CMS.Content";
    public const string CMS_ContinuousIntegration = "CMS.ContinuousIntegration";
    public const string cms_customsystemmodule = "cms.customsystemmodule";
    public const string CMS_CustomTables = "CMS.CustomTables";
    public const string CMS_DataProtection = "CMS.DataProtection";
    public const string CMS_Design = "CMS.Design";
    public const string CMS_DocumentEngine = "CMS.DocumentEngine";
    public const string CMS_Ecommerce = "CMS.Ecommerce";
    public const string CMS_EmailEngine = "CMS.EmailEngine";
    public const string CMS_EmailTemplates = "CMS.EmailTemplates";
    public const string CMS_EventLog = "CMS.EventLog";
    public const string CMS_Form = "CMS.Form";
    public const string CMS_Globalization = "CMS.Globalization";
    public const string CMS_GlobalPermissions = "CMS.GlobalPermissions";
    public const string CMS_Localization = "CMS.Localization";
    public const string CMS_MacroEngine = "CMS.MacroEngine";
    public const string CMS_MediaDialog = "CMS.MediaDialog";
    public const string CMS_MediaLibrary = "CMS.MediaLibrary";
    public const string CMS_Membership = "CMS.Membership";
    public const string CMS_ModuleLicenses = "CMS.ModuleLicenses";
    public const string CMS_ModuleUsageTracking = "CMS.ModuleUsageTracking";
    public const string CMS_Newsletter = "CMS.Newsletter";
    public const string CMS_OnlineMarketing = "CMS.OnlineMarketing";
    public const string CMS_Permissions = "CMS.Permissions";
    public const string CMS_Personas = "CMS.Personas";
    public const string CMS_Relationships = "CMS.Relationships";
    public const string CMS_Reporting = "CMS.Reporting";
    public const string CMS_Roles = "CMS.Roles";
    public const string CMS_ScheduledTasks = "CMS.ScheduledTasks";
    public const string CMS_Scoring = "CMS.Scoring";
    public const string CMS_Search = "CMS.Search";
    public const string CMS_Search_Azure = "CMS.Search.Azure";
    public const string CMS_SharePoint = "CMS.SharePoint";
    public const string CMS_SocialMarketing = "CMS.SocialMarketing";
    public const string CMS_Staging = "CMS.Staging";
    public const string CMS_Synchronization = "CMS.Synchronization";
    public const string CMS_Taxonomy = "CMS.Taxonomy";
    public const string CMS_TranslationServices = "CMS.TranslationServices";
    public const string CMS_UIPersonalization = "CMS.UIPersonalization";
    public const string CMS_Users = "CMS.Users";
    public const string CMS_WebAnalytics = "CMS.WebAnalytics";
    public const string CMS_WebFarm = "CMS.WebFarm";
    public const string CMS_Widgets = "CMS.Widgets";
    public const string CMS_WIFIntegration = "CMS.WIFIntegration";
    public const string CMS_WorkflowEngine = "CMS.WorkflowEngine";
    public const string CMS_WYSIWYGEditor = "CMS.WYSIWYGEditor";
    public const string Licenses = "Licenses";

    public static HashSet<string> All = new(new[]
    {
        CMS, CMS_ABTest, CMS_Activities, CMS_Badges, CMS_Categories, CMS_ContactManagement, CMS_Content, CMS_ContinuousIntegration, cms_customsystemmodule, CMS_CustomTables, CMS_DataProtection,
        CMS_Design, CMS_DocumentEngine, CMS_Ecommerce, CMS_EmailEngine, CMS_EmailTemplates, CMS_EventLog, CMS_Form, CMS_Globalization, CMS_GlobalPermissions, CMS_Localization, CMS_MacroEngine,
        CMS_MediaDialog, CMS_MediaLibrary, CMS_Membership, CMS_ModuleLicenses, CMS_ModuleUsageTracking, CMS_Newsletter, CMS_OnlineMarketing, CMS_Permissions,
        CMS_Personas, CMS_Relationships, CMS_Reporting, CMS_Roles, CMS_ScheduledTasks, CMS_Scoring, CMS_Search, CMS_Search_Azure, CMS_SharePoint, CMS_SocialMarketing, CMS_Staging,
        CMS_Synchronization, CMS_Taxonomy, CMS_TranslationServices, CMS_UIPersonalization, CMS_Users, CMS_WebAnalytics, CMS_WebFarm, CMS_Widgets, CMS_WIFIntegration, CMS_WorkflowEngine,
        CMS_WYSIWYGEditor, Licenses,
    }, StringComparer.InvariantCultureIgnoreCase);

    public static HashSet<string> ConvertToNonSysResource = new(new[] { cms_customsystemmodule }, StringComparer.CurrentCultureIgnoreCase);
}