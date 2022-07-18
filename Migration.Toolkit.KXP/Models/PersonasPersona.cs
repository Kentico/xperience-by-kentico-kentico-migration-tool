namespace Migration.Toolkit.KXP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Personas_Persona")]
    public partial class PersonasPersona
    {
        public PersonasPersona()
        {
            PersonasPersonaContactHistories = new HashSet<PersonasPersonaContactHistory>();
        }

        [Key]
        [Column("PersonaID")]
        public int PersonaId { get; set; }
        [StringLength(200)]
        public string PersonaDisplayName { get; set; } = null!;
        [StringLength(200)]
        public string PersonaName { get; set; } = null!;
        public string? PersonaDescription { get; set; }
        [Required]
        public bool? PersonaEnabled { get; set; }
        [Column("PersonaGUID")]
        public Guid PersonaGuid { get; set; }
        [Column("PersonaPictureMetafileGUID")]
        public Guid? PersonaPictureMetafileGuid { get; set; }
        public int PersonaPointsThreshold { get; set; }

        [InverseProperty("ScorePersona")]
        public virtual OmScore? OmScore { get; set; }
        [InverseProperty("PersonaContactHistoryPersona")]
        public virtual ICollection<PersonasPersonaContactHistory> PersonasPersonaContactHistories { get; set; }
    }
}
