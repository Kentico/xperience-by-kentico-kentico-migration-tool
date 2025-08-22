using CMS.Websites;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Source.Mappers;

public class PageTemplateConfigurationMapper(
    ILogger<PageTemplateConfigurationMapper> logger,
    PrimaryKeyMappingContext pkContext,
    IProtocol protocol,
    VisualBuilderPatcher visualBuilderPatcher
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

            // bool needsDeferredPatch = false;
            string? configurationTemplate = source.PageTemplateConfigurationTemplate;
            string? configurationWidgets = source.PageTemplateConfigurationWidgets;

            (configurationTemplate, configurationWidgets, bool _) = visualBuilderPatcher.PatchJsonDefinitions(source.PageTemplateConfigurationSiteID, configurationTemplate, configurationWidgets).GetAwaiter().GetResult();

            target.PageTemplateConfigurationTemplate = configurationTemplate;
            target.PageTemplateConfigurationWidgets = configurationWidgets;

            return target;
        }

#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
