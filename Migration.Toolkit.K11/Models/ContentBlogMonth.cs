using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("CONTENT_BlogMonth")]
public partial class ContentBlogMonth
{
    [Key]
    [Column("BlogMonthID")]
    public int BlogMonthId { get; set; }

    [StringLength(100)]
    public string BlogMonthName { get; set; } = null!;

    public DateTime BlogMonthStartingDate { get; set; }
}