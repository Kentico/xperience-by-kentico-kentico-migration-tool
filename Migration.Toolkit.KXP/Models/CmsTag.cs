using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_Tag")]
[Index("TagParentId", Name = "IX_CMS_Tag_TagParentID")]
[Index("TagTaxonomyId", Name = "IX_CMS_Tag_TagTaxonomyID")]
[Index("TagTitle", "TagTaxonomyId", Name = "IX_CMS_Tag_TagTitle_TagTaxonomyID_Unique", IsUnique = true)]
public partial class CmsTag
{
    [Key]
    [Column("TagID")]
    public int TagId { get; set; }

    [StringLength(200)]
    public string TagName { get; set; } = null!;

    [Column("TagGUID")]
    public Guid TagGuid { get; set; }

    [Column("TagTaxonomyID")]
    public int TagTaxonomyId { get; set; }

    [Column("TagParentID")]
    public int? TagParentId { get; set; }

    public int? TagOrder { get; set; }

    public string? TagMetadata { get; set; }

    public DateTime TagLastModified { get; set; }

    [StringLength(200)]
    public string TagTitle { get; set; } = null!;

    public string? TagDescription { get; set; }

    [InverseProperty("TagParent")]
    public virtual ICollection<CmsTag> InverseTagParent { get; set; } = new List<CmsTag>();

    [ForeignKey("TagParentId")]
    [InverseProperty("InverseTagParent")]
    public virtual CmsTag? TagParent { get; set; }

    [ForeignKey("TagTaxonomyId")]
    [InverseProperty("CmsTags")]
    public virtual CmsTaxonomy TagTaxonomy { get; set; } = null!;
}
