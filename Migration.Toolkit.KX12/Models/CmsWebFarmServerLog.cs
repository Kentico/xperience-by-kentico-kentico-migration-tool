using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_WebFarmServerLog")]
public class CmsWebFarmServerLog
{
    [Key]
    [Column("WebFarmServerLogID")]
    public int WebFarmServerLogId { get; set; }

    public DateTime LogTime { get; set; }

    [StringLength(200)]
    public string LogCode { get; set; } = null!;

    [Column("ServerID")]
    public int ServerId { get; set; }
}
