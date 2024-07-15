using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public partial class ViewMessagingContactList
{
    [StringLength(200)]
    public string? UserNickName { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(450)]
    public string? FullName { get; set; }

    [Column("ContactListUserID")]
    public int ContactListUserId { get; set; }

    [Column("ContactListContactUserID")]
    public int ContactListContactUserId { get; set; }

    public bool? UserIsHidden { get; set; }

    public bool? UserWaitingForApproval { get; set; }

    public int? UserAccountLockReason { get; set; }

    public bool UserEnabled { get; set; }
}