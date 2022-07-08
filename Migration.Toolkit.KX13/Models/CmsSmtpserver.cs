using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CMS_SMTPServer")]
    public partial class CmsSmtpserver
    {
        public CmsSmtpserver()
        {
            Sites = new HashSet<CmsSite>();
        }

        [Key]
        [Column("ServerID")]
        public int ServerId { get; set; }
        [StringLength(200)]
        public string ServerName { get; set; } = null!;
        [StringLength(50)]
        public string? ServerUserName { get; set; }
        [StringLength(200)]
        public string? ServerPassword { get; set; }
        [Column("ServerUseSSL")]
        public bool ServerUseSsl { get; set; }
        public bool ServerEnabled { get; set; }
        public bool ServerIsGlobal { get; set; }
        [Column("ServerGUID")]
        public Guid ServerGuid { get; set; }
        public DateTime ServerLastModified { get; set; }
        public int? ServerPriority { get; set; }
        public int? ServerDeliveryMethod { get; set; }
        [StringLength(450)]
        public string? ServerPickupDirectory { get; set; }

        [ForeignKey("ServerId")]
        [InverseProperty("Servers")]
        public virtual ICollection<CmsSite> Sites { get; set; }
    }
}
