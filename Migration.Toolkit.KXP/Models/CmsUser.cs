using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_User")]
[Index("UserGuid", Name = "IX_CMS_User_UserGUID", IsUnique = true)]
[Index("UserName", Name = "IX_CMS_User_UserName", IsUnique = true)]
public partial class CmsUser
{
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

    [InverseProperty("HistoryApprovedByUser")]
    public virtual ICollection<CmsAutomationHistory> CmsAutomationHistories { get; set; } = new List<CmsAutomationHistory>();

    [InverseProperty("StateUser")]
    public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; } = new List<CmsAutomationState>();

    [InverseProperty("ContentFolderCreatedByUser")]
    public virtual ICollection<CmsContentFolder> CmsContentFolderContentFolderCreatedByUsers { get; set; } = new List<CmsContentFolder>();

    [InverseProperty("ContentFolderModifiedByUser")]
    public virtual ICollection<CmsContentFolder> CmsContentFolderContentFolderModifiedByUsers { get; set; } = new List<CmsContentFolder>();

    [InverseProperty("ContentItemLanguageMetadataCreatedByUser")]
    public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadatumContentItemLanguageMetadataCreatedByUsers { get; set; } = new List<CmsContentItemLanguageMetadatum>();

    [InverseProperty("ContentItemLanguageMetadataModifiedByUser")]
    public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadatumContentItemLanguageMetadataModifiedByUsers { get; set; } = new List<CmsContentItemLanguageMetadatum>();

    [InverseProperty("User")]
    public virtual ICollection<CmsExternalLogin> CmsExternalLogins { get; set; } = new List<CmsExternalLogin>();

    [InverseProperty("HeadlessTokenCreatedByUser")]
    public virtual ICollection<CmsHeadlessToken> CmsHeadlessTokenHeadlessTokenCreatedByUsers { get; set; } = new List<CmsHeadlessToken>();

    [InverseProperty("HeadlessTokenModifiedByUser")]
    public virtual ICollection<CmsHeadlessToken> CmsHeadlessTokenHeadlessTokenModifiedByUsers { get; set; } = new List<CmsHeadlessToken>();

    [InverseProperty("MacroIdentityEffectiveUser")]
    public virtual ICollection<CmsMacroIdentity> CmsMacroIdentities { get; set; } = new List<CmsMacroIdentity>();

    [InverseProperty("TaskUser")]
    public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; } = new List<CmsScheduledTask>();

    [InverseProperty("SmartFolderCreatedByUser")]
    public virtual ICollection<CmsSmartFolder> CmsSmartFolderSmartFolderCreatedByUsers { get; set; } = new List<CmsSmartFolder>();

    [InverseProperty("SmartFolderModifiedByUser")]
    public virtual ICollection<CmsSmartFolder> CmsSmartFolderSmartFolderModifiedByUsers { get; set; } = new List<CmsSmartFolder>();

    [InverseProperty("UserMacroIdentityUser")]
    public virtual CmsUserMacroIdentity? CmsUserMacroIdentity { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<CmsUserRole> CmsUserRoles { get; set; } = new List<CmsUserRole>();

    [InverseProperty("FileCreatedByUser")]
    public virtual ICollection<MediaFile> MediaFileFileCreatedByUsers { get; set; } = new List<MediaFile>();

    [InverseProperty("FileModifiedByUser")]
    public virtual ICollection<MediaFile> MediaFileFileModifiedByUsers { get; set; } = new List<MediaFile>();

    [InverseProperty("AccountOwnerUser")]
    public virtual ICollection<OmAccount> OmAccounts { get; set; } = new List<OmAccount>();

    [InverseProperty("ContactOwnerUser")]
    public virtual ICollection<OmContact> OmContacts { get; set; } = new List<OmContact>();
}
