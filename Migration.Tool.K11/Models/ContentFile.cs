using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_File")]
public class ContentFile
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
