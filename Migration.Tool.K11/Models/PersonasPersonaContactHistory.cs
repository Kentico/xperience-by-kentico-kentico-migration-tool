using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("Personas_PersonaContactHistory")]
[Index("PersonaContactHistoryPersonaId", Name = "IX_Personas_PersonaContactHistoryPersonaID")]
public class PersonasPersonaContactHistory
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
