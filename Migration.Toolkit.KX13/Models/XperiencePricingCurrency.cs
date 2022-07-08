using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_PricingCurrency")]
    public partial class XperiencePricingCurrency
    {
        [Key]
        [Column("PricingCurrencyID")]
        public int PricingCurrencyId { get; set; }
        public Guid PricingCurrencyGuid { get; set; }
        public DateTime PricingCurrencyLastModified { get; set; }
        [StringLength(200)]
        public string PriceCurrencyDisplayName { get; set; } = null!;
        public int PriceCurrency { get; set; }
        public string? AssociatedCountries { get; set; }
        [StringLength(200)]
        public string PriceCurrencyFormatString { get; set; } = null!;
        [StringLength(1)]
        public string PriceCurrencyGroupSeparator { get; set; } = null!;
        [StringLength(1)]
        public string PriceCurrencyDecimalSeparator { get; set; } = null!;
    }
}
