using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("DancingGoatCore_NavigationItem")]
    public partial class DancingGoatCoreNavigationItem
    {
        [Key]
        [Column("NavigationItemID")]
        public int NavigationItemId { get; set; }
        public string LinkTo { get; set; } = null!;
    }
}
