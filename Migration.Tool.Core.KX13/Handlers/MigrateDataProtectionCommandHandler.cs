using CMS.DataProtection;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Context;
using Migration.Tool.KXP.Context;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateDataProtectionCommandHandler : IRequestHandler<MigrateDataProtectionCommand, CommandResult>, IDisposable
{
    private static readonly int batchSize = 1000;
    private readonly IEntityMapper<KX13M.CmsConsentAgreement, CmsConsentAgreement> consentAgreementMapper;
    private readonly IEntityMapper<KX13M.CmsConsentArchive, CmsConsentArchive> consentArchiveMapper;
    private readonly IEntityMapper<KX13M.CmsConsent, CmsConsent> consentMapper;
    private readonly IDbContextFactory<KX13Context> kx13ContextFactory;
    private readonly IDbContextFactory<KxpContext> kxpContextFactory;
    private readonly ILogger<MigrateDataProtectionCommandHandler> logger;
    private readonly PrimaryKeyMappingContext primaryKeyMappingContext;
    private readonly IProtocol protocol;

    private KxpContext kxpContext;

    public MigrateDataProtectionCommandHandler(
        ILogger<MigrateDataProtectionCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<KX13M.CmsConsent, CmsConsent> consentMapper,
        IEntityMapper<KX13M.CmsConsentArchive, CmsConsentArchive> consentArchiveMapper,
        IEntityMapper<KX13M.CmsConsentAgreement, CmsConsentAgreement> consentAgreementMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        this.logger = logger;
        this.kxpContextFactory = kxpContextFactory;
        this.kx13ContextFactory = kx13ContextFactory;
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
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var kx13Consent in kx13Context.CmsConsents)
        {
            protocol.FetchedSource(kx13Consent);
            logger.LogTrace("Migrating consent {ConsentName} with ConsentGuid {ConsentGuid}", kx13Consent.ConsentName, kx13Consent.ConsentGuid);

            var kxoConsent = await kxpContext.CmsConsents.FirstOrDefaultAsync(consent => consent.ConsentGuid == kx13Consent.ConsentGuid, cancellationToken);
            protocol.FetchedTarget(kxoConsent);

            var mapped = consentMapper.Map(kx13Consent, kxoConsent);
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

                    protocol.Success(kx13Consent, cmsConsent, mapped);
                    logger.LogEntitySetAction(newInstance, cmsConsent);
                    primaryKeyMappingContext.SetMapping<KX13M.CmsConsent>(r => r.ConsentId, kx13Consent.ConsentId, cmsConsent.ConsentId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    logger.LogEntitySetError(sqlException, newInstance, kx13Consent);
                    protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, kx13Consent)
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
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var kx13ArchiveConsent in kx13Context.CmsConsentArchives)
        {
            protocol.FetchedSource(kx13ArchiveConsent);
            logger.LogTrace("Migrating consent archive with ConsentArchiveGuid {ConsentGuid}", kx13ArchiveConsent.ConsentArchiveGuid);

            var kxoConsentArchive = await kxpContext.CmsConsentArchives.FirstOrDefaultAsync(consentArchive => consentArchive.ConsentArchiveGuid == kx13ArchiveConsent.ConsentArchiveGuid, cancellationToken);
            protocol.FetchedTarget(kxoConsentArchive);

            var mapped = consentArchiveMapper.Map(kx13ArchiveConsent, kxoConsentArchive);
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

                    protocol.Success(kx13ArchiveConsent, cmsConsentArchive, mapped);
                    logger.LogEntitySetAction(newInstance, cmsConsentArchive);
                    primaryKeyMappingContext.SetMapping<KX13M.CmsConsentArchive>(r => r.ConsentArchiveGuid,
                        kx13ArchiveConsent.ConsentArchiveId, cmsConsentArchive.ConsentArchiveId);
                }
                /*Violation in unique index or Violation in unique constraint */
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    logger.LogEntitySetError(sqlException, newInstance, kx13ArchiveConsent);
                    protocol.Append(HandbookReferences
                        .DbConstraintBroken(sqlException, kx13ArchiveConsent)
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
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<CmsConsentAgreement>();
        var consentAgreementNews = new List<CmsConsentAgreement>();
        int itemsCount = kx13Context.CmsConsentAgreements.Count();

        foreach (var kx13ConsentAgreement in kx13Context.CmsConsentAgreements)
        {
            protocol.FetchedSource(kx13ConsentAgreement);
            logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", kx13ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = await kxpContext.CmsConsentAgreements.FirstOrDefaultAsync(consentAgreement => consentAgreement.ConsentAgreementGuid == kx13ConsentAgreement.ConsentAgreementGuid, cancellationToken);
            protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = consentAgreementMapper.Map(kx13ConsentAgreement, kxoConsentAgreement);
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

                    foreach (var newKx13ConsentAgreement in consentAgreementNews)
                    {
                        protocol.Success(kx13ConsentAgreement, newKx13ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newKx13ConsentAgreement.ConsentAgreementGuid);
                    }

                    foreach (var updateKx13ConsentAgreement in consentAgreementUpdates)
                    {
                        protocol.Success(kx13ConsentAgreement, updateKx13ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was updated",
                            updateKx13ConsentAgreement.ConsentAgreementGuid);
                    }
                }
                catch (DbUpdateException dbUpdateException) when (
                    dbUpdateException.InnerException is SqlException sqlException &&
                    sqlException.Message.Contains("Cannot insert duplicate key row in object")
                )
                {
                    await kxpContext.DisposeAsync();

                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrints(consentAgreementNews)
                    );
                    logger.LogEntitiesSetError(dbUpdateException, true, consentAgreementNews);


                    protocol.Append(HandbookReferences
                        .ErrorUpdatingTargetInstance<CmsConsentAgreement>(dbUpdateException)
                        .NeedsManualAction()
                        .WithIdentityPrints(consentAgreementUpdates)
                    );

                    var cai = ConsentAgreementInfo.New();
                    protocol.Append(HandbookReferences
                        .ErrorUpdatingTargetInstance<CmsConsentAgreement>(dbUpdateException)
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
