using Kentico.Xperience.UMT.Model;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal abstract class ContentItemDirectiveBase : IUmtModel
{
    public string? PageTemplateIdentifier { get; set; }
    public JObject? PageTemplateProperties { get; set; }
    public ContentFolderOptions? ContentFolderOptions { get; set; }
    public bool RegenerateUrlPath { get; set; } = false;
    public IEnumerable<FormerPageUrlPath>? FormerUrlPaths { get; set; }

    #region IUmtModel
    // This interface is implemented only as means to allow yielding the directive out of content item mapper.
    // It's less than elegant, but fully elegant solution would require deep refactoring.
    public Dictionary<string, object?> CustomProperties => throw new NotImplementedException();
    public string PrintMe() => throw new NotImplementedException();
    #endregion
}
