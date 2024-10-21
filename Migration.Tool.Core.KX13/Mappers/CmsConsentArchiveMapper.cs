using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX13.Mappers;

public class CmsConsentArchiveMapper : EntityMapperBase<KX13M.CmsConsentArchive, CmsConsentArchive>
{
    public CmsConsentArchiveMapper(ILogger<CmsConsentArchiveMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsConsentArchive? CreateNewInstance(KX13M.CmsConsentArchive source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsentArchive MapInternal(KX13M.CmsConsentArchive source, CmsConsentArchive target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentArchiveContent = source.ConsentArchiveContent;
        target.ConsentArchiveGuid = source.ConsentArchiveGuid;
        target.ConsentArchiveLastModified = source.ConsentArchiveLastModified;
        target.ConsentArchiveHash = source.ConsentArchiveHash;

        if (mappingHelper.TranslateRequiredId<KX13M.CmsConsent>(r => r.ConsentId, source.ConsentArchiveConsentId, out int consentId))
        {
            target.ConsentArchiveConsentId = consentId;
        }

        return target;
    }
}
