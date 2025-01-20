using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX13.Models;

[Table("COM_ExchangeTable")]
[Index("ExchangeTableSiteId", Name = "IX_COM_ExchangeTable_ExchangeTableSiteID")]
public class ComExchangeTable
{
    [Key]
    [Column("ExchangeTableID")]
    public int ExchangeTableId { get; set; }

    [StringLength(200)]
    public string ExchangeTableDisplayName { get; set; } = null!;

    public DateTime? ExchangeTableValidFrom { get; set; }

    public DateTime? ExchangeTableValidTo { get; set; }

    [Column("ExchangeTableGUID")]
    public Guid ExchangeTableGuid { get; set; }

    public DateTime ExchangeTableLastModified { get; set; }

    [Column("ExchangeTableSiteID")]
    public int? ExchangeTableSiteId { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal? ExchangeTableRateFromGlobalCurrency { get; set; }

    [InverseProperty("ExchangeTable")]
    public virtual ICollection<ComCurrencyExchangeRate> ComCurrencyExchangeRates { get; set; } = [];

    [ForeignKey("ExchangeTableSiteId")]
    [InverseProperty("ComExchangeTables")]
    public virtual CmsSite? ExchangeTableSite { get; set; }
}
