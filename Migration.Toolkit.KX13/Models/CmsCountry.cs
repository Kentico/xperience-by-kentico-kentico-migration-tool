﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_Country")]
    public partial class CmsCountry
    {
        public CmsCountry()
        {
            CmsStates = new HashSet<CmsState>();
            ComAddresses = new HashSet<ComAddress>();
            ComOrderAddresses = new HashSet<ComOrderAddress>();
            ComTaxClassCountries = new HashSet<ComTaxClassCountry>();
            CustomtableSalesTerritoryCountries = new HashSet<CustomtableSalesTerritoryCountry>();
            OmAccounts = new HashSet<OmAccount>();
            OmContacts = new HashSet<OmContact>();
        }

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
        public virtual ICollection<CmsState> CmsStates { get; set; }
        [InverseProperty("AddressCountry")]
        public virtual ICollection<ComAddress> ComAddresses { get; set; }
        [InverseProperty("AddressCountry")]
        public virtual ICollection<ComOrderAddress> ComOrderAddresses { get; set; }
        [InverseProperty("Country")]
        public virtual ICollection<ComTaxClassCountry> ComTaxClassCountries { get; set; }
        [InverseProperty("Country")]
        public virtual ICollection<CustomtableSalesTerritoryCountry> CustomtableSalesTerritoryCountries { get; set; }
        [InverseProperty("AccountCountry")]
        public virtual ICollection<OmAccount> OmAccounts { get; set; }
        [InverseProperty("ContactCountry")]
        public virtual ICollection<OmContact> OmContacts { get; set; }
    }
}
