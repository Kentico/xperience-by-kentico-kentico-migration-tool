namespace Migration.Tool.Source.Model;

public interface ICmsPageTemplateConfigurationK12K13
{
    int PageTemplateConfigurationID { get; }
    Guid PageTemplateConfigurationGUID { get; }
    int PageTemplateConfigurationSiteID { get; }
    DateTime PageTemplateConfigurationLastModified { get; }
    string PageTemplateConfigurationName { get; }
    string? PageTemplateConfigurationDescription { get; }
    Guid? PageTemplateConfigurationThumbnailGUID { get; }
    string PageTemplateConfigurationTemplate { get; }
    string? PageTemplateConfigurationWidgets { get; }
}

public partial record CmsPageTemplateConfigurationK12 : ICmsPageTemplateConfigurationK12K13;

public partial record CmsPageTemplateConfigurationK13 : ICmsPageTemplateConfigurationK12K13;
