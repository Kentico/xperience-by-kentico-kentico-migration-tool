using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("COM_TaxClassCountry")]
[Index("CountryId", Name = "IX_COM_TaxClassCountry_CountryID")]
[Index("TaxClassId", "CountryId", Name = "IX_COM_TaxClassCountry_TaxClassID_CountryID", IsUnique = true)]
public partial class ComTaxClassCountry
{
    [Key]
    [Column("TaxClassCountryID")]
    public int TaxClassCountryId { get; set; }

    [Column("TaxClassID")]
    public int TaxClassId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [Column(TypeName = "decimal(18, 9)")]
    public decimal TaxValue { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("ComTaxClassCountries")]
    public virtual CmsCountry Country { get; set; } = null!;

    [ForeignKey("TaxClassId")]
    [InverseProperty("ComTaxClassCountries")]
    public virtual ComTaxClass TaxClass { get; set; } = null!;
}
