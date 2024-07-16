using CMS.DataEngine;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KX13.Context;

namespace Migration.Toolkit.Core.KX13.Handlers;

public class MigrateSettingKeysCommandHandler : IRequestHandler<MigrateSettingKeysCommand, CommandResult>
{
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ILogger<MigrateSettingKeysCommandHandler> _logger;
    private readonly IEntityMapper<KX13M.CmsSettingsKey, SettingsKeyInfo> _mapper;
    private readonly IProtocol _protocol;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    public MigrateSettingKeysCommandHandler(
        ILogger<MigrateSettingKeysCommandHandler> logger,
        IEntityMapper<KX13M.CmsSettingsKey, SettingsKeyInfo> mapper,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        IProtocol protocol
    )
    {
        _logger = logger;
        _mapper = mapper;
        _kx13ContextFactory = kx13ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _protocol = protocol;
    }

    public async Task<CommandResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<KX13M.CmsSettingsKey>();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogInformation("CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = kx13Context.CmsSettingsKeys
                .Where(csk => csk.SiteId == null)
                .AsNoTrackingWithIdentityResolution()
            ;

        foreach (var kx13CmsSettingsKey in cmsSettingsKeys)
        {
            _protocol.FetchedSource(kx13CmsSettingsKey);

            var kxoGlobalSettingsKey = GetKxoSettingsKey(kx13CmsSettingsKey);

            bool canBeMigrated = !kxoGlobalSettingsKey?.KeyIsHidden ?? false;
            var kxoCmsSettingsKey = kx13CmsSettingsKey.SiteId is null ? kxoGlobalSettingsKey : GetKxoSettingsKey(kx13CmsSettingsKey);

            if (!canBeMigrated)
            {
                _logger.LogInformation("Setting with key '{KeyName}' is currently not supported for migration", kx13CmsSettingsKey.KeyName);
                _protocol.Append(
                    HandbookReferences
                        .NotCurrentlySupportedSkip<SettingsKeyInfo>()
                        .WithId(nameof(kx13CmsSettingsKey.KeyId), kx13CmsSettingsKey.KeyId)
                        .WithMessage("Settings key is not supported in target instance")
                        .WithData(new { kx13CmsSettingsKey.KeyName, kx13CmsSettingsKey.SiteId, kx13CmsSettingsKey.KeyGuid })
                );
                continue;
            }

            _protocol.FetchedTarget(kxoCmsSettingsKey);

            if (entityConfiguration.ExcludeCodeNames.Contains(kx13CmsSettingsKey.KeyName))
            {
                _protocol.Warning(HandbookReferences.CmsSettingsKeyExclusionListSkip, kx13CmsSettingsKey);
                _logger.LogWarning("KeyName {KeyName} is excluded => skipping", kx13CmsSettingsKey.KeyName);
                continue;
            }

            var mapped = _mapper.Map(kx13CmsSettingsKey, kxoCmsSettingsKey);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                ArgumentNullException.ThrowIfNull(result.Item, nameof(result.Item));

                SettingsKeyInfoProvider.ProviderObject.Set(result.Item);

                _protocol.Success(kx13CmsSettingsKey, kxoCmsSettingsKey, mapped);
                _logger.LogEntitySetAction(result.NewInstance, result.Item);
            }
        }

        return new GenericCommandResult();
    }

    private SettingsKeyInfo? GetKxoSettingsKey(KX13M.CmsSettingsKey kx13CmsSettingsKey) => SettingsKeyInfoProvider.ProviderObject.Get(kx13CmsSettingsKey.KeyName);
}
