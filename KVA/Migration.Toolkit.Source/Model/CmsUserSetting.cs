// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;
public interface ICmsUserSetting : ISourceModel<ICmsUserSetting>
{
    int UserSettingsID { get; }
    string? UserNickName { get; }
    string? UserSignature { get; }
    string? UserURLReferrer { get; }
    string? UserCampaign { get; }
    string? UserCustomData { get; }
    string? UserRegistrationInfo { get; }
    DateTime? UserActivationDate { get; }
    int? UserActivatedByUserID { get; }
    int? UserTimeZoneID { get; }
    int? UserAvatarID { get; }
    int? UserGender { get; }
    DateTime? UserDateOfBirth { get; }
    Guid UserSettingsUserGUID { get; }
    int UserSettingsUserID { get; }
    bool? UserWaitingForApproval { get; }
    string? UserDialogsConfiguration { get; }
    string? UserDescription { get; }
    Guid? UserAuthenticationGUID { get; }
    string? UserSkype { get; }
    string? UserIM { get; }
    string? UserPhone { get; }
    string? UserPosition { get; }
    bool? UserLogActivities { get; }
    string? UserPasswordRequestHash { get; }
    int? UserInvalidLogOnAttempts { get; }
    string? UserInvalidLogOnAttemptsHash { get; }
    int? UserAccountLockReason { get; }
    DateTime? UserPasswordLastChanged { get; }
    bool? UserShowIntroductionTile { get; }
    string? UserDashboardApplications { get; }
    string? UserDismissedSmartTips { get; }

    static string ISourceModel<ICmsUserSetting>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsUserSettingK11.GetPrimaryKeyName(version),
        { Major: 12 } => CmsUserSettingK12.GetPrimaryKeyName(version),
        { Major: 13 } => CmsUserSettingK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static bool ISourceModel<ICmsUserSetting>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsUserSettingK11.IsAvailable(version),
        { Major: 12 } => CmsUserSettingK12.IsAvailable(version),
        { Major: 13 } => CmsUserSettingK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
    static string ISourceModel<ICmsUserSetting>.TableName => "CMS_UserSettings";
    static string ISourceModel<ICmsUserSetting>.GuidColumnName => "UserSettingsUserGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsUserSetting ISourceModel<ICmsUserSetting>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => CmsUserSettingK11.FromReader(reader, version),
        { Major: 12 } => CmsUserSettingK12.FromReader(reader, version),
        { Major: 13 } => CmsUserSettingK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}
public partial record CmsUserSettingK11(int UserSettingsID, string? UserNickName, string? UserPicture, string? UserSignature, string? UserURLReferrer, string? UserCampaign, string? UserMessagingNotificationEmail, string? UserCustomData, string? UserRegistrationInfo, string? UserPreferences, DateTime? UserActivationDate, int? UserActivatedByUserID, int? UserTimeZoneID, int? UserAvatarID, int? UserBadgeID, int? UserActivityPoints, int? UserForumPosts, int? UserBlogComments, int? UserGender, DateTime? UserDateOfBirth, int? UserMessageBoardPosts, Guid UserSettingsUserGUID, int UserSettingsUserID, string? WindowsLiveID, int? UserBlogPosts, bool? UserWaitingForApproval, string? UserDialogsConfiguration, string? UserDescription, string? UserUsedWebParts, string? UserUsedWidgets, string? UserFacebookID, Guid? UserAuthenticationGUID, string? UserSkype, string? UserIM, string? UserPhone, string? UserPosition, string? UserLinkedInID, bool? UserLogActivities, string? UserPasswordRequestHash, int? UserInvalidLogOnAttempts, string? UserInvalidLogOnAttemptsHash, string? UserAvatarType, int? UserAccountLockReason, DateTime? UserPasswordLastChanged, bool? UserShowIntroductionTile, string? UserDashboardApplications, string? UserDismissedSmartTips) : ICmsUserSetting, ISourceModel<CmsUserSettingK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserSettingsID";
    public static string TableName => "CMS_UserSettings";
    public static string GuidColumnName => "UserSettingsUserGUID";
    static CmsUserSettingK11 ISourceModel<CmsUserSettingK11>.FromReader(IDataReader reader, SemanticVersion version) => new CmsUserSettingK11(
            reader.Unbox<int>("UserSettingsID"), reader.Unbox<string?>("UserNickName"), reader.Unbox<string?>("UserPicture"), reader.Unbox<string?>("UserSignature"), reader.Unbox<string?>("UserURLReferrer"), reader.Unbox<string?>("UserCampaign"), reader.Unbox<string?>("UserMessagingNotificationEmail"), reader.Unbox<string?>("UserCustomData"), reader.Unbox<string?>("UserRegistrationInfo"), reader.Unbox<string?>("UserPreferences"), reader.Unbox<DateTime?>("UserActivationDate"), reader.Unbox<int?>("UserActivatedByUserID"), reader.Unbox<int?>("UserTimeZoneID"), reader.Unbox<int?>("UserAvatarID"), reader.Unbox<int?>("UserBadgeID"), reader.Unbox<int?>("UserActivityPoints"), reader.Unbox<int?>("UserForumPosts"), reader.Unbox<int?>("UserBlogComments"), reader.Unbox<int?>("UserGender"), reader.Unbox<DateTime?>("UserDateOfBirth"), reader.Unbox<int?>("UserMessageBoardPosts"), reader.Unbox<Guid>("UserSettingsUserGUID"), reader.Unbox<int>("UserSettingsUserID"), reader.Unbox<string?>("WindowsLiveID"), reader.Unbox<int?>("UserBlogPosts"), reader.Unbox<bool?>("UserWaitingForApproval"), reader.Unbox<string?>("UserDialogsConfiguration"), reader.Unbox<string?>("UserDescription"), reader.Unbox<string?>("UserUsedWebParts"), reader.Unbox<string?>("UserUsedWidgets"), reader.Unbox<string?>("UserFacebookID"), reader.Unbox<Guid?>("UserAuthenticationGUID"), reader.Unbox<string?>("UserSkype"), reader.Unbox<string?>("UserIM"), reader.Unbox<string?>("UserPhone"), reader.Unbox<string?>("UserPosition"), reader.Unbox<string?>("UserLinkedInID"), reader.Unbox<bool?>("UserLogActivities"), reader.Unbox<string?>("UserPasswordRequestHash"), reader.Unbox<int?>("UserInvalidLogOnAttempts"), reader.Unbox<string?>("UserInvalidLogOnAttemptsHash"), reader.Unbox<string?>("UserAvatarType"), reader.Unbox<int?>("UserAccountLockReason"), reader.Unbox<DateTime?>("UserPasswordLastChanged"), reader.Unbox<bool?>("UserShowIntroductionTile"), reader.Unbox<string?>("UserDashboardApplications"), reader.Unbox<string?>("UserDismissedSmartTips")
        );
    public static CmsUserSettingK11 FromReader(IDataReader reader, SemanticVersion version) => new CmsUserSettingK11(
            reader.Unbox<int>("UserSettingsID"), reader.Unbox<string?>("UserNickName"), reader.Unbox<string?>("UserPicture"), reader.Unbox<string?>("UserSignature"), reader.Unbox<string?>("UserURLReferrer"), reader.Unbox<string?>("UserCampaign"), reader.Unbox<string?>("UserMessagingNotificationEmail"), reader.Unbox<string?>("UserCustomData"), reader.Unbox<string?>("UserRegistrationInfo"), reader.Unbox<string?>("UserPreferences"), reader.Unbox<DateTime?>("UserActivationDate"), reader.Unbox<int?>("UserActivatedByUserID"), reader.Unbox<int?>("UserTimeZoneID"), reader.Unbox<int?>("UserAvatarID"), reader.Unbox<int?>("UserBadgeID"), reader.Unbox<int?>("UserActivityPoints"), reader.Unbox<int?>("UserForumPosts"), reader.Unbox<int?>("UserBlogComments"), reader.Unbox<int?>("UserGender"), reader.Unbox<DateTime?>("UserDateOfBirth"), reader.Unbox<int?>("UserMessageBoardPosts"), reader.Unbox<Guid>("UserSettingsUserGUID"), reader.Unbox<int>("UserSettingsUserID"), reader.Unbox<string?>("WindowsLiveID"), reader.Unbox<int?>("UserBlogPosts"), reader.Unbox<bool?>("UserWaitingForApproval"), reader.Unbox<string?>("UserDialogsConfiguration"), reader.Unbox<string?>("UserDescription"), reader.Unbox<string?>("UserUsedWebParts"), reader.Unbox<string?>("UserUsedWidgets"), reader.Unbox<string?>("UserFacebookID"), reader.Unbox<Guid?>("UserAuthenticationGUID"), reader.Unbox<string?>("UserSkype"), reader.Unbox<string?>("UserIM"), reader.Unbox<string?>("UserPhone"), reader.Unbox<string?>("UserPosition"), reader.Unbox<string?>("UserLinkedInID"), reader.Unbox<bool?>("UserLogActivities"), reader.Unbox<string?>("UserPasswordRequestHash"), reader.Unbox<int?>("UserInvalidLogOnAttempts"), reader.Unbox<string?>("UserInvalidLogOnAttemptsHash"), reader.Unbox<string?>("UserAvatarType"), reader.Unbox<int?>("UserAccountLockReason"), reader.Unbox<DateTime?>("UserPasswordLastChanged"), reader.Unbox<bool?>("UserShowIntroductionTile"), reader.Unbox<string?>("UserDashboardApplications"), reader.Unbox<string?>("UserDismissedSmartTips")
        );
};
public partial record CmsUserSettingK12(int UserSettingsID, string? UserNickName, string? UserPicture, string? UserSignature, string? UserURLReferrer, string? UserCampaign, string? UserCustomData, string? UserRegistrationInfo, string? UserPreferences, DateTime? UserActivationDate, int? UserActivatedByUserID, int? UserTimeZoneID, int? UserAvatarID, int? UserBadgeID, int? UserActivityPoints, int? UserForumPosts, int? UserBlogComments, int? UserGender, DateTime? UserDateOfBirth, int? UserMessageBoardPosts, Guid UserSettingsUserGUID, int UserSettingsUserID, string? WindowsLiveID, int? UserBlogPosts, bool? UserWaitingForApproval, string? UserDialogsConfiguration, string? UserDescription, string? UserUsedWebParts, string? UserUsedWidgets, string? UserFacebookID, Guid? UserAuthenticationGUID, string? UserSkype, string? UserIM, string? UserPhone, string? UserPosition, string? UserLinkedInID, bool? UserLogActivities, string? UserPasswordRequestHash, int? UserInvalidLogOnAttempts, string? UserInvalidLogOnAttemptsHash, string? UserAvatarType, int? UserAccountLockReason, DateTime? UserPasswordLastChanged, bool? UserShowIntroductionTile, string? UserDashboardApplications, string? UserDismissedSmartTips) : ICmsUserSetting, ISourceModel<CmsUserSettingK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserSettingsID";
    public static string TableName => "CMS_UserSettings";
    public static string GuidColumnName => "UserSettingsUserGUID";
    static CmsUserSettingK12 ISourceModel<CmsUserSettingK12>.FromReader(IDataReader reader, SemanticVersion version) => new CmsUserSettingK12(
            reader.Unbox<int>("UserSettingsID"), reader.Unbox<string?>("UserNickName"), reader.Unbox<string?>("UserPicture"), reader.Unbox<string?>("UserSignature"), reader.Unbox<string?>("UserURLReferrer"), reader.Unbox<string?>("UserCampaign"), reader.Unbox<string?>("UserCustomData"), reader.Unbox<string?>("UserRegistrationInfo"), reader.Unbox<string?>("UserPreferences"), reader.Unbox<DateTime?>("UserActivationDate"), reader.Unbox<int?>("UserActivatedByUserID"), reader.Unbox<int?>("UserTimeZoneID"), reader.Unbox<int?>("UserAvatarID"), reader.Unbox<int?>("UserBadgeID"), reader.Unbox<int?>("UserActivityPoints"), reader.Unbox<int?>("UserForumPosts"), reader.Unbox<int?>("UserBlogComments"), reader.Unbox<int?>("UserGender"), reader.Unbox<DateTime?>("UserDateOfBirth"), reader.Unbox<int?>("UserMessageBoardPosts"), reader.Unbox<Guid>("UserSettingsUserGUID"), reader.Unbox<int>("UserSettingsUserID"), reader.Unbox<string?>("WindowsLiveID"), reader.Unbox<int?>("UserBlogPosts"), reader.Unbox<bool?>("UserWaitingForApproval"), reader.Unbox<string?>("UserDialogsConfiguration"), reader.Unbox<string?>("UserDescription"), reader.Unbox<string?>("UserUsedWebParts"), reader.Unbox<string?>("UserUsedWidgets"), reader.Unbox<string?>("UserFacebookID"), reader.Unbox<Guid?>("UserAuthenticationGUID"), reader.Unbox<string?>("UserSkype"), reader.Unbox<string?>("UserIM"), reader.Unbox<string?>("UserPhone"), reader.Unbox<string?>("UserPosition"), reader.Unbox<string?>("UserLinkedInID"), reader.Unbox<bool?>("UserLogActivities"), reader.Unbox<string?>("UserPasswordRequestHash"), reader.Unbox<int?>("UserInvalidLogOnAttempts"), reader.Unbox<string?>("UserInvalidLogOnAttemptsHash"), reader.Unbox<string?>("UserAvatarType"), reader.Unbox<int?>("UserAccountLockReason"), reader.Unbox<DateTime?>("UserPasswordLastChanged"), reader.Unbox<bool?>("UserShowIntroductionTile"), reader.Unbox<string?>("UserDashboardApplications"), reader.Unbox<string?>("UserDismissedSmartTips")
        );
    public static CmsUserSettingK12 FromReader(IDataReader reader, SemanticVersion version) => new CmsUserSettingK12(
            reader.Unbox<int>("UserSettingsID"), reader.Unbox<string?>("UserNickName"), reader.Unbox<string?>("UserPicture"), reader.Unbox<string?>("UserSignature"), reader.Unbox<string?>("UserURLReferrer"), reader.Unbox<string?>("UserCampaign"), reader.Unbox<string?>("UserCustomData"), reader.Unbox<string?>("UserRegistrationInfo"), reader.Unbox<string?>("UserPreferences"), reader.Unbox<DateTime?>("UserActivationDate"), reader.Unbox<int?>("UserActivatedByUserID"), reader.Unbox<int?>("UserTimeZoneID"), reader.Unbox<int?>("UserAvatarID"), reader.Unbox<int?>("UserBadgeID"), reader.Unbox<int?>("UserActivityPoints"), reader.Unbox<int?>("UserForumPosts"), reader.Unbox<int?>("UserBlogComments"), reader.Unbox<int?>("UserGender"), reader.Unbox<DateTime?>("UserDateOfBirth"), reader.Unbox<int?>("UserMessageBoardPosts"), reader.Unbox<Guid>("UserSettingsUserGUID"), reader.Unbox<int>("UserSettingsUserID"), reader.Unbox<string?>("WindowsLiveID"), reader.Unbox<int?>("UserBlogPosts"), reader.Unbox<bool?>("UserWaitingForApproval"), reader.Unbox<string?>("UserDialogsConfiguration"), reader.Unbox<string?>("UserDescription"), reader.Unbox<string?>("UserUsedWebParts"), reader.Unbox<string?>("UserUsedWidgets"), reader.Unbox<string?>("UserFacebookID"), reader.Unbox<Guid?>("UserAuthenticationGUID"), reader.Unbox<string?>("UserSkype"), reader.Unbox<string?>("UserIM"), reader.Unbox<string?>("UserPhone"), reader.Unbox<string?>("UserPosition"), reader.Unbox<string?>("UserLinkedInID"), reader.Unbox<bool?>("UserLogActivities"), reader.Unbox<string?>("UserPasswordRequestHash"), reader.Unbox<int?>("UserInvalidLogOnAttempts"), reader.Unbox<string?>("UserInvalidLogOnAttemptsHash"), reader.Unbox<string?>("UserAvatarType"), reader.Unbox<int?>("UserAccountLockReason"), reader.Unbox<DateTime?>("UserPasswordLastChanged"), reader.Unbox<bool?>("UserShowIntroductionTile"), reader.Unbox<string?>("UserDashboardApplications"), reader.Unbox<string?>("UserDismissedSmartTips")
        );
};
public partial record CmsUserSettingK13(int UserSettingsID, string? UserNickName, string? UserSignature, string? UserURLReferrer, string? UserCampaign, string? UserCustomData, string? UserRegistrationInfo, DateTime? UserActivationDate, int? UserActivatedByUserID, int? UserTimeZoneID, int? UserAvatarID, int? UserGender, DateTime? UserDateOfBirth, Guid UserSettingsUserGUID, int UserSettingsUserID, bool? UserWaitingForApproval, string? UserDialogsConfiguration, string? UserDescription, Guid? UserAuthenticationGUID, string? UserSkype, string? UserIM, string? UserPhone, string? UserPosition, bool? UserLogActivities, string? UserPasswordRequestHash, int? UserInvalidLogOnAttempts, string? UserInvalidLogOnAttemptsHash, int? UserAccountLockReason, DateTime? UserPasswordLastChanged, bool? UserShowIntroductionTile, string? UserDashboardApplications, string? UserDismissedSmartTips, string? SuperSettingsCustomizedKey) : ICmsUserSetting, ISourceModel<CmsUserSettingK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "UserSettingsID";
    public static string TableName => "CMS_UserSettings";
    public static string GuidColumnName => "UserSettingsUserGUID";
    static CmsUserSettingK13 ISourceModel<CmsUserSettingK13>.FromReader(IDataReader reader, SemanticVersion version) => new CmsUserSettingK13(
            reader.Unbox<int>("UserSettingsID"), reader.Unbox<string?>("UserNickName"), reader.Unbox<string?>("UserSignature"), reader.Unbox<string?>("UserURLReferrer"), reader.Unbox<string?>("UserCampaign"), reader.Unbox<string?>("UserCustomData"), reader.Unbox<string?>("UserRegistrationInfo"), reader.Unbox<DateTime?>("UserActivationDate"), reader.Unbox<int?>("UserActivatedByUserID"), reader.Unbox<int?>("UserTimeZoneID"), reader.Unbox<int?>("UserAvatarID"), reader.Unbox<int?>("UserGender"), reader.Unbox<DateTime?>("UserDateOfBirth"), reader.Unbox<Guid>("UserSettingsUserGUID"), reader.Unbox<int>("UserSettingsUserID"), reader.Unbox<bool?>("UserWaitingForApproval"), reader.Unbox<string?>("UserDialogsConfiguration"), reader.Unbox<string?>("UserDescription"), reader.Unbox<Guid?>("UserAuthenticationGUID"), reader.Unbox<string?>("UserSkype"), reader.Unbox<string?>("UserIM"), reader.Unbox<string?>("UserPhone"), reader.Unbox<string?>("UserPosition"), reader.Unbox<bool?>("UserLogActivities"), reader.Unbox<string?>("UserPasswordRequestHash"), reader.Unbox<int?>("UserInvalidLogOnAttempts"), reader.Unbox<string?>("UserInvalidLogOnAttemptsHash"), reader.Unbox<int?>("UserAccountLockReason"), reader.Unbox<DateTime?>("UserPasswordLastChanged"), reader.Unbox<bool?>("UserShowIntroductionTile"), reader.Unbox<string?>("UserDashboardApplications"), reader.Unbox<string?>("UserDismissedSmartTips"), reader.Unbox<string?>("SuperSettingsCustomizedKey")
        );
    public static CmsUserSettingK13 FromReader(IDataReader reader, SemanticVersion version) => new CmsUserSettingK13(
            reader.Unbox<int>("UserSettingsID"), reader.Unbox<string?>("UserNickName"), reader.Unbox<string?>("UserSignature"), reader.Unbox<string?>("UserURLReferrer"), reader.Unbox<string?>("UserCampaign"), reader.Unbox<string?>("UserCustomData"), reader.Unbox<string?>("UserRegistrationInfo"), reader.Unbox<DateTime?>("UserActivationDate"), reader.Unbox<int?>("UserActivatedByUserID"), reader.Unbox<int?>("UserTimeZoneID"), reader.Unbox<int?>("UserAvatarID"), reader.Unbox<int?>("UserGender"), reader.Unbox<DateTime?>("UserDateOfBirth"), reader.Unbox<Guid>("UserSettingsUserGUID"), reader.Unbox<int>("UserSettingsUserID"), reader.Unbox<bool?>("UserWaitingForApproval"), reader.Unbox<string?>("UserDialogsConfiguration"), reader.Unbox<string?>("UserDescription"), reader.Unbox<Guid?>("UserAuthenticationGUID"), reader.Unbox<string?>("UserSkype"), reader.Unbox<string?>("UserIM"), reader.Unbox<string?>("UserPhone"), reader.Unbox<string?>("UserPosition"), reader.Unbox<bool?>("UserLogActivities"), reader.Unbox<string?>("UserPasswordRequestHash"), reader.Unbox<int?>("UserInvalidLogOnAttempts"), reader.Unbox<string?>("UserInvalidLogOnAttemptsHash"), reader.Unbox<int?>("UserAccountLockReason"), reader.Unbox<DateTime?>("UserPasswordLastChanged"), reader.Unbox<bool?>("UserShowIntroductionTile"), reader.Unbox<string?>("UserDashboardApplications"), reader.Unbox<string?>("UserDismissedSmartTips"), reader.Unbox<string?>("SuperSettingsCustomizedKey")
        );
};
