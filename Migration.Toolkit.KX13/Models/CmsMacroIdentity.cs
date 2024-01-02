using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_MacroIdentity")]
[Index("MacroIdentityEffectiveUserId", Name = "IX_CMS_MacroIdentity_MacroIdentityEffectiveUserID")]
public partial class CmsMacroIdentity
{
    [Key]
    [Column("MacroIdentityID")]
    public int MacroIdentityId { get; set; }

    public Guid MacroIdentityGuid { get; set; }

    public DateTime MacroIdentityLastModified { get; set; }

    [StringLength(200)]
    public string MacroIdentityName { get; set; } = null!;

    [Column("MacroIdentityEffectiveUserID")]
    public int? MacroIdentityEffectiveUserId { get; set; }

    [InverseProperty("UserMacroIdentityMacroIdentity")]
    public virtual ICollection<CmsUserMacroIdentity> CmsUserMacroIdentities { get; set; } = new List<CmsUserMacroIdentity>();

    [ForeignKey("MacroIdentityEffectiveUserId")]
    [InverseProperty("CmsMacroIdentities")]
    public virtual CmsUser? MacroIdentityEffectiveUser { get; set; }
}
