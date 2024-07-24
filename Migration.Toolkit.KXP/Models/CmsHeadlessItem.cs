using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_HeadlessItem")]
[Index("HeadlessItemContentItemId", Name = "IX_CMS_HeadlessItem_HeadlessItemContentItemID")]
[Index("HeadlessItemHeadlessChannelId", Name = "IX_CMS_HeadlessItem_HeadlessItemHeadlessChannelID")]
public class CmsHeadlessItem
{
    [Key]
    [Column("HeadlessItemID")]
    public int HeadlessItemId { get; set; }

    [Column("HeadlessItemGUID")]
    public Guid HeadlessItemGuid { get; set; }

    [Column("HeadlessItemHeadlessChannelID")]
    public int HeadlessItemHeadlessChannelId { get; set; }

    [Column("HeadlessItemContentItemID")]
    public int HeadlessItemContentItemId { get; set; }

    [StringLength(200)]
    public string HeadlessItemName { get; set; } = null!;

    [ForeignKey("HeadlessItemContentItemId")]
    [InverseProperty("CmsHeadlessItems")]
    public virtual CmsContentItem HeadlessItemContentItem { get; set; } = null!;

    [ForeignKey("HeadlessItemHeadlessChannelId")]
    [InverseProperty("CmsHeadlessItems")]
    public virtual CmsHeadlessChannel HeadlessItemHeadlessChannel { get; set; } = null!;
}
