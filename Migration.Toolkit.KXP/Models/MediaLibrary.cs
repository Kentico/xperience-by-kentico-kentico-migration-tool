using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("Media_Library")]
    [Index("LibraryName", "LibraryGuid", Name = "IX_Media_Library_LibrarySiteID_LibraryName_LibraryGUID", IsUnique = true)]
    public partial class MediaLibrary
    {
        public MediaLibrary()
        {
            MediaFiles = new HashSet<MediaFile>();
        }

        [Key]
        [Column("LibraryID")]
        public int LibraryId { get; set; }
        [StringLength(250)]
        public string LibraryName { get; set; } = null!;
        [StringLength(250)]
        public string LibraryDisplayName { get; set; } = null!;
        public string? LibraryDescription { get; set; }
        [StringLength(250)]
        public string LibraryFolder { get; set; } = null!;
        public int? LibraryAccess { get; set; }
        [Column("LibraryGUID")]
        public Guid? LibraryGuid { get; set; }
        public DateTime? LibraryLastModified { get; set; }

        [InverseProperty("FileLibrary")]
        public virtual ICollection<MediaFile> MediaFiles { get; set; }
    }
}
