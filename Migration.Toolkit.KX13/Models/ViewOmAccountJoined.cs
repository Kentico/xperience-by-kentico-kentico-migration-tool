﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewOmAccountJoined
{
    [Column("AccountID")]
    public int AccountId { get; set; }

    [StringLength(200)]
    public string AccountName { get; set; } = null!;

    [StringLength(100)]
    public string? AccountAddress1 { get; set; }

    [StringLength(100)]
    public string? AccountAddress2 { get; set; }

    [StringLength(100)]
    public string? AccountCity { get; set; }

    [Column("AccountZIP")]
    [StringLength(20)]
    public string? AccountZip { get; set; }

    [Column("AccountStateID")]
    public int? AccountStateId { get; set; }

    [Column("AccountCountryID")]
    public int? AccountCountryId { get; set; }

    [StringLength(200)]
    public string? AccountWebSite { get; set; }

    [StringLength(26)]
    public string? AccountPhone { get; set; }

    [StringLength(254)]
    public string? AccountEmail { get; set; }

    [StringLength(26)]
    public string? AccountFax { get; set; }

    [Column("AccountPrimaryContactID")]
    public int? AccountPrimaryContactId { get; set; }

    [Column("AccountSecondaryContactID")]
    public int? AccountSecondaryContactId { get; set; }

    [Column("AccountStatusID")]
    public int? AccountStatusId { get; set; }

    public string? AccountNotes { get; set; }

    [Column("AccountOwnerUserID")]
    public int? AccountOwnerUserId { get; set; }

    [Column("AccountSubsidiaryOfID")]
    public int? AccountSubsidiaryOfId { get; set; }

    [Column("AccountGUID")]
    public Guid AccountGuid { get; set; }

    public DateTime AccountLastModified { get; set; }

    public DateTime AccountCreated { get; set; }

    [StringLength(100)]
    public string? PrimaryContactFirstName { get; set; }

    [StringLength(100)]
    public string? PrimaryContactMiddleName { get; set; }

    [StringLength(100)]
    public string? PrimaryContactLastName { get; set; }

    [StringLength(100)]
    public string? SecondaryContactFirstName { get; set; }

    [StringLength(100)]
    public string? SecondaryContactMiddleName { get; set; }

    [StringLength(100)]
    public string? SecondaryContactLastName { get; set; }

    [StringLength(200)]
    public string? SubsidiaryOfName { get; set; }

    [StringLength(302)]
    public string PrimaryContactFullName { get; set; } = null!;

    [StringLength(302)]
    public string SecondaryContactFullName { get; set; } = null!;

    [StringLength(201)]
    public string AccountFullAddress { get; set; } = null!;
}
