using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_ACLItem")]
[Index("Aclid", Name = "IX_CMS_ACLItem_ACLID")]
[Index("LastModifiedByUserId", Name = "IX_CMS_ACLItem_LastModifiedByUserID")]
[Index("RoleId", Name = "IX_CMS_ACLItem_RoleID")]
[Index("UserId", Name = "IX_CMS_ACLItem_UserID")]
public partial class CmsAclitem
{
    [Key]
    [Column("ACLItemID")]
    public int AclitemId { get; set; }

    [Column("ACLID")]
    public int Aclid { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("RoleID")]
    public int? RoleId { get; set; }

    public int Allowed { get; set; }

    public int Denied { get; set; }

    public DateTime LastModified { get; set; }

    [Column("LastModifiedByUserID")]
    public int? LastModifiedByUserId { get; set; }

    [Column("ACLItemGUID")]
    public Guid AclitemGuid { get; set; }

    [ForeignKey("Aclid")]
    [InverseProperty("CmsAclitems")]
    public virtual CmsAcl Acl { get; set; } = null!;

    [ForeignKey("LastModifiedByUserId")]
    [InverseProperty("CmsAclitemLastModifiedByUsers")]
    public virtual CmsUser? LastModifiedByUser { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("CmsAclitems")]
    public virtual CmsRole? Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CmsAclitemUsers")]
    public virtual CmsUser? User { get; set; }
}
