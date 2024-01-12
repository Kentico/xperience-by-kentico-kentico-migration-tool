namespace Migration.Toolkit.Core.KX12.Handlers;

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
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

public class MigrateFormsCommandHandler : IRequestHandler<MigrateFormsCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateFormsCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly IEntityMapper<CmsClass, DataClassInfo> _dataClassMapper;
    private readonly IEntityMapper<CmsForm, KXP.Models.CmsForm> _cmsFormMapper;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly BulkDataCopyService _bulkDataCopyService;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    public MigrateFormsCommandHandler(
        ILogger<MigrateFormsCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        IEntityMapper<CmsClass, DataClassInfo> dataClassMapper,
        IEntityMapper<CmsForm, KXP.Models.CmsForm> cmsFormMapper,
        KxpClassFacade kxpClassFacade,
        BulkDataCopyService bulkDataCopyService,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx12ContextFactory = kx12ContextFactory;
        _dataClassMapper = dataClassMapper;
        _cmsFormMapper = cmsFormMapper;
        _kxpClassFacade = kxpClassFacade;
        _bulkDataCopyService = bulkDataCopyService;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = kxpContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigrateFormsCommand request, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var cmsClassForms = kx12Context.CmsClasses
            .Include(c => c.CmsForms)
            .Where(x => x.ClassIsForm == true)
            .OrderBy(x => x.ClassId)
            .AsEnumerable();

        foreach (var k12Class in cmsClassForms)
        {
            _protocol.FetchedSource(k12Class);

            var kxoDataClass = _kxpClassFacade.GetClass(k12Class.ClassGuid);
            _protocol.FetchedTarget(kxoDataClass);

            var classSuccessFullySaved = MapAndSaveUsingKxoApi(k12Class, kxoDataClass);
            if (!classSuccessFullySaved)
            {
                continue;
            }

            foreach (var k12CmsForm in k12Class.CmsForms)
            {
                _protocol.FetchedSource(k12CmsForm);

                var kxoCmsForm = _kxpContext.CmsForms.FirstOrDefault(f => f.FormGuid == k12CmsForm.FormGuid);

                _protocol.FetchedTarget(kxoCmsForm);

                var mapped = _cmsFormMapper.Map(k12CmsForm, kxoCmsForm);
                _protocol.MappedTarget(mapped);

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
                        _logger.LogEntitySetAction(newInstance, cmsForm);

                        _primaryKeyMappingContext.SetMapping<CmsForm>(
                            r => r.FormId,
                            k12Class.ClassId,
                            cmsForm.FormId
                        );
                    }
                    catch (Exception ex)
                    {
                        await _kxpContext.DisposeAsync(); // reset context errors
                        _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);

                        _protocol.Append(HandbookReferences
                            .ErrorCreatingTargetInstance<KXP.Models.CmsForm>(ex)
                            .NeedsManualAction()
                            .WithIdentityPrint(cmsForm)
                        );
                        _logger.LogEntitySetError(ex, newInstance, cmsForm);

                        continue;
                    }
                }

                Debug.Assert(k12Class.ClassTableName != null, "k12Class.ClassTableName != null");

                XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                var xDoc = XDocument.Parse(k12Class.ClassXmlSchema);
                var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                    .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                    .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");
                // TODO tk: 2022-07-08 not true : autoIncrementColumns.First() == csi.IDColumn
                // Debug.Assert(autoIncrementColumns.First() == csi.IDColumn, "autoIncrementColumns.First() == csi.IDColumn");

                var r = (k12Class.ClassTableName, k12Class.ClassGuid, autoIncrementColumns);
                _logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", k12Class.ClassGuid, r);

                // check if data is present in target tables
                if (_bulkDataCopyService.CheckIfDataExistsInTargetTable(k12Class.ClassTableName))
                {
                    _logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                    _protocol.Append(HandbookReferences
                        .DataMustNotExistInTargetInstanceTable(k12Class.ClassTableName)
                    );
                    continue;
                }

                var bulkCopyRequest = new BulkCopyRequest(
                    k12Class.ClassTableName, s => !autoIncrementColumns.Contains(s), _ => true,
                    20000
                );

                _logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
            }
        }

        return new GenericCommandResult();
    }

    private bool MapAndSaveUsingKxoApi(CmsClass k12Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(k12Class, kxoDataClass);
        _protocol.MappedTarget(mapped);

        if (mapped is { Success : true } result)
        {
            var (dataClassInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

            try
            {
                _kxpClassFacade.SetClass(dataClassInfo);

                _protocol.Success(k12Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    k12Class.ClassId,
                    dataClassInfo.ClassID
                );

                return true;
            }
            catch (Exception ex)
            {
                _protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<DataClassInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(dataClassInfo)
                );
                _logger.LogEntitySetError(ex, newInstance, dataClassInfo);
            }
        }

        return false;
    }

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}