using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_SKUOptionCategory")]
[Index("CategoryId", Name = "IX_COM_SKUOptionCategory_CategoryID")]
[Index("Skuid", Name = "IX_COM_SKUOptionCategory_SKUID")]
public partial class ComSkuoptionCategory
{
    [Column("SKUID")]
    public int Skuid { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    public bool? AllowAllOptions { get; set; }

    [Key]
    [Column("SKUCategoryID")]
    public int SkucategoryId { get; set; }

    [Column("SKUCategoryOrder")]
    public int? SkucategoryOrder { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ComSkuoptionCategories")]
    public virtual ComOptionCategory Category { get; set; } = null!;

    [ForeignKey("Skuid")]
    [InverseProperty("ComSkuoptionCategories")]
    public virtual ComSku Sku { get; set; } = null!;
}
