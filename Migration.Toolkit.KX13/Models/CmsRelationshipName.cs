using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_RelationshipName")]
[Index("RelationshipAllowedObjects", Name = "IX_CMS_RelationshipName_RelationshipAllowedObjects")]
[Index("RelationshipName", "RelationshipDisplayName", Name = "IX_CMS_RelationshipName_RelationshipName_RelationshipDisplayName")]
public partial class CmsRelationshipName
{
    [Key]
    [Column("RelationshipNameID")]
    public int RelationshipNameId { get; set; }

    [StringLength(200)]
    public string RelationshipDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string RelationshipName { get; set; } = null!;

    public string? RelationshipAllowedObjects { get; set; }

    [Column("RelationshipGUID")]
    public Guid RelationshipGuid { get; set; }

    public DateTime RelationshipLastModified { get; set; }

    public bool? RelationshipNameIsAdHoc { get; set; }

    [InverseProperty("RelationshipName")]
    public virtual ICollection<CmsRelationship> CmsRelationships { get; set; } = new List<CmsRelationship>();

    [ForeignKey("RelationshipNameId")]
    [InverseProperty("RelationshipNames")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
