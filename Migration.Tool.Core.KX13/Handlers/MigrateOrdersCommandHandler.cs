using CMS.Base;
using CMS.Commerce;
using CMS.Core;
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
using Migration.Tool.Core.KX13.Helpers;
using Migration.Tool.Core.KX13.Mappers;
using Migration.Tool.KX13.Context;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Services.CmsClass;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateOrdersCommandHandler(
    ILogger<MigrateOrdersCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IEntityMapper<OrderInfoMapperSource, OrderInfo> orderInfoMapper,
    IEntityMapper<OrderItemInfoMapperSource, OrderItemInfo> orderItemInfoMapper,
    IEntityMapper<OrderAddressInfoMapperSource, OrderAddressInfo> orderAddressInfoMapper,
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IFieldMigrationService fieldMigrationService,
    KxpClassFacade kxpClassFacade
) : MigrateCommerceHandlerBase(logger, kx13ContextFactory, toolConfiguration, fieldMigrationService, kxpClassFacade),
    IRequestHandler<MigrateOrdersCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateOrdersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await Kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13CommerceSites = await GetCommerceSites(kx13Context);
        var kx13CommerceSiteIds = kx13CommerceSites.Select(s => s.SiteId).ToHashSet();

        // Migrate custom and system fields from KX13 'ecommerce.order' class to XbK counterpart
        await MigrateOrderClass(cancellationToken);
        await MigrateOrderItemsClass(cancellationToken);
        await MigrateOrderAddressClass(cancellationToken);

        await using var kx13OrderStatusContext = await Kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13OrderStatuses = kx13OrderStatusContext.ComOrderStatuses.Where(status => !status.StatusSiteId.HasValue || kx13CommerceSiteIds.Contains(status.StatusSiteId.Value)).ToList();

        var filterStatusCodeNames = ToolConfiguration.CommerceConfiguration?.KX13OrderFilter?.OrderStatusCodeNames;
        int[] statusIds = [];
        if (filterStatusCodeNames is not null && filterStatusCodeNames.Count != 0)
        {
            statusIds = kx13OrderStatuses.Where(status => filterStatusCodeNames.Contains(status.StatusName)).Select(status => status.StatusId).ToArray();
        }

        var kx13Orders = kx13Context.ComOrders
            .Where(o => kx13CommerceSiteIds.Contains(o.OrderSiteId))
            .Include(o => o.OrderSite)
            .FilterByDate(ToolConfiguration.CommerceConfiguration?.KX13OrderFilter?.OrderFromDate, ToolConfiguration.CommerceConfiguration?.KX13OrderFilter?.OrderToDate)
            .FilterByOrderStatus(statusIds)
            .Include(o => o.OrderCurrency)
            .Include(o => o.ComOrderAddresses)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderCustomer)
            .Include(o => o.OrderShippingOption)
            .Include(o => o.OrderPaymentOption);

        var orderStatusesMapping = ToolConfiguration.CommerceConfiguration?.OrderStatuses ?? throw new InvalidOperationException("Order statuses mapping is not configured");
        var xbkOrderStatuses = OrderStatusInfo.Provider.Get().ToList();
        await using var kx13OrderItemContext = await Kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        using (GetCMSActionContext())
        {
            foreach (var kx13Order in kx13Orders)
            {
                logger.LogTrace("Migrating order {OrderId} with OrderGuid {OrderGuid}", kx13Order.OrderId, kx13Order.OrderGuid);

                CustomerInfo? xbkCustomerInfo = null;
                if (kx13Order.OrderCustomer is not null)
                {
                    xbkCustomerInfo = CustomerInfo.Provider.Get()
                        .WhereEquals(nameof(CustomerInfo.CustomerGUID), kx13Order.OrderCustomer.CustomerGuid).FirstOrDefault();
                }

                var xbkOrderStatusName = orderStatusesMapping.FirstOrDefault(os => os.Value is not null && os.Value.Contains(kx13Order.OrderStatus?.StatusName, StringComparer.OrdinalIgnoreCase)).Key;
                if (string.IsNullOrEmpty(xbkOrderStatusName))
                {
                    logger.LogError("Order status {OrderStatusName} not mapped in settings, skipping order {OrderId}", kx13Order.OrderStatus?.StatusName, kx13Order.OrderId);
                    continue;
                }

                var xbkOrderStatus = xbkOrderStatuses.FirstOrDefault(os => os.OrderStatusName.Equals(xbkOrderStatusName, StringComparison.OrdinalIgnoreCase));
                if (xbkOrderStatus is null)
                {
                    logger.LogError("Order status {OrderStatusName} not found in Kentico, skipping order {OrderId}", xbkOrderStatusName, kx13Order.OrderId);
                    continue;
                }

                var xbkOrderInfo = OrderInfo.Provider.Get()
                    .WhereEquals(nameof(OrderInfo.OrderGUID), kx13Order.OrderGuid).FirstOrDefault();

                var mapped = orderInfoMapper.Map(new OrderInfoMapperSource(kx13Order, kx13Order.OrderShippingOption?.ShippingOptionDisplayName, kx13Order.OrderPaymentOption?.PaymentOptionDisplayName, kx13Order.OrderCurrency, xbkOrderStatus, xbkCustomerInfo, kx13Order.OrderSite?.SiteName), xbkOrderInfo);
                SaveOrderUsingKenticoApi(mapped!, kx13Order);

                if (mapped is { Success: true, Item: OrderInfo orderInfo })
                {
                    var kx13OrderItems = kx13OrderItemContext.ComOrderItems
                    .Include(oi => oi.OrderItemSku)
                    .Where(oi => oi.OrderItemOrderId == kx13Order.OrderId);

                    foreach (var kx13OrderItem in kx13OrderItems)
                    {
                        var xbkOrderItemInfo = OrderItemInfo.Provider.Get()
                            .WhereEquals(nameof(OrderItemInfo.OrderItemGUID), kx13OrderItem.OrderItemGuid).FirstOrDefault();

                        var mappedOrderItem = orderItemInfoMapper.Map(new OrderItemInfoMapperSource(kx13OrderItem, orderInfo), xbkOrderItemInfo);

                        SaveOrderItemUsingKenticoApi(mappedOrderItem!, kx13OrderItem);
                    }

                    if (kx13Order.OrderCustomer is not null)
                    {
                        await using var kx13OrderAddressContext = await Kx13ContextFactory.CreateDbContextAsync(cancellationToken);
                        var kx13OrderAddresses = kx13OrderAddressContext.ComOrderAddresses
                            .Where(oa => oa.AddressOrderId == kx13Order.OrderId);

                        foreach (var kx13OrderAddress in kx13OrderAddresses)
                        {
                            var xbkOrderAddressInfo = OrderAddressInfo.Provider.Get()
                                .WhereEquals(nameof(OrderAddressInfo.OrderAddressGUID), kx13OrderAddress.AddressGuid).FirstOrDefault();

                            var mappedOrderAddress = orderAddressInfoMapper.Map(new OrderAddressInfoMapperSource(kx13OrderAddress, kx13Order.OrderCustomer, orderInfo), xbkOrderAddressInfo);

                            SaveOrderAddressUsingKenticoApi(mappedOrderAddress!, kx13OrderAddress);
                        }
                    }

                }
            }
        }

        return new GenericCommandResult();
    }


    private void SaveOrderUsingKenticoApi(IModelMappingResult<OrderInfo> mapped, ComOrder kx13Order)
    {
        if (mapped is { Success: true } result)
        {
            (var orderInfo, bool newInstance) = result;
            if (orderInfo is null)
            {
                throw new InvalidOperationException("Order info is null");
            }

            try
            {
                OrderInfo.Provider.Set(orderInfo);

                logger.LogEntitySetAction(newInstance, orderInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, orderInfo);
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, orderInfo);
                return;
            }

            // Map primary key for future reference
            primaryKeyMappingContext.SetMapping<ComOrder>(r => r.OrderId, kx13Order.OrderId, orderInfo.OrderID);
        }
    }


    private void SaveOrderItemUsingKenticoApi(IModelMappingResult<OrderItemInfo> mapped, ComOrderItem kx13OrderItem)
    {
        if (mapped is { Success: true } result)
        {
            (var orderItemInfo, bool newInstance) = result;
            if (orderItemInfo is null)
            {
                throw new InvalidOperationException("Order item info is null");
            }

            try
            {
                OrderItemInfo.Provider.Set(orderItemInfo);
                logger.LogEntitySetAction(newInstance, orderItemInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, orderItemInfo);
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, orderItemInfo);
                return;
            }

            // Map primary key for future reference
            primaryKeyMappingContext.SetMapping<ComOrderItem>(r => r.OrderItemId, kx13OrderItem.OrderItemId, orderItemInfo.OrderItemID);
        }
    }


    private void SaveOrderAddressUsingKenticoApi(IModelMappingResult<OrderAddressInfo> mapped, ComOrderAddress kx13OrderAddress)
    {
        if (mapped is { Success: true } result)
        {
            (var orderAddressInfo, bool newInstance) = result;
            if (orderAddressInfo is null)
            {
                throw new InvalidOperationException("Order address info is null");
            }

            try
            {
                OrderAddressInfo.Provider.Set(orderAddressInfo);
                logger.LogEntitySetAction(newInstance, orderAddressInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, orderAddressInfo);
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, orderAddressInfo);
                return;
            }

            // Map primary key for future reference
            primaryKeyMappingContext.SetMapping<ComOrderAddress>(r => r.AddressId, kx13OrderAddress.AddressId, orderAddressInfo.OrderAddressID);
        }
    }


    private async Task MigrateOrderClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_ORDER_CLASS_NAME,
            targetClassName: OrderInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: ToolConfiguration.CommerceConfiguration?.IncludeOrderSystemFields,
            logEntityName: nameof(OrderInfo),
            addSiteOriginField: true,
            addCurrencyField: true,
            cancellationToken
        );


    private async Task MigrateOrderItemsClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_ORDER_ITEMS_CLASS_NAME,
            targetClassName: OrderItemInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: ToolConfiguration.CommerceConfiguration?.IncludeOrderItemsSystemFields,
            logEntityName: nameof(OrderItemInfo),
            addSiteOriginField: false,
            addCurrencyField: false,
            cancellationToken
        );


    private async Task MigrateOrderAddressClass(CancellationToken cancellationToken) =>
        await MigrateCommerceClass(
            sourceClassName: CommerceConstants.KX13_ORDER_ADDRESS_CLASS_NAME,
            targetClassName: OrderAddressInfo.TYPEINFO.ObjectClassName,
            includeSystemFieldsConfig: ToolConfiguration.CommerceConfiguration?.IncludeOrderAddressSystemFields,
            logEntityName: nameof(OrderAddressInfo),
            addSiteOriginField: false,
            addCurrencyField: false,
            cancellationToken
        );


    private CMSActionContext GetCMSActionContext()
    {
        var defaultAdmin = UserInfoProvider.ProviderObject.Get(UserInfoProvider.DEFAULT_ADMIN_USERNAME);
        return defaultAdmin == null
            ? throw new InvalidOperationException($"Target XbK doesn't contain default administrator account ('{UserInfoProvider.DEFAULT_ADMIN_USERNAME}')")
            : new CMSActionContext(defaultAdmin) { User = defaultAdmin, UseGlobalAdminContext = true, SendEmails = false };
    }
}
