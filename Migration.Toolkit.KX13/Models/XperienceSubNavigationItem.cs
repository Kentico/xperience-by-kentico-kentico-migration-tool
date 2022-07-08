using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_SubNavigationItem")]
    public partial class XperienceSubNavigationItem
    {
        [Key]
        [Column("SubNavigationItemID")]
        public int SubNavigationItemId { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; } = null!;
        public Guid? RedirectPage { get; set; }
        [Column("RedirectExternalURL")]
        [StringLength(512)]
        public string? RedirectExternalUrl { get; set; }
    }
}
