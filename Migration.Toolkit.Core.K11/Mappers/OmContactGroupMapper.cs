namespace Migration.Toolkit.Core.K11.Mappers;

using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.KXP.Models;

public class OmContactGroupMapper(ILogger<OmContactGroupMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : EntityMapperBase<Toolkit.K11.Models.OmContactGroup, OmContactGroup>(logger, primaryKeyMappingContext, protocol)
{
    protected override OmContactGroup? CreateNewInstance(Toolkit.K11.Models.OmContactGroup tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override OmContactGroup MapInternal(Toolkit.K11.Models.OmContactGroup source, OmContactGroup target, bool newInstance,
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