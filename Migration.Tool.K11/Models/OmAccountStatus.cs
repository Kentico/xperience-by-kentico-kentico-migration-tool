using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("OM_AccountStatus")]
public class OmAccountStatus
{
    [Key]
    [Column("AccountStatusID")]
    public int AccountStatusId { get; set; }

    [StringLength(200)]
    public string AccountStatusName { get; set; } = null!;

    [StringLength(200)]
    public string AccountStatusDisplayName { get; set; } = null!;

    public string? AccountStatusDescription { get; set; }

    [InverseProperty("AccountStatus")]
    public virtual ICollection<OmAccount> OmAccounts { get; set; } = new List<OmAccount>();
}
