using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SharePoint_SharePointLibrary")]
[Index("SharePointLibrarySharePointConnectionId", Name = "IX_SharePoint_SharePointLibrary_SharePointLibrarySharepointConnectionID")]
[Index("SharePointLibrarySiteId", Name = "IX_SharePoint_SharePointLibrary_SharePointlibrarySiteID")]
public partial class SharePointSharePointLibrary
{
    [Key]
    [Column("SharePointLibraryID")]
    public int SharePointLibraryId { get; set; }

    [StringLength(100)]
    public string SharePointLibraryName { get; set; } = null!;

    [Column("SharePointLibrarySharePointConnectionID")]
    public int? SharePointLibrarySharePointConnectionId { get; set; }

    [StringLength(100)]
    public string SharePointLibraryListTitle { get; set; } = null!;

    public int SharePointLibrarySynchronizationPeriod { get; set; }

    [Column("SharePointLibraryGUID")]
    public Guid SharePointLibraryGuid { get; set; }

    [Column("SharePointLibrarySiteID")]
    public int SharePointLibrarySiteId { get; set; }

    [StringLength(100)]
    public string SharePointLibraryDisplayName { get; set; } = null!;

    public DateTime SharePointLibraryLastModified { get; set; }

    public int SharePointLibraryListType { get; set; }

    [ForeignKey("SharePointLibrarySharePointConnectionId")]
    [InverseProperty("SharePointSharePointLibraries")]
    public virtual SharePointSharePointConnection? SharePointLibrarySharePointConnection { get; set; }

    [ForeignKey("SharePointLibrarySiteId")]
    [InverseProperty("SharePointSharePointLibraries")]
    public virtual CmsSite SharePointLibrarySite { get; set; } = null!;

    [InverseProperty("SharePointFileSharePointLibrary")]
    public virtual ICollection<SharePointSharePointFile> SharePointSharePointFiles { get; set; } = new List<SharePointSharePointFile>();
}
