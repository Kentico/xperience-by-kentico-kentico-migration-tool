using AngleSharp.Text;
using CMS.ContentEngine;
using CMS.MediaLibrary;
using CMS.Websites;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services.Ipc;
using Migration.Tool.KXP.Api;
using Migration.Tool.KXP.Api.Auxiliary;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;
using Migration.Tool.Source.Services.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers;

public class PageTemplateConfigurationMapper(
    ILogger<PageTemplateConfigurationMapper> logger,
    PrimaryKeyMappingContext pkContext,
    IProtocol protocol,
    SourceInstanceContext sourceInstanceContext,
    EntityIdentityFacade entityIdentityFacade,
    SpoiledGuidContext spoiledGuidContext,
    ModelFacade modelFacade,
    IAttachmentMigrator attachmentMigrator,
    IAssetFacade assetFacade,
    ToolConfiguration configuration,
    KxpMediaFileFacade mediaFileFacade
    )
    : EntityMapperBase<ICmsPageTemplateConfiguration, PageTemplateConfigurationInfo>(logger, pkContext, protocol)
{
    protected override PageTemplateConfigurationInfo? CreateNewInstance(ICmsPageTemplateConfiguration source, MappingHelper mappingHelper, AddFailure addFailure)
        => source switch
        {
            CmsPageTemplateConfigurationK11 => null,
            CmsPageTemplateConfigurationK12 => PageTemplateConfigurationInfo.New(),
            CmsPageTemplateConfigurationK13 => PageTemplateConfigurationInfo.New(),
            _ => null
        };

    protected override PageTemplateConfigurationInfo MapInternal(ICmsPageTemplateConfiguration s, PageTemplateConfigurationInfo target,
        bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (s is ICmsPageTemplateConfigurationK12K13 source)
        {
            target.PageTemplateConfigurationDescription = source.PageTemplateConfigurationDescription;
            target.PageTemplateConfigurationName = source.PageTemplateConfigurationName;
            target.PageTemplateConfigurationLastModified = source.PageTemplateConfigurationLastModified;
            target.PageTemplateConfigurationIcon = "xp-custom-element";

            if (newInstance)
            {
                target.PageTemplateConfigurationGUID = source.PageTemplateConfigurationGUID;
            }

            if (sourceInstanceContext.HasInfo)
            {
                if (source.PageTemplateConfigurationTemplate != null)
                {
                    var pageTemplateConfiguration = JsonConvert.DeserializeObject<PageTemplateConfiguration>(source.PageTemplateConfigurationTemplate);
                    if (pageTemplateConfiguration?.Identifier != null)
                    {
                        logger.LogTrace("Walk page template configuration {Identifier}", pageTemplateConfiguration.Identifier);


                        var pageTemplateConfigurationFcs =
                            sourceInstanceContext.GetPageTemplateFormComponents(source.PageTemplateConfigurationSiteID, pageTemplateConfiguration.Identifier);
                        if (pageTemplateConfiguration.Properties is { Count: > 0 })
                        {
                            WalkProperties(source.PageTemplateConfigurationSiteID, pageTemplateConfiguration.Properties, pageTemplateConfigurationFcs);
                        }

                        target.PageTemplateConfigurationTemplate = JsonConvert.SerializeObject(pageTemplateConfiguration);
                    }
                }

                if (source.PageTemplateConfigurationWidgets != null)
                {
                    var areas = JsonConvert.DeserializeObject<EditableAreasConfiguration>(source.PageTemplateConfigurationWidgets);
                    if (areas?.EditableAreas is { Count: > 0 })
                    {
                        WalkAreas(source.PageTemplateConfigurationSiteID, areas.EditableAreas);
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

        return null;
    }

    #region "Page template & page widget walkers"

    private void WalkAreas(int siteId, List<EditableAreaConfiguration> areas)
    {
        foreach (var area in areas)
        {
            logger.LogTrace("Walk area {Identifier}", area.Identifier);

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
            logger.LogTrace("Walk section {TypeIdentifier}|{Identifier}", section.TypeIdentifier, section.Identifier);

            var sectionFcs = sourceInstanceContext.GetSectionFormComponents(siteId, section.TypeIdentifier);
            WalkProperties(siteId, section.Properties, sectionFcs);

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
            logger.LogTrace("Walk zone {Name}|{Identifier}", zone.Name, zone.Identifier);

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
            logger.LogTrace("Walk widget {TypeIdentifier}|{Identifier}", widget.TypeIdentifier, widget.Identifier);

            var widgetFcs = sourceInstanceContext.GetWidgetPropertyFormComponents(siteId, widget.TypeIdentifier);
            foreach (var variant in widget.Variants)
            {
                logger.LogTrace("Walk widget variant {Name}|{Identifier}", variant.Name, variant.Identifier);

                if (variant.Properties is { Count: > 0 })
                {
                    WalkProperties(siteId, variant.Properties, widgetFcs);
                }
            }
        }
    }

    private void WalkProperties(int siteId, JObject properties, List<EditingFormControlModel>? formControlModels)
    {
        foreach ((string key, var value) in properties)
        {
            logger.LogTrace("Walk property {Name}|{Identifier}", key, value?.ToString());

            var editingFcm = formControlModels?.FirstOrDefault(x => x.PropertyName.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (editingFcm != null)
            {
                if (FieldMappingInstance.BuiltInModel.NotSupportedInKxpLegacyMode
                        .SingleOrDefault(x => x.OldFormComponent == editingFcm.FormComponentIdentifier) is var (oldFormComponent, newFormComponent))
                {
                    Protocol.Append(HandbookReferences.FormComponentNotSupportedInLegacyMode(oldFormComponent, newFormComponent));
                    logger.LogTrace("Editing form component found {FormComponentName} => no longer supported {Replacement}",
                        editingFcm.FormComponentIdentifier, newFormComponent);

                    switch (oldFormComponent)
                    {
                        case Kx13FormComponents.Kentico_MediaFilesSelector:
                        {
                            var mfis = new List<object>();
                            if (value?.ToObject<List<MediaFilesSelectorItem>>() is { Count: > 0 } items)
                            {
                                foreach (var mfsi in items)
                                {
                                    if (configuration.MigrateMediaToMediaLibrary)
                                    {
                                        if (entityIdentityFacade.Translate<IMediaFile>(mfsi.FileGuid, siteId) is { } mf && mediaFileFacade.GetMediaFile(mf.Identity) is { } mfi)
                                        {
                                            mfis.Add(new Kentico.Components.Web.Mvc.FormComponents.MediaFilesSelectorItem { FileGuid = mfi.FileGUID });
                                        }
                                    }
                                    else
                                    {
                                        var sourceMediaFile = modelFacade.SelectWhere<IMediaFile>("FileGUID = @mediaFileGuid AND FileSiteID = @fileSiteID", new SqlParameter("mediaFileGuid", mfsi.FileGuid), new SqlParameter("fileSiteID", siteId))
                                            .FirstOrDefault();
                                        if (sourceMediaFile != null)
                                        {
                                            var (ownerContentItemGuid, _) = assetFacade.GetRef(sourceMediaFile);
                                            mfis.Add(new ContentItemReference { Identifier = ownerContentItemGuid });
                                        }
                                    }
                                }


                                properties[key] = JToken.FromObject(items.Select(x => new Kentico.Components.Web.Mvc.FormComponents.MediaFilesSelectorItem { FileGuid = entityIdentityFacade.Translate<IMediaFile>(x.FileGuid, siteId).Identity })
                                    .ToList());
                            }

                            break;
                        }
                        case Kx13FormComponents.Kentico_PathSelector:
                        {
                            if (value?.ToObject<List<PathSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new Kentico.Components.Web.Mvc.FormComponents.PathSelectorItem { TreePath = x.NodeAliasPath }).ToList());
                            }

                            break;
                        }
                        case Kx13FormComponents.Kentico_AttachmentSelector when newFormComponent == FormComponents.AdminAssetSelectorComponent:
                        {
                            if (value?.ToObject<List<AttachmentSelectorItem>>() is { Count: > 0 } items)
                            {
                                var nv = new List<object>();
                                foreach (var asi in items)
                                {
                                    var attachment = modelFacade.SelectWhere<ICmsAttachment>("AttachmentSiteID = @attachmentSiteID AND AttachmentGUID = @attachmentGUID", new SqlParameter("attachmentSiteID", siteId),
                                            new SqlParameter("attachmentGUID", asi.FileGuid))
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
                        case Kx13FormComponents.Kentico_PageSelector when newFormComponent == FormComponents.Kentico_Xperience_Admin_Websites_WebPageSelectorComponent:
                        {
                            if (value?.ToObject<List<PageSelectorItem>>() is { Count: > 0 } items)
                            {
                                properties[key] = JToken.FromObject(items.Select(x => new WebPageRelatedItem { WebPageGuid = spoiledGuidContext.EnsureNodeGuid(x.NodeGuid, siteId) }).ToList());
                            }

                            logger.LogTrace("Value migrated from {Old} model to {New} model", oldFormComponent, newFormComponent);
                            break;
                        }
                        case Kx13FormComponents.Kentico_FileUploader:
                        {
                            break;
                        }

                        default:
                            break;
                    }
                }
                else if (FieldMappingInstance.BuiltInModel.SupportedInKxpLegacyMode.Contains(editingFcm.FormComponentIdentifier))
                {
                    // OK
                    logger.LogTrace("Editing form component found {FormComponentName} => supported in legacy mode",
                        editingFcm.FormComponentIdentifier);
                }
                else
                {
                    // unknown control, probably custom
                    Protocol.Append(HandbookReferences.FormComponentCustom(editingFcm.FormComponentIdentifier));
                    logger.LogTrace(
                        "Editing form component found {FormComponentName} => custom or inlined component, don't forget to migrate code accordingly",
                        editingFcm.FormComponentIdentifier);
                }
            }
        }
    }

    #endregion
}
