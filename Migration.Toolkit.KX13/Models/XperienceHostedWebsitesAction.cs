using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_HostedWebsitesAction")]
    public partial class XperienceHostedWebsitesAction
    {
        [Key]
        [Column("HostedWebsitesActionID")]
        public int HostedWebsitesActionId { get; set; }
        [StringLength(200)]
        public string ActionName { get; set; } = null!;
    }
}
