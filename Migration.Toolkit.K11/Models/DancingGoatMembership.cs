using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("DancingGoat_Membership")]
public partial class DancingGoatMembership
{
    [Key]
    [Column("MembershipID")]
    public int MembershipId { get; set; }

    [StringLength(200)]
    public string? MembershipPromotionTitle { get; set; }

    [StringLength(200)]
    public string? MembershipPromotionDescription { get; set; }

    [StringLength(200)]
    public string? MembershipBannerText { get; set; }
}
