namespace Migration.Toolkit.Core.K11.Mappers;

using CMS.Modules;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;

public class ResourceMapper(ILogger<ResourceMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol) : EntityMapperBase<CmsResource, ResourceInfo>(logger, pkContext, protocol)
{
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

        if (target.ResourceName == K12SystemResource.Licenses)
        {
            target.ResourceName = XbkSystemResource.CMS_Licenses;
            logger.LogInformation("Patching CMS Resource 'Licences': name changed to '{ResourceNamePatched}'", XbkSystemResource.CMS_Licenses);
        }

        if (!XbkSystemResource.All.Contains(target.ResourceName) || K12SystemResource.ConvertToNonSysResource.Contains(target.ResourceName))
        {
            // custom resource

            if (target.ResourceName.StartsWith("CMS.", StringComparison.InvariantCultureIgnoreCase))
            {
                var targetResourceNamePatched = target.ResourceName.Substring(4, target.ResourceName.Length - 4);
                logger.LogInformation("Patching CMS Resource '{ResourceName}': name changed to '{ResourceNamePatched}'",  target.ResourceName, targetResourceNamePatched);
                target.ResourceName = targetResourceNamePatched;
            }
        }

        return target;
    }
}