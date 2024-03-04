using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public partial class ViewMessagingIgnoreList
{
    [Column("IgnoreListUserID")]
    public int IgnoreListUserId { get; set; }

    [Column("IgnoreListIgnoredUserID")]
    public int IgnoreListIgnoredUserId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(200)]
    public string? UserNickName { get; set; }

    [StringLength(450)]
    public string? FullName { get; set; }
}
