using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KXP.Models;

public class OmContactStatusMapper : EntityMapperBase<KX13.Models.OmContactStatus, OmContactStatus>
{
    public OmContactStatusMapper(
        ILogger<OmContactStatusMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IMigrationProtocol migrationProtocol
    ) : base(logger, primaryKeyMappingContext, migrationProtocol)
    {
    }

    protected override OmContactStatus? CreateNewInstance(KX13.Models.OmContactStatus tSourceEntity, MappingHelper mappingHelper,
        AddFailure addFailure) => new();

    protected override OmContactStatus MapInternal(KX13.Models.OmContactStatus source, OmContactStatus target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ContactStatusName = source.ContactStatusName;
        target.ContactStatusDisplayName = source.ContactStatusDisplayName;
        target.ContactStatusDescription = source.ContactStatusDescription;

        return target;
    }
}