using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailMarketingRecipient")]
    [Index("EmailMarketingRecipientContactId", Name = "IX_EmailLibrary_EmailMarketingRecipient_EmailMarketingRecipientContactID")]
    [Index("EmailMarketingRecipientEmailConfigurationId", Name = "IX_EmailLibrary_EmailMarketingRecipient_EmailMarketingRecipientEmailConfigurationID")]
    public partial class EmailLibraryEmailMarketingRecipient
    {
        [Key]
        [Column("EmailMarketingRecipientID")]
        public int EmailMarketingRecipientId { get; set; }
        [Column("EmailMarketingRecipientEmailConfigurationID")]
        public int EmailMarketingRecipientEmailConfigurationId { get; set; }
        [Column("EmailMarketingRecipientContactID")]
        public int EmailMarketingRecipientContactId { get; set; }
        [StringLength(254)]
        public string EmailMarketingRecipientContactEmail { get; set; } = null!;
        public int EmailMarketingRecipientStatus { get; set; }
        [StringLength(500)]
        public string? EmailMarketingRecipientErrorMessage { get; set; }
        public DateTime EmailMarketingRecipientLastModified { get; set; }
        public int EmailMarketingRecipientRetryAttempt { get; set; }

        [ForeignKey("EmailMarketingRecipientContactId")]
        [InverseProperty("EmailLibraryEmailMarketingRecipients")]
        public virtual OmContact EmailMarketingRecipientContact { get; set; } = null!;
        [ForeignKey("EmailMarketingRecipientEmailConfigurationId")]
        [InverseProperty("EmailLibraryEmailMarketingRecipients")]
        public virtual EmailLibraryEmailConfiguration EmailMarketingRecipientEmailConfiguration { get; set; } = null!;
    }
}
