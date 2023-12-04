﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[PrimaryKey("PersonaId", "NodeId")]
[Table("Personas_PersonaNode")]
[Index("NodeId", Name = "IX_Personas_PersonaNode_NodeID")]
[Index("PersonaId", Name = "IX_Personas_PersonaNode_PersonaID")]
public partial class PersonasPersonaNode
{
    [Key]
    [Column("PersonaID")]
    public int PersonaId { get; set; }

    [Key]
    [Column("NodeID")]
    public int NodeId { get; set; }

    [ForeignKey("NodeId")]
    [InverseProperty("PersonasPersonaNodes")]
    public virtual CmsTree Node { get; set; } = null!;
}
