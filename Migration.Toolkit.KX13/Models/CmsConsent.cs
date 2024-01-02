using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Consent")]
public partial class CmsConsent
{
    [Key]
    [Column("ConsentID")]
    public int ConsentId { get; set; }

    [StringLength(200)]
    public string ConsentDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string ConsentName { get; set; } = null!;

    public string ConsentContent { get; set; } = null!;

    public Guid ConsentGuid { get; set; }

    public DateTime ConsentLastModified { get; set; }

    [StringLength(100)]
    public string ConsentHash { get; set; } = null!;

    [InverseProperty("ConsentAgreementConsent")]
    public virtual ICollection<CmsConsentAgreement> CmsConsentAgreements { get; set; } = new List<CmsConsentAgreement>();

    [InverseProperty("ConsentArchiveConsent")]
    public virtual ICollection<CmsConsentArchive> CmsConsentArchives { get; set; } = new List<CmsConsentArchive>();
}
