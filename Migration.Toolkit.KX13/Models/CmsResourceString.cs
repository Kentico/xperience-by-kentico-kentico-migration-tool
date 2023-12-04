﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Table("CMS_ResourceString")]
[Index("StringKey", Name = "IX_CMS_ResourceString_StringKey")]
public partial class CmsResourceString
{
    [Key]
    [Column("StringID")]
    public int StringId { get; set; }

    [StringLength(200)]
    public string StringKey { get; set; } = null!;

    public bool StringIsCustom { get; set; }

    [Column("StringGUID")]
    public Guid StringGuid { get; set; }

    [InverseProperty("TranslationString")]
    public virtual ICollection<CmsResourceTranslation> CmsResourceTranslations { get; set; } = new List<CmsResourceTranslation>();
}
