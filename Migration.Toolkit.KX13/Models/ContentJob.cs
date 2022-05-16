using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("CONTENT_Job")]
    public partial class ContentJob
    {
        [Key]
        [Column("JobID")]
        public int JobId { get; set; }
        [StringLength(200)]
        public string JobName { get; set; } = null!;
        public string? JobSummary { get; set; }
        public string? JobDescription { get; set; }
        public string? JobLocation { get; set; }
        public string? JobCompensation { get; set; }
        public string? JobContact { get; set; }
        public Guid? JobAttachment { get; set; }
    }
}
