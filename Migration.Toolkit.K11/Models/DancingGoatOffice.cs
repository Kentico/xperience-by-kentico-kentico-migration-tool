using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Office")]
public class DancingGoatOffice
{
    [Key]
    [Column("OfficeID")]
    public int OfficeId { get; set; }

    [StringLength(200)]
    public string OfficeName { get; set; } = null!;

    [StringLength(200)]
    public string OfficeCodeName { get; set; } = null!;

    [StringLength(200)]
    public string OfficeStreet { get; set; } = null!;

    [StringLength(200)]
    public string OfficeCity { get; set; } = null!;

    [StringLength(200)]
    public string OfficeCountry { get; set; } = null!;

    [StringLength(200)]
    public string OfficeZipCode { get; set; } = null!;

    [StringLength(200)]
    public string OfficePhone { get; set; } = null!;

    [StringLength(200)]
    public string OfficeEmail { get; set; } = null!;
}
