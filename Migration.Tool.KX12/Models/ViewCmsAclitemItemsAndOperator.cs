using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Keyless]
public class ViewCmsAclitemItemsAndOperator
{
    [Column("ACLOwnerNodeID")]
    public int AclownerNodeId { get; set; }

    [Column("ACLItemID")]
    public int AclitemId { get; set; }

    public int Allowed { get; set; }

    public int Denied { get; set; }

    [StringLength(51)]
    public string? Operator { get; set; }

    [StringLength(100)]
    public string? OperatorName { get; set; }

    [Column("ACLID")]
    public int Aclid { get; set; }

    [StringLength(450)]
    public string? OperatorFullName { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("RoleID")]
    public int? RoleId { get; set; }

    [Column("RoleGroupID")]
    public int? RoleGroupId { get; set; }

    [Column("SiteID")]
    public int? SiteId { get; set; }
}
