# Customization: Class Mappings

> **Audience:** Developers remodeling content type structure during migration.

Class mappings (`IClassMapping`) transform content type structure and field definitions between source and target.

## Typical Use Cases

- Merge multiple page types into one content type
- Remodel page types as reusable field schemas
- Rename fields and change data types/form controls
- Split one page type into multiple content types
- Convert custom tables or module classes into content types

## Implementation Pattern

1. Create a class mapping implementation in `Migration.Tool.Extensions`.
2. Add an `IServiceCollection` extension method for each mapping.
3. Define a `MultiClassMapping`.
4. Define key and fields.
5. Optionally add value conversion.
6. Register mapping in DI.
7. Call your extension method from `UseCustomizations` in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs).

### Basic `MultiClassMapping` setup

```csharp
var m = new MultiClassMapping(targetClassName, target =>
{
  // Target content type name
  target.ClassName = "Acme.Article";
  // Database table name of the new type
  target.ClassTableName = "Acme_Article";
  // Display name of the content type
  target.ClassDisplayName = "Article";
  // Type of the class (specifies that the class is a content type)
  target.ClassType = ClassType.CONTENT_TYPE;
  // What the content type is used for (reusable content, pages, email, or headless)
  target.ClassContentTypeType = ClassContentTypeType.WEBSITE;
});

m.BuildField("ArticleID").AsPrimaryKey();
```

### Map fields from multiple source classes

```csharp
var title = m.BuildField("Title");

// Maps the "ArticleTitle" field from the source data class "Article.BlogPost"
title.SetFrom("Article.BlogPost", "ArticleTitle", true);

// Maps the same field from another source class
title.SetFrom("Article.NewsArticle", "ArticleTitle");

// Optionally patch field metadata
title.WithFieldPatch(f => f.Caption = "Article title");
```

### Optional value conversion

```csharp
var publishDate = m.BuildField("PublishDate");
publishDate.SetFrom("Article.BlogPost", "BlogPostDate", true);

publishDate.ConvertFrom("Article.NewsArticle", "ArticleDateAsText", false,
    (v, context) =>
    {
        switch (context)
        {
            case ConvertorTreeNodeContext treeNodeContext:
                // Available TreeNode context
                break;
            default:
                // No specific context available
                break;
        }

        return v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null;
    });
```

## Important Behavior

Mappings replace default migration behavior for source classes where at least one source field is mapped. If only some fields are mapped, unmapped fields from that source class are not migrated automatically.

## Reusable Field Schema Note

Usage of `ReusableSchemaBuilder` in custom class mappings cannot be combined with the `Settings.CreateReusableFieldSchemaForClasses` configuration option.

## Samples and Related Guides

- Sample file: [Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs](../../Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs)
  - `AddSimpleRemodelingSample`
  - `AddClassMergeSample`
  - `AddReusableRemodelingSample`
- Kentico guide: [Remodel page types as reusable field schemas](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides)
- Kentico guide: [Speed up remodeling with AI](https://docs.kentico.com/x/speed_up_remodeling_with_ai_guides)
- Registration details: [Data Transformation Extensions](Customization-Data-Transformation-Extensions.md#registration)
