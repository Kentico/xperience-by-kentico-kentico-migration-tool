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
    private static readonly int batchSize = 1000;
    private readonly IEntityMapper<CmsConsentAgreement, KXP.Models.CmsConsentAgreement> consentAgreementMapper;
    private readonly IEntityMapper<CmsConsentArchive, KXP.Models.CmsConsentArchive> consentArchiveMapper;
    private readonly IEntityMapper<CmsConsent, KXP.Models.CmsConsent> consentMapper;
    private readonly IDbContextFactory<K11Context> k11ContextFactory;
    private readonly IDbContextFactory<KxpContext> kxpContextFactory;
    private readonly ILogger<MigrateDataProtectionCommandHandler> logger;
    private readonly PrimaryKeyMappingContext primaryKeyMappingContext;
    private readonly IProtocol protocol;

    private KxpContext kxpContext;

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
        this.logger = logger;
        this.kxpContextFactory = kxpContextFactory;
        this.k11ContextFactory = k11ContextFactory;
        this.consentMapper = consentMapper;
        this.consentArchiveMapper = consentArchiveMapper;
        this.consentAgreementMapper = consentAgreementMapper;
        this.primaryKeyMappingContext = primaryKeyMappingContext;
        this.protocol = protocol;
        kxpContext = this.kxpContextFactory.CreateDbContext();
    }

    public void Dispose() => kxpContext.Dispose();

    public async Task<CommandResult> Handle(MigrateDataProtectionCommand request, CancellationToken cancellationToken)
    {
        await MigrateConsent(cancellationToken);
        await MigrateConsentArchive(cancellationToken);
        await MigrateConsentAgreement(cancellationToken, batchSize);

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsent(CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k11Consent in k11Context.CmsConsents)
        {
            protocol.FetchedSource(k11Consent);
            logger.LogTrace("Migrating consent {ConsentName} with ConsentGuid {ConsentGuid}", k11Consent.ConsentName, k11Consent.ConsentGuid);

            var kxoConsent = await kxpContext.CmsConsents.FirstOrDefaultAsync(consent => consent.ConsentGuid == k11Consent.ConsentGuid, cancellationToken);
            protocol.FetchedTarget(kxoConsent);

            var mapped = consentMapper.Map(k11Consent, kxoConsent);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var cmsConsent, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsConsent, nameof(cmsConsent));

                if (newInstance)
                {
                    kxpContext.CmsConsents.Add(cmsConsent);
                }
                else
                {
                    kxpContext.CmsConsents.Update(cmsConsent);
                }

                try
                {
                    await kxpContext.SaveChangesAsync(cancellationToken);

                    protocol.Success(k11Consent, cmsConsent, mapped);
                    logger.LogEntitySetAction(newInstance, cmsConsent);
                    primaryKeyMappingContext.SetMapping<CmsConsent>(r => r.ConsentId, k11Consent.ConsentId, cmsConsent.ConsentId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    logger.LogEntitySetError(sqlException, newInstance, k11Consent);
                    protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, k11Consent)
                        .WithMessage("Failed to migrate consent, target database constraint broken.")
                    );

                    await kxpContext.DisposeAsync();
                    kxpContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);
                }
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentArchive(CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k11ArchiveConsent in k11Context.CmsConsentArchives)
        {
            protocol.FetchedSource(k11ArchiveConsent);
            logger.LogTrace("Migrating consent archive with ConsentArchiveGuid {ConsentGuid}", k11ArchiveConsent.ConsentArchiveGuid);

            var kxoConsentArchive = await kxpContext.CmsConsentArchives.FirstOrDefaultAsync(consentArchive => consentArchive.ConsentArchiveGuid == k11ArchiveConsent.ConsentArchiveGuid, cancellationToken);
            protocol.FetchedTarget(kxoConsentArchive);

            var mapped = consentArchiveMapper.Map(k11ArchiveConsent, kxoConsentArchive);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var cmsConsentArchive, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsConsentArchive, nameof(cmsConsentArchive));

                if (newInstance)
                {
                    kxpContext.CmsConsentArchives.Add(cmsConsentArchive);
                }
                else
                {
                    kxpContext.CmsConsentArchives.Update(cmsConsentArchive);
                }

                try
                {
                    await kxpContext.SaveChangesAsync(cancellationToken);

                    protocol.Success(k11ArchiveConsent, cmsConsentArchive, mapped);
                    logger.LogEntitySetAction(newInstance, cmsConsentArchive);
                    primaryKeyMappingContext.SetMapping<CmsConsentArchive>(r => r.ConsentArchiveGuid,
                        k11ArchiveConsent.ConsentArchiveId, cmsConsentArchive.ConsentArchiveId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    logger.LogEntitySetError(sqlException, newInstance, k11ArchiveConsent);
                    protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, k11ArchiveConsent)
                        .WithMessage("Failed to migrate consent archive, target database constraint broken.")
                    );

                    await kxpContext.DisposeAsync();
                    kxpContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);
                }
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentAgreement(CancellationToken cancellationToken, int batchSize)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<KXP.Models.CmsConsentAgreement>();
        var consentAgreementNews = new List<KXP.Models.CmsConsentAgreement>();
        int itemsCount = k11Context.CmsConsentAgreements.Count();

        foreach (var k11ConsentAgreement in k11Context.CmsConsentAgreements)
        {
            protocol.FetchedSource(k11ConsentAgreement);
            logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", k11ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = await kxpContext.CmsConsentAgreements.FirstOrDefaultAsync(consentAgreement => consentAgreement.ConsentAgreementGuid == k11ConsentAgreement.ConsentAgreementGuid, cancellationToken);
            protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = consentAgreementMapper.Map(k11ConsentAgreement, kxoConsentAgreement);
            protocol.MappedTarget(mapped);

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
                kxpContext.CmsConsentAgreements.AddRange(consentAgreementNews);
                kxpContext.CmsConsentAgreements.UpdateRange(consentAgreementUpdates);

                try
                {
                    await kxpContext.SaveChangesAsync(cancellationToken);

                    foreach (var newK11ConsentAgreement in consentAgreementNews)
                    {
                        protocol.Success(k11ConsentAgreement, newK11ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newK11ConsentAgreement.ConsentAgreementGuid);
                    }

                    foreach (var updateK11ConsentAgreement in consentAgreementUpdates)
                    {
                        protocol.Success(k11ConsentAgreement, updateK11ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was updated",
                            updateK11ConsentAgreement.ConsentAgreementGuid);
                    }
                }
                catch (DbUpdateException dbUpdateException) when (
                    dbUpdateException.InnerException is SqlException sqlException &&
                    sqlException.Message.Contains("Cannot insert duplicate key row in object")
                )
                {
                    await kxpContext.DisposeAsync();

                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<KXP.Models.CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrints(consentAgreementNews)
                    );
                    logger.LogEntitiesSetError(dbUpdateException, true, consentAgreementNews);


                    protocol.Append(HandbookReferences
                        .ErrorUpdatingTargetInstance<KXP.Models.CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrints(consentAgreementUpdates)
                    );

                    var cai = ConsentAgreementInfo.New();
                    protocol.Append(HandbookReferences
                        .ErrorUpdatingTargetInstance<KXP.Models.CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrint(cai)
                    );

                    logger.LogEntitiesSetError(dbUpdateException, false, consentAgreementUpdates);

                    kxpContext = await kxpContextFactory.CreateDbContextAsync(cancellationToken);
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
