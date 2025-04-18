using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Keyless]
public class ViewOmAccountContactAccountJoined
{
    [Column("AccountID")]
    public int AccountId { get; set; }

    [StringLength(200)]
    public string AccountName { get; set; } = null!;

    [Column("ContactID")]
    public int ContactId { get; set; }

    [Column("AccountContactID")]
    public int AccountContactId { get; set; }

    [Column("ContactRoleID")]
    public int? ContactRoleId { get; set; }

    [Column("AccountCountryID")]
    public int? AccountCountryId { get; set; }

    [Column("AccountStatusID")]
    public int? AccountStatusId { get; set; }
}
