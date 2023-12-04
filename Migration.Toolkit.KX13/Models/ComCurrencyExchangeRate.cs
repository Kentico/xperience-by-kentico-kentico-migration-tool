using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_CurrencyExchangeRate")]
[Index("ExchangeRateToCurrencyId", Name = "IX_COM_CurrencyExchangeRate_ExchangeRateToCurrencyID")]
[Index("ExchangeTableId", Name = "IX_COM_CurrencyExchangeRate_ExchangeTableID")]
public partial class ComCurrencyExchangeRate
{
    [Key]
    [Column("ExchagneRateID")]
    public int ExchagneRateId { get; set; }

    [Column("ExchangeRateToCurrencyID")]
    public int ExchangeRateToCurrencyId { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal ExchangeRateValue { get; set; }

    [Column("ExchangeTableID")]
    public int ExchangeTableId { get; set; }

    [Column("ExchangeRateGUID")]
    public Guid ExchangeRateGuid { get; set; }

    public DateTime ExchangeRateLastModified { get; set; }

    [ForeignKey("ExchangeRateToCurrencyId")]
    [InverseProperty("ComCurrencyExchangeRates")]
    public virtual ComCurrency ExchangeRateToCurrency { get; set; } = null!;

    [ForeignKey("ExchangeTableId")]
    [InverseProperty("ComCurrencyExchangeRates")]
    public virtual ComExchangeTable ExchangeTable { get; set; } = null!;
}
