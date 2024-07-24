using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;

public class CmsConsentAgreementMapper(ILogger<CmsConsentAgreementMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol)
    : EntityMapperBase<CmsConsentAgreement, KXP.Models.CmsConsentAgreement>(logger, primaryKeyMappingContext, protocol)
{
    protected override KXP.Models.CmsConsentAgreement? CreateNewInstance(CmsConsentAgreement source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override KXP.Models.CmsConsentAgreement MapInternal(CmsConsentAgreement source, KXP.Models.CmsConsentAgreement target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentAgreementGuid = source.ConsentAgreementGuid;
        target.ConsentAgreementRevoked = source.ConsentAgreementRevoked;
        target.ConsentAgreementConsentHash = source.ConsentAgreementConsentHash;
        target.ConsentAgreementTime = source.ConsentAgreementTime;

        if (mappingHelper.TranslateRequiredId<OmContact>(c => c.ContactId, source.ConsentAgreementContactId, out int contactId))
        {
            target.ConsentAgreementContactId = contactId;
        }

        if (mappingHelper.TranslateRequiredId<CmsConsent>(r => r.ConsentId, source.ConsentAgreementConsentId, out int consentId))
        {
            target.ConsentAgreementConsentId = consentId;
        }

        return target;
    }
}
