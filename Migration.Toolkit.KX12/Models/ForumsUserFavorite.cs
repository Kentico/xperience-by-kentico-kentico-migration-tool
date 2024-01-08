using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("Forums_UserFavorites")]
[Index("ForumId", Name = "IX_Forums_UserFavorites_ForumID")]
[Index("PostId", Name = "IX_Forums_UserFavorites_PostID")]
[Index("SiteId", Name = "IX_Forums_UserFavorites_SiteID")]
[Index("UserId", Name = "IX_Forums_UserFavorites_UserID")]
[Index("UserId", "PostId", "ForumId", Name = "IX_Forums_UserFavorites_UserID_PostID_ForumID", IsUnique = true)]
public partial class ForumsUserFavorite
{
    [Key]
    [Column("FavoriteID")]
    public int FavoriteId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("PostID")]
    public int? PostId { get; set; }

    [Column("ForumID")]
    public int? ForumId { get; set; }

    [StringLength(100)]
    public string? FavoriteName { get; set; }

    [Column("SiteID")]
    public int SiteId { get; set; }

    [Column("FavoriteGUID")]
    public Guid FavoriteGuid { get; set; }

    public DateTime FavoriteLastModified { get; set; }

    [ForeignKey("ForumId")]
    [InverseProperty("ForumsUserFavorites")]
    public virtual ForumsForum? Forum { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("ForumsUserFavorites")]
    public virtual ForumsForumPost? Post { get; set; }

    [ForeignKey("SiteId")]
    [InverseProperty("ForumsUserFavorites")]
    public virtual CmsSite Site { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ForumsUserFavorites")]
    public virtual CmsUser User { get; set; } = null!;
}
