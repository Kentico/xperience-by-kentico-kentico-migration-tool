using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_CustomQueries")]
    public partial class XperienceCustomQuery
    {
        [Key]
        [Column("CustomQueriesID")]
        public int CustomQueriesId { get; set; }
        public Guid CustomQueriesGuid { get; set; }
        public DateTime CustomQueriesLastModified { get; set; }
    }
}
