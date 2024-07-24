// ReSharper disable InconsistentNaming

using System.Data;

using Migration.Toolkit.Common;

namespace Migration.Toolkit.Source.Model;

public interface IOmContact : ISourceModel<IOmContact>
{
    int ContactID { get; }
    string? ContactFirstName { get; }
    string? ContactMiddleName { get; }
    string? ContactLastName { get; }
    string? ContactJobTitle { get; }
    string? ContactAddress1 { get; }
    string? ContactCity { get; }
    string? ContactZIP { get; }
    int? ContactStateID { get; }
    int? ContactCountryID { get; }
    string? ContactMobilePhone { get; }
    string? ContactBusinessPhone { get; }
    string? ContactEmail { get; }
    DateTime? ContactBirthday { get; }
    int? ContactGender { get; }
    int? ContactStatusID { get; }
    string? ContactNotes { get; }
    int? ContactOwnerUserID { get; }
    bool? ContactMonitored { get; }
    Guid ContactGUID { get; }
    DateTime ContactLastModified { get; }
    DateTime ContactCreated { get; }
    int? ContactBounces { get; }
    string? ContactCampaign { get; }
    string? ContactSalesForceLeadID { get; }
    bool? ContactSalesForceLeadReplicationDisabled { get; }
    DateTime? ContactSalesForceLeadReplicationDateTime { get; }
    DateTime? ContactSalesForceLeadReplicationSuspensionDateTime { get; }
    string? ContactCompanyName { get; }
    bool? ContactSalesForceLeadReplicationRequired { get; }
    int? ContactPersonaID { get; }

    static string ISourceModel<IOmContact>.GetPrimaryKeyName(SemanticVersion version) => version switch
    {
        { Major: 11 } => OmContactK11.GetPrimaryKeyName(version),
        { Major: 12 } => OmContactK12.GetPrimaryKeyName(version),
        { Major: 13 } => OmContactK13.GetPrimaryKeyName(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static bool ISourceModel<IOmContact>.IsAvailable(SemanticVersion version) => version switch
    {
        { Major: 11 } => OmContactK11.IsAvailable(version),
        { Major: 12 } => OmContactK12.IsAvailable(version),
        { Major: 13 } => OmContactK13.IsAvailable(version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };

    static string ISourceModel<IOmContact>.TableName => "OM_Contact";
    static string ISourceModel<IOmContact>.GuidColumnName => "ContactGUID"; //assumtion, class Guid column doesn't change between versions

    static IOmContact ISourceModel<IOmContact>.FromReader(IDataReader reader, SemanticVersion version) => version switch
    {
        { Major: 11 } => OmContactK11.FromReader(reader, version),
        { Major: 12 } => OmContactK12.FromReader(reader, version),
        { Major: 13 } => OmContactK13.FromReader(reader, version),
        _ => throw new InvalidCastException($"Invalid version {version}")
    };
}

public record OmContactK11(
    int ContactID,
    string? ContactFirstName,
    string? ContactMiddleName,
    string? ContactLastName,
    string? ContactJobTitle,
    string? ContactAddress1,
    string? ContactCity,
    string? ContactZIP,
    int? ContactStateID,
    int? ContactCountryID,
    string? ContactMobilePhone,
    string? ContactBusinessPhone,
    string? ContactEmail,
    DateTime? ContactBirthday,
    int? ContactGender,
    int? ContactStatusID,
    string? ContactNotes,
    int? ContactOwnerUserID,
    bool? ContactMonitored,
    Guid ContactGUID,
    DateTime ContactLastModified,
    DateTime ContactCreated,
    int? ContactBounces,
    string? ContactCampaign,
    string? ContactSalesForceLeadID,
    bool? ContactSalesForceLeadReplicationDisabled,
    DateTime? ContactSalesForceLeadReplicationDateTime,
    DateTime? ContactSalesForceLeadReplicationSuspensionDateTime,
    string? ContactCompanyName,
    bool? ContactSalesForceLeadReplicationRequired,
    int? ContactPersonaID) : IOmContact, ISourceModel<OmContactK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ContactID";
    public static string TableName => "OM_Contact";
    public static string GuidColumnName => "ContactGUID";

    static OmContactK11 ISourceModel<OmContactK11>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("ContactID"), reader.Unbox<string?>("ContactFirstName"), reader.Unbox<string?>("ContactMiddleName"), reader.Unbox<string?>("ContactLastName"), reader.Unbox<string?>("ContactJobTitle"),
        reader.Unbox<string?>("ContactAddress1"), reader.Unbox<string?>("ContactCity"), reader.Unbox<string?>("ContactZIP"), reader.Unbox<int?>("ContactStateID"), reader.Unbox<int?>("ContactCountryID"), reader.Unbox<string?>("ContactMobilePhone"),
        reader.Unbox<string?>("ContactBusinessPhone"), reader.Unbox<string?>("ContactEmail"), reader.Unbox<DateTime?>("ContactBirthday"), reader.Unbox<int?>("ContactGender"), reader.Unbox<int?>("ContactStatusID"),
        reader.Unbox<string?>("ContactNotes"), reader.Unbox<int?>("ContactOwnerUserID"), reader.Unbox<bool?>("ContactMonitored"), reader.Unbox<Guid>("ContactGUID"), reader.Unbox<DateTime>("ContactLastModified"),
        reader.Unbox<DateTime>("ContactCreated"), reader.Unbox<int?>("ContactBounces"), reader.Unbox<string?>("ContactCampaign"), reader.Unbox<string?>("ContactSalesForceLeadID"), reader.Unbox<bool?>("ContactSalesForceLeadReplicationDisabled"),
        reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationDateTime"), reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationSuspensionDateTime"), reader.Unbox<string?>("ContactCompanyName"),
        reader.Unbox<bool?>("ContactSalesForceLeadReplicationRequired"), reader.Unbox<int?>("ContactPersonaID")
    );

    public static OmContactK11 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("ContactID"), reader.Unbox<string?>("ContactFirstName"), reader.Unbox<string?>("ContactMiddleName"), reader.Unbox<string?>("ContactLastName"), reader.Unbox<string?>("ContactJobTitle"),
        reader.Unbox<string?>("ContactAddress1"), reader.Unbox<string?>("ContactCity"), reader.Unbox<string?>("ContactZIP"), reader.Unbox<int?>("ContactStateID"), reader.Unbox<int?>("ContactCountryID"), reader.Unbox<string?>("ContactMobilePhone"),
        reader.Unbox<string?>("ContactBusinessPhone"), reader.Unbox<string?>("ContactEmail"), reader.Unbox<DateTime?>("ContactBirthday"), reader.Unbox<int?>("ContactGender"), reader.Unbox<int?>("ContactStatusID"),
        reader.Unbox<string?>("ContactNotes"), reader.Unbox<int?>("ContactOwnerUserID"), reader.Unbox<bool?>("ContactMonitored"), reader.Unbox<Guid>("ContactGUID"), reader.Unbox<DateTime>("ContactLastModified"),
        reader.Unbox<DateTime>("ContactCreated"), reader.Unbox<int?>("ContactBounces"), reader.Unbox<string?>("ContactCampaign"), reader.Unbox<string?>("ContactSalesForceLeadID"), reader.Unbox<bool?>("ContactSalesForceLeadReplicationDisabled"),
        reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationDateTime"), reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationSuspensionDateTime"), reader.Unbox<string?>("ContactCompanyName"),
        reader.Unbox<bool?>("ContactSalesForceLeadReplicationRequired"), reader.Unbox<int?>("ContactPersonaID")
    );
}

public record OmContactK12(
    int ContactID,
    string? ContactFirstName,
    string? ContactMiddleName,
    string? ContactLastName,
    string? ContactJobTitle,
    string? ContactAddress1,
    string? ContactCity,
    string? ContactZIP,
    int? ContactStateID,
    int? ContactCountryID,
    string? ContactMobilePhone,
    string? ContactBusinessPhone,
    string? ContactEmail,
    DateTime? ContactBirthday,
    int? ContactGender,
    int? ContactStatusID,
    string? ContactNotes,
    int? ContactOwnerUserID,
    bool? ContactMonitored,
    Guid ContactGUID,
    DateTime ContactLastModified,
    DateTime ContactCreated,
    int? ContactBounces,
    string? ContactCampaign,
    string? ContactSalesForceLeadID,
    bool? ContactSalesForceLeadReplicationDisabled,
    DateTime? ContactSalesForceLeadReplicationDateTime,
    DateTime? ContactSalesForceLeadReplicationSuspensionDateTime,
    string? ContactCompanyName,
    bool? ContactSalesForceLeadReplicationRequired,
    int? ContactPersonaID,
    string? FirstUserAgent,
    string? FirstIPAddress,
    string? FirstRequestUrl,
    string? KenticoUrlReferrer,
    string? KenticoContactRegionName,
    string? KenticoContactRegionCode,
    string? KenticoContactPostalCode,
    string? KenticoContactCampaignSource,
    string? KenticoContactCampaignContent,
    bool? NeedRecalculation,
    int? ProfileScore,
    int? EngagementScore,
    int? TotalScore,
    int? Zone,
    Guid? DynamicsLeadGuid,
    Guid? DynamicsContactGuid,
    string? DynamicsAccountType,
    string? DynamicsAccountStatus,
    bool? LegitimateInterest,
    string? DynamicsActivePartnerships,
    DateTime? DynamicsDateOfSync,
    bool? PairedWithDynamicsCrm,
    DateTime? FirstPairDate,
    int? PairedBy,
    bool? IsArchived,
    DateTime? ArchivationDate,
    bool? HasFreeEmail,
    int? SameDomainContacts,
    string? AreYouLookingForCMS,
    string? Role,
    string? KenticoContactBusinessType,
    string? MarketingAutomationVariant,
    string? KontentIntercomUserID,
    string? KontentGoogleAnalyticsUserID,
    string? KontentAmplitudeUserID) : IOmContact, ISourceModel<OmContactK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ContactID";
    public static string TableName => "OM_Contact";
    public static string GuidColumnName => "ContactGUID";

    static OmContactK12 ISourceModel<OmContactK12>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("ContactID"), reader.Unbox<string?>("ContactFirstName"), reader.Unbox<string?>("ContactMiddleName"), reader.Unbox<string?>("ContactLastName"), reader.Unbox<string?>("ContactJobTitle"),
        reader.Unbox<string?>("ContactAddress1"), reader.Unbox<string?>("ContactCity"), reader.Unbox<string?>("ContactZIP"), reader.Unbox<int?>("ContactStateID"), reader.Unbox<int?>("ContactCountryID"), reader.Unbox<string?>("ContactMobilePhone"),
        reader.Unbox<string?>("ContactBusinessPhone"), reader.Unbox<string?>("ContactEmail"), reader.Unbox<DateTime?>("ContactBirthday"), reader.Unbox<int?>("ContactGender"), reader.Unbox<int?>("ContactStatusID"),
        reader.Unbox<string?>("ContactNotes"), reader.Unbox<int?>("ContactOwnerUserID"), reader.Unbox<bool?>("ContactMonitored"), reader.Unbox<Guid>("ContactGUID"), reader.Unbox<DateTime>("ContactLastModified"),
        reader.Unbox<DateTime>("ContactCreated"), reader.Unbox<int?>("ContactBounces"), reader.Unbox<string?>("ContactCampaign"), reader.Unbox<string?>("ContactSalesForceLeadID"), reader.Unbox<bool?>("ContactSalesForceLeadReplicationDisabled"),
        reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationDateTime"), reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationSuspensionDateTime"), reader.Unbox<string?>("ContactCompanyName"),
        reader.Unbox<bool?>("ContactSalesForceLeadReplicationRequired"), reader.Unbox<int?>("ContactPersonaID"), reader.Unbox<string?>("FirstUserAgent"), reader.Unbox<string?>("FirstIPAddress"), reader.Unbox<string?>("FirstRequestUrl"),
        reader.Unbox<string?>("KenticoUrlReferrer"), reader.Unbox<string?>("KenticoContactRegionName"), reader.Unbox<string?>("KenticoContactRegionCode"), reader.Unbox<string?>("KenticoContactPostalCode"),
        reader.Unbox<string?>("KenticoContactCampaignSource"), reader.Unbox<string?>("KenticoContactCampaignContent"), reader.Unbox<bool?>("NeedRecalculation"), reader.Unbox<int?>("ProfileScore"), reader.Unbox<int?>("EngagementScore"),
        reader.Unbox<int?>("TotalScore"), reader.Unbox<int?>("Zone"), reader.Unbox<Guid?>("DynamicsLeadGuid"), reader.Unbox<Guid?>("DynamicsContactGuid"), reader.Unbox<string?>("DynamicsAccountType"), reader.Unbox<string?>("DynamicsAccountStatus"),
        reader.Unbox<bool?>("LegitimateInterest"), reader.Unbox<string?>("DynamicsActivePartnerships"), reader.Unbox<DateTime?>("DynamicsDateOfSync"), reader.Unbox<bool?>("PairedWithDynamicsCrm"), reader.Unbox<DateTime?>("FirstPairDate"),
        reader.Unbox<int?>("PairedBy"), reader.Unbox<bool?>("IsArchived"), reader.Unbox<DateTime?>("ArchivationDate"), reader.Unbox<bool?>("HasFreeEmail"), reader.Unbox<int?>("SameDomainContacts"), reader.Unbox<string?>("AreYouLookingForCMS"),
        reader.Unbox<string?>("Role"), reader.Unbox<string?>("KenticoContactBusinessType"), reader.Unbox<string?>("MarketingAutomationVariant"), reader.Unbox<string?>("KontentIntercomUserID"), reader.Unbox<string?>("KontentGoogleAnalyticsUserID"),
        reader.Unbox<string?>("KontentAmplitudeUserID")
    );

    public static OmContactK12 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("ContactID"), reader.Unbox<string?>("ContactFirstName"), reader.Unbox<string?>("ContactMiddleName"), reader.Unbox<string?>("ContactLastName"), reader.Unbox<string?>("ContactJobTitle"),
        reader.Unbox<string?>("ContactAddress1"), reader.Unbox<string?>("ContactCity"), reader.Unbox<string?>("ContactZIP"), reader.Unbox<int?>("ContactStateID"), reader.Unbox<int?>("ContactCountryID"), reader.Unbox<string?>("ContactMobilePhone"),
        reader.Unbox<string?>("ContactBusinessPhone"), reader.Unbox<string?>("ContactEmail"), reader.Unbox<DateTime?>("ContactBirthday"), reader.Unbox<int?>("ContactGender"), reader.Unbox<int?>("ContactStatusID"),
        reader.Unbox<string?>("ContactNotes"), reader.Unbox<int?>("ContactOwnerUserID"), reader.Unbox<bool?>("ContactMonitored"), reader.Unbox<Guid>("ContactGUID"), reader.Unbox<DateTime>("ContactLastModified"),
        reader.Unbox<DateTime>("ContactCreated"), reader.Unbox<int?>("ContactBounces"), reader.Unbox<string?>("ContactCampaign"), reader.Unbox<string?>("ContactSalesForceLeadID"), reader.Unbox<bool?>("ContactSalesForceLeadReplicationDisabled"),
        reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationDateTime"), reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationSuspensionDateTime"), reader.Unbox<string?>("ContactCompanyName"),
        reader.Unbox<bool?>("ContactSalesForceLeadReplicationRequired"), reader.Unbox<int?>("ContactPersonaID"), reader.Unbox<string?>("FirstUserAgent"), reader.Unbox<string?>("FirstIPAddress"), reader.Unbox<string?>("FirstRequestUrl"),
        reader.Unbox<string?>("KenticoUrlReferrer"), reader.Unbox<string?>("KenticoContactRegionName"), reader.Unbox<string?>("KenticoContactRegionCode"), reader.Unbox<string?>("KenticoContactPostalCode"),
        reader.Unbox<string?>("KenticoContactCampaignSource"), reader.Unbox<string?>("KenticoContactCampaignContent"), reader.Unbox<bool?>("NeedRecalculation"), reader.Unbox<int?>("ProfileScore"), reader.Unbox<int?>("EngagementScore"),
        reader.Unbox<int?>("TotalScore"), reader.Unbox<int?>("Zone"), reader.Unbox<Guid?>("DynamicsLeadGuid"), reader.Unbox<Guid?>("DynamicsContactGuid"), reader.Unbox<string?>("DynamicsAccountType"), reader.Unbox<string?>("DynamicsAccountStatus"),
        reader.Unbox<bool?>("LegitimateInterest"), reader.Unbox<string?>("DynamicsActivePartnerships"), reader.Unbox<DateTime?>("DynamicsDateOfSync"), reader.Unbox<bool?>("PairedWithDynamicsCrm"), reader.Unbox<DateTime?>("FirstPairDate"),
        reader.Unbox<int?>("PairedBy"), reader.Unbox<bool?>("IsArchived"), reader.Unbox<DateTime?>("ArchivationDate"), reader.Unbox<bool?>("HasFreeEmail"), reader.Unbox<int?>("SameDomainContacts"), reader.Unbox<string?>("AreYouLookingForCMS"),
        reader.Unbox<string?>("Role"), reader.Unbox<string?>("KenticoContactBusinessType"), reader.Unbox<string?>("MarketingAutomationVariant"), reader.Unbox<string?>("KontentIntercomUserID"), reader.Unbox<string?>("KontentGoogleAnalyticsUserID"),
        reader.Unbox<string?>("KontentAmplitudeUserID")
    );
}

public record OmContactK13(
    int ContactID,
    string? ContactFirstName,
    string? ContactMiddleName,
    string? ContactLastName,
    string? ContactJobTitle,
    string? ContactAddress1,
    string? ContactCity,
    string? ContactZIP,
    int? ContactStateID,
    int? ContactCountryID,
    string? ContactMobilePhone,
    string? ContactBusinessPhone,
    string? ContactEmail,
    DateTime? ContactBirthday,
    int? ContactGender,
    int? ContactStatusID,
    string? ContactNotes,
    int? ContactOwnerUserID,
    bool? ContactMonitored,
    Guid ContactGUID,
    DateTime ContactLastModified,
    DateTime ContactCreated,
    int? ContactBounces,
    string? ContactCampaign,
    string? ContactSalesForceLeadID,
    bool? ContactSalesForceLeadReplicationDisabled,
    DateTime? ContactSalesForceLeadReplicationDateTime,
    DateTime? ContactSalesForceLeadReplicationSuspensionDateTime,
    string? ContactCompanyName,
    bool? ContactSalesForceLeadReplicationRequired,
    int? ContactPersonaID) : IOmContact, ISourceModel<OmContactK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "ContactID";
    public static string TableName => "OM_Contact";
    public static string GuidColumnName => "ContactGUID";

    static OmContactK13 ISourceModel<OmContactK13>.FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("ContactID"), reader.Unbox<string?>("ContactFirstName"), reader.Unbox<string?>("ContactMiddleName"), reader.Unbox<string?>("ContactLastName"), reader.Unbox<string?>("ContactJobTitle"),
        reader.Unbox<string?>("ContactAddress1"), reader.Unbox<string?>("ContactCity"), reader.Unbox<string?>("ContactZIP"), reader.Unbox<int?>("ContactStateID"), reader.Unbox<int?>("ContactCountryID"), reader.Unbox<string?>("ContactMobilePhone"),
        reader.Unbox<string?>("ContactBusinessPhone"), reader.Unbox<string?>("ContactEmail"), reader.Unbox<DateTime?>("ContactBirthday"), reader.Unbox<int?>("ContactGender"), reader.Unbox<int?>("ContactStatusID"),
        reader.Unbox<string?>("ContactNotes"), reader.Unbox<int?>("ContactOwnerUserID"), reader.Unbox<bool?>("ContactMonitored"), reader.Unbox<Guid>("ContactGUID"), reader.Unbox<DateTime>("ContactLastModified"),
        reader.Unbox<DateTime>("ContactCreated"), reader.Unbox<int?>("ContactBounces"), reader.Unbox<string?>("ContactCampaign"), reader.Unbox<string?>("ContactSalesForceLeadID"), reader.Unbox<bool?>("ContactSalesForceLeadReplicationDisabled"),
        reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationDateTime"), reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationSuspensionDateTime"), reader.Unbox<string?>("ContactCompanyName"),
        reader.Unbox<bool?>("ContactSalesForceLeadReplicationRequired"), reader.Unbox<int?>("ContactPersonaID")
    );

    public static OmContactK13 FromReader(IDataReader reader, SemanticVersion version) => new(
        reader.Unbox<int>("ContactID"), reader.Unbox<string?>("ContactFirstName"), reader.Unbox<string?>("ContactMiddleName"), reader.Unbox<string?>("ContactLastName"), reader.Unbox<string?>("ContactJobTitle"),
        reader.Unbox<string?>("ContactAddress1"), reader.Unbox<string?>("ContactCity"), reader.Unbox<string?>("ContactZIP"), reader.Unbox<int?>("ContactStateID"), reader.Unbox<int?>("ContactCountryID"), reader.Unbox<string?>("ContactMobilePhone"),
        reader.Unbox<string?>("ContactBusinessPhone"), reader.Unbox<string?>("ContactEmail"), reader.Unbox<DateTime?>("ContactBirthday"), reader.Unbox<int?>("ContactGender"), reader.Unbox<int?>("ContactStatusID"),
        reader.Unbox<string?>("ContactNotes"), reader.Unbox<int?>("ContactOwnerUserID"), reader.Unbox<bool?>("ContactMonitored"), reader.Unbox<Guid>("ContactGUID"), reader.Unbox<DateTime>("ContactLastModified"),
        reader.Unbox<DateTime>("ContactCreated"), reader.Unbox<int?>("ContactBounces"), reader.Unbox<string?>("ContactCampaign"), reader.Unbox<string?>("ContactSalesForceLeadID"), reader.Unbox<bool?>("ContactSalesForceLeadReplicationDisabled"),
        reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationDateTime"), reader.Unbox<DateTime?>("ContactSalesForceLeadReplicationSuspensionDateTime"), reader.Unbox<string?>("ContactCompanyName"),
        reader.Unbox<bool?>("ContactSalesForceLeadReplicationRequired"), reader.Unbox<int?>("ContactPersonaID")
    );
}
