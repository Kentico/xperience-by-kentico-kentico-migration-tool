using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KXP.Models;

[Table("CMS_ContentItemTag")]
[Index("ContentItemTagContentItemLanguageMetadataId", Name = "IX_CMS_ContentItemTag_ContentItemTagContentItemLanguageMetadataID")]
[Index("ContentItemTagFieldGuid", Name = "IX_CMS_ContentItemTag_ContentItemTagFieldGUID")]
[Index("ContentItemTagTagGuid", Name = "IX_CMS_ContentItemTag_ContentItemTagTagGUID")]
[Index("ContentItemTagTagGuid", "ContentItemTagFieldGuid", "ContentItemTagContentItemLanguageMetadataId", Name = "IX_CMS_ContentItemTag_ContentItemTagTagGUID_ContentItemTagFieldGUID_ContentItemTagContentItemLanguageMetadataID", IsUnique = true)]
public class CmsContentItemTag
{
    [Key]
    [Column("ContentItemTagID")]
    public int ContentItemTagId { get; set; }

    [Column("ContentItemTagContentItemLanguageMetadataID")]
    public int ContentItemTagContentItemLanguageMetadataId { get; set; }

    [Column("ContentItemTagFieldGUID")]
    public Guid ContentItemTagFieldGuid { get; set; }

    [Column("ContentItemTagTagGUID")]
    public Guid ContentItemTagTagGuid { get; set; }

    [ForeignKey("ContentItemTagContentItemLanguageMetadataId")]
    [InverseProperty("CmsContentItemTags")]
    public virtual CmsContentItemLanguageMetadatum ContentItemTagContentItemLanguageMetadata { get; set; } = null!;
}
