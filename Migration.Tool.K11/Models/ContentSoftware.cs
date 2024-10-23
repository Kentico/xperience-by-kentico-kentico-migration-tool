using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_Software")]
public class ContentSoftware
{
    [Key]
    [Column("SoftwareID")]
    public int SoftwareId { get; set; }

    [StringLength(100)]
    public string? SoftwarePlatform { get; set; }

    [StringLength(100)]
    public string? SoftwareLicense { get; set; }

    [StringLength(100)]
    public string? SoftwareVersion { get; set; }

    [StringLength(100)]
    public string? SoftwareLanguage { get; set; }
}
