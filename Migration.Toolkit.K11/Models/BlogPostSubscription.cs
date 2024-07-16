using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11.Models;

[Table("Blog_PostSubscription")]
[Index("SubscriptionPostDocumentId", Name = "IX_Blog_PostSubscription_SubscriptionPostDocumentID")]
[Index("SubscriptionUserId", Name = "IX_Blog_PostSubscription_SubscriptionUserID")]
public class BlogPostSubscription
{
    [Key]
    [Column("SubscriptionID")]
    public int SubscriptionId { get; set; }

    [Column("SubscriptionPostDocumentID")]
    public int SubscriptionPostDocumentId { get; set; }

    [Column("SubscriptionUserID")]
    public int? SubscriptionUserId { get; set; }

    [StringLength(254)]
    public string? SubscriptionEmail { get; set; }

    public DateTime SubscriptionLastModified { get; set; }

    [Column("SubscriptionGUID")]
    public Guid SubscriptionGuid { get; set; }

    public bool? SubscriptionApproved { get; set; }

    [StringLength(100)]
    public string? SubscriptionApprovalHash { get; set; }

    [ForeignKey("SubscriptionPostDocumentId")]
    [InverseProperty("BlogPostSubscriptions")]
    public virtual CmsDocument SubscriptionPostDocument { get; set; } = null!;

    [ForeignKey("SubscriptionUserId")]
    [InverseProperty("BlogPostSubscriptions")]
    public virtual CmsUser? SubscriptionUser { get; set; }
}
