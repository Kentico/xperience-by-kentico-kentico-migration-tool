using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.KXP.Models;

[Keyless]
public class ViewCmsResourceTranslatedJoined
{
    [Column("StringID")]
    public int StringId { get; set; }

    [StringLength(200)]
    public string StringKey { get; set; } = null!;

    public string? TranslationText { get; set; }

    [Column("CultureID")]
    public int CultureId { get; set; }

    [StringLength(200)]
    public string CultureName { get; set; } = null!;

    [StringLength(50)]
    public string CultureCode { get; set; } = null!;
}
