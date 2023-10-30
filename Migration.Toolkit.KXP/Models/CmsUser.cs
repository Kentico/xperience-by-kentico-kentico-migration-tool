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
            CmsContentItemLanguageMetadatumContentItemLanguageMetadataCreatedByUsers = new HashSet<CmsContentItemLanguageMetadatum>();
            CmsContentItemLanguageMetadatumContentItemLanguageMetadataModifiedByUsers = new HashSet<CmsContentItemLanguageMetadatum>();
            CmsExternalLogins = new HashSet<CmsExternalLogin>();
            CmsMacroIdentities = new HashSet<CmsMacroIdentity>();
            CmsScheduledTasks = new HashSet<CmsScheduledTask>();
            CmsUserRoles = new HashSet<CmsUserRole>();
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
        [InverseProperty("ContentItemLanguageMetadataCreatedByUser")]
        public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadatumContentItemLanguageMetadataCreatedByUsers { get; set; }
        [InverseProperty("ContentItemLanguageMetadataModifiedByUser")]
        public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadatumContentItemLanguageMetadataModifiedByUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsExternalLogin> CmsExternalLogins { get; set; }
        [InverseProperty("MacroIdentityEffectiveUser")]
        public virtual ICollection<CmsMacroIdentity> CmsMacroIdentities { get; set; }
        [InverseProperty("TaskUser")]
        public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; }
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
