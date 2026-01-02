using CMS.Commerce;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record OrderAddressInfoMapperSource(ComOrderAddress Address, ComCustomer Customer, OrderInfo Order);

/// <summary>
/// Mapper for OrderAddressInfo.
/// </summary>
public class OrderAddressInfoMapper(
    ILogger<OrderAddressInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    ToolConfiguration toolConfiguration,
    KxpClassFacade kxpClassFacade)
    : CommerceObjectInfoMapper<OrderAddressInfoMapperSource, OrderAddressInfo, ComOrderAddress>(logger, primaryKeyMappingContext, protocol, kxpClassFacade, toolConfiguration)
{
    protected override string TargetObjectClassName => OrderAddressInfo.TYPEINFO.ObjectClassName;


    protected override OrderAddressInfo? CreateNewInstance(OrderAddressInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();


    protected override ComOrderAddress GetSourceModel(OrderAddressInfoMapperSource source) => source.Address;


    protected override void MapCoreFields(OrderAddressInfoMapperSource source, OrderAddressInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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
        // Typical mapping: Unknown = 0, Billing = 1, Shipping = 2, Company = 3
        var addressType = address.AddressType switch
        {
            0 => new OrderAddressType("Unknown"),
            1 => OrderAddressType.Billing,
            2 => OrderAddressType.Shipping,
            3 => new OrderAddressType("Company"),
            _ => new OrderAddressType("Unknown")
        };
        target.OrderAddressType = addressType;

        // Split personal name into first and last name
        var personalName = (address.AddressPersonalName ?? string.Empty).Trim();
        var nameParts = personalName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        target.OrderAddressFirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        target.OrderAddressLastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

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
        if (address.AddressStateId.HasValue && mappingHelper.TryTranslateId<CmsState>(s => s.StateId, address.AddressStateId.Value, out int? stateId))
        {
            target.OrderAddressStateID = stateId ?? 0;
        }
    }
}

