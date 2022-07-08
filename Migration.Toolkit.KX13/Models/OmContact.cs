using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("OM_Contact")]
    [Index("ContactCountryId", Name = "IX_OM_Contact_ContactCountryID")]
    [Index("ContactEmail", Name = "IX_OM_Contact_ContactEmail")]
    [Index("ContactGuid", Name = "IX_OM_Contact_ContactGUID", IsUnique = true)]
    [Index("ContactLastName", Name = "IX_OM_Contact_ContactLastName")]
    [Index("ContactOwnerUserId", Name = "IX_OM_Contact_ContactOwnerUserID")]
    [Index("ContactPersonaId", "ContactLastName", Name = "IX_OM_Contact_ContactPersonaID_ContactLastName")]
    [Index("ContactStateId", Name = "IX_OM_Contact_ContactStateID")]
    [Index("ContactStatusId", Name = "IX_OM_Contact_ContactStatusID")]
    public partial class OmContact
    {
        public OmContact()
        {
            CmsConsentAgreements = new HashSet<CmsConsentAgreement>();
            OmAccountAccountPrimaryContacts = new HashSet<OmAccount>();
            OmAccountAccountSecondaryContacts = new HashSet<OmAccount>();
            OmAccountContacts = new HashSet<OmAccountContact>();
            OmMemberships = new HashSet<OmMembership>();
            OmScoreContactRules = new HashSet<OmScoreContactRule>();
            OmVisitorToContacts = new HashSet<OmVisitorToContact>();
        }

        [Key]
        [Column("ContactID")]
        public int ContactId { get; set; }
        [StringLength(100)]
        public string? ContactFirstName { get; set; }
        [StringLength(100)]
        public string? ContactMiddleName { get; set; }
        [StringLength(100)]
        public string? ContactLastName { get; set; }
        [StringLength(50)]
        public string? ContactJobTitle { get; set; }
        [StringLength(100)]
        public string? ContactAddress1 { get; set; }
        [StringLength(100)]
        public string? ContactCity { get; set; }
        [Column("ContactZIP")]
        [StringLength(100)]
        public string? ContactZip { get; set; }
        [Column("ContactStateID")]
        public int? ContactStateId { get; set; }
        [Column("ContactCountryID")]
        public int? ContactCountryId { get; set; }
        [StringLength(26)]
        public string? ContactMobilePhone { get; set; }
        [StringLength(26)]
        public string? ContactBusinessPhone { get; set; }
        [StringLength(254)]
        public string? ContactEmail { get; set; }
        public DateTime? ContactBirthday { get; set; }
        public int? ContactGender { get; set; }
        [Column("ContactStatusID")]
        public int? ContactStatusId { get; set; }
        public string? ContactNotes { get; set; }
        [Column("ContactOwnerUserID")]
        public int? ContactOwnerUserId { get; set; }
        public bool? ContactMonitored { get; set; }
        [Column("ContactGUID")]
        public Guid ContactGuid { get; set; }
        public DateTime ContactLastModified { get; set; }
        public DateTime ContactCreated { get; set; }
        public int? ContactBounces { get; set; }
        [StringLength(200)]
        public string? ContactCampaign { get; set; }
        [Column("ContactSalesForceLeadID")]
        [StringLength(18)]
        public string? ContactSalesForceLeadId { get; set; }
        public bool? ContactSalesForceLeadReplicationDisabled { get; set; }
        public DateTime? ContactSalesForceLeadReplicationDateTime { get; set; }
        public DateTime? ContactSalesForceLeadReplicationSuspensionDateTime { get; set; }
        [StringLength(160)]
        public string? ContactCompanyName { get; set; }
        public bool? ContactSalesForceLeadReplicationRequired { get; set; }
        [Column("ContactPersonaID")]
        public int? ContactPersonaId { get; set; }
        [StringLength(450)]
        public string? FirstUserAgent { get; set; }
        [Column("FirstIPAddress")]
        [StringLength(100)]
        public string? FirstIpaddress { get; set; }
        [StringLength(2048)]
        public string? FirstRequestUrl { get; set; }
        public string? KenticoUrlReferrer { get; set; }
        [StringLength(450)]
        public string? KenticoContactRegionName { get; set; }
        [StringLength(20)]
        public string? KenticoContactRegionCode { get; set; }
        [StringLength(20)]
        public string? KenticoContactPostalCode { get; set; }
        [StringLength(200)]
        public string? KenticoContactCampaignSource { get; set; }
        [StringLength(200)]
        public string? KenticoContactCampaignContent { get; set; }
        public bool? NeedRecalculation { get; set; }
        public int? ProfileScore { get; set; }
        public int? EngagementScore { get; set; }
        public int? TotalScore { get; set; }
        public int? Zone { get; set; }
        public Guid? DynamicsLeadGuid { get; set; }
        public Guid? DynamicsContactGuid { get; set; }
        [StringLength(200)]
        public string? DynamicsAccountType { get; set; }
        [StringLength(200)]
        public string? DynamicsAccountStatus { get; set; }
        public bool? LegitimateInterest { get; set; }
        [StringLength(200)]
        public string? DynamicsActivePartnerships { get; set; }
        public DateTime? DynamicsDateOfSync { get; set; }
        public bool? PairedWithDynamicsCrm { get; set; }
        public DateTime? FirstPairDate { get; set; }
        public int? PairedBy { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? ArchivationDate { get; set; }
        public bool? HasFreeEmail { get; set; }
        public int? SameDomainContacts { get; set; }
        [Column("AreYouLookingForCMS")]
        [StringLength(10)]
        public string? AreYouLookingForCms { get; set; }
        [StringLength(50)]
        public string? Role { get; set; }
        [StringLength(200)]
        public string? KenticoContactBusinessType { get; set; }
        [StringLength(1)]
        public string? MarketingAutomationVariant { get; set; }
        [Column("KontentIntercomUserID")]
        [StringLength(50)]
        public string? KontentIntercomUserId { get; set; }
        [Column("KontentGoogleAnalyticsUserID")]
        [StringLength(50)]
        public string? KontentGoogleAnalyticsUserId { get; set; }
        [Column("KontentAmplitudeUserID")]
        [StringLength(500)]
        public string? KontentAmplitudeUserId { get; set; }
        [StringLength(200)]
        public string? PromoCode { get; set; }

        [ForeignKey("ContactCountryId")]
        [InverseProperty("OmContacts")]
        public virtual CmsCountry? ContactCountry { get; set; }
        [ForeignKey("ContactOwnerUserId")]
        [InverseProperty("OmContacts")]
        public virtual CmsUser? ContactOwnerUser { get; set; }
        [ForeignKey("ContactStateId")]
        [InverseProperty("OmContacts")]
        public virtual CmsState? ContactState { get; set; }
        [ForeignKey("ContactStatusId")]
        [InverseProperty("OmContacts")]
        public virtual OmContactStatus? ContactStatus { get; set; }
        [InverseProperty("ConsentAgreementContact")]
        public virtual ICollection<CmsConsentAgreement> CmsConsentAgreements { get; set; }
        [InverseProperty("AccountPrimaryContact")]
        public virtual ICollection<OmAccount> OmAccountAccountPrimaryContacts { get; set; }
        [InverseProperty("AccountSecondaryContact")]
        public virtual ICollection<OmAccount> OmAccountAccountSecondaryContacts { get; set; }
        [InverseProperty("Contact")]
        public virtual ICollection<OmAccountContact> OmAccountContacts { get; set; }
        [InverseProperty("Contact")]
        public virtual ICollection<OmMembership> OmMemberships { get; set; }
        [InverseProperty("Contact")]
        public virtual ICollection<OmScoreContactRule> OmScoreContactRules { get; set; }
        [InverseProperty("VisitorToContactContact")]
        public virtual ICollection<OmVisitorToContact> OmVisitorToContacts { get; set; }
    }
}
