using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KXP.Models;

public class CmsConsentArchiveMapper : EntityMapperBase<KX13.Models.CmsConsentArchive, CmsConsentArchive>
{
    public CmsConsentArchiveMapper(ILogger<CmsConsentArchiveMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsConsentArchive? CreateNewInstance(KX13.Models.CmsConsentArchive source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsentArchive MapInternal(KX13.Models.CmsConsentArchive source, CmsConsentArchive target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentArchiveContent = source.ConsentArchiveContent;
        target.ConsentArchiveGuid = source.ConsentArchiveGuid;
        target.ConsentArchiveLastModified = source.ConsentArchiveLastModified;
        target.ConsentArchiveHash = source.ConsentArchiveHash;
        
        if (mappingHelper.TranslateRequiredId<KX13.Models.CmsConsent>(r => r.ConsentId, source.ConsentArchiveConsentId, out var consentId))
        {
            target.ConsentArchiveConsentId = consentId;    
        }

        return target;
    }
}