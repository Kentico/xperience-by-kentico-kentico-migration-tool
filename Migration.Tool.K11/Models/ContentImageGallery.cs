using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_ImageGallery")]
public class ContentImageGallery
{
    [Key]
    [Column("ImageGalleryID")]
    public int ImageGalleryId { get; set; }

    [StringLength(1000)]
    public string GalleryName { get; set; } = null!;

    public string? GalleryDescription { get; set; }

    public Guid? GalleryTeaserImage { get; set; }
}
