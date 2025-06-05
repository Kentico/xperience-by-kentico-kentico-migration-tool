using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using CMS.Modules;
using Kentico.Xperience.UMT.Services;
using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Builders;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services.BulkCopy;
using Migration.Tool.KXP.Api;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Mappers;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Providers;
using Newtonsoft.Json;

namespace Migration.Tool.Source.Handlers;

public class MigrateCustomTablesHandler(
    ILogger<MigrateCustomTablesHandler> logger,
    ModelFacade modelFacade,
    KxpClassFacade kxpClassFacade,
    IProtocol protocol,
    BulkDataCopyService bulkDataCopyService,
    IImporter importer,
    IUmtMapper<CustomTableMapperSource> mapper,
    IEntityMapper<ICmsClass, DataClassInfo> dataClassMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    ClassMappingProvider classMappingProvider,
    ToolConfiguration toolConfiguration
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
#pragma warning disable CS0618 // Type or member is obsolete
        var resourceInfo = await ResourceInfoProvider.ProviderObject.GetAsync(resourceGuid);
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning disable CS0618 // Type or member is obsolete
            ResourceInfoProvider.ProviderObject.Set(resourceInfo);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        customTableResource = resourceInfo;
        return resourceInfo;
    }

    private async Task MigrateCustomTables()
    {
        var dataClassEntityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<DataClassInfo>();

        using var srcClassesDe = EnumerableHelper.CreateDeferrableItemWrapper(
            modelFacade.Select<ICmsClass>("ClassIsCustomTable=1", "ClassID ASC")
        );

        var manualMappings = classMappingProvider.ExecuteMappings();
        var remapped = new List<(ICmsClass ksClass, DataClassInfo target, IClassMapping mapping)>();

        while (srcClassesDe.GetNext(out var di))
        {
            var (_, ksClass) = di;

            if (manualMappings.TryGetValue(ksClass.ClassName, out var mappedToClass))
            {
                remapped.Add((ksClass, mappedToClass.target, mappedToClass.mappping));
                continue;
            }

            if (!ksClass.ClassIsCustomTable)
            {
                continue;
            }

            if (dataClassEntityConfiguration.ExcludeCodeNames.Contains(ksClass.ClassName,
                    StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (ksClass.ClassInheritsFromClassID is { } classInheritsFromClassId && !primaryKeyMappingContext.HasMapping<ICmsClass>(c => c.ClassID, classInheritsFromClassId))
            {
                // defer migration to later stage
                if (srcClassesDe.TryDeferItem(di))
                {
                    logger.LogTrace("Class {Class} inheritance parent not found, deferring migration to end. Attempt {Attempt}", Printer.GetEntityIdentityPrint(ksClass), di.Recurrence);
                }
                else
                {
                    logger.LogErrorMissingDependency(ksClass, nameof(ksClass.ClassInheritsFromClassID), ksClass.ClassInheritsFromClassID, typeof(DataClassInfo));
                    protocol.Append(HandbookReferences
                        .MissingRequiredDependency<ICmsClass>(nameof(ICmsClass.ClassID), classInheritsFromClassId)
                        .NeedsManualAction()
                    );
                }

                continue;
            }

            protocol.FetchedSource(ksClass);

            var xbkDataClass = kxpClassFacade.GetClass(ksClass.ClassGUID);

            protocol.FetchedTarget(xbkDataClass);

            if (await SaveClassUsingKxoApi(ksClass, xbkDataClass) is { } savedDataClass)
            {
                Debug.Assert(savedDataClass.ClassID != 0, "xbkDataClass.ClassID != 0");
                // MigrateClassSiteMappings(kx13Class, xbkDataClass);

                xbkDataClass = DataClassInfoProvider.ProviderObject.Get(savedDataClass.ClassID);
                // await MigrateAlternativeForms(srcClass, savedDataClass, cancellationToken);

                #region Migrate coupled data class data

                if (ksClass.ClassShowAsSystemTable is false)
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
                    logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", ksClass.ClassGUID, r);

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

        foreach (var (cmsClass, targetDataClass, mapping) in remapped)
        {
            if (string.IsNullOrWhiteSpace(cmsClass.ClassTableName))
            {
                logger.LogError("Class {Class} is missing table name", cmsClass.ClassName);
                continue;
            }

            var customTableItems = modelFacade.SelectAllAsDictionary(cmsClass.ClassTableName);
            foreach (var customTableItem in customTableItems)
            {
                var results = mapper.Map(new CustomTableMapperSource(
                    targetDataClass.ClassFormDefinition,
                    cmsClass.ClassFormDefinition,
                    customTableItem.TryGetValue("ItemGUID", out object? itemGuid) && itemGuid is Guid guid ? guid : Guid.NewGuid(), // TODO tomas.krch: 2024-12-03 provide guid?
                    cmsClass,
                    customTableItem,
                    mapping
                ));
                try
                {
                    var commonDataInfos = new List<ContentItemCommonDataInfo>();
                    foreach (var umtModel in results)
                    {
                        switch (await importer.ImportAsync(umtModel))
                        {
                            case { Success: false } result:
                            {
                                logger.LogError("Failed to import: {Exception}, {ValidationResults}", result.Exception, JsonConvert.SerializeObject(result.ModelValidationResults));
                                break;
                            }
                            case { Success: true, Imported: ContentItemCommonDataInfo ccid }:
                            {
                                commonDataInfos.Add(ccid);
                                Debug.Assert(ccid.ContentItemCommonDataContentLanguageID != 0, "ccid.ContentItemCommonDataContentLanguageID != 0");
                                break;
                            }
                            case { Success: true, Imported: ContentItemLanguageMetadataInfo cclm }:
                            {
                                Debug.Assert(cclm.ContentItemLanguageMetadataContentLanguageID != 0, "ccid.ContentItemCommonDataContentLanguageID != 0");
                                break;
                            }
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occured while mapping custom table item to content item");
                }
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
