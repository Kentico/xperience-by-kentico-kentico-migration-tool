using CMS.DataProtection;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Context;

namespace Migration.Toolkit.Core.K11.Handlers;

public class MigrateDataProtectionCommandHandler : IRequestHandler<MigrateDataProtectionCommand, CommandResult>, IDisposable
{
    private static readonly int _batchSize = 1000;
    private readonly IEntityMapper<CmsConsentAgreement, KXP.Models.CmsConsentAgreement> _consentAgreementMapper;
    private readonly IEntityMapper<CmsConsentArchive, KXP.Models.CmsConsentArchive> _consentArchiveMapper;
    private readonly IEntityMapper<CmsConsent, KXP.Models.CmsConsent> _consentMapper;
    private readonly IDbContextFactory<K11Context> _k11ContextFactory;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly ILogger<MigrateDataProtectionCommandHandler> _logger;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    public MigrateDataProtectionCommandHandler(
        ILogger<MigrateDataProtectionCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<K11Context> k11ContextFactory,
        IEntityMapper<CmsConsent, KXP.Models.CmsConsent> consentMapper,
        IEntityMapper<CmsConsentArchive, KXP.Models.CmsConsentArchive> consentArchiveMapper,
        IEntityMapper<CmsConsentAgreement, KXP.Models.CmsConsentAgreement> consentAgreementMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _k11ContextFactory = k11ContextFactory;
        _consentMapper = consentMapper;
        _consentArchiveMapper = consentArchiveMapper;
        _consentAgreementMapper = consentAgreementMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }

    public void Dispose() => _kxpContext.Dispose();

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
        await using var k11Context = await _k11ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k11Consent in k11Context.CmsConsents)
        {
            _protocol.FetchedSource(k11Consent);
            _logger.LogTrace("Migrating consent {ConsentName} with ConsentGuid {ConsentGuid}", k11Consent.ConsentName, k11Consent.ConsentGuid);

            var kxoConsent = await _kxpContext.CmsConsents.FirstOrDefaultAsync(consent => consent.ConsentGuid == k11Consent.ConsentGuid, cancellationToken);
            _protocol.FetchedTarget(kxoConsent);

            var mapped = _consentMapper.Map(k11Consent, kxoConsent);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var cmsConsent, bool newInstance) = result;
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

                    _protocol.Success(k11Consent, cmsConsent, mapped);
                    _logger.LogEntitySetAction(newInstance, cmsConsent);
                    _primaryKeyMappingContext.SetMapping<CmsConsent>(r => r.ConsentId, k11Consent.ConsentId, cmsConsent.ConsentId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    _logger.LogEntitySetError(sqlException, newInstance, k11Consent);
                    _protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, k11Consent)
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
        await using var k11Context = await _k11ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k11ArchiveConsent in k11Context.CmsConsentArchives)
        {
            _protocol.FetchedSource(k11ArchiveConsent);
            _logger.LogTrace("Migrating consent archive with ConsentArchiveGuid {ConsentGuid}", k11ArchiveConsent.ConsentArchiveGuid);

            var kxoConsentArchive = await _kxpContext.CmsConsentArchives.FirstOrDefaultAsync(consentArchive => consentArchive.ConsentArchiveGuid == k11ArchiveConsent.ConsentArchiveGuid, cancellationToken);
            _protocol.FetchedTarget(kxoConsentArchive);

            var mapped = _consentArchiveMapper.Map(k11ArchiveConsent, kxoConsentArchive);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var cmsConsentArchive, bool newInstance) = result;
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

                    _protocol.Success(k11ArchiveConsent, cmsConsentArchive, mapped);
                    _logger.LogEntitySetAction(newInstance, cmsConsentArchive);
                    _primaryKeyMappingContext.SetMapping<CmsConsentArchive>(r => r.ConsentArchiveGuid,
                        k11ArchiveConsent.ConsentArchiveId, cmsConsentArchive.ConsentArchiveId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    _logger.LogEntitySetError(sqlException, newInstance, k11ArchiveConsent);
                    _protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, k11ArchiveConsent)
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
        await using var k11Context = await _k11ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<KXP.Models.CmsConsentAgreement>();
        var consentAgreementNews = new List<KXP.Models.CmsConsentAgreement>();
        int itemsCount = k11Context.CmsConsentAgreements.Count();

        foreach (var k11ConsentAgreement in k11Context.CmsConsentAgreements)
        {
            _protocol.FetchedSource(k11ConsentAgreement);
            _logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", k11ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = await _kxpContext.CmsConsentAgreements.FirstOrDefaultAsync(consentAgreement => consentAgreement.ConsentAgreementGuid == k11ConsentAgreement.ConsentAgreementGuid, cancellationToken);
            _protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = _consentAgreementMapper.Map(k11ConsentAgreement, kxoConsentAgreement);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var cmsConsentAgreement, bool newInstance) = result;
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

                    foreach (var newK11ConsentAgreement in consentAgreementNews)
                    {
                        _protocol.Success(k11ConsentAgreement, newK11ConsentAgreement, mapped);
                        _logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newK11ConsentAgreement.ConsentAgreementGuid);
                    }

                    foreach (var updateK11ConsentAgreement in consentAgreementUpdates)
                    {
                        _protocol.Success(k11ConsentAgreement, updateK11ConsentAgreement, mapped);
                        _logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was updated",
                            updateK11ConsentAgreement.ConsentAgreementGuid);
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
}
