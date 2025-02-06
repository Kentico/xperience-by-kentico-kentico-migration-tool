using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public record ContentItemSource(ICmsTree SourceNode, string ClassName, ICmsSite SourceSite);
