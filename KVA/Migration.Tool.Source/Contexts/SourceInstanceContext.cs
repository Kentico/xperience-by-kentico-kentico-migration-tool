using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Services.Ipc;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Contexts;

public class SourceInstanceContext(
    IpcService ipcService,
    ILogger<SourceInstanceContext> logger,
    ToolConfiguration configuration,
    ModelFacade modelFacade)
{
    /// <summary>
    /// key is SiteName
    /// </summary>
    private readonly Dictionary<string, SourceInstanceDiscoveredInfo> cachedInfos = new(StringComparer.InvariantCultureIgnoreCase);

    private bool sourceInfoLoaded;

    public bool HasInfo => cachedInfos.Count > 0 && sourceInfoLoaded;

    public bool IsQuerySourceInstanceEnabled() => configuration.OptInFeatures?.QuerySourceInstanceApi?.Enabled ?? false;

    public async Task<bool> RequestSourceInstanceInfo()
    {
        if (!sourceInfoLoaded)
        {
            var result = await ipcService.GetSourceInstanceDiscoveredInfos();
            foreach ((string key, var value) in result)
            {
                cachedInfos.Add(key, value);
                logger.LogInformation("Source instance info loaded for site '{SiteName}' successfully", key);
            }

            sourceInfoLoaded = true;
        }

        return sourceInfoLoaded;
    }

    public List<EditingFormControlModel>? GetWidgetPropertyFormComponents(string siteName, string widgetIdentifier)
    {
        if (cachedInfos.TryGetValue(siteName, out var info))
        {
            return info.WidgetProperties != null && info.WidgetProperties.TryGetValue(widgetIdentifier, out var widgetProperties)
                ? widgetProperties
                : null;
        }

        throw new InvalidOperationException($"No info was loaded for site '{siteName}'");
    }

    public List<EditingFormControlModel>? GetPageTemplateFormComponents(string siteName, string pageTemplateIdentifier)
    {
        if (cachedInfos.TryGetValue(siteName, out var info))
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

        if (cachedInfos.TryGetValue(siteName, out var info))
        {
            return info.SectionProperties != null && info.SectionProperties.TryGetValue(sectionIdentifier, out var sectionFcs)
                ? sectionFcs
                : null;
        }

        throw new InvalidOperationException($"No info was loaded for site '{siteName}'");
    }

    public IList<PageModel> GetNodeUrls(int nodeId, string siteName)
    {
        if (cachedInfos.TryGetValue(siteName, out var info) && info?.PageModels is { Count: > 0 } pageModels)
        {
            return pageModels.Where(pm => pm.NodeId == nodeId).ToList();
        }
        else
        {
            return [];
        }
    }
}
