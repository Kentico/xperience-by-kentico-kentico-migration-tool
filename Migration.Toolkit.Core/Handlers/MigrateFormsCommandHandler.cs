namespace Migration.Toolkit.Core.Handlers;

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
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

public class MigrateFormsCommandHandler : IRequestHandler<MigrateFormsCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateFormsCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
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
        IDbContextFactory<KX13Context> kx13ContextFactory,
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
        _kx13ContextFactory = kx13ContextFactory;
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
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var cmsClassForms = kx13Context.CmsClasses
            .Include(c => c.CmsForms)
            .Where(x => x.ClassIsForm == true)
            .OrderBy(x => x.ClassId)
            .AsEnumerable();

        foreach (var kx13Class in cmsClassForms)
        {
            _protocol.FetchedSource(kx13Class);

            var kxoDataClass = _kxpClassFacade.GetClass(kx13Class.ClassGuid);
            _protocol.FetchedTarget(kxoDataClass);

            var classSuccessFullySaved = MapAndSaveUsingKxoApi(kx13Class, kxoDataClass);
            if (!classSuccessFullySaved)
            {
                continue;
            }

            foreach (var kx13CmsForm in kx13Class.CmsForms)
            {
                _protocol.FetchedSource(kx13CmsForm);

                var kxoCmsForm = _kxpContext.CmsForms.FirstOrDefault(f => f.FormGuid == kx13CmsForm.FormGuid);

                _protocol.FetchedTarget(kxoCmsForm);

                var mapped = _cmsFormMapper.Map(kx13CmsForm, kxoCmsForm);
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
                            kx13Class.ClassId,
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

                Debug.Assert(kx13Class.ClassTableName != null, "kx13Class.ClassTableName != null");
                // var csi = new ClassStructureInfo(kx13Class.ClassXmlSchema, kx13Class.ClassXmlSchema, kx13Class.ClassTableName);

                XNamespace nsSchema = "http://www.w3.org/2001/XMLSchema";
                XNamespace msSchema = "urn:schemas-microsoft-com:xml-msdata";
                var xDoc = XDocument.Parse(kx13Class.ClassXmlSchema);
                var autoIncrementColumns = xDoc.Descendants(nsSchema + "element")
                    .Where(x => x.Attribute(msSchema + "AutoIncrement")?.Value == "true")
                    .Select(x => x.Attribute("name")?.Value).ToImmutableHashSet();

                Debug.Assert(autoIncrementColumns.Count == 1, "autoIncrementColumns.Count == 1");
                // TODO tk: 2022-07-08 not true : autoIncrementColumns.First() == csi.IDColumn
                // Debug.Assert(autoIncrementColumns.First() == csi.IDColumn, "autoIncrementColumns.First() == csi.IDColumn");

                var r = (kx13Class.ClassTableName, kx13Class.ClassGuid, autoIncrementColumns);
                _logger.LogTrace("Class '{ClassGuild}' Resolved as: {Result}", kx13Class.ClassGuid, r);

                // check if data is present in target tables
                if (_bulkDataCopyService.CheckIfDataExistsInTargetTable(kx13Class.ClassTableName))
                {
                    _logger.LogWarning("Data exists in target coupled data table '{TableName}' - cannot migrate, skipping form data migration", r.ClassTableName);
                    _protocol.Append(HandbookReferences
                        .DataMustNotExistInTargetInstanceTable(kx13Class.ClassTableName)
                    );
                    continue;
                }

                var bulkCopyRequest = new BulkCopyRequest(
                    kx13Class.ClassTableName, s => !autoIncrementColumns.Contains(s), _ => true,
                    20000
                );

                _logger.LogTrace("Bulk data copy request: {Request}", bulkCopyRequest);
                _bulkDataCopyService.CopyTableToTable(bulkCopyRequest);
            }
        }

        return new GenericCommandResult();
    }

    private bool MapAndSaveUsingKxoApi(CmsClass kx13Class, DataClassInfo kxoDataClass)
    {
        var mapped = _dataClassMapper.Map(kx13Class, kxoDataClass);
        _protocol.MappedTarget(mapped);

        if (mapped is { Success : true } result)
        {
            var (dataClassInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(dataClassInfo, nameof(dataClassInfo));

            try
            {
                _kxpClassFacade.SetClass(dataClassInfo);

                _protocol.Success(kx13Class, dataClassInfo, mapped);
                _logger.LogEntitySetAction(newInstance, dataClassInfo);

                _primaryKeyMappingContext.SetMapping<CmsClass>(
                    r => r.ClassId,
                    kx13Class.ClassId,
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