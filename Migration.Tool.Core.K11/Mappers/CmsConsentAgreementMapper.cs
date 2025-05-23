﻿using CMS.DataProtection;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Mappers;

public class CmsConsentAgreementMapper(ILogger<CmsConsentAgreementMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol)
    : EntityMapperBase<CmsConsentAgreement, ConsentAgreementInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override ConsentAgreementInfo? CreateNewInstance(CmsConsentAgreement source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override ConsentAgreementInfo MapInternal(CmsConsentAgreement source, ConsentAgreementInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentAgreementGuid = source.ConsentAgreementGuid;
        target.ConsentAgreementRevoked = source.ConsentAgreementRevoked;
        target.ConsentAgreementConsentHash = source.ConsentAgreementConsentHash;
        target.ConsentAgreementTime = source.ConsentAgreementTime;

        if (mappingHelper.TranslateRequiredId<OmContact>(c => c.ContactId, source.ConsentAgreementContactId, out int contactId))
        {
            target.ConsentAgreementContactID = contactId;
        }

        if (mappingHelper.TranslateRequiredId<CmsConsent>(r => r.ConsentId, source.ConsentAgreementConsentId, out int consentId))
        {
            target.ConsentAgreementConsentID = consentId;
        }

        return target;
    }
}
