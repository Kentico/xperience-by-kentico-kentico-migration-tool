using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Event")]
    public partial class XperienceEvent
    {
        [Key]
        [Column("EventID")]
        public int EventId { get; set; }
        [StringLength(200)]
        public string Name { get; set; } = null!;
        [StringLength(200)]
        public string? CardTitle { get; set; }
        [StringLength(500)]
        public string? CardSummary { get; set; }
        [StringLength(512)]
        public string? CardImage { get; set; }
        [StringLength(200)]
        public string? HeaderButtonText { get; set; }
        [StringLength(512)]
        public string? HeaderButtonUrl { get; set; }
        [StringLength(20)]
        public string? HeaderButtonTarget { get; set; }
        public string? FooterText { get; set; }
        [StringLength(400)]
        public string EventLocation { get; set; } = null!;
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        [StringLength(512)]
        public string? Image { get; set; }
        [StringLength(200)]
        public string? SubNavButtonText { get; set; }
        [StringLength(512)]
        public string? SubNavButtonUrl { get; set; }
        [StringLength(20)]
        public string? SubNavButtonTarget { get; set; }
        public bool? Featured { get; set; }
        public bool? SubNavDisable { get; set; }
        public bool? HideFromNavigation { get; set; }
        [Column("CanonicalURL")]
        [StringLength(512)]
        public string? CanonicalUrl { get; set; }
        [StringLength(512)]
        public string? OgImage { get; set; }
        [StringLength(200)]
        public string? CardPinText { get; set; }
        public bool? ShowContactData { get; set; }
    }
}
