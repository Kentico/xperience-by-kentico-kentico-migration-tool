using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public partial class ViewCmsRelationshipJoined
{
    [Column("LeftNodeID")]
    public int LeftNodeId { get; set; }

    [Column("LeftNodeGUID")]
    public Guid LeftNodeGuid { get; set; }

    [StringLength(100)]
    public string LeftNodeName { get; set; } = null!;

    [StringLength(200)]
    public string RelationshipName { get; set; } = null!;

    [Column("RelationshipNameID")]
    public int RelationshipNameId { get; set; }

    [Column("RightNodeID")]
    public int RightNodeId { get; set; }

    [Column("RightNodeGUID")]
    public Guid RightNodeGuid { get; set; }

    [StringLength(100)]
    public string RightNodeName { get; set; } = null!;

    [StringLength(200)]
    public string RelationshipDisplayName { get; set; } = null!;

    public string? RelationshipCustomData { get; set; }

    [Column("LeftClassID")]
    public int LeftClassId { get; set; }

    [Column("RightClassID")]
    public int RightClassId { get; set; }

    [Column("RelationshipID")]
    public int RelationshipId { get; set; }

    public int? RelationshipOrder { get; set; }
}