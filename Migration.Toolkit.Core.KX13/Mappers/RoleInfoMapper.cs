namespace Migration.Toolkit.Core.KX13.Mappers;

using CMS.Membership;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX13.Contexts;

public class RoleInfoMapper : EntityMapperBase<KX13M.CmsRole, RoleInfo>
{
    public RoleInfoMapper(
        ILogger<RoleInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override RoleInfo? CreateNewInstance(Toolkit.KX13.Models.CmsRole source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override RoleInfo MapInternal(Toolkit.KX13.Models.CmsRole source, RoleInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.RoleDisplayName = source.RoleDisplayName;
        target.RoleName = source.RoleName;
        target.RoleDescription = source.RoleDescription;
        target.RoleGUID = source.RoleGuid;
        target.RoleLastModified = source.RoleLastModified;
        return target;
    }
}