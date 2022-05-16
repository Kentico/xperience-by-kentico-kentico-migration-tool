using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Keyless]
    [Table("tmp_Services")]
    public partial class TmpService
    {
        [Column("ID číslo")]
        public double? IdČíslo { get; set; }
        [Column("Jméno partnera")]
        [StringLength(255)]
        public string? JménoPartnera { get; set; }
        [Column("ulice")]
        [StringLength(255)]
        public string? Ulice { get; set; }
        [Column("město")]
        [StringLength(255)]
        public string? Město { get; set; }
        [Column("psč")]
        [StringLength(255)]
        public string? Psč { get; set; }
        [Column("telefon")]
        [StringLength(255)]
        public string? Telefon { get; set; }
        [Column("email")]
        [StringLength(255)]
        public string? Email { get; set; }
        [Column("souřadnice formát Google")]
        [StringLength(255)]
        public string? SouřadniceFormátGoogle { get; set; }
        [StringLength(255)]
        public string? Kategorie { get; set; }
        [Column("Partner pojišťovny")]
        [StringLength(255)]
        public string? PartnerPojišťovny { get; set; }
        [StringLength(255)]
        public string? Vozidla { get; set; }
        [Column("ALFA ROMEO")]
        [StringLength(255)]
        public string? AlfaRomeo { get; set; }
        [Column("AUDI")]
        [StringLength(255)]
        public string? Audi { get; set; }
        [Column("AVIA")]
        [StringLength(255)]
        public string? Avia { get; set; }
        [Column("BMW")]
        [StringLength(255)]
        public string? Bmw { get; set; }
        [Column("CITROEN")]
        [StringLength(255)]
        public string? Citroen { get; set; }
        [Column("DACIA")]
        [StringLength(255)]
        public string? Dacia { get; set; }
        [Column("DODGE")]
        [StringLength(255)]
        public string? Dodge { get; set; }
        [Column("FIAT")]
        [StringLength(255)]
        public string? Fiat { get; set; }
        [Column("FIAT Professional")]
        [StringLength(255)]
        public string? FiatProfessional { get; set; }
        [Column("FORD")]
        [StringLength(255)]
        public string? Ford { get; set; }
        [Column("HONDA")]
        [StringLength(255)]
        public string? Honda { get; set; }
        [StringLength(255)]
        public string? Hyundai { get; set; }
        [Column("CHEVROLET")]
        [StringLength(255)]
        public string? Chevrolet { get; set; }
        [Column("CHRYSLER")]
        [StringLength(255)]
        public string? Chrysler { get; set; }
        [Column("INFINITY")]
        [StringLength(255)]
        public string? Infinity { get; set; }
        [Column("IVECO")]
        [StringLength(255)]
        public string? Iveco { get; set; }
        [StringLength(255)]
        public string? Jaguar { get; set; }
        [Column("JEEP")]
        [StringLength(255)]
        public string? Jeep { get; set; }
        [Column("KIA")]
        [StringLength(255)]
        public string? Kia { get; set; }
        [StringLength(255)]
        public string? Lancia { get; set; }
        [Column("LAND ROVER")]
        [StringLength(255)]
        public string? LandRover { get; set; }
        [StringLength(255)]
        public string? Lexus { get; set; }
        [Column("MAN")]
        [StringLength(255)]
        public string? Man { get; set; }
        [Column("MAZDA")]
        [StringLength(255)]
        public string? Mazda { get; set; }
        [Column("MERCEDES")]
        [StringLength(255)]
        public string? Mercedes { get; set; }
        [Column("MITSUBISHI")]
        [StringLength(255)]
        public string? Mitsubishi { get; set; }
        [Column("MITSUBISHI-FUSO")]
        [StringLength(255)]
        public string? MitsubishiFuso { get; set; }
        [Column("NISSAN")]
        [StringLength(255)]
        public string? Nissan { get; set; }
        [Column("OPEL")]
        [StringLength(255)]
        public string? Opel { get; set; }
        [Column("PEUGEOT")]
        [StringLength(255)]
        public string? Peugeot { get; set; }
        [Column("PORSCHE")]
        [StringLength(255)]
        public string? Porsche { get; set; }
        [Column("RENAULT")]
        [StringLength(255)]
        public string? Renault { get; set; }
        [Column("SAAB")]
        [StringLength(255)]
        public string? Saab { get; set; }
        [StringLength(255)]
        public string? Scania { get; set; }
        [Column("SEAT")]
        [StringLength(255)]
        public string? Seat { get; set; }
        [StringLength(255)]
        public string? Smart { get; set; }
        [Column("SSANG YONG")]
        [StringLength(255)]
        public string? SsangYong { get; set; }
        [Column("SUBARU")]
        [StringLength(255)]
        public string? Subaru { get; set; }
        [Column("SUZUKI")]
        [StringLength(255)]
        public string? Suzuki { get; set; }
        [Column("ŠKODA")]
        [StringLength(255)]
        public string? Škoda { get; set; }
        [Column("TOYOTA")]
        [StringLength(255)]
        public string? Toyota { get; set; }
        [Column("VOLKSWAGEN")]
        [StringLength(255)]
        public string? Volkswagen { get; set; }
        [Column("VOLKSWAGEN užitkové")]
        [StringLength(255)]
        public string? VolkswagenUžitkové { get; set; }
        [Column("VOLVO")]
        [StringLength(255)]
        public string? Volvo { get; set; }
    }
}
