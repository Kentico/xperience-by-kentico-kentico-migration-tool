using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public record LinkedPageSource(ICmsSite SourceSite, ICmsTree SourceNode, ICmsTree LinkedNode);
