using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Migration.Toolkit.KX13.Models;

namespace Migration.Toolkit.KX13.Context;

public partial class KX13Context : DbContext
{
    public KX13Context()
    {
    }

    public KX13Context(DbContextOptions<KX13Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AnalyticsCampaign> AnalyticsCampaigns { get; set; }

    public virtual DbSet<AnalyticsCampaignAsset> AnalyticsCampaignAssets { get; set; }

    public virtual DbSet<AnalyticsCampaignAssetUrl> AnalyticsCampaignAssetUrls { get; set; }

    public virtual DbSet<AnalyticsCampaignConversion> AnalyticsCampaignConversions { get; set; }

    public virtual DbSet<AnalyticsCampaignConversionHit> AnalyticsCampaignConversionHits { get; set; }

    public virtual DbSet<AnalyticsCampaignObjective> AnalyticsCampaignObjectives { get; set; }

    public virtual DbSet<AnalyticsDayHit> AnalyticsDayHits { get; set; }

    public virtual DbSet<AnalyticsExitPage> AnalyticsExitPages { get; set; }

    public virtual DbSet<AnalyticsHourHit> AnalyticsHourHits { get; set; }

    public virtual DbSet<AnalyticsMonthHit> AnalyticsMonthHits { get; set; }

    public virtual DbSet<AnalyticsStatistic> AnalyticsStatistics { get; set; }

    public virtual DbSet<AnalyticsWeekHit> AnalyticsWeekHits { get; set; }

    public virtual DbSet<AnalyticsYearHit> AnalyticsYearHits { get; set; }

    public virtual DbSet<CiFileMetadatum> CiFileMetadata { get; set; }

    public virtual DbSet<CiMigration> CiMigrations { get; set; }

    public virtual DbSet<CmsAcl> CmsAcls { get; set; }

    public virtual DbSet<CmsAclitem> CmsAclitems { get; set; }

    public virtual DbSet<CmsAlternativeForm> CmsAlternativeForms { get; set; }

    public virtual DbSet<CmsAlternativeUrl> CmsAlternativeUrls { get; set; }

    public virtual DbSet<CmsAttachment> CmsAttachments { get; set; }

    public virtual DbSet<CmsAttachmentHistory> CmsAttachmentHistories { get; set; }

    public virtual DbSet<CmsAutomationHistory> CmsAutomationHistories { get; set; }

    public virtual DbSet<CmsAutomationState> CmsAutomationStates { get; set; }

    public virtual DbSet<CmsAutomationTemplate> CmsAutomationTemplates { get; set; }

    public virtual DbSet<CmsAvatar> CmsAvatars { get; set; }

    public virtual DbSet<CmsCategory> CmsCategories { get; set; }

    public virtual DbSet<CmsClass> CmsClasses { get; set; }

    public virtual DbSet<CmsConsent> CmsConsents { get; set; }

    public virtual DbSet<CmsConsentAgreement> CmsConsentAgreements { get; set; }

    public virtual DbSet<CmsConsentArchive> CmsConsentArchives { get; set; }

    public virtual DbSet<CmsCountry> CmsCountries { get; set; }

    public virtual DbSet<CmsCulture> CmsCultures { get; set; }

    public virtual DbSet<CmsDocument> CmsDocuments { get; set; }

    public virtual DbSet<CmsDocumentTypeScope> CmsDocumentTypeScopes { get; set; }

    public virtual DbSet<CmsEmail> CmsEmails { get; set; }

    public virtual DbSet<CmsEmailAttachment> CmsEmailAttachments { get; set; }

    public virtual DbSet<CmsEmailTemplate> CmsEmailTemplates { get; set; }

    public virtual DbSet<CmsEmailUser> CmsEmailUsers { get; set; }

    public virtual DbSet<CmsEventLog> CmsEventLogs { get; set; }

    public virtual DbSet<CmsExternalLogin> CmsExternalLogins { get; set; }

    public virtual DbSet<CmsForm> CmsForms { get; set; }

    public virtual DbSet<CmsFormUserControl> CmsFormUserControls { get; set; }

    public virtual DbSet<CmsHelpTopic> CmsHelpTopics { get; set; }

    public virtual DbSet<CmsLayout> CmsLayouts { get; set; }

    public virtual DbSet<CmsLicenseKey> CmsLicenseKeys { get; set; }

    public virtual DbSet<CmsMacroIdentity> CmsMacroIdentities { get; set; }

    public virtual DbSet<CmsMacroRule> CmsMacroRules { get; set; }

    public virtual DbSet<CmsMembership> CmsMemberships { get; set; }

    public virtual DbSet<CmsMembershipUser> CmsMembershipUsers { get; set; }

    public virtual DbSet<CmsMetaFile> CmsMetaFiles { get; set; }

    public virtual DbSet<CmsModuleLicenseKey> CmsModuleLicenseKeys { get; set; }

    public virtual DbSet<CmsModuleUsageCounter> CmsModuleUsageCounters { get; set; }

    public virtual DbSet<CmsObjectSetting> CmsObjectSettings { get; set; }

    public virtual DbSet<CmsObjectVersionHistory> CmsObjectVersionHistories { get; set; }

    public virtual DbSet<CmsObjectWorkflowTrigger> CmsObjectWorkflowTriggers { get; set; }

    public virtual DbSet<CmsPageFormerUrlPath> CmsPageFormerUrlPaths { get; set; }

    public virtual DbSet<CmsPageTemplate> CmsPageTemplates { get; set; }

    public virtual DbSet<CmsPageTemplateCategory> CmsPageTemplateCategories { get; set; }

    public virtual DbSet<CmsPageTemplateConfiguration> CmsPageTemplateConfigurations { get; set; }

    public virtual DbSet<CmsPageUrlPath> CmsPageUrlPaths { get; set; }

    public virtual DbSet<CmsPermission> CmsPermissions { get; set; }

    public virtual DbSet<CmsPersonalization> CmsPersonalizations { get; set; }

    public virtual DbSet<CmsQuery> CmsQueries { get; set; }

    public virtual DbSet<CmsRelationship> CmsRelationships { get; set; }

    public virtual DbSet<CmsRelationshipName> CmsRelationshipNames { get; set; }

    public virtual DbSet<CmsResource> CmsResources { get; set; }

    public virtual DbSet<CmsResourceLibrary> CmsResourceLibraries { get; set; }

    public virtual DbSet<CmsResourceString> CmsResourceStrings { get; set; }

    public virtual DbSet<CmsResourceTranslation> CmsResourceTranslations { get; set; }

    public virtual DbSet<CmsRole> CmsRoles { get; set; }

    public virtual DbSet<CmsScheduledTask> CmsScheduledTasks { get; set; }

    public virtual DbSet<CmsSearchEngine> CmsSearchEngines { get; set; }

    public virtual DbSet<CmsSearchIndex> CmsSearchIndices { get; set; }

    public virtual DbSet<CmsSearchTask> CmsSearchTasks { get; set; }

    public virtual DbSet<CmsSearchTaskAzure> CmsSearchTaskAzures { get; set; }

    public virtual DbSet<CmsSettingsCategory> CmsSettingsCategories { get; set; }

    public virtual DbSet<CmsSettingsKey> CmsSettingsKeys { get; set; }

    public virtual DbSet<CmsSite> CmsSites { get; set; }

    public virtual DbSet<CmsSiteDomainAlias> CmsSiteDomainAliases { get; set; }

    public virtual DbSet<CmsSmtpserver> CmsSmtpservers { get; set; }

    public virtual DbSet<CmsState> CmsStates { get; set; }

    public virtual DbSet<CmsTag> CmsTags { get; set; }

    public virtual DbSet<CmsTagGroup> CmsTagGroups { get; set; }

    public virtual DbSet<CmsTimeZone> CmsTimeZones { get; set; }

    public virtual DbSet<CmsTransformation> CmsTransformations { get; set; }

    public virtual DbSet<CmsTranslationService> CmsTranslationServices { get; set; }

    public virtual DbSet<CmsTranslationSubmission> CmsTranslationSubmissions { get; set; }

    public virtual DbSet<CmsTranslationSubmissionItem> CmsTranslationSubmissionItems { get; set; }

    public virtual DbSet<CmsTree> CmsTrees { get; set; }

    public virtual DbSet<CmsUielement> CmsUielements { get; set; }

    public virtual DbSet<CmsUser> CmsUsers { get; set; }

    public virtual DbSet<CmsUserCulture> CmsUserCultures { get; set; }

    public virtual DbSet<CmsUserMacroIdentity> CmsUserMacroIdentities { get; set; }

    public virtual DbSet<CmsUserRole> CmsUserRoles { get; set; }

    public virtual DbSet<CmsUserSetting> CmsUserSettings { get; set; }

    public virtual DbSet<CmsUserSite> CmsUserSites { get; set; }

    public virtual DbSet<CmsVersionHistory> CmsVersionHistories { get; set; }

    public virtual DbSet<CmsWebFarmServer> CmsWebFarmServers { get; set; }

    public virtual DbSet<CmsWebFarmServerLog> CmsWebFarmServerLogs { get; set; }

    public virtual DbSet<CmsWebFarmServerMonitoring> CmsWebFarmServerMonitorings { get; set; }

    public virtual DbSet<CmsWebFarmServerTask> CmsWebFarmServerTasks { get; set; }

    public virtual DbSet<CmsWebFarmTask> CmsWebFarmTasks { get; set; }

    public virtual DbSet<CmsWebPart> CmsWebParts { get; set; }

    public virtual DbSet<CmsWebPartCategory> CmsWebPartCategories { get; set; }

    public virtual DbSet<CmsWebPartContainer> CmsWebPartContainers { get; set; }

    public virtual DbSet<CmsWebPartLayout> CmsWebPartLayouts { get; set; }

    public virtual DbSet<CmsWidget> CmsWidgets { get; set; }

    public virtual DbSet<CmsWidgetCategory> CmsWidgetCategories { get; set; }

    public virtual DbSet<CmsWidgetRole> CmsWidgetRoles { get; set; }

    public virtual DbSet<CmsWorkflow> CmsWorkflows { get; set; }

    public virtual DbSet<CmsWorkflowAction> CmsWorkflowActions { get; set; }

    public virtual DbSet<CmsWorkflowHistory> CmsWorkflowHistories { get; set; }

    public virtual DbSet<CmsWorkflowScope> CmsWorkflowScopes { get; set; }

    public virtual DbSet<CmsWorkflowStep> CmsWorkflowSteps { get; set; }

    public virtual DbSet<CmsWorkflowStepRole> CmsWorkflowStepRoles { get; set; }

    public virtual DbSet<CmsWorkflowStepUser> CmsWorkflowStepUsers { get; set; }

    public virtual DbSet<CmsWorkflowTransition> CmsWorkflowTransitions { get; set; }

    public virtual DbSet<ComAddress> ComAddresses { get; set; }

    public virtual DbSet<ComBrand> ComBrands { get; set; }

    public virtual DbSet<ComCarrier> ComCarriers { get; set; }

    public virtual DbSet<ComCollection> ComCollections { get; set; }

    public virtual DbSet<ComCouponCode> ComCouponCodes { get; set; }

    public virtual DbSet<ComCurrency> ComCurrencies { get; set; }

    public virtual DbSet<ComCurrencyExchangeRate> ComCurrencyExchangeRates { get; set; }

    public virtual DbSet<ComCustomer> ComCustomers { get; set; }

    public virtual DbSet<ComCustomerCreditHistory> ComCustomerCreditHistories { get; set; }

    public virtual DbSet<ComDepartment> ComDepartments { get; set; }

    public virtual DbSet<ComDiscount> ComDiscounts { get; set; }

    public virtual DbSet<ComExchangeTable> ComExchangeTables { get; set; }

    public virtual DbSet<ComGiftCard> ComGiftCards { get; set; }

    public virtual DbSet<ComGiftCardCouponCode> ComGiftCardCouponCodes { get; set; }

    public virtual DbSet<ComInternalStatus> ComInternalStatuses { get; set; }

    public virtual DbSet<ComManufacturer> ComManufacturers { get; set; }

    public virtual DbSet<ComMultiBuyCouponCode> ComMultiBuyCouponCodes { get; set; }

    public virtual DbSet<ComMultiBuyDiscount> ComMultiBuyDiscounts { get; set; }

    public virtual DbSet<ComMultiBuyDiscountBrand> ComMultiBuyDiscountBrands { get; set; }

    public virtual DbSet<ComMultiBuyDiscountCollection> ComMultiBuyDiscountCollections { get; set; }

    public virtual DbSet<ComMultiBuyDiscountTree> ComMultiBuyDiscountTrees { get; set; }

    public virtual DbSet<ComOptionCategory> ComOptionCategories { get; set; }

    public virtual DbSet<ComOrder> ComOrders { get; set; }

    public virtual DbSet<ComOrderAddress> ComOrderAddresses { get; set; }

    public virtual DbSet<ComOrderItem> ComOrderItems { get; set; }

    public virtual DbSet<ComOrderItemSkufile> ComOrderItemSkufiles { get; set; }

    public virtual DbSet<ComOrderStatus> ComOrderStatuses { get; set; }

    public virtual DbSet<ComOrderStatusUser> ComOrderStatusUsers { get; set; }

    public virtual DbSet<ComPaymentOption> ComPaymentOptions { get; set; }

    public virtual DbSet<ComPublicStatus> ComPublicStatuses { get; set; }

    public virtual DbSet<ComShippingCost> ComShippingCosts { get; set; }

    public virtual DbSet<ComShippingOption> ComShippingOptions { get; set; }

    public virtual DbSet<ComShoppingCart> ComShoppingCarts { get; set; }

    public virtual DbSet<ComShoppingCartCouponCode> ComShoppingCartCouponCodes { get; set; }

    public virtual DbSet<ComShoppingCartSku> ComShoppingCartSkus { get; set; }

    public virtual DbSet<ComSku> ComSkus { get; set; }

    public virtual DbSet<ComSkufile> ComSkufiles { get; set; }

    public virtual DbSet<ComSkuoptionCategory> ComSkuoptionCategories { get; set; }

    public virtual DbSet<ComSupplier> ComSuppliers { get; set; }

    public virtual DbSet<ComTaxClass> ComTaxClasses { get; set; }

    public virtual DbSet<ComTaxClassCountry> ComTaxClassCountries { get; set; }

    public virtual DbSet<ComTaxClassState> ComTaxClassStates { get; set; }

    public virtual DbSet<ComVolumeDiscount> ComVolumeDiscounts { get; set; }

    public virtual DbSet<ComWishlist> ComWishlists { get; set; }

    public virtual DbSet<ExportHistory> ExportHistories { get; set; }

    public virtual DbSet<ExportTask> ExportTasks { get; set; }

    public virtual DbSet<IntegrationConnector> IntegrationConnectors { get; set; }

    public virtual DbSet<IntegrationSyncLog> IntegrationSyncLogs { get; set; }

    public virtual DbSet<IntegrationSynchronization> IntegrationSynchronizations { get; set; }

    public virtual DbSet<IntegrationTask> IntegrationTasks { get; set; }

    public virtual DbSet<MediaFile> MediaFiles { get; set; }

    public virtual DbSet<MediaLibrary> MediaLibraries { get; set; }

    public virtual DbSet<MediaLibraryRolePermission> MediaLibraryRolePermissions { get; set; }

    public virtual DbSet<NewsletterAbtest> NewsletterAbtests { get; set; }

    public virtual DbSet<NewsletterClickedLink> NewsletterClickedLinks { get; set; }

    public virtual DbSet<NewsletterEmail> NewsletterEmails { get; set; }

    public virtual DbSet<NewsletterEmailTemplate> NewsletterEmailTemplates { get; set; }

    public virtual DbSet<NewsletterEmailWidget> NewsletterEmailWidgets { get; set; }

    public virtual DbSet<NewsletterEmailWidgetTemplate> NewsletterEmailWidgetTemplates { get; set; }

    public virtual DbSet<NewsletterIssueContactGroup> NewsletterIssueContactGroups { get; set; }

    public virtual DbSet<NewsletterLink> NewsletterLinks { get; set; }

    public virtual DbSet<NewsletterNewsletter> NewsletterNewsletters { get; set; }

    public virtual DbSet<NewsletterNewsletterIssue> NewsletterNewsletterIssues { get; set; }

    public virtual DbSet<NewsletterOpenedEmail> NewsletterOpenedEmails { get; set; }

    public virtual DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }

    public virtual DbSet<NewsletterSubscriberNewsletter> NewsletterSubscriberNewsletters { get; set; }

    public virtual DbSet<NewsletterUnsubscription> NewsletterUnsubscriptions { get; set; }

    public virtual DbSet<OmAbtest> OmAbtests { get; set; }

    public virtual DbSet<OmAbvariantDatum> OmAbvariantData { get; set; }

    public virtual DbSet<OmAccount> OmAccounts { get; set; }

    public virtual DbSet<OmAccountContact> OmAccountContacts { get; set; }

    public virtual DbSet<OmAccountStatus> OmAccountStatuses { get; set; }

    public virtual DbSet<OmActivity> OmActivities { get; set; }

    public virtual DbSet<OmActivityRecalculationQueue> OmActivityRecalculationQueues { get; set; }

    public virtual DbSet<OmActivityType> OmActivityTypes { get; set; }

    public virtual DbSet<OmContact> OmContacts { get; set; }

    public virtual DbSet<OmContactChangeRecalculationQueue> OmContactChangeRecalculationQueues { get; set; }

    public virtual DbSet<OmContactGroup> OmContactGroups { get; set; }

    public virtual DbSet<OmContactGroupMember> OmContactGroupMembers { get; set; }

    public virtual DbSet<OmContactRole> OmContactRoles { get; set; }

    public virtual DbSet<OmContactStatus> OmContactStatuses { get; set; }

    public virtual DbSet<OmMembership> OmMemberships { get; set; }

    public virtual DbSet<OmRule> OmRules { get; set; }

    public virtual DbSet<OmScore> OmScores { get; set; }

    public virtual DbSet<OmScoreContactRule> OmScoreContactRules { get; set; }

    public virtual DbSet<OmVisitorToContact> OmVisitorToContacts { get; set; }

    public virtual DbSet<PersonasPersona> PersonasPersonas { get; set; }

    public virtual DbSet<PersonasPersonaContactHistory> PersonasPersonaContactHistories { get; set; }

    public virtual DbSet<ReportingReport> ReportingReports { get; set; }

    public virtual DbSet<ReportingReportCategory> ReportingReportCategories { get; set; }

    public virtual DbSet<ReportingReportGraph> ReportingReportGraphs { get; set; }

    public virtual DbSet<ReportingReportSubscription> ReportingReportSubscriptions { get; set; }

    public virtual DbSet<ReportingReportTable> ReportingReportTables { get; set; }

    public virtual DbSet<ReportingReportValue> ReportingReportValues { get; set; }

    public virtual DbSet<ReportingSavedGraph> ReportingSavedGraphs { get; set; }

    public virtual DbSet<ReportingSavedReport> ReportingSavedReports { get; set; }

    public virtual DbSet<SharePointSharePointConnection> SharePointSharePointConnections { get; set; }

    public virtual DbSet<SharePointSharePointFile> SharePointSharePointFiles { get; set; }

    public virtual DbSet<SharePointSharePointLibrary> SharePointSharePointLibraries { get; set; }

    public virtual DbSet<SmFacebookAccount> SmFacebookAccounts { get; set; }

    public virtual DbSet<SmFacebookApplication> SmFacebookApplications { get; set; }

    public virtual DbSet<SmFacebookPost> SmFacebookPosts { get; set; }

    public virtual DbSet<SmInsight> SmInsights { get; set; }

    public virtual DbSet<SmInsightHitDay> SmInsightHitDays { get; set; }

    public virtual DbSet<SmInsightHitMonth> SmInsightHitMonths { get; set; }

    public virtual DbSet<SmInsightHitWeek> SmInsightHitWeeks { get; set; }

    public virtual DbSet<SmInsightHitYear> SmInsightHitYears { get; set; }

    public virtual DbSet<SmLinkedInAccount> SmLinkedInAccounts { get; set; }

    public virtual DbSet<SmLinkedInApplication> SmLinkedInApplications { get; set; }

    public virtual DbSet<SmLinkedInPost> SmLinkedInPosts { get; set; }

    public virtual DbSet<SmTwitterAccount> SmTwitterAccounts { get; set; }

    public virtual DbSet<SmTwitterApplication> SmTwitterApplications { get; set; }

    public virtual DbSet<SmTwitterPost> SmTwitterPosts { get; set; }

    public virtual DbSet<StagingServer> StagingServers { get; set; }

    public virtual DbSet<StagingSynchronization> StagingSynchronizations { get; set; }

    public virtual DbSet<StagingTask> StagingTasks { get; set; }

    public virtual DbSet<StagingTaskGroup> StagingTaskGroups { get; set; }

    public virtual DbSet<StagingTaskGroupTask> StagingTaskGroupTasks { get; set; }

    public virtual DbSet<StagingTaskGroupUser> StagingTaskGroupUsers { get; set; }

    public virtual DbSet<StagingTaskUser> StagingTaskUsers { get; set; }

    public virtual DbSet<TempFile> TempFiles { get; set; }

    public virtual DbSet<TempPageBuilderWidget> TempPageBuilderWidgets { get; set; }

    public virtual DbSet<ViewCmsAclitemItemsAndOperator> ViewCmsAclitemItemsAndOperators { get; set; }

    public virtual DbSet<ViewCmsObjectVersionHistoryUserJoined> ViewCmsObjectVersionHistoryUserJoineds { get; set; }

    public virtual DbSet<ViewCmsPageTemplateCategoryPageTemplateJoined> ViewCmsPageTemplateCategoryPageTemplateJoineds { get; set; }

    public virtual DbSet<ViewCmsRelationshipJoined> ViewCmsRelationshipJoineds { get; set; }

    public virtual DbSet<ViewCmsResourceStringJoined> ViewCmsResourceStringJoineds { get; set; }

    public virtual DbSet<ViewCmsResourceTranslatedJoined> ViewCmsResourceTranslatedJoineds { get; set; }

    public virtual DbSet<ViewCmsRoleResourcePermissionJoined> ViewCmsRoleResourcePermissionJoineds { get; set; }

    public virtual DbSet<ViewCmsSiteDocumentCount> ViewCmsSiteDocumentCounts { get; set; }

    public virtual DbSet<ViewCmsSiteRoleResourceUielementJoined> ViewCmsSiteRoleResourceUielementJoineds { get; set; }

    public virtual DbSet<ViewCmsTreeJoined> ViewCmsTreeJoineds { get; set; }

    public virtual DbSet<ViewCmsUser> ViewCmsUsers { get; set; }

    public virtual DbSet<ViewCmsUserDocument> ViewCmsUserDocuments { get; set; }

    public virtual DbSet<ViewCmsUserRoleJoined> ViewCmsUserRoleJoineds { get; set; }

    public virtual DbSet<ViewCmsUserRoleMembershipRole> ViewCmsUserRoleMembershipRoles { get; set; }

    public virtual DbSet<ViewCmsUserRoleMembershipRoleValidOnlyJoined> ViewCmsUserRoleMembershipRoleValidOnlyJoineds { get; set; }

    public virtual DbSet<ViewCmsUserSettingsRoleJoined> ViewCmsUserSettingsRoleJoineds { get; set; }

    public virtual DbSet<ViewCmsWebPartCategoryWebpartJoined> ViewCmsWebPartCategoryWebpartJoineds { get; set; }

    public virtual DbSet<ViewCmsWidgetCategoryWidgetJoined> ViewCmsWidgetCategoryWidgetJoineds { get; set; }

    public virtual DbSet<ViewComSkuoptionCategoryOptionCategoryJoined> ViewComSkuoptionCategoryOptionCategoryJoineds { get; set; }

    public virtual DbSet<ViewIntegrationTaskJoined> ViewIntegrationTaskJoineds { get; set; }

    public virtual DbSet<ViewMembershipMembershipUserJoined> ViewMembershipMembershipUserJoineds { get; set; }

    public virtual DbSet<ViewNewsletterSubscriptionsJoined> ViewNewsletterSubscriptionsJoineds { get; set; }

    public virtual DbSet<ViewOmAccountContactAccountJoined> ViewOmAccountContactAccountJoineds { get; set; }

    public virtual DbSet<ViewOmAccountContactContactJoined> ViewOmAccountContactContactJoineds { get; set; }

    public virtual DbSet<ViewOmAccountJoined> ViewOmAccountJoineds { get; set; }

    public virtual DbSet<ViewOmContactGroupMemberAccountJoined> ViewOmContactGroupMemberAccountJoineds { get; set; }

    public virtual DbSet<ViewReportingCategoryReportJoined> ViewReportingCategoryReportJoineds { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalyticsCampaign>(entity =>
        {
            entity.Property(e => e.CampaignDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.CampaignName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.CampaignScheduledTask).WithMany(p => p.AnalyticsCampaigns).HasConstraintName("FK_Analytics_Campaign_CampaignScheduledTaskID_ScheduledTask");

            entity.HasOne(d => d.CampaignSite).WithMany(p => p.AnalyticsCampaigns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_Campaign_StatisticsSiteID_CMS_Site");
        });

        modelBuilder.Entity<AnalyticsCampaignAsset>(entity =>
        {
            entity.Property(e => e.CampaignAssetLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.CampaignAssetType).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CampaignAssetCampaign).WithMany(p => p.AnalyticsCampaignAssets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_CampaignAsset_CampaignAssetCampaignID_Analytics_Campaign");
        });

        modelBuilder.Entity<AnalyticsCampaignAssetUrl>(entity =>
        {
            entity.Property(e => e.CampaignAssetUrlPageTitle).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CampaignAssetUrlTarget).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CampaignAssetUrlCampaignAsset).WithMany(p => p.AnalyticsCampaignAssetUrls)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_CampaignAssetUrl_CampaignAssetUrlCampaignAssetID_Analytics_CampaignAsset");
        });

        modelBuilder.Entity<AnalyticsCampaignConversion>(entity =>
        {
            entity.Property(e => e.CampaignConversionActivityType).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CampaignConversionDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CampaignConversionLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.CampaignConversionName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CampaignConversionCampaign).WithMany(p => p.AnalyticsCampaignConversions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_CampaignConversion_CampaignConversionCampaignID_Analytics_Campaign");
        });

        modelBuilder.Entity<AnalyticsCampaignConversionHit>(entity =>
        {
            entity.Property(e => e.CampaignConversionHitsSourceName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CampaignConversionHitsConversion).WithMany(p => p.AnalyticsCampaignConversionHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_CampaignConversionHits_CampaignConversionHitsConversionID_Analytics_CampaignConversion");
        });

        modelBuilder.Entity<AnalyticsCampaignObjective>(entity =>
        {
            entity.Property(e => e.CampaignObjectiveLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.CampaignObjectiveCampaignConversion).WithMany(p => p.AnalyticsCampaignObjectives)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_CampaignObjective_CampaignObjectiveCampaignConversionID_Analytics_CampaignConversion");

            entity.HasOne(d => d.CampaignObjectiveCampaign).WithOne(p => p.AnalyticsCampaignObjective)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_CampaignObjective_CampaignObjectiveCampaignID_Analytics_Campaign");
        });

        modelBuilder.Entity<AnalyticsDayHit>(entity =>
        {
            entity.HasKey(e => e.HitsId).IsClustered(false);

            entity.HasIndex(e => new { e.HitsStartTime, e.HitsEndTime }, "IX_Analytics_DayHits_HitsStartTime_HitsEndTime")
                .IsDescending()
                .IsClustered();

            entity.HasOne(d => d.HitsStatistics).WithMany(p => p.AnalyticsDayHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_DayHits_HitsStatisticsID_Analytics_Statistics");
        });

        modelBuilder.Entity<AnalyticsExitPage>(entity =>
        {
            entity.Property(e => e.ExitPageUrl).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ExitPageSite).WithMany(p => p.AnalyticsExitPages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_ExitPages_ExitPageSiteID_CMS_Site");
        });

        modelBuilder.Entity<AnalyticsHourHit>(entity =>
        {
            entity.HasKey(e => e.HitsId).IsClustered(false);

            entity.HasIndex(e => new { e.HitsStartTime, e.HitsEndTime }, "IX_Analytics_HourHits_HitsStartTime_HitsEndTime")
                .IsDescending()
                .IsClustered();

            entity.HasOne(d => d.HitsStatistics).WithMany(p => p.AnalyticsHourHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_HourHits_HitsStatisticsID_Analytics_Statistics");
        });

        modelBuilder.Entity<AnalyticsMonthHit>(entity =>
        {
            entity.HasKey(e => e.HitsId).IsClustered(false);

            entity.HasIndex(e => new { e.HitsStartTime, e.HitsEndTime }, "IX_Analytics_MonthHits_HitsStartTime_HitsEndTime")
                .IsDescending()
                .IsClustered();

            entity.HasOne(d => d.HitsStatistics).WithMany(p => p.AnalyticsMonthHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_MonthHits_HitsStatisticsID_Analytics_Statistics");
        });

        modelBuilder.Entity<AnalyticsStatistic>(entity =>
        {
            entity.HasKey(e => e.StatisticsId).IsClustered(false);

            entity.HasIndex(e => e.StatisticsCode, "IX_Analytics_Statistics_StatisticsCode_StatisticsSiteID_StatisticsObjectID_StatisticsObjectCulture").IsClustered();

            entity.Property(e => e.StatisticsCode).HasDefaultValueSql("('')");

            entity.HasOne(d => d.StatisticsSite).WithMany(p => p.AnalyticsStatistics).HasConstraintName("FK_Analytics_Statistics_StatisticsSiteID_CMS_Site");
        });

        modelBuilder.Entity<AnalyticsWeekHit>(entity =>
        {
            entity.HasKey(e => e.HitsId).IsClustered(false);

            entity.HasIndex(e => new { e.HitsStartTime, e.HitsEndTime }, "IX_Analytics_WeekHits_HitsStartTime_HitsEndTime")
                .IsDescending()
                .IsClustered();

            entity.HasOne(d => d.HitsStatistics).WithMany(p => p.AnalyticsWeekHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_WeekHits_HitsStatisticsID_Analytics_Statistics");
        });

        modelBuilder.Entity<AnalyticsYearHit>(entity =>
        {
            entity.HasKey(e => e.HitsId).IsClustered(false);

            entity.HasIndex(e => new { e.HitsStartTime, e.HitsEndTime }, "IX_Analytics_YearHits_HitsStartTime_HitsEndTime")
                .IsDescending()
                .IsClustered();

            entity.HasOne(d => d.HitsStatistics).WithMany(p => p.AnalyticsYearHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Analytics_YearHits_HitsStatisticsID_Analytics_Statistics");
        });

        modelBuilder.Entity<CiFileMetadatum>(entity =>
        {
            entity.Property(e => e.FileHash).HasDefaultValueSql("(N'')");
            entity.Property(e => e.FileLocation).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CiMigration>(entity =>
        {
            entity.Property(e => e.DateApplied).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<CmsAcl>(entity =>
        {
            entity.Property(e => e.AclinheritedAcls).HasDefaultValueSql("('')");
            entity.Property(e => e.AcllastModified).HasDefaultValueSql("('10/30/2008 9:17:31 AM')");

            entity.HasOne(d => d.Aclsite).WithMany(p => p.CmsAcls).HasConstraintName("FK_CMS_ACL_ACLSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsAclitem>(entity =>
        {
            entity.HasOne(d => d.Acl).WithMany(p => p.CmsAclitems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ACLItem_ACLID_CMS_ACL");

            entity.HasOne(d => d.LastModifiedByUser).WithMany(p => p.CmsAclitemLastModifiedByUsers).HasConstraintName("FK_CMS_ACLItem_LastModifiedByUserID_CMS_User");

            entity.HasOne(d => d.Role).WithMany(p => p.CmsAclitems).HasConstraintName("FK_CMS_ACLItem_RoleID_CMS_Role");

            entity.HasOne(d => d.User).WithMany(p => p.CmsAclitemUsers).HasConstraintName("FK_CMS_ACLItem_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsAlternativeForm>(entity =>
        {
            entity.Property(e => e.FormDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.FormHideNewParentFields).HasDefaultValueSql("((0))");
            entity.Property(e => e.FormIsCustom).HasDefaultValueSql("((0))");
            entity.Property(e => e.FormName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.FormClass).WithMany(p => p.CmsAlternativeFormFormClasses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AlternativeForm_FormClassID_CMS_Class");

            entity.HasOne(d => d.FormCoupledClass).WithMany(p => p.CmsAlternativeFormFormCoupledClasses).HasConstraintName("FK_CMS_AlternativeForm_FormCoupledClassID_CMS_Class");
        });

        modelBuilder.Entity<CmsAlternativeUrl>(entity =>
        {
            entity.Property(e => e.AlternativeUrlLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.AlternativeUrlUrl).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.AlternativeUrlDocument).WithMany(p => p.CmsAlternativeUrls)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AlternativeUrl_CMS_Document");

            entity.HasOne(d => d.AlternativeUrlSite).WithMany(p => p.CmsAlternativeUrls)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AlternativeUrl_CMS_Site");
        });

        modelBuilder.Entity<CmsAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).IsClustered(false);

            entity.HasIndex(e => new { e.AttachmentDocumentId, e.AttachmentName, e.AttachmentIsUnsorted, e.AttachmentOrder }, "IX_CMS_Attachment_AttachmentDocumentID_AttachmentIsUnsorted_AttachmentName_AttachmentOrder").IsClustered();

            entity.HasIndex(e => new { e.AttachmentVariantDefinitionIdentifier, e.AttachmentVariantParentId }, "IX_CMS_Attachment_AttachmentVariantParentID_AttachmentVariantDefinitionIdentifier")
                .IsUnique()
                .HasFilter("([AttachmentVariantDefinitionIdentifier] IS NOT NULL AND [AttachmentVariantParentID] IS NOT NULL)");

            entity.HasOne(d => d.AttachmentDocument).WithMany(p => p.CmsAttachments).HasConstraintName("FK_CMS_Attachment_AttachmentDocumentID_CMS_Document");

            entity.HasOne(d => d.AttachmentSite).WithMany(p => p.CmsAttachments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Attachment_AttachmentSiteID_CMS_Site");

            entity.HasOne(d => d.AttachmentVariantParent).WithMany(p => p.InverseAttachmentVariantParent).HasConstraintName("FK_CMS_Attachment_AttachmentVariantParentID_CMS_Attachment");
        });

        modelBuilder.Entity<CmsAttachmentHistory>(entity =>
        {
            entity.HasKey(e => e.AttachmentHistoryId).IsClustered(false);

            entity.HasIndex(e => new { e.AttachmentDocumentId, e.AttachmentName }, "IX_CMS_AttachmentHistory_AttachmentDocumentID_AttachmentName").IsClustered();

            entity.HasIndex(e => new { e.AttachmentVariantDefinitionIdentifier, e.AttachmentVariantParentId }, "IX_CMS_AttachmentHistory_AttachmentVariantParentID_AttachmentVariantDefinitionIdentifier")
                .IsUnique()
                .HasFilter("([AttachmentVariantDefinitionIdentifier] IS NOT NULL AND [AttachmentVariantParentID] IS NOT NULL)");

            entity.HasOne(d => d.AttachmentSite).WithMany(p => p.CmsAttachmentHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AttachmentHistory_AttachmentSiteID_CMS_Site");

            entity.HasOne(d => d.AttachmentVariantParent).WithMany(p => p.InverseAttachmentVariantParent).HasConstraintName("FK_CMS_AttachmentHistory_AttachmentVariantParentID_CMS_AttachmentHistory");
        });

        modelBuilder.Entity<CmsAutomationHistory>(entity =>
        {
            entity.Property(e => e.HistoryRejected).HasDefaultValueSql("((0))");
            entity.Property(e => e.HistoryStepDisplayName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.HistoryApprovedByUser).WithMany(p => p.CmsAutomationHistories).HasConstraintName("FK_CMS_AutomationHistory_HistoryApprovedByUserID");

            entity.HasOne(d => d.HistoryState).WithMany(p => p.CmsAutomationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AutomationHistory_HistoryStateID");

            entity.HasOne(d => d.HistoryStep).WithMany(p => p.CmsAutomationHistoryHistorySteps).HasConstraintName("FK_CMS_AutomationHistory_HistoryStepID");

            entity.HasOne(d => d.HistoryTargetStep).WithMany(p => p.CmsAutomationHistoryHistoryTargetSteps).HasConstraintName("FK_CMS_AutomationHistory_HistoryTargetStepID");

            entity.HasOne(d => d.HistoryWorkflow).WithMany(p => p.CmsAutomationHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AutomationHistory_HistoryWorkflowID");
        });

        modelBuilder.Entity<CmsAutomationState>(entity =>
        {
            entity.HasOne(d => d.StateSite).WithMany(p => p.CmsAutomationStates).HasConstraintName("FK_CMS_AutomationState_StateSiteID_CMS_Site");

            entity.HasOne(d => d.StateStep).WithMany(p => p.CmsAutomationStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AutomationState_StateStepID");

            entity.HasOne(d => d.StateUser).WithMany(p => p.CmsAutomationStates).HasConstraintName("FK_CMS_AutomationState_StateUserID_CMS_User");

            entity.HasOne(d => d.StateWorkflow).WithMany(p => p.CmsAutomationStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AutomationState_StateWorkflowID");
        });

        modelBuilder.Entity<CmsAutomationTemplate>(entity =>
        {
            entity.Property(e => e.TemplateDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.TemplateLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
        });

        modelBuilder.Entity<CmsAvatar>(entity =>
        {
            entity.HasKey(e => e.AvatarId).IsClustered(false);

            entity.HasIndex(e => e.AvatarName, "IX_CMS_Avatar_AvatarName").IsClustered();
        });

        modelBuilder.Entity<CmsCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => new { e.CategoryDisplayName, e.CategoryEnabled }, "IX_CMS_Category_CategoryDisplayName_CategoryEnabled").IsClustered();

            entity.Property(e => e.CategoryDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.CategoryEnabled).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.CategorySite).WithMany(p => p.CmsCategories).HasConstraintName("FK_CMS_Category_CategorySiteID_CMS_Site");

            entity.HasOne(d => d.CategoryUser).WithMany(p => p.CmsCategories).HasConstraintName("FK_CMS_Category_CategoryUserID_CMS_User");
        });

        modelBuilder.Entity<CmsClass>(entity =>
        {
            entity.HasKey(e => e.ClassId).IsClustered(false);

            entity.HasIndex(e => new { e.ClassId, e.ClassName, e.ClassDisplayName }, "IX_CMS_Class_ClassID_ClassName_ClassDisplayName").IsClustered();

            entity.HasOne(d => d.ClassResource).WithMany(p => p.CmsClasses).HasConstraintName("FK_CMS_Class_ClassResourceID_CMS_Resource");

            entity.HasMany(d => d.ChildClasses).WithMany(p => p.ParentClasses)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsAllowedChildClass",
                    r => r.HasOne<CmsClass>().WithMany()
                        .HasForeignKey("ChildClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_AllowedChildClasses_ChildClassID_CMS_Class"),
                    l => l.HasOne<CmsClass>().WithMany()
                        .HasForeignKey("ParentClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_AllowedChildClasses_ParentClassID_CMS_Class"),
                    j =>
                    {
                        j.HasKey("ParentClassId", "ChildClassId");
                        j.ToTable("CMS_AllowedChildClasses");
                        j.HasIndex(new[] { "ChildClassId" }, "IX_CMS_AllowedChildClasses_ChildClassID");
                        j.IndexerProperty<int>("ParentClassId").HasColumnName("ParentClassID");
                        j.IndexerProperty<int>("ChildClassId").HasColumnName("ChildClassID");
                    });

            entity.HasMany(d => d.ParentClasses).WithMany(p => p.ChildClasses)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsAllowedChildClass",
                    r => r.HasOne<CmsClass>().WithMany()
                        .HasForeignKey("ParentClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_AllowedChildClasses_ParentClassID_CMS_Class"),
                    l => l.HasOne<CmsClass>().WithMany()
                        .HasForeignKey("ChildClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_AllowedChildClasses_ChildClassID_CMS_Class"),
                    j =>
                    {
                        j.HasKey("ParentClassId", "ChildClassId");
                        j.ToTable("CMS_AllowedChildClasses");
                        j.HasIndex(new[] { "ChildClassId" }, "IX_CMS_AllowedChildClasses_ChildClassID");
                        j.IndexerProperty<int>("ParentClassId").HasColumnName("ParentClassID");
                        j.IndexerProperty<int>("ChildClassId").HasColumnName("ChildClassID");
                    });

            entity.HasMany(d => d.Sites).WithMany(p => p.Classes)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsClassSite",
                    r => r.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_Class_SiteID_CMS_Site"),
                    l => l.HasOne<CmsClass>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_Class_ClassID_CMS_Class"),
                    j =>
                    {
                        j.HasKey("ClassId", "SiteId");
                        j.ToTable("CMS_ClassSite");
                        j.HasIndex(new[] { "SiteId" }, "IX_CMS_ClassSite_SiteID");
                        j.IndexerProperty<int>("ClassId").HasColumnName("ClassID");
                        j.IndexerProperty<int>("SiteId").HasColumnName("SiteID");
                    });
        });

        modelBuilder.Entity<CmsConsent>(entity =>
        {
            entity.Property(e => e.ConsentContent).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ConsentDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ConsentHash).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ConsentLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.ConsentName).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsConsentAgreement>(entity =>
        {
            entity.Property(e => e.ConsentAgreementTime).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.ConsentAgreementConsent).WithMany(p => p.CmsConsentAgreements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ConsentAgreement_ConsentAgreementConsentID_CMS_Consent");

            entity.HasOne(d => d.ConsentAgreementContact).WithMany(p => p.CmsConsentAgreements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ConsentAgreement_ConsentAgreementContactID_OM_Contact");
        });

        modelBuilder.Entity<CmsConsentArchive>(entity =>
        {
            entity.Property(e => e.ConsentArchiveContent).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ConsentArchiveHash).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ConsentArchiveLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.ConsentArchiveConsent).WithMany(p => p.CmsConsentArchives)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ConsentArchive_ConsentArchiveConsentID_CMS_Consent");
        });

        modelBuilder.Entity<CmsCountry>(entity =>
        {
            entity.HasKey(e => e.CountryId).IsClustered(false);

            entity.HasIndex(e => e.CountryDisplayName, "IX_CMS_Country_CountryDisplayName").IsClustered();

            entity.Property(e => e.CountryDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.CountryLastModified).HasDefaultValueSql("('11/14/2013 1:43:04 PM')");
            entity.Property(e => e.CountryName).HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<CmsCulture>(entity =>
        {
            entity.HasKey(e => e.CultureId).IsClustered(false);

            entity.HasIndex(e => e.CultureName, "IX_CMS_Culture_CultureName").IsClustered();

            entity.Property(e => e.CultureIsUiculture).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<CmsDocument>(entity =>
        {
            entity.Property(e => e.DocumentCanBePublished).HasDefaultValueSql("((1))");
            entity.Property(e => e.DocumentCulture).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.DocumentCheckedOutByUser).WithMany(p => p.CmsDocumentDocumentCheckedOutByUsers).HasConstraintName("FK_CMS_Document_DocumentCheckedOutByUserID_CMS_User");

            entity.HasOne(d => d.DocumentCheckedOutVersionHistory).WithMany(p => p.CmsDocumentDocumentCheckedOutVersionHistories).HasConstraintName("FK_CMS_Document_DocumentCheckedOutVersionHistoryID_CMS_VersionHistory");

            entity.HasOne(d => d.DocumentCreatedByUser).WithMany(p => p.CmsDocumentDocumentCreatedByUsers).HasConstraintName("FK_CMS_Document_DocumentCreatedByUserID_CMS_User");

            entity.HasOne(d => d.DocumentModifiedByUser).WithMany(p => p.CmsDocumentDocumentModifiedByUsers).HasConstraintName("FK_CMS_Document_DocumentModifiedByUserID_CMS_User");

            entity.HasOne(d => d.DocumentNode).WithMany(p => p.CmsDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Document_DocumentNodeID_CMS_Tree");

            entity.HasOne(d => d.DocumentPublishedVersionHistory).WithMany(p => p.CmsDocumentDocumentPublishedVersionHistories).HasConstraintName("FK_CMS_Document_DocumentPublishedVersionHistoryID_CMS_VersionHistory");

            entity.HasOne(d => d.DocumentTagGroup).WithMany(p => p.CmsDocuments).HasConstraintName("FK_CMS_Document_DocumentTagGroupID_CMS_TagGroup");

            entity.HasOne(d => d.DocumentWorkflowStep).WithMany(p => p.CmsDocuments).HasConstraintName("FK_CMS_Document_DocumentWorkflowStepID_CMS_WorkflowStep");

            entity.HasMany(d => d.Categories).WithMany(p => p.Documents)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsDocumentCategory",
                    r => r.HasOne<CmsCategory>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_DocumentCategory_CategoryID_CMS_Category"),
                    l => l.HasOne<CmsDocument>().WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_DocumentCategory_DocumentID_CMS_Document"),
                    j =>
                    {
                        j.HasKey("DocumentId", "CategoryId");
                        j.ToTable("CMS_DocumentCategory");
                        j.HasIndex(new[] { "CategoryId" }, "IX_CMS_DocumentCategory_CategoryID");
                        j.IndexerProperty<int>("DocumentId").HasColumnName("DocumentID");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("CategoryID");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Documents)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsDocumentTag",
                    r => r.HasOne<CmsTag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_DocumentTag_TagID_CMS_Tag"),
                    l => l.HasOne<CmsDocument>().WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_DocumentTag_DocumentID_CMS_Document"),
                    j =>
                    {
                        j.HasKey("DocumentId", "TagId");
                        j.ToTable("CMS_DocumentTag");
                        j.HasIndex(new[] { "TagId" }, "IX_CMS_DocumentTag_TagID");
                        j.IndexerProperty<int>("DocumentId").HasColumnName("DocumentID");
                        j.IndexerProperty<int>("TagId").HasColumnName("TagID");
                    });
        });

        modelBuilder.Entity<CmsDocumentTypeScope>(entity =>
        {
            entity.HasKey(e => e.ScopeId).IsClustered(false);

            entity.HasIndex(e => e.ScopePath, "IX_CMS_DocumentTypeScope_ScopePath").IsClustered();

            entity.Property(e => e.ScopeAllowAllTypes).HasDefaultValueSql("((0))");
            entity.Property(e => e.ScopeAllowLinks).HasDefaultValueSql("((0))");
            entity.Property(e => e.ScopeLastModified).HasDefaultValueSql("('4/30/2013 2:47:21 PM')");
            entity.Property(e => e.ScopePath).HasDefaultValueSql("('')");

            entity.HasOne(d => d.ScopeSite).WithMany(p => p.CmsDocumentTypeScopes).HasConstraintName("FK_CMS_DocumentTypeScope_ScopeSiteID_CMS_Site");

            entity.HasMany(d => d.Classes).WithMany(p => p.Scopes)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsDocumentTypeScopeClass",
                    r => r.HasOne<CmsClass>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_DocumentTypeScopeClass_ClassID_CMS_Class"),
                    l => l.HasOne<CmsDocumentTypeScope>().WithMany()
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_DocumentTypeScopeClass_ScopeID_CMS_DocumentTypeScope"),
                    j =>
                    {
                        j.HasKey("ScopeId", "ClassId");
                        j.ToTable("CMS_DocumentTypeScopeClass");
                        j.HasIndex(new[] { "ClassId" }, "IX_CMS_DocumentTypeScopeClass_ClassID");
                        j.IndexerProperty<int>("ScopeId").HasColumnName("ScopeID");
                        j.IndexerProperty<int>("ClassId").HasColumnName("ClassID");
                    });
        });

        modelBuilder.Entity<CmsEmail>(entity =>
        {
            entity.Property(e => e.EmailFrom).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EmailIsMass).HasDefaultValueSql("((1))");
            entity.Property(e => e.EmailLastModified).HasDefaultValueSql("('6/17/2016 10:11:21 AM')");
            entity.Property(e => e.EmailSubject).HasDefaultValueSql("('')");

            entity.HasMany(d => d.Attachments).WithMany(p => p.Emails)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsAttachmentForEmail",
                    r => r.HasOne<CmsEmailAttachment>().WithMany()
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_AttachmentForEmail_AttachmentID_CMS_EmailAttachment"),
                    l => l.HasOne<CmsEmail>().WithMany()
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_AttachmentForEmail_EmailID_CMS_Email"),
                    j =>
                    {
                        j.HasKey("EmailId", "AttachmentId");
                        j.ToTable("CMS_AttachmentForEmail");
                        j.HasIndex(new[] { "AttachmentId" }, "IX_CMS_AttachmentForEmail_AttachmentID");
                        j.IndexerProperty<int>("EmailId").HasColumnName("EmailID");
                        j.IndexerProperty<int>("AttachmentId").HasColumnName("AttachmentID");
                    });
        });

        modelBuilder.Entity<CmsEmailTemplate>(entity =>
        {
            entity.HasKey(e => e.EmailTemplateId).IsClustered(false);

            entity.HasIndex(e => e.EmailTemplateDisplayName, "IX_CMS_EmailTemplate_EmailTemplateDisplayName").IsClustered();

            entity.Property(e => e.EmailTemplateDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.EmailTemplateName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.EmailTemplateSite).WithMany(p => p.CmsEmailTemplates).HasConstraintName("FK_CMS_Email_EmailTemplateSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsEmailUser>(entity =>
        {
            entity.HasOne(d => d.Email).WithMany(p => p.CmsEmailUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_EmailUser_EmailID_CMS_Email");

            entity.HasOne(d => d.User).WithMany(p => p.CmsEmailUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_EmailUser_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsEventLog>(entity =>
        {
            entity.Property(e => e.DocumentName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EventCode).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EventMachineName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EventTime).HasDefaultValueSql("('4/21/2015 8:21:43 AM')");
            entity.Property(e => e.EventType).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EventUrl).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EventUrlReferrer).HasDefaultValueSql("(N'')");
            entity.Property(e => e.Ipaddress).HasDefaultValueSql("(N'')");
            entity.Property(e => e.Source).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsExternalLogin>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.CmsExternalLogins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ExternalLogin_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsForm>(entity =>
        {
            entity.HasKey(e => e.FormId).IsClustered(false);

            entity.HasIndex(e => e.FormDisplayName, "IX_CMS_Form_FormDisplayName").IsClustered();

            entity.Property(e => e.FormConfirmationEmailSubject).HasDefaultValueSql("(N'')");
            entity.Property(e => e.FormDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.FormEmailAttachUploadedDocs).HasDefaultValueSql("((0))");
            entity.Property(e => e.FormLastModified).HasDefaultValueSql("('9/17/2012 1:37:08 PM')");
            entity.Property(e => e.FormLogActivity).HasDefaultValueSql("((1))");
            entity.Property(e => e.FormName).HasDefaultValueSql("('')");
            entity.Property(e => e.FormSubmitButtonText).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.FormClass).WithMany(p => p.CmsForms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Form_FormClassID_CMS_Class");

            entity.HasOne(d => d.FormSite).WithMany(p => p.CmsForms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Form_FormSiteID_CMS_Site");

            entity.HasMany(d => d.Roles).WithMany(p => p.Forms)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsFormRole",
                    r => r.HasOne<CmsRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_FormRole_RoleID_CMS_Role"),
                    l => l.HasOne<CmsForm>().WithMany()
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_FormRole_FormID_CMS_Form"),
                    j =>
                    {
                        j.HasKey("FormId", "RoleId");
                        j.ToTable("CMS_FormRole");
                        j.HasIndex(new[] { "RoleId" }, "IX_CMS_FormRole_RoleID");
                        j.IndexerProperty<int>("FormId").HasColumnName("FormID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        modelBuilder.Entity<CmsFormUserControl>(entity =>
        {
            entity.HasKey(e => e.UserControlId).IsClustered(false);

            entity.HasIndex(e => e.UserControlDisplayName, "IX_CMS_FormUserControl_UserControlDisplayName").IsClustered();

            entity.Property(e => e.UserControlIsSystem).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserControlPriority).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserControlShowInCustomTables).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.UserControlParent).WithMany(p => p.InverseUserControlParent).HasConstraintName("FK_CMS_FormUserControl_UserControlParentID_CMS_FormUserControl");

            entity.HasOne(d => d.UserControlResource).WithMany(p => p.CmsFormUserControls).HasConstraintName("FK_CMS_FormUserControl_UserControlResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsHelpTopic>(entity =>
        {
            entity.Property(e => e.HelpTopicLink).HasDefaultValueSql("(N'')");
            entity.Property(e => e.HelpTopicName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.HelpTopicUielement).WithMany(p => p.CmsHelpTopics)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_HelpTopic_HelpTopicUIElementID_CMS_UIElement");
        });

        modelBuilder.Entity<CmsLayout>(entity =>
        {
            entity.Property(e => e.LayoutCode).HasDefaultValueSql("('<cms:CMSWebPartZone ZoneID=\"zoneA\" runat=\"server\" />')");
            entity.Property(e => e.LayoutCodeName).HasDefaultValueSql("('')");
            entity.Property(e => e.LayoutDisplayName).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsLicenseKey>(entity =>
        {
            entity.HasKey(e => e.LicenseKeyId).IsClustered(false);

            entity.HasIndex(e => e.LicenseDomain, "IX_CMS_LicenseKey_LicenseDomain").IsClustered();
        });

        modelBuilder.Entity<CmsMacroIdentity>(entity =>
        {
            entity.Property(e => e.MacroIdentityLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.MacroIdentityName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.MacroIdentityEffectiveUser).WithMany(p => p.CmsMacroIdentities).HasConstraintName("FK_CMS_MacroIdentity_MacroIdentityEffectiveUserID_CMS_User");
        });

        modelBuilder.Entity<CmsMacroRule>(entity =>
        {
            entity.Property(e => e.MacroRuleAvailability).HasDefaultValueSql("((0))");
            entity.Property(e => e.MacroRuleCondition).HasDefaultValueSql("(N'')");
            entity.Property(e => e.MacroRuleDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.MacroRuleEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.MacroRuleIsCustom).HasDefaultValueSql("((0))");
            entity.Property(e => e.MacroRuleLastModified).HasDefaultValueSql("('5/1/2012 8:46:33 AM')");
        });

        modelBuilder.Entity<CmsMembership>(entity =>
        {
            entity.HasOne(d => d.MembershipSite).WithMany(p => p.CmsMemberships).HasConstraintName("FK_CMS_Membership_MembershipSiteID_CMS_Site");

            entity.HasMany(d => d.Roles).WithMany(p => p.Memberships)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsMembershipRole",
                    r => r.HasOne<CmsRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_MembershipRole_RoleID_CMS_Role"),
                    l => l.HasOne<CmsMembership>().WithMany()
                        .HasForeignKey("MembershipId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_MembershipRole_MembershipID_CMS_Membership"),
                    j =>
                    {
                        j.HasKey("MembershipId", "RoleId");
                        j.ToTable("CMS_MembershipRole");
                        j.HasIndex(new[] { "RoleId" }, "IX_CMS_MembershipRole_RoleID");
                        j.IndexerProperty<int>("MembershipId").HasColumnName("MembershipID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        modelBuilder.Entity<CmsMembershipUser>(entity =>
        {
            entity.HasOne(d => d.Membership).WithMany(p => p.CmsMembershipUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_MembershipUser_MembershipID_CMS_Membership");

            entity.HasOne(d => d.User).WithMany(p => p.CmsMembershipUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_MembershipUser_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsMetaFile>(entity =>
        {
            entity.HasKey(e => e.MetaFileId).IsClustered(false);

            entity.HasIndex(e => new { e.MetaFileObjectType, e.MetaFileObjectId, e.MetaFileGroupName }, "IX_CMS_Metafile_MetaFileObjectType_MetaFileObjectID_MetaFileGroupName").IsClustered();

            entity.HasOne(d => d.MetaFileSite).WithMany(p => p.CmsMetaFiles).HasConstraintName("FK_CMS_MetaFile_MetaFileSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsModuleLicenseKey>(entity =>
        {
            entity.Property(e => e.ModuleLicenseKeyLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.ModuleLicenseKeyLicense).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ModuleLicenseKeyResource).WithMany(p => p.CmsModuleLicenseKeys)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ModuleLicenseKey_ModuleLicenseKeyResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsModuleUsageCounter>(entity =>
        {
            entity.HasIndex(e => e.ModuleUsageCounterName, "IX_CMS_ModuleUsageCounter_ModuleUsageCounterName")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.ModuleUsageCounterId).ValueGeneratedOnAdd();
            entity.Property(e => e.ModuleUsageCounterName).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsObjectSetting>(entity =>
        {
            entity.Property(e => e.ObjectSettingsObjectType).HasDefaultValueSql("('')");
            entity.Property(e => e.ObjectWorkflowSendEmails).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.ObjectCheckedOutByUser).WithMany(p => p.CmsObjectSettings).HasConstraintName("FK_CMS_ObjectSettings_ObjectCheckedOutByUserID_CMS_User");

            entity.HasOne(d => d.ObjectCheckedOutVersionHistory).WithMany(p => p.CmsObjectSettingObjectCheckedOutVersionHistories).HasConstraintName("FK_CMS_ObjectSettings_ObjectCheckedOutVersionHistoryID_CMS_ObjectVersionHistory");

            entity.HasOne(d => d.ObjectPublishedVersionHistory).WithMany(p => p.CmsObjectSettingObjectPublishedVersionHistories).HasConstraintName("FK_CMS_ObjectSettings_ObjectPublishedVersionHistoryID_CMS_ObjectVersionHistory");

            entity.HasOne(d => d.ObjectWorkflowStep).WithMany(p => p.CmsObjectSettings).HasConstraintName("FK_CMS_ObjectSettings_ObjectWorkflowStepID_CMS_WorkflowStep");
        });

        modelBuilder.Entity<CmsObjectVersionHistory>(entity =>
        {
            entity.HasKey(e => e.VersionId)
                .HasName("PK_CMS_ObjectVersionHistory_VersionID")
                .IsClustered(false);

            entity.HasIndex(e => new { e.VersionObjectType, e.VersionObjectId, e.VersionId }, "PK_CMS_ObjectVersionHistory")
                .IsUnique()
                .IsDescending(false, false, true)
                .IsClustered();

            entity.Property(e => e.VersionNumber).HasDefaultValueSql("('')");

            entity.HasOne(d => d.VersionDeletedByUser).WithMany(p => p.CmsObjectVersionHistoryVersionDeletedByUsers).HasConstraintName("FK_CMS_ObjectVersionHistory_VersionDeletedByUserID_CMS_User");

            entity.HasOne(d => d.VersionModifiedByUser).WithMany(p => p.CmsObjectVersionHistoryVersionModifiedByUsers).HasConstraintName("FK_CMS_ObjectVersionHistory_VersionModifiedByUserID_CMS_User");

            entity.HasOne(d => d.VersionObjectSite).WithMany(p => p.CmsObjectVersionHistories).HasConstraintName("FK_CMS_ObjectVersionHistory_VersionObjectSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsObjectWorkflowTrigger>(entity =>
        {
            entity.Property(e => e.TriggerDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.TriggerObjectType).HasDefaultValueSql("('')");

            entity.HasOne(d => d.TriggerWorkflow).WithMany(p => p.CmsObjectWorkflowTriggers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ObjectWorkflowTrigger_TriggerWorkflowID");
        });

        modelBuilder.Entity<CmsPageFormerUrlPath>(entity =>
        {
            entity.Property(e => e.PageFormerUrlPathCulture).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PageFormerUrlPathLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.PageFormerUrlPathUrlPath).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PageFormerUrlPathUrlPathHash).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.PageFormerUrlPathNode).WithMany(p => p.CmsPageFormerUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_PageFormerUrlPath_PageFormerUrlPathNodeID_CMS_Tree");

            entity.HasOne(d => d.PageFormerUrlPathSite).WithMany(p => p.CmsPageFormerUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_PageFormerUrlPath_PageFormerUrlPathSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsPageTemplate>(entity =>
        {
            entity.HasKey(e => e.PageTemplateId).IsClustered(false);

            entity.HasIndex(e => e.PageTemplateCategoryId, "IX_CMS_PageTemplate_PageTemplateCategoryID").IsClustered();

            entity.Property(e => e.PageTemplateCodeName).HasDefaultValueSql("('')");
            entity.Property(e => e.PageTemplateDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.PageTemplateIconClass).HasDefaultValueSql("(N'icon-layout')");
            entity.Property(e => e.PageTemplateIsLayout).HasDefaultValueSql("((0))");
            entity.Property(e => e.PageTemplateType).HasDefaultValueSql("(N'portal')");

            entity.HasOne(d => d.PageTemplateCategory).WithMany(p => p.CmsPageTemplates).HasConstraintName("FK_CMS_PageTemplate_PageTemplateCategoryID_CMS_PageTemplateCategory");

            entity.HasOne(d => d.PageTemplateLayoutNavigation).WithMany(p => p.CmsPageTemplates).HasConstraintName("FK_CMS_PageTemplate_PageTemplateLayoutID_CMS_Layout");
        });

        modelBuilder.Entity<CmsPageTemplateCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => e.CategoryPath, "IX_CMS_PageTemplateCategory_CategoryPath")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.CategoryChildCount).HasDefaultValueSql("((0))");
            entity.Property(e => e.CategoryDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.CategoryTemplateChildCount).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.CategoryParent).WithMany(p => p.InverseCategoryParent).HasConstraintName("FK_CMS_PageTemplateCategory_CategoryParentID_CMS_PageTemplateCategory");
        });

        modelBuilder.Entity<CmsPageTemplateConfiguration>(entity =>
        {
            entity.Property(e => e.PageTemplateConfigurationLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.PageTemplateConfigurationName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PageTemplateConfigurationTemplate).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.PageTemplateConfigurationSite).WithMany(p => p.CmsPageTemplateConfigurations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_PageTemplateConfiguration_PageTemplateConfigurationSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsPageUrlPath>(entity =>
        {
            entity.Property(e => e.PageUrlPathCulture).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PageUrlPathLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.PageUrlPathUrlPath).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PageUrlPathUrlPathHash).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.PageUrlPathNode).WithMany(p => p.CmsPageUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_PageUrlPath_PageUrlPathNodeID_CMS_Tree");

            entity.HasOne(d => d.PageUrlPathSite).WithMany(p => p.CmsPageUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_PageUrlPath_PageUrlPathSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsPermission>(entity =>
        {
            entity.Property(e => e.PermissionDisplayInMatrix).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Class).WithMany(p => p.CmsPermissions).HasConstraintName("FK_CMS_Permission_ClassID_CMS_Class");

            entity.HasOne(d => d.Resource).WithMany(p => p.CmsPermissions).HasConstraintName("FK_CMS_Permission_ResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsPersonalization>(entity =>
        {
            entity.Property(e => e.PersonalizationLastModified).HasDefaultValueSql("('9/2/2008 5:36:59 PM')");

            entity.HasOne(d => d.PersonalizationSite).WithMany(p => p.CmsPersonalizations).HasConstraintName("FK_CMS_Personalization_PersonalizationSiteID_CMS_Site");

            entity.HasOne(d => d.PersonalizationUser).WithMany(p => p.CmsPersonalizations).HasConstraintName("FK_CMS_Personalization_PersonalizationUserID_CMS_User");
        });

        modelBuilder.Entity<CmsQuery>(entity =>
        {
            entity.Property(e => e.QueryIsCustom).HasDefaultValueSql("((0))");
            entity.Property(e => e.QueryName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.Class).WithMany(p => p.CmsQueries).HasConstraintName("FK_CMS_Query_ClassID_CMS_Class");
        });

        modelBuilder.Entity<CmsRelationship>(entity =>
        {
            entity.HasOne(d => d.LeftNode).WithMany(p => p.CmsRelationshipLeftNodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Relationship_LeftNodeID_CMS_Tree");

            entity.HasOne(d => d.RelationshipName).WithMany(p => p.CmsRelationships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Relationship_RelationshipNameID_CMS_RelationshipName");

            entity.HasOne(d => d.RightNode).WithMany(p => p.CmsRelationshipRightNodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Relationship_RightNodeID_CMS_Tree");
        });

        modelBuilder.Entity<CmsRelationshipName>(entity =>
        {
            entity.Property(e => e.RelationshipDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.RelationshipName).HasDefaultValueSql("('')");

            entity.HasMany(d => d.Sites).WithMany(p => p.RelationshipNames)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsRelationshipNameSite",
                    r => r.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RelationshipNameSite_SiteID_CMS_Site"),
                    l => l.HasOne<CmsRelationshipName>().WithMany()
                        .HasForeignKey("RelationshipNameId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RelationshipNameSite_RelationshipNameID_CMS_RelationshipName"),
                    j =>
                    {
                        j.HasKey("RelationshipNameId", "SiteId");
                        j.ToTable("CMS_RelationshipNameSite");
                        j.HasIndex(new[] { "SiteId" }, "IX_CMS_RelationshipNameSite_SiteID");
                        j.IndexerProperty<int>("RelationshipNameId").HasColumnName("RelationshipNameID");
                        j.IndexerProperty<int>("SiteId").HasColumnName("SiteID");
                    });
        });

        modelBuilder.Entity<CmsResource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).IsClustered(false);

            entity.HasIndex(e => e.ResourceDisplayName, "IX_CMS_Resource_ResourceDisplayName").IsClustered();

            entity.Property(e => e.ResourceHasFiles).HasDefaultValueSql("((0))");
            entity.Property(e => e.ResourceInstallationState).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ResourceInstalledVersion).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ShowInDevelopment).HasDefaultValueSql("((0))");

            entity.HasMany(d => d.Sites).WithMany(p => p.Resources)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsResourceSite",
                    r => r.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_ResourceSite_SiteID_CMS_Site"),
                    l => l.HasOne<CmsResource>().WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_ResourceSite_ResourceID_CMS_Resource"),
                    j =>
                    {
                        j.HasKey("ResourceId", "SiteId");
                        j.ToTable("CMS_ResourceSite");
                        j.HasIndex(new[] { "SiteId" }, "IX_CMS_ResourceSite_SiteID");
                        j.IndexerProperty<int>("ResourceId").HasColumnName("ResourceID");
                        j.IndexerProperty<int>("SiteId").HasColumnName("SiteID");
                    });
        });

        modelBuilder.Entity<CmsResourceLibrary>(entity =>
        {
            entity.Property(e => e.ResourceLibraryPath).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ResourceLibraryResource).WithMany(p => p.CmsResourceLibraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ResourceLibrary_CMS_Resource");
        });

        modelBuilder.Entity<CmsResourceTranslation>(entity =>
        {
            entity.HasOne(d => d.TranslationCulture).WithMany(p => p.CmsResourceTranslations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ResourceTranslation_TranslationCultureID_CMS_Culture");

            entity.HasOne(d => d.TranslationString).WithMany(p => p.CmsResourceTranslations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ResourceTranslation_TranslationStringID_CMS_ResourceString");
        });

        modelBuilder.Entity<CmsRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).IsClustered(false);

            entity.HasIndex(e => new { e.SiteId, e.RoleName, e.RoleDisplayName }, "IX_CMS_Role_SiteID_RoleName_RoleDisplayName")
                .IsUnique()
                .IsClustered();

            entity.HasOne(d => d.Site).WithMany(p => p.CmsRoles).HasConstraintName("FK_CMS_Role_SiteID_CMS_SiteID");

            entity.HasMany(d => d.Elements).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsRoleApplication",
                    r => r.HasOne<CmsUielement>().WithMany()
                        .HasForeignKey("ElementId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RoleApplication_CMS_UIElement"),
                    l => l.HasOne<CmsRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RoleApplication_CMS_Role"),
                    j =>
                    {
                        j.HasKey("RoleId", "ElementId");
                        j.ToTable("CMS_RoleApplication");
                        j.HasIndex(new[] { "ElementId" }, "IX_CMS_RoleApplication");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                        j.IndexerProperty<int>("ElementId").HasColumnName("ElementID");
                    });

            entity.HasMany(d => d.ElementsNavigation).WithMany(p => p.RolesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsRoleUielement",
                    r => r.HasOne<CmsUielement>().WithMany()
                        .HasForeignKey("ElementId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RoleUIElement_ElementID_CMS_UIElement"),
                    l => l.HasOne<CmsRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RoleUIElement_RoleID_CMS_Role"),
                    j =>
                    {
                        j.HasKey("RoleId", "ElementId");
                        j.ToTable("CMS_RoleUIElement");
                        j.HasIndex(new[] { "ElementId" }, "IX_CMS_RoleUIElement_ElementID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                        j.IndexerProperty<int>("ElementId").HasColumnName("ElementID");
                    });

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsRolePermission",
                    r => r.HasOne<CmsPermission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RolePermission_PermissionID_CMS_Permission"),
                    l => l.HasOne<CmsRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_RolePermission_RoleID_CMS_Role"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId");
                        j.ToTable("CMS_RolePermission");
                        j.HasIndex(new[] { "PermissionId" }, "IX_CMS_RolePermission_PermissionID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("PermissionID");
                    });
        });

        modelBuilder.Entity<CmsScheduledTask>(entity =>
        {
            entity.Property(e => e.TaskAllowExternalService).HasDefaultValueSql("((0))");
            entity.Property(e => e.TaskExecutingServerName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.TaskResource).WithMany(p => p.CmsScheduledTasks).HasConstraintName("FK_CMS_ScheduledTask_TaskResourceID_CMS_Resource");

            entity.HasOne(d => d.TaskSite).WithMany(p => p.CmsScheduledTasks).HasConstraintName("FK_CMS_ScheduledTask_TaskSiteID_CMS_Site");

            entity.HasOne(d => d.TaskUser).WithMany(p => p.CmsScheduledTasks).HasConstraintName("FK_CMS_ScheduledTask_TaskUserID_CMS_User");
        });

        modelBuilder.Entity<CmsSearchIndex>(entity =>
        {
            entity.HasKey(e => e.IndexId).IsClustered(false);

            entity.HasIndex(e => e.IndexDisplayName, "IX_CMS_SearchIndex_IndexDisplayName").IsClustered();

            entity.Property(e => e.IndexProvider).HasDefaultValueSql("(N'')");
            entity.Property(e => e.IndexType).HasDefaultValueSql("('')");

            entity.HasMany(d => d.IndexCultures).WithMany(p => p.Indices)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsSearchIndexCulture",
                    r => r.HasOne<CmsCulture>().WithMany()
                        .HasForeignKey("IndexCultureId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SearchIndexCulture_IndexCultureID_CMS_Culture"),
                    l => l.HasOne<CmsSearchIndex>().WithMany()
                        .HasForeignKey("IndexId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SearchIndexCulture_IndexID_CMS_SearchIndex"),
                    j =>
                    {
                        j.HasKey("IndexId", "IndexCultureId");
                        j.ToTable("CMS_SearchIndexCulture");
                        j.HasIndex(new[] { "IndexCultureId" }, "IX_CMS_SearchIndexCulture_IndexCultureID");
                        j.IndexerProperty<int>("IndexId").HasColumnName("IndexID");
                        j.IndexerProperty<int>("IndexCultureId").HasColumnName("IndexCultureID");
                    });

            entity.HasMany(d => d.IndexSites).WithMany(p => p.Indices)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsSearchIndexSite",
                    r => r.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("IndexSiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SearchIndexSite_IndexSiteID_CMS_Site"),
                    l => l.HasOne<CmsSearchIndex>().WithMany()
                        .HasForeignKey("IndexId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SearchIndexSite_IndexID_CMS_SearchIndex"),
                    j =>
                    {
                        j.HasKey("IndexId", "IndexSiteId");
                        j.ToTable("CMS_SearchIndexSite");
                        j.HasIndex(new[] { "IndexSiteId" }, "IX_CMS_SearchIndexSite_IndexSiteID");
                        j.IndexerProperty<int>("IndexId").HasColumnName("IndexID");
                        j.IndexerProperty<int>("IndexSiteId").HasColumnName("IndexSiteID");
                    });
        });

        modelBuilder.Entity<CmsSearchTask>(entity =>
        {
            entity.HasKey(e => e.SearchTaskId).IsClustered(false);

            entity.HasIndex(e => new { e.SearchTaskPriority, e.SearchTaskStatus, e.SearchTaskServerName }, "IX_CMS_SearchTask_SearchTaskPriority_SearchTaskStatus_SearchTaskServerName")
                .IsDescending(true, false, false)
                .IsClustered();

            entity.Property(e => e.SearchTaskCreated).HasDefaultValueSql("('4/15/2009 11:23:52 AM')");
            entity.Property(e => e.SearchTaskStatus).HasDefaultValueSql("('')");
            entity.Property(e => e.SearchTaskType).HasDefaultValueSql("('')");
            entity.Property(e => e.SearchTaskValue).HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<CmsSearchTaskAzure>(entity =>
        {
            entity.Property(e => e.SearchTaskAzureAdditionalData).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SearchTaskAzureCreated).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.SearchTaskAzureType).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsSettingsCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => e.CategoryOrder, "IX_CMS_SettingsCategory_CategoryOrder").IsClustered();

            entity.Property(e => e.CategoryDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.CategoryIsCustom).HasDefaultValueSql("((0))");
            entity.Property(e => e.CategoryIsGroup).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.CategoryParent).WithMany(p => p.InverseCategoryParent).HasConstraintName("FK_CMS_SettingsCategory_CMS_SettingsCategory1");

            entity.HasOne(d => d.CategoryResource).WithMany(p => p.CmsSettingsCategories).HasConstraintName("FK_CMS_SettingsCategory_CategoryResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsSettingsKey>(entity =>
        {
            entity.Property(e => e.KeyDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.KeyExplanationText).HasDefaultValueSql("(N'')");
            entity.Property(e => e.KeyIsCustom).HasDefaultValueSql("((0))");
            entity.Property(e => e.KeyIsGlobal).HasDefaultValueSql("((0))");
            entity.Property(e => e.KeyIsHidden).HasDefaultValueSql("((0))");
            entity.Property(e => e.KeyName).HasDefaultValueSql("('')");
            entity.Property(e => e.KeyType).HasDefaultValueSql("('')");

            entity.HasOne(d => d.KeyCategory).WithMany(p => p.CmsSettingsKeys).HasConstraintName("FK_CMS_SettingsKey_KeyCategoryID_CMS_SettingsCategory");

            entity.HasOne(d => d.Site).WithMany(p => p.CmsSettingsKeys).HasConstraintName("FK_CMS_SettingsKey_SiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsSite>(entity =>
        {
            entity.HasKey(e => e.SiteId).IsClustered(false);

            entity.HasIndex(e => e.SiteDisplayName, "IX_CMS_Site_SiteDisplayName").IsClustered();

            entity.Property(e => e.SiteDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.SiteDomainName).HasDefaultValueSql("('')");
            entity.Property(e => e.SiteName).HasDefaultValueSql("('')");
            entity.Property(e => e.SitePresentationUrl).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SiteStatus).HasDefaultValueSql("('')");

            entity.HasMany(d => d.Cultures).WithMany(p => p.Sites)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsSiteCulture",
                    r => r.HasOne<CmsCulture>().WithMany()
                        .HasForeignKey("CultureId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SiteCulture_CultureID_CMS_Culture"),
                    l => l.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SiteCulture_SiteID_CMS_Site"),
                    j =>
                    {
                        j.HasKey("SiteId", "CultureId");
                        j.ToTable("CMS_SiteCulture");
                        j.HasIndex(new[] { "CultureId" }, "IX_CMS_SiteCulture_CultureID");
                        j.IndexerProperty<int>("SiteId").HasColumnName("SiteID");
                        j.IndexerProperty<int>("CultureId").HasColumnName("CultureID");
                    });
        });

        modelBuilder.Entity<CmsSiteDomainAlias>(entity =>
        {
            entity.HasOne(d => d.Site).WithMany(p => p.CmsSiteDomainAliases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_SiteDomainAlias_SiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsSmtpserver>(entity =>
        {
            entity.Property(e => e.ServerDeliveryMethod).HasDefaultValueSql("((0))");

            entity.HasMany(d => d.Sites).WithMany(p => p.Servers)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsSmtpserverSite",
                    r => r.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SMTPServerSite_CMS_Site"),
                    l => l.HasOne<CmsSmtpserver>().WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_SMTPServerSite_CMS_SMTPServer"),
                    j =>
                    {
                        j.HasKey("ServerId", "SiteId");
                        j.ToTable("CMS_SMTPServerSite");
                        j.HasIndex(new[] { "SiteId" }, "IX_CMS_SMTPServerSite_SiteID");
                        j.IndexerProperty<int>("ServerId").HasColumnName("ServerID");
                        j.IndexerProperty<int>("SiteId").HasColumnName("SiteID");
                    });
        });

        modelBuilder.Entity<CmsState>(entity =>
        {
            entity.HasKey(e => e.StateId).IsClustered(false);

            entity.HasIndex(e => e.StateDisplayName, "IX_CMS_State_CountryID_StateDisplayName").IsClustered();

            entity.Property(e => e.StateDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.StateName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.Country).WithMany(p => p.CmsStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_State_CountryID_CMS_Country");
        });

        modelBuilder.Entity<CmsTag>(entity =>
        {
            entity.HasKey(e => e.TagId).IsClustered(false);

            entity.HasIndex(e => e.TagName, "IX_CMS_Tag_TagName").IsClustered();

            entity.HasOne(d => d.TagGroup).WithMany(p => p.CmsTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Tag_TagGroupID_CMS_TagGroup");
        });

        modelBuilder.Entity<CmsTagGroup>(entity =>
        {
            entity.HasKey(e => e.TagGroupId).IsClustered(false);

            entity.HasIndex(e => e.TagGroupDisplayName, "IX_CMS_TagGroup_TagGroupDisplayName").IsClustered();

            entity.Property(e => e.TagGroupDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.TagGroupName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.TagGroupSite).WithMany(p => p.CmsTagGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_TagGroup_TagGroupSiteID_CMS_Site");
        });

        modelBuilder.Entity<CmsTimeZone>(entity =>
        {
            entity.HasKey(e => e.TimeZoneId).IsClustered(false);

            entity.HasIndex(e => e.TimeZoneDisplayName, "IX_CMS_TimeZone_TimeZoneDisplayName").IsClustered();

            entity.Property(e => e.TimeZoneDaylight).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<CmsTransformation>(entity =>
        {
            entity.HasKey(e => e.TransformationId).IsClustered(false);

            entity.HasIndex(e => new { e.TransformationClassId, e.TransformationName }, "IX_CMS_Transformation_TransformationClassID_TransformationName").IsClustered();

            entity.Property(e => e.TransformationCode).HasDefaultValueSql("(N'')");
            entity.Property(e => e.TransformationName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.TransformationType).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.TransformationClass).WithMany(p => p.CmsTransformations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Transformation_TransformationClassID_CMS_Class");
        });

        modelBuilder.Entity<CmsTranslationService>(entity =>
        {
            entity.Property(e => e.TranslationServiceSupportsCancel).HasDefaultValueSql("((0))");
            entity.Property(e => e.TranslationServiceSupportsStatusUpdate).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<CmsTranslationSubmission>(entity =>
        {
            entity.Property(e => e.SubmissionSourceCulture).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SubmissionTargetCulture).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.SubmissionService).WithMany(p => p.CmsTranslationSubmissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_TranslationSubmission_CMS_TranslationService");

            entity.HasOne(d => d.SubmissionSubmittedByUser).WithMany(p => p.CmsTranslationSubmissions).HasConstraintName("FK_CMS_TranslationSubmission_CMS_User");
        });

        modelBuilder.Entity<CmsTranslationSubmissionItem>(entity =>
        {
            entity.HasOne(d => d.SubmissionItemSubmission).WithMany(p => p.CmsTranslationSubmissionItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_TranslationSubmissionItem_CMS_TranslationSubmission");
        });

        modelBuilder.Entity<CmsTree>(entity =>
        {
            entity.Property(e => e.NodeHasChildren).HasDefaultValueSql("((0))");
            entity.Property(e => e.NodeHasLinks).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.NodeAcl).WithMany(p => p.CmsTrees).HasConstraintName("FK_CMS_Tree_NodeACLID_CMS_ACL");

            entity.HasOne(d => d.NodeClass).WithMany(p => p.CmsTrees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Tree_NodeClassID_CMS_Class");

            entity.HasOne(d => d.NodeLinkedNode).WithMany(p => p.InverseNodeLinkedNode).HasConstraintName("FK_CMS_Tree_NodeLinkedNodeID_CMS_Tree");

            entity.HasOne(d => d.NodeLinkedNodeSite).WithMany(p => p.CmsTreeNodeLinkedNodeSites).HasConstraintName("FK_CMS_Tree_NodeLinkedNodeSiteID_CMS_Site");

            entity.HasOne(d => d.NodeOriginalNode).WithMany(p => p.InverseNodeOriginalNode).HasConstraintName("FK_CMS_Tree_NodeOriginalNodeID_CMS_Tree");

            entity.HasOne(d => d.NodeOwnerNavigation).WithMany(p => p.CmsTrees).HasConstraintName("FK_CMS_Tree_NodeOwner_CMS_User");

            entity.HasOne(d => d.NodeParent).WithMany(p => p.InverseNodeParent).HasConstraintName("FK_CMS_Tree_NodeParentID_CMS_Tree");

            entity.HasOne(d => d.NodeSite).WithMany(p => p.CmsTreeNodeSites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Tree_NodeSiteID_CMS_Site");

            entity.HasOne(d => d.NodeSku).WithMany(p => p.CmsTrees).HasConstraintName("FK_CMS_Tree_NodeSKUID_COM_SKU");
        });

        modelBuilder.Entity<CmsUielement>(entity =>
        {
            entity.HasKey(e => e.ElementId).IsClustered(false);

            entity.HasIndex(e => new { e.ElementResourceId, e.ElementLevel, e.ElementParentId, e.ElementOrder, e.ElementCaption }, "IX_CMS_UIElement_ElementResourceID_ElementLevel_ElementParentID_ElementOrder_ElementCaption").IsClustered();

            entity.Property(e => e.ElementCheckModuleReadPermission).HasDefaultValueSql("((1))");
            entity.Property(e => e.ElementIsCustom).HasDefaultValueSql("((0))");
            entity.Property(e => e.ElementIsGlobalApplication).HasDefaultValueSql("((0))");
            entity.Property(e => e.ElementIsMenu).HasDefaultValueSql("((0))");
            entity.Property(e => e.ElementName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ElementSize).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ElementPageTemplate).WithMany(p => p.CmsUielements).HasConstraintName("FK_CMS_UIElement_ElementPageTemplateID_CMS_PageTemplate");

            entity.HasOne(d => d.ElementParent).WithMany(p => p.InverseElementParent).HasConstraintName("FK_CMS_UIElement_ElementParentID_CMS_UIElement");

            entity.HasOne(d => d.ElementResource).WithMany(p => p.CmsUielements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UIElement_ElementResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsUser>(entity =>
        {
            entity.Property(e => e.UserIsDomain).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserIsExternal).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserIsHidden).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserName).HasDefaultValueSql("('')");
            entity.Property(e => e.UserPassword).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsUserCulture>(entity =>
        {
            entity.HasOne(d => d.Culture).WithMany(p => p.CmsUserCultures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserCulture_CultureID_CMS_Culture");

            entity.HasOne(d => d.Site).WithMany(p => p.CmsUserCultures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserCulture_SiteID_CMS_Site");

            entity.HasOne(d => d.User).WithMany(p => p.CmsUserCultures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserCulture_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsUserMacroIdentity>(entity =>
        {
            entity.Property(e => e.UserMacroIdentityLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.UserMacroIdentityMacroIdentity).WithMany(p => p.CmsUserMacroIdentities).HasConstraintName("FK_CMS_UserMacroIdentity_UserMacroIdentityMacroIdentityID_CMS_MacroIdentity");

            entity.HasOne(d => d.UserMacroIdentityUser).WithOne(p => p.CmsUserMacroIdentity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserMacroIdentity_UserMacroIdentityUserID_CMS_User");
        });

        modelBuilder.Entity<CmsUserRole>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.CmsUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserRole_RoleID_CMS_Role");

            entity.HasOne(d => d.User).WithMany(p => p.CmsUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserRole_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsUserSetting>(entity =>
        {
            entity.Property(e => e.UserAccountLockReason).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserInvalidLogOnAttempts).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserWaitingForApproval).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.UserActivatedByUser).WithMany(p => p.CmsUserSettingUserActivatedByUsers).HasConstraintName("FK_CMS_UserSettings_UserActivatedByUserID_CMS_User");

            entity.HasOne(d => d.UserAvatar).WithMany(p => p.CmsUserSettings).HasConstraintName("FK_CMS_UserSettings_UserAvatarID_CMS_Avatar");

            entity.HasOne(d => d.UserSettingsUser).WithMany(p => p.CmsUserSettingUserSettingsUsers)
                .HasPrincipalKey(p => p.UserGuid)
                .HasForeignKey(d => d.UserSettingsUserGuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserSettings_UserSettingsUserGUID_CMS_User");

            entity.HasOne(d => d.UserSettingsUserNavigation).WithOne(p => p.CmsUserSettingUserSettingsUserNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserSettings_UserSettingsUserID_CMS_User");

            entity.HasOne(d => d.UserTimeZone).WithMany(p => p.CmsUserSettings).HasConstraintName("FK_CMS_UserSettings_UserTimeZoneID_CMS_TimeZone");
        });

        modelBuilder.Entity<CmsUserSite>(entity =>
        {
            entity.HasOne(d => d.Site).WithMany(p => p.CmsUserSites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserSite_SiteID_CMS_Site");

            entity.HasOne(d => d.User).WithMany(p => p.CmsUserSites)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_UserSite_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsVersionHistory>(entity =>
        {
            entity.HasKey(e => e.VersionHistoryId).IsClustered(false);

            entity.HasIndex(e => e.DocumentId, "IX_CMS_VersionHistory_DocumentID").IsClustered();

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.CmsVersionHistoryModifiedByUsers).HasConstraintName("FK_CMS_VersionHistory_ModifiedByUserID_CMS_User");

            entity.HasOne(d => d.NodeSite).WithMany(p => p.CmsVersionHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_VersionHistory_NodeSiteID_CMS_Site");

            entity.HasOne(d => d.VersionClass).WithMany(p => p.CmsVersionHistories).HasConstraintName("FK_CMS_VersionHistory_VersionClassID_CMS_Class");

            entity.HasOne(d => d.VersionDeletedByUser).WithMany(p => p.CmsVersionHistoryVersionDeletedByUsers).HasConstraintName("FK_CMS_VersionHistory_DeletedByUserID_CMS_User");

            entity.HasOne(d => d.VersionWorkflow).WithMany(p => p.CmsVersionHistories).HasConstraintName("FK_CMS_VersionHistory_VersionWorkflowID_CMS_Workflow");

            entity.HasOne(d => d.VersionWorkflowStep).WithMany(p => p.CmsVersionHistories).HasConstraintName("FK_CMS_VersionHistory_VersionWorkflowStepID_CMS_WorkflowStep");

            entity.HasMany(d => d.AttachmentHistories).WithMany(p => p.VersionHistories)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsVersionAttachment",
                    r => r.HasOne<CmsAttachmentHistory>().WithMany()
                        .HasForeignKey("AttachmentHistoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_VersionAttachment_AttachmentHistoryID_CMS_AttachmentHistory"),
                    l => l.HasOne<CmsVersionHistory>().WithMany()
                        .HasForeignKey("VersionHistoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_VersionAttachment_VersionHistoryID_CMS_VersionHistory"),
                    j =>
                    {
                        j.HasKey("VersionHistoryId", "AttachmentHistoryId");
                        j.ToTable("CMS_VersionAttachment");
                        j.HasIndex(new[] { "AttachmentHistoryId" }, "IX_CMS_VersionAttachment_AttachmentHistoryID");
                        j.IndexerProperty<int>("VersionHistoryId").HasColumnName("VersionHistoryID");
                        j.IndexerProperty<int>("AttachmentHistoryId").HasColumnName("AttachmentHistoryID");
                    });
        });

        modelBuilder.Entity<CmsWebFarmServer>(entity =>
        {
            entity.HasKey(e => e.ServerId).IsClustered(false);

            entity.HasIndex(e => e.ServerDisplayName, "IX_CMS_WebFarmServer_ServerDisplayName").IsClustered();

            entity.Property(e => e.ServerDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ServerLastModified).HasDefaultValueSql("('9/17/2013 12:18:06 PM')");
            entity.Property(e => e.ServerName).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsWebFarmServerLog>(entity =>
        {
            entity.Property(e => e.LogCode).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsWebFarmServerTask>(entity =>
        {
            entity.HasOne(d => d.Server).WithMany(p => p.CmsWebFarmServerTasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebFarmServerTask_ServerID_CMS_WebFarmServer");

            entity.HasOne(d => d.Task).WithMany(p => p.CmsWebFarmServerTasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebFarmServerTask_TaskID_CMS_WebFarmTask");
        });

        modelBuilder.Entity<CmsWebFarmTask>(entity =>
        {
            entity.Property(e => e.TaskGuid).HasDefaultValueSql("('00000000-0000-0000-0000-000000000000')");
            entity.Property(e => e.TaskIsMemory).HasDefaultValueSql("((0))");
            entity.Property(e => e.TaskType).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<CmsWebPart>(entity =>
        {
            entity.Property(e => e.WebPartDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.WebPartFileName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.WebPartLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.WebPartName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.WebPartProperties).HasDefaultValueSql("(N'')");
            entity.Property(e => e.WebPartSkipInsertProperties).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.WebPartCategory).WithMany(p => p.CmsWebParts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPart_WebPartCategoryID_CMS_WebPartCategory");

            entity.HasOne(d => d.WebPartParent).WithMany(p => p.InverseWebPartParent).HasConstraintName("FK_CMS_WebPart_WebPartParentID_CMS_WebPart");

            entity.HasOne(d => d.WebPartResource).WithMany(p => p.CmsWebParts).HasConstraintName("FK_CMS_WebPart_WebPartResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsWebPartCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => e.CategoryPath, "IX_CMS_WebPartCategory_CategoryPath")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.CategoryPath).HasDefaultValueSql("('')");

            entity.HasOne(d => d.CategoryParent).WithMany(p => p.InverseCategoryParent).HasConstraintName("FK_CMS_WebPartCategory_CategoryParentID_CMS_WebPartCategory");
        });

        modelBuilder.Entity<CmsWebPartContainer>(entity =>
        {
            entity.HasKey(e => e.ContainerId).IsClustered(false);

            entity.HasIndex(e => e.ContainerDisplayName, "IX_CMS_WebPartContainer_ContainerDisplayName").IsClustered();

            entity.Property(e => e.ContainerDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.ContainerName).HasDefaultValueSql("('')");

            entity.HasMany(d => d.Sites).WithMany(p => p.Containers)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsWebPartContainerSite",
                    r => r.HasOne<CmsSite>().WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_WebPartContainerSite_SiteID_CMS_Site"),
                    l => l.HasOne<CmsWebPartContainer>().WithMany()
                        .HasForeignKey("ContainerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_WebPartContainerSite_ContainerID_CMS_WebPartContainer"),
                    j =>
                    {
                        j.HasKey("ContainerId", "SiteId");
                        j.ToTable("CMS_WebPartContainerSite");
                        j.HasIndex(new[] { "SiteId" }, "IX_CMS_WebPartContainerSite_SiteID");
                        j.IndexerProperty<int>("ContainerId").HasColumnName("ContainerID");
                        j.IndexerProperty<int>("SiteId").HasColumnName("SiteID");
                    });
        });

        modelBuilder.Entity<CmsWebPartLayout>(entity =>
        {
            entity.HasKey(e => e.WebPartLayoutId).IsClustered(false);

            entity.HasIndex(e => new { e.WebPartLayoutWebPartId, e.WebPartLayoutCodeName }, "IX_CMS_WebPartLayout_WebPartLayoutWebPartID_WebPartLayoutCodeName").IsClustered();

            entity.Property(e => e.WebPartLayoutCodeName).HasDefaultValueSql("('')");
            entity.Property(e => e.WebPartLayoutDisplayName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.WebPartLayoutWebPart).WithMany(p => p.CmsWebPartLayouts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPartLayout_WebPartLayoutWebPartID_CMS_WebPart");
        });

        modelBuilder.Entity<CmsWidget>(entity =>
        {
            entity.HasKey(e => e.WidgetId).IsClustered(false);

            entity.HasIndex(e => new { e.WidgetCategoryId, e.WidgetDisplayName }, "IX_CMS_Widget_WidgetCategoryID_WidgetDisplayName").IsClustered();

            entity.Property(e => e.WidgetSecurity).HasDefaultValueSql("((2))");

            entity.HasOne(d => d.WidgetCategory).WithMany(p => p.CmsWidgets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Widget_WidgetCategoryID_CMS_WidgetCategory");

            entity.HasOne(d => d.WidgetLayout).WithMany(p => p.CmsWidgets).HasConstraintName("FK_CMS_Widget_WidgetLayoutID_CMS_WebPartLayout");

            entity.HasOne(d => d.WidgetWebPart).WithMany(p => p.CmsWidgets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Widget_WidgetWebPartID_CMS_WebPart");
        });

        modelBuilder.Entity<CmsWidgetCategory>(entity =>
        {
            entity.HasKey(e => e.WidgetCategoryId).IsClustered(false);

            entity.HasIndex(e => e.WidgetCategoryPath, "IX_CMS_WidgetCategory_CategoryPath")
                .IsUnique()
                .IsClustered();

            entity.HasOne(d => d.WidgetCategoryParent).WithMany(p => p.InverseWidgetCategoryParent).HasConstraintName("FK_CMS_WidgetCategory_WidgetCategoryParentID_CMS_WidgetCategory");
        });

        modelBuilder.Entity<CmsWidgetRole>(entity =>
        {
            entity.HasOne(d => d.Permission).WithMany(p => p.CmsWidgetRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WidgetRole_PermissionID_CMS_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.CmsWidgetRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WidgetRole_RoleID_CMS_Role");

            entity.HasOne(d => d.Widget).WithMany(p => p.CmsWidgetRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WidgetRole_WidgetID_CMS_Widget");
        });

        modelBuilder.Entity<CmsWorkflow>(entity =>
        {
            entity.HasKey(e => e.WorkflowId).IsClustered(false);

            entity.HasIndex(e => e.WorkflowDisplayName, "IX_CMS_Workflow_WorkflowDisplayName").IsClustered();

            entity.Property(e => e.WorkflowAutoPublishChanges).HasDefaultValueSql("((0))");
            entity.Property(e => e.WorkflowDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.WorkflowEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.WorkflowName).HasDefaultValueSql("('')");
            entity.Property(e => e.WorkflowSendApproveEmails).HasDefaultValueSql("((1))");
            entity.Property(e => e.WorkflowSendArchiveEmails).HasDefaultValueSql("((1))");
            entity.Property(e => e.WorkflowSendPublishEmails).HasDefaultValueSql("((1))");
            entity.Property(e => e.WorkflowSendReadyForApprovalEmails).HasDefaultValueSql("((1))");
            entity.Property(e => e.WorkflowSendRejectEmails).HasDefaultValueSql("((1))");
            entity.Property(e => e.WorkflowUseCheckinCheckout).HasDefaultValueSql("((0))");

            entity.HasMany(d => d.Users).WithMany(p => p.Workflows)
                .UsingEntity<Dictionary<string, object>>(
                    "CmsWorkflowUser",
                    r => r.HasOne<CmsUser>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_WorkflowUser_UserID_CMS_User"),
                    l => l.HasOne<CmsWorkflow>().WithMany()
                        .HasForeignKey("WorkflowId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CMS_WorkflowUser_WorkflowID_CMS_Workflow"),
                    j =>
                    {
                        j.HasKey("WorkflowId", "UserId").HasName("PK_CMS_WorkflowUser_1");
                        j.ToTable("CMS_WorkflowUser");
                        j.HasIndex(new[] { "UserId" }, "IX_CMS_WorkflowUser_UserID");
                        j.IndexerProperty<int>("WorkflowId").HasColumnName("WorkflowID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                    });
        });

        modelBuilder.Entity<CmsWorkflowAction>(entity =>
        {
            entity.Property(e => e.ActionEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.ActionWorkflowType).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ActionResource).WithMany(p => p.CmsWorkflowActions).HasConstraintName("FK_CMS_WorkflowAction_ActionResourceID");
        });

        modelBuilder.Entity<CmsWorkflowHistory>(entity =>
        {
            entity.Property(e => e.HistoryRejected).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.CmsWorkflowHistories).HasConstraintName("FK_CMS_WorkflowHistory_ApprovedByUserID_CMS_User");

            entity.HasOne(d => d.HistoryWorkflow).WithMany(p => p.CmsWorkflowHistories).HasConstraintName("FK_CMS_WorkflowHistory_HistoryWorkflowID_CMS_Workflow");

            entity.HasOne(d => d.Step).WithMany(p => p.CmsWorkflowHistorySteps).HasConstraintName("FK_CMS_WorkflowHistory_StepID_CMS_WorkflowStep");

            entity.HasOne(d => d.TargetStep).WithMany(p => p.CmsWorkflowHistoryTargetSteps).HasConstraintName("FK_CMS_WorkflowHistory_TargetStepID_CMS_WorkflowStep");

            entity.HasOne(d => d.VersionHistory).WithMany(p => p.CmsWorkflowHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowHistory_VersionHistoryID_CMS_VersionHistory");
        });

        modelBuilder.Entity<CmsWorkflowScope>(entity =>
        {
            entity.HasKey(e => e.ScopeId).IsClustered(false);

            entity.HasIndex(e => e.ScopeStartingPath, "IX_CMS_WorkflowScope_ScopeStartingPath").IsClustered();

            entity.HasOne(d => d.ScopeClass).WithMany(p => p.CmsWorkflowScopes).HasConstraintName("FK_CMS_WorkflowScope_ScopeClassID_CMS_Class");

            entity.HasOne(d => d.ScopeCulture).WithMany(p => p.CmsWorkflowScopes).HasConstraintName("FK_CMS_WorkflowScope_ScopeCultureID_CMS_Culture");

            entity.HasOne(d => d.ScopeSite).WithMany(p => p.CmsWorkflowScopes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowScope_ScopeSiteID_CMS_Site");

            entity.HasOne(d => d.ScopeWorkflow).WithMany(p => p.CmsWorkflowScopes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowScope_ScopeWorkflowID_CMS_WorkflowID");
        });

        modelBuilder.Entity<CmsWorkflowStep>(entity =>
        {
            entity.Property(e => e.StepAllowPublish).HasDefaultValueSql("((0))");
            entity.Property(e => e.StepAllowReject).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.StepAction).WithMany(p => p.CmsWorkflowSteps).HasConstraintName("FK_CMS_WorkflowStep_StepActionID");

            entity.HasOne(d => d.StepWorkflow).WithMany(p => p.CmsWorkflowSteps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowStep_StepWorkflowID");
        });

        modelBuilder.Entity<CmsWorkflowStepRole>(entity =>
        {
            entity.HasKey(e => e.WorkflowStepRoleId).IsClustered(false);

            entity.HasIndex(e => new { e.StepId, e.StepSourcePointGuid, e.RoleId }, "IX_CMS_WorkflowStepRoles_StepID_StepSourcePointGUID_RoleID")
                .IsUnique()
                .IsClustered();

            entity.HasOne(d => d.Role).WithMany(p => p.CmsWorkflowStepRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowStepRoles_RoleID_CMS_Role");

            entity.HasOne(d => d.Step).WithMany(p => p.CmsWorkflowStepRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowStepRoles_StepID_CMS_WorkflowStep");
        });

        modelBuilder.Entity<CmsWorkflowStepUser>(entity =>
        {
            entity.HasKey(e => e.WorkflowStepUserId).IsClustered(false);

            entity.HasIndex(e => new { e.StepId, e.StepSourcePointGuid, e.UserId }, "IX_CMS_WorkflowStepUser_StepID_StepSourcePointGUID_UserID")
                .IsUnique()
                .IsClustered();

            entity.HasOne(d => d.Step).WithMany(p => p.CmsWorkflowStepUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowStepUser_StepID_CMS_WorkflowStep");

            entity.HasOne(d => d.User).WithMany(p => p.CmsWorkflowStepUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowStepUser_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsWorkflowTransition>(entity =>
        {
            entity.HasOne(d => d.TransitionEndStep).WithMany(p => p.CmsWorkflowTransitionTransitionEndSteps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowTransition_TransitionEndStepID_CMS_WorkflowStep");

            entity.HasOne(d => d.TransitionStartStep).WithMany(p => p.CmsWorkflowTransitionTransitionStartSteps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowTransition_TransitionStartStepID_CMS_WorkflowStep");

            entity.HasOne(d => d.TransitionWorkflow).WithMany(p => p.CmsWorkflowTransitions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowTransition_TransitionWorkflowID_CMS_Workflow");
        });

        modelBuilder.Entity<ComAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK_COM_CustomerAdress");

            entity.Property(e => e.AddressCity).HasDefaultValueSql("('')");
            entity.Property(e => e.AddressLastModified).HasDefaultValueSql("('10/18/2012 3:39:07 PM')");
            entity.Property(e => e.AddressLine1).HasDefaultValueSql("(N'')");
            entity.Property(e => e.AddressName).HasDefaultValueSql("('')");
            entity.Property(e => e.AddressPersonalName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.AddressZip).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.AddressCountry).WithMany(p => p.ComAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Address_AddressCountryID_CMS_Country");

            entity.HasOne(d => d.AddressCustomer).WithMany(p => p.ComAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Address_AddressCustomerID_COM_Customer");

            entity.HasOne(d => d.AddressState).WithMany(p => p.ComAddresses).HasConstraintName("FK_COM_Address_AddressStateID_CMS_State");
        });

        modelBuilder.Entity<ComBrand>(entity =>
        {
            entity.Property(e => e.BrandDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.BrandEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.BrandLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.BrandName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.BrandSite).WithMany(p => p.ComBrands)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Brand_BrandSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComCarrier>(entity =>
        {
            entity.Property(e => e.CarrierAssemblyName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CarrierClassName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CarrierDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CarrierLastModified).HasDefaultValueSql("('9/22/2014 3:00:14 PM')");
            entity.Property(e => e.CarrierName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CarrierSite).WithMany(p => p.ComCarriers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Carrier_CarrierSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComCollection>(entity =>
        {
            entity.Property(e => e.CollectionDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CollectionEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.CollectionLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.CollectionName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CollectionSite).WithMany(p => p.ComCollections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Collection_CollectionSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComCouponCode>(entity =>
        {
            entity.Property(e => e.CouponCodeCode).HasDefaultValueSql("('')");

            entity.HasOne(d => d.CouponCodeDiscount).WithMany(p => p.ComCouponCodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_CouponCode_CouponCodeDiscountID_COM_Discount");
        });

        modelBuilder.Entity<ComCurrency>(entity =>
        {
            entity.Property(e => e.CurrencyCode).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CurrencyDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CurrencyFormatString).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CurrencyName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CurrencySite).WithMany(p => p.ComCurrencies).HasConstraintName("FK_COM_Currency_CurrencySiteID_CMS_Site");
        });

        modelBuilder.Entity<ComCurrencyExchangeRate>(entity =>
        {
            entity.HasOne(d => d.ExchangeRateToCurrency).WithMany(p => p.ComCurrencyExchangeRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_CurrencyExchangeRate_ExchangeRateToCurrencyID_COM_Currency");

            entity.HasOne(d => d.ExchangeTable).WithMany(p => p.ComCurrencyExchangeRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_CurrencyExchangeRate_ExchangeTableID_COM_ExchangeTable");
        });

        modelBuilder.Entity<ComCustomer>(entity =>
        {
            entity.HasIndex(e => e.CustomerCompany, "IX_COM_Customer_CustomerCompany")
                .HasFilter("([CustomerCompany] IS NOT NULL)")
                .HasFillFactor(90);

            entity.HasOne(d => d.CustomerSite).WithMany(p => p.ComCustomers).HasConstraintName("FK_COM_Customer_CustomerSiteID_CMS_Site");

            entity.HasOne(d => d.CustomerUser).WithMany(p => p.ComCustomers).HasConstraintName("FK_COM_Customer_CustomerUserID_CMS_User");
        });

        modelBuilder.Entity<ComCustomerCreditHistory>(entity =>
        {
            entity.Property(e => e.EventCreditLastModified).HasDefaultValueSql("('9/26/2012 12:21:38 PM')");
            entity.Property(e => e.EventDate).HasDefaultValueSql("('9/27/2012 2:48:56 PM')");
            entity.Property(e => e.EventName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.EventCustomer).WithMany(p => p.ComCustomerCreditHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_CustomerCreditHistory_EventCustomerID_COM_Customer");

            entity.HasOne(d => d.EventSite).WithMany(p => p.ComCustomerCreditHistories).HasConstraintName("FK_COM_CustomerCreditHistory_EventSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComDepartment>(entity =>
        {
            entity.Property(e => e.DepartmentDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.DepartmentName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.DepartmentDefaultTaxClass).WithMany(p => p.ComDepartments).HasConstraintName("FK_COM_Department_DepartmentDefaultTaxClassID_COM_TaxClass");

            entity.HasOne(d => d.DepartmentSite).WithMany(p => p.ComDepartments).HasConstraintName("FK_COM_Department_DepartmentSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComDiscount>(entity =>
        {
            entity.Property(e => e.DiscountApplyFurtherDiscounts).HasDefaultValueSql("((1))");
            entity.Property(e => e.DiscountApplyTo).HasDefaultValueSql("('Order')");
            entity.Property(e => e.DiscountDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.DiscountEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.DiscountOrder).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.DiscountSite).WithMany(p => p.ComDiscounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Discount_DiscountSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComExchangeTable>(entity =>
        {
            entity.HasKey(e => e.ExchangeTableId).IsClustered(false);

            entity.HasIndex(e => new { e.ExchangeTableValidFrom, e.ExchangeTableValidTo }, "IX_COM_ExchangeTable_ExchangeTableValidFrom_ExchangeTableValidTo")
                .IsDescending()
                .IsClustered();

            entity.Property(e => e.ExchangeTableDisplayName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ExchangeTableSite).WithMany(p => p.ComExchangeTables).HasConstraintName("FK_COM_ExchangeTable_ExchangeTableSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComGiftCard>(entity =>
        {
            entity.Property(e => e.GiftCardCustomerRestriction).HasDefaultValueSql("(N'enum1')");
            entity.Property(e => e.GiftCardDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.GiftCardEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.GiftCardLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.GiftCardName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.GiftCardSite).WithMany(p => p.ComGiftCards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_GiftCard_GiftCardSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComGiftCardCouponCode>(entity =>
        {
            entity.Property(e => e.GiftCardCouponCodeCode).HasDefaultValueSql("(N'')");
            entity.Property(e => e.GiftCardCouponCodeLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.GiftCardCouponCodeGiftCard).WithMany(p => p.ComGiftCardCouponCodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_GiftCardCouponCode_GiftCardCouponCodeGiftCardID_COM_GiftCard");
        });

        modelBuilder.Entity<ComInternalStatus>(entity =>
        {
            entity.Property(e => e.InternalStatusDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.InternalStatusEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.InternalStatusLastModified).HasDefaultValueSql("('9/20/2012 2:45:44 PM')");
            entity.Property(e => e.InternalStatusName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.InternalStatusSite).WithMany(p => p.ComInternalStatuses).HasConstraintName("FK_COM_InternalStatus_InternalStatusSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComManufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).IsClustered(false);

            entity.HasIndex(e => new { e.ManufacturerDisplayName, e.ManufacturerEnabled }, "IX_COM_Manufacturer_ManufacturerDisplayName_ManufacturerEnabled").IsClustered();

            entity.Property(e => e.ManufacturerDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ManufacturerEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.ManufacturerLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.ManufacturerSite).WithMany(p => p.ComManufacturers).HasConstraintName("FK_COM_Manufacturer_ManufacturerSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComMultiBuyCouponCode>(entity =>
        {
            entity.Property(e => e.MultiBuyCouponCodeCode).HasDefaultValueSql("(N'')");
            entity.Property(e => e.MultiBuyCouponCodeUseCount).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.MultiBuyCouponCodeMultiBuyDiscount).WithMany(p => p.ComMultiBuyCouponCodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyCouponCode_MultiBuyCouponCodeMultiBuyDiscountID_COM_MultiBuyDiscount");
        });

        modelBuilder.Entity<ComMultiBuyDiscount>(entity =>
        {
            entity.Property(e => e.MultiBuyDiscountApplyFurtherDiscounts).HasDefaultValueSql("((1))");
            entity.Property(e => e.MultiBuyDiscountAutoAddEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.MultiBuyDiscountCustomerRestriction).HasDefaultValueSql("(N'All')");
            entity.Property(e => e.MultiBuyDiscountEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.MultiBuyDiscountIsFlat).HasDefaultValueSql("((1))");
            entity.Property(e => e.MultiBuyDiscountMinimumBuyCount).HasDefaultValueSql("((1))");
            entity.Property(e => e.MultiBuyDiscountUsesCoupons).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.MultiBuyDiscountApplyToSku).WithMany(p => p.ComMultiBuyDiscounts).HasConstraintName("FK_COM_MultiBuyDiscount_MultiBuyDiscountApplyToSKUID_COM_SKU");

            entity.HasOne(d => d.MultiBuyDiscountSite).WithMany(p => p.ComMultiBuyDiscounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscount_MultiBuyDiscountSiteID_CMS_Site");

            entity.HasMany(d => d.Departments).WithMany(p => p.MultiBuyDiscounts)
                .UsingEntity<Dictionary<string, object>>(
                    "ComMultiBuyDiscountDepartment",
                    r => r.HasOne<ComDepartment>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_MultiBuyDiscountDepartment_DepartmentID_COM_Department"),
                    l => l.HasOne<ComMultiBuyDiscount>().WithMany()
                        .HasForeignKey("MultiBuyDiscountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_MultiBuyDiscountDepartment_MultiBuyDiscountID_COM_MultiBuyDiscount"),
                    j =>
                    {
                        j.HasKey("MultiBuyDiscountId", "DepartmentId");
                        j.ToTable("COM_MultiBuyDiscountDepartment");
                        j.HasIndex(new[] { "DepartmentId" }, "IX_COM_MultiBuyDiscountDepartment_DepartmentID");
                        j.IndexerProperty<int>("MultiBuyDiscountId").HasColumnName("MultiBuyDiscountID");
                        j.IndexerProperty<int>("DepartmentId").HasColumnName("DepartmentID");
                    });

            entity.HasMany(d => d.Skus).WithMany(p => p.MultiBuyDiscounts)
                .UsingEntity<Dictionary<string, object>>(
                    "ComMultiBuyDiscountSku",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("Skuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_MultiBuyDiscountSKU_SKUID_COM_SKU"),
                    l => l.HasOne<ComMultiBuyDiscount>().WithMany()
                        .HasForeignKey("MultiBuyDiscountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_MultiBuyDiscountSKU_MultiBuyDiscountID_COM_MultiBuyDiscount"),
                    j =>
                    {
                        j.HasKey("MultiBuyDiscountId", "Skuid");
                        j.ToTable("COM_MultiBuyDiscountSKU");
                        j.HasIndex(new[] { "Skuid" }, "IX_COM_MultiBuyDiscountSKU_SKUID");
                        j.IndexerProperty<int>("MultiBuyDiscountId").HasColumnName("MultiBuyDiscountID");
                        j.IndexerProperty<int>("Skuid").HasColumnName("SKUID");
                    });
        });

        modelBuilder.Entity<ComMultiBuyDiscountBrand>(entity =>
        {
            entity.Property(e => e.BrandIncluded).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Brand).WithMany(p => p.ComMultiBuyDiscountBrands)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscountBrand_BrandID_COM_Brand");

            entity.HasOne(d => d.MultiBuyDiscount).WithMany(p => p.ComMultiBuyDiscountBrands)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscountBrand_MultiBuyDiscountID_COM_MultiBuyDiscount");
        });

        modelBuilder.Entity<ComMultiBuyDiscountCollection>(entity =>
        {
            entity.Property(e => e.CollectionIncluded).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Collection).WithMany(p => p.ComMultiBuyDiscountCollections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscountCollection_CollectionID_COM_Collection");

            entity.HasOne(d => d.MultibuyDiscount).WithMany(p => p.ComMultiBuyDiscountCollections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscountCollection_MultiBuyDiscountID_COM_MultiBuyDiscount");
        });

        modelBuilder.Entity<ComMultiBuyDiscountTree>(entity =>
        {
            entity.Property(e => e.NodeIncluded).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.MultiBuyDiscount).WithMany(p => p.ComMultiBuyDiscountTrees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscountTree_MultiBuyDiscountID_COM_MultiBuyDiscount");

            entity.HasOne(d => d.Node).WithMany(p => p.ComMultiBuyDiscountTrees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_MultiBuyDiscountTree_NodeID_CMS_Tree");
        });

        modelBuilder.Entity<ComOptionCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => new { e.CategoryDisplayName, e.CategoryEnabled }, "IX_COM_OptionCategory_CategoryDisplayName_CategoryEnabled").IsClustered();

            entity.Property(e => e.CategoryDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CategoryDisplayPrice).HasDefaultValueSql("((1))");
            entity.Property(e => e.CategoryEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.CategoryName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.CategorySelectionType).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.CategorySite).WithMany(p => p.ComOptionCategories).HasConstraintName("FK_COM_OptionCategory_CategorySiteID_CMS_Site");
        });

        modelBuilder.Entity<ComOrder>(entity =>
        {
            entity.HasOne(d => d.OrderCreatedByUser).WithMany(p => p.ComOrders).HasConstraintName("FK_COM_Order_OrderCreatedByUserID_CMS_User");

            entity.HasOne(d => d.OrderCurrency).WithMany(p => p.ComOrders).HasConstraintName("FK_COM_Order_OrderCurrencyID_COM_Currency");

            entity.HasOne(d => d.OrderCustomer).WithMany(p => p.ComOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Order_OrderCustomerID_COM_Customer");

            entity.HasOne(d => d.OrderPaymentOption).WithMany(p => p.ComOrders).HasConstraintName("FK_COM_Order_OrderPaymentOptionID_COM_PaymentOption");

            entity.HasOne(d => d.OrderShippingOption).WithMany(p => p.ComOrders).HasConstraintName("FK_COM_Order_OrderShippingOptionID_COM_ShippingOption");

            entity.HasOne(d => d.OrderSite).WithMany(p => p.ComOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Order_OrderSiteID_CMS_Site");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.ComOrders).HasConstraintName("FK_COM_Order_OrderStatusID_COM_Status");
        });

        modelBuilder.Entity<ComOrderAddress>(entity =>
        {
            entity.HasOne(d => d.AddressCountry).WithMany(p => p.ComOrderAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderAddress_AddressCountryID_CMS_Country");

            entity.HasOne(d => d.AddressOrder).WithMany(p => p.ComOrderAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderAddress_AddressOrderID_COM_Order");

            entity.HasOne(d => d.AddressState).WithMany(p => p.ComOrderAddresses).HasConstraintName("FK_COM_OrderAddress_AddressStateID_CMS_State");
        });

        modelBuilder.Entity<ComOrderItem>(entity =>
        {
            entity.HasOne(d => d.OrderItemOrder).WithMany(p => p.ComOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderItem_OrderItemOrderID_COM_Order");

            entity.HasOne(d => d.OrderItemSku).WithMany(p => p.ComOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderItem_OrderItemSKUID_COM_SKU");
        });

        modelBuilder.Entity<ComOrderItemSkufile>(entity =>
        {
            entity.HasOne(d => d.File).WithMany(p => p.ComOrderItemSkufiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderItemSKUFile_COM_SKUFile");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.ComOrderItemSkufiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderItemSKUFile_COM_OrderItem");
        });

        modelBuilder.Entity<ComOrderStatus>(entity =>
        {
            entity.Property(e => e.StatusDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.StatusEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.StatusName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.StatusSendNotification).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.StatusSite).WithMany(p => p.ComOrderStatuses).HasConstraintName("FK_COM_OrderStatus_StatusSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComOrderStatusUser>(entity =>
        {
            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.ComOrderStatusUsers).HasConstraintName("FK_COM_OrderStatusUser_ChangedByUserID_CMS_User");

            entity.HasOne(d => d.FromStatus).WithMany(p => p.ComOrderStatusUserFromStatuses).HasConstraintName("FK_COM_OrderStatusUser_FromStatusID_COM_Status");

            entity.HasOne(d => d.Order).WithMany(p => p.ComOrderStatusUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderStatusUser_OrderID_COM_Order");

            entity.HasOne(d => d.ToStatus).WithMany(p => p.ComOrderStatusUserToStatuses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_OrderStatusUser_ToStatusID_COM_Status");
        });

        modelBuilder.Entity<ComPaymentOption>(entity =>
        {
            entity.HasKey(e => e.PaymentOptionId).IsClustered(false);

            entity.HasIndex(e => new { e.PaymentOptionSiteId, e.PaymentOptionDisplayName, e.PaymentOptionEnabled }, "IX_COM_PaymentOption_PaymentOptionSiteID_PaymentOptionDisplayName_PaymentOptionEnabled").IsClustered();

            entity.Property(e => e.PaymentOptionAllowIfNoShipping).HasDefaultValueSql("((0))");
            entity.Property(e => e.PaymentOptionDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PaymentOptionEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.PaymentOptionLastModified).HasDefaultValueSql("('9/27/2012 4:18:26 PM')");
            entity.Property(e => e.PaymentOptionName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.PaymentOptionAuthorizedOrderStatus).WithMany(p => p.ComPaymentOptionPaymentOptionAuthorizedOrderStatuses).HasConstraintName("FK_COM_PaymentOption_PaymentOptionAuthorizedOrderStatusID_COM_OrderStatus");

            entity.HasOne(d => d.PaymentOptionFailedOrderStatus).WithMany(p => p.ComPaymentOptionPaymentOptionFailedOrderStatuses).HasConstraintName("FK_COM_PaymentOption_PaymentOptionFailedOrderStatusID_COM_OrderStatus");

            entity.HasOne(d => d.PaymentOptionSite).WithMany(p => p.ComPaymentOptions).HasConstraintName("FK_COM_PaymentOption_PaymentOptionSiteID_CMS_Site");

            entity.HasOne(d => d.PaymentOptionSucceededOrderStatus).WithMany(p => p.ComPaymentOptionPaymentOptionSucceededOrderStatuses).HasConstraintName("FK_COM_PaymentOption_PaymentOptionSucceededOrderStatusID_COM_OrderStatus");
        });

        modelBuilder.Entity<ComPublicStatus>(entity =>
        {
            entity.HasKey(e => e.PublicStatusId).IsClustered(false);

            entity.HasIndex(e => new { e.PublicStatusDisplayName, e.PublicStatusEnabled }, "IX_COM_PublicStatus_PublicStatusDisplayName_PublicStatusEnabled").IsClustered();

            entity.Property(e => e.PublicStatusDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PublicStatusEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.PublicStatusName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.PublicStatusSite).WithMany(p => p.ComPublicStatuses).HasConstraintName("FK_COM_PublicStatus_PublicStatusSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComShippingCost>(entity =>
        {
            entity.HasKey(e => e.ShippingCostId).HasName("PK__COM_ShippingCost");

            entity.HasOne(d => d.ShippingCostShippingOption).WithMany(p => p.ComShippingCosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_ShippingCost_ShippingCostShippingOptionID_COM_ShippingOption");
        });

        modelBuilder.Entity<ComShippingOption>(entity =>
        {
            entity.HasKey(e => e.ShippingOptionId).IsClustered(false);

            entity.HasIndex(e => e.ShippingOptionDisplayName, "IX_COM_ShippingOptionDisplayName").IsClustered();

            entity.Property(e => e.ShippingOptionDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.ShippingOptionEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.ShippingOptionLastModified).HasDefaultValueSql("('9/26/2012 12:44:18 PM')");
            entity.Property(e => e.ShippingOptionName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ShippingOptionCarrier).WithMany(p => p.ComShippingOptions).HasConstraintName("FK_COM_ShippingOption_ShippingOptionCarrierID_COM_Carrier");

            entity.HasOne(d => d.ShippingOptionSite).WithMany(p => p.ComShippingOptions).HasConstraintName("FK_COM_ShippingOption_ShippingOptionSiteID_CMS_Site");

            entity.HasOne(d => d.ShippingOptionTaxClass).WithMany(p => p.ComShippingOptions).HasConstraintName("FK_COM_ShippingOption_ShippingOptionTaxClassID_COM_TaxClass");
        });

        modelBuilder.Entity<ComShoppingCart>(entity =>
        {
            entity.HasOne(d => d.ShoppingCartBillingAddress).WithMany(p => p.ComShoppingCartShoppingCartBillingAddresses).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartBillingAddressID_COM_Address");

            entity.HasOne(d => d.ShoppingCartCompanyAddress).WithMany(p => p.ComShoppingCartShoppingCartCompanyAddresses).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartCompanyAddressID_COM_Address");

            entity.HasOne(d => d.ShoppingCartCurrency).WithMany(p => p.ComShoppingCarts).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartCurrencyID_COM_Currency");

            entity.HasOne(d => d.ShoppingCartCustomer).WithMany(p => p.ComShoppingCarts).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartCustomerID_COM_Customer");

            entity.HasOne(d => d.ShoppingCartPaymentOption).WithMany(p => p.ComShoppingCarts).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartPaymentOptionID_COM_PaymentOption");

            entity.HasOne(d => d.ShoppingCartShippingAddress).WithMany(p => p.ComShoppingCartShoppingCartShippingAddresses).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartShippingAddressID_COM_Address");

            entity.HasOne(d => d.ShoppingCartShippingOption).WithMany(p => p.ComShoppingCarts).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartShippingOptionID_COM_ShippingOption");

            entity.HasOne(d => d.ShoppingCartSite).WithMany(p => p.ComShoppingCarts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_ShoppingCart_ShoppingCartSiteID_CMS_Site");

            entity.HasOne(d => d.ShoppingCartUser).WithMany(p => p.ComShoppingCarts).HasConstraintName("FK_COM_ShoppingCart_ShoppingCartUserID_CMS_User");
        });

        modelBuilder.Entity<ComShoppingCartCouponCode>(entity =>
        {
            entity.Property(e => e.CouponCode).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.ComShoppingCartCouponCodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_ShoppingCartCouponCode_ShoppingCartID_COM_ShoppingCart");
        });

        modelBuilder.Entity<ComShoppingCartSku>(entity =>
        {
            entity.Property(e => e.CartItemAutoAddedUnits).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.ComShoppingCartSkus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_ShoppingCartSKU_ShoppingCartID_COM_ShoppingCart");

            entity.HasOne(d => d.Sku).WithMany(p => p.ComShoppingCartSkus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_ShoppingCartSKU_SKUID_COM_SKU");
        });

        modelBuilder.Entity<ComSku>(entity =>
        {
            entity.HasIndex(e => e.Skunumber, "IX_COM_SKU_SKUNumber")
                .HasFilter("([SKUNumber] IS NOT NULL)")
                .HasFillFactor(90);

            entity.Property(e => e.SkubundleInventoryType).HasDefaultValueSql("('REMOVEBUNDLE')");
            entity.Property(e => e.Skuenabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.Skuname).HasDefaultValueSql("('')");
            entity.Property(e => e.SkusellOnlyAvailable).HasDefaultValueSql("((0))");
            entity.Property(e => e.SkutrackInventory).HasDefaultValueSql("(N'ByProduct')");

            entity.HasOne(d => d.Skubrand).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUBrandID_COM_Brand");

            entity.HasOne(d => d.Skucollection).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUCollectionID_COM_Collection");

            entity.HasOne(d => d.Skudepartment).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUDepartmentID_COM_Department");

            entity.HasOne(d => d.SkuinternalStatus).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUInternalStatusID_COM_InternalStatus");

            entity.HasOne(d => d.Skumanufacturer).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUManufacturerID_COM_Manifacturer");

            entity.HasOne(d => d.SkuoptionCategory).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUOptionCategoryID_COM_OptionCategory");

            entity.HasOne(d => d.SkuparentSku).WithMany(p => p.InverseSkuparentSku).HasConstraintName("FK_COM_SKU_SKUParentSKUID_COM_SKU");

            entity.HasOne(d => d.SkupublicStatus).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUPublicStatusID_COM_PublicStatus");

            entity.HasOne(d => d.Skusite).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUSiteID_CMS_Site");

            entity.HasOne(d => d.Skusupplier).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUSupplierID_COM_Supplier");

            entity.HasOne(d => d.SkutaxClass).WithMany(p => p.ComSkus).HasConstraintName("FK_COM_SKU_SKUTaxClass_COM_TaxClass");

            entity.HasMany(d => d.Bundles).WithMany(p => p.Skus)
                .UsingEntity<Dictionary<string, object>>(
                    "ComBundle",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("BundleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_Bundle_BundleID_COM_SKU"),
                    l => l.HasOne<ComSku>().WithMany()
                        .HasForeignKey("Skuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_Bundle_SKUID_COM_SKU"),
                    j =>
                    {
                        j.HasKey("BundleId", "Skuid");
                        j.ToTable("COM_Bundle");
                        j.HasIndex(new[] { "Skuid" }, "IX_COM_Bundle_SKUID");
                        j.IndexerProperty<int>("BundleId").HasColumnName("BundleID");
                        j.IndexerProperty<int>("Skuid").HasColumnName("SKUID");
                    });

            entity.HasMany(d => d.OptionSkus).WithMany(p => p.SkusNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ComSkuallowedOption",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("OptionSkuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_SKUOption_OptionSKUID_COM_SKU"),
                    l => l.HasOne<ComSku>().WithMany()
                        .HasForeignKey("Skuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_SKUOption_SKUID_COM_SKU"),
                    j =>
                    {
                        j.HasKey("OptionSkuid", "Skuid").HasName("PK_COM_SKUOption");
                        j.ToTable("COM_SKUAllowedOption");
                        j.HasIndex(new[] { "Skuid" }, "IX_COM_SKUAllowedOption_SKUID");
                        j.IndexerProperty<int>("OptionSkuid").HasColumnName("OptionSKUID");
                        j.IndexerProperty<int>("Skuid").HasColumnName("SKUID");
                    });

            entity.HasMany(d => d.OptionSkusNavigation).WithMany(p => p.VariantSkus)
                .UsingEntity<Dictionary<string, object>>(
                    "ComVariantOption",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("OptionSkuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_VariantOption_OptionSKUID_COM_SKU"),
                    l => l.HasOne<ComSku>().WithMany()
                        .HasForeignKey("VariantSkuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_VariantOption_VariantSKUID_COM_SKU"),
                    j =>
                    {
                        j.HasKey("VariantSkuid", "OptionSkuid");
                        j.ToTable("COM_VariantOption");
                        j.HasIndex(new[] { "OptionSkuid" }, "IX_COM_VariantOption_OptionSKUID");
                        j.IndexerProperty<int>("VariantSkuid").HasColumnName("VariantSKUID");
                        j.IndexerProperty<int>("OptionSkuid").HasColumnName("OptionSKUID");
                    });

            entity.HasMany(d => d.Skus).WithMany(p => p.Bundles)
                .UsingEntity<Dictionary<string, object>>(
                    "ComBundle",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("Skuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_Bundle_SKUID_COM_SKU"),
                    l => l.HasOne<ComSku>().WithMany()
                        .HasForeignKey("BundleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_Bundle_BundleID_COM_SKU"),
                    j =>
                    {
                        j.HasKey("BundleId", "Skuid");
                        j.ToTable("COM_Bundle");
                        j.HasIndex(new[] { "Skuid" }, "IX_COM_Bundle_SKUID");
                        j.IndexerProperty<int>("BundleId").HasColumnName("BundleID");
                        j.IndexerProperty<int>("Skuid").HasColumnName("SKUID");
                    });

            entity.HasMany(d => d.SkusNavigation).WithMany(p => p.OptionSkus)
                .UsingEntity<Dictionary<string, object>>(
                    "ComSkuallowedOption",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("Skuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_SKUOption_SKUID_COM_SKU"),
                    l => l.HasOne<ComSku>().WithMany()
                        .HasForeignKey("OptionSkuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_SKUOption_OptionSKUID_COM_SKU"),
                    j =>
                    {
                        j.HasKey("OptionSkuid", "Skuid").HasName("PK_COM_SKUOption");
                        j.ToTable("COM_SKUAllowedOption");
                        j.HasIndex(new[] { "Skuid" }, "IX_COM_SKUAllowedOption_SKUID");
                        j.IndexerProperty<int>("OptionSkuid").HasColumnName("OptionSKUID");
                        j.IndexerProperty<int>("Skuid").HasColumnName("SKUID");
                    });

            entity.HasMany(d => d.VariantSkus).WithMany(p => p.OptionSkusNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ComVariantOption",
                    r => r.HasOne<ComSku>().WithMany()
                        .HasForeignKey("VariantSkuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_VariantOption_VariantSKUID_COM_SKU"),
                    l => l.HasOne<ComSku>().WithMany()
                        .HasForeignKey("OptionSkuid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_COM_VariantOption_OptionSKUID_COM_SKU"),
                    j =>
                    {
                        j.HasKey("VariantSkuid", "OptionSkuid");
                        j.ToTable("COM_VariantOption");
                        j.HasIndex(new[] { "OptionSkuid" }, "IX_COM_VariantOption_OptionSKUID");
                        j.IndexerProperty<int>("VariantSkuid").HasColumnName("VariantSKUID");
                        j.IndexerProperty<int>("OptionSkuid").HasColumnName("OptionSKUID");
                    });
        });

        modelBuilder.Entity<ComSkufile>(entity =>
        {
            entity.HasOne(d => d.FileSku).WithMany(p => p.ComSkufiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_SKUFile_COM_SKU");
        });

        modelBuilder.Entity<ComSkuoptionCategory>(entity =>
        {
            entity.HasOne(d => d.Category).WithMany(p => p.ComSkuoptionCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_SKUOptionCategory_CategoryID_COM_OptionCategory");

            entity.HasOne(d => d.Sku).WithMany(p => p.ComSkuoptionCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_SKUOptionCategory_SKUID_COM_SKU");
        });

        modelBuilder.Entity<ComSupplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).IsClustered(false);

            entity.HasIndex(e => new { e.SupplierDisplayName, e.SupplierEnabled }, "IX_COM_Supplier_SupplierDisplayName_SupplierEnabled").IsClustered();

            entity.Property(e => e.SupplierDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.SupplierEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.SupplierLastModified).HasDefaultValueSql("('9/21/2012 12:34:09 PM')");

            entity.HasOne(d => d.SupplierSite).WithMany(p => p.ComSuppliers).HasConstraintName("FK_COM_Supplier_SupplierSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComTaxClass>(entity =>
        {
            entity.HasKey(e => e.TaxClassId).IsClustered(false);

            entity.HasIndex(e => e.TaxClassDisplayName, "IX_COM_TaxClass_TaxClassDisplayName").IsClustered();

            entity.Property(e => e.TaxClassDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.TaxClassLastModified).HasDefaultValueSql("('9/20/2012 1:31:27 PM')");
            entity.Property(e => e.TaxClassName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.TaxClassZeroIfIdsupplied).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.TaxClassSite).WithMany(p => p.ComTaxClasses).HasConstraintName("FK_COM_TaxClass_TaxClassSiteID_CMS_Site");
        });

        modelBuilder.Entity<ComTaxClassCountry>(entity =>
        {
            entity.HasOne(d => d.Country).WithMany(p => p.ComTaxClassCountries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_TaxCategoryCountry_CountryID_CMS_Country");

            entity.HasOne(d => d.TaxClass).WithMany(p => p.ComTaxClassCountries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_TaxCategoryCountry_TaxClassID_COM_TaxClass");
        });

        modelBuilder.Entity<ComTaxClassState>(entity =>
        {
            entity.HasOne(d => d.State).WithMany(p => p.ComTaxClassStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_TaxClassState_StateID_CMS_State");

            entity.HasOne(d => d.TaxClass).WithMany(p => p.ComTaxClassStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_TaxClassState_TaxClassID_COM_TaxClass");
        });

        modelBuilder.Entity<ComVolumeDiscount>(entity =>
        {
            entity.HasOne(d => d.VolumeDiscountSku).WithMany(p => p.ComVolumeDiscounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_VolumeDiscount_VolumeDiscountSKUID_COM_SKU");
        });

        modelBuilder.Entity<ComWishlist>(entity =>
        {
            entity.HasOne(d => d.Site).WithMany(p => p.ComWishlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Wishlist_SiteID_CMS_Site");

            entity.HasOne(d => d.Sku).WithMany(p => p.ComWishlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Wishlist_SKUID_COM_SKU");

            entity.HasOne(d => d.User).WithMany(p => p.ComWishlists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_COM_Wishlist_UserID_CMS_User");
        });

        modelBuilder.Entity<ExportHistory>(entity =>
        {
            entity.HasKey(e => e.ExportId).IsClustered(false);

            entity.HasIndex(e => e.ExportDateTime, "IX_Export_History_ExportDateTime")
                .IsDescending()
                .IsClustered();

            entity.Property(e => e.ExportFileName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ExportSite).WithMany(p => p.ExportHistories).HasConstraintName("FK_Export_History_ExportSiteID_CMS_Site");

            entity.HasOne(d => d.ExportUser).WithMany(p => p.ExportHistories).HasConstraintName("FK_Export_History_ExportUserID_CMS_User");
        });

        modelBuilder.Entity<ExportTask>(entity =>
        {
            entity.HasOne(d => d.TaskSite).WithMany(p => p.ExportTasks).HasConstraintName("FK_Export_Task_TaskSiteID_CMS_Site");
        });

        modelBuilder.Entity<IntegrationConnector>(entity =>
        {
            entity.HasKey(e => e.ConnectorId).IsClustered(false);

            entity.HasIndex(e => e.ConnectorDisplayName, "IX_Integration_Connector_ConnectorDisplayName").IsClustered();

            entity.Property(e => e.ConnectorEnabled).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<IntegrationSyncLog>(entity =>
        {
            entity.HasOne(d => d.SyncLogSynchronization).WithMany(p => p.IntegrationSyncLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Integration_SyncLog_SyncLogSynchronizationID_Integration_Synchronization");
        });

        modelBuilder.Entity<IntegrationSynchronization>(entity =>
        {
            entity.HasOne(d => d.SynchronizationConnector).WithMany(p => p.IntegrationSynchronizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Integration_Synchronization_SynchronizationConnectorID_Integration_Connector");

            entity.HasOne(d => d.SynchronizationTask).WithMany(p => p.IntegrationSynchronizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Integration_Synchronization_SynchronizationTaskID_Integration_Task");
        });

        modelBuilder.Entity<IntegrationTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).IsClustered(false);

            entity.HasIndex(e => e.TaskNodeAliasPath, "IX_Integration_Task_TaskNodeAliasPath").IsClustered();

            entity.HasOne(d => d.TaskSite).WithMany(p => p.IntegrationTasks).HasConstraintName("FK_IntegrationTask_TaskSiteID_CMS_Site");
        });

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasKey(e => e.FileId).IsClustered(false);

            entity.HasIndex(e => e.FilePath, "IX_Media_File_FilePath").IsClustered();

            entity.Property(e => e.FileCreatedWhen).HasDefaultValueSql("('11/11/2008 4:10:00 PM')");
            entity.Property(e => e.FileModifiedWhen).HasDefaultValueSql("('11/11/2008 4:11:15 PM')");
            entity.Property(e => e.FileTitle).HasDefaultValueSql("('')");

            entity.HasOne(d => d.FileCreatedByUser).WithMany(p => p.MediaFileFileCreatedByUsers).HasConstraintName("FK_Media_File_FileCreatedByUserID_CMS_User");

            entity.HasOne(d => d.FileLibrary).WithMany(p => p.MediaFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_File_FileLibraryID_Media_Library");

            entity.HasOne(d => d.FileModifiedByUser).WithMany(p => p.MediaFileFileModifiedByUsers).HasConstraintName("FK_Media_File_FileModifiedByUserID_CMS_User");

            entity.HasOne(d => d.FileSite).WithMany(p => p.MediaFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_File_FileSiteID_CMS_Site");
        });

        modelBuilder.Entity<MediaLibrary>(entity =>
        {
            entity.HasKey(e => e.LibraryId).IsClustered(false);

            entity.HasIndex(e => new { e.LibrarySiteId, e.LibraryDisplayName }, "IX_Media_Library_LibraryDisplayName").IsClustered();

            entity.Property(e => e.LibraryName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.LibrarySite).WithMany(p => p.MediaLibraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_Library_LibrarySiteID_CMS_Site");
        });

        modelBuilder.Entity<MediaLibraryRolePermission>(entity =>
        {
            entity.HasOne(d => d.Library).WithMany(p => p.MediaLibraryRolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_LibraryRolePermission_LibraryID_Media_Library");

            entity.HasOne(d => d.Permission).WithMany(p => p.MediaLibraryRolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_LibraryRolePermission_PermissionID_CMS_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.MediaLibraryRolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_LibraryRolePermission_RoleID_CMS_Role");
        });

        modelBuilder.Entity<NewsletterAbtest>(entity =>
        {
            entity.Property(e => e.TestLastModified).HasDefaultValueSql("('12/5/2011 4:56:38 PM')");

            entity.HasOne(d => d.TestIssue).WithOne(p => p.NewsletterAbtestTestIssue)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_ABTest_Newsletter_NewsletterIssue");

            entity.HasOne(d => d.TestWinnerIssue).WithMany(p => p.NewsletterAbtestTestWinnerIssues).HasConstraintName("FK_Newsletter_ABTest_TestWinnerIssueID_Newsletter_NewsletterIssue");

            entity.HasOne(d => d.TestWinnerScheduledTask).WithMany(p => p.NewsletterAbtests).HasConstraintName("FK_Newsletter_ABTest_TestWinnerScheduledTaskID_CMS_ScheduledTask");
        });

        modelBuilder.Entity<NewsletterClickedLink>(entity =>
        {
            entity.Property(e => e.ClickedLinkEmail).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ClickedLinkNewsletterLink).WithMany(p => p.NewsletterClickedLinks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_ClickedLink_Newsletter_Link");
        });

        modelBuilder.Entity<NewsletterEmail>(entity =>
        {
            entity.HasOne(d => d.EmailNewsletterIssue).WithMany(p => p.NewsletterEmails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_Emails_EmailNewsletterIssueID_Newsletter_NewsletterIssue");

            entity.HasOne(d => d.EmailSite).WithMany(p => p.NewsletterEmails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_Emails_EmailSiteID_CMS_Site");

            entity.HasOne(d => d.EmailSubscriber).WithMany(p => p.NewsletterEmails).HasConstraintName("FK_Newsletter_Emails_EmailSubscriberID_Newsletter_Subscriber");
        });

        modelBuilder.Entity<NewsletterEmailTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).IsClustered(false);

            entity.HasIndex(e => new { e.TemplateSiteId, e.TemplateDisplayName }, "IX_Newsletter_EmailTemplate_TemplateSiteID_TemplateDisplayName").IsClustered();

            entity.Property(e => e.TemplateDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.TemplateIconClass).HasDefaultValueSql("(N'icon-accordion')");
            entity.Property(e => e.TemplateName).HasDefaultValueSql("('')");
            entity.Property(e => e.TemplateType).HasDefaultValueSql("('')");

            entity.HasOne(d => d.TemplateSite).WithMany(p => p.NewsletterEmailTemplates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_EmailTemplate_TemplateSiteID_CMS_Site");

            entity.HasMany(d => d.Newsletters).WithMany(p => p.Templates)
                .UsingEntity<Dictionary<string, object>>(
                    "NewsletterEmailTemplateNewsletter",
                    r => r.HasOne<NewsletterNewsletter>().WithMany()
                        .HasForeignKey("NewsletterId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Newsletter_EmailTemplateNewsletter_Newsletter_Newsletter"),
                    l => l.HasOne<NewsletterEmailTemplate>().WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Newsletter_EmailTemplateNewsletter_Newsletter_EmailTemplate"),
                    j =>
                    {
                        j.HasKey("TemplateId", "NewsletterId");
                        j.ToTable("Newsletter_EmailTemplateNewsletter");
                        j.HasIndex(new[] { "NewsletterId" }, "IX_Newsletter_EmailTemplateNewsletter_NewsletterID");
                        j.IndexerProperty<int>("TemplateId").HasColumnName("TemplateID");
                        j.IndexerProperty<int>("NewsletterId").HasColumnName("NewsletterID");
                    });
        });

        modelBuilder.Entity<NewsletterEmailWidget>(entity =>
        {
            entity.Property(e => e.EmailWidgetDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.EmailWidgetIconCssClass).HasDefaultValueSql("(N'icon-cogwheel-square')");
            entity.Property(e => e.EmailWidgetLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
            entity.Property(e => e.EmailWidgetName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.EmailWidgetSite).WithMany(p => p.NewsletterEmailWidgets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_EmailWidget_EmailWidgetSiteID_CMS_Site");
        });

        modelBuilder.Entity<NewsletterEmailWidgetTemplate>(entity =>
        {
            entity.HasOne(d => d.EmailWidget).WithMany(p => p.NewsletterEmailWidgetTemplates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_EmailWidgetTemplate_EmailWidgetID_Newsletter_EmailWidget");

            entity.HasOne(d => d.Template).WithMany(p => p.NewsletterEmailWidgetTemplates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_EmailWidgetTemplate_TemplateID_Newsletter_EmailTemplate");
        });

        modelBuilder.Entity<NewsletterIssueContactGroup>(entity =>
        {
            entity.HasOne(d => d.ContactGroup).WithMany(p => p.NewsletterIssueContactGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_IssueContactGroup_ContactGroupID");
        });

        modelBuilder.Entity<NewsletterLink>(entity =>
        {
            entity.HasOne(d => d.LinkIssue).WithMany(p => p.NewsletterLinks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_Link_Newsletter_NewsletterIssue");
        });

        modelBuilder.Entity<NewsletterNewsletter>(entity =>
        {
            entity.HasKey(e => e.NewsletterId).IsClustered(false);

            entity.HasIndex(e => new { e.NewsletterSiteId, e.NewsletterDisplayName }, "IX_Newsletter_Newsletter_NewsletterSiteID_NewsletterDisplayName").IsClustered();

            entity.Property(e => e.NewsletterDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.NewsletterEnableOptIn).HasDefaultValueSql("((0))");
            entity.Property(e => e.NewsletterLastModified).HasDefaultValueSql("('3/13/2015 2:53:28 PM')");
            entity.Property(e => e.NewsletterLogActivity).HasDefaultValueSql("((1))");
            entity.Property(e => e.NewsletterName).HasDefaultValueSql("('')");
            entity.Property(e => e.NewsletterSendOptInConfirmation).HasDefaultValueSql("((0))");
            entity.Property(e => e.NewsletterSenderEmail).HasDefaultValueSql("(N'')");
            entity.Property(e => e.NewsletterSenderName).HasDefaultValueSql("('')");
            entity.Property(e => e.NewsletterSource).HasDefaultValueSql("(N'T')");
            entity.Property(e => e.NewsletterTrackClickedLinks).HasDefaultValueSql("((1))");
            entity.Property(e => e.NewsletterTrackOpenEmails).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.NewsletterDynamicScheduledTask).WithMany(p => p.NewsletterNewsletters).HasConstraintName("FK_Newsletter_Newsletter_NewsletterDynamicScheduledTaskID_CMS_ScheduledTask");

            entity.HasOne(d => d.NewsletterOptInTemplate).WithMany(p => p.NewsletterNewsletterNewsletterOptInTemplates).HasConstraintName("FK_Newsletter_Newsletter_NewsletterOptInTemplateID_EmailTemplate");

            entity.HasOne(d => d.NewsletterSite).WithMany(p => p.NewsletterNewsletters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_Newsletter_NewsletterSiteID_CMS_Site");

            entity.HasOne(d => d.NewsletterUnsubscriptionTemplate).WithMany(p => p.NewsletterNewsletterNewsletterUnsubscriptionTemplates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_Newsletter_NewsletterUnsubscriptionTemplateID_Newsletter_EmailTemplate");
        });

        modelBuilder.Entity<NewsletterNewsletterIssue>(entity =>
        {
            entity.Property(e => e.IssueDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.IssueSubject).HasDefaultValueSql("('')");

            entity.HasOne(d => d.IssueNewsletter).WithMany(p => p.NewsletterNewsletterIssues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_NewsletterIssue_IssueNewsletterID_Newsletter_Newsletter");

            entity.HasOne(d => d.IssueSite).WithMany(p => p.NewsletterNewsletterIssues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_NewsletterIssue_IssueSiteID_CMS_Site");

            entity.HasOne(d => d.IssueTemplate).WithMany(p => p.NewsletterNewsletterIssues).HasConstraintName("FK_Newsletter_NewsletterIssue_IssueTemplateID_Newsletter_EmailTemplate");

            entity.HasOne(d => d.IssueVariantOfIssue).WithMany(p => p.InverseIssueVariantOfIssue).HasConstraintName("FK_Newsletter_NewsletterIssue_IssueVariantOfIssue_NewsletterIssue");
        });

        modelBuilder.Entity<NewsletterOpenedEmail>(entity =>
        {
            entity.Property(e => e.OpenedEmailEmail).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.OpenedEmailIssue).WithMany(p => p.NewsletterOpenedEmails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_OpenedEmail_OpenedEmailIssueID_Newsletter_NewsletterIssue");
        });

        modelBuilder.Entity<NewsletterSubscriber>(entity =>
        {
            entity.HasKey(e => e.SubscriberId).IsClustered(false);

            entity.HasIndex(e => new { e.SubscriberSiteId, e.SubscriberFullName }, "IX_Newsletter_Subscriber_SubscriberSiteID_SubscriberFullName").IsClustered();

            entity.Property(e => e.SubscriberType).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.SubscriberSite).WithMany(p => p.NewsletterSubscribers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_Subscriber_SubscriberSiteID_CMS_Site");
        });

        modelBuilder.Entity<NewsletterSubscriberNewsletter>(entity =>
        {
            entity.Property(e => e.SubscriptionApproved).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Newsletter).WithMany(p => p.NewsletterSubscriberNewsletters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_SubscriberNewsletter_NewsletterID_Newsletter_Newsletter");

            entity.HasOne(d => d.Subscriber).WithMany(p => p.NewsletterSubscriberNewsletters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Newsletter_SubscriberNewsletter_SubscriberID_Newsletter_Subscriber");
        });

        modelBuilder.Entity<NewsletterUnsubscription>(entity =>
        {
            entity.Property(e => e.UnsubscriptionEmail).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.UnsubscriptionFromIssue).WithMany(p => p.NewsletterUnsubscriptions).HasConstraintName("FK_Newsletter_Unsubscription_UnsubscriptionFromIssueID_Newsletter_NewsletterIssue");

            entity.HasOne(d => d.UnsubscriptionNewsletter).WithMany(p => p.NewsletterUnsubscriptions).HasConstraintName("FK_Newsletter_Unsubscription_UnsubscriptionNewsletterID_Newsletter_Newsletter");
        });

        modelBuilder.Entity<OmAbtest>(entity =>
        {
            entity.Property(e => e.AbtestDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.AbtestIncludedTraffic).HasDefaultValueSql("((100))");
            entity.Property(e => e.AbtestName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.AbtestOriginalPage).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.AbtestSite).WithMany(p => p.OmAbtests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ABTest_SiteID_CMS_Site");
        });

        modelBuilder.Entity<OmAbvariantDatum>(entity =>
        {
            entity.Property(e => e.AbvariantDisplayName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.AbvariantTest).WithMany(p => p.OmAbvariantData)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ABVariantData_ABVariantTestID_OM_ABTest");
        });

        modelBuilder.Entity<OmAccount>(entity =>
        {
            entity.HasOne(d => d.AccountCountry).WithMany(p => p.OmAccounts).HasConstraintName("FK_OM_Account_CMS_Country");

            entity.HasOne(d => d.AccountOwnerUser).WithMany(p => p.OmAccounts).HasConstraintName("FK_OM_Account_CMS_User");

            entity.HasOne(d => d.AccountPrimaryContact).WithMany(p => p.OmAccountAccountPrimaryContacts).HasConstraintName("FK_OM_Account_OM_Contact_PrimaryContact");

            entity.HasOne(d => d.AccountSecondaryContact).WithMany(p => p.OmAccountAccountSecondaryContacts).HasConstraintName("FK_OM_Account_OM_Contact_SecondaryContact");

            entity.HasOne(d => d.AccountState).WithMany(p => p.OmAccounts).HasConstraintName("FK_OM_Account_CMS_State");

            entity.HasOne(d => d.AccountStatus).WithMany(p => p.OmAccounts).HasConstraintName("FK_OM_Account_OM_AccountStatus");

            entity.HasOne(d => d.AccountSubsidiaryOf).WithMany(p => p.InverseAccountSubsidiaryOf).HasConstraintName("FK_OM_Account_OM_Account_SubsidiaryOf");
        });

        modelBuilder.Entity<OmAccountContact>(entity =>
        {
            entity.HasOne(d => d.Account).WithMany(p => p.OmAccountContacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_AccountContact_OM_Account");

            entity.HasOne(d => d.Contact).WithMany(p => p.OmAccountContacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_AccountContact_OM_Contact");

            entity.HasOne(d => d.ContactRole).WithMany(p => p.OmAccountContacts).HasConstraintName("FK_OM_AccountContact_OM_ContactRole");
        });

        modelBuilder.Entity<OmActivity>(entity =>
        {
            entity.HasIndex(e => e.ActivityCampaign, "IX_OM_Activity_ActivityCampaign")
                .HasFilter("([ActivityCampaign] IS NOT NULL)")
                .HasFillFactor(90);
        });

        modelBuilder.Entity<OmActivityType>(entity =>
        {
            entity.Property(e => e.ActivityTypeEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.ActivityTypeIsCustom).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<OmContact>(entity =>
        {
            entity.Property(e => e.ContactCreated).HasDefaultValueSql("('5/3/2011 10:51:13 AM')");
            entity.Property(e => e.ContactMonitored).HasDefaultValueSql("((0))");
            entity.Property(e => e.ContactSalesForceLeadReplicationDisabled).HasDefaultValueSql("((0))");
            entity.Property(e => e.ContactSalesForceLeadReplicationRequired).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ContactCountry).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_CMS_Country");

            entity.HasOne(d => d.ContactOwnerUser).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_CMS_User");

            entity.HasOne(d => d.ContactState).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_CMS_State");

            entity.HasOne(d => d.ContactStatus).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_OM_ContactStatus");
        });

        modelBuilder.Entity<OmContactGroup>(entity =>
        {
            entity.HasKey(e => e.ContactGroupId).HasName("PK_CMS_ContactGroup");

            entity.Property(e => e.ContactGroupName).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<OmContactGroupMember>(entity =>
        {
            entity.Property(e => e.ContactGroupMemberFromCondition).HasDefaultValueSql("((0))");
            entity.Property(e => e.ContactGroupMemberFromManual).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ContactGroupMemberContactGroup).WithMany(p => p.OmContactGroupMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ContactGroupMembers_OM_ContactGroup");
        });

        modelBuilder.Entity<OmContactRole>(entity =>
        {
            entity.Property(e => e.ContactRoleDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.ContactRoleName).HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<OmContactStatus>(entity =>
        {
            entity.Property(e => e.ContactStatusDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.ContactStatusName).HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<OmMembership>(entity =>
        {
            entity.HasOne(d => d.Contact).WithMany(p => p.OmMemberships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_Membership_OM_Contact");
        });

        modelBuilder.Entity<OmRule>(entity =>
        {
            entity.Property(e => e.RuleDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.RuleName).HasDefaultValueSql("(N'[_][_]AUTO[_][_]')");
            entity.Property(e => e.RuleType).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.RuleScore).WithMany(p => p.OmRules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_Rule_OM_Score");
        });

        modelBuilder.Entity<OmScore>(entity =>
        {
            entity.HasIndex(e => e.ScorePersonaId, "IX_OM_Score_ScorePersonaID")
                .IsUnique()
                .HasFilter("([ScorePersonaID] IS NOT NULL)");

            entity.HasOne(d => d.ScorePersona).WithOne(p => p.OmScore).HasConstraintName("FK_OM_Score_Personas_Persona");
        });

        modelBuilder.Entity<OmScoreContactRule>(entity =>
        {
            entity.HasOne(d => d.Contact).WithMany(p => p.OmScoreContactRules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ScoreContactRule_OM_Contact");

            entity.HasOne(d => d.Rule).WithMany(p => p.OmScoreContactRules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ScoreContactRule_OM_Rule");

            entity.HasOne(d => d.Score).WithMany(p => p.OmScoreContactRules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ScoreContactRule_OM_Score");
        });

        modelBuilder.Entity<OmVisitorToContact>(entity =>
        {
            entity.HasOne(d => d.VisitorToContactContact).WithMany(p => p.OmVisitorToContacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_VisitorToContact_OM_Contact_Cascade");
        });

        modelBuilder.Entity<PersonasPersona>(entity =>
        {
            entity.Property(e => e.PersonaDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.PersonaEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.PersonaName).HasDefaultValueSql("(N'[_][_]AUTO[_][_]')");
            entity.Property(e => e.PersonaPointsThreshold).HasDefaultValueSql("((100))");
        });

        modelBuilder.Entity<PersonasPersonaContactHistory>(entity =>
        {
            entity.Property(e => e.PersonaContactHistoryDate).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");

            entity.HasOne(d => d.PersonaContactHistoryPersona).WithMany(p => p.PersonasPersonaContactHistories).HasConstraintName("FK_Personas_PersonaContactHistory_Personas_Persona");
        });

        modelBuilder.Entity<ReportingReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).IsClustered(false);

            entity.HasIndex(e => new { e.ReportDisplayName, e.ReportCategoryId }, "IX_Reporting_Report_ReportCategoryID_ReportDisplayName").IsClustered();

            entity.Property(e => e.ReportAccess).HasDefaultValueSql("((1))");
            entity.Property(e => e.ReportDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.ReportEnableSubscription).HasDefaultValueSql("((0))");
            entity.Property(e => e.ReportName).HasDefaultValueSql("('')");

            entity.HasOne(d => d.ReportCategory).WithMany(p => p.ReportingReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_Report_ReportCategoryID_Reporting_ReportCategory");
        });

        modelBuilder.Entity<ReportingReportCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => e.CategoryPath, "IX_Reporting_ReportCategory_CategoryPath")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.CategoryCodeName).HasDefaultValueSql("('')");
            entity.Property(e => e.CategoryDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.CategoryPath).HasDefaultValueSql("('')");

            entity.HasOne(d => d.CategoryParent).WithMany(p => p.InverseCategoryParent).HasConstraintName("FK_Reporting_ReportCategory_CategoryID_Reporting_ReportCategory_ParentCategoryID");
        });

        modelBuilder.Entity<ReportingReportGraph>(entity =>
        {
            entity.HasOne(d => d.GraphReport).WithMany(p => p.ReportingReportGraphs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_ReportGraph_GraphReportID_Reporting_Report");
        });

        modelBuilder.Entity<ReportingReportSubscription>(entity =>
        {
            entity.Property(e => e.ReportSubscriptionEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.ReportSubscriptionInterval).HasDefaultValueSql("('')");
            entity.Property(e => e.ReportSubscriptionLastModified).HasDefaultValueSql("('3/9/2012 11:17:19 AM')");
            entity.Property(e => e.ReportSubscriptionOnlyNonEmpty).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.ReportSubscriptionGraph).WithMany(p => p.ReportingReportSubscriptions).HasConstraintName("FK_Reporting_ReportSubscription_ReportSubscriptionGraphID_Reporting_ReportGraph");

            entity.HasOne(d => d.ReportSubscriptionReport).WithMany(p => p.ReportingReportSubscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_ReportSubscription_ReportSubscriptionReportID_Reporting_Report");

            entity.HasOne(d => d.ReportSubscriptionSite).WithMany(p => p.ReportingReportSubscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_ReportSubscription_ReportSubscriptionSiteID_CMS_Site");

            entity.HasOne(d => d.ReportSubscriptionTable).WithMany(p => p.ReportingReportSubscriptions).HasConstraintName("FK_Reporting_ReportSubscription_ReportSubscriptionTableID_Reporting_ReportTable");

            entity.HasOne(d => d.ReportSubscriptionUser).WithMany(p => p.ReportingReportSubscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_ReportSubscription_ReportSubscriptionUserID_CMS_User");

            entity.HasOne(d => d.ReportSubscriptionValue).WithMany(p => p.ReportingReportSubscriptions).HasConstraintName("FK_Reporting_ReportSubscription_ReportSubscriptionValueID_Reporting_ReportValue");
        });

        modelBuilder.Entity<ReportingReportTable>(entity =>
        {
            entity.HasOne(d => d.TableReport).WithMany(p => p.ReportingReportTables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_ReportTable_TableReportID_Reporting_Report");
        });

        modelBuilder.Entity<ReportingReportValue>(entity =>
        {
            entity.HasOne(d => d.ValueReport).WithMany(p => p.ReportingReportValues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_ReportValue_ValueReportID_Reporting_Report");
        });

        modelBuilder.Entity<ReportingSavedGraph>(entity =>
        {
            entity.HasOne(d => d.SavedGraphSavedReport).WithMany(p => p.ReportingSavedGraphs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_SavedGraph_SavedGraphSavedReportID_Reporting_SavedReport");
        });

        modelBuilder.Entity<ReportingSavedReport>(entity =>
        {
            entity.HasKey(e => e.SavedReportId).IsClustered(false);

            entity.HasIndex(e => new { e.SavedReportReportId, e.SavedReportDate }, "IX_Reporting_SavedReport_SavedReportReportID_SavedReportDate")
                .IsDescending(false, true)
                .IsClustered();

            entity.HasOne(d => d.SavedReportCreatedByUser).WithMany(p => p.ReportingSavedReports).HasConstraintName("FK_Reporting_SavedReport_SavedReportCreatedByUserID_CMS_User");

            entity.HasOne(d => d.SavedReportReport).WithMany(p => p.ReportingSavedReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporting_SavedReport_SavedReportReportID_Reporting_Report");
        });

        modelBuilder.Entity<SharePointSharePointConnection>(entity =>
        {
            entity.HasOne(d => d.SharePointConnectionSite).WithMany(p => p.SharePointSharePointConnections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SharePoint_SharePointConnection_CMS_Site");
        });

        modelBuilder.Entity<SharePointSharePointFile>(entity =>
        {
            entity.Property(e => e.SharePointFileEtag).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointFileExtension).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointFileMimeType).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointFileName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointFileServerRelativeUrl).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.SharePointFileSharePointLibrary).WithMany(p => p.SharePointSharePointFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SharePoint_SharePointFile_SharePoint_SharePointLibrary");

            entity.HasOne(d => d.SharePointFileSite).WithMany(p => p.SharePointSharePointFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SharePoint_SharePointFile_CMS_Site");
        });

        modelBuilder.Entity<SharePointSharePointLibrary>(entity =>
        {
            entity.Property(e => e.SharePointLibraryDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointLibraryLastModified).HasDefaultValueSql("('10/3/2014 2:45:04 PM')");
            entity.Property(e => e.SharePointLibraryListTitle).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointLibraryName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.SharePointLibrarySharePointConnectionId).HasDefaultValueSql("((0))");
            entity.Property(e => e.SharePointLibrarySynchronizationPeriod).HasDefaultValueSql("((720))");

            entity.HasOne(d => d.SharePointLibrarySharePointConnection).WithMany(p => p.SharePointSharePointLibraries).HasConstraintName("FK_SharePoint_SharePointLibrary_SharePoint_SharePointConnection");

            entity.HasOne(d => d.SharePointLibrarySite).WithMany(p => p.SharePointSharePointLibraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SharePoint_SharePointLibrary_CMS_Site");
        });

        modelBuilder.Entity<SmFacebookAccount>(entity =>
        {
            entity.Property(e => e.FacebookAccountPageAccessToken).HasDefaultValueSql("('')");
            entity.Property(e => e.FacebookAccountPageId).HasDefaultValueSql("('')");

            entity.HasOne(d => d.FacebookAccountFacebookApplication).WithMany(p => p.SmFacebookAccounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_FacebookAccount_SM_FacebookApplication");

            entity.HasOne(d => d.FacebookAccountSite).WithMany(p => p.SmFacebookAccounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_FacebookAccount_CMS_Site");
        });

        modelBuilder.Entity<SmFacebookApplication>(entity =>
        {
            entity.Property(e => e.FacebookApplicationConsumerKey).HasDefaultValueSql("('')");
            entity.Property(e => e.FacebookApplicationConsumerSecret).HasDefaultValueSql("('')");
            entity.Property(e => e.FacebookApplicationLastModified).HasDefaultValueSql("('5/28/2013 1:02:36 PM')");

            entity.HasOne(d => d.FacebookApplicationSite).WithMany(p => p.SmFacebookApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_FacebookApplication_CMS_Site");
        });

        modelBuilder.Entity<SmFacebookPost>(entity =>
        {
            entity.Property(e => e.FacebookPostIsCreatedByUser).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.FacebookPostFacebookAccount).WithMany(p => p.SmFacebookPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_FacebookPost_SM_FacebookAccount");

            entity.HasOne(d => d.FacebookPostSite).WithMany(p => p.SmFacebookPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_FacebookPost_CMS_Site");
        });

        modelBuilder.Entity<SmInsight>(entity =>
        {
            entity.Property(e => e.InsightExternalId).HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<SmInsightHitDay>(entity =>
        {
            entity.HasOne(d => d.InsightHitInsight).WithMany(p => p.SmInsightHitDays).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SmInsightHitMonth>(entity =>
        {
            entity.HasOne(d => d.InsightHitInsight).WithMany(p => p.SmInsightHitMonths).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SmInsightHitWeek>(entity =>
        {
            entity.HasOne(d => d.InsightHitInsight).WithMany(p => p.SmInsightHitWeeks).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SmInsightHitYear>(entity =>
        {
            entity.HasOne(d => d.InsightHitInsight).WithMany(p => p.SmInsightHitYears).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SmLinkedInAccount>(entity =>
        {
            entity.Property(e => e.LinkedInAccountAccessToken).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInAccountDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInAccountName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInAccountProfileId).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<SmLinkedInApplication>(entity =>
        {
            entity.Property(e => e.LinkedInApplicationConsumerKey).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInApplicationConsumerSecret).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInApplicationDisplayName).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInApplicationName).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.LinkedInApplicationSite).WithMany(p => p.SmLinkedInApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_LinkedInApplication_CMS_Site");
        });

        modelBuilder.Entity<SmLinkedInPost>(entity =>
        {
            entity.Property(e => e.LinkedInPostComment).HasDefaultValueSql("(N'')");
            entity.Property(e => e.LinkedInPostIsCreatedByUser).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.LinkedInPostLinkedInAccount).WithMany(p => p.SmLinkedInPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_LinkedInPost_SM_LinkedInAccount");

            entity.HasOne(d => d.LinkedInPostSite).WithMany(p => p.SmLinkedInPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_LinkedInPost_CMS_Site");
        });

        modelBuilder.Entity<SmTwitterAccount>(entity =>
        {
            entity.Property(e => e.TwitterAccountAccessToken).HasDefaultValueSql("('')");
            entity.Property(e => e.TwitterAccountAccessTokenSecret).HasDefaultValueSql("('')");

            entity.HasOne(d => d.TwitterAccountSite).WithMany(p => p.SmTwitterAccounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_TwitterAccount_CMS_Site");

            entity.HasOne(d => d.TwitterAccountTwitterApplication).WithMany(p => p.SmTwitterAccounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_TwitterAccount_SM_TwitterApplication");
        });

        modelBuilder.Entity<SmTwitterApplication>(entity =>
        {
            entity.Property(e => e.TwitterApplicationConsumerKey).HasDefaultValueSql("('')");
            entity.Property(e => e.TwitterApplicationConsumerSecret).HasDefaultValueSql("('')");

            entity.HasOne(d => d.TwitterApplicationSite).WithMany(p => p.SmTwitterApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_TwitterApplication_CMS_Site");
        });

        modelBuilder.Entity<SmTwitterPost>(entity =>
        {
            entity.Property(e => e.TwitterPostIsCreatedByUser).HasDefaultValueSql("((1))");
            entity.Property(e => e.TwitterPostText).HasDefaultValueSql("('')");

            entity.HasOne(d => d.TwitterPostSite).WithMany(p => p.SmTwitterPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_TwitterPost_CMS_Site");

            entity.HasOne(d => d.TwitterPostTwitterAccount).WithMany(p => p.SmTwitterPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SM_TwitterPost_SM_TwitterAccount");
        });

        modelBuilder.Entity<StagingServer>(entity =>
        {
            entity.HasKey(e => e.ServerId).IsClustered(false);

            entity.HasIndex(e => new { e.ServerSiteId, e.ServerDisplayName }, "IX_Staging_Server_ServerSiteID_ServerDisplayName").IsClustered();

            entity.Property(e => e.ServerAuthentication).HasDefaultValueSql("('USERNAME')");
            entity.Property(e => e.ServerDisplayName).HasDefaultValueSql("('')");
            entity.Property(e => e.ServerEnabled).HasDefaultValueSql("((1))");
            entity.Property(e => e.ServerName).HasDefaultValueSql("('')");
            entity.Property(e => e.ServerUrl).HasDefaultValueSql("(N'')");

            entity.HasOne(d => d.ServerSite).WithMany(p => p.StagingServers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_Server_ServerSiteID_CMS_Site");
        });

        modelBuilder.Entity<StagingSynchronization>(entity =>
        {
            entity.HasOne(d => d.SynchronizationServer).WithMany(p => p.StagingSynchronizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_Synchronization_SynchronizationServerID_Staging_Server");

            entity.HasOne(d => d.SynchronizationTask).WithMany(p => p.StagingSynchronizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_Synchronization_SynchronizationTaskID_Staging_Task");
        });

        modelBuilder.Entity<StagingTask>(entity =>
        {
            entity.Property(e => e.TaskServers).HasDefaultValueSql("('null')");

            entity.HasOne(d => d.TaskSite).WithMany(p => p.StagingTasks).HasConstraintName("FK_Staging_Task_TaskSiteID_CMS_Site");
        });

        modelBuilder.Entity<StagingTaskGroup>(entity =>
        {
            entity.Property(e => e.TaskGroupCodeName).HasDefaultValueSql("(N'')");
        });

        modelBuilder.Entity<StagingTaskGroupTask>(entity =>
        {
            entity.HasOne(d => d.TaskGroup).WithMany(p => p.StagingTaskGroupTasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_TaskGroupTask_Staging_TaskGroup");

            entity.HasOne(d => d.Task).WithMany(p => p.StagingTaskGroupTasks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_TaskGroupTask_Staging_Task");
        });

        modelBuilder.Entity<StagingTaskGroupUser>(entity =>
        {
            entity.HasOne(d => d.TaskGroup).WithMany(p => p.StagingTaskGroupUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_TaskGroupUser_Staging_TaskGroup");

            entity.HasOne(d => d.User).WithOne(p => p.StagingTaskGroupUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_TaskGroupUser_CMS_User");
        });

        modelBuilder.Entity<StagingTaskUser>(entity =>
        {
            entity.HasOne(d => d.Task).WithMany(p => p.StagingTaskUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_TaskUser_StagingTask");

            entity.HasOne(d => d.User).WithMany(p => p.StagingTaskUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staging_TaskUser_CMS_User");
        });

        modelBuilder.Entity<TempFile>(entity =>
        {
            entity.Property(e => e.FileDirectory).HasDefaultValueSql("('')");
            entity.Property(e => e.FileExtension).HasDefaultValueSql("('')");
            entity.Property(e => e.FileLastModified).HasDefaultValueSql("('6/29/2010 1:57:54 PM')");
            entity.Property(e => e.FileMimeType).HasDefaultValueSql("('')");
            entity.Property(e => e.FileName).HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<TempPageBuilderWidget>(entity =>
        {
            entity.Property(e => e.PageBuilderWidgetsLastModified).HasDefaultValueSql("('1/1/0001 12:00:00 AM')");
        });

        modelBuilder.Entity<ViewCmsAclitemItemsAndOperator>(entity =>
        {
            entity.ToView("View_CMS_ACLItem_ItemsAndOperators");
        });

        modelBuilder.Entity<ViewCmsObjectVersionHistoryUserJoined>(entity =>
        {
            entity.ToView("View_CMS_ObjectVersionHistoryUser_Joined");
        });

        modelBuilder.Entity<ViewCmsPageTemplateCategoryPageTemplateJoined>(entity =>
        {
            entity.ToView("View_CMS_PageTemplateCategoryPageTemplate_Joined");
        });

        modelBuilder.Entity<ViewCmsRelationshipJoined>(entity =>
        {
            entity.ToView("View_CMS_Relationship_Joined");
        });

        modelBuilder.Entity<ViewCmsResourceStringJoined>(entity =>
        {
            entity.ToView("View_CMS_ResourceString_Joined");
        });

        modelBuilder.Entity<ViewCmsResourceTranslatedJoined>(entity =>
        {
            entity.ToView("View_CMS_ResourceTranslated_Joined");
        });

        modelBuilder.Entity<ViewCmsRoleResourcePermissionJoined>(entity =>
        {
            entity.ToView("View_CMS_RoleResourcePermission_Joined");
        });

        modelBuilder.Entity<ViewCmsSiteDocumentCount>(entity =>
        {
            entity.ToView("View_CMS_Site_DocumentCount");

            entity.Property(e => e.SiteId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ViewCmsSiteRoleResourceUielementJoined>(entity =>
        {
            entity.ToView("View_CMS_SiteRoleResourceUIElement_Joined");
        });

        modelBuilder.Entity<ViewCmsTreeJoined>(entity =>
        {
            entity.ToView("View_CMS_Tree_Joined");
        });

        modelBuilder.Entity<ViewCmsUser>(entity =>
        {
            entity.ToView("View_CMS_User");
        });

        modelBuilder.Entity<ViewCmsUserDocument>(entity =>
        {
            entity.ToView("View_CMS_UserDocuments");
        });

        modelBuilder.Entity<ViewCmsUserRoleJoined>(entity =>
        {
            entity.ToView("View_CMS_UserRole_Joined");
        });

        modelBuilder.Entity<ViewCmsUserRoleMembershipRole>(entity =>
        {
            entity.ToView("View_CMS_UserRoleMembershipRole");
        });

        modelBuilder.Entity<ViewCmsUserRoleMembershipRoleValidOnlyJoined>(entity =>
        {
            entity.ToView("View_CMS_UserRole_MembershipRole_ValidOnly_Joined");
        });

        modelBuilder.Entity<ViewCmsUserSettingsRoleJoined>(entity =>
        {
            entity.ToView("View_CMS_UserSettingsRole_Joined");
        });

        modelBuilder.Entity<ViewCmsWebPartCategoryWebpartJoined>(entity =>
        {
            entity.ToView("View_CMS_WebPartCategoryWebpart_Joined");
        });

        modelBuilder.Entity<ViewCmsWidgetCategoryWidgetJoined>(entity =>
        {
            entity.ToView("View_CMS_WidgetCategoryWidget_Joined");
        });

        modelBuilder.Entity<ViewComSkuoptionCategoryOptionCategoryJoined>(entity =>
        {
            entity.ToView("View_COM_SKUOptionCategory_OptionCategory_Joined");
        });

        modelBuilder.Entity<ViewIntegrationTaskJoined>(entity =>
        {
            entity.ToView("View_Integration_Task_Joined");
        });

        modelBuilder.Entity<ViewMembershipMembershipUserJoined>(entity =>
        {
            entity.ToView("View_Membership_MembershipUser_Joined");
        });

        modelBuilder.Entity<ViewNewsletterSubscriptionsJoined>(entity =>
        {
            entity.ToView("View_Newsletter_Subscriptions_Joined");
        });

        modelBuilder.Entity<ViewOmAccountContactAccountJoined>(entity =>
        {
            entity.ToView("View_OM_AccountContact_AccountJoined");
        });

        modelBuilder.Entity<ViewOmAccountContactContactJoined>(entity =>
        {
            entity.ToView("View_OM_AccountContact_ContactJoined");
        });

        modelBuilder.Entity<ViewOmAccountJoined>(entity =>
        {
            entity.ToView("View_OM_Account_Joined");
        });

        modelBuilder.Entity<ViewOmContactGroupMemberAccountJoined>(entity =>
        {
            entity.ToView("View_OM_ContactGroupMember_AccountJoined");
        });

        modelBuilder.Entity<ViewReportingCategoryReportJoined>(entity =>
        {
            entity.ToView("View_Reporting_CategoryReport_Joined");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
