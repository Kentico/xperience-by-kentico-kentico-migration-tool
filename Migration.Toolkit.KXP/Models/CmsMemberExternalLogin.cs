using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_MemberExternalLogin")]
    [Index("MemberExternalLoginLoginProvider", "MemberExternalLoginIdentityKey", Name = "IX_CMS_MemberExternalLogin_MemberExternalLoginLoginProvider_MemberExternalLoginIdentityKey", IsUnique = true)]
    [Index("MemberExternalLoginMemberId", Name = "IX_CMS_MemberExternalLogin_MemberExternalLoginMemberID")]
    public partial class CmsMemberExternalLogin
    {
        [Key]
        [Column("MemberExternalLoginID")]
        public int MemberExternalLoginId { get; set; }
        [Column("MemberExternalLoginMemberID")]
        public int MemberExternalLoginMemberId { get; set; }
        [StringLength(100)]
        public string MemberExternalLoginLoginProvider { get; set; } = null!;
        [StringLength(100)]
        public string MemberExternalLoginIdentityKey { get; set; } = null!;
    }
}
