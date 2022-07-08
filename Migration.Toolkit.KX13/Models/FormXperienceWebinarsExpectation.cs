using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Webinars_Expectation")]
    public partial class FormXperienceWebinarsExpectation
    {
        [Key]
        [Column("Webinars_ExpectationID")]
        public int WebinarsExpectationId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [Column("FormContactGUID")]
        public Guid? FormContactGuid { get; set; }
        [StringLength(200)]
        public string? DocumentFieldComponent { get; set; }
        public string? TextArea { get; set; }
    }
}
