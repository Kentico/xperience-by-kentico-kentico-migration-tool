using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_TabContent")]
    public partial class XperienceTabContent
    {
        [Key]
        [Column("TabContentID")]
        public int TabContentId { get; set; }
        [StringLength(200)]
        public string TabHeading { get; set; } = null!;
        [StringLength(200)]
        public string? TabIcon { get; set; }
        [StringLength(512)]
        public string ContentHeading { get; set; } = null!;
        public string ContentBody { get; set; } = null!;
        [StringLength(200)]
        public string? LinkText { get; set; }
        [StringLength(512)]
        public string? LinkUrl { get; set; }
        [StringLength(20)]
        public string? LinkTarget { get; set; }
        [StringLength(512)]
        public string Image { get; set; } = null!;
        [StringLength(200)]
        public string? Video { get; set; }
        [StringLength(200)]
        public string? VideoTitle { get; set; }
        [StringLength(200)]
        public string? VideoSubtitle { get; set; }
    }
}
