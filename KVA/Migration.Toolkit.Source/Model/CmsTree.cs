namespace Migration.Toolkit.Source.Model;
// ReSharper disable InconsistentNaming

using System.Data;
using Migration.Toolkit.Common;

public interface ICmsTree: ISourceModel<ICmsTree>
{
    int NodeID { get; }
    string NodeAliasPath { get; }
    string NodeName { get; }
    string NodeAlias { get; }
    int NodeClassID { get; }
    int? NodeParentID { get; }
    int NodeLevel { get; }
    int? NodeACLID { get; }
    int NodeSiteID { get; }
    Guid NodeGUID { get; }
    int? NodeOrder { get; }
    bool? IsSecuredNode { get; }
    int? NodeSKUID { get; }
    int? NodeLinkedNodeID { get; }
    int? NodeOwner { get; }
    string? NodeCustomData { get; }
    int? NodeLinkedNodeSiteID { get; }
    bool? NodeHasChildren { get; }
    bool? NodeHasLinks { get; }
    int? NodeOriginalNodeID { get; }
    bool NodeIsACLOwner { get; }    

    static string ISourceModel<ICmsTree>.GetPrimaryKeyName(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsTreeK11.GetPrimaryKeyName(version),
            { Major: 12 } => CmsTreeK12.GetPrimaryKeyName(version),
            { Major: 13 } => CmsTreeK13.GetPrimaryKeyName(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static bool ISourceModel<ICmsTree>.IsAvailable(SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsTreeK11.IsAvailable(version),
            { Major: 12 } => CmsTreeK12.IsAvailable(version),
            { Major: 13 } => CmsTreeK13.IsAvailable(version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
    static string ISourceModel<ICmsTree>.TableName => "CMS_Tree";
    static string ISourceModel<ICmsTree>.GuidColumnName => "NodeGUID"; //assumtion, class Guid column doesn't change between versions
    static ICmsTree ISourceModel<ICmsTree>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return version switch
        {
            { Major: 11 } => CmsTreeK11.FromReader(reader, version),
            { Major: 12 } => CmsTreeK12.FromReader(reader, version),
            { Major: 13 } => CmsTreeK13.FromReader(reader, version),
            _ => throw new InvalidCastException($"Invalid version {version}")
        };
    }
}
public partial record CmsTreeK11(int NodeID, string NodeAliasPath, string NodeName, string NodeAlias, int NodeClassID, int? NodeParentID, int NodeLevel, int? NodeACLID, int NodeSiteID, Guid NodeGUID, int? NodeOrder, bool? IsSecuredNode, int? NodeCacheMinutes, int? NodeSKUID, string? NodeDocType, string? NodeHeadTags, string? NodeBodyElementAttributes, string? NodeInheritPageLevels, int? RequiresSSL, int? NodeLinkedNodeID, int? NodeOwner, string? NodeCustomData, int? NodeGroupID, int? NodeLinkedNodeSiteID, int? NodeTemplateID, bool? NodeTemplateForAllCultures, bool? NodeInheritPageTemplate, bool? NodeAllowCacheInFileSystem, bool? NodeHasChildren, bool? NodeHasLinks, int? NodeOriginalNodeID, bool NodeIsContentOnly, bool NodeIsACLOwner, string? NodeBodyScripts): ICmsTree, ISourceModel<CmsTreeK11>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "NodeID";   
    public static string TableName => "CMS_Tree";
    public static string GuidColumnName => "NodeGUID";
    static CmsTreeK11 ISourceModel<CmsTreeK11>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsTreeK11(
            reader.Unbox<int>("NodeID"), reader.Unbox<string>("NodeAliasPath"), reader.Unbox<string>("NodeName"), reader.Unbox<string>("NodeAlias"), reader.Unbox<int>("NodeClassID"), reader.Unbox<int?>("NodeParentID"), reader.Unbox<int>("NodeLevel"), reader.Unbox<int?>("NodeACLID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<Guid>("NodeGUID"), reader.Unbox<int?>("NodeOrder"), reader.Unbox<bool?>("IsSecuredNode"), reader.Unbox<int?>("NodeCacheMinutes"), reader.Unbox<int?>("NodeSKUID"), reader.Unbox<string?>("NodeDocType"), reader.Unbox<string?>("NodeHeadTags"), reader.Unbox<string?>("NodeBodyElementAttributes"), reader.Unbox<string?>("NodeInheritPageLevels"), reader.Unbox<int?>("RequiresSSL"), reader.Unbox<int?>("NodeLinkedNodeID"), reader.Unbox<int?>("NodeOwner"), reader.Unbox<string?>("NodeCustomData"), reader.Unbox<int?>("NodeGroupID"), reader.Unbox<int?>("NodeLinkedNodeSiteID"), reader.Unbox<int?>("NodeTemplateID"), reader.Unbox<bool?>("NodeTemplateForAllCultures"), reader.Unbox<bool?>("NodeInheritPageTemplate"), reader.Unbox<bool?>("NodeAllowCacheInFileSystem"), reader.Unbox<bool?>("NodeHasChildren"), reader.Unbox<bool?>("NodeHasLinks"), reader.Unbox<int?>("NodeOriginalNodeID"), reader.Unbox<bool>("NodeIsContentOnly"), reader.Unbox<bool>("NodeIsACLOwner"), reader.Unbox<string?>("NodeBodyScripts")                
        );
    }
    public static CmsTreeK11 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsTreeK11(
            reader.Unbox<int>("NodeID"), reader.Unbox<string>("NodeAliasPath"), reader.Unbox<string>("NodeName"), reader.Unbox<string>("NodeAlias"), reader.Unbox<int>("NodeClassID"), reader.Unbox<int?>("NodeParentID"), reader.Unbox<int>("NodeLevel"), reader.Unbox<int?>("NodeACLID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<Guid>("NodeGUID"), reader.Unbox<int?>("NodeOrder"), reader.Unbox<bool?>("IsSecuredNode"), reader.Unbox<int?>("NodeCacheMinutes"), reader.Unbox<int?>("NodeSKUID"), reader.Unbox<string?>("NodeDocType"), reader.Unbox<string?>("NodeHeadTags"), reader.Unbox<string?>("NodeBodyElementAttributes"), reader.Unbox<string?>("NodeInheritPageLevels"), reader.Unbox<int?>("RequiresSSL"), reader.Unbox<int?>("NodeLinkedNodeID"), reader.Unbox<int?>("NodeOwner"), reader.Unbox<string?>("NodeCustomData"), reader.Unbox<int?>("NodeGroupID"), reader.Unbox<int?>("NodeLinkedNodeSiteID"), reader.Unbox<int?>("NodeTemplateID"), reader.Unbox<bool?>("NodeTemplateForAllCultures"), reader.Unbox<bool?>("NodeInheritPageTemplate"), reader.Unbox<bool?>("NodeAllowCacheInFileSystem"), reader.Unbox<bool?>("NodeHasChildren"), reader.Unbox<bool?>("NodeHasLinks"), reader.Unbox<int?>("NodeOriginalNodeID"), reader.Unbox<bool>("NodeIsContentOnly"), reader.Unbox<bool>("NodeIsACLOwner"), reader.Unbox<string?>("NodeBodyScripts")                
        );
    }
};
public partial record CmsTreeK12(int NodeID, string NodeAliasPath, string NodeName, string NodeAlias, int NodeClassID, int? NodeParentID, int NodeLevel, int? NodeACLID, int NodeSiteID, Guid NodeGUID, int? NodeOrder, bool? IsSecuredNode, int? NodeCacheMinutes, int? NodeSKUID, string? NodeDocType, string? NodeHeadTags, string? NodeBodyElementAttributes, string? NodeInheritPageLevels, int? RequiresSSL, int? NodeLinkedNodeID, int? NodeOwner, string? NodeCustomData, int? NodeGroupID, int? NodeLinkedNodeSiteID, int? NodeTemplateID, bool? NodeTemplateForAllCultures, bool? NodeInheritPageTemplate, bool? NodeAllowCacheInFileSystem, bool? NodeHasChildren, bool? NodeHasLinks, int? NodeOriginalNodeID, bool NodeIsContentOnly, bool NodeIsACLOwner, string? NodeBodyScripts): ICmsTree, ISourceModel<CmsTreeK12>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "NodeID";   
    public static string TableName => "CMS_Tree";
    public static string GuidColumnName => "NodeGUID";
    static CmsTreeK12 ISourceModel<CmsTreeK12>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsTreeK12(
            reader.Unbox<int>("NodeID"), reader.Unbox<string>("NodeAliasPath"), reader.Unbox<string>("NodeName"), reader.Unbox<string>("NodeAlias"), reader.Unbox<int>("NodeClassID"), reader.Unbox<int?>("NodeParentID"), reader.Unbox<int>("NodeLevel"), reader.Unbox<int?>("NodeACLID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<Guid>("NodeGUID"), reader.Unbox<int?>("NodeOrder"), reader.Unbox<bool?>("IsSecuredNode"), reader.Unbox<int?>("NodeCacheMinutes"), reader.Unbox<int?>("NodeSKUID"), reader.Unbox<string?>("NodeDocType"), reader.Unbox<string?>("NodeHeadTags"), reader.Unbox<string?>("NodeBodyElementAttributes"), reader.Unbox<string?>("NodeInheritPageLevels"), reader.Unbox<int?>("RequiresSSL"), reader.Unbox<int?>("NodeLinkedNodeID"), reader.Unbox<int?>("NodeOwner"), reader.Unbox<string?>("NodeCustomData"), reader.Unbox<int?>("NodeGroupID"), reader.Unbox<int?>("NodeLinkedNodeSiteID"), reader.Unbox<int?>("NodeTemplateID"), reader.Unbox<bool?>("NodeTemplateForAllCultures"), reader.Unbox<bool?>("NodeInheritPageTemplate"), reader.Unbox<bool?>("NodeAllowCacheInFileSystem"), reader.Unbox<bool?>("NodeHasChildren"), reader.Unbox<bool?>("NodeHasLinks"), reader.Unbox<int?>("NodeOriginalNodeID"), reader.Unbox<bool>("NodeIsContentOnly"), reader.Unbox<bool>("NodeIsACLOwner"), reader.Unbox<string?>("NodeBodyScripts")                
        );
    }
    public static CmsTreeK12 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsTreeK12(
            reader.Unbox<int>("NodeID"), reader.Unbox<string>("NodeAliasPath"), reader.Unbox<string>("NodeName"), reader.Unbox<string>("NodeAlias"), reader.Unbox<int>("NodeClassID"), reader.Unbox<int?>("NodeParentID"), reader.Unbox<int>("NodeLevel"), reader.Unbox<int?>("NodeACLID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<Guid>("NodeGUID"), reader.Unbox<int?>("NodeOrder"), reader.Unbox<bool?>("IsSecuredNode"), reader.Unbox<int?>("NodeCacheMinutes"), reader.Unbox<int?>("NodeSKUID"), reader.Unbox<string?>("NodeDocType"), reader.Unbox<string?>("NodeHeadTags"), reader.Unbox<string?>("NodeBodyElementAttributes"), reader.Unbox<string?>("NodeInheritPageLevels"), reader.Unbox<int?>("RequiresSSL"), reader.Unbox<int?>("NodeLinkedNodeID"), reader.Unbox<int?>("NodeOwner"), reader.Unbox<string?>("NodeCustomData"), reader.Unbox<int?>("NodeGroupID"), reader.Unbox<int?>("NodeLinkedNodeSiteID"), reader.Unbox<int?>("NodeTemplateID"), reader.Unbox<bool?>("NodeTemplateForAllCultures"), reader.Unbox<bool?>("NodeInheritPageTemplate"), reader.Unbox<bool?>("NodeAllowCacheInFileSystem"), reader.Unbox<bool?>("NodeHasChildren"), reader.Unbox<bool?>("NodeHasLinks"), reader.Unbox<int?>("NodeOriginalNodeID"), reader.Unbox<bool>("NodeIsContentOnly"), reader.Unbox<bool>("NodeIsACLOwner"), reader.Unbox<string?>("NodeBodyScripts")                
        );
    }
};
public partial record CmsTreeK13(int NodeID, string NodeAliasPath, string NodeName, string NodeAlias, int NodeClassID, int? NodeParentID, int NodeLevel, int? NodeACLID, int NodeSiteID, Guid NodeGUID, int? NodeOrder, bool? IsSecuredNode, int? NodeSKUID, int? NodeLinkedNodeID, int? NodeOwner, string? NodeCustomData, int? NodeLinkedNodeSiteID, bool? NodeHasChildren, bool? NodeHasLinks, int? NodeOriginalNodeID, bool NodeIsACLOwner): ICmsTree, ISourceModel<CmsTreeK13>
{
    public static bool IsAvailable(SemanticVersion version) => true;
    public static string GetPrimaryKeyName(SemanticVersion version) => "NodeID";   
    public static string TableName => "CMS_Tree";
    public static string GuidColumnName => "NodeGUID";
    static CmsTreeK13 ISourceModel<CmsTreeK13>.FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsTreeK13(
            reader.Unbox<int>("NodeID"), reader.Unbox<string>("NodeAliasPath"), reader.Unbox<string>("NodeName"), reader.Unbox<string>("NodeAlias"), reader.Unbox<int>("NodeClassID"), reader.Unbox<int?>("NodeParentID"), reader.Unbox<int>("NodeLevel"), reader.Unbox<int?>("NodeACLID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<Guid>("NodeGUID"), reader.Unbox<int?>("NodeOrder"), reader.Unbox<bool?>("IsSecuredNode"), reader.Unbox<int?>("NodeSKUID"), reader.Unbox<int?>("NodeLinkedNodeID"), reader.Unbox<int?>("NodeOwner"), reader.Unbox<string?>("NodeCustomData"), reader.Unbox<int?>("NodeLinkedNodeSiteID"), reader.Unbox<bool?>("NodeHasChildren"), reader.Unbox<bool?>("NodeHasLinks"), reader.Unbox<int?>("NodeOriginalNodeID"), reader.Unbox<bool>("NodeIsACLOwner")                
        );
    }
    public static CmsTreeK13 FromReader(IDataReader reader, SemanticVersion version)
    {
        return new CmsTreeK13(
            reader.Unbox<int>("NodeID"), reader.Unbox<string>("NodeAliasPath"), reader.Unbox<string>("NodeName"), reader.Unbox<string>("NodeAlias"), reader.Unbox<int>("NodeClassID"), reader.Unbox<int?>("NodeParentID"), reader.Unbox<int>("NodeLevel"), reader.Unbox<int?>("NodeACLID"), reader.Unbox<int>("NodeSiteID"), reader.Unbox<Guid>("NodeGUID"), reader.Unbox<int?>("NodeOrder"), reader.Unbox<bool?>("IsSecuredNode"), reader.Unbox<int?>("NodeSKUID"), reader.Unbox<int?>("NodeLinkedNodeID"), reader.Unbox<int?>("NodeOwner"), reader.Unbox<string?>("NodeCustomData"), reader.Unbox<int?>("NodeLinkedNodeSiteID"), reader.Unbox<bool?>("NodeHasChildren"), reader.Unbox<bool?>("NodeHasLinks"), reader.Unbox<int?>("NodeOriginalNodeID"), reader.Unbox<bool>("NodeIsACLOwner")                
        );
    }
};

