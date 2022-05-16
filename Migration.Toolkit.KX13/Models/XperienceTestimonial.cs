using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_Testimonial")]
    public partial class XperienceTestimonial
    {
        [Key]
        [Column("TestimonialID")]
        public int TestimonialId { get; set; }
        [StringLength(200)]
        public string Name { get; set; } = null!;
        [StringLength(512)]
        public string QuoteHeading { get; set; } = null!;
        [StringLength(512)]
        public string? QuoteSubText { get; set; }
        [StringLength(512)]
        public string Image { get; set; } = null!;
        [StringLength(300)]
        public string Author { get; set; } = null!;
        [StringLength(512)]
        public string? IconImage { get; set; }
    }
}
