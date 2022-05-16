﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_UIElement")]
    [Index("ElementGuid", Name = "IX_CMS_UIElement_ElementGUID", IsUnique = true)]
    [Index("ElementPageTemplateId", Name = "IX_CMS_UIElement_ElementPageTemplateID")]
    [Index("ElementParentId", Name = "IX_CMS_UIElement_ElementParentID")]
    public partial class CmsUielement
    {
        public CmsUielement()
        {
            CmsHelpTopics = new HashSet<CmsHelpTopic>();
            InverseElementParent = new HashSet<CmsUielement>();
            Roles = new HashSet<CmsRole>();
            RolesNavigation = new HashSet<CmsRole>();
        }

        [Key]
        [Column("ElementID")]
        public int ElementId { get; set; }
        [StringLength(200)]
        public string ElementDisplayName { get; set; } = null!;
        [StringLength(200)]
        public string ElementName { get; set; } = null!;
        [StringLength(200)]
        public string? ElementCaption { get; set; }
        [Column("ElementTargetURL")]
        [StringLength(650)]
        public string? ElementTargetUrl { get; set; }
        [Column("ElementResourceID")]
        public int ElementResourceId { get; set; }
        [Column("ElementParentID")]
        public int? ElementParentId { get; set; }
        public int ElementChildCount { get; set; }
        public int? ElementOrder { get; set; }
        public int ElementLevel { get; set; }
        [Column("ElementIDPath")]
        [StringLength(450)]
        public string ElementIdpath { get; set; } = null!;
        [StringLength(200)]
        public string? ElementIconPath { get; set; }
        public bool? ElementIsCustom { get; set; }
        public DateTime ElementLastModified { get; set; }
        [Column("ElementGUID")]
        public Guid ElementGuid { get; set; }
        public int? ElementSize { get; set; }
        public string? ElementDescription { get; set; }
        [StringLength(20)]
        public string? ElementFromVersion { get; set; }
        [Column("ElementPageTemplateID")]
        public int? ElementPageTemplateId { get; set; }
        [StringLength(50)]
        public string? ElementType { get; set; }
        public string? ElementProperties { get; set; }
        public bool? ElementIsMenu { get; set; }
        [StringLength(200)]
        public string? ElementFeature { get; set; }
        [StringLength(100)]
        public string? ElementIconClass { get; set; }
        public bool? ElementIsGlobalApplication { get; set; }
        public bool? ElementCheckModuleReadPermission { get; set; }
        public string? ElementAccessCondition { get; set; }
        public string? ElementVisibilityCondition { get; set; }
        public bool ElementRequiresGlobalAdminPriviligeLevel { get; set; }

        [ForeignKey("ElementPageTemplateId")]
        [InverseProperty("CmsUielements")]
        public virtual CmsPageTemplate? ElementPageTemplate { get; set; }
        [ForeignKey("ElementParentId")]
        [InverseProperty("InverseElementParent")]
        public virtual CmsUielement? ElementParent { get; set; }
        [ForeignKey("ElementResourceId")]
        [InverseProperty("CmsUielements")]
        public virtual CmsResource ElementResource { get; set; } = null!;
        [InverseProperty("HelpTopicUielement")]
        public virtual ICollection<CmsHelpTopic> CmsHelpTopics { get; set; }
        [InverseProperty("ElementParent")]
        public virtual ICollection<CmsUielement> InverseElementParent { get; set; }

        [ForeignKey("ElementId")]
        [InverseProperty("Elements")]
        public virtual ICollection<CmsRole> Roles { get; set; }
        [ForeignKey("ElementId")]
        [InverseProperty("ElementsNavigation")]
        public virtual ICollection<CmsRole> RolesNavigation { get; set; }
    }
}
