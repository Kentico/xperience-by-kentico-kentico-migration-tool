using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Avatar")]
[Index("AvatarGuid", Name = "IX_CMS_Avatar_AvatarGUID")]
public partial class CmsAvatar
{
    [Key]
    [Column("AvatarID")]
    public int AvatarId { get; set; }

    [StringLength(200)]
    public string? AvatarName { get; set; }

    [StringLength(200)]
    public string AvatarFileName { get; set; } = null!;

    [StringLength(10)]
    public string AvatarFileExtension { get; set; } = null!;

    public byte[]? AvatarBinary { get; set; }

    [Column("AvatarGUID")]
    public Guid AvatarGuid { get; set; }

    public DateTime AvatarLastModified { get; set; }

    [StringLength(100)]
    public string AvatarMimeType { get; set; } = null!;

    public int AvatarFileSize { get; set; }

    public int? AvatarImageHeight { get; set; }

    public int? AvatarImageWidth { get; set; }

    [InverseProperty("UserAvatar")]
    public virtual ICollection<CmsUserSetting> CmsUserSettings { get; set; } = new List<CmsUserSetting>();
}
