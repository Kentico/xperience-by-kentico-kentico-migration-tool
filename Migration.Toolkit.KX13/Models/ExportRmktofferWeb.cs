using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models
{
    [Keyless]
    [Table("export_RMKTOffer_web")]
    public partial class ExportRmktofferWeb
    {
        [Column("RowID")]
        public int RowId { get; set; }
        [Column("CaseID")]
        public int? CaseId { get; set; }
        [Column("PublicID")]
        [StringLength(20)]
        [Unicode(false)]
        public string? PublicId { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? CarCategory { get; set; }
        public int? ManufactureYear { get; set; }
        public int? ManufactureMonth { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Manufacturer { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Model { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? ModelFullName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Fuel { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? GearBox { get; set; }
        [Column("PriceInclVAT", TypeName = "decimal(19, 5)")]
        public decimal? PriceInclVat { get; set; }
        [Column("PriceVATReclaimable")]
        public bool? PriceVatreclaimable { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateOfFirstRegistration { get; set; }
        public int? Mileage { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? BodyType { get; set; }
        public int? Doors { get; set; }
        public int? Seats { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Color { get; set; }
        public int? Cubature { get; set; }
        public int? Cylinders { get; set; }
        public int? PowerKw { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? EmissionClass { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? DriveType { get; set; }
        public bool? ServiceBook { get; set; }
        public bool? FirstOwner { get; set; }
        [Column("CZOrigin")]
        public bool? Czorigin { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Roof { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? SunRoof { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Lights { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Parking { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Clima { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Cruise { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? SeatHeating { get; set; }
        [Column("hasABS")]
        public bool? HasAbs { get; set; }
        [Column("isAdaptedForDisabled")]
        public bool? IsAdaptedForDisabled { get; set; }
        [Column("hasAlarm")]
        public bool? HasAlarm { get; set; }
        [Column("hasASR")]
        public bool? HasAsr { get; set; }
        [Column("hasAutoDifferentialLock")]
        public bool? HasAutoDifferentialLock { get; set; }
        [Column("hasAuxiliaryHeating")]
        public bool? HasAuxiliaryHeating { get; set; }
        [Column("hasESP")]
        public bool? HasEsp { get; set; }
        [Column("hasFogLights")]
        public bool? HasFogLights { get; set; }
        [Column("hasHeatedFrontShield")]
        public bool? HasHeatedFrontShield { get; set; }
        [Column("hasLeatherInterior")]
        public bool? HasLeatherInterior { get; set; }
        [Column("hasNavigation")]
        public bool? HasNavigation { get; set; }
        [Column("hasPDS")]
        public bool? HasPds { get; set; }
        [Column("hasTrailerCoupling")]
        public bool? HasTrailerCoupling { get; set; }
        [Unicode(false)]
        public string? Configuration { get; set; }
        [Column("displayWeb")]
        public bool? DisplayWeb { get; set; }
        [Column("updatedAt", TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [Column("validFrom", TypeName = "datetime")]
        public DateTime ValidFrom { get; set; }
        [Column("validTo", TypeName = "datetime")]
        public DateTime? ValidTo { get; set; }
        [Column(TypeName = "decimal(19, 5)")]
        public decimal? OriginalListPrice { get; set; }
    }
}
