using CMS.DataEngine;
using CMS.DataProtection;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Context;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateDataProtectionCommandHandler : IRequestHandler<MigrateDataProtectionCommand, CommandResult>
{
    private static readonly int batchSize = 1000;
    private readonly IEntityMapper<KX13M.CmsConsentAgreement, ConsentAgreementInfo> consentAgreementMapper;
    private readonly IEntityMapper<KX13M.CmsConsentArchive, ConsentArchiveInfo> consentArchiveMapper;
    private readonly IEntityMapper<KX13M.CmsConsent, ConsentInfo> consentMapper;
    private readonly IDbContextFactory<KX13Context> kx13ContextFactory;
    private readonly ILogger<MigrateDataProtectionCommandHandler> logger;
    private readonly PrimaryKeyMappingContext primaryKeyMappingContext;
    private readonly IProtocol protocol;

    public MigrateDataProtectionCommandHandler(
        ILogger<MigrateDataProtectionCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<KX13M.CmsConsent, ConsentInfo> consentMapper,
        IEntityMapper<KX13M.CmsConsentArchive, ConsentArchiveInfo> consentArchiveMapper,
        IEntityMapper<KX13M.CmsConsentAgreement, ConsentAgreementInfo> consentAgreementMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        this.logger = logger;
        this.kx13ContextFactory = kx13ContextFactory;
        this.consentMapper = consentMapper;
        this.consentArchiveMapper = consentArchiveMapper;
        this.consentAgreementMapper = consentAgreementMapper;
        this.primaryKeyMappingContext = primaryKeyMappingContext;
        this.protocol = protocol;
    }

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

            var kxoConsent = ConsentInfo.Provider.Get().WhereEquals(nameof(ConsentInfo.ConsentGuid), kx13Consent.ConsentGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsent);

            var mapped = consentMapper.Map(kx13Consent, kxoConsent);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var consentInfo, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(consentInfo, nameof(consentInfo));

                try
                {
                    using (var conn = ConnectionHelper.GetConnection())
                    {
                        using var scope = new CMSTransactionScope(conn);
                        string originalHash = consentInfo.ConsentHash;

                        ConsentInfo.Provider.Set(consentInfo);

                        // Provider regenerates the hash. Manually revert to the original one
                        conn.DataConnection.ExecuteNonQuery("UPDATE [CMS_Consent] SET ConsentHash=@originalHash", new QueryDataParameters() { { "originalHash", originalHash } }, QueryTypeEnum.SQLQuery, true);
                        scope.Commit();
                    }

                    protocol.Success(kx13Consent, consentInfo, mapped);
                    logger.LogEntitySetAction(newInstance, consentInfo);
                    primaryKeyMappingContext.SetMapping<KX13M.CmsConsent>(r => r.ConsentId, kx13Consent.ConsentId, consentInfo.ConsentID);
                }
                catch (Exception ex)
                {
                    logger.LogEntitySetError(ex, newInstance, consentInfo);
                    protocol.Append(HandbookReferences
                        .ErrorCreatingTargetInstance<ConsentInfo>(ex)
                        .NeedsManualAction()
                        .WithIdentityPrint(consentInfo)
                    );
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

            var kxoConsentArchive = ConsentArchiveInfo.Provider.Get().WhereEquals(nameof(ConsentArchiveInfo.ConsentArchiveGuid), kx13ArchiveConsent.ConsentArchiveGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsentArchive);

            var mapped = consentArchiveMapper.Map(kx13ArchiveConsent, kxoConsentArchive);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var consentArchiveInfo, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(consentArchiveInfo, nameof(consentArchiveInfo));

                ConsentArchiveInfo.Provider.Set(consentArchiveInfo);
                protocol.Success(kx13ArchiveConsent, consentArchiveInfo, mapped);
                logger.LogEntitySetAction(newInstance, consentArchiveInfo);
                primaryKeyMappingContext.SetMapping<KX13M.CmsConsentArchive>(r => r.ConsentArchiveGuid,
                kx13ArchiveConsent.ConsentArchiveId, consentArchiveInfo.ConsentArchiveID);
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentAgreement(CancellationToken cancellationToken, int batchSize)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<ConsentAgreementInfo>();
        var consentAgreementNews = new List<ConsentAgreementInfo>();
        int itemsCount = kx13Context.CmsConsentAgreements.Count();

        foreach (var kx13ConsentAgreement in kx13Context.CmsConsentAgreements)
        {
            protocol.FetchedSource(kx13ConsentAgreement);
            logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", kx13ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = ConsentAgreementInfo.Provider.Get().WhereEquals(nameof(ConsentAgreementInfo.ConsentAgreementGuid), kx13ConsentAgreement.ConsentAgreementGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = consentAgreementMapper.Map(kx13ConsentAgreement, kxoConsentAgreement);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var agreementInfo, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(agreementInfo, nameof(agreementInfo));

                if (newInstance)
                {
                    consentAgreementNews.Add(agreementInfo);
                }
                else
                {
                    ConsentAgreementInfo.Provider.Set(agreementInfo);
                }
            }

            index++;
            indexFull++;

            if (index == batchSize || indexFull == itemsCount)
            {
                try
                {
                    ConsentAgreementInfo.Provider.BulkInsert(consentAgreementNews);
                    foreach (var newKx13ConsentAgreement in consentAgreementNews)
                    {
                        protocol.Success(kx13ConsentAgreement, newKx13ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newKx13ConsentAgreement.ConsentAgreementGuid);
                    }
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
