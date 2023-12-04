using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_TaxClassState")]
[Index("StateId", Name = "IX_COM_TaxClassState_StateID")]
[Index("TaxClassId", "StateId", Name = "IX_COM_TaxClassState_TaxClassID_StateID", IsUnique = true)]
public partial class ComTaxClassState
{
    [Key]
    [Column("TaxClassStateID")]
    public int TaxClassStateId { get; set; }

    [Column("TaxClassID")]
    public int TaxClassId { get; set; }

    [Column("StateID")]
    public int StateId { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal TaxValue { get; set; }

    [ForeignKey("StateId")]
    [InverseProperty("ComTaxClassStates")]
    public virtual CmsState State { get; set; } = null!;

    [ForeignKey("TaxClassId")]
    [InverseProperty("ComTaxClassStates")]
    public virtual ComTaxClass TaxClass { get; set; } = null!;
}
