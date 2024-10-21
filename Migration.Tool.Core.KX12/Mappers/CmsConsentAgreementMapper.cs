using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX12.Mappers;

public class CmsConsentAgreementMapper : EntityMapperBase<KX12M.CmsConsentAgreement, CmsConsentAgreement>
{
    public CmsConsentAgreementMapper(ILogger<CmsConsentAgreementMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsConsentAgreement? CreateNewInstance(KX12M.CmsConsentAgreement source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsentAgreement MapInternal(KX12M.CmsConsentAgreement source, CmsConsentAgreement target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentAgreementGuid = source.ConsentAgreementGuid;
        target.ConsentAgreementRevoked = source.ConsentAgreementRevoked;
        target.ConsentAgreementConsentHash = source.ConsentAgreementConsentHash;
        target.ConsentAgreementTime = source.ConsentAgreementTime;

        if (mappingHelper.TranslateRequiredId<KX12M.OmContact>(c => c.ContactId, source.ConsentAgreementContactId, out int contactId))
        {
            target.ConsentAgreementContactId = contactId;
        }

        if (mappingHelper.TranslateRequiredId<KX12M.CmsConsent>(r => r.ConsentId, source.ConsentAgreementConsentId, out int consentId))
        {
            target.ConsentAgreementConsentId = consentId;
        }

        return target;
    }
}
