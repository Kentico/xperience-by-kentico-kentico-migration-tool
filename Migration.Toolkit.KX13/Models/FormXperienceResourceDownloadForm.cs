using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_ResourceDownloadForm")]
    public partial class FormXperienceResourceDownloadForm
    {
        [Key]
        [Column("ResourceDownloadFormID")]
        public int ResourceDownloadFormId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string FirstName { get; set; } = null!;
        [StringLength(500)]
        public string Surname { get; set; } = null!;
        [StringLength(500)]
        public string EmailAddress { get; set; } = null!;
        [StringLength(500)]
        public string? Company { get; set; }
        [StringLength(500)]
        public string? ResourceUrl { get; set; }
        [StringLength(500)]
        public string? PageUrl { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
        [StringLength(500)]
        public string? JobPosition { get; set; }
        [StringLength(500)]
        public string? Phone { get; set; }
    }
}
