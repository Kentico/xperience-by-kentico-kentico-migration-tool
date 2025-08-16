namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal class LinkedPageActionProvider : ILinkedPageActionProvider
{
    internal LinkedPageDirectiveBase? Directive { get; private set; }

    public void Drop() => Directive = new DropLinkedPageDirective();
    public void Materialize() => Directive = new MaterializeLinkedPageDirective();
    public void StoreReferenceInAncestor(int parentLevel, string fieldName) => Directive = new StoreLinkedPageAsReferenceDirective { ParentLevel = parentLevel, FieldName = fieldName };
}
