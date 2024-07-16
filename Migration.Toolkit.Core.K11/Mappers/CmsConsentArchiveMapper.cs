using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;

public class CmsConsentArchiveMapper(
    ILogger<CmsConsentArchiveMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : EntityMapperBase<CmsConsentArchive, KXP.Models.CmsConsentArchive>(logger, primaryKeyMappingContext, protocol)
{
    protected override KXP.Models.CmsConsentArchive? CreateNewInstance(CmsConsentArchive source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override KXP.Models.CmsConsentArchive MapInternal(CmsConsentArchive source, KXP.Models.CmsConsentArchive target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentArchiveContent = source.ConsentArchiveContent;
        target.ConsentArchiveGuid = source.ConsentArchiveGuid;
        target.ConsentArchiveLastModified = source.ConsentArchiveLastModified;
        target.ConsentArchiveHash = source.ConsentArchiveHash;

        if (mappingHelper.TranslateRequiredId<CmsConsent>(r => r.ConsentId, source.ConsentArchiveConsentId, out int consentId))
        {
            target.ConsentArchiveConsentId = consentId;
        }

        return target;
    }
}
