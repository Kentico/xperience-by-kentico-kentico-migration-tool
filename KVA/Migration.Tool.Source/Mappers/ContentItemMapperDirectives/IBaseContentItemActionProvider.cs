namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public interface IBaseContentItemActionProvider
{
    void OverrideContentFolder(Guid contentFolderGuid);
    void OverrideContentFolder(string displayNamePath);
    void OverrideWorkspace(string name, string displayName);
    void OverrideWorkspace(Guid guid);
}
