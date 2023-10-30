using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_WebsiteCaptchaSettings")]
    public partial class CmsWebsiteCaptchaSetting
    {
        [Key]
        [Column("WebsiteCaptchaSettingsID")]
        public int WebsiteCaptchaSettingsId { get; set; }
        [Column("WebsiteCaptchaSettingsWebsiteChannelID")]
        public int WebsiteCaptchaSettingsWebsiteChannelId { get; set; }
        [StringLength(200)]
        public string WebsiteCaptchaSettingsReCaptchaSiteKey { get; set; } = null!;
        [StringLength(200)]
        public string WebsiteCaptchaSettingsReCaptchaSecretKey { get; set; } = null!;
        public double? WebsiteCaptchaSettingsReCaptchaThreshold { get; set; }
        public int WebsiteCaptchaSettingsReCaptchaVersion { get; set; }
        [Column("WebsiteCaptchaSettingsGUID")]
        public Guid WebsiteCaptchaSettingsGuid { get; set; }
    }
}
