using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_ResourceMetadata")]
    public partial class XperienceResourceMetadatum
    {
        [Key]
        [Column("ResourceMetadataID")]
        public int ResourceMetadataId { get; set; }
        public Guid ResourceMetadataGuid { get; set; }
        public DateTime ResourceMetadataLastModified { get; set; }
        [StringLength(200)]
        public string ResourceMetadataName { get; set; } = null!;
        [StringLength(200)]
        public string ResourceMetadataPublicId { get; set; } = null!;
        [StringLength(100)]
        public string? ResourceMetadataAltText { get; set; }
        [StringLength(100)]
        public string? ResourceMetadataDescription { get; set; }
        public int? ResourceMetadataWidth { get; set; }
        public int? ResourceMetadataHeight { get; set; }
        public double? ResourceMetadataAspectRatio { get; set; }
        public string? ResourceMetadataContextData { get; set; }
    }
}
