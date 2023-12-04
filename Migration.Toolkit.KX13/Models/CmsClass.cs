using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Class")]
[Index("ClassName", Name = "IX_CMS_Class_ClassName", IsUnique = true)]
[Index("ClassName", "ClassGuid", Name = "IX_CMS_Class_ClassName_ClassGUID")]
[Index("ClassResourceId", Name = "IX_CMS_Class_ClassResourceID")]
[Index("ClassShowAsSystemTable", "ClassIsCustomTable", "ClassIsCoupledClass", "ClassIsDocumentType", Name = "IX_CMS_Class_ClassShowAsSystemTable_ClassIsCustomTable_ClassIsCoupledClass_ClassIsDocumentType")]
public partial class CmsClass
{
    [Key]
    [Column("ClassID")]
    public int ClassId { get; set; }

    [StringLength(100)]
    public string ClassDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string ClassName { get; set; } = null!;

    public bool ClassUsesVersioning { get; set; }

    public bool ClassIsDocumentType { get; set; }

    public bool ClassIsCoupledClass { get; set; }

    public string ClassXmlSchema { get; set; } = null!;

    public string ClassFormDefinition { get; set; } = null!;

    [StringLength(100)]
    public string ClassNodeNameSource { get; set; } = null!;

    [StringLength(100)]
    public string? ClassTableName { get; set; }

    public string? ClassFormLayout { get; set; }

    public bool? ClassShowAsSystemTable { get; set; }

    public bool? ClassUsePublishFromTo { get; set; }

    public bool? ClassShowTemplateSelection { get; set; }

    [Column("ClassSKUMappings")]
    public string? ClassSkumappings { get; set; }

    public bool? ClassIsMenuItemType { get; set; }

    [StringLength(100)]
    public string? ClassNodeAliasSource { get; set; }

    public DateTime ClassLastModified { get; set; }

    [Column("ClassGUID")]
    public Guid ClassGuid { get; set; }

    [Column("ClassCreateSKU")]
    public bool? ClassCreateSku { get; set; }

    public bool? ClassIsProduct { get; set; }

    public bool ClassIsCustomTable { get; set; }

    [StringLength(1000)]
    public string? ClassShowColumns { get; set; }

    [StringLength(200)]
    public string? ClassSearchTitleColumn { get; set; }

    [StringLength(200)]
    public string? ClassSearchContentColumn { get; set; }

    [StringLength(200)]
    public string? ClassSearchImageColumn { get; set; }

    [StringLength(200)]
    public string? ClassSearchCreationDateColumn { get; set; }

    public string? ClassSearchSettings { get; set; }

    [Column("ClassInheritsFromClassID")]
    public int? ClassInheritsFromClassId { get; set; }

    public bool? ClassSearchEnabled { get; set; }

    [Column("ClassSKUDefaultDepartmentName")]
    [StringLength(200)]
    public string? ClassSkudefaultDepartmentName { get; set; }

    [Column("ClassSKUDefaultDepartmentID")]
    public int? ClassSkudefaultDepartmentId { get; set; }

    public string? ClassContactMapping { get; set; }

    public bool? ClassContactOverwriteEnabled { get; set; }

    [Column("ClassSKUDefaultProductType")]
    [StringLength(50)]
    public string? ClassSkudefaultProductType { get; set; }

    [StringLength(100)]
    public string? ClassConnectionString { get; set; }

    public bool? ClassIsProductSection { get; set; }

    [StringLength(50)]
    public string? ClassFormLayoutType { get; set; }

    [Column("ClassVersionGUID")]
    [StringLength(50)]
    public string? ClassVersionGuid { get; set; }

    [StringLength(100)]
    public string? ClassDefaultObjectType { get; set; }

    public bool? ClassIsForm { get; set; }

    [Column("ClassResourceID")]
    public int? ClassResourceId { get; set; }

    [StringLength(400)]
    public string? ClassCustomizedColumns { get; set; }

    public string? ClassCodeGenerationSettings { get; set; }

    [StringLength(200)]
    public string? ClassIconClass { get; set; }

    [Column("ClassURLPattern")]
    [StringLength(200)]
    public string? ClassUrlpattern { get; set; }

    public bool ClassUsesPageBuilder { get; set; }

    public bool ClassIsNavigationItem { get; set; }

    [Column("ClassHasURL")]
    public bool ClassHasUrl { get; set; }

    public bool ClassHasMetadata { get; set; }

    public int? ClassSearchIndexDataSource { get; set; }

    [ForeignKey("ClassResourceId")]
    [InverseProperty("CmsClasses")]
    public virtual CmsResource? ClassResource { get; set; }

    [InverseProperty("FormClass")]
    public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormClasses { get; set; } = new List<CmsAlternativeForm>();

    [InverseProperty("FormCoupledClass")]
    public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormCoupledClasses { get; set; } = new List<CmsAlternativeForm>();

    [InverseProperty("FormClass")]
    public virtual ICollection<CmsForm> CmsForms { get; set; } = new List<CmsForm>();

    [InverseProperty("Class")]
    public virtual ICollection<CmsPermission> CmsPermissions { get; set; } = new List<CmsPermission>();

    [InverseProperty("Class")]
    public virtual ICollection<CmsQuery> CmsQueries { get; set; } = new List<CmsQuery>();

    [InverseProperty("TransformationClass")]
    public virtual ICollection<CmsTransformation> CmsTransformations { get; set; } = new List<CmsTransformation>();

    [InverseProperty("NodeClass")]
    public virtual ICollection<CmsTree> CmsTrees { get; set; } = new List<CmsTree>();

    [InverseProperty("VersionClass")]
    public virtual ICollection<CmsVersionHistory> CmsVersionHistories { get; set; } = new List<CmsVersionHistory>();

    [InverseProperty("ScopeClass")]
    public virtual ICollection<CmsWorkflowScope> CmsWorkflowScopes { get; set; } = new List<CmsWorkflowScope>();

    [ForeignKey("ParentClassId")]
    [InverseProperty("ParentClasses")]
    public virtual ICollection<CmsClass> ChildClasses { get; set; } = new List<CmsClass>();

    [ForeignKey("ChildClassId")]
    [InverseProperty("ChildClasses")]
    public virtual ICollection<CmsClass> ParentClasses { get; set; } = new List<CmsClass>();

    [ForeignKey("ClassId")]
    [InverseProperty("Classes")]
    public virtual ICollection<CmsDocumentTypeScope> Scopes { get; set; } = new List<CmsDocumentTypeScope>();

    [ForeignKey("ClassId")]
    [InverseProperty("Classes")]
    public virtual ICollection<CmsSite> Sites { get; set; } = new List<CmsSite>();
}
