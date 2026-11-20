namespace Migration.Tool.Source.Mappers.FormMapperDirectives;
public interface IFormActionProvider
{
    /// <summary>
    /// Migrates form to workspace with passed <paramref name="name"/>. If such workspace doesn't exist, it is created.
    /// </summary>
    void OverrideWorkspace(string name, string displayName);

    /// <summary>
    /// Migrates form to existing workspace with passed <paramref name="guid"/>
    /// </summary>
    void OverrideWorkspace(Guid guid);
}
