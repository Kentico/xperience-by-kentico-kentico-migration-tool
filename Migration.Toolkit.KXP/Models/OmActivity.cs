using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("OM_Activity")]
    [Index("ActivityChannelId", Name = "IX_OM_Activity_ActivityChannelID")]
    [Index("ActivityContactId", Name = "IX_OM_Activity_ActivityContactID")]
    [Index("ActivityCreated", Name = "IX_OM_Activity_ActivityCreated")]
    [Index("ActivityItemDetailId", Name = "IX_OM_Activity_ActivityItemDetailID")]
    [Index("ActivityLanguageId", Name = "IX_OM_Activity_ActivityLanguageID")]
    [Index("ActivityType", "ActivityItemId", "ActivityWebPageItemGuid", Name = "IX_OM_Activity_ActivityType_ActivityItemID_ActivityWebPageItemGUID_ActivityUTMSource_ActivityUTMContent")]
    public partial class OmActivity
    {
        [Key]
        [Column("ActivityID")]
        public int ActivityId { get; set; }
        [Column("ActivityContactID")]
        public int ActivityContactId { get; set; }
        public DateTime? ActivityCreated { get; set; }
        [StringLength(250)]
        public string ActivityType { get; set; } = null!;
        [Column("ActivityItemID")]
        public int? ActivityItemId { get; set; }
        [Column("ActivityItemDetailID")]
        public int? ActivityItemDetailId { get; set; }
        [StringLength(250)]
        public string? ActivityValue { get; set; }
        [Column("ActivityURL")]
        public string? ActivityUrl { get; set; }
        [StringLength(250)]
        public string? ActivityTitle { get; set; }
        public string? ActivityComment { get; set; }
        [Column("ActivityURLReferrer")]
        public string? ActivityUrlreferrer { get; set; }
        [Column("ActivityUTMSource")]
        [StringLength(200)]
        public string? ActivityUtmsource { get; set; }
        [Column("ActivityUTMContent")]
        [StringLength(200)]
        public string? ActivityUtmcontent { get; set; }
        [Column("ActivityTrackedWebsiteID")]
        public int? ActivityTrackedWebsiteId { get; set; }
        [Column("ActivityWebPageItemGUID")]
        public Guid? ActivityWebPageItemGuid { get; set; }
        [Column("ActivityLanguageID")]
        public int? ActivityLanguageId { get; set; }
        [Column("ActivityChannelID")]
        public int? ActivityChannelId { get; set; }

        [ForeignKey("ActivityChannelId")]
        [InverseProperty("OmActivities")]
        public virtual CmsChannel? ActivityChannel { get; set; }
        [ForeignKey("ActivityLanguageId")]
        [InverseProperty("OmActivities")]
        public virtual CmsContentLanguage? ActivityLanguage { get; set; }
    }
}
