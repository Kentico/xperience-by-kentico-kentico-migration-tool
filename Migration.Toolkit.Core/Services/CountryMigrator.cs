namespace Migration.Toolkit.Core.Services;

using CMS.Globalization;
using CMS.MediaLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.KXP.Context;

public class CountryMigrator
{
    private readonly ILogger<CountryMigrator> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;
    private readonly IEntityMapper<CmsCountry, CountryInfo> _countryMapper;
    private readonly IEntityMapper<CmsState, StateInfo> _stateMapper;
    private readonly KxpApiInitializer _kxpApiInitializer;

    public CountryMigrator(
        ILogger<CountryMigrator> logger,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        IEntityMapper<KX13M.CmsCountry, CountryInfo> countryMapper,
        IEntityMapper<KX13M.CmsState, StateInfo> stateMapper,
        KxpApiInitializer kxpApiInitializer
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
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
        
        var kx13Context = _kx13ContextFactory.CreateDbContext();

        var kx13Countries = kx13Context.CmsCountries.AsNoTracking();
        foreach (var kx13CmsCountry in kx13Countries)
        {
            var kxpCountryInfo = CountryInfoProvider.ProviderObject.Get(kx13CmsCountry.CountryName);

            if (kxpCountryInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = _countryMapper.Map(kx13CmsCountry, null);
            _protocol.MappedTarget(mapped);

            if (mapped is (var countryInfo, var newInstance) { Success: true })
            {
                try
                {
                    CountryInfoProvider.ProviderObject.Set(countryInfo);

                    _protocol.Success(kx13CmsCountry, countryInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, countryInfo);
                    
                    _primaryKeyMappingContext.SetMapping<KX13M.CmsCountry>(r => r.CountryId, kx13CmsCountry.CountryId, countryInfo.CountryID);
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

        
        var kx13States = kx13Context.CmsStates.AsNoTracking();
        foreach (var kx13CmsState in kx13States)
        {
            var kxpStateInfo = StateInfoProvider.ProviderObject.Get(kx13CmsState.StateName);

            if (kxpStateInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = _stateMapper.Map(kx13CmsState, null);
            _protocol.MappedTarget(mapped);

            if (mapped is (var stateInfo, var newInstance) { Success: true })
            {
                try
                {
                    StateInfoProvider.ProviderObject.Set(stateInfo);

                    _protocol.Success(kx13CmsState, stateInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, stateInfo);
                    
                    _primaryKeyMappingContext.SetMapping<KX13M.CmsState>(r => r.StateId, kx13CmsState.StateId, stateInfo.StateID);
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