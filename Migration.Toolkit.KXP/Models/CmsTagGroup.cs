namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("CMS_TagGroup")]
    [Index("TagGroupSiteId", Name = "IX_CMS_TagGroup_TagGroupSiteID")]
    public partial class CmsTagGroup
    {
        public CmsTagGroup()
        {
            CmsDocuments = new HashSet<CmsDocument>();
            CmsTags = new HashSet<CmsTag>();
        }

        [Key]
        [Column("TagGroupID")]
        public int TagGroupId { get; set; }
        [StringLength(250)]
        public string TagGroupDisplayName { get; set; } = null!;
        [StringLength(250)]
        public string TagGroupName { get; set; } = null!;
        public string? TagGroupDescription { get; set; }
        [Column("TagGroupSiteID")]
        public int TagGroupSiteId { get; set; }
        public bool TagGroupIsAdHoc { get; set; }
        public DateTime TagGroupLastModified { get; set; }
        [Column("TagGroupGUID")]
        public Guid TagGroupGuid { get; set; }

        [ForeignKey("TagGroupSiteId")]
        [InverseProperty("CmsTagGroups")]
        public virtual CmsSite TagGroupSite { get; set; } = null!;
        [InverseProperty("DocumentTagGroup")]
        public virtual ICollection<CmsDocument> CmsDocuments { get; set; }
        [InverseProperty("TagGroup")]
        public virtual ICollection<CmsTag> CmsTags { get; set; }
    }
}
