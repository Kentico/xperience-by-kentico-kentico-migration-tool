using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Country")]
public partial class CmsCountry
{
    [Key]
    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(200)]
    public string CountryDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string CountryName { get; set; } = null!;

    [Column("CountryGUID")]
    public Guid CountryGuid { get; set; }

    public DateTime CountryLastModified { get; set; }

    [StringLength(2)]
    public string? CountryTwoLetterCode { get; set; }

    [StringLength(3)]
    public string? CountryThreeLetterCode { get; set; }

    [InverseProperty("Country")]
    public virtual ICollection<CmsState> CmsStates { get; set; } = new List<CmsState>();

    [InverseProperty("AddressCountry")]
    public virtual ICollection<ComAddress> ComAddresses { get; set; } = new List<ComAddress>();

    [InverseProperty("AddressCountry")]
    public virtual ICollection<ComOrderAddress> ComOrderAddresses { get; set; } = new List<ComOrderAddress>();

    [InverseProperty("Country")]
    public virtual ICollection<ComTaxClassCountry> ComTaxClassCountries { get; set; } = new List<ComTaxClassCountry>();

    [InverseProperty("AccountCountry")]
    public virtual ICollection<OmAccount> OmAccounts { get; set; } = new List<OmAccount>();

    [InverseProperty("ContactCountry")]
    public virtual ICollection<OmContact> OmContacts { get; set; } = new List<OmContact>();
}
