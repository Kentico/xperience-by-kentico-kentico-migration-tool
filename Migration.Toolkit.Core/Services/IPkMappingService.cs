namespace Migration.Toolkit.Core.Services;

public interface IPkMappingService
{
    bool TryMapSiteId(int? sourceSiteId, out int? mappedSiteId);
}