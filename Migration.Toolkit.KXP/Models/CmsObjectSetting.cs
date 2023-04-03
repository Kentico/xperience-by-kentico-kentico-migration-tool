using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_ObjectSettings")]
    [Index("ObjectSettingsObjectId", "ObjectSettingsObjectType", Name = "IX_CMS_ObjectSettings_ObjectSettingsObjectType_ObjectSettingsObjectID", IsUnique = true)]
    [Index("ObjectWorkflowStepId", Name = "IX_CMS_ObjectSettings_ObjectWorkflowStepID")]
    public partial class CmsObjectSetting
    {
        [Key]
        [Column("ObjectSettingsID")]
        public int ObjectSettingsId { get; set; }
        public string? ObjectTags { get; set; }
        [Column("ObjectWorkflowStepID")]
        public int? ObjectWorkflowStepId { get; set; }
        [Column("ObjectSettingsObjectID")]
        public int ObjectSettingsObjectId { get; set; }
        [StringLength(100)]
        public string ObjectSettingsObjectType { get; set; } = null!;
        public string? ObjectComments { get; set; }
        public bool? ObjectWorkflowSendEmails { get; set; }

        [ForeignKey("ObjectWorkflowStepId")]
        [InverseProperty("CmsObjectSettings")]
        public virtual CmsWorkflowStep? ObjectWorkflowStep { get; set; }
    }
}
