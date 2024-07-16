using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("CMS_DeviceProfile")]
public class CmsDeviceProfile
{
    [Key]
    [Column("ProfileID")]
    public int ProfileId { get; set; }

    [StringLength(100)]
    public string ProfileName { get; set; } = null!;

    [StringLength(200)]
    public string ProfileDisplayName { get; set; } = null!;

    public int? ProfileOrder { get; set; }

    public string? ProfileMacro { get; set; }

    public string? ProfileUserAgents { get; set; }

    [Required]
    public bool? ProfileEnabled { get; set; }

    public int? ProfilePreviewWidth { get; set; }

    public int? ProfilePreviewHeight { get; set; }

    [Column("ProfileGUID")]
    public Guid? ProfileGuid { get; set; }

    public DateTime? ProfileLastModified { get; set; }

    [InverseProperty("DeviceProfile")]
    public virtual ICollection<CmsDeviceProfileLayout> CmsDeviceProfileLayouts { get; set; } = new List<CmsDeviceProfileLayout>();

    [InverseProperty("Profile")]
    public virtual ICollection<CmsTemplateDeviceLayout> CmsTemplateDeviceLayouts { get; set; } = new List<CmsTemplateDeviceLayout>();
}
