using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;

namespace Migration.Toolkit.Core.Mappers;

using Migration.Toolkit.KXP.Models;

public class CmsResourceMapper : EntityMapperBase<Migration.Toolkit.KX13.Models.CmsResource, CmsResource>
{
    private readonly ILogger<CmsResourceMapper> _logger;

    public CmsResourceMapper(ILogger<CmsResourceMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IMigrationProtocol protocol) :
        base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
    }

    protected override CmsResource? CreateNewInstance(KX13.Models.CmsResource source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override CmsResource MapInternal(KX13.Models.CmsResource source, CmsResource target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (source.ResourceGuid != target.ResourceGuid)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch on resources S={SourceGuild}, T={TargetGuid}", source.ResourceGuid, target.ResourceGuid);
            // allowing to run through, same resource is not required for target instance
            // return new ModelMappingFailedKeyMismatch<Migration.Toolkit.KXP.Models.CmsResource>();
        }

        // avoid updating resource
        if (!newInstance) return target;

        target.ResourceDisplayName = source.ResourceDisplayName;
        target.ResourceName = source.ResourceName;
        target.ResourceDescription = source.ResourceDescription;
        target.ShowInDevelopment = source.ShowInDevelopment;
        target.ResourceUrl = source.ResourceUrl;
        target.ResourceGuid = source.ResourceGuid;
        target.ResourceLastModified = source.ResourceLastModified;
        target.ResourceIsInDevelopment = source.ResourceIsInDevelopment;
        target.ResourceHasFiles = source.ResourceHasFiles;
        target.ResourceVersion = source.ResourceVersion;
        target.ResourceAuthor = source.ResourceAuthor;
        target.ResourceInstallationState = source.ResourceInstallationState;
        target.ResourceInstalledVersion = source.ResourceInstalledVersion;

        return target;
    }
}