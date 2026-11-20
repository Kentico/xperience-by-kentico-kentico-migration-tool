using Migration.Tool.Common.ContentItemOptions;

namespace Migration.Tool.Source.Mappers.FormMapperDirectives;
internal class FormActionProvider : IFormActionProvider
{
    internal WorkspaceOptions? WorkspaceOptions { get; private set; }

    public void OverrideWorkspace(string name, string displayName) =>
        WorkspaceOptions = new WorkspaceOptions(Name: name, DisplayName: displayName);
    public void OverrideWorkspace(Guid guid) =>
        WorkspaceOptions = new WorkspaceOptions(Guid: guid);
}
