using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Toolkit.KX13.Models
{
    [Table("Xperience_WebsiteSettings")]
    public partial class XperienceWebsiteSetting
    {
        [Key]
        [Column("WebsiteSettingsID")]
        public int WebsiteSettingsId { get; set; }
        public string MainNavigation { get; set; } = null!;
        [StringLength(512)]
        public string? ButtonText { get; set; }
        [StringLength(512)]
        public string? ButtonUrl { get; set; }
        [StringLength(20)]
        public string? ButtonTarget { get; set; }
        public string? FooterText { get; set; }
        public string? FooterNav1 { get; set; }
        public string? FooterNav2 { get; set; }
        public string? FooterNav3 { get; set; }
        public string? FooterNav4 { get; set; }
        [StringLength(200)]
        public string? CopyrightMessage { get; set; }
        [StringLength(200)]
        public string? SmallPrintMenu { get; set; }
        public string CookieBannerMessage { get; set; } = null!;
        [StringLength(200)]
        public string CookieAcceptButton { get; set; } = null!;
        [StringLength(200)]
        public string CookieDeclineButton { get; set; } = null!;
        [StringLength(200)]
        public string CookieRedirectTo { get; set; } = null!;
        [StringLength(512)]
        public string? ButtonText2 { get; set; }
        [StringLength(512)]
        public string? ButtonUrl2 { get; set; }
        [StringLength(20)]
        public string? ButtonTarget2 { get; set; }
        [StringLength(512)]
        public string? ButtonSearchUrl { get; set; }
        [StringLength(512)]
        public string? ButtonSearchTitle { get; set; }
    }
}
