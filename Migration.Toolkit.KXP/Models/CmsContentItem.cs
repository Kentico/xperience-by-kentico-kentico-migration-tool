using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models;

[Table("CMS_ContentItem")]
[Index("ContentItemContentFolderId", Name = "IX_CMS_ContentItem_ContentItemContentFolderID")]
[Index("ContentItemContentTypeId", Name = "IX_CMS_ContentItem_ContentItemContentTypeID")]
[Index("ContentItemName", Name = "IX_CMS_ContentItem_ContentItemName", IsUnique = true)]
public partial class CmsContentItem
{
    [Key]
    [Column("ContentItemID")]
    public int ContentItemId { get; set; }

    [Column("ContentItemGUID")]
    public Guid ContentItemGuid { get; set; }

    [StringLength(100)]
    public string ContentItemName { get; set; } = null!;

    public bool ContentItemIsReusable { get; set; }

    public bool ContentItemIsSecured { get; set; }

    [Column("ContentItemContentTypeID")]
    public int? ContentItemContentTypeId { get; set; }

    [Column("ContentItemChannelID")]
    public int? ContentItemChannelId { get; set; }

    [Column("ContentItemContentFolderID")]
    public int? ContentItemContentFolderId { get; set; }

    [InverseProperty("ContentItemCommonDataContentItem")]
    public virtual ICollection<CmsContentItemCommonDatum> CmsContentItemCommonData { get; set; } = new List<CmsContentItemCommonDatum>();

    [InverseProperty("ContentItemLanguageMetadataContentItem")]
    public virtual ICollection<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadata { get; set; } = new List<CmsContentItemLanguageMetadatum>();

    [InverseProperty("ContentItemReferenceTargetItem")]
    public virtual ICollection<CmsContentItemReference> CmsContentItemReferences { get; set; } = new List<CmsContentItemReference>();

    [InverseProperty("HeadlessItemContentItem")]
    public virtual ICollection<CmsHeadlessItem> CmsHeadlessItems { get; set; } = new List<CmsHeadlessItem>();

    [InverseProperty("WebPageItemContentItem")]
    public virtual ICollection<CmsWebPageItem> CmsWebPageItems { get; set; } = new List<CmsWebPageItem>();

    [ForeignKey("ContentItemChannelId")]
    [InverseProperty("CmsContentItems")]
    public virtual CmsChannel? ContentItemChannel { get; set; }

    [ForeignKey("ContentItemContentFolderId")]
    [InverseProperty("CmsContentItems")]
    public virtual CmsContentFolder? ContentItemContentFolder { get; set; }

    [ForeignKey("ContentItemContentTypeId")]
    [InverseProperty("CmsContentItems")]
    public virtual CmsClass? ContentItemContentType { get; set; }

    [InverseProperty("EmailConfigurationContentItem")]
    public virtual ICollection<EmailLibraryEmailConfiguration> EmailLibraryEmailConfigurations { get; set; } = new List<EmailLibraryEmailConfiguration>();
}
