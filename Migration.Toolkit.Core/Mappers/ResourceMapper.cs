namespace Migration.Toolkit.Core.Mappers;

using CMS.Modules;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.KX13.Models;

public class ResourceMapper : EntityMapperBase<KX13M.CmsResource, ResourceInfo>
{
    private readonly ILogger<ResourceMapper> _logger;

    public ResourceMapper(ILogger<ResourceMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : base(logger, pkContext, protocol)
    {
        _logger = logger;
    }

    protected override ResourceInfo? CreateNewInstance(CmsResource source, MappingHelper mappingHelper, AddFailure addFailure)
        => ResourceInfo.New();

    protected override ResourceInfo MapInternal(CmsResource source, ResourceInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ResourceDescription = source.ResourceDescription;
        target.ResourceDisplayName = source.ResourceDisplayName;
        target.ResourceGUID = source.ResourceGuid;
        target.ResourceIsInDevelopment = false; // TODO tk: 2022-10-10 if true, module is not shown in UI of XbK
        target.ResourceLastModified = source.ResourceLastModified;
        target.ResourceName = source.ResourceName;

        if (target.ResourceName == Kx13SystemResource.Licenses)
        {
            target.ResourceName = XbkSystemResource.CMS_Licenses;
            _logger.LogInformation("Patching CMS Resource 'Licences': name changed to '{ResourceNamePatched}'", XbkSystemResource.CMS_Licenses);
        }

        if (!XbkSystemResource.All.Contains(target.ResourceName) || Kx13SystemResource.ConvertToNonSysResource.Contains(target.ResourceName))
        {
            // custom resource

            if (target.ResourceName.StartsWith("CMS.", StringComparison.InvariantCultureIgnoreCase))
            {
                var targetResourceNamePatched = target.ResourceName.Substring(4, target.ResourceName.Length - 4);
                _logger.LogInformation("Patching CMS Resource '{ResourceName}': name changed to '{ResourceNamePatched}'",  target.ResourceName, targetResourceNamePatched);
                target.ResourceName = targetResourceNamePatched;
            }
        }

        return target;
    }
}