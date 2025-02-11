using CMS.DataEngine;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.K11;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Handlers;

public class MigrateSettingKeysCommandHandler(
    ILogger<MigrateSettingKeysCommandHandler> logger,
    IEntityMapper<CmsSettingsKey, SettingsKeyInfo> mapper,
    IDbContextFactory<K11Context> k11ContextFactory,
    ToolConfiguration toolConfiguration,
    IProtocol protocol)
    : IRequestHandler<MigrateSettingKeysCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = toolConfiguration.EntityConfigurations.GetEntityConfiguration<CmsSettingsKey>();

        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = k11Context.CmsSettingsKeys
                .Where(csk => csk.SiteId == null)
                .AsNoTrackingWithIdentityResolution()
            ;

        foreach (var k11CmsSettingsKey in cmsSettingsKeys)
        {
            protocol.FetchedSource(k11CmsSettingsKey);

            var kxoGlobalSettingsKey = GetKxoSettingsKey(k11CmsSettingsKey);

            bool canBeMigrated = !kxoGlobalSettingsKey?.KeyIsHidden ?? false;
            var kxoCmsSettingsKey = k11CmsSettingsKey.SiteId is null ? kxoGlobalSettingsKey : GetKxoSettingsKey(k11CmsSettingsKey);

            if (!canBeMigrated)
            {
                logger.LogInformation("Setting with key '{KeyName}' is currently not supported for migration", k11CmsSettingsKey.KeyName);
                protocol.Append(
                    HandbookReferences
                        .NotCurrentlySupportedSkip<SettingsKeyInfo>()
                        .WithId(nameof(k11CmsSettingsKey.KeyId), k11CmsSettingsKey.KeyId)
                        .WithMessage("Settings key is not supported in target instance")
                        .WithData(new { k11CmsSettingsKey.KeyName, k11CmsSettingsKey.SiteId, k11CmsSettingsKey.KeyGuid })
                );
                continue;
            }

            protocol.FetchedTarget(kxoCmsSettingsKey);

            if (entityConfiguration.ExcludeCodeNames.Contains(k11CmsSettingsKey.KeyName))
            {
                protocol.Warning(HandbookReferences.CmsSettingsKeyExclusionListSkip, k11CmsSettingsKey);
                logger.LogWarning("KeyName {KeyName} is excluded => skipping", k11CmsSettingsKey.KeyName);
                continue;
            }

            var mapped = mapper.Map(k11CmsSettingsKey, kxoCmsSettingsKey);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                ArgumentNullException.ThrowIfNull(result.Item, nameof(result.Item));

                SettingsKeyInfoProvider.ProviderObject.Set(result.Item);

                protocol.Success(k11CmsSettingsKey, kxoCmsSettingsKey, mapped);
                logger.LogEntitySetAction(result.NewInstance, result.Item);
            }
        }

        return new GenericCommandResult();
    }

    private SettingsKeyInfo? GetKxoSettingsKey(CmsSettingsKey k11CmsSettingsKey) => SettingsKeyInfoProvider.ProviderObject.Get(k11CmsSettingsKey.KeyName);
}
