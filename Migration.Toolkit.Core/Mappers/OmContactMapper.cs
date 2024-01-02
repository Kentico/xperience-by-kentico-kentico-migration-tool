using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;


namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Models;

public class OmContactMapper : EntityMapperBase<KX13.Models.OmContact, OmContact>
{
    private readonly ILogger<OmContactMapper> _logger;
    private readonly IEntityMapper<KX13M.OmContactStatus, OmContactStatus> _contactStatusMapper;

    public OmContactMapper(
        ILogger<OmContactMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IEntityMapper<KX13.Models.OmContactStatus, OmContactStatus> contactStatusMapper,
        IProtocol protocol
    ): base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _contactStatusMapper = contactStatusMapper;
    }

    protected override OmContact? CreateNewInstance(KX13.Models.OmContact tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override OmContact MapInternal(KX13.Models.OmContact source, OmContact target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (!newInstance && source.ContactGuid != target.ContactGuid)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch");
        }

        // do not try to insert pk
        // target.ContactId = source.ContactId;
        target.ContactFirstName = source.ContactFirstName;
        target.ContactMiddleName = source.ContactMiddleName;
        target.ContactLastName = source.ContactLastName;
        target.ContactJobTitle = source.ContactJobTitle;
        target.ContactAddress1 = source.ContactAddress1;
        target.ContactCity = source.ContactCity;
        target.ContactZip = source.ContactZip;
        target.ContactMobilePhone = source.ContactMobilePhone;
        target.ContactBusinessPhone = source.ContactBusinessPhone;
        target.ContactEmail = source.ContactEmail;
        target.ContactBirthday = source.ContactBirthday;
        target.ContactGender = source.ContactGender;
        target.ContactNotes = source.ContactNotes;
        target.ContactMonitored = source.ContactMonitored;
        target.ContactGuid = source.ContactGuid;
        target.ContactLastModified = source.ContactLastModified;
        target.ContactCreated = source.ContactCreated;
        target.ContactBounces = source.ContactBounces;
        target.ContactCampaign = source.ContactCampaign;
        target.ContactSalesForceLeadReplicationDisabled = source.ContactSalesForceLeadReplicationDisabled;
        target.ContactSalesForceLeadReplicationDateTime = source.ContactSalesForceLeadReplicationDateTime;
        target.ContactSalesForceLeadReplicationSuspensionDateTime = source.ContactSalesForceLeadReplicationSuspensionDateTime;
        target.ContactCompanyName = source.ContactCompanyName;
        target.ContactSalesForceLeadReplicationRequired = source.ContactSalesForceLeadReplicationRequired;

        // TODO tk: 2022-06-13 resolve migration of target.ContactStateId = _primaryKeyMappingContext.MapFromSource<K13M.CmsState>(u => u.StateId, source.ContactStateId);
        // TODO tk: 2022-06-13 resolve migration of target.ContactCountryId = _primaryKeyMappingContext.MapFromSource<K13M.CmsCountry>(u => u.CountryId, source.ContactCountryId);

        if (source.ContactStatus != null)
        {
            switch (_contactStatusMapper.Map(source.ContactStatus, target.ContactStatus))
            {
                case { Success: true } result:
                {
                    target.ContactStatus = result.Item;
                    break;
                }
                case { Success: false } result:
                {
                    addFailure(new MapperResultFailure<OmContact>(result?.HandbookReference));
                    break;
                }
            }
        }
        else
        {
            target.ContactStatus = null;
        }

        target.ContactSalesForceLeadId = source.ContactSalesForceLeadId;
        if (mappingHelper.TranslateIdAllowNulls<KX13M.CmsUser>(u => u.UserId, source.ContactOwnerUserId, out var userId))
        {
            target.ContactOwnerUserId = userId;
        }

        return target;
    }
}