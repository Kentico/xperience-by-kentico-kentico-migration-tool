using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KXP.Models;

public class CmsRoleMapper : EntityMapperBase<KX13.Models.CmsRole, CmsRole>
{
    public CmsRoleMapper(
        ILogger<CmsRoleMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override CmsRole? CreateNewInstance(KX13.Models.CmsRole source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsRole MapInternal(KX13.Models.CmsRole source, CmsRole target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.RoleDisplayName = source.RoleDisplayName;
        target.RoleName = source.RoleName;
        target.RoleDescription = source.RoleDescription;
        
        if (mappingHelper.TranslateId<KX13.Models.CmsSite>(c => c.SiteId, source.SiteId, out var siteId))
        {
            target.SiteId = siteId;
        }

        target.RoleGuid = source.RoleGuid;
        target.RoleLastModified = source.RoleLastModified;
        target.RoleIsDomain = source.RoleIsDomain;

        return target;
    }
}