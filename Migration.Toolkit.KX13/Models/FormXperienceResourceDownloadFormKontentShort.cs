using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_ResourceDownloadFormKontentShort")]
    public partial class FormXperienceResourceDownloadFormKontentShort
    {
        [Key]
        [Column("ResourceDownloadFormKontentShortID")]
        public int ResourceDownloadFormKontentShortId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [Column("email")]
        [StringLength(500)]
        public string Email { get; set; } = null!;
        [Column("company")]
        [StringLength(500)]
        public string Company { get; set; } = null!;
        [Column("consentMarketingSalesEU")]
        public bool? ConsentMarketingSalesEu { get; set; }
    }
}
