using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_PriceTier")]
    public partial class XperiencePriceTier
    {
        [Key]
        [Column("PriceTierID")]
        public int PriceTierId { get; set; }
        [StringLength(200)]
        public string Title { get; set; } = null!;
        [StringLength(200)]
        public string? RecommendedSize { get; set; }
        [StringLength(200)]
        public string? SubTitle { get; set; }
        [StringLength(200)]
        public string? PriceText { get; set; }
        [StringLength(512)]
        public string LinkUrl { get; set; } = null!;
        [StringLength(20)]
        public string? LinkTarget { get; set; }
        public string FeaturesList { get; set; } = null!;
        public bool IsFeatured { get; set; }
        [StringLength(200)]
        public string FeaturePromptText { get; set; } = null!;
    }
}
