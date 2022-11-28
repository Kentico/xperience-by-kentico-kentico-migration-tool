using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using CMS.Membership;
using Migration.Toolkit.KXP.Models;

public class RoleInfoMapper : EntityMapperBase<KX13.Models.CmsRole, RoleInfo>
{
    public RoleInfoMapper(
        ILogger<RoleInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
    }

    protected override RoleInfo? CreateNewInstance(KX13.Models.CmsRole source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override RoleInfo MapInternal(KX13.Models.CmsRole source, RoleInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.RoleDisplayName = source.RoleDisplayName;
        target.RoleName = source.RoleName;
        target.RoleDescription = source.RoleDescription;

        if (mappingHelper.TranslateIdAllowNulls<KX13.Models.CmsSite>(c => c.SiteId, source.SiteId, out var siteId))
        {
            target.SiteID = siteId ?? 0;
        }

        target.RoleGUID = source.RoleGuid;
        target.RoleLastModified = source.RoleLastModified;
        // target. = source.RoleIsDomain;

        return target;
    }
}