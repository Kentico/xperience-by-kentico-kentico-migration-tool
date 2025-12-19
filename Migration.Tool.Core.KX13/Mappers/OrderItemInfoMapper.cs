using CMS.Commerce;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record OrderItemInfoMapperSource(ComOrderItem OrderItem, OrderInfo Order);


public class OrderItemInfoMapper(
    ILogger<OrderItemInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade)
    : EntityMapperBase<OrderItemInfoMapperSource, OrderItemInfo>(logger, primaryKeyMappingContext, protocol)
{

    protected override OrderItemInfo? CreateNewInstance(OrderItemInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();
    protected override OrderItemInfo MapInternal(OrderItemInfoMapperSource source, OrderItemInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
        target.OrderItemSKU = orderItem.OrderItemSku.Skunumber;
        target.OrderItemName = orderItem.OrderItemSkuname;
        target.OrderItemQuantity = orderItem.OrderItemUnitCount;
        target.OrderItemUnitPrice = orderItem.OrderItemUnitPrice;
        target.OrderItemTotalPrice = orderItem.OrderItemTotalPrice;

        var customized = kxpClassFacade.GetCustomizedFieldInfosAll(OrderItemInfo.TYPEINFO.ObjectClassName);

        foreach (var customizedFieldInfo in customized)
        {
            string fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<ComOrderItem>.TryGetPropertyValue(orderItem, fieldName, StringComparison.InvariantCultureIgnoreCase, out object? value))
            {
                target.SetValue(fieldName, value);
            }
        }

        return target;
    }
}

