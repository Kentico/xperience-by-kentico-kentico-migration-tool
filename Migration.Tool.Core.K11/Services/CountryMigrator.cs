using CMS.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11;
using Migration.Tool.K11.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.K11.Services;

public class CountryMigrator(
    ILogger<CountryMigrator> logger,
    IDbContextFactory<K11Context> k11ContextFactory,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    IEntityMapper<CmsCountry, CountryInfo> countryMapper,
    IEntityMapper<CmsState, StateInfo> stateMapper,
    KxpApiInitializer kxpApiInitializer)
{
    public void MigrateCountriesAndStates()
    {
        if (!kxpApiInitializer.EnsureApiIsInitialized())
        {
            throw new InvalidOperationException("Failed to initialize Kentico API. Please check configuration.");
        }

        var k11Context = k11ContextFactory.CreateDbContext();

        var k11Countries = k11Context.CmsCountries.AsNoTracking();
        foreach (var k11CmsCountry in k11Countries)
        {
            var kxpCountryInfo = CountryInfoProvider.ProviderObject.Get(k11CmsCountry.CountryName);

            if (kxpCountryInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = countryMapper.Map(k11CmsCountry, null);
            protocol.MappedTarget(mapped);

            if (mapped is (var countryInfo, var newInstance) { Success: true, Item: not null })
            {
                try
                {
                    CountryInfoProvider.ProviderObject.Set(countryInfo);

                    protocol.Success(k11CmsCountry, countryInfo, mapped);
                    logger.LogEntitySetAction(newInstance, countryInfo);

                    primaryKeyMappingContext.SetMapping<CmsCountry>(r => r.CountryId, k11CmsCountry.CountryId, countryInfo!.CountryID);
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


        var k11States = k11Context.CmsStates.AsNoTracking();
        foreach (var k11CmsState in k11States)
        {
            var kxpStateInfo = StateInfoProvider.ProviderObject.Get(k11CmsState.StateName);

            if (kxpStateInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = stateMapper.Map(k11CmsState, null);
            protocol.MappedTarget(mapped);

            if (mapped is (var stateInfo, var newInstance) { Success: true, Item: not null })
            {
                try
                {
                    StateInfoProvider.ProviderObject.Set(stateInfo);

                    protocol.Success(k11CmsState, stateInfo, mapped);
                    logger.LogEntitySetAction(newInstance, stateInfo);

                    primaryKeyMappingContext.SetMapping<CmsState>(r => r.StateId, k11CmsState.StateId, stateInfo!.StateID);
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
