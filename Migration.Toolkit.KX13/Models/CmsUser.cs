using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_User")]
    [Index("Email", Name = "IX_CMS_User_Email")]
    [Index("FullName", Name = "IX_CMS_User_FullName")]
    [Index("UserEnabled", "UserIsHidden", Name = "IX_CMS_User_UserEnabled_UserIsHidden")]
    [Index("UserGuid", Name = "IX_CMS_User_UserGUID", IsUnique = true)]
    [Index("UserName", Name = "IX_CMS_User_UserName", IsUnique = true)]
    [Index("UserPrivilegeLevel", Name = "IX_CMS_User_UserPrivilegeLevel")]
    public partial class CmsUser
    {
        public CmsUser()
        {
            CmsAclitemLastModifiedByUsers = new HashSet<CmsAclitem>();
            CmsAclitemUsers = new HashSet<CmsAclitem>();
            CmsAutomationHistories = new HashSet<CmsAutomationHistory>();
            CmsAutomationStates = new HashSet<CmsAutomationState>();
            CmsCategories = new HashSet<CmsCategory>();
            CmsDocumentDocumentCheckedOutByUsers = new HashSet<CmsDocument>();
            CmsDocumentDocumentCreatedByUsers = new HashSet<CmsDocument>();
            CmsDocumentDocumentModifiedByUsers = new HashSet<CmsDocument>();
            CmsEmailUsers = new HashSet<CmsEmailUser>();
            CmsExternalLogins = new HashSet<CmsExternalLogin>();
            CmsMacroIdentities = new HashSet<CmsMacroIdentity>();
            CmsMembershipUsers = new HashSet<CmsMembershipUser>();
            CmsObjectSettings = new HashSet<CmsObjectSetting>();
            CmsObjectVersionHistoryVersionDeletedByUsers = new HashSet<CmsObjectVersionHistory>();
            CmsObjectVersionHistoryVersionModifiedByUsers = new HashSet<CmsObjectVersionHistory>();
            CmsPersonalizations = new HashSet<CmsPersonalization>();
            CmsScheduledTasks = new HashSet<CmsScheduledTask>();
            CmsTranslationSubmissions = new HashSet<CmsTranslationSubmission>();
            CmsTrees = new HashSet<CmsTree>();
            CmsUserCultures = new HashSet<CmsUserCulture>();
            CmsUserRoles = new HashSet<CmsUserRole>();
            CmsUserSettingUserActivatedByUsers = new HashSet<CmsUserSetting>();
            CmsUserSettingUserSettingsUserGus = new HashSet<CmsUserSetting>();
            CmsUserSites = new HashSet<CmsUserSite>();
            CmsVersionHistoryModifiedByUsers = new HashSet<CmsVersionHistory>();
            CmsVersionHistoryVersionDeletedByUsers = new HashSet<CmsVersionHistory>();
            CmsWorkflowHistories = new HashSet<CmsWorkflowHistory>();
            CmsWorkflowStepUsers = new HashSet<CmsWorkflowStepUser>();
            ComCustomers = new HashSet<ComCustomer>();
            ComOrderStatusUsers = new HashSet<ComOrderStatusUser>();
            ComOrders = new HashSet<ComOrder>();
            ComShoppingCarts = new HashSet<ComShoppingCart>();
            ComWishlists = new HashSet<ComWishlist>();
            ExportHistories = new HashSet<ExportHistory>();
            MediaFileFileCreatedByUsers = new HashSet<MediaFile>();
            MediaFileFileModifiedByUsers = new HashSet<MediaFile>();
            OmAccounts = new HashSet<OmAccount>();
            OmContacts = new HashSet<OmContact>();
            ReportingReportSubscriptions = new HashSet<ReportingReportSubscription>();
            ReportingSavedReports = new HashSet<ReportingSavedReport>();
            StagingTaskUsers = new HashSet<StagingTaskUser>();
            Workflows = new HashSet<CmsWorkflow>();
        }

        [Key]
        [Column("UserID")]
        public int UserId { get; set; }
        [StringLength(100)]
        public string UserName { get; set; } = null!;
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? MiddleName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        [StringLength(254)]
        public string? Email { get; set; }
        [StringLength(100)]
        public string UserPassword { get; set; } = null!;
        [StringLength(50)]
        public string? PreferredCultureCode { get; set; }
        [Column("PreferredUICultureCode")]
        [StringLength(50)]
        public string? PreferredUicultureCode { get; set; }
        public bool UserEnabled { get; set; }
        public bool? UserIsExternal { get; set; }
        [StringLength(10)]
        public string? UserPasswordFormat { get; set; }
        public DateTime? UserCreated { get; set; }
        public DateTime? LastLogon { get; set; }
        [StringLength(200)]
        public string? UserStartingAliasPath { get; set; }
        [Column("UserGUID")]
        public Guid UserGuid { get; set; }
        public DateTime UserLastModified { get; set; }
        public string? UserLastLogonInfo { get; set; }
        public bool? UserIsHidden { get; set; }
        public bool? UserIsDomain { get; set; }
        public bool? UserHasAllowedCultures { get; set; }
        [Column("UserMFRequired")]
        public bool? UserMfrequired { get; set; }
        public int UserPrivilegeLevel { get; set; }
        [StringLength(72)]
        public string? UserSecurityStamp { get; set; }
        [Column("UserMFSecret")]
        public byte[]? UserMfsecret { get; set; }
        [Column("UserMFTimestep")]
        public long? UserMftimestep { get; set; }

        [InverseProperty("UserMacroIdentityUser")]
        public virtual CmsUserMacroIdentity CmsUserMacroIdentity { get; set; } = null!;
        [InverseProperty("UserSettingsUser")]
        public virtual CmsUserSetting CmsUserSettingUserSettingsUser { get; set; } = null!;
        [InverseProperty("User")]
        public virtual StagingTaskGroupUser StagingTaskGroupUser { get; set; } = null!;
        [InverseProperty("LastModifiedByUser")]
        public virtual ICollection<CmsAclitem> CmsAclitemLastModifiedByUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsAclitem> CmsAclitemUsers { get; set; }
        [InverseProperty("HistoryApprovedByUser")]
        public virtual ICollection<CmsAutomationHistory> CmsAutomationHistories { get; set; }
        [InverseProperty("StateUser")]
        public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; }
        [InverseProperty("CategoryUser")]
        public virtual ICollection<CmsCategory> CmsCategories { get; set; }
        [InverseProperty("DocumentCheckedOutByUser")]
        public virtual ICollection<CmsDocument> CmsDocumentDocumentCheckedOutByUsers { get; set; }
        [InverseProperty("DocumentCreatedByUser")]
        public virtual ICollection<CmsDocument> CmsDocumentDocumentCreatedByUsers { get; set; }
        [InverseProperty("DocumentModifiedByUser")]
        public virtual ICollection<CmsDocument> CmsDocumentDocumentModifiedByUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsEmailUser> CmsEmailUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsExternalLogin> CmsExternalLogins { get; set; }
        [InverseProperty("MacroIdentityEffectiveUser")]
        public virtual ICollection<CmsMacroIdentity> CmsMacroIdentities { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsMembershipUser> CmsMembershipUsers { get; set; }
        [InverseProperty("ObjectCheckedOutByUser")]
        public virtual ICollection<CmsObjectSetting> CmsObjectSettings { get; set; }
        [InverseProperty("VersionDeletedByUser")]
        public virtual ICollection<CmsObjectVersionHistory> CmsObjectVersionHistoryVersionDeletedByUsers { get; set; }
        [InverseProperty("VersionModifiedByUser")]
        public virtual ICollection<CmsObjectVersionHistory> CmsObjectVersionHistoryVersionModifiedByUsers { get; set; }
        [InverseProperty("PersonalizationUser")]
        public virtual ICollection<CmsPersonalization> CmsPersonalizations { get; set; }
        [InverseProperty("TaskUser")]
        public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; }
        [InverseProperty("SubmissionSubmittedByUser")]
        public virtual ICollection<CmsTranslationSubmission> CmsTranslationSubmissions { get; set; }
        [InverseProperty("NodeOwnerNavigation")]
        public virtual ICollection<CmsTree> CmsTrees { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsUserCulture> CmsUserCultures { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; }
        [InverseProperty("UserActivatedByUser")]
        public virtual ICollection<CmsUserSetting> CmsUserSettingUserActivatedByUsers { get; set; }
        public virtual ICollection<CmsUserSetting> CmsUserSettingUserSettingsUserGus { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsUserSite> CmsUserSites { get; set; }
        [InverseProperty("ModifiedByUser")]
        public virtual ICollection<CmsVersionHistory> CmsVersionHistoryModifiedByUsers { get; set; }
        [InverseProperty("VersionDeletedByUser")]
        public virtual ICollection<CmsVersionHistory> CmsVersionHistoryVersionDeletedByUsers { get; set; }
        [InverseProperty("ApprovedByUser")]
        public virtual ICollection<CmsWorkflowHistory> CmsWorkflowHistories { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsWorkflowStepUser> CmsWorkflowStepUsers { get; set; }
        [InverseProperty("CustomerUser")]
        public virtual ICollection<ComCustomer> ComCustomers { get; set; }
        [InverseProperty("ChangedByUser")]
        public virtual ICollection<ComOrderStatusUser> ComOrderStatusUsers { get; set; }
        [InverseProperty("OrderCreatedByUser")]
        public virtual ICollection<ComOrder> ComOrders { get; set; }
        [InverseProperty("ShoppingCartUser")]
        public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<ComWishlist> ComWishlists { get; set; }
        [InverseProperty("ExportUser")]
        public virtual ICollection<ExportHistory> ExportHistories { get; set; }
        [InverseProperty("FileCreatedByUser")]
        public virtual ICollection<MediaFile> MediaFileFileCreatedByUsers { get; set; }
        [InverseProperty("FileModifiedByUser")]
        public virtual ICollection<MediaFile> MediaFileFileModifiedByUsers { get; set; }
        [InverseProperty("AccountOwnerUser")]
        public virtual ICollection<OmAccount> OmAccounts { get; set; }
        [InverseProperty("ContactOwnerUser")]
        public virtual ICollection<OmContact> OmContacts { get; set; }
        [InverseProperty("ReportSubscriptionUser")]
        public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; }
        [InverseProperty("SavedReportCreatedByUser")]
        public virtual ICollection<ReportingSavedReport> ReportingSavedReports { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<StagingTaskUser> StagingTaskUsers { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Users")]
        public virtual ICollection<CmsWorkflow> Workflows { get; set; }
    }
}
