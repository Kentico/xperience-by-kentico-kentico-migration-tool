using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_CallToAction")]
    public partial class XperienceCallToAction
    {
        [Key]
        [Column("CallToActionID")]
        public int CallToActionId { get; set; }
        [StringLength(200)]
        public string? Title { get; set; }
        public string? Content { get; set; }
        [Column("ThemeID")]
        [StringLength(200)]
        public string? ThemeId { get; set; }
        [StringLength(200)]
        public string? PrimaryAccentOverride { get; set; }
        [StringLength(200)]
        public string? SecondaryAccentOverride { get; set; }
        [StringLength(200)]
        public string? TextAccentOverride { get; set; }
        public bool? IsFullWidth { get; set; }
        [StringLength(200)]
        public string? LinkText1 { get; set; }
        public Guid? LinkNode1 { get; set; }
        [StringLength(512)]
        public string? ExternalLink1 { get; set; }
        public bool? LinkOpenNewTab1 { get; set; }
        [StringLength(200)]
        public string? LinkText2 { get; set; }
        public Guid? LinkNode2 { get; set; }
        [StringLength(512)]
        public string? ExternalLink2 { get; set; }
        public bool? LinkOpenNewTab2 { get; set; }
        [StringLength(200)]
        public string? TextBetweenLinks { get; set; }
    }
}
