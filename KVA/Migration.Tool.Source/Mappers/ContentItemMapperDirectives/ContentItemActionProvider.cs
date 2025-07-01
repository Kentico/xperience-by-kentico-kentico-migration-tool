using Migration.Tool.Source.Model;
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

    public void OverrideWorkspace(string name, string displayName) =>
        Directive.WorkspaceOptions = new WorkspaceOptions(Name: name, DisplayName: name);
    public void OverrideWorkspace(Guid guid) =>
        Directive.WorkspaceOptions = new WorkspaceOptions(Guid: guid);

    public void RegenerateUrlPath() => Directive.RegenerateUrlPath = true;
    public void OverrideFormerUrlPaths(IEnumerable<FormerPageUrlPath> formerPaths) => Directive.FormerUrlPaths = formerPaths;
    public void LinkChildren(string fieldName, IEnumerable<ICmsTree> children)
    {
        foreach (var child in children)
        {
            Directive.ChildLinks.Add((fieldName, child));
        }
    }
}
