using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Models;

[Keyless]
public partial class ViewCmsObjectVersionHistoryUserJoined
{
    [Column("VersionID")]
    public int VersionId { get; set; }

    [Column("VersionObjectID")]
    public int? VersionObjectId { get; set; }

    [StringLength(100)]
    public string VersionObjectType { get; set; } = null!;

    [Column("VersionObjectSiteID")]
    public int? VersionObjectSiteId { get; set; }

    [StringLength(450)]
    public string VersionObjectDisplayName { get; set; } = null!;

    [Column("VersionXML")]
    public string VersionXml { get; set; } = null!;

    [Column("VersionBinaryDataXML")]
    public string? VersionBinaryDataXml { get; set; }

    [Column("VersionModifiedByUserID")]
    public int? VersionModifiedByUserId { get; set; }

    public DateTime VersionModifiedWhen { get; set; }

    [Column("VersionDeletedByUserID")]
    public int? VersionDeletedByUserId { get; set; }

    public DateTime? VersionDeletedWhen { get; set; }

    [StringLength(50)]
    public string VersionNumber { get; set; } = null!;

    [Column("VersionSiteBindingIDs")]
    public string? VersionSiteBindingIds { get; set; }

    public string? VersionComment { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [StringLength(100)]
    public string? UserName { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(450)]
    public string? FullName { get; set; }

    [StringLength(254)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? UserPassword { get; set; }

    [StringLength(50)]
    public string? PreferredCultureCode { get; set; }

    [Column("PreferredUICultureCode")]
    [StringLength(50)]
    public string? PreferredUicultureCode { get; set; }

    public bool? UserEnabled { get; set; }

    public bool? UserIsExternal { get; set; }

    [StringLength(10)]
    public string? UserPasswordFormat { get; set; }

    public DateTime? UserCreated { get; set; }

    public DateTime? LastLogon { get; set; }

    [StringLength(200)]
    public string? UserStartingAliasPath { get; set; }

    [Column("UserGUID")]
    public Guid? UserGuid { get; set; }

    public DateTime? UserLastModified { get; set; }

    public string? UserLastLogonInfo { get; set; }

    public bool? UserIsHidden { get; set; }

    public bool? UserIsDomain { get; set; }

    public bool? UserHasAllowedCultures { get; set; }

    [Column("UserMFRequired")]
    public bool? UserMfrequired { get; set; }

    public int? UserPrivilegeLevel { get; set; }

    [StringLength(72)]
    public string? UserSecurityStamp { get; set; }

    [Column("UserMFSecret")]
    public byte[]? UserMfsecret { get; set; }

    [Column("UserMFTimestep")]
    public long? UserMftimestep { get; set; }
}
