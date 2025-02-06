using CMS.ContactManagement;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;

namespace Migration.Tool.Core.KX12.Mappers;

public class OmContactStatusMapper : EntityMapperBase<KX12M.OmContactStatus, ContactStatusInfo>
{
    public OmContactStatusMapper(
        ILogger<OmContactStatusMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override ContactStatusInfo? CreateNewInstance(KX12M.OmContactStatus tSourceEntity, MappingHelper mappingHelper,
        AddFailure addFailure) => new();

    protected override ContactStatusInfo MapInternal(KX12M.OmContactStatus source, ContactStatusInfo target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ContactStatusName = source.ContactStatusName;
        target.ContactStatusDisplayName = source.ContactStatusDisplayName;
        target.ContactStatusDescription = source.ContactStatusDescription;

        return target;
    }
}
