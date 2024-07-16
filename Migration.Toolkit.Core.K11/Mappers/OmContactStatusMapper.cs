using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;

public class OmContactStatusMapper(
    ILogger<OmContactStatusMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : EntityMapperBase<OmContactStatus, KXP.Models.OmContactStatus>(logger, primaryKeyMappingContext, protocol)
{
    protected override KXP.Models.OmContactStatus? CreateNewInstance(OmContactStatus tSourceEntity, MappingHelper mappingHelper,
        AddFailure addFailure) => new();

    protected override KXP.Models.OmContactStatus MapInternal(OmContactStatus source, KXP.Models.OmContactStatus target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ContactStatusName = source.ContactStatusName;
        target.ContactStatusDisplayName = source.ContactStatusDisplayName;
        target.ContactStatusDescription = source.ContactStatusDescription;

        return target;
    }
}
