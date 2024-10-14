using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migration.Tool.K11.Models;

[Table("CONTENT_Blog")]
public class ContentBlog
{
    [Key]
    [Column("BlogID")]
    public int BlogId { get; set; }

    [StringLength(200)]
    public string BlogName { get; set; } = null!;

    public string BlogDescription { get; set; } = null!;

    public string? BlogSideColumnText { get; set; }

    public Guid? BlogTeaser { get; set; }

    [StringLength(10)]
    public string BlogOpenCommentsFor { get; set; } = null!;

    public bool? BlogRequireEmails { get; set; }

    [StringLength(254)]
    public string? BlogSendCommentsToEmail { get; set; }

    [Required]
    public bool? BlogAllowAnonymousComments { get; set; }

    [Required]
    [Column("BlogUseCAPTCHAForComments")]
    public bool? BlogUseCaptchaforComments { get; set; }

    public bool BlogModerateComments { get; set; }

    [StringLength(450)]
    public string? BlogModerators { get; set; }

    public bool? BlogEnableSubscriptions { get; set; }

    public int? BlogEnableOptIn { get; set; }

    public int? BlogSendOptInConfirmation { get; set; }

    [StringLength(250)]
    public string? BlogUnsubscriptionUrl { get; set; }

    [Column("BlogOptInApprovalURL")]
    [StringLength(450)]
    public string? BlogOptInApprovalUrl { get; set; }
}
