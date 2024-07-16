
using CMS.DataProtection;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Migration.Toolkit.KXP.Context;

namespace Migration.Toolkit.Core.KX12.Handlers;
public class MigrateDataProtectionCommandHandler : IRequestHandler<MigrateDataProtectionCommand, CommandResult>, IDisposable
{
    private readonly ILogger<MigrateDataProtectionCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly IEntityMapper<CmsConsent, KXP.Models.CmsConsent> _consentMapper;
    private readonly IEntityMapper<CmsConsentArchive, KXP.Models.CmsConsentArchive> _consentArchiveMapper;
    private readonly IEntityMapper<CmsConsentAgreement, KXP.Models.CmsConsentAgreement> _consentAgreementMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    private static readonly int _batchSize = 1000;

    public MigrateDataProtectionCommandHandler(
        ILogger<MigrateDataProtectionCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        IEntityMapper<CmsConsent, KXP.Models.CmsConsent> consentMapper,
        IEntityMapper<CmsConsentArchive, KXP.Models.CmsConsentArchive> consentArchiveMapper,
        IEntityMapper<CmsConsentAgreement, KXP.Models.CmsConsentAgreement> consentAgreementMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx12ContextFactory = kx12ContextFactory;
        _consentMapper = consentMapper;
        _consentArchiveMapper = consentArchiveMapper;
        _consentAgreementMapper = consentAgreementMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigrateDataProtectionCommand request, CancellationToken cancellationToken)
    {
        int batchSize = _batchSize;

        await MigrateConsent(cancellationToken);
        await MigrateConsentArchive(cancellationToken);
        await MigrateConsentAgreement(cancellationToken, batchSize);

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsent(CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k12Consent in kx12Context.CmsConsents)
        {
            _protocol.FetchedSource(k12Consent);
            _logger.LogTrace("Migrating consent {ConsentName} with ConsentGuid {ConsentGuid}", k12Consent.ConsentName, k12Consent.ConsentGuid);

            var kxoConsent = await _kxpContext.CmsConsents.FirstOrDefaultAsync(consent => consent.ConsentGuid == k12Consent.ConsentGuid, cancellationToken);
            _protocol.FetchedTarget(kxoConsent);

            var mapped = _consentMapper.Map(k12Consent, kxoConsent);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                var (cmsConsent, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsConsent, nameof(cmsConsent));

                if (newInstance)
                {
                    _kxpContext.CmsConsents.Add(cmsConsent);
                }
                else
                {
                    _kxpContext.CmsConsents.Update(cmsConsent);
                }

                try
                {
                    await _kxpContext.SaveChangesAsync(cancellationToken);

                    _protocol.Success(k12Consent, cmsConsent, mapped);
                    _logger.LogEntitySetAction(newInstance, cmsConsent);
                    _primaryKeyMappingContext.SetMapping<CmsConsent>(r => r.ConsentId, k12Consent.ConsentId, cmsConsent.ConsentId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    _logger.LogEntitySetError(sqlException, newInstance, k12Consent);
                    _protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, k12Consent)
                        .WithMessage("Failed to migrate consent, target database constraint broken.")
                    );

                    await _kxpContext.DisposeAsync();
                    _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);
                }
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentArchive(CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k12ArchiveConsent in kx12Context.CmsConsentArchives)
        {
            _protocol.FetchedSource(k12ArchiveConsent);
            _logger.LogTrace("Migrating consent archive with ConsentArchiveGuid {ConsentGuid}", k12ArchiveConsent.ConsentArchiveGuid);

            var kxoConsentArchive = await _kxpContext.CmsConsentArchives.FirstOrDefaultAsync(consentArchive => consentArchive.ConsentArchiveGuid == k12ArchiveConsent.ConsentArchiveGuid, cancellationToken);
            _protocol.FetchedTarget(kxoConsentArchive);

            var mapped = _consentArchiveMapper.Map(k12ArchiveConsent, kxoConsentArchive);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                var (cmsConsentArchive, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsConsentArchive, nameof(cmsConsentArchive));

                if (newInstance)
                {
                    _kxpContext.CmsConsentArchives.Add(cmsConsentArchive);
                }
                else
                {
                    _kxpContext.CmsConsentArchives.Update(cmsConsentArchive);
                }

                try
                {
                    await _kxpContext.SaveChangesAsync(cancellationToken);

                    _protocol.Success(k12ArchiveConsent, cmsConsentArchive, mapped);
                    _logger.LogEntitySetAction(newInstance, cmsConsentArchive);
                    _primaryKeyMappingContext.SetMapping<CmsConsentArchive>(r => r.ConsentArchiveGuid,
                        k12ArchiveConsent.ConsentArchiveId, cmsConsentArchive.ConsentArchiveId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    _logger.LogEntitySetError(sqlException, newInstance, k12ArchiveConsent);
                    _protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, k12ArchiveConsent)
                        .WithMessage("Failed to migrate consent archive, target database constraint broken.")
                    );

                    await _kxpContext.DisposeAsync();
                    _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);
                }
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentAgreement(CancellationToken cancellationToken, int batchSize)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<KXP.Models.CmsConsentAgreement>();
        var consentAgreementNews = new List<KXP.Models.CmsConsentAgreement>();
        int itemsCount = kx12Context.CmsConsentAgreements.Count();

        foreach (var k12ConsentAgreement in kx12Context.CmsConsentAgreements)
        {
            _protocol.FetchedSource(k12ConsentAgreement);
            _logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", k12ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = await _kxpContext.CmsConsentAgreements.FirstOrDefaultAsync(consentAgreement => consentAgreement.ConsentAgreementGuid == k12ConsentAgreement.ConsentAgreementGuid, cancellationToken);
            _protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = _consentAgreementMapper.Map(k12ConsentAgreement, kxoConsentAgreement);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                var (cmsConsentAgreement, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsConsentAgreement, nameof(cmsConsentAgreement));

                if (newInstance)
                {
                    consentAgreementNews.Add(cmsConsentAgreement);
                }
                else
                {
                    consentAgreementUpdates.Add(cmsConsentAgreement);
                }
            }

            index++;
            indexFull++;

            if (index == batchSize || indexFull == itemsCount)
            {
                _kxpContext.CmsConsentAgreements.AddRange(consentAgreementNews);
                _kxpContext.CmsConsentAgreements.UpdateRange(consentAgreementUpdates);

                try
                {
                    await _kxpContext.SaveChangesAsync(cancellationToken);

                    foreach (var newK12ConsentAgreement in consentAgreementNews)
                    {
                        _protocol.Success(k12ConsentAgreement, newK12ConsentAgreement, mapped);
                        _logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newK12ConsentAgreement.ConsentAgreementGuid);
                    }

                    foreach (var updateK12ConsentAgreement in consentAgreementUpdates)
                    {
                        _protocol.Success(k12ConsentAgreement, updateK12ConsentAgreement, mapped);
                        _logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was updated",
                            updateK12ConsentAgreement.ConsentAgreementGuid);
                    }
                }
                catch (DbUpdateException dbUpdateException) when (
                    dbUpdateException.InnerException is SqlException sqlException &&
                    sqlException.Message.Contains("Cannot insert duplicate key row in object")
                )
                {
                    await _kxpContext.DisposeAsync();

                    _protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<KXP.Models.CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrints(consentAgreementNews)
                    );
                    _logger.LogEntitiesSetError(dbUpdateException, true, consentAgreementNews);


                    _protocol.Append(HandbookReferences
                        .ErrorUpdatingTargetInstance<KXP.Models.CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrints(consentAgreementUpdates)
                    );

                    var cai = ConsentAgreementInfo.New();
                    _protocol.Append(HandbookReferences
                        .ErrorUpdatingTargetInstance<KXP.Models.CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrint(cai)
                    );

                    _logger.LogEntitiesSetError(dbUpdateException, false, consentAgreementUpdates);

                    _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);
                }
                finally
                {
                    index = 0;
                    consentAgreementUpdates = [];
                    consentAgreementNews = [];
                }
            }
        }

        return new GenericCommandResult();
    }

    public void Dispose() => _kxpContext.Dispose();
}
