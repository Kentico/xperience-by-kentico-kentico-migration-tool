namespace Migration.Toolkit.Core.KX12.Services;

using CMS.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KX12.Models;
using Migration.Toolkit.KXP.Api;

public class CountryMigrator
{
    private readonly ILogger<CountryMigrator> _logger;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;
    private readonly IEntityMapper<CmsCountry, CountryInfo> _countryMapper;
    private readonly IEntityMapper<CmsState, StateInfo> _stateMapper;
    private readonly KxpApiInitializer _kxpApiInitializer;

    public CountryMigrator(
        ILogger<CountryMigrator> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        IEntityMapper<KX12M.CmsCountry, CountryInfo> countryMapper,
        IEntityMapper<KX12M.CmsState, StateInfo> stateMapper,
        KxpApiInitializer kxpApiInitializer
    )
    {
        _logger = logger;
        _kx12ContextFactory = kx12ContextFactory;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _countryMapper = countryMapper;
        _stateMapper = stateMapper;
        _kxpApiInitializer = kxpApiInitializer;
    }

    public void MigrateCountriesAndStates()
    {
        if (!_kxpApiInitializer.EnsureApiIsInitialized())
        {
            throw new InvalidOperationException($"Falied to initialize kentico API. Please check configuration.");
        }

        var kx12Context = _kx12ContextFactory.CreateDbContext();

        var k12Countries = kx12Context.CmsCountries.AsNoTracking();
        foreach (var k12CmsCountry in k12Countries)
        {
            var kxpCountryInfo = CountryInfoProvider.ProviderObject.Get(k12CmsCountry.CountryName);

            if (kxpCountryInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = _countryMapper.Map(k12CmsCountry, null);
            _protocol.MappedTarget(mapped);

            if (mapped is (var countryInfo, var newInstance) { Success: true })
            {
                try
                {
                    CountryInfoProvider.ProviderObject.Set(countryInfo);

                    _protocol.Success(k12CmsCountry, countryInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, countryInfo);

                    _primaryKeyMappingContext.SetMapping<KX12M.CmsCountry>(r => r.CountryId, k12CmsCountry.CountryId, countryInfo.CountryID);
                }
                catch (Exception exception)
                {
                    _logger.LogEntitySetError(exception, newInstance, countryInfo);
                    _protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<CountryInfo>(exception)
                        .NeedsManualAction()
                        .WithIdentityPrint(countryInfo)
                    );
                }
            }
        }


        var k12States = kx12Context.CmsStates.AsNoTracking();
        foreach (var k12CmsState in k12States)
        {
            var kxpStateInfo = StateInfoProvider.ProviderObject.Get(k12CmsState.StateName);

            if (kxpStateInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = _stateMapper.Map(k12CmsState, null);
            _protocol.MappedTarget(mapped);

            if (mapped is (var stateInfo, var newInstance) { Success: true })
            {
                try
                {
                    StateInfoProvider.ProviderObject.Set(stateInfo);

                    _protocol.Success(k12CmsState, stateInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, stateInfo);

                    _primaryKeyMappingContext.SetMapping<KX12M.CmsState>(r => r.StateId, k12CmsState.StateId, stateInfo.StateID);
                }
                catch (Exception exception)
                {
                    _logger.LogEntitySetError(exception, newInstance, stateInfo);
                    _protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<StateInfo>(exception)
                        .NeedsManualAction()
                        .WithIdentityPrint(stateInfo)
                    );
                }
            }
        }
    }
}