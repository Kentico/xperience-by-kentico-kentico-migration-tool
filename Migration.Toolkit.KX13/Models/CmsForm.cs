using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_Form")]
[Index("FormClassId", Name = "IX_CMS_Form_FormClassID")]
[Index("FormSiteId", Name = "IX_CMS_Form_FormSiteID")]
public partial class CmsForm
{
    [Key]
    [Column("FormID")]
    public int FormId { get; set; }

    [StringLength(100)]
    public string FormDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string FormName { get; set; } = null!;

    [StringLength(998)]
    public string? FormSendToEmail { get; set; }

    [StringLength(254)]
    public string? FormSendFromEmail { get; set; }

    [StringLength(250)]
    public string? FormEmailSubject { get; set; }

    public string? FormEmailTemplate { get; set; }

    public bool? FormEmailAttachUploadedDocs { get; set; }

    [Column("FormClassID")]
    public int FormClassId { get; set; }

    public int FormItems { get; set; }

    public string? FormReportFields { get; set; }

    [StringLength(400)]
    public string? FormRedirectToUrl { get; set; }

    public string? FormDisplayText { get; set; }

    public bool FormClearAfterSave { get; set; }

    [StringLength(400)]
    public string? FormSubmitButtonText { get; set; }

    [Column("FormSiteID")]
    public int FormSiteId { get; set; }

    [StringLength(254)]
    public string? FormConfirmationEmailField { get; set; }

    public string? FormConfirmationTemplate { get; set; }

    [StringLength(254)]
    public string? FormConfirmationSendFromEmail { get; set; }

    [StringLength(250)]
    public string? FormConfirmationEmailSubject { get; set; }

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

    [ForeignKey("FormSiteId")]
    [InverseProperty("CmsForms")]
    public virtual CmsSite FormSite { get; set; } = null!;

    [ForeignKey("FormId")]
    [InverseProperty("Forms")]
    public virtual ICollection<CmsRole> Roles { get; set; } = new List<CmsRole>();
}
