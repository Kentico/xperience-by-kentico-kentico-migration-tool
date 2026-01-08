using CMS.Commerce;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record OrderItemInfoMapperSource
(ComOrderItem OrderItem,
OrderInfo Order);

/// <summary>
/// Mapper for OrderItemInfo.
/// </summary>
public class OrderItemInfoMapper(
    ILogger<OrderItemInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    ToolConfiguration toolConfiguration,
    KxpClassFacade kxpClassFacade)
    : CommerceObjectInfoMapper<OrderItemInfoMapperSource, OrderItemInfo, ComOrderItem>(logger, primaryKeyMappingContext, protocol, kxpClassFacade, toolConfiguration)
{
    protected override string TargetObjectClassName => OrderItemInfo.TYPEINFO.ObjectClassName;

    protected override OrderItemInfo? CreateNewInstance(OrderItemInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();


    protected override ComOrderItem GetSourceModel(OrderItemInfoMapperSource source) => source.OrderItem;


    protected override void MapCoreFields(OrderItemInfoMapperSource source, OrderItemInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (orderItem, order) = source;

        if (!newInstance && orderItem.OrderItemGuid != target.OrderItemGUID)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.OrderItemGUID = orderItem.OrderItemGuid;
        target.OrderItemOrderID = order.OrderID;
        target.OrderItemSKU = orderItem.OrderItemSku?.Skunumber;
        target.OrderItemName = orderItem.OrderItemSkuname;
        target.OrderItemQuantity = orderItem.OrderItemUnitCount;
        target.OrderItemUnitPrice = orderItem.OrderItemUnitPrice;
        target.OrderItemTotalPrice = orderItem.OrderItemTotalPrice;
        target.OrderItemTotalTax = 0;
        target.OrderItemTaxRate = 0;
    }
}

