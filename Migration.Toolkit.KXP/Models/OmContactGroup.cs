using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("OM_ContactGroup")]
public partial class OmContactGroup
{
    [Key]
    [Column("ContactGroupID")]
    public int ContactGroupId { get; set; }

    [StringLength(200)]
    public string ContactGroupName { get; set; } = null!;

    [StringLength(200)]
    public string ContactGroupDisplayName { get; set; } = null!;

    public string? ContactGroupDescription { get; set; }

    public string? ContactGroupDynamicCondition { get; set; }

    public bool? ContactGroupEnabled { get; set; }

    public DateTime? ContactGroupLastModified { get; set; }

    [Column("ContactGroupGUID")]
    public Guid? ContactGroupGuid { get; set; }

    public int? ContactGroupStatus { get; set; }

    public bool? ContactGroupIsRecipientList { get; set; }

    [InverseProperty("EmailSubscriptionConfirmationRecipientList")]
    public virtual ICollection<EmailLibraryEmailSubscriptionConfirmation> EmailLibraryEmailSubscriptionConfirmations { get; set; } = new List<EmailLibraryEmailSubscriptionConfirmation>();

    [InverseProperty("RecipientListSettingsRecipientList")]
    public virtual ICollection<EmailLibraryRecipientListSetting> EmailLibraryRecipientListSettings { get; set; } = new List<EmailLibraryRecipientListSetting>();

    [InverseProperty("SendConfigurationRecipientList")]
    public virtual ICollection<EmailLibrarySendConfiguration> EmailLibrarySendConfigurations { get; set; } = new List<EmailLibrarySendConfiguration>();

    [InverseProperty("ContactGroupMemberContactGroup")]
    public virtual ICollection<OmContactGroupMember> OmContactGroupMembers { get; set; } = new List<OmContactGroupMember>();
}
