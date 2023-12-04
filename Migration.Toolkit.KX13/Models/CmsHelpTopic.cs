using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_HelpTopic")]
[Index("HelpTopicUielementId", Name = "IX_CMS_HelpTopic_HelpTopicUIElementID")]
public partial class CmsHelpTopic
{
    [Key]
    [Column("HelpTopicID")]
    public int HelpTopicId { get; set; }

    [Column("HelpTopicUIElementID")]
    public int HelpTopicUielementId { get; set; }

    [StringLength(200)]
    public string HelpTopicName { get; set; } = null!;

    [StringLength(1023)]
    public string HelpTopicLink { get; set; } = null!;

    public DateTime HelpTopicLastModified { get; set; }

    [Column("HelpTopicGUID")]
    public Guid HelpTopicGuid { get; set; }

    public int? HelpTopicOrder { get; set; }

    public string? HelpTopicVisibilityCondition { get; set; }

    [ForeignKey("HelpTopicUielementId")]
    [InverseProperty("CmsHelpTopics")]
    public virtual CmsUielement HelpTopicUielement { get; set; } = null!;
}
