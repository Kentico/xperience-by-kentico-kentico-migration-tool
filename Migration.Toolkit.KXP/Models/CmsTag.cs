namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("CMS_Tag")]
    [Index("TagGroupId", Name = "IX_CMS_Tag_TagGroupID")]
    public partial class CmsTag
    {
        public CmsTag()
        {
            Documents = new HashSet<CmsDocument>();
        }

        [Key]
        [Column("TagID")]
        public int TagId { get; set; }
        [StringLength(250)]
        public string TagName { get; set; } = null!;
        public int TagCount { get; set; }
        [Column("TagGroupID")]
        public int TagGroupId { get; set; }
        [Column("TagGUID")]
        public Guid TagGuid { get; set; }

        [ForeignKey("TagGroupId")]
        [InverseProperty("CmsTags")]
        public virtual CmsTagGroup TagGroup { get; set; } = null!;

        [ForeignKey("TagId")]
        [InverseProperty("Tags")]
        public virtual ICollection<CmsDocument> Documents { get; set; }
    }
}
