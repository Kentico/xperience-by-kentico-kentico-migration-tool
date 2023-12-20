using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_SKUFile")]
[Index("FileSkuid", Name = "IX_COM_SKUFile_FileSKUID")]
public partial class ComSkufile
{
    [Key]
    [Column("FileID")]
    public int FileId { get; set; }

    [Column("FileGUID")]
    public Guid FileGuid { get; set; }

    [Column("FileSKUID")]
    public int FileSkuid { get; set; }

    [StringLength(450)]
    public string FilePath { get; set; } = null!;

    [StringLength(50)]
    public string FileType { get; set; } = null!;

    public DateTime FileLastModified { get; set; }

    [StringLength(250)]
    public string FileName { get; set; } = null!;

    [Column("FileMetaFileGUID")]
    public Guid? FileMetaFileGuid { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<ComOrderItemSkufile> ComOrderItemSkufiles { get; set; } = new List<ComOrderItemSkufile>();

    [ForeignKey("FileSkuid")]
    [InverseProperty("ComSkufiles")]
    public virtual ComSku FileSku { get; set; } = null!;
}
