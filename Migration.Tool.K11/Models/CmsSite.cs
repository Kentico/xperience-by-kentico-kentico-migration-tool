using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Migration.Tool.K11.Models;

[Table("CMS_Site")]
[Index("SiteDefaultEditorStylesheet", Name = "IX_CMS_Site_SiteDefaultEditorStylesheet")]
[Index("SiteDefaultStylesheetId", Name = "IX_CMS_Site_SiteDefaultStylesheetID")]
[Index("SiteDomainName", "SiteStatus", Name = "IX_CMS_Site_SiteDomainName_SiteStatus")]
[Index("SiteName", Name = "IX_CMS_Site_SiteName")]
public class CmsSite
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
    public virtual ICollection<AnalyticsCampaign> AnalyticsCampaigns { get; set; } = [];

    [InverseProperty("ConversionSite")]
    public virtual ICollection<AnalyticsConversion> AnalyticsConversions { get; set; } = [];

    [InverseProperty("StatisticsSite")]
    public virtual ICollection<AnalyticsStatistic> AnalyticsStatistics { get; set; } = [];

    [InverseProperty("BoardSite")]
    public virtual ICollection<BoardBoard> BoardBoards { get; set; } = [];

    [InverseProperty("ChatNotificationSite")]
    public virtual ICollection<ChatNotification> ChatNotifications { get; set; } = [];

    [InverseProperty("ChatOnlineSupportSite")]
    public virtual ICollection<ChatOnlineSupport> ChatOnlineSupports { get; set; } = [];

    [InverseProperty("ChatOnlineUserSite")]
    public virtual ICollection<ChatOnlineUser> ChatOnlineUsers { get; set; } = [];

    [InverseProperty("ChatRoomSite")]
    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = [];

    [InverseProperty("ChatSupportCannedResponseSite")]
    public virtual ICollection<ChatSupportCannedResponse> ChatSupportCannedResponses { get; set; } = [];

    [InverseProperty("ReportSite")]
    public virtual ICollection<CmsAbuseReport> CmsAbuseReports { get; set; } = [];

    [InverseProperty("Aclsite")]
    public virtual ICollection<CmsAcl> CmsAcls { get; set; } = [];

    [InverseProperty("AttachmentSite")]
    public virtual ICollection<CmsAttachmentHistory> CmsAttachmentHistories { get; set; } = [];

    [InverseProperty("AttachmentSite")]
    public virtual ICollection<CmsAttachment> CmsAttachments { get; set; } = [];

    [InverseProperty("StateSite")]
    public virtual ICollection<CmsAutomationState> CmsAutomationStates { get; set; } = [];

    [InverseProperty("IpaddressSite")]
    public virtual ICollection<CmsBannedIp> CmsBannedIps { get; set; } = [];

    [InverseProperty("BannerCategorySite")]
    public virtual ICollection<CmsBannerCategory> CmsBannerCategories { get; set; } = [];

    [InverseProperty("BannerSite")]
    public virtual ICollection<CmsBanner> CmsBanners { get; set; } = [];

    [InverseProperty("CategorySite")]
    public virtual ICollection<CmsCategory> CmsCategories { get; set; } = [];

    [InverseProperty("AliasSite")]
    public virtual ICollection<CmsDocumentAlias> CmsDocumentAliases { get; set; } = [];

    [InverseProperty("ScopeSite")]
    public virtual ICollection<CmsDocumentTypeScope> CmsDocumentTypeScopes { get; set; } = [];

    [InverseProperty("EmailTemplateSite")]
    public virtual ICollection<CmsEmailTemplate> CmsEmailTemplates { get; set; } = [];

    [InverseProperty("FormSite")]
    public virtual ICollection<CmsForm> CmsForms { get; set; } = [];

    [InverseProperty("MembershipSite")]
    public virtual ICollection<CmsMembership> CmsMemberships { get; set; } = [];

    [InverseProperty("MetaFileSite")]
    public virtual ICollection<CmsMetaFile> CmsMetaFiles { get; set; } = [];

    [InverseProperty("VersionObjectSite")]
    public virtual ICollection<CmsObjectVersionHistory> CmsObjectVersionHistories { get; set; } = [];

    [InverseProperty("PageTemplateScopeSite")]
    public virtual ICollection<CmsPageTemplateScope> CmsPageTemplateScopes { get; set; } = [];

    [InverseProperty("PageTemplateSite")]
    public virtual ICollection<CmsPageTemplate> CmsPageTemplates { get; set; } = [];

    [InverseProperty("PersonalizationSite")]
    public virtual ICollection<CmsPersonalization> CmsPersonalizations { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<CmsRole> CmsRoles { get; set; } = [];

    [InverseProperty("TaskSite")]
    public virtual ICollection<CmsScheduledTask> CmsScheduledTasks { get; set; } = [];

    [InverseProperty("SessionSite")]
    public virtual ICollection<CmsSession> CmsSessions { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<CmsSettingsKey> CmsSettingsKeys { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<CmsSiteDomainAlias> CmsSiteDomainAliases { get; set; } = [];

    [InverseProperty("TagGroupSite")]
    public virtual ICollection<CmsTagGroup> CmsTagGroups { get; set; } = [];

    [InverseProperty("NodeLinkedNodeSite")]
    public virtual ICollection<CmsTree> CmsTreeNodeLinkedNodeSites { get; set; } = [];

    [InverseProperty("NodeSite")]
    public virtual ICollection<CmsTree> CmsTreeNodeSites { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<CmsUserCulture> CmsUserCultures { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<CmsUserSite> CmsUserSites { get; set; } = [];

    [InverseProperty("NodeSite")]
    public virtual ICollection<CmsVersionHistory> CmsVersionHistories { get; set; } = [];

    [InverseProperty("ScopeSite")]
    public virtual ICollection<CmsWorkflowScope> CmsWorkflowScopes { get; set; } = [];

    [InverseProperty("BrandSite")]
    public virtual ICollection<ComBrand> ComBrands { get; set; } = [];

    [InverseProperty("CarrierSite")]
    public virtual ICollection<ComCarrier> ComCarriers { get; set; } = [];

    [InverseProperty("CollectionSite")]
    public virtual ICollection<ComCollection> ComCollections { get; set; } = [];

    [InverseProperty("CurrencySite")]
    public virtual ICollection<ComCurrency> ComCurrencies { get; set; } = [];

    [InverseProperty("EventSite")]
    public virtual ICollection<ComCustomerCreditHistory> ComCustomerCreditHistories { get; set; } = [];

    [InverseProperty("CustomerSite")]
    public virtual ICollection<ComCustomer> ComCustomers { get; set; } = [];

    [InverseProperty("DepartmentSite")]
    public virtual ICollection<ComDepartment> ComDepartments { get; set; } = [];

    [InverseProperty("DiscountSite")]
    public virtual ICollection<ComDiscount> ComDiscounts { get; set; } = [];

    [InverseProperty("ExchangeTableSite")]
    public virtual ICollection<ComExchangeTable> ComExchangeTables { get; set; } = [];

    [InverseProperty("GiftCardSite")]
    public virtual ICollection<ComGiftCard> ComGiftCards { get; set; } = [];

    [InverseProperty("InternalStatusSite")]
    public virtual ICollection<ComInternalStatus> ComInternalStatuses { get; set; } = [];

    [InverseProperty("ManufacturerSite")]
    public virtual ICollection<ComManufacturer> ComManufacturers { get; set; } = [];

    [InverseProperty("MultiBuyDiscountSite")]
    public virtual ICollection<ComMultiBuyDiscount> ComMultiBuyDiscounts { get; set; } = [];

    [InverseProperty("CategorySite")]
    public virtual ICollection<ComOptionCategory> ComOptionCategories { get; set; } = [];

    [InverseProperty("StatusSite")]
    public virtual ICollection<ComOrderStatus> ComOrderStatuses { get; set; } = [];

    [InverseProperty("OrderSite")]
    public virtual ICollection<ComOrder> ComOrders { get; set; } = [];

    [InverseProperty("PaymentOptionSite")]
    public virtual ICollection<ComPaymentOption> ComPaymentOptions { get; set; } = [];

    [InverseProperty("PublicStatusSite")]
    public virtual ICollection<ComPublicStatus> ComPublicStatuses { get; set; } = [];

    [InverseProperty("ShippingOptionSite")]
    public virtual ICollection<ComShippingOption> ComShippingOptions { get; set; } = [];

    [InverseProperty("ShoppingCartSite")]
    public virtual ICollection<ComShoppingCart> ComShoppingCarts { get; set; } = [];

    [InverseProperty("Skusite")]
    public virtual ICollection<ComSku> ComSkus { get; set; } = [];

    [InverseProperty("SupplierSite")]
    public virtual ICollection<ComSupplier> ComSuppliers { get; set; } = [];

    [InverseProperty("TaxClassSite")]
    public virtual ICollection<ComTaxClass> ComTaxClasses { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<ComWishlist> ComWishlists { get; set; } = [];

    [InverseProperty("GroupSite")]
    public virtual ICollection<CommunityGroup> CommunityGroups { get; set; } = [];

    [InverseProperty("ExportSite")]
    public virtual ICollection<ExportHistory> ExportHistories { get; set; } = [];

    [InverseProperty("TaskSite")]
    public virtual ICollection<ExportTask> ExportTasks { get; set; } = [];

    [InverseProperty("AttachmentSite")]
    public virtual ICollection<ForumsAttachment> ForumsAttachments { get; set; } = [];

    [InverseProperty("GroupSite")]
    public virtual ICollection<ForumsForumGroup> ForumsForumGroups { get; set; } = [];

    [InverseProperty("ForumSite")]
    public virtual ICollection<ForumsForum> ForumsForums { get; set; } = [];

    [InverseProperty("Site")]
    public virtual ICollection<ForumsUserFavorite> ForumsUserFavorites { get; set; } = [];

    [InverseProperty("TaskSite")]
    public virtual ICollection<IntegrationTask> IntegrationTasks { get; set; } = [];

    [InverseProperty("FileSite")]
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = [];

    [InverseProperty("LibrarySite")]
    public virtual ICollection<MediaLibrary> MediaLibraries { get; set; } = [];

    [InverseProperty("TemplateSite")]
    public virtual ICollection<NewsletterEmailTemplate> NewsletterEmailTemplates { get; set; } = [];

    [InverseProperty("EmailWidgetSite")]
    public virtual ICollection<NewsletterEmailWidget> NewsletterEmailWidgets { get; set; } = [];

    [InverseProperty("EmailSite")]
    public virtual ICollection<NewsletterEmail> NewsletterEmails { get; set; } = [];

    [InverseProperty("IssueSite")]
    public virtual ICollection<NewsletterNewsletterIssue> NewsletterNewsletterIssues { get; set; } = [];

    [InverseProperty("NewsletterSite")]
    public virtual ICollection<NewsletterNewsletter> NewsletterNewsletters { get; set; } = [];

    [InverseProperty("SubscriberSite")]
    public virtual ICollection<NewsletterSubscriber> NewsletterSubscribers { get; set; } = [];

    [InverseProperty("SubscriptionSite")]
    public virtual ICollection<NotificationSubscription> NotificationSubscriptions { get; set; } = [];

    [InverseProperty("TemplateSite")]
    public virtual ICollection<NotificationTemplate> NotificationTemplates { get; set; } = [];

    [InverseProperty("AbtestSite")]
    public virtual ICollection<OmAbtest> OmAbtests { get; set; } = [];

    [InverseProperty("AbvariantSite")]
    public virtual ICollection<OmAbvariant> OmAbvariants { get; set; } = [];

    [InverseProperty("MvtestSite")]
    public virtual ICollection<OmMvtest> OmMvtests { get; set; } = [];

    [InverseProperty("PollSite")]
    public virtual ICollection<PollsPoll> PollsPolls { get; set; } = [];

    [InverseProperty("ReportSubscriptionSite")]
    public virtual ICollection<ReportingReportSubscription> ReportingReportSubscriptions { get; set; } = [];

    [InverseProperty("SharePointConnectionSite")]
    public virtual ICollection<SharePointSharePointConnection> SharePointSharePointConnections { get; set; } = [];

    [InverseProperty("SharePointFileSite")]
    public virtual ICollection<SharePointSharePointFile> SharePointSharePointFiles { get; set; } = [];

    [InverseProperty("SharePointLibrarySite")]
    public virtual ICollection<SharePointSharePointLibrary> SharePointSharePointLibraries { get; set; } = [];

    [ForeignKey("SiteDefaultEditorStylesheet")]
    [InverseProperty("CmsSiteSiteDefaultEditorStylesheetNavigations")]
    public virtual CmsCssStylesheet? SiteDefaultEditorStylesheetNavigation { get; set; }

    [ForeignKey("SiteDefaultStylesheetId")]
    [InverseProperty("CmsSiteSiteDefaultStylesheets")]
    public virtual CmsCssStylesheet? SiteDefaultStylesheet { get; set; }

    [InverseProperty("FacebookAccountSite")]
    public virtual ICollection<SmFacebookAccount> SmFacebookAccounts { get; set; } = [];

    [InverseProperty("FacebookApplicationSite")]
    public virtual ICollection<SmFacebookApplication> SmFacebookApplications { get; set; } = [];

    [InverseProperty("FacebookPostSite")]
    public virtual ICollection<SmFacebookPost> SmFacebookPosts { get; set; } = [];

    [InverseProperty("LinkedInApplicationSite")]
    public virtual ICollection<SmLinkedInApplication> SmLinkedInApplications { get; set; } = [];

    [InverseProperty("LinkedInPostSite")]
    public virtual ICollection<SmLinkedInPost> SmLinkedInPosts { get; set; } = [];

    [InverseProperty("TwitterAccountSite")]
    public virtual ICollection<SmTwitterAccount> SmTwitterAccounts { get; set; } = [];

    [InverseProperty("TwitterApplicationSite")]
    public virtual ICollection<SmTwitterApplication> SmTwitterApplications { get; set; } = [];

    [InverseProperty("TwitterPostSite")]
    public virtual ICollection<SmTwitterPost> SmTwitterPosts { get; set; } = [];

    [InverseProperty("ServerSite")]
    public virtual ICollection<StagingServer> StagingServers { get; set; } = [];

    [InverseProperty("TaskSite")]
    public virtual ICollection<StagingTask> StagingTasks { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsClass> Classes { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsWebPartContainer> Containers { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsCulture> Cultures { get; set; } = [];

    [ForeignKey("IndexSiteId")]
    [InverseProperty("IndexSites")]
    public virtual ICollection<CmsSearchIndex> Indices { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsPageTemplate> PageTemplates { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<PollsPoll> Polls { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsRelationshipName> RelationshipNames { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsResource> Resources { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsSmtpserver> Servers { get; set; } = [];

    [ForeignKey("SiteId")]
    [InverseProperty("Sites")]
    public virtual ICollection<CmsCssStylesheet> Stylesheets { get; set; } = [];
}
