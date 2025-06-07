using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public record ContentItemSource(ICmsTree? SourceNode, string SourceClassName, string TargetClassName, ICmsSite? SourceSite, IEnumerable<FormerPageUrlPath>? FormerUrlPaths, IEnumerable<ICmsTree>? ChildNodes);
