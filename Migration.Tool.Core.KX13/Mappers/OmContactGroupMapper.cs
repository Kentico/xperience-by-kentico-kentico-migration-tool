using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX13.Mappers;

public class OmContactGroupMapper : EntityMapperBase<KX13M.OmContactGroup, OmContactGroup>
{
    public OmContactGroupMapper(
        ILogger<OmContactGroupMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override OmContactGroup? CreateNewInstance(KX13M.OmContactGroup tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override OmContactGroup MapInternal(KX13M.OmContactGroup source, OmContactGroup target, bool newInstance,
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
