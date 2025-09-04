using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("STORECONTENT_MediaPlayer")]
public class StorecontentMediaPlayer
{
    [Key]
    [Column("MediaPlayerID")]
    public int MediaPlayerId { get; set; }

    public double? MediaPlayerWeight { get; set; }

    [StringLength(100)]
    public string? MediaPlayerDimensions { get; set; }

    [StringLength(20)]
    public string? MediaPlayerMemoryCapacity { get; set; }

    [StringLength(50)]
    public string? MediaPlayerMemoryType { get; set; }

    [StringLength(100)]
    public string? MediaPlayerMemoryCard { get; set; }

    public bool? MediaPlayerRadio { get; set; }

    [StringLength(200)]
    public string? MediaPlayerSupportedFormats { get; set; }

    [StringLength(100)]
    public string? MediaPlayerDisplay { get; set; }
}
