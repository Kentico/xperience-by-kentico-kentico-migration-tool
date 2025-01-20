using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.KX12.Models;

[Table("OM_ContactRole")]
public class OmContactRole
{
    [Key]
    [Column("ContactRoleID")]
    public int ContactRoleId { get; set; }

    [StringLength(200)]
    public string ContactRoleName { get; set; } = null!;

    [StringLength(200)]
    public string ContactRoleDisplayName { get; set; } = null!;

    public string? ContactRoleDescription { get; set; }

    [InverseProperty("ContactRole")]
    public virtual ICollection<OmAccountContact> OmAccountContacts { get; set; } = [];
}
