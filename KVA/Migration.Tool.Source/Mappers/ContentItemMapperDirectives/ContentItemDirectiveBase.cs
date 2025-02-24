using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal abstract class ContentItemDirectiveBase
{
    public string? PageTemplateIdentifier { get; set; }
    public JObject? PageTemplateProperties { get; set; }
    public ContentFolderOptions? ContentFolderOptions { get; set; }

}
