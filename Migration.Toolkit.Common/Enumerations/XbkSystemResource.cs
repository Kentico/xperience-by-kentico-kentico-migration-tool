// ReSharper disable InconsistentNaming

namespace Migration.Toolkit.Common.Enumerations;

public static class XbkSystemResource
{
    public const string CMS = "CMS";
    public const string CMS_ABTest = "CMS.ABTest";
    public const string CMS_Activities = "CMS.Activities";
    public const string CMS_ContactManagement = "CMS.ContactManagement";
    public const string CMS_Content = "CMS.Content";
    public const string CMS_ContinuousIntegration = "CMS.ContinuousIntegration";
    public const string CMS_CrossSiteTracking = "CMS.CrossSiteTracking";
    public const string CMS_CustomTables = "CMS.CustomTables";
    public const string CMS_DataProtection = "CMS.DataProtection";
    public const string CMS_Design = "CMS.Design";
    public const string CMS_DocumentEngine = "CMS.DocumentEngine";
    public const string CMS_EmailEngine = "CMS.EmailEngine";
    public const string CMS_EmailLibrary = "CMS.EmailLibrary";
    public const string CMS_EmailTemplates = "CMS.EmailTemplates";
    public const string CMS_EventLog = "CMS.EventLog";
    public const string CMS_Form = "CMS.Form";
    public const string CMS_Globalization = "CMS.Globalization";
    public const string CMS_GlobalPermissions = "CMS.GlobalPermissions";
    public const string CMS_Licenses = "CMS.Licenses";
    public const string CMS_Localization = "CMS.Localization";
    public const string CMS_MacroEngine = "CMS.MacroEngine";
    public const string CMS_MediaDialog = "CMS.MediaDialog";
    public const string CMS_MediaLibrary = "CMS.MediaLibrary";
    public const string CMS_Membership = "CMS.Membership";
    public const string CMS_ModuleLicenses = "CMS.ModuleLicenses";
    public const string CMS_OnlineMarketing = "CMS.OnlineMarketing";
    public const string CMS_Permissions = "CMS.Permissions";
    public const string CMS_Roles = "CMS.Roles";
    public const string CMS_ScheduledTasks = "CMS.ScheduledTasks";
    public const string CMS_Search = "CMS.Search";
    public const string CMS_Search_Azure = "CMS.Search.Azure";
    public const string CMS_Staging = "CMS.Staging";
    public const string CMS_Synchronization = "CMS.Synchronization";
    public const string CMS_UIPersonalization = "CMS.UIPersonalization";
    public const string CMS_Users = "CMS.Users";
    public const string CMS_WebFarm = "CMS.WebFarm";
    public const string CMS_WorkflowEngine = "CMS.WorkflowEngine";
    public const string CMS_WYSIWYGEditor = "CMS.WYSIWYGEditor";

    public static HashSet<string> All = new(new[]
    {
        CMS, CMS_ABTest, CMS_Activities, CMS_ContactManagement, CMS_Content, CMS_ContinuousIntegration, CMS_CrossSiteTracking, CMS_CustomTables, CMS_DataProtection, CMS_Design, CMS_DocumentEngine,
        CMS_EmailEngine, CMS_EmailLibrary, CMS_EmailTemplates, CMS_EventLog, CMS_Form, CMS_Globalization, CMS_GlobalPermissions, CMS_Licenses, CMS_Localization, CMS_MacroEngine, CMS_MediaDialog,
        CMS_MediaLibrary, CMS_Membership, CMS_ModuleLicenses, CMS_OnlineMarketing, CMS_Permissions, CMS_Roles, CMS_ScheduledTasks, CMS_Search, CMS_Search_Azure, CMS_Staging, CMS_Synchronization,
        CMS_UIPersonalization, CMS_Users, CMS_WebFarm, CMS_WorkflowEngine, CMS_WYSIWYGEditor,
    }, StringComparer.InvariantCultureIgnoreCase);
}