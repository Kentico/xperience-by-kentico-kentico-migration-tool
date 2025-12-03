using CMS.Commerce;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record OrderInfoMapperSource
(ComOrder Order,
string? ShippingDisplayName,
string? PaymentDisplayName,
ComCurrency? Currency,
OrderStatusInfo OrderStatus,
CustomerInfo? Customer,
string? SiteName);


public class OrderInfoMapper(
    ILogger<OrderInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade)
    : EntityMapperBase<OrderInfoMapperSource, OrderInfo>(logger, primaryKeyMappingContext, protocol)
{

    protected override OrderInfo? CreateNewInstance(OrderInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();
    protected override OrderInfo MapInternal(OrderInfoMapperSource source, OrderInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (order, shippingDisplayName, paymentDisplayName, currency, orderStatus, customer, siteName) = source;

        if (!newInstance && order.OrderGuid != target.OrderGUID)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.OrderGUID = order.OrderGuid;

        if (customer is not null)
        {
            target.OrderCustomerID = customer.CustomerID;
        }

        target.OrderNumber = order.OrderInvoiceNumber;
        target.OrderCreatedWhen = order.OrderDate;
        target.OrderTotalPrice = order.OrderTotalPrice;
        target.OrderTotalTax = order.OrderTotalTax;
        target.OrderTotalShipping = order.OrderTotalShipping.GetValueOrDefault();
        target.OrderGrandTotal = order.OrderGrandTotal;
        target.OrderModifiedWhen = order.OrderLastModified;
        target.OrderShippingMethodDisplayName = shippingDisplayName;
        target.OrderPaymentMethodDisplayName = paymentDisplayName;
        target.OrderShippingMethodPrice = 0;

        target.OrderOrderStatusID = orderStatus.OrderStatusID;


        var customized = kxpClassFacade.GetCustomizedFieldInfosAll(OrderInfo.TYPEINFO.ObjectClassName);

        if (currency is not null)
        {
            target.SetValue(CommerceConstants.CURRENCY_CODE_FIELD_NAME, currency.CurrencyCode);
        }

        if (!string.IsNullOrEmpty(siteName))
        {
            target.SetValue(CommerceConstants.SITE_ORIGIN_FIELD_NAME, siteName);
        }

        foreach (var customizedFieldInfo in customized)
        {
            string fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<ComOrder>.TryGetPropertyValue(order, fieldName, StringComparison.InvariantCultureIgnoreCase, out object? value))
            {
                target.SetValue(fieldName, value);
            }
        }

        return target;
    }
}

