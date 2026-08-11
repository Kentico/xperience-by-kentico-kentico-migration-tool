# Customization: Custom Class Mappings

> **Audience:** Developers remodeling content type structure during migration.

Class mappings (`IClassMapping`) transform content type structure and field definitions between source and target instances.

You can create multiple class mappings, each handling different source content types. For example, one mapping can merge blog-related page types while another handles product page types.

**Use cases:**

- Merge multiple page types (e.g., `Article.BlogPost`, `Article.NewsArticle`) into a single content type in the [Content hub](https://docs.kentico.com/x/barWCQ)
- Remodel page types as [reusable field schemas](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides) by extracting common fields
- Change field names, data types, or form controls (e.g., rename `OldName` to `NewName` and convert from `text` to `longtext` or custom type)
- Add new fields
- Split one page type into multiple content types
- Convert custom tables or module classes to content types
- Migrate custom fields added to system tables, such as - _Contact management > Contact management - Contact_.

## Create Custom Class Mappings

1. Create a new class.
1. Add an `IServiceCollection` extension method. Use a separate method for every class mapping that you wish to configure.
1. Within the extension method, define a new `MultiClassMapping` object:

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
   // For page content types, set to `true` to preserve URLs and routing (custom mappings do not inherit this automatically)
   // When omitted, defaults to `false` and routing is disabled; leave at the default for reusable content
   target.ClassWebPageHasUrl = true;
   });
   ```

   > [!WARNING]
   > When your target content type is used for pages (`ClassContentTypeType.WEBSITE`), you must explicitly set `target.ClassWebPageHasUrl = true` to preserve the content type's URLs and routing (the "Include in routing" option in the administration).
   >
   > The default migration (without a custom class mapping) sets this automatically based on the source page type. Custom class mappings build the target content type from scratch and do not inherit this behavior, so when the property is omitted it defaults to `false` and routing is disabled. Once the content type is created without routing, the setting cannot be enabled afterward - you need to correct the mapping and run the migration again.
   >
   > For reusable content (for example, items migrated into the [Content hub](https://docs.kentico.com/x/barWCQ)), this property does not apply - leave it at its default of `false`.

1. Define a new primary key:

   ```csharp
   m.BuildField("ArticleID").AsPrimaryKey();
   ```

1. Define individual fields of the new content type:

   ```csharp
   // Builds a new title field
   var title = m.BuildField("Title");

   // You can map any number of source fields. The migration creates items of the target data class for every item from a mapped source data class.
   // The default migration is skipped for any data class / page type where you map at least one source field.

   // Maps the "ArticleTitle" field from the source data class "Article.BlogPost"
   // Sets the isTemplate parameter to true, which makes the new field inherit the source field definition as a template
   // The field definition is migrated according to the migration tool's data type and form control/component mappings
   title.SetFrom("Article.BlogPost", "ArticleTitle", true);
   // Maps the "ArticleTitle" field from a second source data class "Article.NewsArticle"
   // The isTemplate parameter is not set, so only the value is taken from this source field
   title.SetFrom("Article.NewsArticle", "ArticleTitle");

   // You can modify the field definition, e.g., change the caption of the field
   title.WithFieldPatch(f => f.Caption = "Article title");
   ```

1. (_Optional_) You can add custom value conversions:

   ```csharp
   var publishDate = m.BuildField("PublishDate");
   publishDate.SetFrom("Article.BlogPost", "BlogPostDate", true);

   // Uses value conversion to modify the field value
   publishDate.ConvertFrom("Article.NewsArticle", "ArticleDateAsText", false,
   (v, context) =>
   {
       switch (context)
       {
       case ConvertorTreeNodeContext treeNodeContext:
           // You can use the available TreeNode context here
           // (var nodeGuid, int nodeSiteId, int? nodeSKUID, int? documentId, bool migratingFromVersionHistory) = treeNodeContext;
           break;
       default:
           // No context is available (possibly when the tool is extended with other conversion possibilities)
           break;
       }

       return v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null;
   });
   publishDate.WithFieldPatch(f => f.Caption = "Publish date");
   ```

1. Complete the implementation by [registering the class mapping](#registration) in the dependency injection container.

1. Ensure that your class mapping extension methods run during the startup of the migration tool. Call the methods from `UseCustomizations` in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs).

## Important Behavior

Mappings replace the default migration behavior for any source class with at least one mapped field. When the mapping covers only a subset of fields, the remaining fields in that source class are not migrated automatically.

## Reusable Field Schema Note

Usage of `ReusableSchemaBuilder` in custom class mappings cannot be combined with the `Settings.CreateReusableFieldSchemaForClasses` configuration option.

## Custom Contact Fields Note

If SQL Server Management Studio reports an invalid column name after the migration, refresh IntelliSense with `Ctrl+Shift+R`. To learn how to display migrated custom database fields of the `OM.Contact` table, see [Add a custom field to the Contact profile](https://docs.kentico.com/guides/development/customizations-and-integrations/add-custom-contact-field#define-your-ui-form).

### Remodel Page Types as Reusable Field Schemas Guide

For an end-to-end example of how to extract common fields from two page types from Kentico Xperience 13 and move them to a [reusable field schema](https://docs.kentico.com/x/D4_OD) shared by both web page content types in Xperience by Kentico, follow this [migration guide](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides).

If you need class mappings to alter several data classes in Kentico Xperience 13, consider [using AI tools to help you generate mappings quickly](https://docs.kentico.com/x/speed_up_remodeling_with_ai_guides).

## Sample Class Mappings

You can find sample class mappings in [Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs](../../Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs).

- `AddSimpleRemodelingSample` showcases how to change the mapping of a single page type.
- `AddClassMergeExample` showcases how to merge two page types into a single content type.
- `AddReusableRemodelingSample` showcases how to migrate a page type as reusable content.
- `AddCustomContactFieldMappingSample` showcases how to migrate custom fields added to system tables, such as `OM.Contact`.

## Registration

Register class mappings in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs) as `IClassMapping` implementations (typically via your `IServiceCollection` extension method).

Example registration in your mapping extension method:

```csharp
serviceCollection.AddSingleton<IClassMapping>(m);
```

> [!WARNING]
> After adding or updating class mappings, rebuild the migration tool solution before running migration.

For general registration guidance across all customization types, see [Data Transformation Extensions](Data-Transformation-Extensions.md#registration).
