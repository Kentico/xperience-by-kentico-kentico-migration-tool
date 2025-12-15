using CMS.Commerce;
using CMS.Membership;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Constants;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public record CustomerInfoMapperSource(KX13M.ComCustomer Customer, string? CommerceSiteName, MemberInfo? Member);

public class CustomerInfoMapper(
    ILogger<CustomerInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade)
    : EntityMapperBase<CustomerInfoMapperSource, CustomerInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override CustomerInfo? CreateNewInstance(CustomerInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CustomerInfo MapInternal(CustomerInfoMapperSource source, CustomerInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (customer, commerceSiteName, member) = source;

        if (!newInstance && customer.CustomerGuid != target.CustomerGUID)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.CustomerGUID = customer.CustomerGuid;

        if (member is not null)
        {
            target.CustomerMemberID = member.MemberID;
        }

        target.CustomerCreatedWhen = customer.CustomerCreated.GetValueOrDefault();
        target.CustomerFirstName = customer.CustomerFirstName;
        target.CustomerLastName = customer.CustomerLastName;
        target.CustomerEmail = customer.CustomerEmail;
        target.CustomerPhone = customer.CustomerPhone;

        var customized = kxpClassFacade.GetCustomizedFieldInfosAll(CustomerInfo.TYPEINFO.ObjectClassName);

        var channelNameField = customized.FirstOrDefault(x => x.FieldName == CommerceConstants.SITE_ORIGIN_FIELD_NAME);
        if (channelNameField is not null)
        {
            target.SetValue(channelNameField.FieldName, commerceSiteName);
        }

        foreach (var customizedFieldInfo in customized)
        {
            string fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<KX13M.ComCustomer>.TryGetPropertyValue(customer, fieldName, StringComparison.InvariantCultureIgnoreCase, out object? value))
            {
                target.SetValue(fieldName, value);
            }
        }

        return target;
    }
}
