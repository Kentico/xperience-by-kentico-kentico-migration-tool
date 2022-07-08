using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Organisations")]
    public partial class XperienceOrganisation
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        public DateTime? ItemCreatedWhen { get; set; }
        public DateTime? ItemModifiedWhen { get; set; }
        [Column("ItemGUID")]
        public Guid ItemGuid { get; set; }
        [StringLength(512)]
        public string OrganisationName { get; set; } = null!;
        [StringLength(512)]
        public string OrganisationLogo { get; set; } = null!;
    }
}
