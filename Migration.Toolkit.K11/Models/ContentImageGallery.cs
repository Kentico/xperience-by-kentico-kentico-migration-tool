using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_ImageGallery")]
public partial class ContentImageGallery
{
    [Key]
    [Column("ImageGalleryID")]
    public int ImageGalleryId { get; set; }

    [StringLength(1000)]
    public string GalleryName { get; set; } = null!;

    public string? GalleryDescription { get; set; }

    public Guid? GalleryTeaserImage { get; set; }
}