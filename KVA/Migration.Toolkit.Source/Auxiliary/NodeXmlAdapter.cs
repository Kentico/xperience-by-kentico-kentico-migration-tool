using System.Xml.Linq;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Enumerations;
// ReSharper disable InconsistentNaming // generated class

namespace Migration.Toolkit.Source.Auxiliary;

internal class NodeXmlAdapter
{
    private readonly XElement xClass;

    public NodeXmlAdapter(string xml)
    {
        var xDoc = XDocument.Parse(xml);
        if (xDoc.Root?.FirstNode is XElement dClass)
        {
            xClass = dClass;
            ParsingSuccessful = true;
        }
        else
        {
            xClass = null!;
            ParsingSuccessful = false;
        }
    }

    public bool ParsingSuccessful { get; }

    public int? NodeID => xClass.Element(NodeXmlColumns.NODE_ID)?.Value<int>();
    public string? NodeAliasPath => xClass.Element(NodeXmlColumns.NODE_ALIAS_PATH)?.Value;
    public string? NodeName => xClass.Element(NodeXmlColumns.NODE_NAME)?.Value;
    public string? NodeAlias => xClass.Element(NodeXmlColumns.NODE_ALIAS)?.Value;
    public int? NodeClassID => xClass.Element(NodeXmlColumns.NODE_CLASS_ID)?.Value<int>();
    public int? NodeParentID => xClass.Element(NodeXmlColumns.NODE_PARENT_ID)?.Value<int>();
    public int? NodeLevel => xClass.Element(NodeXmlColumns.NODE_LEVEL)?.Value<int>();
    public int? NodeSiteID => xClass.Element(NodeXmlColumns.NODE_SITE_ID)?.Value<int>();

    [Obsolete("NodeGUID is not unique, use other means of node identification", true)]
    public Guid? NodeGUID => xClass.Element(NodeXmlColumns.NODE_GUID)?.Value<Guid>();

    public int? NodeOrder => xClass.Element(NodeXmlColumns.NODE_ORDER)?.Value<int>();
    public int? NodeOwner => xClass.Element(NodeXmlColumns.NODE_OWNER)?.Value<int>();
    public bool? NodeHasChildren => xClass.Element(NodeXmlColumns.NODE_HAS_CHILDREN)?.ValueAsBool();
    public bool? NodeHasLinks => xClass.Element(NodeXmlColumns.NODE_HAS_LINKS)?.ValueAsBool();
    public int? NodeOriginalNodeID => xClass.Element(NodeXmlColumns.NODE_ORIGINAL_NODE_ID)?.Value<int>();
    public bool? NodeIsPage => xClass.Element(NodeXmlColumns.NODE_IS_PAGE)?.ValueAsBool();
    public bool? NodeIsSecured => xClass.Element(NodeXmlColumns.NODE_IS_SECURED)?.ValueAsBool();
    public int? DocumentID => xClass.Element(NodeXmlColumns.DOCUMENT_ID)?.Value<int>();
    public string? DocumentName => xClass.Element(NodeXmlColumns.DOCUMENT_NAME)?.Value;
    public DateTime? DocumentModifiedWhen => xClass.Element(NodeXmlColumns.DOCUMENT_MODIFIED_WHEN)?.Value<DateTime>();
    public int? DocumentModifiedByUserID => xClass.Element(NodeXmlColumns.DOCUMENT_MODIFIED_BY_USER_ID)?.Value<int>();
    public int? DocumentCreatedByUserID => xClass.Element(NodeXmlColumns.DOCUMENT_CREATED_BY_USER_ID)?.Value<int>();
    public DateTime? DocumentCreatedWhen => xClass.Element(NodeXmlColumns.DOCUMENT_CREATED_WHEN)?.Value<DateTime>();
    public int? DocumentCheckedOutVersionHistoryID => xClass.Element(NodeXmlColumns.DOCUMENT_CHECKED_OUT_VERSION_HISTORY_ID)?.Value<int>();
    public int? DocumentPublishedVersionHistoryID => xClass.Element(NodeXmlColumns.DOCUMENT_PUBLISHED_VERSION_HISTORY_ID)?.Value<int>();
    public int? DocumentWorkflowStepID => xClass.Element(NodeXmlColumns.DOCUMENT_WORKFLOW_STEP_ID)?.Value<int>();
    public string? DocumentCulture => xClass.Element(NodeXmlColumns.DOCUMENT_CULTURE)?.Value;
    public int? DocumentNodeID => xClass.Element(NodeXmlColumns.DOCUMENT_NODE_ID)?.Value<int>();
    public string? DocumentContent => xClass.Element(NodeXmlColumns.DOCUMENT_CONTENT)?.Value;
    public string? DocumentLastVersionNumber => xClass.Element(NodeXmlColumns.DOCUMENT_LAST_VERSION_NUMBER)?.Value;
    public bool? DocumentIsArchived => xClass.Element(NodeXmlColumns.DOCUMENT_IS_ARCHIVED)?.ValueAsBool();

    [Obsolete("DocumentGUID is not unique, use other means of document identification", true)]
    public Guid? DocumentGUID => xClass.Element(NodeXmlColumns.DOCUMENT_GUID)?.Value<Guid>();

    public Guid? DocumentWorkflowCycleGUID => xClass.Element(NodeXmlColumns.DOCUMENT_WORKFLOW_CYCLE_GUID)?.Value<Guid>();
    public bool? DocumentCanBePublished => xClass.Element(NodeXmlColumns.DOCUMENT_CAN_BE_PUBLISHED)?.ValueAsBool();
    public string? DocumentPageBuilderWidgets => xClass.Element(NodeXmlColumns.DOCUMENT_PAGE_BUILDER_WIDGETS)?.Value;
    public string? ClassName => xClass.Element(NodeXmlColumns.CLASS_NAME)?.Value;

    public string? DocumentPageTemplateConfiguration => xClass.Element(NodeXmlColumns.DOCUMENT_PAGE_TEMPLATE_CONFIGURATION)?.Value;

    public string? GetValue(string columnName) => xClass.Element(columnName)?.Value;

    public bool HasValueSet(string columnName) => xClass.Element(columnName) != null;
}
