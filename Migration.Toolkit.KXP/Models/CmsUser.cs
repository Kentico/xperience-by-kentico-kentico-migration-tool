using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_User")]
    [Index("UserGuid", Name = "IX_CMS_User_UserGUID", IsUnique = true)]
    [Index("UserName", Name = "IX_CMS_User_UserName", IsUnique = true)]
    public partial class CmsUser
    {
        public CmsUser()
        {
            CmsAutomationHistories = new HashSet<CmsAutomationHistory>();
            CmsAutomationStates = new HashSet<CmsAutomationState>();
            CmsDocumentDocumentCheckedOutByUsers = new HashSet<CmsDocument>();
            CmsDocumentDocumentCreatedByUsers = new HashSet<CmsDocument>();
            CmsDocumentDocumentModifiedByUsers = new HashSet<CmsDocument>();
            CmsExternalLogins = new HashSet<CmsExternalLogin>();
            CmsMacroIdentities = new HashSet<CmsMacroIdentity>();
            CmsPersonalizations = new HashSet<CmsPersonalization>();
            CmsScheduledTasks = new HashSet<CmsScheduledTask>();
            CmsTrees = new HashSet<CmsTree>();
            CmsUserRoles = new HashSet<CmsUserRole>();
            CmsVersionHistoryModifiedByUsers = new HashSet<CmsVersionHistory>();
            CmsVersionHistoryVersionDeletedByUsers = new HashSet<CmsVersionHistory>();
            CmsWorkflowHistories = new HashSet<CmsWorkflowHistory>();
            CmsWorkflowStepUsers = new HashSet<CmsWorkflowStepUser>();
            MediaFileFileCreatedByUsers = new HashSet<MediaFile>();
            MediaFileFileModifiedByUsers = new HashSet<MediaFile>();
            OmAccounts = new HashSet<OmAccount>();
            OmContacts = new HashSet<OmContact>();
            Workflows = new HashSet<CmsWorkflow>();
        }

        [Key]
        [Column("UserID")]
        public int UserId { get; set; }
        [StringLength(254)]
        public string UserName { get; set; } = null!;
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(254)]
        public string? Email { get; set; }
        [StringLength(100)]
        public string UserPassword { get; set; } = null!;
        public bool UserEnabled { get; set; }
        public DateTime? UserCreated { get; set; }
        public DateTime? LastLogon { get; set; }
        [Column("UserGUID")]
        public Guid UserGuid { get; set; }
        public DateTime UserLastModified { get; set; }
        [StringLength(72)]
        public string? UserSecurityStamp { get; set; }
        public DateTime? UserPasswordLastChanged { get; set; }
        public bool UserIsPendingRegistration { get; set; }
        public DateTime? UserRegistrationLinkExpiration { get; set; }
        public bool UserAdministrationAccess { get; set; }
        public bool UserIsExternal { get; set; }

        [InverseProperty("UserMacroIdentityUser")]
        public virtual CmsUserMacroIdentity CmsUserMacroIdentity { get; set; } = null!;
        [InverseProperty("HistoryApprovedByUser")]
        public virtual ICollection<CmsAutomationHistory> CmsAutomationHistories { get; set; }
        [InverseProperty("StateUser")]
        public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; }
        [InverseProperty("DocumentCheckedOutByUser")]
        public virtual ICollection<CmsDocument> CmsDocumentDocumentCheckedOutByUsers { get; set; }
        [InverseProperty("DocumentCreatedByUser")]
        public virtual ICollection<CmsDocument> CmsDocumentDocumentCreatedByUsers { get; set; }
        [InverseProperty("DocumentModifiedByUser")]
        public virtual ICollection<CmsDocument> CmsDocumentDocumentModifiedByUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsExternalLogin> CmsExternalLogins { get; set; }
        [InverseProperty("MacroIdentityEffectiveUser")]
        public virtual ICollection<CmsMacroIdentity> CmsMacroIdentities { get; set; }
        [InverseProperty("PersonalizationUser")]
        public virtual ICollection<CmsPersonalization> CmsPersonalizations { get; set; }
        [InverseProperty("TaskUser")]
        public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; }
        [InverseProperty("NodeOwnerNavigation")]
        public virtual ICollection<CmsTree> CmsTrees { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; }
        [InverseProperty("ModifiedByUser")]
        public virtual ICollection<CmsVersionHistory> CmsVersionHistoryModifiedByUsers { get; set; }
        [InverseProperty("VersionDeletedByUser")]
        public virtual ICollection<CmsVersionHistory> CmsVersionHistoryVersionDeletedByUsers { get; set; }
        [InverseProperty("ApprovedByUser")]
        public virtual ICollection<CmsWorkflowHistory> CmsWorkflowHistories { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsWorkflowStepUser> CmsWorkflowStepUsers { get; set; }
        [InverseProperty("FileCreatedByUser")]
        public virtual ICollection<MediaFile> MediaFileFileCreatedByUsers { get; set; }
        [InverseProperty("FileModifiedByUser")]
        public virtual ICollection<MediaFile> MediaFileFileModifiedByUsers { get; set; }
        [InverseProperty("AccountOwnerUser")]
        public virtual ICollection<OmAccount> OmAccounts { get; set; }
        [InverseProperty("ContactOwnerUser")]
        public virtual ICollection<OmContact> OmContacts { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Users")]
        public virtual ICollection<CmsWorkflow> Workflows { get; set; }
    }
}
