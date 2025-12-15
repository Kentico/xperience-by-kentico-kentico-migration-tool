using CMS.Commerce;

using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KX13.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record CustomerAddressInfoMapperSource(ComAddress Address, CustomerInfo Customer);


public class CustomerAddressInfoMapper(
    ILogger<CustomerAddressInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade)
    : EntityMapperBase<CustomerAddressInfoMapperSource, CustomerAddressInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override CustomerAddressInfo? CreateNewInstance(CustomerAddressInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CustomerAddressInfo MapInternal(CustomerAddressInfoMapperSource source, CustomerAddressInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (address, customer) = source;

        if (!newInstance && address.AddressGuid != target.CustomerAddressGUID)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.CustomerAddressGUID = address.AddressGuid.GetValueOrDefault();
        target.CustomerAddressCustomerID = customer.CustomerID;

        // Split personal name into first and last name
        var nameParts = address.AddressPersonalName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        target.CustomerAddressFirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        // Join remaining parts as last name
        target.CustomerAddressLastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

        target.CustomerAddressLine1 = address.AddressLine1;
        target.CustomerAddressLine2 = address.AddressLine2;
        target.CustomerAddressCity = address.AddressCity;
        target.CustomerAddressZip = address.AddressZip;
        target.CustomerAddressPhone = address.AddressPhone;
        target.CustomerAddressEmail = customer.CustomerEmail;

        var customized = kxpClassFacade.GetCustomizedFieldInfosAll(CustomerAddressInfo.TYPEINFO.ObjectClassName);

        foreach (var customizedFieldInfo in customized)
        {
            string fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<ComAddress>.TryGetPropertyValue(address, fieldName, StringComparison.InvariantCultureIgnoreCase, out object? value))
            {
                target.SetValue(fieldName, value);
            }
        }

        return target;
    }
}
