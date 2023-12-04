using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_FormUserControl")]
[Index("UserControlCodeName", Name = "IX_CMS_FormUserControl_UserControlCodeName", IsUnique = true)]
[Index("UserControlParentId", Name = "IX_CMS_FormUserControl_UserControlParentID")]
[Index("UserControlResourceId", Name = "IX_CMS_FormUserControl_UserControlResourceID")]
public partial class CmsFormUserControl
{
    [Key]
    [Column("UserControlID")]
    public int UserControlId { get; set; }

    [StringLength(200)]
    public string UserControlDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string UserControlCodeName { get; set; } = null!;

    [StringLength(400)]
    public string UserControlFileName { get; set; } = null!;

    public bool UserControlForText { get; set; }

    public bool UserControlForLongText { get; set; }

    public bool UserControlForInteger { get; set; }

    public bool UserControlForDecimal { get; set; }

    public bool UserControlForDateTime { get; set; }

    public bool UserControlForBoolean { get; set; }

    public bool UserControlForFile { get; set; }

    public bool? UserControlShowInDocumentTypes { get; set; }

    public bool? UserControlShowInSystemTables { get; set; }

    public bool? UserControlShowInWebParts { get; set; }

    public bool? UserControlShowInReports { get; set; }

    [Column("UserControlGUID")]
    public Guid UserControlGuid { get; set; }

    public DateTime UserControlLastModified { get; set; }

    public bool UserControlForGuid { get; set; }

    public bool? UserControlShowInCustomTables { get; set; }

    public string? UserControlParameters { get; set; }

    public bool UserControlForDocAttachments { get; set; }

    [Column("UserControlResourceID")]
    public int? UserControlResourceId { get; set; }

    [Column("UserControlParentID")]
    public int? UserControlParentId { get; set; }

    public string? UserControlDescription { get; set; }

    public int? UserControlPriority { get; set; }

    public bool? UserControlIsSystem { get; set; }

    public bool UserControlForBinary { get; set; }

    public bool UserControlForDocRelationships { get; set; }

    [StringLength(200)]
    public string? UserControlAssemblyName { get; set; }

    [StringLength(200)]
    public string? UserControlClassName { get; set; }

    [InverseProperty("UserControlParent")]
    public virtual ICollection<CmsFormUserControl> InverseUserControlParent { get; set; } = new List<CmsFormUserControl>();

    [ForeignKey("UserControlParentId")]
    [InverseProperty("InverseUserControlParent")]
    public virtual CmsFormUserControl? UserControlParent { get; set; }

    [ForeignKey("UserControlResourceId")]
    [InverseProperty("CmsFormUserControls")]
    public virtual CmsResource? UserControlResource { get; set; }
}
