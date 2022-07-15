using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXO.Models
{
    [Table("CMS_ACL")]
    [Index("AclinheritedAcls", Name = "IX_CMS_ACL_ACLInheritedACLs")]
    [Index("AclsiteId", Name = "IX_CMS_ACL_ACLSiteID")]
    public partial class CmsAcl
    {
        public CmsAcl()
        {
            CmsAclitems = new HashSet<CmsAclitem>();
            CmsTrees = new HashSet<CmsTree>();
        }

        [Key]
        [Column("ACLID")]
        public int Aclid { get; set; }
        [Column("ACLInheritedACLs")]
        public string AclinheritedAcls { get; set; } = null!;
        [Column("ACLGUID")]
        public Guid Aclguid { get; set; }
        [Column("ACLLastModified")]
        public DateTime AcllastModified { get; set; }
        [Column("ACLSiteID")]
        public int? AclsiteId { get; set; }

        [ForeignKey("AclsiteId")]
        [InverseProperty("CmsAcls")]
        public virtual CmsSite? Aclsite { get; set; }
        [InverseProperty("Acl")]
        public virtual ICollection<CmsAclitem> CmsAclitems { get; set; }
        [InverseProperty("NodeAcl")]
        public virtual ICollection<CmsTree> CmsTrees { get; set; }
    }
}
