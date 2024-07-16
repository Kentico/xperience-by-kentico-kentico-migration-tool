using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_MacroRuleCategory")]
public class CmsMacroRuleCategory
{
    [Key]
    [Column("MacroRuleCategoryID")]
    public int MacroRuleCategoryId { get; set; }

    public Guid MacroRuleCategoryGuid { get; set; }

    public DateTime MacroRuleCategoryLastModified { get; set; }

    [StringLength(200)]
    public string MacroRuleCategoryName { get; set; } = null!;

    [StringLength(200)]
    public string MacroRuleCategoryDisplayName { get; set; } = null!;

    [StringLength(450)]
    public string? MacroRuleCategoryDescription { get; set; }

    public bool MacroRuleCategoryEnabled { get; set; }

    [InverseProperty("MacroRuleCategory")]
    public virtual ICollection<CmsMacroRuleMacroRuleCategory> CmsMacroRuleMacroRuleCategories { get; set; } = new List<CmsMacroRuleMacroRuleCategory>();
}
