namespace Migration.Toolkit.Core.KX12.Handlers;

using CMS.DataEngine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;

public class MigrateSettingKeysCommandHandler: IRequestHandler<MigrateSettingKeysCommand, CommandResult>
{
    private readonly ILogger<MigrateSettingKeysCommandHandler> _logger;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly IProtocol _protocol;
    private readonly IEntityMapper<CmsSettingsKey, SettingsKeyInfo> _mapper;

    public MigrateSettingKeysCommandHandler(
        ILogger<MigrateSettingKeysCommandHandler> logger,
        IEntityMapper<CmsSettingsKey, SettingsKeyInfo> mapper,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        IProtocol protocol
        )
    {
        _logger = logger;
        _mapper = mapper;
        _kx12ContextFactory = kx12ContextFactory;
        _toolkitConfiguration = toolkitConfiguration;
        _protocol = protocol;
    }

    public async Task<CommandResult> Handle(MigrateSettingKeysCommand request, CancellationToken cancellationToken)
    {
        var entityConfiguration = _toolkitConfiguration.EntityConfigurations.GetEntityConfiguration<CmsSettingsKey>();

        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        _logger.LogInformation("CmsSettingsKey synchronization starting");
        var cmsSettingsKeys = kx12Context.CmsSettingsKeys
                .Where(csk => csk.SiteId == null)
                .AsNoTrackingWithIdentityResolution()
            ;

        foreach (var k12CmsSettingsKey in cmsSettingsKeys)
        {
            _protocol.FetchedSource(k12CmsSettingsKey);

            var kxoGlobalSettingsKey = GetKxoSettingsKey(k12CmsSettingsKey);

            var canBeMigrated = !kxoGlobalSettingsKey?.KeyIsHidden ?? false;
            var kxoCmsSettingsKey = k12CmsSettingsKey.SiteId is null ? kxoGlobalSettingsKey : GetKxoSettingsKey(k12CmsSettingsKey);

            if (!canBeMigrated)
            {
                _logger.LogWarning("Setting with key '{KeyName}' is currently not supported for migration", k12CmsSettingsKey.KeyName);
                _protocol.Append(
                    HandbookReferences
                        .NotCurrentlySupportedSkip<SettingsKeyInfo>()
                        .WithId(nameof(k12CmsSettingsKey.KeyId), k12CmsSettingsKey.KeyId)
                        .WithMessage("Settings key is not supported in target instance")
                        .WithData(new
                        {
                            k12CmsSettingsKey.KeyName,
                            k12CmsSettingsKey.SiteId,
                            k12CmsSettingsKey.KeyGuid
                        })
                );
                continue;
            }

            _protocol.FetchedTarget(kxoCmsSettingsKey);

            if (entityConfiguration.ExcludeCodeNames.Contains(k12CmsSettingsKey.KeyName))
            {
                _protocol.Warning(HandbookReferences.CmsSettingsKeyExclusionListSkip, k12CmsSettingsKey);
                _logger.LogWarning("KeyName {KeyName} is excluded => skipping", k12CmsSettingsKey.KeyName);
                continue;
            }

            var mapped = _mapper.Map(k12CmsSettingsKey, kxoCmsSettingsKey);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true } result)
            {
                ArgumentNullException.ThrowIfNull(result.Item, nameof(result.Item));

                SettingsKeyInfoProvider.ProviderObject.Set(result.Item);

                _protocol.Success(k12CmsSettingsKey, kxoCmsSettingsKey, mapped);
                _logger.LogEntitySetAction(result.NewInstance, result.Item);
            }
        }

        return new GenericCommandResult();
    }

    private SettingsKeyInfo? GetKxoSettingsKey(CmsSettingsKey k12CmsSettingsKey)
    {
        return SettingsKeyInfoProvider.ProviderObject.Get(k12CmsSettingsKey.KeyName);
    }
}