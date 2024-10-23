using Microsoft.EntityFrameworkCore;

using Migration.Tool.KXP.Models;

namespace Migration.Tool.KXP.Context;

public partial class KxpContext : DbContext
{
    public KxpContext()
    {
    }

    public KxpContext(DbContextOptions<KxpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CdMigration> CdMigrations { get; set; }

    public virtual DbSet<CiFileMetadatum> CiFileMetadata { get; set; }

    public virtual DbSet<CiMigration> CiMigrations { get; set; }

    public virtual DbSet<CmsAlternativeForm> CmsAlternativeForms { get; set; }

    public virtual DbSet<CmsApplicationPermission> CmsApplicationPermissions { get; set; }

    public virtual DbSet<CmsAutomationHistory> CmsAutomationHistories { get; set; }

    public virtual DbSet<CmsAutomationState> CmsAutomationStates { get; set; }

    public virtual DbSet<CmsAutomationTemplate> CmsAutomationTemplates { get; set; }

    public virtual DbSet<CmsChannel> CmsChannels { get; set; }

    public virtual DbSet<CmsClass> CmsClasses { get; set; }

    public virtual DbSet<CmsConsent> CmsConsents { get; set; }

    public virtual DbSet<CmsConsentAgreement> CmsConsentAgreements { get; set; }

    public virtual DbSet<CmsConsentArchive> CmsConsentArchives { get; set; }

    public virtual DbSet<CmsContentFolder> CmsContentFolders { get; set; }

    public virtual DbSet<CmsContentItem> CmsContentItems { get; set; }

    public virtual DbSet<CmsContentItemCommonDatum> CmsContentItemCommonData { get; set; }

    public virtual DbSet<CmsContentItemLanguageMetadatum> CmsContentItemLanguageMetadata { get; set; }

    public virtual DbSet<CmsContentItemReference> CmsContentItemReferences { get; set; }

    public virtual DbSet<CmsContentItemTag> CmsContentItemTags { get; set; }

    public virtual DbSet<CmsContentLanguage> CmsContentLanguages { get; set; }

    public virtual DbSet<CmsContentTypeChannel> CmsContentTypeChannels { get; set; }

    public virtual DbSet<CmsContentWorkflow> CmsContentWorkflows { get; set; }

    public virtual DbSet<CmsContentWorkflowContentType> CmsContentWorkflowContentTypes { get; set; }

    public virtual DbSet<CmsContentWorkflowStep> CmsContentWorkflowSteps { get; set; }

    public virtual DbSet<CmsContentWorkflowStepRole> CmsContentWorkflowStepRoles { get; set; }

    public virtual DbSet<CmsCountry> CmsCountries { get; set; }

    public virtual DbSet<CmsCulture> CmsCultures { get; set; }

    public virtual DbSet<CmsEmail> CmsEmails { get; set; }

    public virtual DbSet<CmsEmailAttachment> CmsEmailAttachments { get; set; }

    public virtual DbSet<CmsEventLog> CmsEventLogs { get; set; }

    public virtual DbSet<CmsExternalLogin> CmsExternalLogins { get; set; }

    public virtual DbSet<CmsForm> CmsForms { get; set; }

    public virtual DbSet<CmsFormFeaturedField> CmsFormFeaturedFields { get; set; }

    public virtual DbSet<CmsHeadlessChannel> CmsHeadlessChannels { get; set; }

    public virtual DbSet<CmsHeadlessItem> CmsHeadlessItems { get; set; }

    public virtual DbSet<CmsHeadlessToken> CmsHeadlessTokens { get; set; }

    public virtual DbSet<CmsLicenseKey> CmsLicenseKeys { get; set; }

    public virtual DbSet<CmsMacroIdentity> CmsMacroIdentities { get; set; }

    public virtual DbSet<CmsMacroRule> CmsMacroRules { get; set; }

    public virtual DbSet<CmsMacroRuleCategory> CmsMacroRuleCategories { get; set; }

    public virtual DbSet<CmsMacroRuleMacroRuleCategory> CmsMacroRuleMacroRuleCategories { get; set; }

    public virtual DbSet<CmsMember> CmsMembers { get; set; }

    public virtual DbSet<CmsMemberExternalLogin> CmsMemberExternalLogins { get; set; }

    public virtual DbSet<CmsObjectWorkflowTrigger> CmsObjectWorkflowTriggers { get; set; }

    public virtual DbSet<CmsPageTemplateConfiguration> CmsPageTemplateConfigurations { get; set; }

    public virtual DbSet<CmsQuery> CmsQueries { get; set; }

    public virtual DbSet<CmsResource> CmsResources { get; set; }

    public virtual DbSet<CmsResourceString> CmsResourceStrings { get; set; }

    public virtual DbSet<CmsResourceTranslation> CmsResourceTranslations { get; set; }

    public virtual DbSet<CmsRole> CmsRoles { get; set; }

    public virtual DbSet<CmsScheduledTask> CmsScheduledTasks { get; set; }

    public virtual DbSet<CmsSettingsCategory> CmsSettingsCategories { get; set; }

    public virtual DbSet<CmsSettingsKey> CmsSettingsKeys { get; set; }

    public virtual DbSet<CmsSmartFolder> CmsSmartFolders { get; set; }

    public virtual DbSet<CmsState> CmsStates { get; set; }

    public virtual DbSet<CmsTag> CmsTags { get; set; }

    public virtual DbSet<CmsTaxonomy> CmsTaxonomies { get; set; }

    public virtual DbSet<CmsUser> CmsUsers { get; set; }

    public virtual DbSet<CmsUserMacroIdentity> CmsUserMacroIdentities { get; set; }

    public virtual DbSet<CmsUserRole> CmsUserRoles { get; set; }

    public virtual DbSet<CmsWebFarmServer> CmsWebFarmServers { get; set; }

    public virtual DbSet<CmsWebFarmServerLog> CmsWebFarmServerLogs { get; set; }

    public virtual DbSet<CmsWebFarmServerMonitoring> CmsWebFarmServerMonitorings { get; set; }

    public virtual DbSet<CmsWebFarmServerTask> CmsWebFarmServerTasks { get; set; }

    public virtual DbSet<CmsWebFarmTask> CmsWebFarmTasks { get; set; }

    public virtual DbSet<CmsWebPageFormerUrlPath> CmsWebPageFormerUrlPaths { get; set; }

    public virtual DbSet<CmsWebPageItem> CmsWebPageItems { get; set; }

    public virtual DbSet<CmsWebPageUrlPath> CmsWebPageUrlPaths { get; set; }

    public virtual DbSet<CmsWebsiteCaptchaSetting> CmsWebsiteCaptchaSettings { get; set; }

    public virtual DbSet<CmsWebsiteChannel> CmsWebsiteChannels { get; set; }

    public virtual DbSet<CmsWorkflow> CmsWorkflows { get; set; }

    public virtual DbSet<CmsWorkflowAction> CmsWorkflowActions { get; set; }

    public virtual DbSet<CmsWorkflowStep> CmsWorkflowSteps { get; set; }

    public virtual DbSet<CmsWorkflowTransition> CmsWorkflowTransitions { get; set; }

    public virtual DbSet<EmailLibraryEmailBounce> EmailLibraryEmailBounces { get; set; }

    public virtual DbSet<EmailLibraryEmailChannel> EmailLibraryEmailChannels { get; set; }

    public virtual DbSet<EmailLibraryEmailChannelSender> EmailLibraryEmailChannelSenders { get; set; }

    public virtual DbSet<EmailLibraryEmailConfiguration> EmailLibraryEmailConfigurations { get; set; }

    public virtual DbSet<EmailLibraryEmailLink> EmailLibraryEmailLinks { get; set; }

    public virtual DbSet<EmailLibraryEmailMarketingRecipient> EmailLibraryEmailMarketingRecipients { get; set; }

    public virtual DbSet<EmailLibraryEmailStatistic> EmailLibraryEmailStatistics { get; set; }

    public virtual DbSet<EmailLibraryEmailStatisticsHit> EmailLibraryEmailStatisticsHits { get; set; }

    public virtual DbSet<EmailLibraryEmailSubscriptionConfirmation> EmailLibraryEmailSubscriptionConfirmations { get; set; }

    public virtual DbSet<EmailLibraryEmailTemplate> EmailLibraryEmailTemplates { get; set; }

    public virtual DbSet<EmailLibraryEmailTemplateContentType> EmailLibraryEmailTemplateContentTypes { get; set; }

    public virtual DbSet<EmailLibraryRecipientListSetting> EmailLibraryRecipientListSettings { get; set; }

    public virtual DbSet<EmailLibrarySendConfiguration> EmailLibrarySendConfigurations { get; set; }

    public virtual DbSet<MediaFile> MediaFiles { get; set; }

    public virtual DbSet<MediaLibrary> MediaLibraries { get; set; }

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

    public virtual DbSet<OmTrackedWebsite> OmTrackedWebsites { get; set; }

    public virtual DbSet<OmVisitorToContact> OmVisitorToContacts { get; set; }

    public virtual DbSet<TempFile> TempFiles { get; set; }

    public virtual DbSet<TempPageBuilderWidget> TempPageBuilderWidgets { get; set; }

    public virtual DbSet<ViewCmsResourceStringJoined> ViewCmsResourceStringJoineds { get; set; }

    public virtual DbSet<ViewCmsResourceTranslatedJoined> ViewCmsResourceTranslatedJoineds { get; set; }

    public virtual DbSet<ViewOmAccountContactAccountJoined> ViewOmAccountContactAccountJoineds { get; set; }

    public virtual DbSet<ViewOmAccountContactContactJoined> ViewOmAccountContactContactJoineds { get; set; }

    public virtual DbSet<ViewOmAccountJoined> ViewOmAccountJoineds { get; set; }

    public virtual DbSet<ViewOmContactGroupMemberAccountJoined> ViewOmContactGroupMemberAccountJoineds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CdMigration>(entity =>
        {
            entity.Property(e => e.DateApplied).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.RowsAffected).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<CiFileMetadatum>(entity =>
        {
            entity.Property(e => e.FileHash).HasDefaultValue("");
            entity.Property(e => e.FileLocation).HasDefaultValue("");
        });

        modelBuilder.Entity<CiMigration>(entity =>
        {
            entity.Property(e => e.DateApplied).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.RowsAffected).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<CmsAlternativeForm>(entity =>
        {
            entity.Property(e => e.FormDisplayName).HasDefaultValue("");
            entity.Property(e => e.FormIsCustom).HasDefaultValue(false);
            entity.Property(e => e.FormName).HasDefaultValue("");

            entity.HasOne(d => d.FormClass).WithMany(p => p.CmsAlternativeFormFormClasses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AlternativeForm_FormClassID_CMS_Class");

            entity.HasOne(d => d.FormCoupledClass).WithMany(p => p.CmsAlternativeFormFormCoupledClasses).HasConstraintName("FK_CMS_AlternativeForm_FormCoupledClassID_CMS_Class");
        });

        modelBuilder.Entity<CmsApplicationPermission>(entity =>
        {
            entity.Property(e => e.ApplicationName).HasDefaultValue("");
            entity.Property(e => e.PermissionName).HasDefaultValue("");

            entity.HasOne(d => d.Role).WithMany(p => p.CmsApplicationPermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ApplicationPermission_RoleID_CMS_Role");
        });

        modelBuilder.Entity<CmsAutomationHistory>(entity =>
        {
            entity.Property(e => e.HistoryRejected).HasDefaultValue(false);
            entity.Property(e => e.HistoryStepDisplayName).HasDefaultValue("");

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
            entity.HasOne(d => d.StateStep).WithMany(p => p.CmsAutomationStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AutomationState_StateStepID");

            entity.HasOne(d => d.StateUser).WithMany(p => p.CmsAutomationStates).HasConstraintName("FK_CMS_AutomationState_StateUserID_CMS_User");

            entity.HasOne(d => d.StateWorkflow).WithMany(p => p.CmsAutomationStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_AutomationState_StateWorkflowID");
        });

        modelBuilder.Entity<CmsAutomationTemplate>(entity => entity.Property(e => e.TemplateDisplayName).HasDefaultValue(""));

        modelBuilder.Entity<CmsChannel>(entity =>
        {
            entity.Property(e => e.ChannelDisplayName).HasDefaultValue("");
            entity.Property(e => e.ChannelName).HasDefaultValue("");
            entity.Property(e => e.ChannelSize).HasDefaultValue("Standard");
            entity.Property(e => e.ChannelType).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsClass>(entity =>
        {
            entity.HasKey(e => e.ClassId).IsClustered(false);

            entity.HasIndex(e => new { e.ClassId, e.ClassName, e.ClassDisplayName }, "IX_CMS_Class_ClassID_ClassName_ClassDisplayName").IsClustered();

            entity.HasIndex(e => e.ClassShortName, "IX_CMS_Class_ClassShortName")
                .IsUnique()
                .HasFilter("([ClassShortName] IS NOT NULL)");

            entity.Property(e => e.ClassType).HasDefaultValue("Other");

            entity.HasOne(d => d.ClassResource).WithMany(p => p.CmsClasses).HasConstraintName("FK_CMS_Class_ClassResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsConsent>(entity =>
        {
            entity.Property(e => e.ConsentContent).HasDefaultValue("");
            entity.Property(e => e.ConsentDisplayName).HasDefaultValue("");
            entity.Property(e => e.ConsentHash).HasDefaultValue("");
            entity.Property(e => e.ConsentName).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsConsentAgreement>(entity =>
        {
            entity.HasOne(d => d.ConsentAgreementConsent).WithMany(p => p.CmsConsentAgreements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ConsentAgreement_ConsentAgreementConsentID_CMS_Consent");

            entity.HasOne(d => d.ConsentAgreementContact).WithMany(p => p.CmsConsentAgreements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ConsentAgreement_ConsentAgreementContactID_OM_Contact");
        });

        modelBuilder.Entity<CmsConsentArchive>(entity =>
        {
            entity.Property(e => e.ConsentArchiveContent).HasDefaultValue("");
            entity.Property(e => e.ConsentArchiveHash).HasDefaultValue("");

            entity.HasOne(d => d.ConsentArchiveConsent).WithMany(p => p.CmsConsentArchives)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ConsentArchive_ConsentArchiveConsentID_CMS_Consent");
        });

        modelBuilder.Entity<CmsContentFolder>(entity =>
        {
            entity.Property(e => e.ContentFolderDisplayName).HasDefaultValue("");
            entity.Property(e => e.ContentFolderName).HasDefaultValue("");

            entity.HasOne(d => d.ContentFolderCreatedByUser).WithMany(p => p.CmsContentFolderContentFolderCreatedByUsers).HasConstraintName("FK_CMS_ContentFolder_ContentFolderCreatedByUserID_CMS_User");

            entity.HasOne(d => d.ContentFolderModifiedByUser).WithMany(p => p.CmsContentFolderContentFolderModifiedByUsers).HasConstraintName("FK_CMS_ContentFolder_ContentFolderModifiedByUserID_CMS_User");

            entity.HasOne(d => d.ContentFolderParentFolder).WithMany(p => p.InverseContentFolderParentFolder).HasConstraintName("FK_CMS_ContentFolder_ContentFolderParentFolderID_CMS_ContentFolder");
        });

        modelBuilder.Entity<CmsContentItem>(entity =>
        {
            entity.HasIndex(e => e.ContentItemChannelId, "IX_CMS_ContentItem_ContentItemChannelID").HasFilter("([ContentItemChannelID] IS NOT NULL)");

            entity.Property(e => e.ContentItemName).HasDefaultValue("");

            entity.HasOne(d => d.ContentItemChannel).WithMany(p => p.CmsContentItems).HasConstraintName("FK_CMS_ContentItem_ContentItemChannelID");

            entity.HasOne(d => d.ContentItemContentFolder).WithMany(p => p.CmsContentItems).HasConstraintName("FK_CMS_ContentItem_ContentItemContentFolderID_CMS_ContentFolder");

            entity.HasOne(d => d.ContentItemContentType).WithMany(p => p.CmsContentItems).HasConstraintName("FK_CMS_ContentItem_ContentItemContentTypeID_CMS_Class");
        });

        modelBuilder.Entity<CmsContentItemCommonDatum>(entity =>
        {
            entity.HasOne(d => d.ContentItemCommonDataContentItem).WithMany(p => p.CmsContentItemCommonData)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentItemCommonData_ContentItemCommonDataContentItemID_CMS_ContentItem");

            entity.HasOne(d => d.ContentItemCommonDataContentLanguage).WithMany(p => p.CmsContentItemCommonData)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentItemCommonData_ContentItemCommonDataContentLanguageID_CMS_ContentLanguage");
        });

        modelBuilder.Entity<CmsContentItemLanguageMetadatum>(entity =>
        {
            entity.Property(e => e.ContentItemLanguageMetadataDisplayName).HasDefaultValue("");

            entity.HasOne(d => d.ContentItemLanguageMetadataContentItem).WithMany(p => p.CmsContentItemLanguageMetadata)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataContentItemID_CMS_ContentItem");

            entity.HasOne(d => d.ContentItemLanguageMetadataContentLanguage).WithMany(p => p.CmsContentItemLanguageMetadata)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataContentLanguageID_CMS_ContentLanguage");

            entity.HasOne(d => d.ContentItemLanguageMetadataContentWorkflowStep).WithMany(p => p.CmsContentItemLanguageMetadata)
                .HasConstraintName("FK_CMS_ContentItemLanguageMetadata_ContentItemLanguageMetadataContentWorkflowStepID_CMS_ContentWorkflowStep");

            entity.HasOne(d => d.ContentItemLanguageMetadataCreatedByUser).WithMany(p => p.CmsContentItemLanguageMetadatumContentItemLanguageMetadataCreatedByUsers)
                .HasConstraintName("FK_CMS_ContentItemLanguageMetadata_CMS_ContentItemLanguageMetadataCreatedByUserID_CMS_User");

            entity.HasOne(d => d.ContentItemLanguageMetadataModifiedByUser).WithMany(p => p.CmsContentItemLanguageMetadatumContentItemLanguageMetadataModifiedByUsers)
                .HasConstraintName("FK_CMS_ContentItemLanguageMetadata_CMS_ContentItemLanguageMetadataModifiedByUserID_CMS_User");
        });

        modelBuilder.Entity<CmsContentItemReference>(entity =>
        {
            entity.HasOne(d => d.ContentItemReferenceSourceCommonData).WithMany(p => p.CmsContentItemReferences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentItemReference_ContentItemReferenceSourceCommonDataID_CMS_ContentItemCommonData");

            entity.HasOne(d => d.ContentItemReferenceTargetItem).WithMany(p => p.CmsContentItemReferences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentItemReference_ContentItemReferenceTargetItemID_CMS_ContentItem");
        });

        modelBuilder.Entity<CmsContentItemTag>(entity => entity.HasOne(d => d.ContentItemTagContentItemLanguageMetadata).WithMany(p => p.CmsContentItemTags)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CMS_ContentItemTag_ContentItemTagContentItemLanguageMetadataID_CMS_ContentItemLanguageMetadata"));

        modelBuilder.Entity<CmsContentLanguage>(entity =>
        {
            entity.Property(e => e.ContentLanguageCultureFormat).HasDefaultValue("");
            entity.Property(e => e.ContentLanguageDisplayName).HasDefaultValue("");
            entity.Property(e => e.ContentLanguageName).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsContentTypeChannel>(entity =>
        {
            entity.HasOne(d => d.ContentTypeChannelChannel).WithMany(p => p.CmsContentTypeChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentTypeChannel_ContentTypeChannelChannelID_CMS_Channel");

            entity.HasOne(d => d.ContentTypeChannelContentType).WithMany(p => p.CmsContentTypeChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentTypeChannel_ContentTypeChannelConentTypeID_CMS_Class");
        });

        modelBuilder.Entity<CmsContentWorkflow>(entity =>
        {
            entity.Property(e => e.ContentWorkflowDisplayName).HasDefaultValue("");
            entity.Property(e => e.ContentWorkflowName).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsContentWorkflowContentType>(entity =>
        {
            entity.HasOne(d => d.ContentWorkflowContentTypeContentType).WithMany(p => p.CmsContentWorkflowContentTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentWorkflowContentType_ContentWorkflowContentTypeContentTypeID_CMS_Class");

            entity.HasOne(d => d.ContentWorkflowContentTypeContentWorkflow).WithMany(p => p.CmsContentWorkflowContentTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentWorkflowContentType_ContentWorkflowContentTypeContentWorkflowID_CMS_ContentWorkflow");
        });

        modelBuilder.Entity<CmsContentWorkflowStep>(entity =>
        {
            entity.Property(e => e.ContentWorkflowStepDisplayName).HasDefaultValue("");
            entity.Property(e => e.ContentWorkflowStepIconClass).HasDefaultValue("");
            entity.Property(e => e.ContentWorkflowStepName).HasDefaultValue("");

            entity.HasOne(d => d.ContentWorkflowStepWorkflow).WithMany(p => p.CmsContentWorkflowSteps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentWorkflowStep_ContentWorkflowStepWorkflowID_CMS_ContentWorkflow");
        });

        modelBuilder.Entity<CmsContentWorkflowStepRole>(entity =>
        {
            entity.HasOne(d => d.ContentWorkflowStepRoleContentWorkflowStep).WithMany(p => p.CmsContentWorkflowStepRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentWorkflowStepRole_ContentWorkflowStepRoleContentWorkflowStepID_CMS_ContentWorkflowStep");

            entity.HasOne(d => d.ContentWorkflowStepRoleRole).WithMany(p => p.CmsContentWorkflowStepRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ContentWorkflowStepRole_ContentWorkflowStepRoleRoleID_CMS_Role");
        });

        modelBuilder.Entity<CmsCountry>(entity =>
        {
            entity.HasKey(e => e.CountryId).IsClustered(false);

            entity.HasIndex(e => e.CountryDisplayName, "IX_CMS_Country_CountryDisplayName").IsClustered();

            entity.Property(e => e.CountryDisplayName).HasDefaultValue("");
            entity.Property(e => e.CountryLastModified).HasDefaultValueSql("('11/14/2013 1:43:04 PM')");
            entity.Property(e => e.CountryName).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsCulture>(entity =>
        {
            entity.HasKey(e => e.CultureId).IsClustered(false);

            entity.HasIndex(e => e.CultureName, "IX_CMS_Culture_CultureName").IsClustered();

            entity.Property(e => e.CultureIsUiculture).HasDefaultValue(false);
        });

        modelBuilder.Entity<CmsEmail>(entity =>
        {
            entity.Property(e => e.EmailFrom).HasDefaultValue("");
            entity.Property(e => e.EmailSubject).HasDefaultValue("");

            entity.HasOne(d => d.EmailEmailConfiguration).WithMany(p => p.CmsEmails).HasConstraintName("FK_CMS_Email_EmailEmailConfigurationID_EmailLibrary_EmailConfiguration");

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

        modelBuilder.Entity<CmsEventLog>(entity =>
        {
            entity.Property(e => e.EventCode).HasDefaultValue("");
            entity.Property(e => e.EventMachineName).HasDefaultValue("");
            entity.Property(e => e.EventTime).HasDefaultValueSql("('4/21/2015 8:21:43 AM')");
            entity.Property(e => e.EventType).HasDefaultValue("");
            entity.Property(e => e.EventUrl).HasDefaultValue("");
            entity.Property(e => e.EventUrlReferrer).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.Source).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsExternalLogin>(entity =>
        {
            entity.Property(e => e.IdentityKey).HasDefaultValue("");
            entity.Property(e => e.LoginProvider).HasDefaultValue("");

            entity.HasOne(d => d.User).WithMany(p => p.CmsExternalLogins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ExternalLogin_UserID_CMS_User");
        });

        modelBuilder.Entity<CmsForm>(entity =>
        {
            entity.HasKey(e => e.FormId).IsClustered(false);

            entity.HasIndex(e => e.FormDisplayName, "IX_CMS_Form_FormDisplayName").IsClustered();

            entity.Property(e => e.FormDisplayName).HasDefaultValue("");
            entity.Property(e => e.FormLastModified).HasDefaultValueSql("('9/17/2012 1:37:08 PM')");
            entity.Property(e => e.FormLogActivity).HasDefaultValue(true);
            entity.Property(e => e.FormName).HasDefaultValue("");
            entity.Property(e => e.FormSubmitButtonText).HasDefaultValue("");

            entity.HasOne(d => d.FormClass).WithMany(p => p.CmsForms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Form_FormClassID_CMS_Class");

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

        modelBuilder.Entity<CmsFormFeaturedField>(entity =>
        {
            entity.Property(e => e.FormFeaturedFieldEnabled).HasDefaultValue(true);
            entity.Property(e => e.FormFeaturedFieldMapping).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsHeadlessChannel>(entity =>
        {
            entity.HasOne(d => d.HeadlessChannelChannel).WithMany(p => p.CmsHeadlessChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_HeadlessChannel_HeadlessChannelChannelID_CMS_Channel");

            entity.HasOne(d => d.HeadlessChannelPrimaryContentLanguage).WithMany(p => p.CmsHeadlessChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_HeadlessChannel_HeadlessChannelPrimaryContentLanguageID_CMS_ContentLanguage");
        });

        modelBuilder.Entity<CmsHeadlessItem>(entity =>
        {
            entity.Property(e => e.HeadlessItemName).HasDefaultValue("");

            entity.HasOne(d => d.HeadlessItemContentItem).WithMany(p => p.CmsHeadlessItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_HeadlessItem_HeadlessItemContentItemID_CMS_ContentItem");

            entity.HasOne(d => d.HeadlessItemHeadlessChannel).WithMany(p => p.CmsHeadlessItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_HeadlessItem_HeadlessItemHeadlessChannelID_CMS_HeadlessChannel");
        });

        modelBuilder.Entity<CmsHeadlessToken>(entity =>
        {
            entity.Property(e => e.HeadlessTokenAccessType).HasDefaultValue("Published");
            entity.Property(e => e.HeadlessTokenDisplayName).HasDefaultValue("");
            entity.Property(e => e.HeadlessTokenEnabled).HasDefaultValue(true);
            entity.Property(e => e.HeadlessTokenHash).HasDefaultValue("");

            entity.HasOne(d => d.HeadlessTokenCreatedByUser).WithMany(p => p.CmsHeadlessTokenHeadlessTokenCreatedByUsers).HasConstraintName("FK_CMS_HeadlessToken_HeadlessTokenCreatedByUserID_CMS_User");

            entity.HasOne(d => d.HeadlessTokenHeadlessChannel).WithMany(p => p.CmsHeadlessTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_HeadlessToken_HeadlessTokenHeadlessChannelID_CMS_HeadlessChannel");

            entity.HasOne(d => d.HeadlessTokenModifiedByUser).WithMany(p => p.CmsHeadlessTokenHeadlessTokenModifiedByUsers).HasConstraintName("FK_CMS_HeadlessToken_HeadlessTokenModifiedByUserID_CMS_User");
        });

        modelBuilder.Entity<CmsLicenseKey>(entity =>
        {
            entity.HasKey(e => e.LicenseKeyId).IsClustered(false);

            entity.HasIndex(e => e.LicenseDomain, "IX_CMS_LicenseKey_LicenseDomain").IsClustered();
        });

        modelBuilder.Entity<CmsMacroIdentity>(entity =>
        {
            entity.Property(e => e.MacroIdentityName).HasDefaultValue("");

            entity.HasOne(d => d.MacroIdentityEffectiveUser).WithMany(p => p.CmsMacroIdentities).HasConstraintName("FK_CMS_MacroIdentity_MacroIdentityEffectiveUserID_CMS_User");
        });

        modelBuilder.Entity<CmsMacroRule>(entity =>
        {
            entity.Property(e => e.MacroRuleCondition).HasDefaultValue("");
            entity.Property(e => e.MacroRuleDisplayName).HasDefaultValue("");
            entity.Property(e => e.MacroRuleEnabled).HasDefaultValue(true);
            entity.Property(e => e.MacroRuleIsCustom).HasDefaultValue(false);
            entity.Property(e => e.MacroRuleLastModified).HasDefaultValue(new DateTime(2012, 1, 5, 8, 46, 33, 0, DateTimeKind.Unspecified));
        });

        modelBuilder.Entity<CmsMacroRuleMacroRuleCategory>(entity =>
        {
            entity.HasOne(d => d.MacroRuleCategory).WithMany(p => p.CmsMacroRuleMacroRuleCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_MacroRuleMacroRuleCategory_MacroRuleCategoryID_CMS_MacroRuleMacroRuleCategory");

            entity.HasOne(d => d.MacroRule).WithMany(p => p.CmsMacroRuleMacroRuleCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_MacroRuleMacroRuleCategory_MacroRuleID_CMS_MacroRule");
        });

        modelBuilder.Entity<CmsMember>(entity =>
        {
            entity.HasIndex(e => e.MemberEmail, "IX_CMS_Member_MemberEmail")
                .IsUnique()
                .HasFilter("([MemberEmail] IS NOT NULL AND [MemberEmail]<>'')");

            entity.HasIndex(e => e.MemberName, "IX_CMS_Member_MemberName")
                .IsUnique()
                .HasFilter("([MemberName] IS NOT NULL AND [MemberName]<>'')");
        });

        modelBuilder.Entity<CmsMemberExternalLogin>(entity =>
        {
            entity.Property(e => e.MemberExternalLoginIdentityKey).HasDefaultValue("");
            entity.Property(e => e.MemberExternalLoginLoginProvider).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsObjectWorkflowTrigger>(entity =>
        {
            entity.Property(e => e.TriggerDisplayName).HasDefaultValue("");
            entity.Property(e => e.TriggerObjectType).HasDefaultValue("");

            entity.HasOne(d => d.TriggerWorkflow).WithMany(p => p.CmsObjectWorkflowTriggers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_ObjectWorkflowTrigger_TriggerWorkflowID");
        });

        modelBuilder.Entity<CmsPageTemplateConfiguration>(entity =>
        {
            entity.Property(e => e.PageTemplateConfigurationIcon).HasDefaultValue("xp-layout");
            entity.Property(e => e.PageTemplateConfigurationName).HasDefaultValue("");
            entity.Property(e => e.PageTemplateConfigurationTemplate).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsQuery>(entity =>
        {
            entity.Property(e => e.QueryIsCustom).HasDefaultValue(false);
            entity.Property(e => e.QueryName).HasDefaultValue("");

            entity.HasOne(d => d.Class).WithMany(p => p.CmsQueries).HasConstraintName("FK_CMS_Query_ClassID_CMS_Class");
        });

        modelBuilder.Entity<CmsResource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).IsClustered(false);

            entity.HasIndex(e => e.ResourceDisplayName, "IX_CMS_Resource_ResourceDisplayName").IsClustered();
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

        modelBuilder.Entity<CmsScheduledTask>(entity =>
        {
            entity.Property(e => e.TaskExecutingServerName).HasDefaultValue("");
            entity.Property(e => e.TaskInterval).HasDefaultValue("");

            entity.HasOne(d => d.TaskUser).WithMany(p => p.CmsScheduledTasks).HasConstraintName("FK_CMS_ScheduledTask_TaskUserID_CMS_User");
        });

        modelBuilder.Entity<CmsSettingsCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).IsClustered(false);

            entity.HasIndex(e => e.CategoryOrder, "IX_CMS_SettingsCategory_CategoryOrder").IsClustered();

            entity.Property(e => e.CategoryDisplayName).HasDefaultValue("");
            entity.Property(e => e.CategoryIsCustom).HasDefaultValue(false);
            entity.Property(e => e.CategoryIsGroup).HasDefaultValue(false);

            entity.HasOne(d => d.CategoryParent).WithMany(p => p.InverseCategoryParent).HasConstraintName("FK_CMS_SettingsCategory_CMS_SettingsCategory1");

            entity.HasOne(d => d.CategoryResource).WithMany(p => p.CmsSettingsCategories).HasConstraintName("FK_CMS_SettingsCategory_CategoryResourceID_CMS_Resource");
        });

        modelBuilder.Entity<CmsSettingsKey>(entity =>
        {
            entity.Property(e => e.KeyDisplayName).HasDefaultValue("");
            entity.Property(e => e.KeyExplanationText).HasDefaultValue("");
            entity.Property(e => e.KeyIsCustom).HasDefaultValue(false);
            entity.Property(e => e.KeyIsHidden).HasDefaultValue(false);
            entity.Property(e => e.KeyName).HasDefaultValue("");
            entity.Property(e => e.KeyType).HasDefaultValue("");

            entity.HasOne(d => d.KeyCategory).WithMany(p => p.CmsSettingsKeys).HasConstraintName("FK_CMS_SettingsKey_KeyCategoryID_CMS_SettingsCategory");
        });

        modelBuilder.Entity<CmsSmartFolder>(entity =>
        {
            entity.Property(e => e.SmartFolderDisplayName).HasDefaultValue("");
            entity.Property(e => e.SmartFolderFilter).HasDefaultValue("");
            entity.Property(e => e.SmartFolderName).HasDefaultValue("");

            entity.HasOne(d => d.SmartFolderCreatedByUser).WithMany(p => p.CmsSmartFolderSmartFolderCreatedByUsers).HasConstraintName("FK_CMS_SmartFolder_SmartFolderCreatedByUserID_CMS_User");

            entity.HasOne(d => d.SmartFolderModifiedByUser).WithMany(p => p.CmsSmartFolderSmartFolderModifiedByUsers).HasConstraintName("FK_CMS_SmartFolder_SmartFolderModifiedByUserID_CMS_User");
        });

        modelBuilder.Entity<CmsState>(entity =>
        {
            entity.HasKey(e => e.StateId).IsClustered(false);

            entity.HasIndex(e => e.StateDisplayName, "IX_CMS_State_CountryID_StateDisplayName").IsClustered();

            entity.Property(e => e.StateDisplayName).HasDefaultValue("");
            entity.Property(e => e.StateName).HasDefaultValue("");

            entity.HasOne(d => d.Country).WithMany(p => p.CmsStates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_State_CountryID_CMS_Country");
        });

        modelBuilder.Entity<CmsTag>(entity =>
        {
            entity.Property(e => e.TagName).HasDefaultValue("");
            entity.Property(e => e.TagTitle).HasDefaultValue("");

            entity.HasOne(d => d.TagParent).WithMany(p => p.InverseTagParent).HasConstraintName("FK_CMS_Tag_TagParentID_CMS_Tag");

            entity.HasOne(d => d.TagTaxonomy).WithMany(p => p.CmsTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_Tag_TagTaxonomyID_CMS_Taxonomy");
        });

        modelBuilder.Entity<CmsTaxonomy>(entity =>
        {
            entity.Property(e => e.TaxonomyName).HasDefaultValue("");
            entity.Property(e => e.TaxonomyTitle).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsUser>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_CMS_User_Email")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL AND [Email]<>'')");

            entity.Property(e => e.UserName).HasDefaultValue("");
            entity.Property(e => e.UserPassword).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsUserMacroIdentity>(entity =>
        {
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

        modelBuilder.Entity<CmsWebFarmServer>(entity =>
        {
            entity.Property(e => e.ServerDisplayName).HasDefaultValue("");
            entity.Property(e => e.ServerLastModified).HasDefaultValueSql("('9/17/2013 12:18:06 PM')");
            entity.Property(e => e.ServerName).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsWebFarmServerLog>(entity => entity.Property(e => e.LogCode).HasDefaultValue(""));

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
            entity.Property(e => e.TaskGuid).HasDefaultValue(new Guid("00000000-0000-0000-0000-000000000000"));
            entity.Property(e => e.TaskIsMemory).HasDefaultValue(false);
            entity.Property(e => e.TaskType).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsWebPageFormerUrlPath>(entity =>
        {
            entity.Property(e => e.WebPageFormerUrlPath).HasDefaultValue("");
            entity.Property(e => e.WebPageFormerUrlPathHash).HasDefaultValue("");

            entity.HasOne(d => d.WebPageFormerUrlPathContentLanguage).WithMany(p => p.CmsWebPageFormerUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageFormerUrlPath_WebPageFormerUrlPathContentLanguageID_CMS_ContentLanguage");

            entity.HasOne(d => d.WebPageFormerUrlPathWebPageItem).WithMany(p => p.CmsWebPageFormerUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageFormerUrlPath_WebPageFormerUrlPathWebPageItemID_CMS_WebPageItem");

            entity.HasOne(d => d.WebPageFormerUrlPathWebsiteChannel).WithMany(p => p.CmsWebPageFormerUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageFormerUrlPath_WebPageFormerUrlPathWebsiteChannelID_CMS_WebsiteChannel");
        });

        modelBuilder.Entity<CmsWebPageItem>(entity =>
        {
            entity.Property(e => e.WebPageItemName).HasDefaultValue("");
            entity.Property(e => e.WebPageItemTreePath).HasDefaultValue("");

            entity.HasOne(d => d.WebPageItemContentItem).WithMany(p => p.CmsWebPageItems).HasConstraintName("FK_CMS_WebPageItem_WebPageItemContentItemID_CMS_ContentItem");

            entity.HasOne(d => d.WebPageItemParent).WithMany(p => p.InverseWebPageItemParent).HasConstraintName("FK_CMS_WebPageItem_WebPageItemParentID_CMS_WebPageItem");

            entity.HasOne(d => d.WebPageItemWebsiteChannel).WithMany(p => p.CmsWebPageItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageItem_WebPageItemWebsiteChannelID_CMS_WebsiteChannel");
        });

        modelBuilder.Entity<CmsWebPageUrlPath>(entity =>
        {
            entity.Property(e => e.WebPageUrlPath).HasDefaultValue("");
            entity.Property(e => e.WebPageUrlPathHash).HasDefaultValue("");
            entity.Property(e => e.WebPageUrlPathIsLatest).HasDefaultValue(true);

            entity.HasOne(d => d.WebPageUrlPathContentLanguage).WithMany(p => p.CmsWebPageUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageUrlPath_WebPageUrlPathContentLanguageID_CMS_ContentLanguage");

            entity.HasOne(d => d.WebPageUrlPathWebPageItem).WithMany(p => p.CmsWebPageUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageUrlPath_WebPageUrlPathWebPageItemID_CMS_WebPageItem");

            entity.HasOne(d => d.WebPageUrlPathWebsiteChannel).WithMany(p => p.CmsWebPageUrlPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebPageUrlPath_WebPageUrlPathWebsiteChannelID_CMS_WebsiteChannel");
        });

        modelBuilder.Entity<CmsWebsiteCaptchaSetting>(entity =>
        {
            entity.Property(e => e.WebsiteCaptchaSettingsReCaptchaSecretKey).HasDefaultValue("");
            entity.Property(e => e.WebsiteCaptchaSettingsReCaptchaSiteKey).HasDefaultValue("");
        });

        modelBuilder.Entity<CmsWebsiteChannel>(entity =>
        {
            entity.Property(e => e.WebsiteChannelDefaultCookieLevel).HasDefaultValue(1000);
            entity.Property(e => e.WebsiteChannelDomain).HasDefaultValue("");
            entity.Property(e => e.WebsiteChannelStoreFormerUrls).HasDefaultValue(true);

            entity.HasOne(d => d.WebsiteChannelChannel).WithMany(p => p.CmsWebsiteChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebsiteChannel_WebsiteChannelChannelID_CMS_Channel");

            entity.HasOne(d => d.WebsiteChannelPrimaryContentLanguage).WithMany(p => p.CmsWebsiteChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WebsiteChannel_WebsiteChannelPrimaryContentLanguageID_CMS_ContentLanguage");
        });

        modelBuilder.Entity<CmsWorkflow>(entity =>
        {
            entity.HasKey(e => e.WorkflowId).IsClustered(false);

            entity.HasIndex(e => e.WorkflowDisplayName, "IX_CMS_Workflow_WorkflowDisplayName").IsClustered();

            entity.Property(e => e.WorkflowAutoPublishChanges).HasDefaultValue(false);
            entity.Property(e => e.WorkflowDisplayName).HasDefaultValue("");
            entity.Property(e => e.WorkflowEnabled).HasDefaultValue(true);
            entity.Property(e => e.WorkflowName).HasDefaultValue("");
            entity.Property(e => e.WorkflowSendApproveEmails).HasDefaultValue(true);
            entity.Property(e => e.WorkflowSendArchiveEmails).HasDefaultValue(true);
            entity.Property(e => e.WorkflowSendPublishEmails).HasDefaultValue(true);
            entity.Property(e => e.WorkflowSendReadyForApprovalEmails).HasDefaultValue(true);
            entity.Property(e => e.WorkflowSendRejectEmails).HasDefaultValue(true);
            entity.Property(e => e.WorkflowUseCheckinCheckout).HasDefaultValue(false);
        });

        modelBuilder.Entity<CmsWorkflowAction>(entity =>
        {
            entity.Property(e => e.ActionEnabled).HasDefaultValue(true);

            entity.HasOne(d => d.ActionResource).WithMany(p => p.CmsWorkflowActions).HasConstraintName("FK_CMS_WorkflowAction_ActionResourceID");
        });

        modelBuilder.Entity<CmsWorkflowStep>(entity =>
        {
            entity.Property(e => e.StepAllowPublish).HasDefaultValue(false);
            entity.Property(e => e.StepAllowReject).HasDefaultValue(true);

            entity.HasOne(d => d.StepAction).WithMany(p => p.CmsWorkflowSteps).HasConstraintName("FK_CMS_WorkflowStep_StepActionID");

            entity.HasOne(d => d.StepWorkflow).WithMany(p => p.CmsWorkflowSteps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CMS_WorkflowStep_StepWorkflowID");
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

        modelBuilder.Entity<EmailLibraryEmailChannel>(entity =>
        {
            entity.Property(e => e.EmailChannelSendingDomain).HasDefaultValue("");
            entity.Property(e => e.EmailChannelServiceDomain).HasDefaultValue("");

            entity.HasOne(d => d.EmailChannelChannel).WithMany(p => p.EmailLibraryEmailChannels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailChannel_EmailChannelChannelID_CMS_Channel");
        });

        modelBuilder.Entity<EmailLibraryEmailChannelSender>(entity =>
        {
            entity.Property(e => e.EmailChannelSenderDisplayName).HasDefaultValue("");
            entity.Property(e => e.EmailChannelSenderName).HasDefaultValue("");

            entity.HasOne(d => d.EmailChannelSenderEmailChannel).WithMany(p => p.EmailLibraryEmailChannelSenders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailChannelSender_EmailChannelSenderEmailChannelID_EmailLibrary_EmailChannel");
        });

        modelBuilder.Entity<EmailLibraryEmailConfiguration>(entity =>
        {
            entity.Property(e => e.EmailConfigurationName).HasDefaultValue("");
            entity.Property(e => e.EmailConfigurationPurpose).HasDefaultValue("Regular");

            entity.HasOne(d => d.EmailConfigurationContentItem).WithMany(p => p.EmailLibraryEmailConfigurations).HasConstraintName("FK_EmailLibrary_EmailConfiguration_EmailConfigurationContentItemID_CMS_ContentItem");

            entity.HasOne(d => d.EmailConfigurationEmailChannel).WithMany(p => p.EmailLibraryEmailConfigurations).HasConstraintName("FK_EmailLibrary_EmailConfiguration_EmailConfigurationEmailChannelID_EmailLibrary_EmailChannel");
        });

        modelBuilder.Entity<EmailLibraryEmailLink>(entity =>
        {
            entity.Property(e => e.EmailLinkDescription).HasDefaultValue("");
            entity.Property(e => e.EmailLinkTarget).HasDefaultValue("");

            entity.HasOne(d => d.EmailLinkEmailConfiguration).WithMany(p => p.EmailLibraryEmailLinks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailLink_EmailLinkEmailConfigurationID_EmailLibrary_EmailConfiguration");
        });

        modelBuilder.Entity<EmailLibraryEmailMarketingRecipient>(entity =>
        {
            entity.Property(e => e.EmailMarketingRecipientContactEmail).HasDefaultValue("");

            entity.HasOne(d => d.EmailMarketingRecipientContact).WithMany(p => p.EmailLibraryEmailMarketingRecipients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailMarketingRecipient_EmailMarketingRecipientContactID_OM_Contact");

            entity.HasOne(d => d.EmailMarketingRecipientEmailConfiguration).WithMany(p => p.EmailLibraryEmailMarketingRecipients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailMarketingRecipient_EmailMarketingRecipientEmailConfigurationID_EmailLibrary_EmailConfiguration");
        });

        modelBuilder.Entity<EmailLibraryEmailStatistic>(entity => entity.HasOne(d => d.EmailStatisticsEmailConfiguration).WithMany(p => p.EmailLibraryEmailStatistics)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmailLibrary_EmailStatistics_EmailStatisticsEmailConfigurationID_EmailLibrary_EmailConfiguration"));

        modelBuilder.Entity<EmailLibraryEmailStatisticsHit>(entity =>
        {
            entity.HasOne(d => d.EmailStatisticsHitsEmailConfiguration).WithMany(p => p.EmailLibraryEmailStatisticsHits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailStatisticsHits_EmailStatisticsHitsEmailConfigurationID_EmailLibrary_EmailConfiguration");

            entity.HasOne(d => d.EmailStatisticsHitsEmailLink).WithMany(p => p.EmailLibraryEmailStatisticsHits).HasConstraintName("FK_EmailLibrary_EmailStatisticsHits_EmailStatisticsHitsEmailLinkID_EmailLibrary_EmailLink");
        });

        modelBuilder.Entity<EmailLibraryEmailSubscriptionConfirmation>(entity =>
        {
            entity.HasOne(d => d.EmailSubscriptionConfirmationContact).WithMany(p => p.EmailLibraryEmailSubscriptionConfirmations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailSubscriptionConfirmation_EmailSubscriptionConfirmationContactID_OM_Contact");

            entity.HasOne(d => d.EmailSubscriptionConfirmationRecipientList).WithMany(p => p.EmailLibraryEmailSubscriptionConfirmations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailSubscriptionConfirmation_EmailSubscriptionConfirmationRecipientListID_OM_ContactGroup");
        });

        modelBuilder.Entity<EmailLibraryEmailTemplate>(entity =>
        {
            entity.Property(e => e.EmailTemplateCode).HasDefaultValue("");
            entity.Property(e => e.EmailTemplateDisplayName).HasDefaultValue("");
            entity.Property(e => e.EmailTemplateName).HasDefaultValue("");
        });

        modelBuilder.Entity<EmailLibraryEmailTemplateContentType>(entity =>
        {
            entity.HasOne(d => d.EmailTemplateContentTypeContentType).WithMany(p => p.EmailLibraryEmailTemplateContentTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailTemplateContentType_EmailTemplateContentTypeContentTypeID_CMS_Class");

            entity.HasOne(d => d.EmailTemplateContentTypeEmailTemplate).WithMany(p => p.EmailLibraryEmailTemplateContentTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_EmailTemplateContentType_EmailTemplateContentTypeEmailTemplateID_EmailLibrary_EmailTemplate");
        });

        modelBuilder.Entity<EmailLibraryRecipientListSetting>(entity => entity.HasOne(d => d.RecipientListSettingsRecipientList).WithMany(p => p.EmailLibraryRecipientListSettings)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmailLibrary_RecipientListSettings_RecipientListSettingsRecipientListID_OM_ContactGroup"));

        modelBuilder.Entity<EmailLibrarySendConfiguration>(entity =>
        {
            entity.HasOne(d => d.SendConfigurationEmailConfiguration).WithOne(p => p.EmailLibrarySendConfiguration)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_SendConfiguration_SendConfigurationEmailConfigurationID_EmailLibrary_EmailConfiguration");

            entity.HasOne(d => d.SendConfigurationRecipientList).WithMany(p => p.EmailLibrarySendConfigurations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailLibrary_SendConfiguration_SendConfigurationRecipientListID_OM_ContactGroup");
        });

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasKey(e => e.FileId).IsClustered(false);

            entity.HasIndex(e => e.FilePath, "IX_Media_File_FilePath").IsClustered();

            entity.Property(e => e.FileCreatedWhen).HasDefaultValue(new DateTime(2008, 11, 11, 16, 10, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.FileModifiedWhen).HasDefaultValue(new DateTime(2008, 11, 11, 16, 11, 15, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.FileTitle).HasDefaultValue("");

            entity.HasOne(d => d.FileCreatedByUser).WithMany(p => p.MediaFileFileCreatedByUsers).HasConstraintName("FK_Media_File_FileCreatedByUserID_CMS_User");

            entity.HasOne(d => d.FileLibrary).WithMany(p => p.MediaFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_File_FileLibraryID_Media_Library");

            entity.HasOne(d => d.FileModifiedByUser).WithMany(p => p.MediaFileFileModifiedByUsers).HasConstraintName("FK_Media_File_FileModifiedByUserID_CMS_User");
        });

        modelBuilder.Entity<MediaLibrary>(entity =>
        {
            entity.HasKey(e => e.LibraryId).IsClustered(false);

            entity.HasIndex(e => e.LibraryDisplayName, "IX_Media_Library_LibraryDisplayName").IsClustered();

            entity.Property(e => e.LibraryName).HasDefaultValue("");
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
            entity.HasOne(d => d.ActivityChannel).WithMany(p => p.OmActivities).HasConstraintName("FK_OM_Activity_ActivityChannelID_CMS_Channel");

            entity.HasOne(d => d.ActivityLanguage).WithMany(p => p.OmActivities).HasConstraintName("FK_OM_Activity_ActivityLanguageID_CMS_ContentLanguage");
        });

        modelBuilder.Entity<OmActivityType>(entity =>
        {
            entity.Property(e => e.ActivityTypeEnabled).HasDefaultValue(true);
            entity.Property(e => e.ActivityTypeIsCustom).HasDefaultValue(true);
        });

        modelBuilder.Entity<OmContact>(entity =>
        {
            entity.Property(e => e.ContactCreated).HasDefaultValue(new DateTime(2011, 3, 5, 10, 51, 13, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.ContactMonitored).HasDefaultValue(false);
            entity.Property(e => e.ContactSalesForceLeadReplicationDisabled).HasDefaultValue(false);
            entity.Property(e => e.ContactSalesForceLeadReplicationRequired).HasDefaultValue(false);

            entity.HasOne(d => d.ContactCountry).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_CMS_Country");

            entity.HasOne(d => d.ContactOwnerUser).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_CMS_User");

            entity.HasOne(d => d.ContactState).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_CMS_State");

            entity.HasOne(d => d.ContactStatus).WithMany(p => p.OmContacts).HasConstraintName("FK_OM_Contact_OM_ContactStatus");
        });

        modelBuilder.Entity<OmContactGroup>(entity =>
        {
            entity.HasKey(e => e.ContactGroupId).HasName("PK_CMS_ContactGroup");

            entity.Property(e => e.ContactGroupIsRecipientList).HasDefaultValue(false);
            entity.Property(e => e.ContactGroupName).HasDefaultValue("");
        });

        modelBuilder.Entity<OmContactGroupMember>(entity =>
        {
            entity.Property(e => e.ContactGroupMemberFromCondition).HasDefaultValue(false);
            entity.Property(e => e.ContactGroupMemberFromManual).HasDefaultValue(false);

            entity.HasOne(d => d.ContactGroupMemberContactGroup).WithMany(p => p.OmContactGroupMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OM_ContactGroupMembers_OM_ContactGroup");
        });

        modelBuilder.Entity<OmContactRole>(entity =>
        {
            entity.Property(e => e.ContactRoleDisplayName).HasDefaultValue("");
            entity.Property(e => e.ContactRoleName).HasDefaultValue("");
        });

        modelBuilder.Entity<OmContactStatus>(entity =>
        {
            entity.Property(e => e.ContactStatusDisplayName).HasDefaultValue("");
            entity.Property(e => e.ContactStatusName).HasDefaultValue("");
        });

        modelBuilder.Entity<OmTrackedWebsite>(entity =>
        {
            entity.Property(e => e.TrackedWebsiteDisplayName).HasDefaultValue("");
            entity.Property(e => e.TrackedWebsiteEnabled).HasDefaultValue(false);
            entity.Property(e => e.TrackedWebsiteName).HasDefaultValue("");
            entity.Property(e => e.TrackedWebsiteUrl).HasDefaultValue("");
        });

        modelBuilder.Entity<OmVisitorToContact>(entity => entity.HasOne(d => d.VisitorToContactContact).WithMany(p => p.OmVisitorToContacts)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OM_VisitorToContact_OM_Contact_Cascade"));

        modelBuilder.Entity<TempFile>(entity =>
        {
            entity.Property(e => e.FileDirectory).HasDefaultValue("");
            entity.Property(e => e.FileExtension).HasDefaultValue("");
            entity.Property(e => e.FileLastModified).HasDefaultValueSql("('6/29/2010 1:57:54 PM')");
            entity.Property(e => e.FileMimeType).HasDefaultValue("");
            entity.Property(e => e.FileName).HasDefaultValue("");
        });

        modelBuilder.Entity<ViewCmsResourceStringJoined>(entity => entity.ToView("View_CMS_ResourceString_Joined"));

        modelBuilder.Entity<ViewCmsResourceTranslatedJoined>(entity => entity.ToView("View_CMS_ResourceTranslated_Joined"));

        modelBuilder.Entity<ViewOmAccountContactAccountJoined>(entity => entity.ToView("View_OM_AccountContact_AccountJoined"));

        modelBuilder.Entity<ViewOmAccountContactContactJoined>(entity => entity.ToView("View_OM_AccountContact_ContactJoined"));

        modelBuilder.Entity<ViewOmAccountJoined>(entity => entity.ToView("View_OM_Account_Joined"));

        modelBuilder.Entity<ViewOmContactGroupMemberAccountJoined>(entity => entity.ToView("View_OM_ContactGroupMember_AccountJoined"));

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
