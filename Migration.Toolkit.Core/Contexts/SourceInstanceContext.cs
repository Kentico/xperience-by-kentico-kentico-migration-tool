namespace Migration.Toolkit.Core.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.KX13.Context;

public class SourceInstanceContext
{
    private readonly IpcService _ipcService;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly ILogger<SourceInstanceContext> _logger;
    private readonly ToolkitConfiguration _configuration;

    private readonly Dictionary<string, SourceInstanceDiscoveredInfo> _cachedInfos = new(StringComparer.InvariantCultureIgnoreCase);

    private bool _sourceInfoLoaded;

    public bool HasInfo => _cachedInfos.Count > 0 && _sourceInfoLoaded;

    public SourceInstanceContext(IpcService ipcService, IDbContextFactory<KX13Context> kx13ContextFactory, ILogger<SourceInstanceContext> logger,
        ToolkitConfiguration configuration)
    {
        _ipcService = ipcService;
        _kx13ContextFactory = kx13ContextFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public bool IsQuerySourceInstanceEnabled()
    {
        return _configuration.OptInFeatures?.QuerySourceInstanceApi?.Enabled ?? false;
    }

    public async Task<bool> RequestSourceInstanceInfo()
    {
        if (!_sourceInfoLoaded)
        {
            var result = await _ipcService.GetSourceInstanceDiscoveredInfos();
            foreach (var (key, value) in result)
            {
                _cachedInfos.Add(key, value);
                _logger.LogInformation("Source instance info loaded for site '{SiteName}' successfully", key);
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
        var context = _kx13ContextFactory.CreateDbContext();
        var siteName = context.CmsSites.FirstOrDefault(s => s.SiteId == siteId)?.SiteName
                       ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        return GetWidgetPropertyFormComponents(siteName, widgetIdentifier);
    }

    public List<EditingFormControlModel>? GetPageTemplateFormComponents(int siteId, string pageTemplateIdentifier)
    {
        var context = _kx13ContextFactory.CreateDbContext();
        var siteName = context.CmsSites.FirstOrDefault(s => s.SiteId == siteId)?.SiteName
                       ?? throw new InvalidOperationException($"Source site with SiteID '{siteId}' not exists");

        return GetPageTemplateFormComponents(siteName, pageTemplateIdentifier);
    }

    public List<EditingFormControlModel>? GetSectionFormComponents(int siteId, string sectionIdentifier)
    {
        var context = _kx13ContextFactory.CreateDbContext();
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