using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[PrimaryKey("UserId", "Skuid", "SiteId")]
[Table("COM_Wishlist")]
[Index("Skuid", Name = "IX_COM_Wishlist_SKUID")]
[Index("SiteId", "UserId", Name = "IX_COM_Wishlist_SiteID_UserID")]
public partial class ComWishlist
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Key]
    [Column("SKUID")]
    public int Skuid { get; set; }

    [Key]
    [Column("SiteID")]
    public int SiteId { get; set; }

    [ForeignKey("SiteId")]
    [InverseProperty("ComWishlists")]
    public virtual CmsSite Site { get; set; } = null!;

    [ForeignKey("Skuid")]
    [InverseProperty("ComWishlists")]
    public virtual ComSku Sku { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ComWishlists")]
    public virtual CmsUser User { get; set; } = null!;
}
