using CMS.DataEngine;
using CMS.DataProtection;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Handlers;

public class MigrateDataProtectionCommandHandler(
    ILogger<MigrateDataProtectionCommandHandler> logger,
    IDbContextFactory<K11Context> k11ContextFactory,
    IEntityMapper<CmsConsentAgreement, ConsentAgreementInfo> consentAgreementMapper,
    IEntityMapper<CmsConsentArchive, ConsentArchiveInfo> consentArchiveMapper,
    IEntityMapper<CmsConsent, ConsentInfo> consentMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol
    ) : IRequestHandler<MigrateDataProtectionCommand, CommandResult>
{
    private static readonly int batchSize = 1000;

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

            var kxoConsent = ConsentInfo.Provider.Get().WhereEquals(nameof(ConsentInfo.ConsentGuid), k11Consent.ConsentGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsent);

            var mapped = consentMapper.Map(k11Consent, kxoConsent);
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

                    protocol.Success(k11Consent, consentInfo, mapped);
                    logger.LogEntitySetAction(newInstance, consentInfo);
                    primaryKeyMappingContext.SetMapping<CmsConsent>(r => r.ConsentId, k11Consent.ConsentId, consentInfo.ConsentID);
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
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var k11ArchiveConsent in k11Context.CmsConsentArchives)
        {
            protocol.FetchedSource(k11ArchiveConsent);
            logger.LogTrace("Migrating consent archive with ConsentArchiveGuid {ConsentGuid}", k11ArchiveConsent.ConsentArchiveGuid);

            var kxoConsentArchive = ConsentArchiveInfo.Provider.Get().WhereEquals(nameof(ConsentArchiveInfo.ConsentArchiveGuid), k11ArchiveConsent.ConsentArchiveGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsentArchive);

            var mapped = consentArchiveMapper.Map(k11ArchiveConsent, kxoConsentArchive);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                (var consentArchiveInfo, bool newInstance) = result;
                ArgumentNullException.ThrowIfNull(consentArchiveInfo, nameof(consentArchiveInfo));

                ConsentArchiveInfo.Provider.Set(consentArchiveInfo);
                protocol.Success(k11ArchiveConsent, consentArchiveInfo, mapped);
                logger.LogEntitySetAction(newInstance, consentArchiveInfo);
                primaryKeyMappingContext.SetMapping<CmsConsentArchive>(r => r.ConsentArchiveGuid,
                k11ArchiveConsent.ConsentArchiveId, consentArchiveInfo.ConsentArchiveID);
            }
        }

        return new GenericCommandResult();
    }

    private async Task<CommandResult> MigrateConsentAgreement(CancellationToken cancellationToken, int batchSize)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);
        int index = 0;
        int indexFull = 0;
        var consentAgreementUpdates = new List<ConsentAgreementInfo>();
        var consentAgreementNews = new List<ConsentAgreementInfo>();
        int itemsCount = k11Context.CmsConsentAgreements.Count();

        foreach (var k11ConsentAgreement in k11Context.CmsConsentAgreements)
        {
            protocol.FetchedSource(k11ConsentAgreement);
            logger.LogTrace("Migrating consent agreement with ConsentAgreementGuid {ConsentAgreementGuid}", k11ConsentAgreement.ConsentAgreementGuid);

            var kxoConsentAgreement = ConsentAgreementInfo.Provider.Get().WhereEquals(nameof(ConsentAgreementInfo.ConsentAgreementGuid), k11ConsentAgreement.ConsentAgreementGuid).FirstOrDefault();
            protocol.FetchedTarget(kxoConsentAgreement);

            var mapped = consentAgreementMapper.Map(k11ConsentAgreement, kxoConsentAgreement);
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
                    foreach (var newK11ConsentAgreement in consentAgreementNews)
                    {
                        protocol.Success(k11ConsentAgreement, newK11ConsentAgreement, mapped);
                        logger.LogDebug("CmsConsentAgreement: with ConsentAgreementGuid \'{ConsentAgreementGuid}\' was inserted",
                            newK11ConsentAgreement.ConsentAgreementGuid);
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
