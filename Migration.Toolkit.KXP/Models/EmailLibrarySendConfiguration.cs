using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("EmailLibrary_SendConfiguration")]
    [Index("SendConfigurationEmailConfigurationId", Name = "IX_EmailLibrary_SendConfiguration_SendConfigurationEmailConfigurationID")]
    [Index("SendConfigurationRecipientListId", Name = "IX_EmailLibrary_SendConfiguration_SendConfigurationRecipientListID")]
    [Index("SendConfigurationEmailConfigurationId", Name = "UQ_EmailLibrary_SendConfiguration_SendConfigurationEmailConfigurationID", IsUnique = true)]
    public partial class EmailLibrarySendConfiguration
    {
        [Key]
        [Column("SendConfigurationID")]
        public int SendConfigurationId { get; set; }
        [Column("SendConfigurationEmailConfigurationID")]
        public int SendConfigurationEmailConfigurationId { get; set; }
        [Column("SendConfigurationRecipientListID")]
        public int SendConfigurationRecipientListId { get; set; }
        public DateTime? SendConfigurationScheduledTime { get; set; }
        [Column("SendConfigurationGUID")]
        public Guid SendConfigurationGuid { get; set; }
        public int SendConfigurationStatus { get; set; }

        [ForeignKey("SendConfigurationEmailConfigurationId")]
        [InverseProperty("EmailLibrarySendConfiguration")]
        public virtual EmailLibraryEmailConfiguration SendConfigurationEmailConfiguration { get; set; } = null!;
        [ForeignKey("SendConfigurationRecipientListId")]
        [InverseProperty("EmailLibrarySendConfigurations")]
        public virtual OmContactGroup SendConfigurationRecipientList { get; set; } = null!;
    }
}
