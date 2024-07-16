using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.KX12.Mappers;

public class CmsConsentArchiveMapper : EntityMapperBase<KX12M.CmsConsentArchive, CmsConsentArchive>
{
    public CmsConsentArchiveMapper(ILogger<CmsConsentArchiveMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsConsentArchive? CreateNewInstance(KX12M.CmsConsentArchive source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsentArchive MapInternal(KX12M.CmsConsentArchive source, CmsConsentArchive target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentArchiveContent = source.ConsentArchiveContent;
        target.ConsentArchiveGuid = source.ConsentArchiveGuid;
        target.ConsentArchiveLastModified = source.ConsentArchiveLastModified;
        target.ConsentArchiveHash = source.ConsentArchiveHash;

        if (mappingHelper.TranslateRequiredId<KX12M.CmsConsent>(r => r.ConsentId, source.ConsentArchiveConsentId, out int consentId))
        {
            target.ConsentArchiveConsentId = consentId;
        }

        return target;
    }
}
