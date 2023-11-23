namespace Migration.Toolkit.Core.Auxiliary;

using System;
using System.Xml.Linq;

internal class NodeXmlAdapter {
    private readonly XDocument _xDoc;
    private readonly XElement? _xClass;

    public bool ParsingSuccessful { get; }

    public NodeXmlAdapter(string xml) {
        _xDoc = XDocument.Parse(xml);
        if (_xDoc.Root?.FirstNode is XElement dClass)
        {
            _xClass = dClass;
            ParsingSuccessful = true;
        }
        else
        {
            _xClass = null;
            ParsingSuccessful = false;
        }
    }

    public string GetValue(string columnName) {
        return _xClass.Element(columnName)?.Value;
    }

    public bool HasValueSet(string columnName)
    {
        return _xClass.Element(columnName) != null;
    }

    public int? NodeID => _xClass.Element("NodeID")?.Value<int>();
    public string NodeAliasPath => _xClass.Element("NodeAliasPath")?.Value;
    public string NodeName => _xClass.Element("NodeName")?.Value;
    public string NodeAlias => _xClass.Element("NodeAlias")?.Value;
    public int? NodeClassID => _xClass.Element("NodeClassID")?.Value<int>();
    public int? NodeParentID => _xClass.Element("NodeParentID")?.Value<int>();
    public int? NodeLevel => _xClass.Element("NodeLevel")?.Value<int>();
    public int? NodeSiteID => _xClass.Element("NodeSiteID")?.Value<int>();
    public Guid? NodeGUID => _xClass.Element("NodeGUID")?.Value<Guid>();
    public int? NodeOrder => _xClass.Element("NodeOrder")?.Value<int>();
    public int? NodeOwner => _xClass.Element("NodeOwner")?.Value<int>();
    public bool? NodeHasChildren => _xClass.Element("NodeHasChildren")?.ValueAsBool();
    public bool? NodeHasLinks => _xClass.Element("NodeHasLinks")?.ValueAsBool();
    public int? NodeOriginalNodeID => _xClass.Element("NodeOriginalNodeID")?.Value<int>();
    public bool? NodeIsPage => _xClass.Element("NodeIsPage")?.ValueAsBool();
    public bool? NodeIsSecured => _xClass.Element("NodeIsSecured")?.ValueAsBool();
    public int? DocumentID => _xClass.Element("DocumentID")?.Value<int>();
    public string DocumentName => _xClass.Element("DocumentName")?.Value;
    public DateTime? DocumentModifiedWhen => _xClass.Element("DocumentModifiedWhen")?.Value<DateTime>();
    public int? DocumentModifiedByUserID => _xClass.Element("DocumentModifiedByUserID")?.Value<int>();
    public int? DocumentCreatedByUserID => _xClass.Element("DocumentCreatedByUserID")?.Value<int>();
    public DateTime? DocumentCreatedWhen => _xClass.Element("DocumentCreatedWhen")?.Value<DateTime>();
    public int? DocumentCheckedOutVersionHistoryID => _xClass.Element("DocumentCheckedOutVersionHistoryID")?.Value<int>();
    public int? DocumentPublishedVersionHistoryID => _xClass.Element("DocumentPublishedVersionHistoryID")?.Value<int>();
    public int? DocumentWorkflowStepID => _xClass.Element("DocumentWorkflowStepID")?.Value<int>();
    public string DocumentCulture => _xClass.Element("DocumentCulture")?.Value;
    public int? DocumentNodeID => _xClass.Element("DocumentNodeID")?.Value<int>();
    public string DocumentContent => _xClass.Element("DocumentContent")?.Value;
    public string DocumentLastVersionNumber => _xClass.Element("DocumentLastVersionNumber")?.Value;
    public bool? DocumentIsArchived => _xClass.Element("DocumentIsArchived")?.ValueAsBool();
    public Guid? DocumentGUID => _xClass.Element("DocumentGUID")?.Value<Guid>();
    public Guid? DocumentWorkflowCycleGUID => _xClass.Element("DocumentWorkflowCycleGUID")?.Value<Guid>();
    public bool? DocumentCanBePublished => _xClass.Element("DocumentCanBePublished")?.ValueAsBool();
    public string DocumentPageBuilderWidgets => _xClass.Element("DocumentPageBuilderWidgets")?.Value;
    public string ClassName => _xClass.Element("ClassName")?.Value;

    public string DocumentPageTemplateConfiguration => _xClass.Element("DocumentPageTemplateConfiguration")?.Value;
}
