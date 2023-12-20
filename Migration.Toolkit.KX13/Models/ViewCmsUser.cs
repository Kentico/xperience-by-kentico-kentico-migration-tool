using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewCmsUser
{
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

    [StringLength(450)]
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

    [Column("UserSettingsID")]
    public int? UserSettingsId { get; set; }

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
    public Guid? UserSettingsUserGuid { get; set; }

    [Column("UserSettingsUserID")]
    public int? UserSettingsUserId { get; set; }

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

    [Column("AvatarID")]
    public int? AvatarId { get; set; }

    [StringLength(200)]
    public string? AvatarFileName { get; set; }

    [Column("AvatarGUID")]
    public Guid? AvatarGuid { get; set; }
}
