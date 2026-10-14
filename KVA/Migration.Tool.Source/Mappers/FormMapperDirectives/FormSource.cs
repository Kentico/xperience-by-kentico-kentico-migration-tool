using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers.FormMapperDirectives;
public record FormSource(ICmsForm SourceForm, ICmsSite? SourceSite);
