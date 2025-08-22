using CMS.ContactManagement;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11.Models;

namespace Migration.Tool.Core.K11.Mappers;

public class OmContactGroupMapper(
    ILogger<OmContactGroupMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : EntityMapperBase<OmContactGroup, ContactGroupInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override ContactGroupInfo? CreateNewInstance(OmContactGroup tSourceEntity, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override ContactGroupInfo MapInternal(OmContactGroup source, ContactGroupInfo target, bool newInstance,
        MappingHelper mappingHelper, AddFailure addFailure)
    {
        T RequireValue<T>(string propertyName, T? value, T defaultValue) where T : struct
        {
            if (value.HasValue)
            {
                return value.Value;
            }
            else
            {
                addFailure(new MapperResultFailure<ContactGroupInfo>(
                    HandbookReferences.InvalidSourceData<OmContactGroup>()
                    .WithMessage("Required property value is null")
                    .WithId(nameof(source.ContactGroupId), source.ContactGroupId)
                    .WithData(new { MissingProperty = propertyName })));
                return defaultValue;
            }
        }

        target.ContactGroupName = source.ContactGroupName;
        target.ContactGroupDisplayName = source.ContactGroupDisplayName;
        target.ContactGroupDescription = source.ContactGroupDescription;
        target.ContactGroupDynamicCondition = source.ContactGroupDynamicCondition;
        target.ContactGroupEnabled = RequireValue(nameof(source.ContactGroupEnabled), source.ContactGroupEnabled, false);
        target.ContactGroupLastModified = RequireValue(nameof(source.ContactGroupLastModified), source.ContactGroupLastModified, DateTime.MinValue);
        target.ContactGroupGUID = RequireValue(nameof(source.ContactGroupGuid), source.ContactGroupGuid, Guid.Empty);
        target.ContactGroupStatus = (ContactGroupStatusEnum)RequireValue(nameof(source.ContactGroupStatus), source.ContactGroupStatus, 0);

        return target;
    }
}
