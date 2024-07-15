using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_Taxonomy")]
public partial class CmsTaxonomy
{
    [Key]
    [Column("TaxonomyID")]
    public int TaxonomyId { get; set; }

    [StringLength(200)]
    public string TaxonomyName { get; set; } = null!;

    [Column("TaxonomyGUID")]
    public Guid TaxonomyGuid { get; set; }

    public string? TaxonomyMetadata { get; set; }

    public DateTime TaxonomyLastModified { get; set; }

    [StringLength(200)]
    public string TaxonomyTitle { get; set; } = null!;

    public string? TaxonomyDescription { get; set; }

    [InverseProperty("TagTaxonomy")]
    public virtual ICollection<CmsTag> CmsTags { get; set; } = new List<CmsTag>();
}