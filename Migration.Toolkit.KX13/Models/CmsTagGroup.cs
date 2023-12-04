using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_TagGroup")]
[Index("TagGroupSiteId", Name = "IX_CMS_TagGroup_TagGroupSiteID")]
public partial class CmsTagGroup
{
    [Key]
    [Column("TagGroupID")]
    public int TagGroupId { get; set; }

    [StringLength(250)]
    public string TagGroupDisplayName { get; set; } = null!;

    [StringLength(250)]
    public string TagGroupName { get; set; } = null!;

    public string? TagGroupDescription { get; set; }

    [Column("TagGroupSiteID")]
    public int TagGroupSiteId { get; set; }

    public bool TagGroupIsAdHoc { get; set; }

    public DateTime TagGroupLastModified { get; set; }

    [Column("TagGroupGUID")]
    public Guid TagGroupGuid { get; set; }

    [InverseProperty("DocumentTagGroup")]
    public virtual ICollection<CmsDocument> CmsDocuments { get; set; } = new List<CmsDocument>();

    [InverseProperty("TagGroup")]
    public virtual ICollection<CmsTag> CmsTags { get; set; } = new List<CmsTag>();

    [ForeignKey("TagGroupSiteId")]
    [InverseProperty("CmsTagGroups")]
    public virtual CmsSite TagGroupSite { get; set; } = null!;
}
