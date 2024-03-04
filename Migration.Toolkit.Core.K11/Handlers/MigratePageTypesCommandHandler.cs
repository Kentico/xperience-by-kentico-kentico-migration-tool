namespace Migration.Toolkit.Core.K11.Handlers;

using CMS.ContentEngine;
using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Helpers;
using Migration.Toolkit.Core.K11.Services;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;

public class MigratePageTypesCommandHandler(ILogger<MigratePageTypesCommandHandler> logger,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IDbContextFactory<K11Context> k11ContextFactory,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxpClassFacade kxpClassFacade,
        IProtocol protocol,
        ToolkitConfiguration toolkitConfiguration,
        KeyMappingContext keyMappingContext
        )
    : IRequestHandler<MigratePageTypesCommand, CommandResult>
{
    private const string CLASS_CMS_ROOT = "CMS.Root";

    public async Task<CommandResult> Handle(MigratePageTypesCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var entityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsClass>();

        var cmsClassesDocumentTypes = k11Context.CmsClasses
                .Include(c => c.Sites)
                .Where(x => x.ClassIsDocumentType)
                .OrderBy(x => x.ClassId)
                .AsEnumerable()
            ;

        using var k11Classes = EnumerableHelper.CreateDeferrableItemWrapper(cmsClassesDocumentTypes);

        while(k11Classes.GetNext(out var di))
        {
            var (_, k11Class) = di;

            if (k11Class.ClassInheritsFromClassId is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<CmsClass>(c=> c.ClassId, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (k11Classes.TryDeferItem(di))
                {
                    logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(k11Class), di.Recurrence);
                }
                else
                {
                    logger.LogErrorMissingDependency(k11Class, nameof(k11Class.ClassInheritsFromClassId), k11Class.ClassInheritsFromClassId, typeof(DataClassInfo));
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<CmsClass>(nameof(CmsClass.ClassId), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            protocol.FetchedSource(k11Class);

            if (entityConfiguration.ExcludeCodeNames.Contains(k11Class.ClassName, StringComparer.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.EntityExplicitlyExcludedByCodeName(k11Class.ClassName, "PageType"), k11Class);
                logger.LogWarning("CmsClass: {ClassName} was skipped => it is explicitly excluded in configuration", k11Class.ClassName);
                continue;
            }

            if (string.Equals(k11Class.ClassName, CLASS_CMS_ROOT, StringComparison.InvariantCultureIgnoreCase))
            {
                protocol.Warning(HandbookReferences.CmsClassCmsRootClassTypeSkip, k11Class);
                logger.LogInformation("CmsClass: {ClassName} was skipped => CMS.Root cannot be migrated", k11Class.ClassName);
                continue;
            }

            if (string.Equals(k11Class.ClassName, "cms.site", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            var kxoDataClass = kxpClassFacade.GetClass(k11Class.ClassGuid);
            protocol.FetchedTarget(kxoDataClass);

            if (SaveUsingKxoApi(k11Class, kxoDataClass) is { } targetClassId)
            {
                foreach (var sourceSite in k11Class.Sites)
                {
                    if (keyMappingContext.MapSourceKey<CmsSite, KXP.Models.CmsChannel, int?>(
                            s => s.SiteId,
                            s => s.SiteGuid,
                            sourceSite.SiteId,
                            t => t.ChannelId,
                            t => t.ChannelGuid
                        ) is {Success:true, Mapped: {} channelId})
                    {
                        var info = new ContentTypeChannelInfo
                        {
                            ContentTypeChannelChannelID = channelId,
                            ContentTypeChannelContentTypeID = targetClassId
                        };
                        ContentTypeChannelInfoProvider.ProviderObject.Set(info);
                    }
                    else
                    {
                        logger.LogWarning("Channel for site '{SiteName}' not found", sourceSite.SiteName);
                    }
                }
            }
        }

        return new GenericCommandResult();
    }

    private int? SaveUsingKxoApi(CmsClass k11Class, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(k11Class, kxoDataClass);
        protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success : true } result)
            {
                var (dataClassInfo, newInstance) = result;
                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(k11Class, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    k11Class.ClassId,
                    dataClassInfo.ClassID
                );

                return dataClassInfo.ClassID;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", k11Class.ClassName);
        }

        return null;
    }
}