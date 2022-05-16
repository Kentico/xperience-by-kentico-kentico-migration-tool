using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Keyless]
    [Table("OM_ContactGroup_backup")]
    public partial class OmContactGroupBackup
    {
        [Column("ContactGroupID")]
        public int ContactGroupId { get; set; }
        [StringLength(200)]
        public string ContactGroupName { get; set; } = null!;
        [StringLength(200)]
        public string ContactGroupDisplayName { get; set; } = null!;
        public string? ContactGroupDescription { get; set; }
        public string? ContactGroupDynamicCondition { get; set; }
        public bool? ContactGroupEnabled { get; set; }
        public DateTime? ContactGroupLastModified { get; set; }
        [Column("ContactGroupGUID")]
        public Guid? ContactGroupGuid { get; set; }
        public int? ContactGroupStatus { get; set; }
    }
}
