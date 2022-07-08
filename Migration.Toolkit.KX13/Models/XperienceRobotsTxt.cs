using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_RobotsTxt")]
    public partial class XperienceRobotsTxt
    {
        [Key]
        [Column("RobotsTxtID")]
        public int RobotsTxtId { get; set; }
        public string? Content { get; set; }
    }
}
