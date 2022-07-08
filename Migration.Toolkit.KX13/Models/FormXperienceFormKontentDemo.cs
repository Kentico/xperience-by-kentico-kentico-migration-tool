using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_FormKontentDemo")]
    public partial class FormXperienceFormKontentDemo
    {
        [Key]
        [Column("FormKontentDemoID")]
        public int FormKontentDemoId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [Column("firstName")]
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [Column("lastName")]
        [StringLength(500)]
        public string LastName { get; set; } = null!;
        [Column("email")]
        [StringLength(500)]
        public string Email { get; set; } = null!;
        [Column("company")]
        [StringLength(500)]
        public string Company { get; set; } = null!;
        [Column("country")]
        [StringLength(200)]
        public string Country { get; set; } = null!;
        [Column("role")]
        [StringLength(500)]
        public string Role { get; set; } = null!;
        [Column("message")]
        public string? Message { get; set; }
        [Column("consentMarketingSalesEU")]
        public bool? ConsentMarketingSalesEu { get; set; }
    }
}
