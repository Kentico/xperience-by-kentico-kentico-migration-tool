using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Agenda")]
    public partial class XperienceAgendum
    {
        [Key]
        [Column("AgendaID")]
        public int AgendaId { get; set; }
        [StringLength(200)]
        public string Time { get; set; } = null!;
        [StringLength(300)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [StringLength(200)]
        public string? SpeakerName { get; set; }
        [StringLength(200)]
        public string? SpeakerImage { get; set; }
        [StringLength(200)]
        public string? SpeakerJobTitle { get; set; }
        [StringLength(200)]
        public string? SpeakerName2 { get; set; }
        [StringLength(200)]
        public string? SpeakerImage2 { get; set; }
        [StringLength(200)]
        public string? SpeakerJobTitle2 { get; set; }
        [StringLength(200)]
        public string? Track { get; set; }
        [StringLength(20)]
        public string? VideoCode { get; set; }
        [StringLength(200)]
        public string? VideoThumbnail { get; set; }
        [StringLength(20)]
        public string? VideoCode2 { get; set; }
        [StringLength(200)]
        public string? VideoThumbnail2 { get; set; }
        [StringLength(20)]
        public string? VideoCode3 { get; set; }
        [StringLength(200)]
        public string? VideoThumbnail3 { get; set; }
        [StringLength(200)]
        public string? SpeakerName3 { get; set; }
        [StringLength(200)]
        public string? SpeakerImage3 { get; set; }
        [StringLength(200)]
        public string? SpeakerJobTitle3 { get; set; }
        [StringLength(200)]
        public string? SpeakerName4 { get; set; }
        [StringLength(200)]
        public string? SpeakerImage4 { get; set; }
        [StringLength(200)]
        public string? SpeakerJobTitle4 { get; set; }
        [StringLength(200)]
        public string? SpeakerName5 { get; set; }
        [StringLength(200)]
        public string? SpeakerImage5 { get; set; }
        [StringLength(200)]
        public string? SpeakerJobTitle5 { get; set; }
    }
}
