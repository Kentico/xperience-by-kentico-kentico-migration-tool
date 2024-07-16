using System.Xml.Linq;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Enumerations;

namespace Migration.Toolkit.Source.Auxiliary;
internal class NodeXmlAdapter
{
    private readonly XElement _xClass;

    public bool ParsingSuccessful { get; }

    public NodeXmlAdapter(string xml)
    {
        var xDoc = XDocument.Parse(xml);
        if (xDoc.Root?.FirstNode is XElement dClass)
        {
            _xClass = dClass;
            ParsingSuccessful = true;
        }
        else
        {
            _xClass = null!;
            ParsingSuccessful = false;
        }
    }

    public string? GetValue(string columnName) => _xClass.Element(columnName)?.Value;

    public bool HasValueSet(string columnName) => _xClass.Element(columnName) != null;

    public int? NodeID => _xClass.Element(NodeXmlColumns.NODE_ID)?.Value<int>();
    public string? NodeAliasPath => _xClass.Element(NodeXmlColumns.NODE_ALIAS_PATH)?.Value;
    public string? NodeName => _xClass.Element(NodeXmlColumns.NODE_NAME)?.Value;
    public string? NodeAlias => _xClass.Element(NodeXmlColumns.NODE_ALIAS)?.Value;
    public int? NodeClassID => _xClass.Element(NodeXmlColumns.NODE_CLASS_ID)?.Value<int>();
    public int? NodeParentID => _xClass.Element(NodeXmlColumns.NODE_PARENT_ID)?.Value<int>();
    public int? NodeLevel => _xClass.Element(NodeXmlColumns.NODE_LEVEL)?.Value<int>();
    public int? NodeSiteID => _xClass.Element(NodeXmlColumns.NODE_SITE_ID)?.Value<int>();
    [Obsolete("NodeGUID is not unique, use other means of node identification", true)]
    public Guid? NodeGUID => _xClass.Element(NodeXmlColumns.NODE_GUID)?.Value<Guid>();
    public int? NodeOrder => _xClass.Element(NodeXmlColumns.NODE_ORDER)?.Value<int>();
    public int? NodeOwner => _xClass.Element(NodeXmlColumns.NODE_OWNER)?.Value<int>();
    public bool? NodeHasChildren => _xClass.Element(NodeXmlColumns.NODE_HAS_CHILDREN)?.ValueAsBool();
    public bool? NodeHasLinks => _xClass.Element(NodeXmlColumns.NODE_HAS_LINKS)?.ValueAsBool();
    public int? NodeOriginalNodeID => _xClass.Element(NodeXmlColumns.NODE_ORIGINAL_NODE_ID)?.Value<int>();
    public bool? NodeIsPage => _xClass.Element(NodeXmlColumns.NODE_IS_PAGE)?.ValueAsBool();
    public bool? NodeIsSecured => _xClass.Element(NodeXmlColumns.NODE_IS_SECURED)?.ValueAsBool();
    public int? DocumentID => _xClass.Element(NodeXmlColumns.DOCUMENT_ID)?.Value<int>();
    public string? DocumentName => _xClass.Element(NodeXmlColumns.DOCUMENT_NAME)?.Value;
    public DateTime? DocumentModifiedWhen => _xClass.Element(NodeXmlColumns.DOCUMENT_MODIFIED_WHEN)?.Value<DateTime>();
    public int? DocumentModifiedByUserID => _xClass.Element(NodeXmlColumns.DOCUMENT_MODIFIED_BY_USER_ID)?.Value<int>();
    public int? DocumentCreatedByUserID => _xClass.Element(NodeXmlColumns.DOCUMENT_CREATED_BY_USER_ID)?.Value<int>();
    public DateTime? DocumentCreatedWhen => _xClass.Element(NodeXmlColumns.DOCUMENT_CREATED_WHEN)?.Value<DateTime>();
    public int? DocumentCheckedOutVersionHistoryID => _xClass.Element(NodeXmlColumns.DOCUMENT_CHECKED_OUT_VERSION_HISTORY_ID)?.Value<int>();
    public int? DocumentPublishedVersionHistoryID => _xClass.Element(NodeXmlColumns.DOCUMENT_PUBLISHED_VERSION_HISTORY_ID)?.Value<int>();
    public int? DocumentWorkflowStepID => _xClass.Element(NodeXmlColumns.DOCUMENT_WORKFLOW_STEP_ID)?.Value<int>();
    public string? DocumentCulture => _xClass.Element(NodeXmlColumns.DOCUMENT_CULTURE)?.Value;
    public int? DocumentNodeID => _xClass.Element(NodeXmlColumns.DOCUMENT_NODE_ID)?.Value<int>();
    public string? DocumentContent => _xClass.Element(NodeXmlColumns.DOCUMENT_CONTENT)?.Value;
    public string? DocumentLastVersionNumber => _xClass.Element(NodeXmlColumns.DOCUMENT_LAST_VERSION_NUMBER)?.Value;
    public bool? DocumentIsArchived => _xClass.Element(NodeXmlColumns.DOCUMENT_IS_ARCHIVED)?.ValueAsBool();
    [Obsolete("DocumentGUID is not unique, use other means of document identification", true)]
    public Guid? DocumentGUID => _xClass.Element(NodeXmlColumns.DOCUMENT_GUID)?.Value<Guid>();
    public Guid? DocumentWorkflowCycleGUID => _xClass.Element(NodeXmlColumns.DOCUMENT_WORKFLOW_CYCLE_GUID)?.Value<Guid>();
    public bool? DocumentCanBePublished => _xClass.Element(NodeXmlColumns.DOCUMENT_CAN_BE_PUBLISHED)?.ValueAsBool();
    public string? DocumentPageBuilderWidgets => _xClass.Element(NodeXmlColumns.DOCUMENT_PAGE_BUILDER_WIDGETS)?.Value;
    public string? ClassName => _xClass.Element(NodeXmlColumns.CLASS_NAME)?.Value;

    public string? DocumentPageTemplateConfiguration => _xClass.Element(NodeXmlColumns.DOCUMENT_PAGE_TEMPLATE_CONFIGURATION)?.Value;
}
