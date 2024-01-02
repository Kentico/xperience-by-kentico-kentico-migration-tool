using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_UserSettings")]
[Index("UserActivatedByUserId", Name = "IX_CMS_UserSettings_UserActivatedByUserID")]
[Index("UserAuthenticationGuid", Name = "IX_CMS_UserSettings_UserAuthenticationGUID")]
[Index("UserAvatarId", Name = "IX_CMS_UserSettings_UserAvatarID")]
[Index("UserGender", Name = "IX_CMS_UserSettings_UserGender")]
[Index("UserNickName", Name = "IX_CMS_UserSettings_UserNickName")]
[Index("UserPasswordRequestHash", Name = "IX_CMS_UserSettings_UserPasswordRequestHash")]
[Index("UserSettingsUserGuid", Name = "IX_CMS_UserSettings_UserSettingsUserGUID")]
[Index("UserSettingsUserId", Name = "IX_CMS_UserSettings_UserSettingsUserID", IsUnique = true)]
[Index("UserTimeZoneId", Name = "IX_CMS_UserSettings_UserTimeZoneID")]
[Index("UserWaitingForApproval", Name = "IX_CMS_UserSettings_UserWaitingForApproval")]
public partial class CmsUserSetting
{
    [Key]
    [Column("UserSettingsID")]
    public int UserSettingsId { get; set; }

    [StringLength(200)]
    public string? UserNickName { get; set; }

    public string? UserSignature { get; set; }

    [Column("UserURLReferrer")]
    [StringLength(450)]
    public string? UserUrlreferrer { get; set; }

    [StringLength(200)]
    public string? UserCampaign { get; set; }

    public string? UserCustomData { get; set; }

    public string? UserRegistrationInfo { get; set; }

    public DateTime? UserActivationDate { get; set; }

    [Column("UserActivatedByUserID")]
    public int? UserActivatedByUserId { get; set; }

    [Column("UserTimeZoneID")]
    public int? UserTimeZoneId { get; set; }

    [Column("UserAvatarID")]
    public int? UserAvatarId { get; set; }

    public int? UserGender { get; set; }

    public DateTime? UserDateOfBirth { get; set; }

    [Column("UserSettingsUserGUID")]
    public Guid UserSettingsUserGuid { get; set; }

    [Column("UserSettingsUserID")]
    public int UserSettingsUserId { get; set; }

    public bool? UserWaitingForApproval { get; set; }

    public string? UserDialogsConfiguration { get; set; }

    public string? UserDescription { get; set; }

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

    public bool? UserLogActivities { get; set; }

    [StringLength(100)]
    public string? UserPasswordRequestHash { get; set; }

    public int? UserInvalidLogOnAttempts { get; set; }

    [StringLength(100)]
    public string? UserInvalidLogOnAttemptsHash { get; set; }

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

    public virtual CmsUser UserSettingsUser { get; set; } = null!;

    [ForeignKey("UserSettingsUserId")]
    [InverseProperty("CmsUserSettingUserSettingsUserNavigation")]
    public virtual CmsUser UserSettingsUserNavigation { get; set; } = null!;

    [ForeignKey("UserTimeZoneId")]
    [InverseProperty("CmsUserSettings")]
    public virtual CmsTimeZone? UserTimeZone { get; set; }
}
