using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_State")]
    [Index("CountryId", Name = "IX_CMS_State_CountryID")]
    [Index("StateCode", Name = "IX_CMS_State_StateCode")]
    public partial class CmsState
    {
        public CmsState()
        {
            ComAddresses = new HashSet<ComAddress>();
            ComOrderAddresses = new HashSet<ComOrderAddress>();
            ComTaxClassStates = new HashSet<ComTaxClassState>();
            CustomtableSalesTerritoryStates = new HashSet<CustomtableSalesTerritoryState>();
            OmAccounts = new HashSet<OmAccount>();
            OmContacts = new HashSet<OmContact>();
        }

        [Key]
        [Column("StateID")]
        public int StateId { get; set; }
        [StringLength(200)]
        public string StateDisplayName { get; set; } = null!;
        [StringLength(200)]
        public string StateName { get; set; } = null!;
        [StringLength(100)]
        public string? StateCode { get; set; }
        [Column("CountryID")]
        public int CountryId { get; set; }
        [Column("StateGUID")]
        public Guid StateGuid { get; set; }
        public DateTime StateLastModified { get; set; }

        [ForeignKey("CountryId")]
        [InverseProperty("CmsStates")]
        public virtual CmsCountry Country { get; set; } = null!;
        [InverseProperty("AddressState")]
        public virtual ICollection<ComAddress> ComAddresses { get; set; }
        [InverseProperty("AddressState")]
        public virtual ICollection<ComOrderAddress> ComOrderAddresses { get; set; }
        [InverseProperty("State")]
        public virtual ICollection<ComTaxClassState> ComTaxClassStates { get; set; }
        [InverseProperty("State")]
        public virtual ICollection<CustomtableSalesTerritoryState> CustomtableSalesTerritoryStates { get; set; }
        [InverseProperty("AccountState")]
        public virtual ICollection<OmAccount> OmAccounts { get; set; }
        [InverseProperty("ContactState")]
        public virtual ICollection<OmContact> OmContacts { get; set; }
    }
}
