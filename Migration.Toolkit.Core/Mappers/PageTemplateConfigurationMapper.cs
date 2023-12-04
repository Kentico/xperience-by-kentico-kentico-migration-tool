namespace Migration.Toolkit.Core.Mappers;


using CMS.MediaLibrary;
using CMS.Websites;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Common.Services.Ipc;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Services.CmsClass;
using Migration.Toolkit.KX13.Auxiliary;
using Migration.Toolkit.KX13.Models;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PageTemplateConfigurationMapper : EntityMapperBase<KX13M.CmsPageTemplateConfiguration, PageTemplateConfigurationInfo>
{
    private readonly ILogger<PageTemplateConfigurationMapper> _logger;
    private readonly SourceInstanceContext _sourceInstanceContext;

    public PageTemplateConfigurationMapper(
        ILogger<PageTemplateConfigurationMapper> logger,
        PrimaryKeyMappingContext pkContext,
        IProtocol protocol,
        SourceInstanceContext sourceInstanceContext
    ) : base(logger, pkContext, protocol)
    {
        _logger = logger;
        _sourceInstanceContext = sourceInstanceContext;
    }

    protected override PageTemplateConfigurationInfo? CreateNewInstance(CmsPageTemplateConfiguration source, MappingHelper mappingHelper,
        AddFailure addFailure)
        => PageTemplateConfigurationInfo.New();

    protected override PageTemplateConfigurationInfo MapInternal(KX13M.CmsPageTemplateConfiguration source, PageTemplateConfigurationInfo target,
        bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        target.PageTemplateConfigurationDescription = source.PageTemplateConfigurationDescription;
        target.PageTemplateConfigurationName = source.PageTemplateConfigurationName;
        target.PageTemplateConfigurationLastModified = source.PageTemplateConfigurationLastModified;
        target.PageTemplateConfigurationIcon = "xp-custom-element"; // TODO tomas.krch: 2023-11-27 some better default icon pick?

        if (newInstance)
        {
            target.PageTemplateConfigurationGUID = source.PageTemplateConfigurationGuid;
        }

        if (_sourceInstanceContext.HasInfo)
        {
            if (source.PageTemplateConfigurationTemplate != null)
            {
                var pageTemplateConfiguration = JsonConvert.DeserializeObject<PageTemplateConfiguration>(source.PageTemplateConfigurationTemplate);
                if (pageTemplateConfiguration?.Identifier != null)
                {
                    _logger.LogTrace("Walk page template configuration {Identifier}", pageTemplateConfiguration.Identifier);


                    var pageTemplateConfigurationFcs =
                        _sourceInstanceContext.GetPageTemplateFormComponents(source.PageTemplateConfigurationSiteId, pageTemplateConfiguration.Identifier);
                    if (pageTemplateConfiguration.Properties is { Count: > 0 })
                    {
                        WalkProperties(pageTemplateConfiguration.Properties, pageTemplateConfigurationFcs);
                    }

                    target.PageTemplateConfigurationTemplate = JsonConvert.SerializeObject(pageTemplateConfiguration);
                }
            }

            if (source.PageTemplateConfigurationWidgets != null)
            {
                var areas = JsonConvert.DeserializeObject<EditableAreasConfiguration>(source.PageTemplateConfigurationWidgets);
                if (areas?.EditableAreas is { Count : > 0 })
                {
                    WalkAreas(source.PageTemplateConfigurationSiteId, areas.EditableAreas);
                }

                target.PageTemplateConfigurationWidgets = JsonConvert.SerializeObject(areas);
            }
        }
        else
        {
            // simply copy if no info is available
            target.PageTemplateConfigurationTemplate = source.PageTemplateConfigurationTemplate;
            target.PageTemplateConfigurationWidgets = source.PageTemplateConfigurationWidgets;
        }

        return target;
    }

    // TODO tk: 2022-09-14 move walker logic to separate class
    #region "Page template & page widget walkers"

    private void WalkAreas(int siteId, List<EditableAreaConfiguration> areas)
    {
        foreach (var area in areas)
        {
            _logger.LogTrace("Walk area {Identifier}", area.Identifier);

            if (area.Sections is { Count: > 0 })
            {
                WalkSections(siteId, area.Sections);
            }
        }
    }

    private void WalkSections(int siteId, List<SectionConfiguration> sections)
    {
        foreach (var section in sections)
        {
            _logger.LogTrace("Walk section {TypeIdentifier}|{Identifier}", section.TypeIdentifier, section.Identifier);

            var sectionFcs = _sourceInstanceContext.GetSectionFormComponents(siteId, section.TypeIdentifier);
            WalkProperties(section.Properties, sectionFcs);

            if (section.Zones is { Count: > 0 })
            {
                WalkZones(siteId, section.Zones);
            }
        }
    }

    private void WalkZones(int siteId, List<ZoneConfiguration> zones)
    {
        foreach (var zone in zones)
        {
            _logger.LogTrace("Walk zone {Name}|{Identifier}", zone.Name, zone.Identifier);

            if (zone.Widgets is { Count: > 0 })
            {
                WalkWidgets(siteId, zone.Widgets);
            }
        }
    }

    private void WalkWidgets(int siteId, List<WidgetConfiguration> widgets)
    {
        foreach (var widget in widgets)
        {
            _logger.LogTrace("Walk widget {TypeIdentifier}|{Identifier}", widget.TypeIdentifier, widget.Identifier);

            var widgetFcs = _sourceInstanceContext.GetWidgetPropertyFormComponents(siteId, widget.TypeIdentifier);
            foreach (var variant in widget.Variants)
            {
                _logger.LogTrace("Walk widget variant {Name}|{Identifier}", variant.Name, variant.Identifier);

                if (variant.Properties is { Count: > 0 })
                {
                    WalkProperties(variant.Properties, widgetFcs);
                }
            }
        }
    }

    private void WalkProperties(JObject properties, List<EditingFormControlModel>? formControlModels)
    {
        foreach (var (key, value) in properties)
        {
            _logger.LogTrace("Walk property {Name}|{Identifier}", key, value?.ToString());

            var editingFcm = formControlModels?.FirstOrDefault(x => x.PropertyName.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (editingFcm != null)
            {
                if (FieldMappingInstance.BuiltInModel.NotSupportedInKxpLegacyMode
                        .SingleOrDefault(x => x.OldFormComponent == editingFcm.FormComponentIdentifier) is var (oldFormComponent, newFormComponent))
                {
                    Protocol.Append(HandbookReferences.FormComponentNotSupportedInLegacyMode(oldFormComponent, newFormComponent));
                    _logger.LogTrace("Editing form component found {FormComponentName} => no longer supported {Replacement}",
                        editingFcm.FormComponentIdentifier, newFormComponent);

                    switch (oldFormComponent)
                    {
                        case Kx13FormComponents.Kentico_AttachmentSelector when newFormComponent == FormComponents.AdminAssetSelectorComponent:
                        {
                            if (value?.ToObject<List<AttachmentSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new AssetRelatedItem
                                {
                                    Identifier = x.FileGuid
                                }).ToList());
                            }

                            _logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }
                        case Kx13FormComponents.Kentico_PageSelector when newFormComponent == FormComponents.AdminPageSelectorComponent:
                        {
                            if (value?.ToObject<List<PageSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new WebPageRelatedItem
                                {
                                    WebPageGuid = x.NodeGuid
                                }).ToList());
                            }

                            _logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }
                    }
                }
                else if (FieldMappingInstance.BuiltInModel.SupportedInKxpLegacyMode.Contains(editingFcm.FormComponentIdentifier))
                {
                    // OK
                    _logger.LogTrace("Editing form component found {FormComponentName} => supported in legacy mode",
                        editingFcm.FormComponentIdentifier);
                }
                else
                {
                    // unknown control, probably custom
                    Protocol.Append(HandbookReferences.FormComponentCustom(editingFcm.FormComponentIdentifier));
                    _logger.LogTrace(
                        "Editing form component found {FormComponentName} => custom or inlined component, don't forget to migrate code accordingly",
                        editingFcm.FormComponentIdentifier);
                }
            }
        }
    }

    #endregion
}