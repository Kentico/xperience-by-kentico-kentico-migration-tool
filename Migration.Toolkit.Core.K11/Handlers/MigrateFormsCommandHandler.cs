namespace Migration.Toolkit.Core.K11.Handlers;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;
using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services.BulkCopy;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

public class MigrateFormsCommandHandler(ILogger<MigrateFormsCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<K11Context> k11ContextFactory,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IEntityMapper<CmsForm, KXP.Models.CmsForm> cmsFormMapper,
        KxpClassFacade kxpClassFacade,
        BulkDataCopyService bulkDataCopyService,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : IRequestHandler<MigrateFormsCommand, CommandResult>, IDisposable
{
    private KxpContext _kxpContext = kxpContextFactory.CreateDbContext();

    public async Task<CommandResult> Handle(MigrateFormsCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var cmsClassForms = k11Context.CmsClasses
            .Include(c => c.CmsForms)
            .Where(x => x.ClassIsForm == true)
            .OrderBy(x => x.ClassId)
            .AsEnumerable();

        foreach (var k11Class in cmsClassForms)
        {
            protocol.FetchedSource(k11Class);

            var kxoDataClass = kxpClassFacade.GetClass(k11Class.ClassGuid);
            protocol.FetchedTarget(kxoDataClass);

            var classSuccessFullySaved = MapAndSaveUsingKxoApi(k11Class, kxoDataClass);
            if (!classSuccessFullySaved)
            {
                continue;
            }

            foreach (var k11CmsForm in k11Class.CmsForms)
            {
                protocol.FetchedSource(k11CmsForm);

                var kxoCmsForm = _kxpContext.CmsForms.FirstOrDefault(f => f.FormGuid == k11CmsForm.FormGuid);

                protocol.FetchedTarget(kxoCmsForm);

                var mapped = cmsFormMapper.Map(k11CmsForm, kxoCmsForm);
                protocol.MappedTarget(mapped);

                if (mapped is { Success : true } result)
                {
                    var (cmsForm, newInstance) = result;
                    ArgumentNullException.ThrowIfNull(cmsForm, nameof(cmsForm));

                    try
                    {
                        if (newInstance)
                        {
                            _kxpContext.CmsForms.Add(cmsForm);
                        }
                        else
                        {
                            _kxpContext.CmsForms.Update(cmsForm);
                        }

                        await _kxpContext.SaveChangesAsync(cancellationToken);
                        logger.LogEntitySetAction(newInstance, cmsForm);

                        primaryKeyMappingContext.SetMapping<CmsForm>(
                            r => r.FormId,
                            k11Class.ClassId,
                            cmsForm.FormId
                        );
                    }
                    catch (Exception ex)
                    {
                        await _kxpContext.DisposeAsync(); // reset context errors
                        _kxpContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);

                        protocol.Append(HandbookReferences
                            .ErrorCreatingTargetInstance<KXP.Models.CmsForm>(ex)
                            .NeedsManualAction()
                            .WithIdentityPrint(cmsForm)
                        );
                        logger.LogEntitySetError(ex, newInstance, cmsForm);

                        continue;
                    }
                }

                XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                var xDoc = XDocument.Parse(k11Class.ClassXmlSchema);
                var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                    .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                    .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");

                var r = (k11Class.ClassTableName, k11Class.ClassGuid, autoIncrementColumns);
                logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", k11Class.ClassGuid, r);

                // check if data is present in target tables
                if (bulkDataCopyService.CheckIfDataExistsInTargetTable(k11Class.ClassTableName))
                {
                    logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                    protocol.Append(HandbookReferences
                        .DataMustNotExistInTargetInstanceTable(k11Class.ClassTableName)
                    );
                    continue;
                }

                var bulkCopyRequest = new BulkCopyRequest(
                    k11Class.ClassTableName, s => !autoIncrementColumns.Contains(s), _ => true,
                    20000
                );

                logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
            }
        }

        return new GenericCommandResult();
    }

    private bool MapAndSaveUsingKxoApi(CmsClass k11Class, DataClassInfo kxoDataClass)
    {
        var mapped = dataClassMapper.Map(k11Class, kxoDataClass);
        protocol.MappedTarget(mapped);

        if (mapped is { Success : true } result)
        {
            var (dataClassInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

            try
            {
                kxpClassFacade.SetClass(dataClassInfo);

                protocol.Success(k11Class, dataClassInfo, mapped);
                logger.LogEntitySetAction(newInstance, dataClassInfo);

                primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    k11Class.ClassId,
                    dataClassInfo.ClassID
                );

                return true;
            }
            catch (Exception ex)
            {
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<DataClassInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(dataClassInfo)
                );
                logger.LogEntitySetError(ex, newInstance, dataClassInfo);
            }
        }

        return false;
    }

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}