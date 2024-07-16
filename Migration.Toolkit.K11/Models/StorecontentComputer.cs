using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.K11.Models;

[Table("STORECONTENT_Computer")]
public partial class StorecontentComputer
{
    [Key]
    [Column("ComputerID")]
    public int ComputerId { get; set; }

    [StringLength(200)]
    public string? ComputerProcessor { get; set; }

    [StringLength(200)]
    public string? ComputerMemory { get; set; }

    [StringLength(200)]
    public string? ComputerHardDrive { get; set; }

    [StringLength(200)]
    public string? ComputerOpticalDrive { get; set; }

    [StringLength(300)]
    public string? ComputerGraphicsCard { get; set; }

    [StringLength(500)]
    public string? ComputerInputsOutputs { get; set; }

    [StringLength(100)]
    public string? ComputerOperatingSystem { get; set; }

    [StringLength(200)]
    public string? ComputerDimensions { get; set; }

    public double? ComputerWeight { get; set; }
}
