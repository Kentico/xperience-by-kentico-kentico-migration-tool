using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("Chat_PopupWindowSettings")]
public class ChatPopupWindowSetting
{
    [Key]
    [Column("ChatPopupWindowSettingsID")]
    public int ChatPopupWindowSettingsId { get; set; }

    [StringLength(255)]
    public string MessageTransformationName { get; set; } = null!;

    [StringLength(255)]
    public string ErrorTransformationName { get; set; } = null!;

    [StringLength(255)]
    public string ErrorClearTransformationName { get; set; } = null!;

    [StringLength(255)]
    public string UserTransformationName { get; set; } = null!;

    public int ChatPopupWindowSettingsHashCode { get; set; }
}
