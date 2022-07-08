using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_TestLukas")]
    public partial class FormXperienceTestLuka
    {
        [Key]
        [Column("TestLukasID")]
        public int TestLukasId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        public Guid? CustomConsentAgreement { get; set; }
    }
}
