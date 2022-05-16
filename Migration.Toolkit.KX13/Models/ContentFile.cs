﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CONTENT_File")]
    public partial class ContentFile
    {
        [Key]
        [Column("FileID")]
        public int FileId { get; set; }
        [StringLength(500)]
        public string? FileDescription { get; set; }
        [StringLength(100)]
        public string FileName { get; set; } = null!;
        public Guid? FileAttachment { get; set; }
    }
}
