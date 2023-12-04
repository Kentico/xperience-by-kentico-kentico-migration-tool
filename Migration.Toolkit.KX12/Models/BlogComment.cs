using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Blog_Comment")]
[Index("CommentApprovedByUserId", Name = "IX_Blog_Comment_CommentApprovedByUserID")]
[Index("CommentPostDocumentId", Name = "IX_Blog_Comment_CommentPostDocumentID")]
[Index("CommentUserId", Name = "IX_Blog_Comment_CommentUserID")]
public partial class BlogComment
{
    [Key]
    [Column("CommentID")]
    public int CommentId { get; set; }

    [StringLength(200)]
    public string CommentUserName { get; set; } = null!;

    [Column("CommentUserID")]
    public int? CommentUserId { get; set; }

    [StringLength(450)]
    public string? CommentUrl { get; set; }

    public string CommentText { get; set; } = null!;

    [Column("CommentApprovedByUserID")]
    public int? CommentApprovedByUserId { get; set; }

    [Column("CommentPostDocumentID")]
    public int CommentPostDocumentId { get; set; }

    public DateTime CommentDate { get; set; }

    public bool? CommentIsSpam { get; set; }

    public bool? CommentApproved { get; set; }

    [StringLength(254)]
    public string? CommentEmail { get; set; }

    public string? CommentInfo { get; set; }

    [Column("CommentGUID")]
    public Guid CommentGuid { get; set; }

    [ForeignKey("CommentApprovedByUserId")]
    [InverseProperty("BlogCommentCommentApprovedByUsers")]
    public virtual CmsUser? CommentApprovedByUser { get; set; }

    [ForeignKey("CommentPostDocumentId")]
    [InverseProperty("BlogComments")]
    public virtual CmsDocument CommentPostDocument { get; set; } = null!;

    [ForeignKey("CommentUserId")]
    [InverseProperty("BlogCommentCommentUsers")]
    public virtual CmsUser? CommentUser { get; set; }
}
