using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_UserSettings")]
[Index("UserActivatedByUserId", Name = "IX_CMS_UserSettings_UserActivatedByUserID")]
[Index("UserAuthenticationGuid", Name = "IX_CMS_UserSettings_UserAuthenticationGUID")]
[Index("UserAvatarId", Name = "IX_CMS_UserSettings_UserAvatarID")]
[Index("UserBadgeId", Name = "IX_CMS_UserSettings_UserBadgeID")]
[Index("UserFacebookId", Name = "IX_CMS_UserSettings_UserFacebookID")]
[Index("UserGender", Name = "IX_CMS_UserSettings_UserGender")]
[Index("UserNickName", Name = "IX_CMS_UserSettings_UserNickName")]
[Index("UserPasswordRequestHash", Name = "IX_CMS_UserSettings_UserPasswordRequestHash")]
[Index("UserSettingsUserGuid", Name = "IX_CMS_UserSettings_UserSettingsUserGUID")]
[Index("UserSettingsUserId", Name = "IX_CMS_UserSettings_UserSettingsUserID", IsUnique = true)]
[Index("UserTimeZoneId", Name = "IX_CMS_UserSettings_UserTimeZoneID")]
[Index("UserWaitingForApproval", Name = "IX_CMS_UserSettings_UserWaitingForApproval")]
[Index("WindowsLiveId", Name = "IX_CMS_UserSettings_WindowsLiveID")]
public class CmsUserSetting
{
    [Key]
    [Column("UserSettingsID")]
    public int UserSettingsId { get; set; }

    [StringLength(200)]
    public string? UserNickName { get; set; }

    [StringLength(200)]
    public string? UserPicture { get; set; }

    public string? UserSignature { get; set; }

    [Column("UserURLReferrer")]
    [StringLength(450)]
    public string? UserUrlreferrer { get; set; }

    [StringLength(200)]
    public string? UserCampaign { get; set; }

    [StringLength(200)]
    public string? UserMessagingNotificationEmail { get; set; }

    public string? UserCustomData { get; set; }

    public string? UserRegistrationInfo { get; set; }

    public string? UserPreferences { get; set; }

    public DateTime? UserActivationDate { get; set; }

    [Column("UserActivatedByUserID")]
    public int? UserActivatedByUserId { get; set; }

    [Column("UserTimeZoneID")]
    public int? UserTimeZoneId { get; set; }

    [Column("UserAvatarID")]
    public int? UserAvatarId { get; set; }

    [Column("UserBadgeID")]
    public int? UserBadgeId { get; set; }

    public int? UserActivityPoints { get; set; }

    public int? UserForumPosts { get; set; }

    public int? UserBlogComments { get; set; }

    public int? UserGender { get; set; }

    public DateTime? UserDateOfBirth { get; set; }

    public int? UserMessageBoardPosts { get; set; }

    [Column("UserSettingsUserGUID")]
    public Guid UserSettingsUserGuid { get; set; }

    [Column("UserSettingsUserID")]
    public int UserSettingsUserId { get; set; }

    [Column("WindowsLiveID")]
    [StringLength(50)]
    public string? WindowsLiveId { get; set; }

    public int? UserBlogPosts { get; set; }

    public bool? UserWaitingForApproval { get; set; }

    public string? UserDialogsConfiguration { get; set; }

    public string? UserDescription { get; set; }

    [StringLength(1000)]
    public string? UserUsedWebParts { get; set; }

    [StringLength(1000)]
    public string? UserUsedWidgets { get; set; }

    [Column("UserFacebookID")]
    [StringLength(100)]
    public string? UserFacebookId { get; set; }

    [Column("UserAuthenticationGUID")]
    public Guid? UserAuthenticationGuid { get; set; }

    [StringLength(100)]
    public string? UserSkype { get; set; }

    [Column("UserIM")]
    [StringLength(100)]
    public string? UserIm { get; set; }

    [StringLength(26)]
    public string? UserPhone { get; set; }

    [StringLength(200)]
    public string? UserPosition { get; set; }

    [Column("UserLinkedInID")]
    [StringLength(100)]
    public string? UserLinkedInId { get; set; }

    public bool? UserLogActivities { get; set; }

    [StringLength(100)]
    public string? UserPasswordRequestHash { get; set; }

    public int? UserInvalidLogOnAttempts { get; set; }

    [StringLength(100)]
    public string? UserInvalidLogOnAttemptsHash { get; set; }

    [StringLength(200)]
    public string? UserAvatarType { get; set; }

    public int? UserAccountLockReason { get; set; }

    public DateTime? UserPasswordLastChanged { get; set; }

    public bool? UserShowIntroductionTile { get; set; }

    public string? UserDashboardApplications { get; set; }

    public string? UserDismissedSmartTips { get; set; }

    [ForeignKey("UserActivatedByUserId")]
    [InverseProperty("CmsUserSettingUserActivatedByUsers")]
    public virtual CmsUser? UserActivatedByUser { get; set; }

    [ForeignKey("UserAvatarId")]
    [InverseProperty("CmsUserSettings")]
    public virtual CmsAvatar? UserAvatar { get; set; }

    [ForeignKey("UserBadgeId")]
    [InverseProperty("CmsUserSettings")]
    public virtual CmsBadge? UserBadge { get; set; }

    public virtual CmsUser UserSettingsUser { get; set; } = null!;

    [ForeignKey("UserSettingsUserId")]
    [InverseProperty("CmsUserSettingUserSettingsUserNavigation")]
    public virtual CmsUser UserSettingsUserNavigation { get; set; } = null!;

    [ForeignKey("UserTimeZoneId")]
    [InverseProperty("CmsUserSettings")]
    public virtual CmsTimeZone? UserTimeZone { get; set; }
}
