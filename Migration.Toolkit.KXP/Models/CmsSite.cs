using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXP.Models
{
    [Table("CMS_Site")]
    [Index("SiteDomainName", "SiteStatus", Name = "IX_CMS_Site_SiteDomainName_SiteStatus")]
    [Index("SiteName", Name = "IX_CMS_Site_SiteName")]
    public partial class CmsSite
    {
        public CmsSite()
        {
            CmsAlternativeUrls = new HashSet<CmsAlternativeUrl>();
            CmsAutomationStates = new HashSet<CmsAutomationState>();
            CmsContentRelationshipItems = new HashSet<CmsContentRelationshipItem>();
            CmsContentRelationships = new HashSet<CmsContentRelationship>();
            CmsForms = new HashSet<CmsForm>();
            CmsMetaFiles = new HashSet<CmsMetaFile>();
            CmsPageFormerUrlPaths = new HashSet<CmsPageFormerUrlPath>();
            CmsPageTemplateConfigurations = new HashSet<CmsPageTemplateConfiguration>();
            CmsPageUrlPaths = new HashSet<CmsPageUrlPath>();
            CmsPersonalizations = new HashSet<CmsPersonalization>();
            CmsRoles = new HashSet<CmsRole>();
            CmsScheduledTasks = new HashSet<CmsScheduledTask>();
            CmsSettingsKeys = new HashSet<CmsSettingsKey>();
            CmsSiteDomainAliases = new HashSet<CmsSiteDomainAlias>();
            CmsTreeNodeLinkedNodeSites = new HashSet<CmsTree>();
            CmsTreeNodeSites = new HashSet<CmsTree>();
            CmsVersionHistories = new HashSet<CmsVersionHistory>();
            CmsWorkflowScopes = new HashSet<CmsWorkflowScope>();
            EmailLibraryEmailConfigurations = new HashSet<EmailLibraryEmailConfiguration>();
            EmailLibraryEmailTemplates = new HashSet<EmailLibraryEmailTemplate>();
            MediaFiles = new HashSet<MediaFile>();
            MediaLibraries = new HashSet<MediaLibrary>();
            OmAbtests = new HashSet<OmAbtest>();
            OmTrackedWebsites = new HashSet<OmTrackedWebsite>();
            Cultures = new HashSet<CmsCulture>();
        }

        [Key]
        [Column("SiteID")]
        public int SiteId { get; set; }
        [StringLength(100)]
        public string SiteName { get; set; } = null!;
        [StringLength(200)]
        public string SiteDisplayName { get; set; } = null!;
        public string? SiteDescription { get; set; }
        [StringLength(20)]
        public string SiteStatus { get; set; } = null!;
        [StringLength(400)]
        public string SiteDomainName { get; set; } = null!;
        [StringLength(50)]
        public string? SiteDefaultVisitorCulture { get; set; }
        [Column("SiteGUID")]
        public Guid SiteGuid { get; set; }
        public DateTime SiteLastModified { get; set; }

        [InverseProperty("AlternativeUrlSite")]
        public virtual ICollection<CmsAlternativeUrl> CmsAlternativeUrls { get; set; }
        [InverseProperty("StateSite")]
        public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; }
        [InverseProperty("ContentRelationshipItemSite")]
        public virtual ICollection<CmsContentRelationshipItem> CmsContentRelationshipItems { get; set; }
        [InverseProperty("ContentRelationshipSite")]
        public virtual ICollection<CmsContentRelationship> CmsContentRelationships { get; set; }
        [InverseProperty("FormSite")]
        public virtual ICollection<CmsForm> CmsForms { get; set; }
        [InverseProperty("MetaFileSite")]
        public virtual ICollection<CmsMetaFile> CmsMetaFiles { get; set; }
        [InverseProperty("PageFormerUrlPathSite")]
        public virtual ICollection<CmsPageFormerUrlPath> CmsPageFormerUrlPaths { get; set; }
        [InverseProperty("PageTemplateConfigurationSite")]
        public virtual ICollection<CmsPageTemplateConfiguration> CmsPageTemplateConfigurations { get; set; }
        [InverseProperty("PageUrlPathSite")]
        public virtual ICollection<CmsPageUrlPath> CmsPageUrlPaths { get; set; }
        [InverseProperty("PersonalizationSite")]
        public virtual ICollection<CmsPersonalization> CmsPersonalizations { get; set; }
        [InverseProperty("Site")]
        public virtual ICollection<CmsRole> CmsRoles { get; set; }
        [InverseProperty("TaskSite")]
        public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; }
        [InverseProperty("Site")]
        public virtual ICollection<CmsSettingsKey> CmsSettingsKeys { get; set; }
        [InverseProperty("Site")]
        public virtual ICollection<CmsSiteDomainAlias> CmsSiteDomainAliases { get; set; }
        [InverseProperty("NodeLinkedNodeSite")]
        public virtual ICollection<CmsTree> CmsTreeNodeLinkedNodeSites { get; set; }
        [InverseProperty("NodeSite")]
        public virtual ICollection<CmsTree> CmsTreeNodeSites { get; set; }
        [InverseProperty("NodeSite")]
        public virtual ICollection<CmsVersionHistory> CmsVersionHistories { get; set; }
        [InverseProperty("ScopeSite")]
        public virtual ICollection<CmsWorkflowScope> CmsWorkflowScopes { get; set; }
        [InverseProperty("EmailConfigurationSite")]
        public virtual ICollection<EmailLibraryEmailConfiguration> EmailLibraryEmailConfigurations { get; set; }
        [InverseProperty("EmailTemplateSite")]
        public virtual ICollection<EmailLibraryEmailTemplate> EmailLibraryEmailTemplates { get; set; }
        [InverseProperty("FileSite")]
        public virtual ICollection<MediaFile> MediaFiles { get; set; }
        [InverseProperty("LibrarySite")]
        public virtual ICollection<MediaLibrary> MediaLibraries { get; set; }
        [InverseProperty("AbtestSite")]
        public virtual ICollection<OmAbtest> OmAbtests { get; set; }
        [InverseProperty("TrackedWebsiteSite")]
        public virtual ICollection<OmTrackedWebsite> OmTrackedWebsites { get; set; }

        [ForeignKey("SiteId")]
        [InverseProperty("Sites")]
        public virtual ICollection<CmsCulture> Cultures { get; set; }
    }
}
