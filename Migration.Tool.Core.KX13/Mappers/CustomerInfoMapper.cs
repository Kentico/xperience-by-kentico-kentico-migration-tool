using CMS.Commerce;
using CMS.Membership;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
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
    KxpClassFacade kxpClassFacade,
    ToolConfiguration toolConfiguration)
    : CommerceObjectInfoMapper<CustomerInfoMapperSource, CustomerInfo, KX13M.ComCustomer>(logger, primaryKeyMappingContext, protocol, kxpClassFacade, toolConfiguration)
{
    protected override CustomerInfo? CreateNewInstance(CustomerInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override void MapCoreFields(CustomerInfoMapperSource source, CustomerInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (customer, _, member) = source;

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
    }

    protected override void MapSystemAndCustomFields(CustomerInfoMapperSource source, CustomerInfo target, IEnumerable<CustomizedFieldInfo> customizedFieldInfos, string systemFieldPrefix)
    {
        var (_, commerceSiteName, _) = source;

        string siteOriginFieldName = $"{systemFieldPrefix}{CommerceConstants.SITE_ORIGIN_FIELD_NAME}";

        var siteOriginField = customizedFieldInfos.FirstOrDefault(fi => string.Equals(fi.FieldName, siteOriginFieldName, StringComparison.OrdinalIgnoreCase));
        if (siteOriginField is not null)
        {
            target.SetValue(siteOriginField.FieldName, commerceSiteName);
        }

        base.MapSystemAndCustomFields(source, target, customizedFieldInfos, systemFieldPrefix);
    }

    protected override KX13M.ComCustomer GetSourceModel(CustomerInfoMapperSource source) => source.Customer;

    protected override string GetTargetObjectClassName() => CustomerInfo.TYPEINFO.ObjectClassName;
}
