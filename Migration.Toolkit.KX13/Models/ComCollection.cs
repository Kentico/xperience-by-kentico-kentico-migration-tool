using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_Collection")]
[Index("CollectionDisplayName", Name = "IX_COM_Collection_CollectionDisplayName")]
[Index("CollectionSiteId", "CollectionEnabled", Name = "IX_COM_Collection_CollectionSiteID_CollectionEnabled")]
public partial class ComCollection
{
    [Key]
    [Column("CollectionID")]
    public int CollectionId { get; set; }

    [StringLength(200)]
    public string CollectionDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string CollectionName { get; set; } = null!;

    public string? CollectionDescription { get; set; }

    [Column("CollectionSiteID")]
    public int CollectionSiteId { get; set; }

    [Required]
    public bool? CollectionEnabled { get; set; }

    public Guid CollectionGuid { get; set; }

    public DateTime CollectionLastModified { get; set; }

    [ForeignKey("CollectionSiteId")]
    [InverseProperty("ComCollections")]
    public virtual CmsSite CollectionSite { get; set; } = null!;

    [InverseProperty("Collection")]
    public virtual ICollection<ComMultiBuyDiscountCollection> ComMultiBuyDiscountCollections { get; set; } = new List<ComMultiBuyDiscountCollection>();

    [InverseProperty("Skucollection")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();
}
