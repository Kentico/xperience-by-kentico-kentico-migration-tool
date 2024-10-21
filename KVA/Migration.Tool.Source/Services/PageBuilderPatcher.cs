﻿using CMS.ContentEngine;
using CMS.MediaLibrary;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Model;
using Migration.Tool.Common.Services.Ipc;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Services;

public record PageBuilderPatchResult(string? Configuration, string? Widgets, bool NeedsDeferredPatch);

public class PageBuilderPatcher(
    ILogger<PageBuilderPatcher> logger,
    SourceInstanceContext sourceInstanceContext,
    WidgetMigrationService widgetMigrationService,
    ModelFacade modelFacade,
    IAttachmentMigrator attachmentMigrator
)
{
    public async Task<PageBuilderPatchResult> PatchJsonDefinitions(int sourceSiteId, string? pageTemplateConfiguration, string? pageBuilderWidgets)
    {
        bool needsDeferredPatch = false;
        if (sourceInstanceContext.HasInfo)
        {
            if (pageTemplateConfiguration != null)
            {
                var pageTemplateConfigurationObj = JsonConvert.DeserializeObject<PageTemplateConfiguration>(pageTemplateConfiguration);
                if (pageTemplateConfigurationObj?.Identifier != null)
                {
                    logger.LogTrace("Walk page template configuration {Identifier}", pageTemplateConfigurationObj.Identifier);

                    var pageTemplateConfigurationFcs =
                        sourceInstanceContext.GetPageTemplateFormComponents(sourceSiteId, pageTemplateConfigurationObj.Identifier);
                    if (pageTemplateConfigurationObj.Properties is { Count: > 0 })
                    {
                        bool ndp = await MigrateProperties(sourceSiteId, pageTemplateConfigurationObj.Properties, pageTemplateConfigurationFcs);
                        needsDeferredPatch = ndp || needsDeferredPatch;
                    }

                    pageTemplateConfiguration = JsonConvert.SerializeObject(pageTemplateConfigurationObj);
                }
            }

            if (pageBuilderWidgets != null)
            {
                var areas = JsonConvert.DeserializeObject<EditableAreasConfiguration>(pageBuilderWidgets);
                if (areas?.EditableAreas is { Count: > 0 })
                {
                    bool ndp = await WalkAreas(sourceSiteId, areas.EditableAreas);
                    needsDeferredPatch = ndp || needsDeferredPatch;
                }

                pageBuilderWidgets = JsonConvert.SerializeObject(areas);
            }
        }

        return new PageBuilderPatchResult(pageTemplateConfiguration, pageBuilderWidgets, needsDeferredPatch);
    }

    #region "Page template & page widget walkers"

    private async Task<bool> WalkAreas(int siteId, List<EditableAreaConfiguration> areas)
    {
        bool needsDeferredPatch = false;
        foreach (var area in areas)
        {
            logger.LogTrace("Walk area {Identifier}", area.Identifier);

            if (area.Sections is { Count: > 0 })
            {
                bool ndp = await WalkSections(siteId, area.Sections);
                needsDeferredPatch = ndp || needsDeferredPatch;
            }
        }

        return needsDeferredPatch;
    }

    private async Task<bool> WalkSections(int siteId, List<SectionConfiguration> sections)
    {
        bool needsDeferredPatch = false;
        foreach (var section in sections)
        {
            logger.LogTrace("Walk section {TypeIdentifier}|{Identifier}", section.TypeIdentifier, section.Identifier);

            var sectionFcs = sourceInstanceContext.GetSectionFormComponents(siteId, section.TypeIdentifier);
            bool ndp1 = await MigrateProperties(siteId, section.Properties, sectionFcs);
            needsDeferredPatch = ndp1 || needsDeferredPatch;

            if (section.Zones is { Count: > 0 })
            {
                bool ndp = await WalkZones(siteId, section.Zones);
                needsDeferredPatch = ndp || needsDeferredPatch;
            }
        }

        return needsDeferredPatch;
    }

    private async Task<bool> WalkZones(int siteId, List<ZoneConfiguration> zones)
    {
        bool needsDeferredPatch = false;
        foreach (var zone in zones)
        {
            logger.LogTrace("Walk zone {Name}|{Identifier}", zone.Name, zone.Identifier);

            if (zone.Widgets is { Count: > 0 })
            {
                bool ndp = await WalkWidgets(siteId, zone.Widgets);
                needsDeferredPatch = ndp || needsDeferredPatch;
            }
        }

        return needsDeferredPatch;
    }

    private async Task<bool> WalkWidgets(int siteId, List<WidgetConfiguration> widgets)
    {
        bool needsDeferredPatch = false;
        foreach (var widget in widgets)
        {
            logger.LogTrace("Walk widget {TypeIdentifier}|{Identifier}", widget.TypeIdentifier, widget.Identifier);
            var widgetCompos = sourceInstanceContext.GetWidgetPropertyFormComponents(siteId, widget.TypeIdentifier);

            foreach (var variant in widget.Variants)
            {
                logger.LogTrace("Migrating widget variant {Name}|{Identifier}", variant.Name, variant.Identifier);

                if (variant.Properties is { Count: > 0 } properties)
                {
                    foreach ((string key, var value) in properties)
                    {
                        logger.LogTrace("Migrating widget property {Name}|{Identifier}", key, value?.ToString());
                        await MigrateProperties(siteId, properties, widgetCompos);
                    }
                }
            }
        }

        return needsDeferredPatch;
    }

    private async Task<bool> MigrateProperties(int siteId, JObject properties, List<EditingFormControlModel>? formControlModels)
    {
        bool needsDeferredPatch = false;
        foreach ((string key, var value) in properties)
        {
            logger.LogTrace("Walk property {Name}|{Identifier}", key, value?.ToString());

            var editingFcm = formControlModels?.FirstOrDefault(x => x.PropertyName.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (editingFcm != null)
            {
                var context = new WidgetPropertyMigrationContext(siteId, editingFcm);
                var widgetPropertyMigration = widgetMigrationService.GetWidgetPropertyMigrations(context, key);
                bool allowDefaultMigrations = true;
                bool customMigrationApplied = false;
                if (widgetPropertyMigration != null)
                {
                    (var migratedValue, bool ndp, allowDefaultMigrations) = await widgetPropertyMigration.MigrateWidgetProperty(key, value, context);
                    needsDeferredPatch = ndp || needsDeferredPatch;
                    properties[key] = migratedValue;
                    customMigrationApplied = true;
                    logger.LogTrace("Migration {Migration} applied to {Value}, resulting in {Result}", widgetPropertyMigration.GetType().FullName, value?.ToString() ?? "<null>", migratedValue?.ToString() ?? "<null>");
                }

                if (allowDefaultMigrations)
                {
                    if (FieldMappingInstance.BuiltInModel.NotSupportedInKxpLegacyMode
                            .SingleOrDefault(x => x.OldFormComponent == editingFcm.FormComponentIdentifier) is var (oldFormComponent, newFormComponent))
                    {
                        logger.LogTrace("Editing form component found {FormComponentName} => no longer supported {Replacement}", editingFcm.FormComponentIdentifier, newFormComponent);

                        switch (oldFormComponent)
                        {
                            case Kx13FormComponents.Kentico_AttachmentSelector when newFormComponent == FormComponents.AdminAssetSelectorComponent:
                            {
                                if (value?.ToObject<List<AttachmentSelectorItem>>() is { Count: > 0 } items)
                                {
                                    var nv = new List<object>();
                                    foreach (var asi in items)
                                    {
                                        var attachment = modelFacade.SelectWhere<ICmsAttachment>("AttachmentSiteID = @attachmentSiteId AND AttachmentGUID = @attachmentGUID",
                                                new SqlParameter("attachmentSiteID", siteId),
                                                new SqlParameter("attachmentGUID", asi.FileGuid)
                                            )
                                            .FirstOrDefault();
                                        if (attachment != null)
                                        {
                                            switch (attachmentMigrator.MigrateAttachment(attachment).GetAwaiter().GetResult())
                                            {
                                                case MigrateAttachmentResultMediaFile { Success: true, MediaFileInfo: { } x }:
                                                {
                                                    nv.Add(new AssetRelatedItem { Identifier = x.FileGUID, Dimensions = new AssetDimensions { Height = x.FileImageHeight, Width = x.FileImageWidth }, Name = x.FileName, Size = x.FileSize });
                                                    break;
                                                }
                                                case MigrateAttachmentResultContentItem { Success: true, ContentItemGuid: { } contentItemGuid }:
                                                {
                                                    nv.Add(new ContentItemReference { Identifier = contentItemGuid });
                                                    break;
                                                }
                                                default:
                                                {
                                                    logger.LogWarning("Attachment '{AttachmentGUID}' failed to migrate", asi.FileGuid);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            logger.LogWarning("Attachment '{AttachmentGUID}' not found", asi.FileGuid);
                                        }
                                    }

                                    properties[key] = JToken.FromObject(nv);
                                }

                                logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                                break;
                            }

                            default:
                                break;
                        }
                    }
                    else if (!customMigrationApplied)
                    {
                        if (FieldMappingInstance.BuiltInModel.SupportedInKxpLegacyMode.Contains(editingFcm.FormComponentIdentifier))
                        {
                            // OK
                            logger.LogTrace("Editing form component found {FormComponentName} => supported in legacy mode", editingFcm.FormComponentIdentifier);
                        }
                        else
                        {
                            // unknown control, probably custom
                            logger.LogTrace("Editing form component found {FormComponentName} => custom or inlined component, don't forget to migrate code accordingly", editingFcm.FormComponentIdentifier);
                        }
                    }


                    if ("NodeAliasPath".Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        needsDeferredPatch = true;
                        properties["TreePath"] = value;
                        properties.Remove(key);
                    }
                }
            }
        }

        return needsDeferredPatch;
    }

    #endregion
}
