
using CMS.Modules;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Mappers;
public class ResourceMapper(ILogger<ResourceMapper> logger, PrimaryKeyMappingContext pkContext, IProtocol protocol)
    : EntityMapperBase<ICmsResource, ResourceInfo>(logger, pkContext, protocol)
{
    protected override ResourceInfo? CreateNewInstance(ICmsResource source, MappingHelper mappingHelper, AddFailure addFailure)
        => ResourceInfo.New();

    protected override ResourceInfo MapInternal(ICmsResource source, ResourceInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.ResourceDescription = source.ResourceDescription;
        target.ResourceDisplayName = source.ResourceDisplayName;
        target.ResourceGUID = source.ResourceGUID;
        target.ResourceIsInDevelopment = false; // TODO tk: 2022-10-10 if true, module is not shown in UI of XbK
        target.ResourceLastModified = source.ResourceLastModified;
        target.ResourceName = source.ResourceName;

        if (target.ResourceName == Kx13SystemResource.Licenses)
        {
            target.ResourceName = XbkSystemResource.CMS_Licenses;
            logger.LogInformation("Patching CMS Resource 'Licences': name changed to '{ResourceNamePatched}'", XbkSystemResource.CMS_Licenses);
        }

        if (!XbkSystemResource.All.Contains(target.ResourceName) || Kx13SystemResource.ConvertToNonSysResource.Contains(target.ResourceName))
        {
            // custom resource

            if (target.ResourceName.StartsWith("CMS.", StringComparison.InvariantCultureIgnoreCase))
            {
                string targetResourceNamePatched = target.ResourceName[4..];
                logger.LogInformation("Patching CMS Resource '{ResourceName}': name changed to '{ResourceNamePatched}'", target.ResourceName, targetResourceNamePatched);
                target.ResourceName = targetResourceNamePatched;
            }
        }

        return target;
    }
}
