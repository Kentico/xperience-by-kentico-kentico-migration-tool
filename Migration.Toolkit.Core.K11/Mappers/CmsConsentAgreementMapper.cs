namespace Migration.Toolkit.Core.K11.Mappers;

using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.KXP.Models;

public class CmsConsentAgreementMapper(ILogger<CmsConsentAgreementMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol)
    : EntityMapperBase<Toolkit.K11.Models.CmsConsentAgreement, CmsConsentAgreement>(logger, primaryKeyMappingContext, protocol)
{
    protected override CmsConsentAgreement? CreateNewInstance(Toolkit.K11.Models.CmsConsentAgreement source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsentAgreement MapInternal(Toolkit.K11.Models.CmsConsentAgreement source, CmsConsentAgreement target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentAgreementGuid = source.ConsentAgreementGuid;
        target.ConsentAgreementRevoked = source.ConsentAgreementRevoked;
        target.ConsentAgreementConsentHash = source.ConsentAgreementConsentHash;
        target.ConsentAgreementTime = source.ConsentAgreementTime;

        if (mappingHelper.TranslateRequiredId<Toolkit.K11.Models.OmContact>(c => c.ContactId, source.ConsentAgreementContactId, out var contactId))
        {
            target.ConsentAgreementContactId = contactId;
        }

        if (mappingHelper.TranslateRequiredId<Toolkit.K11.Models.CmsConsent>(r => r.ConsentId, source.ConsentAgreementConsentId, out var consentId))
        {
            target.ConsentAgreementConsentId = consentId;
        }

        return target;
    }
}