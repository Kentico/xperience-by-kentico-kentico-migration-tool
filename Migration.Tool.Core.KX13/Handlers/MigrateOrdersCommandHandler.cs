using CMS.Commerce;
using CMS.DataEngine;
using CMS.FormEngine;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Contexts;
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
    IProtocol protocol,
    ToolConfiguration toolConfiguration,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IFieldMigrationService fieldMigrationService,
    KxpClassFacade kxpClassFacade)
    : IRequestHandler<MigrateOrdersCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateOrdersCommand request, CancellationToken cancellationToken)
    {
        // Migrate custom fields from KX13 ecommerce.order class to XbK OrderInfo class
        await MigrateOrderClass(cancellationToken);

        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var commerceSiteName = toolConfiguration.CommerceConfiguration?.CommerceSiteName ?? string.Empty;

        var kx13Orders = kx13Context.ComOrders
            .Include(o => o.OrderSite)
            .Where(o => o.OrderSite.SiteName == commerceSiteName)
            .Include(o => o.OrderCurrency)
            .Include(o => o.ComOrderAddresses)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderCustomer)
            .Include(o => o.OrderShippingOption)
            .Include(o => o.OrderPaymentOption);

        await using var kx13OrderStatusContext = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13OrderStatuses = kx13OrderStatusContext.ComOrderStatuses.ToList();
        var commerceConfiguration = toolConfiguration.CommerceConfiguration;
        var orderStatusesMapping = (commerceConfiguration?.OrderStatuses) ?? throw new InvalidOperationException("Order statuses mapping is not configured.");
        var xbkOrderStatuses = OrderStatusInfo.Provider.Get().ToList();

        foreach (var kx13Order in kx13Orders)
        {
            protocol.FetchedSource(kx13Order);
            logger.LogTrace("Migrating order {OrderId} with OrderGuid {OrderGuid}", kx13Order.OrderId, kx13Order.OrderGuid);

            // Get the customer info from target database
            CustomerInfo? xbkCustomerInfo = null;
            if (kx13Order.OrderCustomer is not null)
            {
                xbkCustomerInfo = CustomerInfo.Provider.Get()
                    .WhereEquals(nameof(CustomerInfo.CustomerGUID), kx13Order.OrderCustomer.CustomerGuid).FirstOrDefault();
                protocol.FetchedTarget(xbkCustomerInfo);
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
            protocol.MappedTarget(mapped);

            SaveOrderUsingKenticoApi(mapped!, kx13Order);

            if (mapped is { Success: true, Item: OrderInfo orderInfo })
            {
                await using var kx13OrderItemContext = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
                var kx13OrderItems = kx13OrderItemContext.ComOrderItems
                .Include(oi => oi.OrderItemSku)
                .Where(oi => oi.OrderItemOrderId == kx13Order.OrderId);

                foreach (var kx13OrderItem in kx13OrderItems)
                {
                    var xbkOrderItemInfo = OrderItemInfo.Provider.Get()
                        .WhereEquals(nameof(OrderItemInfo.OrderItemGUID), kx13OrderItem.OrderItemGuid).FirstOrDefault();

                    var mappedOrderItem = orderItemInfoMapper.Map(new OrderItemInfoMapperSource(kx13OrderItem, orderInfo), xbkOrderItemInfo);
                    protocol.MappedTarget(mappedOrderItem);

                    SaveOrderItemUsingKenticoApi(mappedOrderItem!, kx13OrderItem);
                }

                if (kx13Order.OrderCustomer is not null)
                {
                    await using var kx13OrderAddressContext = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
                    var kx13OrderAddresses = kx13OrderAddressContext.ComOrderAddresses
                        .Where(oa => oa.AddressOrderId == kx13Order.OrderId);

                    foreach (var kx13OrderAddress in kx13OrderAddresses)
                    {
                        var xbkOrderAddressInfo = OrderAddressInfo.Provider.Get()
                            .WhereEquals(nameof(OrderAddressInfo.OrderAddressGUID), kx13OrderAddress.AddressGuid).FirstOrDefault();

                        var mappedOrderAddress = orderAddressInfoMapper.Map(new OrderAddressInfoMapperSource(kx13OrderAddress, kx13Order.OrderCustomer, orderInfo), xbkOrderAddressInfo);
                        protocol.MappedTarget(mappedOrderAddress);

                        SaveOrderAddressUsingKenticoApi(mappedOrderAddress!, kx13OrderAddress);
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
            ArgumentNullException.ThrowIfNull(orderInfo);

            try
            {
                OrderInfo.Provider.Set(orderInfo);

                protocol.Success(kx13Order, orderInfo, mapped);
                logger.LogEntitySetAction(newInstance, orderInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, orderInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13Order)
                    .WithData(new { kx13Order.OrderId, kx13Order.OrderGuid })
                    .WithMessage("Failed to migrate order, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, orderInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<OrderInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(orderInfo)
                );
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
            ArgumentNullException.ThrowIfNull(orderItemInfo);

            try
            {
                OrderItemInfo.Provider.Set(orderItemInfo);

                protocol.Success(kx13OrderItem, orderItemInfo, mapped);
                logger.LogEntitySetAction(newInstance, orderItemInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, orderItemInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13OrderItem)
                    .WithData(new { kx13OrderItem.OrderItemId, kx13OrderItem.OrderItemGuid })
                    .WithMessage("Failed to migrate order item, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, orderItemInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<OrderItemInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(orderItemInfo)
                );
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
            ArgumentNullException.ThrowIfNull(orderAddressInfo);

            try
            {
                OrderAddressInfo.Provider.Set(orderAddressInfo);

                protocol.Success(kx13OrderAddress, orderAddressInfo, mapped);
                logger.LogEntitySetAction(newInstance, orderAddressInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, orderAddressInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13OrderAddress)
                    .WithData(new { kx13OrderAddress.AddressId, kx13OrderAddress.AddressGuid })
                    .WithMessage("Failed to migrate order address, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, orderAddressInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<OrderAddressInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(orderAddressInfo)
                );
                return;
            }

            // Map primary key for future reference
            primaryKeyMappingContext.SetMapping<ComOrderAddress>(r => r.AddressId, kx13OrderAddress.AddressId, orderAddressInfo.OrderAddressID);
        }
    }


    private async Task MigrateOrderClass(CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        // Get KX13 ecommerce.order class definition
        var kx13OrderClass = kx13Context.CmsClasses
            .FirstOrDefault(c => c.ClassName == "ecommerce.order");

        if (kx13OrderClass == null)
        {
            logger.LogWarning("KX13 ecommerce.order class not found, skipping custom field migration");
            return;
        }

        // Get XbK OrderInfo class
        var xbkOrderClass = kxpClassFacade.GetClass(OrderInfo.TYPEINFO.ObjectClassName);
        if (xbkOrderClass == null)
        {
            logger.LogWarning("XbK OrderInfo class not found, skipping custom field migration");
            return;
        }

        // Patch KX13 form definition
        var patcher = new FormDefinitionPatcher(
            logger,
            kx13OrderClass.ClassFormDefinition,
            fieldMigrationService,
            false, // classIsForm
            false, // classIsDocumentType
            true,  // discardSysFields (remove system fields, keep custom)
            false  // classIsCustom
        );
        patcher.PatchFields();
        patcher.RemoveCategories();

        var patchedDefinition = patcher.GetPatched();
        if (string.IsNullOrWhiteSpace(patchedDefinition))
        {
            logger.LogDebug("No custom fields found in KX13 ecommerce.order class");
            return;
        }

        // Merge custom fields into XbK class
        var xbkFormInfo = new FormInfo(xbkOrderClass.ClassFormDefinition);
        var kx13FormInfo = new FormInfo(patchedDefinition);
        var existingColumns = xbkFormInfo.GetColumnNames();
        int addedFieldsCount = 0;

        foreach (string? columnName in kx13FormInfo.GetColumnNames())
        {
            var field = kx13FormInfo.GetFormField(columnName);
            if (!field.PrimaryKey && !field.System && !existingColumns.Contains(columnName))
            {
                field.System = false;
                xbkFormInfo.AddFormItem(field);
                addedFieldsCount++;
                logger.LogInformation("Added custom field '{FieldName}' to OrderInfo class", columnName);
            }
        }

        // Add custom CurrencyCode field if it doesn't exist
        if (!existingColumns.Contains(CommerceConstants.CURRENCY_CODE_FIELD_NAME) && !kx13FormInfo.GetColumnNames().Contains(CommerceConstants.CURRENCY_CODE_FIELD_NAME))
        {
            var currencyCodeField = new FormFieldInfo
            {
                Name = CommerceConstants.CURRENCY_CODE_FIELD_NAME,
                DataType = FieldDataType.Text,
                Size = 10,
                AllowEmpty = true,
                System = false,
                Visible = true,
                Enabled = true,
                Caption = "Currency Code",
                Guid = Guid.NewGuid()
            };
            //currencyCodeField.Settings["controlname"] = FormComponents.AdminTextInputComponent;

            xbkFormInfo.AddFormItem(currencyCodeField);
            addedFieldsCount++;
            logger.LogInformation("Added new custom field '{FieldName}' to OrderInfo class", CommerceConstants.CURRENCY_CODE_FIELD_NAME);
        }

        // Add custom SiteOriginName field if it doesn't exist
        if (!existingColumns.Contains(CommerceConstants.SITE_ORIGIN_FIELD_NAME) && !kx13FormInfo.GetColumnNames().Contains(CommerceConstants.SITE_ORIGIN_FIELD_NAME))
        {
            var siteOriginNameField = new FormFieldInfo
            {
                Name = CommerceConstants.SITE_ORIGIN_FIELD_NAME,
                DataType = FieldDataType.Text,
                Size = 200,
                AllowEmpty = true,
                System = false,
                Visible = true,
                Enabled = true,
                Caption = "Site Origin Name",
                Guid = Guid.NewGuid()
            };

            xbkFormInfo.AddFormItem(siteOriginNameField);
            addedFieldsCount++;
            logger.LogInformation("Added new custom field '{FieldName}' to OrderInfo class", CommerceConstants.SITE_ORIGIN_FIELD_NAME);
        }

        if (addedFieldsCount > 0)
        {
            xbkOrderClass.ClassFormDefinition = xbkFormInfo.GetXmlDefinition();
            DataClassInfoProvider.ProviderObject.Set(xbkOrderClass);
            logger.LogInformation("Migrated {Count} custom field(s) to OrderInfo class", addedFieldsCount);
        }
        else
        {
            logger.LogDebug("No new custom fields to migrate to OrderInfo class");
        }
    }
}

