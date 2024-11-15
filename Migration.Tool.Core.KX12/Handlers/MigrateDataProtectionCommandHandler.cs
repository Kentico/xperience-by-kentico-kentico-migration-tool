using CMS.DataEngine;
using CMS.DataProtection;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.KX12.Context;

namespace Migration.Tool.Core.KX12.Handlers;

public class MigrateDataProtectionCommandHandler : IRequestHandler<MigrateDataProtectionCommand, CommandResult>
{
    private static readonly int batchSize = 1000;
    private readonly IEntityMapper<KX12M.CmsConsentAgreement, ConsentAgreementInfo> consentAgreementMapper;
    private readonly IEntityMapper<KX12M.CmsConsentArchive, ConsentArchiveInfo> consentArchiveMapper;
    private readonly IEntityMapper<KX12M.CmsConsent, ConsentInfo> consentMapper;
    private readonly IDbContextFactory<KX12Context> kx12ContextFactory;
    private readonly ILogger<MigrateDataProtectionCommandHandler> logger;
    private readonly PrimaryKeyMappingContext primaryKeyMappingContext;
    private readonly IProtocol protocol;

    public MigrateDataProtectionCommandHandler(
        ILogger<MigrateDataProtectionCommandHandler> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        IEntityMapper<KX12M.CmsConsent, ConsentInfo> consentMapper,
        IEntityMapper<KX12M.CmsConsentArchive, ConsentArchiveInfo> consentArchiveMapper,
        IEntityMapper<KX12M.CmsConsentAgreement, ConsentAgreementInfo> consentAgreementMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        this.logger = logger;
        this.kx12ContextFactory = kx12ContextFactory;
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
        await using var kx12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k12Consent in kx12Context.CmsConsents)
        {
            protocol.FetchedSource(k12Consent);
            logger.LogTrace("Migrating consent {ConsentName} with ConsentGuid {ConsentGuid}", k12Consent.ConsentName, k12Consent.ConsentGuid);

            var kxoConsent = ConsentInfo.Provider.Get().WhereEquals(nameof(ConsentInfo.ConsentGuid), k12Consent.ConsentGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsent);

            var mapped = consentMapper.Map(k12Consent, kxoConsent);
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

                    protocol.Success(k12Consent, consentInfo, mapped);
                    logger.LogEntitySetAction(newInstance, consentInfo);
                    primaryKeyMappingContext.SetMapping<KX12M.CmsConsent>(r => r.ConsentId, k12Consent.ConsentId, consentInfo.ConsentID);
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
        await using var kx12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k12ArchiveConsent in kx12Context.CmsConsentArchives)
        {
            protocol.FetchedSource(k12ArchiveConsent);
            logger.LogTrace("Migrating consent archive with ConsentArchiveGuid {ConsentGuid}", k12ArchiveConsent.ConsentArchiveGuid);

            var kxoConsentArchive = ConsentArchiveInfo.Provider.Get().WhereEquals(nameof(ConsentArchiveInfo.ConsentArchiveGuid), k12ArchiveConsent.ConsentArchiveGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsentArchive);

            var mapped = consentArchiveMapper.Map(k12ArchiveConsent, kxoConsentArchive);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var consentArchiveInfo, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(consentArchiveInfo, nameof(consentArchiveInfo));

                ConsentArchiveInfo.Provider.Set(consentArchiveInfo);
                protocol.Success(k12ArchiveConsent, consentArchiveInfo, mapped);
                logger.LogEntitySetAction(newInstance, consentArchiveInfo);
                primaryKeyMappingContext.SetMapping<ConsentArchiveInfo>(r => r.ConsentArchiveGuid,
                k12ArchiveConsent.ConsentArchiveId, consentArchiveInfo.ConsentArchiveID);
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentAgreement(CancellationToken cancellationToken, int batchSize)
    {
        await using var kx12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<ConsentAgreementInfo>();
        var consentAgreementNews = new List<ConsentAgreementInfo>();
        int itemsCount = kx12Context.CmsConsentAgreements.Count();

        foreach (var k12ConsentAgreement in kx12Context.CmsConsentAgreements)
        {
            protocol.FetchedSource(k12ConsentAgreement);
            logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", k12ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = ConsentAgreementInfo.Provider.Get().WhereEquals(nameof(ConsentAgreementInfo.ConsentAgreementGuid), k12ConsentAgreement.ConsentAgreementGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = consentAgreementMapper.Map(k12ConsentAgreement, kxoConsentAgreement);
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
                    foreach (var newK12ConsentAgreement in consentAgreementNews)
                    {
                        protocol.Success(k12ConsentAgreement, newK12ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newK12ConsentAgreement.ConsentAgreementGuid);
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
