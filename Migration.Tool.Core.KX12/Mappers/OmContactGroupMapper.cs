using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX12.Mappers;

public class OmContactGroupMapper : EntityMapperBase<KX12M.OmContactGroup, OmContactGroup>
{
    public OmContactGroupMapper(
        ILogger<OmContactGroupMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override OmContactGroup? CreateNewInstance(KX12M.OmContactGroup tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override OmContactGroup MapInternal(KX12M.OmContactGroup source, OmContactGroup target, bool newInstance,
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
