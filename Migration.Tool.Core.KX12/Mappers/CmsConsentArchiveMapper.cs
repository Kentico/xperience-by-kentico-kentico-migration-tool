using CMS.DataProtection;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;

namespace Migration.Tool.Core.KX12.Mappers;

public class CmsConsentArchiveMapper : EntityMapperBase<KX12M.CmsConsentArchive, ConsentArchiveInfo>
{
    public CmsConsentArchiveMapper(ILogger<CmsConsentArchiveMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override ConsentArchiveInfo? CreateNewInstance(KX12M.CmsConsentArchive source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override ConsentArchiveInfo MapInternal(KX12M.CmsConsentArchive source, ConsentArchiveInfo target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ConsentArchiveContent = source.ConsentArchiveContent;
        target.ConsentArchiveGuid = source.ConsentArchiveGuid;
        target.ConsentArchiveLastModified = source.ConsentArchiveLastModified;
        target.ConsentArchiveHash = source.ConsentArchiveHash;

        if (mappingHelper.TranslateRequiredId<KX12M.CmsConsent>(r => r.ConsentId, source.ConsentArchiveConsentId, out int consentId))
        {
            target.ConsentArchiveConsentID = consentId;
        }

        return target;
    }
}
