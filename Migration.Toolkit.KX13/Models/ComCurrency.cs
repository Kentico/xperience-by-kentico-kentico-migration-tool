using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Currency")]
[Index("CurrencyDisplayName", Name = "IX_COM_Currency_CurrencyDisplayName")]
[Index("CurrencySiteId", Name = "IX_COM_Currency_CurrencySiteID")]
public partial class ComCurrency
{
    [Key]
    [Column("CurrencyID")]
    public int CurrencyId { get; set; }

    [StringLength(200)]
    public string CurrencyName { get; set; } = null!;

    [StringLength(200)]
    public string CurrencyDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string CurrencyCode { get; set; } = null!;

    public int? CurrencyRoundTo { get; set; }

    public bool CurrencyEnabled { get; set; }

    [StringLength(200)]
    public string CurrencyFormatString { get; set; } = null!;

    public bool CurrencyIsMain { get; set; }

    [Column("CurrencyGUID")]
    public Guid? CurrencyGuid { get; set; }

    public DateTime CurrencyLastModified { get; set; }

    [Column("CurrencySiteID")]
    public int? CurrencySiteId { get; set; }

    [InverseProperty("ExchangeRateToCurrency")]
    public virtual ICollection<ComCurrencyExchangeRate> ComCurrencyExchangeRates { get; set; } = new List<ComCurrencyExchangeRate>();

    [InverseProperty("OrderCurrency")]
    public virtual ICollection<ComOrder> ComOrders { get; set; } = new List<ComOrder>();

    [InverseProperty("ShoppingCartCurrency")]
    public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; } = new List<ComShoppingCart>();

    [ForeignKey("CurrencySiteId")]
    [InverseProperty("ComCurrencies")]
    public virtual CmsSite? CurrencySite { get; set; }
}
