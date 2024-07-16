
using CMS.ContentEngine;
using CMS.DataEngine;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Models;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

namespace Migration.Toolkit.Source.Handlers;
public class MigratePageTypesCommandHandler(
    ILogger<MigratePageTypesCommandHandler> logger,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KxpClassFacade kxpClassFacade,
    IProtocol protocol,
    ToolkitConfiguration toolkitConfiguration,
    ModelFacade modelFacade,
    PageTemplateMigrator pageTemplateMigrator,
    ReusableSchemaService reusableSchemaService
)
    : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        using var ksClasses = EnumerableHelper.CreateDeferrableItemWrapper(
            modelFacade.Select<ICmsClass>("ClassIsDocumentType=1", "ClassID")
                .OrderBy(x => x.ClassID)
        );

        while (ksClasses.GetNext(out var di))
        {
            var (_, ksClass) = di;

            if (ksClass.ClassInheritsFromClassID is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<CmsClass>(c => c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (ksClasses.TryDeferItem(di))
                {
                    logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(ksClass), di.Recurrence);
                }
                else
                {
                    logger.LogErrorMissingDependency(ksClass, nameof(ksClass.ClassInheritsFromClassID), ksClass.ClassInheritsFromClassID, typeof(DataClassInfo));
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            protocol.FetchedSource(ksClass);

            if (entityConfiguration.ExcludeCodeNames.Contains(ksClass.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(ksClass.ClassName, "PageType"), ksClass);
                logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", ksClass.ClassName);
                continue;
            }

            if (string.Equals(ksClass.ClassName, CLASS_CMS_ROOT, StringComparison.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, ksClass);
                logger.LogInformation("CmsClass: {ClassName} was skipped => CMS.Root cannot be migrated", ksClass.ClassName);
                continue;
            }

            if (string.Equals(ksClass.ClassName, "cms.site", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            var kxoDataClass = kxpClassFacade.GetClass(ksClass.ClassGUID);
            protocol.FetchedTarget(kxoDataClass);

            if (SaveUsingKxoApi(ksClass, kxoDataClass) is { } targetClassId)
            {
                foreach (var cmsClassSite in modelFacade.SelectWhere<ICmsClassSite>("ClassID = @classId", new SqlParameter("classId", ksClass.ClassID)))
                {
                    if (modelFacade.SelectById<ICmsSite>(cmsClassSite.SiteID) is { SiteGUID: var siteGuid })
                    {
                        if (ChannelInfoProvider.ProviderObject.Get(siteGuid) is { ChannelID: var channelId })
                        {
                            var info = new ContentTypeChannelInfo { ContentTypeChannelChannelID = channelId, ContentTypeChannelContentTypeID = targetClassId };
                            ContentTypeChannelInfoProvider.ProviderObject.Set(info);
                        }
                        else
                        {
                            logger.LogWarning("Channel for site with SiteGUID '{SiteGuid}' not found", siteGuid);
                        }
                    }
                    else
                    {
                        logger.LogWarning("Source site with SiteID '{SiteId}' not found", cmsClassSite.SiteID);
                    }
                }
            }
        }

        await MigratePageTemplateConfigurations();

        return new GenericCommandResult();
    }

    private async Task MigratePageTemplateConfigurations()
    {
        if (modelFacade.IsAvailable<ICmsPageTemplateConfiguration>())
        {
            foreach (var ksPageTemplateConfiguration in modelFacade.SelectAll<ICmsPageTemplateConfiguration>())
            {
                await pageTemplateMigrator.MigratePageTemplateConfigurationAsync(ksPageTemplateConfiguration);
            }
        }
    }

    private int? SaveUsingKxoApi(ICmsClass ksClass, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(ksClass, kxoDataClass);
        protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success: true })
            {
                var (dataClassInfo, newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(dataClassInfo.ClassName))
                {
                    dataClassInfo = reusableSchemaService.ConvertToReusableSchema(dataClassInfo);
                }

                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(ksClass, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    ksClass.ClassID,
                    dataClassInfo.ClassID
                );

                return dataClassInfo.ClassID;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", ksClass.ClassName);
        }

        return null;
    }
}
