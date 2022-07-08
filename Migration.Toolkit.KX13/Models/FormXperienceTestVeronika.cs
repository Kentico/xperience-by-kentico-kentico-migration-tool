using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Test_Veronika")]
    public partial class FormXperienceTestVeronika
    {
        [Key]
        [Column("Test_VeronikaID")]
        public int TestVeronikaId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string? FirstName { get; set; }
        [StringLength(500)]
        public string EmailInput { get; set; } = null!;
        [StringLength(500)]
        public string? Campagn { get; set; }
    }
}
