using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Contexts;

public class SourceInstanceContext(
    IpcService ipcService,
    ILogger<SourceInstanceContext> logger,
    ToolkitConfiguration configuration,
    ModelFacade modelFacade)
{
    private readonly Dictionary<string, SourceInstanceDiscoveredInfo> _cachedInfos = new(StringComparer.InvariantCultureIgnoreCase);

    private bool _sourceInfoLoaded;

    public bool HasInfo => _cachedInfos.Count > 0 && _sourceInfoLoaded;

    public bool IsQuerySourceInstanceEnabled() => configuration.OptInFeatures?.QuerySourceInstanceApi?.Enabled ?? false;

    public async Task<bool> RequestSourceInstanceInfo()
    {
        if (!_sourceInfoLoaded)
        {
            var result = await ipcService.GetSourceInstanceDiscoveredInfos();
            foreach ((string key, var value) in result)
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
        string siteName =
            modelFacade.SelectById<ICmsSite>(siteId)?.SiteName
            ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        return GetWidgetPropertyFormComponents(siteName, widgetIdentifier);
    }

    public List<EditingFormControlModel>? GetPageTemplateFormComponents(int siteId, string pageTemplateIdentifier)
    {
        string siteName =
            modelFacade.SelectById<ICmsSite>(siteId)?.SiteName
            ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        return GetPageTemplateFormComponents(siteName, pageTemplateIdentifier);
    }

    public List<EditingFormControlModel>? GetSectionFormComponents(int siteId, string sectionIdentifier)
    {
        string siteName =
            modelFacade.SelectById<ICmsSite>(siteId)?.SiteName
            ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        if (_cachedInfos.TryGetValue(siteName, out var info))
        {
            return info.SectionProperties != null && info.SectionProperties.TryGetValue(sectionIdentifier, out var sectionFcs)
                ? sectionFcs
                : null;
        }

        throw new InvalidOperationException($"No info was loaded for site '{siteName}'");
    }
}
