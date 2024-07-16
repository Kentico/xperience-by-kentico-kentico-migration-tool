using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX13.Contexts;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.KX13.Mappers;

public class OmContactStatusMapper : EntityMapperBase<KX13M.OmContactStatus, OmContactStatus>
{
    public OmContactStatusMapper(
        ILogger<OmContactStatusMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override OmContactStatus? CreateNewInstance(KX13M.OmContactStatus tSourceEntity, MappingHelper mappingHelper,
        AddFailure addFailure) => new();

    protected override OmContactStatus MapInternal(KX13M.OmContactStatus source, OmContactStatus target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ContactStatusName = source.ContactStatusName;
        target.ContactStatusDisplayName = source.ContactStatusDisplayName;
        target.ContactStatusDescription = source.ContactStatusDescription;

        return target;
    }
}
