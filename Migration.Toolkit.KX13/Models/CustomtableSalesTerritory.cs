using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_SalesTerritory")]
    public partial class CustomtableSalesTerritory
    {
        public CustomtableSalesTerritory()
        {
            CustomtableSalesTerritoryCountries = new HashSet<CustomtableSalesTerritoryCountry>();
            CustomtableSalesTerritoryStates = new HashSet<CustomtableSalesTerritoryState>();
        }

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
        [StringLength(200)]
        public string SalesTerritoryDisplayName { get; set; } = null!;
        [StringLength(200)]
        public string SalesTerritoryCodeName { get; set; } = null!;
        public bool IsPrimaryTerritory { get; set; }
        public bool IsCallUsDefined { get; set; }
        [StringLength(200)]
        public string? SalesTerritoryLiveDisplayName { get; set; }
        public int SchedulingType { get; set; }

        [InverseProperty("SalesTerritory")]
        public virtual ICollection<CustomtableSalesTerritoryCountry> CustomtableSalesTerritoryCountries { get; set; }
        [InverseProperty("SalesTerritory")]
        public virtual ICollection<CustomtableSalesTerritoryState> CustomtableSalesTerritoryStates { get; set; }
    }
}
