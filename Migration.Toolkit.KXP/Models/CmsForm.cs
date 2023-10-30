﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Form")]
    [Index("FormClassId", Name = "IX_CMS_Form_FormClassID")]
    public partial class CmsForm
    {
        public CmsForm()
        {
            Roles = new HashSet<CmsRole>();
        }

        [Key]
        [Column("FormID")]
        public int FormId { get; set; }
        [StringLength(100)]
        public string FormDisplayName { get; set; } = null!;
        [StringLength(100)]
        public string FormName { get; set; } = null!;
        [Column("FormClassID")]
        public int FormClassId { get; set; }
        public int FormItems { get; set; }
        public string? FormReportFields { get; set; }
        [StringLength(400)]
        public string? FormSubmitButtonText { get; set; }
        public int? FormAccess { get; set; }
        [StringLength(255)]
        public string? FormSubmitButtonImage { get; set; }
        [Column("FormGUID")]
        public Guid FormGuid { get; set; }
        public DateTime FormLastModified { get; set; }
        [Required]
        public bool? FormLogActivity { get; set; }
        public string? FormBuilderLayout { get; set; }

        [ForeignKey("FormClassId")]
        [InverseProperty("CmsForms")]
        public virtual CmsClass FormClass { get; set; } = null!;

        [ForeignKey("FormId")]
        [InverseProperty("Forms")]
        public virtual ICollection<CmsRole> Roles { get; set; }
    }
}
