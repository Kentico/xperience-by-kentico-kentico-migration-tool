using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;

using CMS.DataEngine;
using CMS.Modules;

using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Handlers;

public class MigrateCustomTablesHandler(
    ILogger<MigrateCustomTablesHandler> logger,
    ModelFacade modelFacade,
    KxpClassFacade kxpClassFacade,
    IProtocol protocol,
    BulkDataCopyService bulkDataCopyService,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext
// ReusableSchemaService reusableSchemaService
)
    : IRequestHandler<MigrateCustomTablesCommand, CommandResult>
{
    private readonly Guid resourceGuidNamespace = new("C4E3F5FD-9220-4300-91CE-8EB565D3235E");
    private ResourceInfo? customTableResource;

    public async Task<CommandResult> Handle(MigrateCustomTablesCommand request, CancellationToken cancellationToken)
    {
        await MigrateCustomTables();

        return new GenericCommandResult();
    }

    private async Task<ResourceInfo> EnsureCustomTablesResource()
    {
        if (customTableResource != null)
        {
            return customTableResource;
        }

        const string resourceName = "customtables";
        var resourceGuid = GuidV5.NewNameBased(resourceGuidNamespace, resourceName);
        var resourceInfo = await ResourceInfoProvider.ProviderObject.GetAsync(resourceGuid);
        if (resourceInfo == null)
        {
            resourceInfo = new ResourceInfo
            {
                ResourceDisplayName = "Custom tables",
                ResourceName = resourceName,
                ResourceDescription = "Container resource for migrated custom tables",
                ResourceGUID = resourceGuid,
                ResourceLastModified = default,
                ResourceIsInDevelopment = false
            };
            ResourceInfoProvider.ProviderObject.Set(resourceInfo);
        }

        customTableResource = resourceInfo;
        return resourceInfo;
    }

    private async Task MigrateCustomTables()
    {
        using var srcClassesDe = EnumerableHelper.CreateDeferrableItemWrapper(
            modelFacade.Select<ICmsClass>("ClassIsCustomTable=1", "ClassID ASC")
        );

        while (srcClassesDe.GetNext(out var di))
        {
            var (_, srcClass) = di;

            if (!srcClass.ClassIsCustomTable)
            {
                continue;
            }

            if (srcClass.ClassInheritsFromClassID is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<ICmsClass>(c => c.ClassID, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (srcClassesDe.TryDeferItem(di))
                {
                    logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(srcClass), di.Recurrence);
                }
                else
                {
                    logger.LogErrorMissingDependency(srcClass, nameof(srcClass.ClassInheritsFromClassID), srcClass.ClassInheritsFromClassID, typeof(DataClassInfo));
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<ICmsClass>(nameof(ICmsClass.ClassID), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            protocol.FetchedSource(srcClass);

            var xbkDataClass = kxpClassFacade.GetClass(srcClass.ClassGUID);

            protocol.FetchedTarget(xbkDataClass);

            if (await SaveClassUsingKxoApi(srcClass, xbkDataClass) is { } savedDataClass)
            {
                Debug.Assert(savedDataClass.ClassID != 0, "xbkDataClass.ClassID != 0");
                // MigrateClassSiteMappings(kx13Class, xbkDataClass);

                xbkDataClass = DataClassInfoProvider.ProviderObject.Get(savedDataClass.ClassID);
                // await MigrateAlternativeForms(srcClass, savedDataClass, cancellationToken);

                #region Migrate coupled data class data

                if (srcClass.ClassShowAsSystemTable is false)
                {
                    Debug.Assert(xbkDataClass.ClassTableName != null, "kx13Class.ClassTableName != null");
                    // var csi = new ClassStructureInfo(kx13Class.ClassXmlSchema, kx13Class.ClassXmlSchema, kx13Class.ClassTableName);

                    XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                    XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                    var xDoc = XDocument.Parse(xbkDataClass.ClassXmlSchema);
                    var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                        .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                        .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                    Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");
                    var r = (xbkDataClass.ClassTableName, xbkDataClass.ClassGUID, autoIncrementColumns);
                    logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", srcClass.ClassGUID, r);

                    try
                    {
                        // check if data is present in target tables
                        if (bulkDataCopyService.CheckIfDataExistsInTargetTable(xbkDataClass.ClassTableName))
                        {
                            logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                            protocol.Append(HandbookReferences.DataMustNotExistInTargetInstanceTable(xbkDataClass.ClassTableName));
                            continue;
                        }

                        var bulkCopyRequest = new BulkCopyRequest(
                            xbkDataClass.ClassTableName,
                            s => true, // s => !autoIncrementColumns.Contains(s),
                            _ => true,
                            20000
                        );

                        logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                        bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error while copying data to table");
                    }
                }

                #endregion
            }
        }
    }

    private async Task<DataClassInfo?> SaveClassUsingKxoApi(ICmsClass srcClass, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(srcClass, kxoDataClass);
        protocol.MappedTarget(mapped);

        try
        {
            if (mapped is { Success: true } result)
            {
                (var dataClassInfo, bool newInstance) = result;

                ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

                // if (reusableSchemaService.IsConversionToReusableFieldSchemaRequested(dataClassInfo.ClassName))
                // {
                //     dataClassInfo = reusableSchemaService.ConvertToReusableSchema(dataClassInfo);
                // }

                var containerResource = await EnsureCustomTablesResource();
                dataClassInfo.ClassResourceID = containerResource.ResourceID;
                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(srcClass, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<ICmsClass>(
                    r => r.ClassID,
                    srcClass.ClassID,
                    dataClassInfo.ClassID
                );

                return dataClassInfo;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving page type {ClassName}", srcClass.ClassName);
        }

        return null;
    }
}
