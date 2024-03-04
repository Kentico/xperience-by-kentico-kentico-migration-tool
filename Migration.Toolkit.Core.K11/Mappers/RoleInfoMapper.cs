namespace Migration.Toolkit.Core.K11.Mappers;

using CMS.Membership;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

public class RoleInfoMapper(ILogger<RoleInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : EntityMapperBase<CmsRole, RoleInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override RoleInfo? CreateNewInstance(CmsRole source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override RoleInfo MapInternal(CmsRole source, RoleInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.RoleDisplayName = source.RoleDisplayName;
        target.RoleName = source.RoleName;
        target.RoleDescription = source.RoleDescription;
        target.RoleGUID = source.RoleGuid;
        target.RoleLastModified = source.RoleLastModified;
        return target;
    }
}