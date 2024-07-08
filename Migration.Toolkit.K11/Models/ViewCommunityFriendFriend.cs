using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public partial class ViewCommunityFriendFriend
{
    [Column("FriendID")]
    public int FriendId { get; set; }

    [Column("FriendRequestedUserID")]
    public int FriendRequestedUserId { get; set; }

    [Column("FriendUserID")]
    public int FriendUserId { get; set; }

    public DateTime FriendRequestedWhen { get; set; }

    public string? FriendComment { get; set; }

    public int? FriendApprovedBy { get; set; }

    public DateTime? FriendApprovedWhen { get; set; }

    public int? FriendRejectedBy { get; set; }

    public DateTime? FriendRejectedWhen { get; set; }

    [Column("FriendGUID")]
    public Guid FriendGuid { get; set; }

    public int FriendStatus { get; set; }

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

    [StringLength(254)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string UserPassword { get; set; } = null!;

    [StringLength(10)]
    public string? PreferredCultureCode { get; set; }

    [Column("PreferredUICultureCode")]
    [StringLength(10)]
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

    public bool? UserHasAllowedCultures { get; set; }

    public int UserPrivilegeLevel { get; set; }

    [StringLength(72)]
    public string? UserSecurityStamp { get; set; }

    [Column("UserMFSecret")]
    public byte[]? UserMfsecret { get; set; }

    [Column("UserMFTimestep")]
    public long? UserMftimestep { get; set; }

    [StringLength(450)]
    public string? FullName { get; set; }

    public string? UserVisibility { get; set; }

    public bool? UserIsDomain { get; set; }

    [Column("UserMFRequired")]
    public bool? UserMfrequired { get; set; }

    [Column("UserSettingsID")]
    public int? UserSettingsId { get; set; }

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
    public Guid? UserSettingsUserGuid { get; set; }

    [Column("UserSettingsUserID")]
    public int? UserSettingsUserId { get; set; }

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

    [Column("AvatarID")]
    public int? AvatarId { get; set; }

    [StringLength(200)]
    public string? AvatarFileName { get; set; }

    [Column("AvatarGUID")]
    public Guid? AvatarGuid { get; set; }
}