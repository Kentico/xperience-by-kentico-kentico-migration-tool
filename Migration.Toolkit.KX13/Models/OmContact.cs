using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

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

    [StringLength(100)]
    public string? ContactCompanyName { get; set; }

    public bool? ContactSalesForceLeadReplicationRequired { get; set; }

    [Column("ContactPersonaID")]
    public int? ContactPersonaId { get; set; }

    [InverseProperty("ConsentAgreementContact")]
    public virtual ICollection<CmsConsentAgreement> CmsConsentAgreements { get; set; } = new List<CmsConsentAgreement>();

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

    [InverseProperty("AccountPrimaryContact")]
    public virtual ICollection<OmAccount> OmAccountAccountPrimaryContacts { get; set; } = new List<OmAccount>();

    [InverseProperty("AccountSecondaryContact")]
    public virtual ICollection<OmAccount> OmAccountAccountSecondaryContacts { get; set; } = new List<OmAccount>();

    [InverseProperty("Contact")]
    public virtual ICollection<OmAccountContact> OmAccountContacts { get; set; } = new List<OmAccountContact>();

    [InverseProperty("Contact")]
    public virtual ICollection<OmMembership> OmMemberships { get; set; } = new List<OmMembership>();

    [InverseProperty("Contact")]
    public virtual ICollection<OmScoreContactRule> OmScoreContactRules { get; set; } = new List<OmScoreContactRule>();

    [InverseProperty("VisitorToContactContact")]
    public virtual ICollection<OmVisitorToContact> OmVisitorToContacts { get; set; } = new List<OmVisitorToContact>();
}
