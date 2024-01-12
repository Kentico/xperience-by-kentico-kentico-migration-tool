using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("BadWords_Word")]
[Index("WordIsGlobal", Name = "IX_BadWords_Word_WordIsGlobal")]
public partial class BadWordsWord
{
    [Key]
    [Column("WordID")]
    public int WordId { get; set; }

    [Column("WordGUID")]
    public Guid WordGuid { get; set; }

    public DateTime WordLastModified { get; set; }

    [StringLength(200)]
    public string WordExpression { get; set; } = null!;

    [StringLength(200)]
    public string? WordReplacement { get; set; }

    public int? WordAction { get; set; }

    public bool WordIsGlobal { get; set; }

    public bool WordIsRegularExpression { get; set; }

    public bool? WordMatchWholeWord { get; set; }

    [ForeignKey("WordId")]
    [InverseProperty("Words")]
    public virtual ICollection<CmsCulture> Cultures { get; set; } = new List<CmsCulture>();
}
