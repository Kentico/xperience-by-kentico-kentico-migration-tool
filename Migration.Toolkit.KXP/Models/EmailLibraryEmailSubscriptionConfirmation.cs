using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailSubscriptionConfirmation")]
    [Index("EmailSubscriptionConfirmationContactId", Name = "IX_EmailLibrary_EmailSubscriptionConfirmation_EmailSubscriptionConfirmationContactID")]
    [Index("EmailSubscriptionConfirmationRecipientListId", Name = "IX_EmailLibrary_EmailSubscriptionConfirmation_EmailSubscriptionConfirmationRecipientListID")]
    public partial class EmailLibraryEmailSubscriptionConfirmation
    {
        [Key]
        [Column("EmailSubscriptionConfirmationID")]
        public int EmailSubscriptionConfirmationId { get; set; }
        [Column("EmailSubscriptionConfirmationContactID")]
        public int EmailSubscriptionConfirmationContactId { get; set; }
        [Column("EmailSubscriptionConfirmationRecipientListID")]
        public int EmailSubscriptionConfirmationRecipientListId { get; set; }
        public bool EmailSubscriptionConfirmationIsApproved { get; set; }
        public DateTime EmailSubscriptionConfirmationDate { get; set; }
        [Column("EmailSubscriptionConfirmationGUID")]
        public Guid EmailSubscriptionConfirmationGuid { get; set; }

        [ForeignKey("EmailSubscriptionConfirmationContactId")]
        [InverseProperty("EmailLibraryEmailSubscriptionConfirmations")]
        public virtual OmContact EmailSubscriptionConfirmationContact { get; set; } = null!;
        [ForeignKey("EmailSubscriptionConfirmationRecipientListId")]
        [InverseProperty("EmailLibraryEmailSubscriptionConfirmations")]
        public virtual OmContactGroup EmailSubscriptionConfirmationRecipientList { get; set; } = null!;
    }
}
