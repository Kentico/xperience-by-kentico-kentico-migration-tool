using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Member")]
    public partial class CmsMember
    {
        [Key]
        [Column("MemberID")]
        public int MemberId { get; set; }
        [StringLength(254)]
        public string? MemberEmail { get; set; }
        public bool MemberEnabled { get; set; }
        public DateTime MemberCreated { get; set; }
        public Guid MemberGuid { get; set; }
        [StringLength(254)]
        public string? MemberName { get; set; }
        [StringLength(100)]
        public string? MemberPassword { get; set; }
        public bool MemberIsExternal { get; set; }
        [StringLength(72)]
        public string? MemberSecurityStamp { get; set; }
    }
}
