using CMS.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Context;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Services;

public class CountryMigrator(
    ILogger<CountryMigrator> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    IEntityMapper<KX13M.CmsCountry, CountryInfo> countryMapper,
    IEntityMapper<KX13M.CmsState, StateInfo> stateMapper,
    KxpApiInitializer kxpApiInitializer)
{
    public void MigrateCountriesAndStates()
    {
        if (!kxpApiInitializer.EnsureApiIsInitialized())
        {
            throw new InvalidOperationException("Failed to initialize Kentico API. Please check configuration.");
        }

        var kx13Context = kx13ContextFactory.CreateDbContext();

        var kx13Countries = kx13Context.CmsCountries.AsNoTracking();
        foreach (var kx13CmsCountry in kx13Countries)
        {
            var kxpCountryInfo = CountryInfoProvider.ProviderObject.Get(kx13CmsCountry.CountryName);

            if (kxpCountryInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = countryMapper.Map(kx13CmsCountry, null);
            protocol.MappedTarget(mapped);

            if (mapped is (var countryInfo, var newInstance) { Success: true, Item: not null })
            {
                try
                {
                    CountryInfoProvider.ProviderObject.Set(countryInfo);

                    protocol.Success(kx13CmsCountry, countryInfo, mapped);
                    logger.LogEntitySetAction(newInstance, countryInfo);

                    primaryKeyMappingContext.SetMapping<KX13M.CmsCountry>(r => r.CountryId, kx13CmsCountry.CountryId, countryInfo!.CountryID);
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


        var kx13States = kx13Context.CmsStates.AsNoTracking();
        foreach (var kx13CmsState in kx13States)
        {
            var kxpStateInfo = StateInfoProvider.ProviderObject.Get(kx13CmsState.StateName);

            if (kxpStateInfo != null) // do not update when exists
            {
                continue;
            }

            var mapped = stateMapper.Map(kx13CmsState, null);
            protocol.MappedTarget(mapped);

            if (mapped is (var stateInfo, var newInstance) { Success: true, Item: not null })
            {
                try
                {
                    StateInfoProvider.ProviderObject.Set(stateInfo);

                    protocol.Success(kx13CmsState, stateInfo, mapped);
                    logger.LogEntitySetAction(newInstance, stateInfo);

                    primaryKeyMappingContext.SetMapping<KX13M.CmsState>(r => r.StateId, kx13CmsState.StateId, stateInfo!.StateID);
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
