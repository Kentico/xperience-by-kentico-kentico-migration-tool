using CMS.DataEngine;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KX12.Context;

namespace Migration.Toolkit.Core.KX12.Handlers;

public class MigrateSettingKeysCommandHandler(
    ILogger<MigrateSettingKeysCommandHandler> logger,
    IEntityMapper<KX12M.CmsSettingsKey, SettingsKeyInfo> mapper,
    IDbContextFactory<KX12Context> kx12ContextFactory,
    ToolkitConfiguration toolkitConfiguration,
    IProtocol protocol)
    : IRequestHandler<MigrateSettingKeysCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<KX12M.CmsSettingsKey>();

        await using var kx12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = kx12Context.CmsSettingsKeys
                .Where(csk => csk.SiteId == null)
                .AsNoTrackingWithIdentityResolution()
            ;

        foreach (var k12CmsSettingsKey in cmsSettingsKeys)
        {
            protocol.FetchedSource(k12CmsSettingsKey);

            var kxoGlobalSettingsKey = GetKxoSettingsKey(k12CmsSettingsKey);

            bool canBeMigrated = !kxoGlobalSettingsKey?.KeyIsHidden ?? false;
            var kxoCmsSettingsKey = k12CmsSettingsKey.SiteId is null ? kxoGlobalSettingsKey : GetKxoSettingsKey(k12CmsSettingsKey);

            if (!canBeMigrated)
            {
                logger.LogInformation("Setting with key '{KeyName}' is currently not supported for migration", k12CmsSettingsKey.KeyName);
                protocol.Append(
                    HandbookReferences
                        .NotCurrentlySupportedSkip<SettingsKeyInfo>()
                        .WithId(nameof(k12CmsSettingsKey.KeyId), k12CmsSettingsKey.KeyId)
                        .WithMessage("Settings key is not supported in target instance")
                        .WithData(new { k12CmsSettingsKey.KeyName, k12CmsSettingsKey.SiteId, k12CmsSettingsKey.KeyGuid })
                );
                continue;
            }

            protocol.FetchedTarget(kxoCmsSettingsKey);

            if (entityConfiguration.ExcludeCodeNames.Contains(k12CmsSettingsKey.KeyName))
            {
                protocol.Warning(HandbookReferences.CmsSettingsKeyExclusionListSkip, k12CmsSettingsKey);
                logger.LogWarning("KeyName {KeyName} is excluded => skipping", k12CmsSettingsKey.KeyName);
                continue;
            }

            var mapped = mapper.Map(k12CmsSettingsKey, kxoCmsSettingsKey);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                ArgumentNullException.ThrowIfNull(result.Item, nameof(result.Item));

                SettingsKeyInfoProvider.ProviderObject.Set(result.Item);

                protocol.Success(k12CmsSettingsKey, kxoCmsSettingsKey, mapped);
                logger.LogEntitySetAction(result.NewInstance, result.Item);
            }
        }

        return new GenericCommandResult();
    }

    private SettingsKeyInfo? GetKxoSettingsKey(KX12M.CmsSettingsKey k12CmsSettingsKey) => SettingsKeyInfoProvider.ProviderObject.Get(k12CmsSettingsKey.KeyName);
}
