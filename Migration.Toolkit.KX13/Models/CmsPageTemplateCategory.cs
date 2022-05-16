﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_PageTemplateCategory")]
    [Index("CategoryLevel", Name = "IX_CMS_PageTemplateCategory_CategoryLevel")]
    [Index("CategoryParentId", Name = "IX_CMS_PageTemplateCategory_CategoryParentID")]
    public partial class CmsPageTemplateCategory
    {
        public CmsPageTemplateCategory()
        {
            CmsPageTemplates = new HashSet<CmsPageTemplate>();
            InverseCategoryParent = new HashSet<CmsPageTemplateCategory>();
        }

        [Key]
        [Column("CategoryID")]
        public int CategoryId { get; set; }
        [StringLength(200)]
        public string CategoryDisplayName { get; set; } = null!;
        [Column("CategoryParentID")]
        public int? CategoryParentId { get; set; }
        [StringLength(200)]
        public string? CategoryName { get; set; }
        [Column("CategoryGUID")]
        public Guid CategoryGuid { get; set; }
        public DateTime CategoryLastModified { get; set; }
        [StringLength(450)]
        public string? CategoryImagePath { get; set; }
        public int? CategoryChildCount { get; set; }
        public int? CategoryTemplateChildCount { get; set; }
        public string? CategoryPath { get; set; }
        public int? CategoryLevel { get; set; }

        [ForeignKey("CategoryParentId")]
        [InverseProperty("InverseCategoryParent")]
        public virtual CmsPageTemplateCategory? CategoryParent { get; set; }
        [InverseProperty("PageTemplateCategory")]
        public virtual ICollection<CmsPageTemplate> CmsPageTemplates { get; set; }
        [InverseProperty("CategoryParent")]
        public virtual ICollection<CmsPageTemplateCategory> InverseCategoryParent { get; set; }
    }
}
