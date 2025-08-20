namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public interface ILinkedPageActionProvider
{
    /// <summary>
    /// Do not migrate the node
    /// </summary>
    public void Drop();

    /// <summary>
    /// Create a clone of the source node. After materialization, this page will be passed down for subsequent processing as normal.
    /// This is the default if no action provider method is called.
    /// </summary>
    public void Materialize();

    /// <summary>
    /// Store reference to the linked node in a combined selector field of the node's ancestor
    /// </summary>
    /// <param name="parentLevel">Relative level of the page to host the widget on. -1 is direct parent, -2 is grandparent, etc.</param>
    /// <param name="fieldName">Name of the field to store the reference in. If the field doesn't exist, it will be created. 
    /// If the field doesn't include the linked node's type in its allowed types, it will be added.</param>
    public void StoreReferenceInAncestor(int parentLevel, string fieldName);
}
