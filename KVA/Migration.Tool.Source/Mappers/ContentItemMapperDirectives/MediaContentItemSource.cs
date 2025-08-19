using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
public record MediaContentItemSource(ICmsSite SourceSite, IMediaLibrary SourceMediaLibrary, IMediaFile SourceMediaFile);
