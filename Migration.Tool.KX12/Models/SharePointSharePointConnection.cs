using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KX12.Models;

[Table("SharePoint_SharePointConnection")]
[Index("SharePointConnectionSiteId", Name = "IX_SharePoint_SharePointConnection_SharePointConnectionSiteID")]
public class SharePointSharePointConnection
{
    [Key]
    [Column("SharePointConnectionID")]
    public int SharePointConnectionId { get; set; }

    [Column("SharePointConnectionGUID")]
    public Guid SharePointConnectionGuid { get; set; }

    [Column("SharePointConnectionSiteID")]
    public int SharePointConnectionSiteId { get; set; }

    [StringLength(512)]
    public string SharePointConnectionSiteUrl { get; set; } = null!;

    [StringLength(30)]
    public string SharePointConnectionAuthMode { get; set; } = null!;

    [StringLength(100)]
    public string SharePointConnectionDisplayName { get; set; } = null!;

    [StringLength(100)]
    public string SharePointConnectionName { get; set; } = null!;

    [StringLength(30)]
    public string SharePointConnectionSharePointVersion { get; set; } = null!;

    [StringLength(100)]
    public string? SharePointConnectionUserName { get; set; }

    [StringLength(100)]
    public string? SharePointConnectionPassword { get; set; }

    [StringLength(100)]
    public string? SharePointConnectionDomain { get; set; }

    public DateTime SharePointConnectionLastModified { get; set; }

    [ForeignKey("SharePointConnectionSiteId")]
    [InverseProperty("SharePointSharePointConnections")]
    public virtual CmsSite SharePointConnectionSite { get; set; } = null!;

    [InverseProperty("SharePointLibrarySharePointConnection")]
    public virtual ICollection<SharePointSharePointLibrary> SharePointSharePointLibraries { get; set; } = new List<SharePointSharePointLibrary>();
}
