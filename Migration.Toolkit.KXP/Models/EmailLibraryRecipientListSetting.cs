using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_RecipientListSettings")]
    [Index("RecipientListSettingsRecipientListId", Name = "IX_EmailLibrary_RecipientListSettings_RecipientListSettingsRecipientListID")]
    public partial class EmailLibraryRecipientListSetting
    {
        [Key]
        [Column("RecipientListSettingsID")]
        public int RecipientListSettingsId { get; set; }
        [Column("RecipientListSettingsRecipientListID")]
        public int RecipientListSettingsRecipientListId { get; set; }
        public Guid? RecipientListSettingsAfterUnsubscriptionPage { get; set; }
        public bool RecipientListSettingsSendUnsubscriptionConfirmationEmail { get; set; }
        [Column("RecipientListSettingsUnsubscriptionConfirmationEmailID")]
        public int? RecipientListSettingsUnsubscriptionConfirmationEmailId { get; set; }
        public Guid? RecipientListSettingsAfterConfirmationPage { get; set; }
        public bool RecipientListSettingsSendSubscriptionConfirmationEmail { get; set; }
        [Column("RecipientListSettingsSubscriptionConfirmationEmailID")]
        public int? RecipientListSettingsSubscriptionConfirmationEmailId { get; set; }
        [Column("RecipientListSettingsGUID")]
        public Guid RecipientListSettingsGuid { get; set; }

        [ForeignKey("RecipientListSettingsRecipientListId")]
        [InverseProperty("EmailLibraryRecipientListSettings")]
        public virtual OmContactGroup RecipientListSettingsRecipientList { get; set; } = null!;
    }
}
