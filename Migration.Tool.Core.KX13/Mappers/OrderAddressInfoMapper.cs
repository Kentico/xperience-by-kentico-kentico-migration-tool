using CMS.Commerce;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record OrderAddressInfoMapperSource(ComOrderAddress Address, ComCustomer Customer, OrderInfo Order);


public class OrderAddressInfoMapper(
    ILogger<OrderAddressInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade)
    : EntityMapperBase<OrderAddressInfoMapperSource, OrderAddressInfo>(logger, primaryKeyMappingContext, protocol)
{

    protected override OrderAddressInfo? CreateNewInstance(OrderAddressInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();
    protected override OrderAddressInfo MapInternal(OrderAddressInfoMapperSource source, OrderAddressInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (address, customer, order) = source;

        if (!newInstance && address.AddressGuid != target.OrderAddressGUID)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.OrderAddressGUID = address.AddressGuid.GetValueOrDefault();
        target.OrderAddressOrderID = order.OrderID;

        // Map AddressType (int) to OrderAddressType (enum with Name property)
        // Typical mapping: 0 = Billing, 1 = Shipping, 2 = Company
        var addressType = address.AddressType switch
        {
            0 => new OrderAddressType("Unknown"),
            1 => OrderAddressType.Billing,
            2 => OrderAddressType.Shipping,
            3 => new OrderAddressType("Company"),
            _ => new OrderAddressType("Unknown") // Default to Billing if unknown
        };
        target.OrderAddressType = addressType;

        // Split personal name into first and last name
        var nameParts = address.AddressPersonalName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        target.OrderAddressFirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        target.OrderAddressLastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        //target.OrderAddressCompany
        target.OrderAddressEmail = customer.CustomerEmail;

        target.OrderAddressLine1 = address.AddressLine1;
        target.OrderAddressLine2 = address.AddressLine2;
        target.OrderAddressCity = address.AddressCity;
        target.OrderAddressZip = address.AddressZip;
        target.OrderAddressPhone = address.AddressPhone;

        // Map country ID
        if (mappingHelper.TranslateRequiredId<CmsCountry>(k => k.CountryId, address.AddressCountryId, out int countryId))
        {
            target.OrderAddressCountryID = countryId;
        }

        // Map state ID (optional)
        if (address.AddressStateId.HasValue)
        {
            if (mappingHelper.TryTranslateId<CmsState>(s => s.StateId, address.AddressStateId.Value, out int? stateId))
            {
                target.OrderAddressStateID = stateId ?? 0;
            }
        }

        var customized = kxpClassFacade.GetCustomizedFieldInfosAll(OrderAddressInfo.TYPEINFO.ObjectClassName);

        foreach (var customizedFieldInfo in customized)
        {
            string fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<ComOrderAddress>.TryGetPropertyValue(address, fieldName, StringComparison.InvariantCultureIgnoreCase, out object? value))
            {
                target.SetValue(fieldName, value);
            }
        }

        return target;
    }
}

