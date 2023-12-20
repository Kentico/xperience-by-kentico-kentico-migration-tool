using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[PrimaryKey("EmailId", "UserId")]
[Table("CMS_EmailUser")]
[Index("Status", Name = "IX_CMS_EmailUser_Status")]
[Index("UserId", Name = "IX_CMS_EmailUser_UserID")]
public partial class CmsEmailUser
{
    [Key]
    [Column("EmailID")]
    public int EmailId { get; set; }

    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    public string? LastSendResult { get; set; }

    public DateTime? LastSendAttempt { get; set; }

    public int? Status { get; set; }

    [ForeignKey("EmailId")]
    [InverseProperty("CmsEmailUsers")]
    public virtual CmsEmail Email { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CmsEmailUsers")]
    public virtual CmsUser User { get; set; } = null!;
}
