using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_EmailChannelSender")]
    [Index("EmailChannelSenderEmailChannelId", Name = "IX_EmailLibrary_EmailChannelSender_EmailChannelSenderEmailChannelID")]
    public partial class EmailLibraryEmailChannelSender
    {
        [Key]
        [Column("EmailChannelSenderID")]
        public int EmailChannelSenderId { get; set; }
        [Column("EmailChannelSenderEmailChannelID")]
        public int EmailChannelSenderEmailChannelId { get; set; }
        [StringLength(250)]
        public string EmailChannelSenderName { get; set; } = null!;
        [StringLength(250)]
        public string EmailChannelSenderDisplayName { get; set; } = null!;
        [Column("EmailChannelSenderGUID")]
        public Guid EmailChannelSenderGuid { get; set; }
        public DateTime EmailChannelSenderCreated { get; set; }

        [ForeignKey("EmailChannelSenderEmailChannelId")]
        [InverseProperty("EmailLibraryEmailChannelSenders")]
        public virtual EmailLibraryEmailChannel EmailChannelSenderEmailChannel { get; set; } = null!;
    }
}
