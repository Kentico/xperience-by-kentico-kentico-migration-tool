using System.Text.Json;
using Migration.Tool.Source.Mappers.ContentItemMapperDirectives;

namespace Migration.Tool.Extensions.CommunityMigrations;

/// <summary>
/// This content item director overrides target content type based on class name of the source page.
/// The override logic is driven by a JSON file. Unspecified source classes are not affected.
/// </summary>
/// <remarks>
/// JSON file example:
/// {
///     "mapping": {
///         "contentTypes": {
///             "DancingGoatCore.Article": "NewProjectNamespace.PrefabricatedArticleType",
///             "DancingGoatCore.Product": "NewProjectNamespace.PrefabricatedProductType"
///         }
///     }
/// }
/// </remarks>
public class JsonBasedTypeRemapDirector : ContentItemDirectorBase
{
    private readonly Dictionary<string, string> definitions;

    public JsonBasedTypeRemapDirector(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            throw new ArgumentException("JSON file path cannot be null or empty", nameof(jsonFilePath));
        }

        var jsonContent = File.ReadAllText(jsonFilePath);
        var mappingData = JsonSerializer.Deserialize<MappingDefinitions>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        definitions = mappingData?.Mapping?.ContentTypes ?? throw new InvalidOperationException("Invalid JSON structure. Expected 'mapping.contentTypes'.");
    }

    public override void Direct(ContentItemSource source, IContentItemActionProvider options)
    {
        if (definitions.TryGetValue(source.SourceClassName, out var targetTypeName))
        {
            options.OverrideTargetType(targetTypeName);
        }
    }

    private class MappingDefinitions
    {
        public MappingData? Mapping { get; set; }

        public class MappingData
        {
            public Dictionary<string, string>? ContentTypes { get; set; }
        }
    }
}
