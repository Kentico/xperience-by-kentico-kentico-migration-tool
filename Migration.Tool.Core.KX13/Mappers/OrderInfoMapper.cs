using CMS.Commerce;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
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
string? CommerceSiteName);


public class OrderInfoMapper(
    ILogger<OrderInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    ToolConfiguration toolConfiguration,
    KxpClassFacade kxpClassFacade)
    : CommerceObjectInfoMapper<OrderInfoMapperSource, OrderInfo, ComOrder>(logger, primaryKeyMappingContext, protocol, kxpClassFacade, toolConfiguration)
{
    protected override string TargetObjectClassName => OrderInfo.TYPEINFO.ObjectClassName;

    protected override OrderInfo? CreateNewInstance(OrderInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();


    protected override ComOrder GetSourceModel(OrderInfoMapperSource source) => source.Order;


    protected override void MapCoreFields(OrderInfoMapperSource source, OrderInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (order, shippingDisplayName, paymentDisplayName, _, orderStatus, customer, _) = source;

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
    }


    protected override void MapSystemAndCustomFields(OrderInfoMapperSource source, OrderInfo target, IEnumerable<CustomizedFieldInfo> customizedFieldInfos, string systemFieldPrefix)
    {
        var (_, _, _, currency, _, _, commerceSiteName) = source;

        string siteOriginFieldName = $"{systemFieldPrefix}{CommerceConstants.SITE_ORIGIN_FIELD_NAME}";

        var siteOriginField = customizedFieldInfos.FirstOrDefault(fi => string.Equals(fi.FieldName, siteOriginFieldName, StringComparison.OrdinalIgnoreCase));
        if (siteOriginField is not null)
        {
            target.SetValue(siteOriginField.FieldName, commerceSiteName);
        }

        string currencyCodeFieldName = $"{systemFieldPrefix}{CommerceConstants.CURRENCY_CODE_FIELD_NAME}";

        var currencyCodeField = customizedFieldInfos.FirstOrDefault(fi => string.Equals(fi.FieldName, currencyCodeFieldName, StringComparison.OrdinalIgnoreCase));
        if (currencyCodeField is not null)
        {
            target.SetValue(currencyCodeField.FieldName, currency?.CurrencyCode);
        }

        base.MapSystemAndCustomFields(source, target, customizedFieldInfos, systemFieldPrefix);
    }
}

