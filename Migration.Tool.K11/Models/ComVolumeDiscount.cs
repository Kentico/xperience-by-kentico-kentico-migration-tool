using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("COM_VolumeDiscount")]
[Index("VolumeDiscountSkuid", Name = "IX_COM_VolumeDiscount_VolumeDiscountSKUID")]
public class ComVolumeDiscount
{
    [Key]
    [Column("VolumeDiscountID")]
    public int VolumeDiscountId { get; set; }

    [Column("VolumeDiscountSKUID")]
    public int VolumeDiscountSkuid { get; set; }

    public int VolumeDiscountMinCount { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal VolumeDiscountValue { get; set; }

    public bool VolumeDiscountIsFlatValue { get; set; }

    [Column("VolumeDiscountGUID")]
    public Guid VolumeDiscountGuid { get; set; }

    public DateTime VolumeDiscountLastModified { get; set; }

    [ForeignKey("VolumeDiscountSkuid")]
    [InverseProperty("ComVolumeDiscounts")]
    public virtual ComSku VolumeDiscountSku { get; set; } = null!;
}
