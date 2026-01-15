using CMS.Commerce;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Handlers.Base;
using Migration.Tool.Core.KX13.Mappers;
using Migration.Tool.KX13.Context;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateCustomersCommandHandler(
    ILogger<MigrateCustomersCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IEntityMapper<CustomerInfoMapperSource, CustomerInfo> customerInfoMapper,
    IEntityMapper<CustomerAddressInfoMapperSource, CustomerAddressInfo> customerAddressInfoMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    ToolConfiguration toolConfiguration,
    IFieldMigrationService fieldMigrationService,
    KxpClassFacade kxpClassFacade) :
    MigrateCommerceHandlerBase(logger, kx13ContextFactory, toolConfiguration, fieldMigrationService, kxpClassFacade),
    IRequestHandler<MigrateCustomersCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCustomersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await Kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13CommerceSites = await GetCommerceSites(kx13Context);
        var kx13CommerceSiteIds = kx13CommerceSites.Select(s => s.SiteId).ToHashSet();

        // Migrate custom and system fields from KX13 'ecommerce.customer' and 'ecommerce.address' classes to XbK counterparts
        await MigrateCustomerClass(cancellationToken);
        await MigrateCustomerAddressClass(cancellationToken);

        var kx13Customers = kx13Context.ComCustomers
            .Where(c => !c.CustomerSiteId.HasValue || kx13CommerceSiteIds.Contains(c.CustomerSiteId.Value))
            .Include(c => c.CustomerSite)
            .Include(c => c.ComAddresses)
            .Include(c => c.CustomerUser);

        foreach (var kx13Customer in kx13Customers)
        {
            logger.LogTrace("Migrating customer {CustomerId} with CustomerGuid {CustomerGuid}", kx13Customer.CustomerId, kx13Customer.CustomerGuid);

            var kx13User = kx13Customer.CustomerUser;
            var xbkCustomerInfo = CustomerInfo.Provider.Get(kx13Customer.CustomerGuid);

            MemberInfo? xbkMemberInfo = null;
            if (kx13User is not null)
            {
                xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(kx13User.UserGuid);
            }

            var sourceSiteName = kx13Customer.CustomerSite?.SiteName;
            var mapped = customerInfoMapper.Map(new CustomerInfoMapperSource(kx13Customer, sourceSiteName, xbkMemberInfo), xbkCustomerInfo);

            SaveCustomerUsingKenticoApi(mapped!, kx13User);

            if (mapped is { Success: true, Item: CustomerInfo customerInfo })
            {
                foreach (var kx13CustomerAddress in kx13Customer.ComAddresses.Where(a => a.AddressGuid.HasValue))
                {
                    var xbkCustomerAddress = CustomerAddressInfo.Provider.Get(kx13CustomerAddress.AddressGuid!.Value);
                    var mappedAddresses = customerAddressInfoMapper.Map(new CustomerAddressInfoMapperSource(kx13CustomerAddress, customerInfo), xbkCustomerAddress);
                    SaveCustomerAddressUsingKenticoApi(mappedAddresses!, kx13CustomerAddress);
                }
            }
        }

        return new GenericCommandResult();
    }

    private async Task MigrateCustomerClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_CUSTOMER_CLASS_NAME,
            targetClassName: CustomerInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: ToolConfiguration.CommerceConfiguration?.IncludeCustomerSystemFields,
            logEntityName: nameof(CustomerInfo),
            addSiteOriginField: true,
            addCurrencyField: false,
            cancellationToken
        );

    private async Task MigrateCustomerAddressClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_ADDRESS_CLASS_NAME,
            targetClassName: CustomerAddressInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: ToolConfiguration.CommerceConfiguration?.IncludeAddressSystemFields,
            logEntityName: nameof(CustomerAddressInfo),
            addSiteOriginField: false,
            addCurrencyField: false,
            cancellationToken
        );


    private void SaveCustomerUsingKenticoApi(IModelMappingResult<CustomerInfo> mapped, CmsUser? kx13User) =>
        SaveEntityUsingKenticoApi(
            mapped,
            CustomerInfo.Provider.Set,
            entity =>
            {
                if (kx13User is not null)
                {
                    primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, kx13User.UserId, entity.CustomerID);
                }
            });

    private void SaveCustomerAddressUsingKenticoApi(IModelMappingResult<CustomerAddressInfo> mapped, ComAddress kx13CustomerAddress) =>
        SaveEntityUsingKenticoApi(
            mapped,
            CustomerAddressInfo.Provider.Set,
            entity => primaryKeyMappingContext.SetMapping<ComAddress>(r => r.AddressId, kx13CustomerAddress.AddressId, entity.CustomerAddressID));


    private void SaveEntityUsingKenticoApi<TInfo>(
        IModelMappingResult<TInfo> mapped,
        Action<TInfo> saveAction,
        Action<TInfo>? postSaveAction = null)
        where TInfo : class
    {
        if (mapped is { Success: true } result)
        {
            (var entity, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                saveAction(entity);
                logger.LogEntitySetAction(newInstance, entity);
            }
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, entity);
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, entity);
                return;
            }

            postSaveAction?.Invoke(entity);
        }
    }
}
