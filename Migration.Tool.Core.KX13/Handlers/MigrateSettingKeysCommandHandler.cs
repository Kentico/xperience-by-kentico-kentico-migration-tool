using CMS.DataEngine;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.KX13.Context;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateSettingKeysCommandHandler(
    ILogger<MigrateSettingKeysCommandHandler> logger,
    IEntityMapper<KX13M.CmsSettingsKey, SettingsKeyInfo> mapper,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    ToolConfiguration toolConfiguration,
    IProtocol protocol)
    : IRequestHandler<MigrateSettingKeysCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<KX13M.CmsSettingsKey>();

        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = kx13Context.CmsSettingsKeys
                .Where(csk => csk.SiteId == null)
                .AsNoTrackingWithIdentityResolution()
            ;

        foreach (var kx13CmsSettingsKey in cmsSettingsKeys)
        {
            protocol.FetchedSource(kx13CmsSettingsKey);

            var kxoGlobalSettingsKey = GetKxoSettingsKey(kx13CmsSettingsKey);

            bool canBeMigrated = !kxoGlobalSettingsKey?.KeyIsHidden ?? false;
            var kxoCmsSettingsKey = kx13CmsSettingsKey.SiteId is null ? kxoGlobalSettingsKey : GetKxoSettingsKey(kx13CmsSettingsKey);

            if (!canBeMigrated)
            {
                logger.LogInformation("Setting with key '{KeyName}' is currently not supported for migration", kx13CmsSettingsKey.KeyName);
                protocol.Append(
                    HandbookReferences
                        .NotCurrentlySupportedSkip<SettingsKeyInfo>()
                        .WithId(nameof(kx13CmsSettingsKey.KeyId), kx13CmsSettingsKey.KeyId)
                        .WithMessage("Settings key is not supported in target instance")
                        .WithData(new { kx13CmsSettingsKey.KeyName, kx13CmsSettingsKey.SiteId, kx13CmsSettingsKey.KeyGuid })
                );
                continue;
            }

            protocol.FetchedTarget(kxoCmsSettingsKey);

            if (entityConfiguration.ExcludeCodeNames.Contains(kx13CmsSettingsKey.KeyName))
            {
                protocol.Warning(HandbookReferences.CmsSettingsKeyExclusionListSkip, kx13CmsSettingsKey);
                logger.LogWarning("KeyName {KeyName} is excluded => skipping", kx13CmsSettingsKey.KeyName);
                continue;
            }

            var mapped = mapper.Map(kx13CmsSettingsKey, kxoCmsSettingsKey);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                ArgumentNullException.ThrowIfNull(result.Item, nameof(result.Item));

                SettingsKeyInfoProvider.ProviderObject.Set(result.Item);

                protocol.Success(kx13CmsSettingsKey, kxoCmsSettingsKey, mapped);
                logger.LogEntitySetAction(result.NewInstance, result.Item);
            }
        }

        return new GenericCommandResult();
    }

    private SettingsKeyInfo? GetKxoSettingsKey(KX13M.CmsSettingsKey kx13CmsSettingsKey) => SettingsKeyInfoProvider.ProviderObject.Get(kx13CmsSettingsKey.KeyName);
}
