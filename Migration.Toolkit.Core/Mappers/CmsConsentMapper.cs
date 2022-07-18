using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KXP.Models;

public class CmsConsentMapper : EntityMapperBase<KX13.Models.CmsConsent, CmsConsent>
{
    public CmsConsentMapper(ILogger<CmsConsentMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol): base(logger, pkContext, protocol)
    {
    }

    protected override CmsConsent? CreateNewInstance(KX13.Models.CmsConsent source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsConsent MapInternal(KX13.Models.CmsConsent source, CmsConsent target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentDisplayName = source.ConsentDisplayName;
        target.ConsentName= source.ConsentName;
        target.ConsentContent = source.ConsentContent;
        target.ConsentGuid = source.ConsentGuid;
        target.ConsentLastModified = source.ConsentLastModified;
        target.ConsentHash = source.ConsentHash;
       
        return target;
    }
}