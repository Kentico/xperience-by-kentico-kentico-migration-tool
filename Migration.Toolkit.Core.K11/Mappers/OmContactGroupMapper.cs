using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

namespace Migration.Toolkit.Core.K11.Mappers;

public class OmContactGroupMapper(
    ILogger<OmContactGroupMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : EntityMapperBase<OmContactGroup, KXP.Models.OmContactGroup>(logger, primaryKeyMappingContext, protocol)
{
    protected override KXP.Models.OmContactGroup? CreateNewInstance(OmContactGroup tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override KXP.Models.OmContactGroup MapInternal(OmContactGroup source, KXP.Models.OmContactGroup target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ContactGroupName = source.ContactGroupName;
        target.ContactGroupDisplayName = source.ContactGroupDisplayName;
        target.ContactGroupDescription = source.ContactGroupDescription;
        target.ContactGroupDynamicCondition = source.ContactGroupDynamicCondition;
        target.ContactGroupEnabled = source.ContactGroupEnabled;
        target.ContactGroupLastModified = source.ContactGroupLastModified;
        target.ContactGroupGuid = source.ContactGroupGuid;
        target.ContactGroupStatus = source.ContactGroupStatus;

        return target;
    }
}
