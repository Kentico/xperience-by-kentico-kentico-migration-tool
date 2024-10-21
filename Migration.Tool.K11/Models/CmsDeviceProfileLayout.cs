using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_DeviceProfileLayout")]
[Index("DeviceProfileId", Name = "IX_CMS_DeviceProfileLayout_DeviceProfileID")]
[Index("SourceLayoutId", Name = "IX_CMS_DeviceProfileLayout_SourceLayoutID")]
[Index("TargetLayoutId", Name = "IX_CMS_DeviceProfileLayout_TargetLayoutID")]
public class CmsDeviceProfileLayout
{
    [Key]
    [Column("DeviceProfileLayoutID")]
    public int DeviceProfileLayoutId { get; set; }

    [Column("DeviceProfileID")]
    public int DeviceProfileId { get; set; }

    [Column("SourceLayoutID")]
    public int SourceLayoutId { get; set; }

    [Column("TargetLayoutID")]
    public int TargetLayoutId { get; set; }

    [Column("DeviceProfileLayoutGUID")]
    public Guid DeviceProfileLayoutGuid { get; set; }

    public DateTime DeviceProfileLayoutLastModified { get; set; }

    [ForeignKey("DeviceProfileId")]
    [InverseProperty("CmsDeviceProfileLayouts")]
    public virtual CmsDeviceProfile DeviceProfile { get; set; } = null!;

    [ForeignKey("SourceLayoutId")]
    [InverseProperty("CmsDeviceProfileLayoutSourceLayouts")]
    public virtual CmsLayout SourceLayout { get; set; } = null!;

    [ForeignKey("TargetLayoutId")]
    [InverseProperty("CmsDeviceProfileLayoutTargetLayouts")]
    public virtual CmsLayout TargetLayout { get; set; } = null!;
}
