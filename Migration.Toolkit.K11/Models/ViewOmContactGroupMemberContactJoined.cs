using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Keyless]
public partial class ViewOmContactGroupMemberContactJoined
{
    [Column("ContactID")]
    public int ContactId { get; set; }

    [StringLength(100)]
    public string? ContactFirstName { get; set; }

    [StringLength(100)]
    public string? ContactMiddleName { get; set; }

    [StringLength(100)]
    public string? ContactLastName { get; set; }

    [StringLength(50)]
    public string? ContactJobTitle { get; set; }

    [StringLength(100)]
    public string? ContactAddress1 { get; set; }

    [StringLength(100)]
    public string? ContactCity { get; set; }

    [Column("ContactZIP")]
    [StringLength(100)]
    public string? ContactZip { get; set; }

    [Column("ContactStateID")]
    public int? ContactStateId { get; set; }

    [Column("ContactCountryID")]
    public int? ContactCountryId { get; set; }

    [StringLength(26)]
    public string? ContactMobilePhone { get; set; }

    [StringLength(26)]
    public string? ContactBusinessPhone { get; set; }

    [StringLength(254)]
    public string? ContactEmail { get; set; }

    public DateTime? ContactBirthday { get; set; }

    public int? ContactGender { get; set; }

    [Column("ContactStatusID")]
    public int? ContactStatusId { get; set; }

    public string? ContactNotes { get; set; }

    [Column("ContactOwnerUserID")]
    public int? ContactOwnerUserId { get; set; }

    public bool? ContactMonitored { get; set; }

    [Column("ContactGUID")]
    public Guid ContactGuid { get; set; }

    public DateTime ContactLastModified { get; set; }

    public DateTime ContactCreated { get; set; }

    public int? ContactBounces { get; set; }

    [StringLength(200)]
    public string? ContactCampaign { get; set; }

    [Column("ContactGroupMemberContactGroupID")]
    public int ContactGroupMemberContactGroupId { get; set; }

    public bool? ContactGroupMemberFromCondition { get; set; }

    public bool? ContactGroupMemberFromAccount { get; set; }

    public bool? ContactGroupMemberFromManual { get; set; }

    [Column("ContactGroupMemberID")]
    public int ContactGroupMemberId { get; set; }

    [StringLength(200)]
    public string ContactGroupDisplayName { get; set; } = null!;

    [StringLength(200)]
    public string ContactGroupName { get; set; } = null!;

    [Column("ContactGroupID")]
    public int ContactGroupId { get; set; }

    [StringLength(100)]
    public string? ContactCompanyName { get; set; }
}