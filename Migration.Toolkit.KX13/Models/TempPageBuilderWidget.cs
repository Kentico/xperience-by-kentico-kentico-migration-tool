using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models;

[Table("Temp_PageBuilderWidgets")]
public class TempPageBuilderWidget
{
    [Key]
    [Column("PageBuilderWidgetsID")]
    public int PageBuilderWidgetsId { get; set; }

    public string? PageBuilderWidgetsConfiguration { get; set; }

    public Guid PageBuilderWidgetsGuid { get; set; }

    public DateTime PageBuilderWidgetsLastModified { get; set; }

    public string? PageBuilderTemplateConfiguration { get; set; }
}
