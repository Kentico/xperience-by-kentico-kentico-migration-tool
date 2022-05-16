using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_SalesTerritoryCountry")]
    public partial class CustomtableSalesTerritoryCountry
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        public int? ItemCreatedBy { get; set; }
        public DateTime? ItemCreatedWhen { get; set; }
        public int? ItemModifiedBy { get; set; }
        public DateTime? ItemModifiedWhen { get; set; }
        public int? ItemOrder { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [Column("SalesTerritoryID")]
        public int SalesTerritoryId { get; set; }
        [Column("CountryID")]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        [InverseProperty("CustomtableSalesTerritoryCountries")]
        public virtual CmsCountry Country { get; set; } = null!;
        [ForeignKey("SalesTerritoryId")]
        [InverseProperty("CustomtableSalesTerritoryCountries")]
        public virtual CustomtableSalesTerritory SalesTerritory { get; set; } = null!;
    }
}
