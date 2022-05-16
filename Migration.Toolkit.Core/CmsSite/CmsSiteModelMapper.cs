using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using K13M = Migration.Toolkit.KX13.Models;
using K14M = Migration.Toolkit.KXO.Models;

namespace Migration.Toolkit.Core.CmsSite;

public class CmsSiteModelMapper : IEntityMapper<Migration.Toolkit.KX13.Models.CmsSite, Migration.Toolkit.KXO.Models.CmsSite>
{
    private readonly ILogger<CmsSiteModelMapper> _logger;

    public CmsSiteModelMapper(ILogger<CmsSiteModelMapper> logger)
    {
        _logger = logger;
    }

    public ModelMappingResult<K14M.CmsSite> Map(K13M.CmsSite? source, K14M.CmsSite? target)
    {
        if (source is null)
        {
            _logger.LogTrace("Source entity is not defined.");
            return new ModelMappingFailedSourceNotDefined<K14M.CmsSite>();
        }

        var newInstance = false;
        if (target is null)
        {
            _logger.LogTrace("Null target supplied, creating new instance.");
            target = new K14M.CmsSite();
            newInstance = true;
        }
        else if (source.SiteGuid != target.SiteGuid)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity Guid mismatch.");
            return new ModelMappingFailedKeyMismatch<K14M.CmsSite>();
        }

        // map entity
        target.SiteGuid = source.SiteGuid;
        target.SiteName = source.SiteName;
        target.SiteStatus = source.SiteStatus;
        target.SiteDisplayName = source.SiteDisplayName;
        target.SiteDescription = source.SiteDescription;
        target.SiteDomainName = source.SiteDomainName;
        target.SiteLastModified = source.SiteLastModified;
        target.SiteDefaultVisitorCulture = source.SiteDefaultVisitorCulture;

        return new ModelMappingSuccess<K14M.CmsSite>(target, newInstance);
    }
}