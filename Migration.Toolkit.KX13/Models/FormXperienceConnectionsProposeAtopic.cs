using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_Connections_ProposeATopic")]
    public partial class FormXperienceConnectionsProposeAtopic
    {
        [Key]
        [Column("Connections_ProposeATopicID")]
        public int ConnectionsProposeAtopicId { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [StringLength(500)]
        public string? TextInput { get; set; }
        [StringLength(500)]
        public string? EmailInput { get; set; }
        public string TextArea { get; set; } = null!;
        [StringLength(200)]
        public string RadioButtons { get; set; } = null!;
        [Column("RadioButtons_1")]
        [StringLength(200)]
        public string RadioButtons1 { get; set; } = null!;
        [Column("TextArea_1")]
        public string? TextArea1 { get; set; }
        public Guid? ConsentAgreement { get; set; }
    }
}
