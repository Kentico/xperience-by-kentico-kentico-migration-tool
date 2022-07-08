using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Form_Xperience_SOTM2022")]
    public partial class FormXperienceSotm2022
    {
        [Key]
        [Column("SOTM2022ID")]
        public int Sotm2022id { get; set; }
        public DateTime FormInserted { get; set; }
        public DateTime FormUpdated { get; set; }
        [Column("TextArea_1")]
        public string TextArea1 { get; set; } = null!;
        [StringLength(500)]
        public string TextInput { get; set; } = null!;
        [Column("TextInput_1")]
        [StringLength(500)]
        public string TextInput1 { get; set; } = null!;
        [Column("TextInput_2")]
        [StringLength(500)]
        public string TextInput2 { get; set; } = null!;
        public string TextArea { get; set; } = null!;
        [Column("TextArea_2")]
        public string TextArea2 { get; set; } = null!;
        public Guid? CustomConsentAgreement { get; set; }
        [Column("TextInput_3")]
        [StringLength(500)]
        public string TextInput3 { get; set; } = null!;
        [Column("TextInput_4")]
        [StringLength(500)]
        public string TextInput4 { get; set; } = null!;
        [StringLength(200)]
        public string RadioButtons { get; set; } = null!;
        [Column("RadioButtons_1")]
        [StringLength(200)]
        public string? RadioButtons1 { get; set; }
    }
}
