using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("customtable_SampleTable")]
public class CustomtableSampleTable
{
    [Key]
    [Column("ItemID")]
    public int ItemId { get; set; }

    public int? ItemCreatedBy { get; set; }

    public DateTime? ItemCreatedWhen { get; set; }

    public int? ItemModifiedBy { get; set; }

    public DateTime? ItemModifiedWhen { get; set; }

    public int? ItemOrder { get; set; }

    [StringLength(400)]
    public string ItemText { get; set; } = null!;

    [Column("ItemGUID")]
    public Guid ItemGuid { get; set; }
}
