using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("CMS_AlternativeUrl")]
[Index("AlternativeUrlDocumentId", Name = "IX_CMS_AlternativeUrl_AlternativeUrlDocumentID")]
[Index("AlternativeUrlSiteId", "AlternativeUrlUrl", Name = "IX_CMS_AlternativeUrl_AlternativeUrlSiteID_AlternativeUrlUrl", IsUnique = true)]
public class CmsAlternativeUrl
{
    [Key]
    [Column("AlternativeUrlID")]
    public int AlternativeUrlId { get; set; }

    [Column("AlternativeUrlGUID")]
    public Guid AlternativeUrlGuid { get; set; }

    [Column("AlternativeUrlDocumentID")]
    public int AlternativeUrlDocumentId { get; set; }

    [Column("AlternativeUrlSiteID")]
    public int AlternativeUrlSiteId { get; set; }

    public string AlternativeUrlUrl { get; set; } = null!;

    public DateTime AlternativeUrlLastModified { get; set; }

    [ForeignKey("AlternativeUrlDocumentId")]
    [InverseProperty("CmsAlternativeUrls")]
    public virtual CmsDocument AlternativeUrlDocument { get; set; } = null!;

    [ForeignKey("AlternativeUrlSiteId")]
    [InverseProperty("CmsAlternativeUrls")]
    public virtual CmsSite AlternativeUrlSite { get; set; } = null!;
}
