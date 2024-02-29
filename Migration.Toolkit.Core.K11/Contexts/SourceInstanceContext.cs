namespace Migration.Toolkit.Core.K11.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.K11;

public class SourceInstanceContext(IpcService ipcService, IDbContextFactory<K11Context> k11ContextFactory, ILogger<SourceInstanceContext> logger,
    ToolkitConfiguration configuration)
{
    private readonly Dictionary<string, SourceInstanceDiscoveredInfo> _cachedInfos = new(StringComparer.InvariantCultureIgnoreCase);

    private bool _sourceInfoLoaded;

    public bool HasInfo => _cachedInfos.Count > 0 && _sourceInfoLoaded;

    public bool IsQuerySourceInstanceEnabled()
    {
        return configuration.OptInFeatures?.QuerySourceInstanceApi?.Enabled ?? false;
    }

    public async Task<bool> RequestSourceInstanceInfo()
    {
        if (!_sourceInfoLoaded)
        {
            var result = await ipcService.GetSourceInstanceDiscoveredInfos();
            foreach (var (key, value) in result)
            {
                _cachedInfos.Add(key, value);
                logger.LogInformation("Source instance info loaded for site '{SiteName}' successfully", key);
            }

            _sourceInfoLoaded = true;
        }

        return _sourceInfoLoaded;
    }

    public List<EditingFormControlModel>? GetWidgetPropertyFormComponents(string siteName, string widgetIdentifier)
    {
        if (_cachedInfos.TryGetValue(siteName, out var info))
        {
            return info.WidgetProperties != null && info.WidgetProperties.TryGetValue(widgetIdentifier, out var widgetProperties)
                ? widgetProperties
                : null;
        }

        throw new InvalidOperationException($"No info was loaded for site '{siteName}'");
    }

    public List<EditingFormControlModel>? GetPageTemplateFormComponents(string siteName, string pageTemplateIdentifier)
    {
        if (_cachedInfos.TryGetValue(siteName, out var info))
        {
            return info.PageTemplateProperties != null && info.PageTemplateProperties.TryGetValue(pageTemplateIdentifier, out var pageTemplate)
                ? pageTemplate
                : null;
        }

        throw new InvalidOperationException($"No info was loaded for site '{siteName}'");
    }

    public List<EditingFormControlModel>? GetWidgetPropertyFormComponents(int siteId, string widgetIdentifier)
    {
        var context = k11ContextFactory.CreateDbContext();
        var siteName = context.CmsSites.FirstOrDefault(s => s.SiteId == siteId)?.SiteName
                       ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        return GetWidgetPropertyFormComponents(siteName, widgetIdentifier);
    }

    public List<EditingFormControlModel>? GetPageTemplateFormComponents(int siteId, string pageTemplateIdentifier)
    {
        var context = k11ContextFactory.CreateDbContext();
        var siteName = context.CmsSites.FirstOrDefault(s => s.SiteId == siteId)?.SiteName
                       ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        return GetPageTemplateFormComponents(siteName, pageTemplateIdentifier);
    }

    public List<EditingFormControlModel>? GetSectionFormComponents(int siteId, string sectionIdentifier)
    {
        var context = k11ContextFactory.CreateDbContext();
        var siteName = context.CmsSites.FirstOrDefault(s => s.SiteId == siteId)?.SiteName
                       ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        if (_cachedInfos.TryGetValue(siteName, out var info))
        {
            return info.SectionProperties != null && info.SectionProperties.TryGetValue(sectionIdentifier, out var sectionFcs)
                ? sectionFcs
                : null;
        }
        else
        {
            throw new InvalidOperationException($"No info was loaded for site '{siteName}'");
        }
    }
}