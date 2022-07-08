using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Unsubscription")]
    public partial class XperienceUnsubscription
    {
        [Key]
        [Column("UnsubscriptionID")]
        public int UnsubscriptionId { get; set; }
        [StringLength(200)]
        public string? Name { get; set; }
    }
}
