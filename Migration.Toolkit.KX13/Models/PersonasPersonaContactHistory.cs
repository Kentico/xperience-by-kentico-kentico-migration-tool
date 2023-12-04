using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("Personas_PersonaContactHistory")]
[Index("PersonaContactHistoryPersonaId", Name = "IX_Personas_PersonaContactHistoryPersonaID")]
public partial class PersonasPersonaContactHistory
{
    [Key]
    [Column("PersonaContactHistoryID")]
    public int PersonaContactHistoryId { get; set; }

    [Column("PersonaContactHistoryPersonaID")]
    public int? PersonaContactHistoryPersonaId { get; set; }

    [Column(TypeName = "date")]
    public DateTime PersonaContactHistoryDate { get; set; }

    public int PersonaContactHistoryContacts { get; set; }

    [ForeignKey("PersonaContactHistoryPersonaId")]
    [InverseProperty("PersonasPersonaContactHistories")]
    public virtual PersonasPersona? PersonaContactHistoryPersona { get; set; }
}
