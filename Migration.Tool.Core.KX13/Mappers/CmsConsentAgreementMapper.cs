using CMS.DataProtection;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;

namespace Migration.Tool.Core.KX13.Mappers;

public class CmsConsentAgreementMapper : EntityMapperBase<KX13M.CmsConsentAgreement, ConsentAgreementInfo>
{
    public CmsConsentAgreementMapper(ILogger<CmsConsentAgreementMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override ConsentAgreementInfo? CreateNewInstance(KX13M.CmsConsentAgreement source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override ConsentAgreementInfo MapInternal(KX13M.CmsConsentAgreement source, ConsentAgreementInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentAgreementGuid = source.ConsentAgreementGuid;
        target.ConsentAgreementRevoked = source.ConsentAgreementRevoked;
        target.ConsentAgreementConsentHash = source.ConsentAgreementConsentHash;
        target.ConsentAgreementTime = source.ConsentAgreementTime;

        if (mappingHelper.TranslateRequiredId<KX13M.OmContact>(c => c.ContactId, source.ConsentAgreementContactId, out int contactId))
        {
            target.ConsentAgreementContactID = contactId;
        }

        if (mappingHelper.TranslateRequiredId<KX13M.CmsConsent>(r => r.ConsentId, source.ConsentAgreementConsentId, out int consentId))
        {
            target.ConsentAgreementConsentID = consentId;
        }

        return target;
    }
}
