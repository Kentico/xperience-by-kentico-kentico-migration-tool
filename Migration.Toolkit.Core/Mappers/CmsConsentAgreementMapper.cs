using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Models;

public class CmsConsentAgreementMapper : EntityMapperBase<KX13.Models.CmsConsentAgreement, CmsConsentAgreement>
{
    public CmsConsentAgreementMapper(ILogger<CmsConsentAgreementMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol): base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsConsentAgreement? CreateNewInstance(KX13.Models.CmsConsentAgreement source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsentAgreement MapInternal(KX13.Models.CmsConsentAgreement source, CmsConsentAgreement target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentAgreementGuid = source.ConsentAgreementGuid;
        target.ConsentAgreementRevoked = source.ConsentAgreementRevoked;
        target.ConsentAgreementConsentHash = source.ConsentAgreementConsentHash;
        target.ConsentAgreementTime = source.ConsentAgreementTime;

        if (mappingHelper.TranslateRequiredId<KX13.Models.OmContact>(c => c.ContactId, source.ConsentAgreementContactId, out var contactId))
        {
            target.ConsentAgreementContactId = contactId;
        }

        if (mappingHelper.TranslateRequiredId<KX13.Models.CmsConsent>(r => r.ConsentId, source.ConsentAgreementConsentId, out var consentId))
        {
            target.ConsentAgreementConsentId = consentId;
        }

        return target;
    }
}