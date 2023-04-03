using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Class")]
    [Index("ClassName", Name = "IX_CMS_Class_ClassName", IsUnique = true)]
    [Index("ClassName", "ClassGuid", Name = "IX_CMS_Class_ClassName_ClassGUID")]
    [Index("ClassResourceId", Name = "IX_CMS_Class_ClassResourceID")]
    public partial class CmsClass
    {
        public CmsClass()
        {
            CmsAlternativeFormFormClasses = new HashSet<CmsAlternativeForm>();
            CmsAlternativeFormFormCoupledClasses = new HashSet<CmsAlternativeForm>();
            CmsForms = new HashSet<CmsForm>();
            CmsQueries = new HashSet<CmsQuery>();
            CmsTrees = new HashSet<CmsTree>();
            CmsVersionHistories = new HashSet<CmsVersionHistory>();
            CmsWorkflowScopes = new HashSet<CmsWorkflowScope>();
        }

        [Key]
        [Column("ClassID")]
        public int ClassId { get; set; }
        [StringLength(100)]
        public string ClassDisplayName { get; set; } = null!;
        [StringLength(100)]
        public string ClassName { get; set; } = null!;
        public bool ClassIsDocumentType { get; set; }
        public bool ClassIsCoupledClass { get; set; }
        public string ClassXmlSchema { get; set; } = null!;
        public string ClassFormDefinition { get; set; } = null!;
        [StringLength(100)]
        public string ClassNodeNameSource { get; set; } = null!;
        [StringLength(100)]
        public string? ClassTableName { get; set; }
        public bool? ClassShowAsSystemTable { get; set; }
        public bool? ClassUsePublishFromTo { get; set; }
        public bool? ClassShowTemplateSelection { get; set; }
        [StringLength(100)]
        public string? ClassNodeAliasSource { get; set; }
        public DateTime ClassLastModified { get; set; }
        [Column("ClassGUID")]
        public Guid ClassGuid { get; set; }
        [StringLength(1000)]
        public string? ClassShowColumns { get; set; }
        [Column("ClassInheritsFromClassID")]
        public int? ClassInheritsFromClassId { get; set; }
        public string? ClassContactMapping { get; set; }
        public bool? ClassContactOverwriteEnabled { get; set; }
        [StringLength(100)]
        public string? ClassConnectionString { get; set; }
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
        [Column("ClassHasURL")]
        public bool ClassHasUrl { get; set; }
        public bool ClassHasMetadata { get; set; }
        public bool ClassIsPage { get; set; }
        public bool ClassHasUnmanagedDbSchema { get; set; }

        [ForeignKey("ClassResourceId")]
        [InverseProperty("CmsClasses")]
        public virtual CmsResource? ClassResource { get; set; }
        [InverseProperty("FormClass")]
        public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormClasses { get; set; }
        [InverseProperty("FormCoupledClass")]
        public virtual ICollection<CmsAlternativeForm> CmsAlternativeFormFormCoupledClasses { get; set; }
        [InverseProperty("FormClass")]
        public virtual ICollection<CmsForm> CmsForms { get; set; }
        [InverseProperty("Class")]
        public virtual ICollection<CmsQuery> CmsQueries { get; set; }
        [InverseProperty("NodeClass")]
        public virtual ICollection<CmsTree> CmsTrees { get; set; }
        [InverseProperty("VersionClass")]
        public virtual ICollection<CmsVersionHistory> CmsVersionHistories { get; set; }
        [InverseProperty("ScopeClass")]
        public virtual ICollection<CmsWorkflowScope> CmsWorkflowScopes { get; set; }
    }
}
