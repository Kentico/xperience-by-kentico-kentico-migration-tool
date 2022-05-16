﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CONTENT_PressRelease")]
    public partial class ContentPressRelease
    {
        [Key]
        [Column("PressReleaseID")]
        public int PressReleaseId { get; set; }
        [StringLength(400)]
        public string PressReleaseTitle { get; set; } = null!;
        public DateTime PressReleaseDate { get; set; }
        public string PressReleaseSummary { get; set; } = null!;
        public string PressReleaseText { get; set; } = null!;
        public string? PressReleaseAbout { get; set; }
        public string? PressReleaseTrademarks { get; set; }
    }
}
