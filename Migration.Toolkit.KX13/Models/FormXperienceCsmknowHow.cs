using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_CSMKnow_how")]
    public partial class FormXperienceCsmknowHow
    {
        [Key]
        [Column("CSMKnow_howID")]
        public int CsmknowHowId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string? LastName { get; set; }
        [StringLength(500)]
        public string? EmailInput { get; set; }
        [StringLength(200)]
        public string? Role { get; set; }
        [StringLength(500)]
        public string? CompanyName { get; set; }
        public string? Details { get; set; }
    }
}
