
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.K11.Mappers;
public class OmContactStatusMapper(ILogger<OmContactStatusMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : EntityMapperBase<Toolkit.K11.Models.OmContactStatus, OmContactStatus>(logger, primaryKeyMappingContext, protocol)
{
    protected override OmContactStatus? CreateNewInstance(Toolkit.K11.Models.OmContactStatus tSourceEntity, MappingHelper mappingHelper,
        AddFailure addFailure) => new();

    protected override OmContactStatus MapInternal(Toolkit.K11.Models.OmContactStatus source, OmContactStatus target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ContactStatusName = source.ContactStatusName;
        target.ContactStatusDisplayName = source.ContactStatusDisplayName;
        target.ContactStatusDescription = source.ContactStatusDescription;

        return target;
    }
}
