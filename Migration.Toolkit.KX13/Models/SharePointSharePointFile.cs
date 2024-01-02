using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("SharePoint_SharePointFile")]
[Index("SharePointFileSiteId", Name = "IX_SharePoint_SharePointFile_SharePointFileSiteID")]
[Index("SharePointFileSharePointLibraryId", "SharePointFileServerRelativeUrl", Name = "UQ_SharePoint_SharePointFile_LibraryID_ServerRelativeURL", IsUnique = true)]
public partial class SharePointSharePointFile
{
    [Key]
    [Column("SharePointFileID")]
    public int SharePointFileId { get; set; }

    [Column("SharePointFileGUID")]
    public Guid SharePointFileGuid { get; set; }

    [Column("SharePointFileSiteID")]
    public int SharePointFileSiteId { get; set; }

    [StringLength(150)]
    public string SharePointFileName { get; set; } = null!;

    [StringLength(150)]
    public string? SharePointFileExtension { get; set; }

    [StringLength(255)]
    public string? SharePointFileMimeType { get; set; }

    [Column("SharePointFileETag")]
    [StringLength(255)]
    public string? SharePointFileEtag { get; set; }

    public long SharePointFileSize { get; set; }

    public DateTime SharePointFileServerLastModified { get; set; }

    [Column("SharePointFileServerRelativeURL")]
    [StringLength(300)]
    public string SharePointFileServerRelativeUrl { get; set; } = null!;

    [Column("SharePointFileSharePointLibraryID")]
    public int SharePointFileSharePointLibraryId { get; set; }

    public byte[]? SharePointFileBinary { get; set; }

    [ForeignKey("SharePointFileSharePointLibraryId")]
    [InverseProperty("SharePointSharePointFiles")]
    public virtual SharePointSharePointLibrary SharePointFileSharePointLibrary { get; set; } = null!;

    [ForeignKey("SharePointFileSiteId")]
    [InverseProperty("SharePointSharePointFiles")]
    public virtual CmsSite SharePointFileSite { get; set; } = null!;
}
