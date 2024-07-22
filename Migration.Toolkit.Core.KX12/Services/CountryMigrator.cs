using CMS.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KXP.Api;

namespace Migration.Toolkit.Core.KX12.Services;

public class CountryMigrator(
    ILogger<CountryMigrator> logger,
    IDbContextFactory<KX12Context> kx12ContextFactory,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    IEntityMapper<KX12M.CmsCountry, CountryInfo> countryMapper,
    IEntityMapper<KX12M.CmsState, StateInfo> stateMapper,
    KxpApiInitializer kxpApiInitializer)
{
    public void MigrateCountriesAndStates()
    {
        if (!kxpApiInitializer.EnsureApiIsInitialized())
        {
            throw new InvalidOperationException("Falied to initialize kentico API. Please check configuration.");
        }

        var kx12Context = kx12ContextFactory.CreateDbContext();

        var k12Countries = kx12Context.CmsCountries.AsNoTracking();
        foreach (var k12CmsCountry in k12Countries)
        {
            var kxpCountryInfo = CountryInfoProvider.ProviderObject.Get(k12CmsCountry.CountryName);

            if (kxpCountryInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = countryMapper.Map(k12CmsCountry, null);
            protocol.MappedTarget(mapped);

            if (mapped is (var countryInfo, var newInstance) { Success: true })
            {
                try
                {
                    CountryInfoProvider.ProviderObject.Set(countryInfo);

                    protocol.Success(k12CmsCountry, countryInfo, mapped);
                    logger.LogEntitySetAction(newInstance, countryInfo);

                    primaryKeyMappingContext.SetMapping<KX12M.CmsCountry>(r => r.CountryId, k12CmsCountry.CountryId, countryInfo.CountryID);
                }
                catch (Exception exception)
                {
                    logger.LogEntitySetError(exception, newInstance, countryInfo);
                    protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<CountryInfo>(exception)
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

            var mapped = stateMapper.Map(k12CmsState, null);
            protocol.MappedTarget(mapped);

            if (mapped is (var stateInfo, var newInstance) { Success: true })
            {
                try
                {
                    StateInfoProvider.ProviderObject.Set(stateInfo);

                    protocol.Success(k12CmsState, stateInfo, mapped);
                    logger.LogEntitySetAction(newInstance, stateInfo);

                    primaryKeyMappingContext.SetMapping<KX12M.CmsState>(r => r.StateId, k12CmsState.StateId, stateInfo.StateID);
                }
                catch (Exception exception)
                {
                    logger.LogEntitySetError(exception, newInstance, stateInfo);
                    protocol.Append(HandbookReferences.ErrorCreatingTargetInstance<StateInfo>(exception)
                        .NeedsManualAction()
                        .WithIdentityPrint(stateInfo)
                    );
                }
            }
        }
    }
}
