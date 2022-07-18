namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("CMS_ModuleLicenseKey")]
    [Index("ModuleLicenseKeyResourceId", Name = "IX_CMS_ModuleLicenseKey_ModuleLicenseKeyResourceID")]
    public partial class CmsModuleLicenseKey
    {
        [Key]
        [Column("ModuleLicenseKeyID")]
        public int ModuleLicenseKeyId { get; set; }
        public Guid ModuleLicenseKeyGuid { get; set; }
        public DateTime ModuleLicenseKeyLastModified { get; set; }
        public string ModuleLicenseKeyLicense { get; set; } = null!;
        [Column("ModuleLicenseKeyResourceID")]
        public int ModuleLicenseKeyResourceId { get; set; }

        [ForeignKey("ModuleLicenseKeyResourceId")]
        [InverseProperty("CmsModuleLicenseKeys")]
        public virtual CmsResource ModuleLicenseKeyResource { get; set; } = null!;
    }
}
