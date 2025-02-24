using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal partial class ContentItemActionProvider : IContentItemActionProvider
{
    internal ContentItemDirectiveBase Directive { get; private set; } = new PassthroughDirective();

    public void Drop() => Directive = new DropDirective();
    public void AsWidget(string widgetType, Guid? widgetGuid, Guid? widgetVariantGuid, Action<IConvertToWidgetOptions> options)
    {
        Directive = new ConvertToWidgetDirective(widgetType, widgetGuid, widgetVariantGuid);
        options((ConvertToWidgetDirective)Directive);
    }
    public void OverridePageTemplate(string templateIdentifier, JObject? templateProperties)
    {
        Directive.PageTemplateIdentifier = templateIdentifier;
        Directive.PageTemplateProperties = templateProperties;
    }
    public void OverrideContentFolder(Guid contentFolderGuid) => Directive.ContentFolderOptions = new ContentFolderOptions(Guid: contentFolderGuid);
    public void OverrideContentFolder(string displayNamePath) => Directive.ContentFolderOptions = new ContentFolderOptions(DisplayNamePath: displayNamePath);
}
