using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("customtable_SalesTerritoryState")]
    public partial class CustomtableSalesTerritoryState
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
        [Column("StateID")]
        public int StateId { get; set; }

        [ForeignKey("SalesTerritoryId")]
        [InverseProperty("CustomtableSalesTerritoryStates")]
        public virtual CustomtableSalesTerritory SalesTerritory { get; set; } = null!;
        [ForeignKey("StateId")]
        [InverseProperty("CustomtableSalesTerritoryStates")]
        public virtual CmsState State { get; set; } = null!;
    }
}
