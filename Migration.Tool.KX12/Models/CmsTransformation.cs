using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("CMS_Transformation")]
[Index("TransformationClassId", Name = "IX_CMS_Transformation_TransformationClassID")]
public class CmsTransformation
{
    [Key]
    [Column("TransformationID")]
    public int TransformationId { get; set; }

    [StringLength(100)]
    public string TransformationName { get; set; } = null!;

    public string TransformationCode { get; set; } = null!;

    [StringLength(50)]
    public string TransformationType { get; set; } = null!;

    [Column("TransformationClassID")]
    public int TransformationClassId { get; set; }

    [Column("TransformationVersionGUID")]
    [StringLength(50)]
    public string? TransformationVersionGuid { get; set; }

    [Column("TransformationGUID")]
    public Guid TransformationGuid { get; set; }

    public DateTime TransformationLastModified { get; set; }

    public bool? TransformationIsHierarchical { get; set; }

    [Column("TransformationHierarchicalXML")]
    public string? TransformationHierarchicalXml { get; set; }

    [Column("TransformationCSS")]
    public string? TransformationCss { get; set; }

    [StringLength(700)]
    public string? TransformationPreferredDocument { get; set; }

    [ForeignKey("TransformationClassId")]
    [InverseProperty("CmsTransformations")]
    public virtual CmsClass TransformationClass { get; set; } = null!;
}
