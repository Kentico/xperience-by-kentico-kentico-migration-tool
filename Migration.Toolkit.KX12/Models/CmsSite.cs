using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX12.Models;

[Table("CMS_Site")]
[Index("SiteDefaultEditorStylesheet", Name = "IX_CMS_Site_SiteDefaultEditorStylesheet")]
[Index("SiteDefaultStylesheetId", Name = "IX_CMS_Site_SiteDefaultStylesheetID")]
[Index("SiteDomainName", "SiteStatus", Name = "IX_CMS_Site_SiteDomainName_SiteStatus")]
[Index("SiteName", Name = "IX_CMS_Site_SiteName")]
public partial class CmsSite
{
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

    [Column("SiteDefaultStylesheetID")]
    public int? SiteDefaultStylesheetId { get; set; }

    [StringLength(50)]
    public string? SiteDefaultVisitorCulture { get; set; }

    public int? SiteDefaultEditorStylesheet { get; set; }

    [Column("SiteGUID")]
    public Guid SiteGuid { get; set; }

    public DateTime SiteLastModified { get; set; }

    public bool? SiteIsOffline { get; set; }

    [Column("SiteOfflineRedirectURL")]
    [StringLength(400)]
    public string? SiteOfflineRedirectUrl { get; set; }

    public string? SiteOfflineMessage { get; set; }

    [Column("SitePresentationURL")]
    [StringLength(400)]
    public string? SitePresentationUrl { get; set; }

    public bool? SiteIsContentOnly { get; set; }

    [InverseProperty("CampaignSite")]
    public virtual ICollection<AnalyticsCampaign> AnalyticsCampaigns { get; set; } = new List<AnalyticsCampaign>();

    [InverseProperty("ConversionSite")]
    public virtual ICollection<AnalyticsConversion> AnalyticsConversions { get; set; } = new List<AnalyticsConversion>();

    [InverseProperty("StatisticsSite")]
    public virtual ICollection<AnalyticsStatistic> AnalyticsStatistics { get; set; } = new List<AnalyticsStatistic>();

    [InverseProperty("BoardSite")]
    public virtual ICollection<BoardBoard> BoardBoards { get; set; } = new List<BoardBoard>();

    [InverseProperty("ChatNotificationSite")]
    public virtual ICollection<ChatNotification> ChatNotifications { get; set; } = new List<ChatNotification>();

    [InverseProperty("ChatOnlineSupportSite")]
    public virtual ICollection<ChatOnlineSupport> ChatOnlineSupports { get; set; } = new List<ChatOnlineSupport>();

    [InverseProperty("ChatOnlineUserSite")]
    public virtual ICollection<ChatOnlineUser> ChatOnlineUsers { get; set; } = new List<ChatOnlineUser>();

    [InverseProperty("ChatRoomSite")]
    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    [InverseProperty("ChatSupportCannedResponseSite")]
    public virtual ICollection<ChatSupportCannedResponse> ChatSupportCannedResponses { get; set; } = new List<ChatSupportCannedResponse>();

    [InverseProperty("ReportSite")]
    public virtual ICollection<CmsAbuseReport> CmsAbuseReports { get; set; } = new List<CmsAbuseReport>();

    [InverseProperty("Aclsite")]
    public virtual ICollection<CmsAcl> CmsAcls { get; set; } = new List<CmsAcl>();

    [InverseProperty("AlternativeUrlSite")]
    public virtual ICollection<CmsAlternativeUrl> CmsAlternativeUrls { get; set; } = new List<CmsAlternativeUrl>();

    [InverseProperty("AttachmentSite")]
    public virtual ICollection<CmsAttachmentHistory> CmsAttachmentHistories { get; set; } = new List<CmsAttachmentHistory>();

    [InverseProperty("AttachmentSite")]
    public virtual ICollection<CmsAttachment> CmsAttachments { get; set; } = new List<CmsAttachment>();

    [InverseProperty("StateSite")]
    public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; } = new List<CmsAutomationState>();

    [InverseProperty("IpaddressSite")]
    public virtual ICollection<CmsBannedIp> CmsBannedIps { get; set; } = new List<CmsBannedIp>();

    [InverseProperty("BannerCategorySite")]
    public virtual ICollection<CmsBannerCategory> CmsBannerCategories { get; set; } = new List<CmsBannerCategory>();

    [InverseProperty("BannerSite")]
    public virtual ICollection<CmsBanner> CmsBanners { get; set; } = new List<CmsBanner>();

    [InverseProperty("CategorySite")]
    public virtual ICollection<CmsCategory> CmsCategories { get; set; } = new List<CmsCategory>();

    [InverseProperty("AliasSite")]
    public virtual ICollection<CmsDocumentAlias> CmsDocumentAliases { get; set; } = new List<CmsDocumentAlias>();

    [InverseProperty("ScopeSite")]
    public virtual ICollection<CmsDocumentTypeScope> CmsDocumentTypeScopes { get; set; } = new List<CmsDocumentTypeScope>();

    [InverseProperty("EmailTemplateSite")]
    public virtual ICollection<CmsEmailTemplate> CmsEmailTemplates { get; set; } = new List<CmsEmailTemplate>();

    [InverseProperty("FormSite")]
    public virtual ICollection<CmsForm> CmsForms { get; set; } = new List<CmsForm>();

    [InverseProperty("MembershipSite")]
    public virtual ICollection<CmsMembership> CmsMemberships { get; set; } = new List<CmsMembership>();

    [InverseProperty("MetaFileSite")]
    public virtual ICollection<CmsMetaFile> CmsMetaFiles { get; set; } = new List<CmsMetaFile>();

    [InverseProperty("VersionObjectSite")]
    public virtual ICollection<CmsObjectVersionHistory> CmsObjectVersionHistories { get; set; } = new List<CmsObjectVersionHistory>();

    [InverseProperty("PageTemplateConfigurationSite")]
    public virtual ICollection<CmsPageTemplateConfiguration> CmsPageTemplateConfigurations { get; set; } = new List<CmsPageTemplateConfiguration>();

    [InverseProperty("PageTemplateScopeSite")]
    public virtual ICollection<CmsPageTemplateScope> CmsPageTemplateScopes { get; set; } = new List<CmsPageTemplateScope>();

    [InverseProperty("PageTemplateSite")]
    public virtual ICollection<CmsPageTemplate> CmsPageTemplates { get; set; } = new List<CmsPageTemplate>();

    [InverseProperty("PersonalizationSite")]
    public virtual ICollection<CmsPersonalization> CmsPersonalizations { get; set; } = new List<CmsPersonalization>();

    [InverseProperty("Site")]
    public virtual ICollection<CmsRole> CmsRoles { get; set; } = new List<CmsRole>();

    [InverseProperty("TaskSite")]
    public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; } = new List<CmsScheduledTask>();

    [InverseProperty("SessionSite")]
    public virtual ICollection<CmsSession> CmsSessions { get; set; } = new List<CmsSession>();

    [InverseProperty("Site")]
    public virtual ICollection<CmsSettingsKey> CmsSettingsKeys { get; set; } = new List<CmsSettingsKey>();

    [InverseProperty("Site")]
    public virtual ICollection<CmsSiteDomainAlias> CmsSiteDomainAliases { get; set; } = new List<CmsSiteDomainAlias>();

    [InverseProperty("TagGroupSite")]
    public virtual ICollection<CmsTagGroup> CmsTagGroups { get; set; } = new List<CmsTagGroup>();

    [InverseProperty("NodeLinkedNodeSite")]
    public virtual ICollection<CmsTree> CmsTreeNodeLinkedNodeSites { get; set; } = new List<CmsTree>();

    [InverseProperty("NodeSite")]
    public virtual ICollection<CmsTree> CmsTreeNodeSites { get; set; } = new List<CmsTree>();

    [InverseProperty("Site")]
    public virtual ICollection<CmsUserCulture> CmsUserCultures { get; set; } = new List<CmsUserCulture>();

    [InverseProperty("Site")]
    public virtual ICollection<CmsUserSite> CmsUserSites { get; set; } = new List<CmsUserSite>();

    [InverseProperty("NodeSite")]
    public virtual ICollection<CmsVersionHistory> CmsVersionHistories { get; set; } = new List<CmsVersionHistory>();

    [InverseProperty("ScopeSite")]
    public virtual ICollection<CmsWorkflowScope> CmsWorkflowScopes { get; set; } = new List<CmsWorkflowScope>();

    [InverseProperty("BrandSite")]
    public virtual ICollection<ComBrand> ComBrands { get; set; } = new List<ComBrand>();

    [InverseProperty("CarrierSite")]
    public virtual ICollection<ComCarrier> ComCarriers { get; set; } = new List<ComCarrier>();

    [InverseProperty("CollectionSite")]
    public virtual ICollection<ComCollection> ComCollections { get; set; } = new List<ComCollection>();

    [InverseProperty("CurrencySite")]
    public virtual ICollection<ComCurrency> ComCurrencies { get; set; } = new List<ComCurrency>();

    [InverseProperty("EventSite")]
    public virtual ICollection<ComCustomerCreditHistory> ComCustomerCreditHistories { get; set; } = new List<ComCustomerCreditHistory>();

    [InverseProperty("CustomerSite")]
    public virtual ICollection<ComCustomer> ComCustomers { get; set; } = new List<ComCustomer>();

    [InverseProperty("DepartmentSite")]
    public virtual ICollection<ComDepartment> ComDepartments { get; set; } = new List<ComDepartment>();

    [InverseProperty("DiscountSite")]
    public virtual ICollection<ComDiscount> ComDiscounts { get; set; } = new List<ComDiscount>();

    [InverseProperty("ExchangeTableSite")]
    public virtual ICollection<ComExchangeTable> ComExchangeTables { get; set; } = new List<ComExchangeTable>();

    [InverseProperty("GiftCardSite")]
    public virtual ICollection<ComGiftCard> ComGiftCards { get; set; } = new List<ComGiftCard>();

    [InverseProperty("InternalStatusSite")]
    public virtual ICollection<ComInternalStatus> ComInternalStatuses { get; set; } = new List<ComInternalStatus>();

    [InverseProperty("ManufacturerSite")]
    public virtual ICollection<ComManufacturer> ComManufacturers { get; set; } = new List<ComManufacturer>();

    [InverseProperty("MultiBuyDiscountSite")]
    public virtual ICollection<ComMultiBuyDiscount> ComMultiBuyDiscounts { get; set; } = new List<ComMultiBuyDiscount>();

    [InverseProperty("CategorySite")]
    public virtual ICollection<ComOptionCategory> ComOptionCategories { get; set; } = new List<ComOptionCategory>();

    [InverseProperty("StatusSite")]
    public virtual ICollection<ComOrderStatus> ComOrderStatuses { get; set; } = new List<ComOrderStatus>();

    [InverseProperty("OrderSite")]
    public virtual ICollection<ComOrder> ComOrders { get; set; } = new List<ComOrder>();

    [InverseProperty("PaymentOptionSite")]
    public virtual ICollection<ComPaymentOption> ComPaymentOptions { get; set; } = new List<ComPaymentOption>();

    [InverseProperty("PublicStatusSite")]
    public virtual ICollection<ComPublicStatus> ComPublicStatuses { get; set; } = new List<ComPublicStatus>();

    [InverseProperty("ShippingOptionSite")]
    public virtual ICollection<ComShippingOption> ComShippingOptions { get; set; } = new List<ComShippingOption>();

    [InverseProperty("ShoppingCartSite")]
    public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; } = new List<ComShoppingCart>();

    [InverseProperty("Skusite")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = new List<ComSku>();

    [InverseProperty("SupplierSite")]
    public virtual ICollection<ComSupplier> ComSuppliers { get; set; } = new List<ComSupplier>();

    [InverseProperty("TaxClassSite")]
    public virtual ICollection<ComTaxClass> ComTaxClasses { get; set; } = new List<ComTaxClass>();

    [InverseProperty("Site")]
    public virtual ICollection<ComWishlist> ComWishlists { get; set; } = new List<ComWishlist>();

    [InverseProperty("GroupSite")]
    public virtual ICollection<CommunityGroup> CommunityGroups { get; set; } = new List<CommunityGroup>();

    [InverseProperty("ExportSite")]
    public virtual ICollection<ExportHistory> ExportHistories { get; set; } = new List<ExportHistory>();

    [InverseProperty("TaskSite")]
    public virtual ICollection<ExportTask> ExportTasks { get; set; } = new List<ExportTask>();

    [InverseProperty("AttachmentSite")]
    public virtual ICollection<ForumsAttachment> ForumsAttachments { get; set; } = new List<ForumsAttachment>();

    [InverseProperty("GroupSite")]
    public virtual ICollection<ForumsForumGroup> ForumsForumGroups { get; set; } = new List<ForumsForumGroup>();

    [InverseProperty("ForumSite")]
    public virtual ICollection<ForumsForum> ForumsForums { get; set; } = new List<ForumsForum>();

    [InverseProperty("Site")]
    public virtual ICollection<ForumsUserFavorite> ForumsUserFavorites { get; set; } = new List<ForumsUserFavorite>();

    [InverseProperty("TaskSite")]
    public virtual ICollection<IntegrationTask> IntegrationTasks { get; set; } = new List<IntegrationTask>();

    [InverseProperty("FileSite")]
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    [InverseProperty("LibrarySite")]
    public virtual ICollection<MediaLibrary> MediaLibraries { get; set; } = new List<MediaLibrary>();

    [InverseProperty("TemplateSite")]
    public virtual ICollection<NewsletterEmailTemplate> NewsletterEmailTemplates { get; set; } = new List<NewsletterEmailTemplate>();

    [InverseProperty("EmailWidgetSite")]
    public virtual ICollection<NewsletterEmailWidget> NewsletterEmailWidgets { get; set; } = new List<NewsletterEmailWidget>();

    [InverseProperty("EmailSite")]
    public virtual ICollection<NewsletterEmail> NewsletterEmails { get; set; } = new List<NewsletterEmail>();

    [InverseProperty("IssueSite")]
    public virtual ICollection<NewsletterNewsletterIssue> NewsletterNewsletterIssues { get; set; } = new List<NewsletterNewsletterIssue>();

    [InverseProperty("NewsletterSite")]
    public virtual ICollection<NewsletterNewsletter> NewsletterNewsletters { get; set; } = new List<NewsletterNewsletter>();

    [InverseProperty("SubscriberSite")]
    public virtual ICollection<NewsletterSubscriber> NewsletterSubscribers { get; set; } = new List<NewsletterSubscriber>();

    [InverseProperty("SubscriptionSite")]
    public virtual ICollection<NotificationSubscription> NotificationSubscriptions { get; set; } = new List<NotificationSubscription>();

    [InverseProperty("TemplateSite")]
    public virtual ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();

    [InverseProperty("AbtestSite")]
    public virtual ICollection<OmAbtest> OmAbtests { get; set; } = new List<OmAbtest>();

    [InverseProperty("AbvariantSite")]
    public virtual ICollection<OmAbvariant> OmAbvariants { get; set; } = new List<OmAbvariant>();

    [InverseProperty("MvtestSite")]
    public virtual ICollection<OmMvtest> OmMvtests { get; set; } = new List<OmMvtest>();

    [InverseProperty("PollSite")]
    public virtual ICollection<PollsPoll> PollsPolls { get; set; } = new List<PollsPoll>();

    [InverseProperty("ReportSubscriptionSite")]
    public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; } = new List<ReportingReportSubscription>();

    [InverseProperty("SharePointConnectionSite")]
    public virtual ICollection<SharePointSharePointConnection> SharePointSharePointConnections { get; set; } = new List<SharePointSharePointConnection>();

    [InverseProperty("SharePointFileSite")]
    public virtual ICollection<SharePointSharePointFile> SharePointSharePointFiles { get; set; } = new List<SharePointSharePointFile>();

    [InverseProperty("SharePointLibrarySite")]
    public virtual ICollection<SharePointSharePointLibrary> SharePointSharePointLibraries { get; set; } = new List<SharePointSharePointLibrary>();

    [ForeignKey("SiteDefaultEditorStylesheet")]
    [InverseProperty("CmsSiteSiteDefaultEditorStylesheetNavigations")]
    public virtual CmsCssStylesheet? SiteDefaultEditorStylesheetNavigation { get; set; }

    [ForeignKey("SiteDefaultStylesheetId")]
    [InverseProperty("CmsSiteSiteDefaultStylesheets")]
    public virtual CmsCssStylesheet? SiteDefaultStylesheet { get; set; }

    [InverseProperty("FacebookAccountSite")]
    public virtual ICollection<SmFacebookAccount> SmFacebookAccounts { get; set; } = new List<SmFacebookAccount>();

    [InverseProperty("FacebookApplicationSite")]
    public virtual ICollection<SmFacebookApplication> SmFacebookApplications { get; set; } = new List<SmFacebookApplication>();

    [InverseProperty("FacebookPostSite")]
    public virtual ICollection<SmFacebookPost> SmFacebookPosts { get; set; } = new List<SmFacebookPost>();

    [InverseProperty("LinkedInApplicationSite")]
    public virtual ICollection<SmLinkedInApplication> SmLinkedInApplications { get; set; } = new List<SmLinkedInApplication>();

    [InverseProperty("LinkedInPostSite")]
    public virtual ICollection<SmLinkedInPost> SmLinkedInPosts { get; set; } = new List<SmLinkedInPost>();

    [InverseProperty("TwitterAccountSite")]
    public virtual ICollection<SmTwitterAccount> SmTwitterAccounts { get; set; } = new List<SmTwitterAccount>();

    [InverseProperty("TwitterApplicationSite")]
    public virtual ICollection<SmTwitterApplication> SmTwitterApplications { get; set; } = new List<SmTwitterApplication>();

    [InverseProperty("TwitterPostSite")]
    public virtual ICollection<SmTwitterPost> SmTwitterPosts { get; set; } = new List<SmTwitterPost>();

    [InverseProperty("ServerSite")]
    public virtual ICollection<StagingServer> StagingServers { get; set; } = new List<StagingServer>();

    [InverseProperty("TaskSite")]
    public virtual ICollection<StagingTask> StagingTasks { get; set; } = new List<StagingTask>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsClass> Classes { get; set; } = new List<CmsClass>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsWebPartContainer> Containers { get; set; } = new List<CmsWebPartContainer>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsCulture> Cultures { get; set; } = new List<CmsCulture>();

    [ForeignKey("IndexSiteId")]
    [InverseProperty("IndexSites")]
    public virtual ICollection<CmsSearchIndex> Indices { get; set; } = new List<CmsSearchIndex>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsPageTemplate> PageTemplates { get; set; } = new List<CmsPageTemplate>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<PollsPoll> Polls { get; set; } = new List<PollsPoll>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsRelationshipName> RelationshipNames { get; set; } = new List<CmsRelationshipName>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsResource> Resources { get; set; } = new List<CmsResource>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsSmtpserver> Servers { get; set; } = new List<CmsSmtpserver>();

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsCssStylesheet> Stylesheets { get; set; } = new List<CmsCssStylesheet>();
}
