using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_Personalization")]
[Index("PersonalizationId", "PersonalizationUserId", "PersonalizationDocumentId", "PersonalizationDashboardName", Name = "IX_CMS_Personalization_PersonalizationID_PersonalizationUserID_PersonalizationDocumentID_PersonalizationDashboardName", IsUnique = true)]
[Index("PersonalizationSiteId", Name = "IX_CMS_Personalization_PersonalizationSiteID_SiteID")]
[Index("PersonalizationUserId", Name = "IX_CMS_Personalization_PersonalizationUserID")]
public partial class CmsPersonalization
{
    [Key]
    [Column("PersonalizationID")]
    public int PersonalizationId { get; set; }

    [Column("PersonalizationGUID")]
    public Guid PersonalizationGuid { get; set; }

    public DateTime PersonalizationLastModified { get; set; }

    [Column("PersonalizationUserID")]
    public int? PersonalizationUserId { get; set; }

    [Column("PersonalizationDocumentID")]
    public int? PersonalizationDocumentId { get; set; }

    public string? PersonalizationWebParts { get; set; }

    [StringLength(200)]
    public string? PersonalizationDashboardName { get; set; }

    [Column("PersonalizationSiteID")]
    public int? PersonalizationSiteId { get; set; }

    [ForeignKey("PersonalizationDocumentId")]
    [InverseProperty("CmsPersonalizations")]
    public virtual CmsDocument? PersonalizationDocument { get; set; }

    [ForeignKey("PersonalizationSiteId")]
    [InverseProperty("CmsPersonalizations")]
    public virtual CmsSite? PersonalizationSite { get; set; }

    [ForeignKey("PersonalizationUserId")]
    [InverseProperty("CmsPersonalizations")]
    public virtual CmsUser? PersonalizationUser { get; set; }
}
