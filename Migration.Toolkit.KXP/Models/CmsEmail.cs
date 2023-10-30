﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Email")]
    [Index("EmailEmailConfigurationId", Name = "IX_CMS_Email_EmailEmailConfigurationID")]
    [Index("EmailPriority", "EmailId", Name = "IX_CMS_Email_EmailPriority_EmailID", IsUnique = true)]
    public partial class CmsEmail
    {
        public CmsEmail()
        {
            Attachments = new HashSet<CmsEmailAttachment>();
        }

        [Key]
        [Column("EmailID")]
        public int EmailId { get; set; }
        [StringLength(254)]
        public string EmailFrom { get; set; } = null!;
        [StringLength(998)]
        public string? EmailTo { get; set; }
        [StringLength(998)]
        public string? EmailCc { get; set; }
        [StringLength(998)]
        public string? EmailBcc { get; set; }
        [StringLength(450)]
        public string EmailSubject { get; set; } = null!;
        public string? EmailBody { get; set; }
        public string? EmailPlainTextBody { get; set; }
        public int EmailFormat { get; set; }
        public int EmailPriority { get; set; }
        public string? EmailLastSendResult { get; set; }
        public DateTime? EmailLastSendAttempt { get; set; }
        [Column("EmailGUID")]
        public Guid EmailGuid { get; set; }
        public int? EmailStatus { get; set; }
        [StringLength(254)]
        public string? EmailReplyTo { get; set; }
        public string? EmailHeaders { get; set; }
        public DateTime? EmailCreated { get; set; }
        [Column("EmailEmailConfigurationID")]
        public int? EmailEmailConfigurationId { get; set; }
        public Guid? EmailMailoutGuid { get; set; }

        [ForeignKey("EmailEmailConfigurationId")]
        [InverseProperty("CmsEmails")]
        public virtual EmailLibraryEmailConfiguration? EmailEmailConfiguration { get; set; }

        [ForeignKey("EmailId")]
        [InverseProperty("Emails")]
        public virtual ICollection<CmsEmailAttachment> Attachments { get; set; }
    }
}
