# Migration Tool Customization

## About This Project

**Migration.Tool.Extensions** is the customization layer for the Kentico Migration Tool. Use this project to implement custom logic that transforms your data during migration from Kentico Xperience 13, Kentico 12, or Kentico 11 to Xperience by Kentico.

The project enables you to:
- Customize migration behavior with custom transformations
- Handle project-specific data structures and field mappings
- Transform widget properties and Page Builder content
- Remodel content types and merge page types
- Control how linked pages and child relationships are migrated

> **User Interaction:** This project is where you implement custom migration logic. Modify this project to add field migrations, widget migrations, and custom class mappings that transform your data during migration.

> This page provides implementation guidance for custom migrations. For conceptual understanding and migration strategies, see the [Upgrade Deep Dive guides](https://docs.kentico.com/x/upgrade_deep_dives_guides) or take the [Developer training: Upgrade to Xperience by Kentico](https://docs.kentico.com/paths/xbyk-upgrade-developer) learning path.

> [!TIP]
> When working with KX13 source APIs, AI coding assistants can use the [Kentico Xperience 13 library on Context7](https://context7.com/websites/kentico_13) for accurate KX13 documentation context. Note that Context7 is a third-party service not maintained or supported by Kentico.

## Table of Contents

- [Available Customization Types](#available-customization-types)
- [When Custom Migrations Execute](#when-custom-migrations-execute)
- [Implementation Guides](#implementation-guides)
  - [Custom Class Mappings](#custom-class-mappings)
    - [Remodel Page Types as Reusable Field Schemas Guide](#remodel-page-types-as-reusable-field-schemas-guide)
    - [Sample Class Mappings](#sample-class-mappings)
  - [Customize Linked Page Handling](#customize-linked-page-handling)
  - [Migrate Pages to Widgets](#migrate-pages-to-widgets)
  - [Custom Child Links](#custom-child-links)
  - [Customize Field Migrations](#customize-field-migrations)
  - [Customize Widget Migrations](#customize-widget-migrations)
  - [Customize Widget Property Migrations](#customize-widget-property-migrations)
- [Registration](#registration)
- [Working with Source and Target APIs](#working-with-source-and-target-apis)

---

## Available Customization Types

Migration presents an opportunity to remodel your content structure. The following customization types allow you to transform data during migration.

Each customization type is implemented by creating a class that implements a specific interface (e.g., `IFieldMigration`, `IWidgetMigration`) or inherits from a base class (e.g., `ContentItemDirectorBase`). These interfaces and classes are provided by the Migration Tool and define the methods your custom logic must implement.

> [!NOTE]
> You can register **multiple implementations** of each customization type. For example, you can have multiple Directors, multiple class mappings, or multiple field migrations—each handling different content types or scenarios. The migration tool evaluates all registered implementations and applies those that match the current context.

> [!IMPORTANT] After implementing any custom migration, you must register it in the dependency injection container. See [Registration](#registration) for complete registration instructions.

### Custom Class Mappings (`IClassMapping`)

Transforms content type structure and field definitions between source and target instances.

You can create multiple class mappings, each handling different source content types. For example, one mapping can merge blog-related page types while another handles product page types.

**Use cases:**

- Merge multiple page types (e.g., `Article.BlogPost`, `Article.NewsArticle`) into a single content type
- Remodel page types as reusable field schemas by extracting common fields
- Change field names, data types, or form controls (e.g., rename `OldName` to `NewName` and convert from `text` to `longtext` or custom type)
- Split one page type into multiple content types
- Convert custom tables or module classes to content types

### Content Item Directors (`ContentItemDirectorBase`)

Controls migration behavior and relationships of individual content items during data migration.

You can create multiple Directors, each targeting different content types or scenarios. For example, one Director can handle linked pages for `Article` types while another handles `Product` types differently.

**Use cases:**

- Handle linked pages by materializing, dropping, or storing as references (e.g., drop links in `/Archive/`, materialize others)
- Migrate pages as widgets (e.g., convert `NewsItem` pages to `NewsWidget` widgets on their parent pages)
- Link child pages as content item references (e.g., link child `Book` pages in a `Books` field when migrating `Author` pages to reusable content)
- Apply conditional logic based on content structure or hierarchy

### Widget Migrations (`IWidgetMigration`)

Changes widget types or restructures widget data.

**Use cases:**

- Handle renamed widget types
- Consolidate multiple widgets into one
- Change widget structure

### Widget Property Migrations (`IWidgetPropertyMigration`)

Transforms individual widget property values.

**Use cases:**

- Update content references in widget properties
- Convert property value formats
- Transform paths or URLs in widget data

### Field Migrations (`IFieldMigration`)

Transforms individual field values during data migration.

**Use cases:**

- Handle custom form controls
- Convert data formats (date formats, URL structures)
- Transform content (HTML cleanup, path updates)

## When Custom Migrations Execute

When registered, customizations execute based on CLI migration parameters. The order depends on parameter dependencies (e.g., `--pages` requires `--page-types` to run first):

```powershell
.\Migration.Tool.CLI.exe migrate --page-types --custom-modules --forms --pages
```

For detailed information about all available CLI parameters, their dependencies, and execution order, see [Migrate command parameters](../Migration.Tool.CLI/README.md#migrate-command-parameters) in the Migration.Tool.CLI README.

| Migration Type | CLI Parameter |
|---|---|
| **Custom Class Mappings** | `--page-types` `--custom-modules` |
| **Content Item Directors** | `--pages` |
| **Field Migrations** | `--pages` `--forms` `--custom-tables` |
| **Widget Migrations** | `--pages` |
| **Widget Property Migrations** | `--pages` |

> [!NOTE]
> Class mappings run before page data migration. Field value transformations defined in class mappings (using `ConvertFrom`) are applied during content type structure migration, not during individual page processing.

**Execution during page migration:**

As multiple custom migrations run during the `--pages` migration, custom migrations execute in the following order for each page:

1. Content item directors control overall behavior (e.g., dropping pages, converting to widgets, handling linked pages)
2. Widget migrations and widget property migrations transform Page Builder content
3. Field migrations transform page field values

> [!TIP]
> Widget migrations execute before content items are saved to the database. To query migrated content items from the database during widget migration (e.g., to modify their property value), run the migration tool twice.

After all pages are migrated, the tool performs a final pass to update `TreePath` properties (converting `NodeAliasPath` references). This happens automatically and doesn't require custom migration logic.

### Criteria for Custom Migration Execution

The Kentico Migration Tool uses a handler-based architecture where handlers automatically call your custom migrations during data transformation. Typically, you do not interact with handlers directly—simply register your migrations and the migration tool will invoke them automatically.

Custom migrations are called during transformation:

1. **Selection** - The tool evaluates your migration's boolean `ShallMigrate()` method, which determines if your migration is applicable in the provided context (e.g., matching field names, widget types, or content types)
2. **Ranking** - If multiple migrations match, the one with the lowest `Rank` value is selected and applied
3. **Execution** - Once selected, the tool invokes your transformation method to migrate the data (e.g., `MigrateValue()`, `MigrateWidget()`, or `Direct()`)

## Implementation Guides

The following sections provide detailed instructions for implementing each type of custom migration.

### Custom Class Mappings

You can customize class mappings to adjust the content model between the source instance and the target Xperience by Kentico instance. For example, you can merge multiple page types into a single content type, remodel page types as [reusable field schemas](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides), or migrate them to the [content hub](https://docs.kentico.com/x/barWCQ) as reusable content.

1. Create a new class.
2. Add an `IServiceCollection` extension method. Use a separate method for every class mapping that you wish to configure.
3. Within the extension method, define a new `MultiClassMapping` object:

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
    ```

4. Define a new primary key:

    ```csharp
    m.BuildField("ArticleID").AsPrimaryKey();
    ```

5. Define individual fields of the new content type:

    ```csharp
    // Builds a new title field
    var title = m.BuildField("Title");

    // You can map any number of source fields. The migration creates items of the target data class for every item from a mapped source data class.
    // The default migration is skipped for any data class / page type where you map at least one source field

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

6. (*Optional*) You can add custom value conversions:

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
                    // (var nodeGuid, int nodeSiteId, int? documentId, bool migratingFromVersionHistory) = treeNodeContext;
                    break;
                default:
                    // No context is available (possibly when the tool is extended with other conversion possibilities)
                    break;
            }

            return v?.ToString() is { } av && !string.IsNullOrWhiteSpace(av) ? DateTime.Parse(av) : null;
        });
    startDate.WithFieldPatch(f => f.Caption = "Event start date");
    ```

7. Complete the implementation by [registering the class mapping](#registration) in the dependency injection container.

8. Ensure that your class mapping extension methods run during the startup of the migration tool. Call the methods from `UseCustomizations` in the [ServiceCollectionExtensions](/Migration.Tool.Extensions/ServiceCollectionExtensions.cs) class.

**Note**: Your mappings now replace the default migration functionality for all data classes (page types, custom tables or custom module classes) that you use as a source. Any class where you set at least one source field is affected. If you map only some fields from a source class, the remaining fields are not migrated at all.

If you need class mappings to alter several data classes in Kentico Xperience 13, consider [using AI tools to help you generate mappings quickly](https://docs.kentico.com/x/speed_up_remodeling_with_ai_guides).

#### Remodel Page Types as Reusable Field Schemas Guide

For an end-to-end example of how to extract common fields from two page types from Kentico Xperience 13 and move them to a [reusable field schema](https://docs.kentico.com/x/D4_OD) shared by both web page content types in Xperience by Kentico follow this [migration guide](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides) in the documentation.

Note that any usage of `ReusableSchemaBuilder` in custom class mappings cannot be combined together with the `Settings.CreateReusableFieldSchemaForClasses` configuration option.

#### Sample Class Mappings

You can find sample class mappings in the [ClassMappingSample.cs](/Migration.Tool.Extensions/ClassMappings/ClassMappingSample.cs) file.

- `AddSimpleRemodelingSample` showcases how to change the mapping of a single page type
- `AddClassMergeSample` showcases how to merge two page types into a single content type
- `AddReusableRemodelingSample` showcases how to migrate a page type as reusable content

### Customize Linked Page Handling

When migrating from Kentico versions that support [linked pages](https://docs.kentico.com/13/managing-website-content/working-with-pages/copying-and-moving-pages-creating-linked-pages#creating-linked-pages) (pages that reference content from other pages in the content tree), you need to decide how to handle them since Xperience by Kentico doesn't support linked pages in the same way.

The linked pages director feature provides a flexible solution to customize how linked pages are handled during migration. You can choose to materialize linked content, drop it entirely, or store references in ancestor pages.

#### Understanding Linked Pages

In older Kentico versions, linked pages allowed you to create pages that displayed content from other pages without duplicating the actual content. This was useful for:
- Sharing content across multiple sections of a website
- Creating content references without data duplication  
- Building complex content hierarchies

#### Migration Strategies

The linked pages director offers three main strategies for handling linked pages:

#### 1. Materialize (Default)
Creates a full copy of the linked content as an independent page.
- **Use when**: You want to preserve the content structure but can accept content duplication
- **Result**: Each linked page becomes a separate content item with its own copy of the data

#### 2. Drop
Completely skips migration of the linked page.
- **Use when**: The linked content is no longer needed or should be handled manually
- **Result**: The linked page will not be migrated to the target instance

#### 3. Store as Reference
Creates a content item reference field in an ancestor page that points to the original linked content.
- **Use when**: You want to preserve the relationship without duplicating content
- **Result**: The linked content is referenced through a content item selector field

#### Implementation

In `Migration.Tool.Extensions/CommunityMigrations`, create a new file with a class that inherits from the `ContentItemDirectorBase` class and override the `DirectLinkedNode(source, options)` method.

The `LinkedPageSource` provides access to:
- `source.SourceSite` - The site where the linked page exists
- `source.SourceNode` - The node that contains the link  
- `source.LinkedNode` - The target node being linked to

Implement your decision logic based on available node properties (`NodeClassID`, `NodeAliasPath`, `NodeName`, etc.) and call the appropriate action method.

#### Available Actions

#### `options.Drop()`
Skips migration of the linked page entirely. Use for temporary content, archived pages, or content that should be handled manually.

#### `options.Materialize()`
Creates an independent copy of the linked content (default behavior). This preserves the content structure but results in content duplication.

#### `options.StoreReferenceInAncestor(parentLevel, fieldName)`
Creates a content item reference field in an ancestor page that points to the original linked content.

**Parameters:**
- `parentLevel`: Relative level of the ancestor page (-1 = direct parent, -2 = grandparent, etc.)
- `fieldName`: Name of the content item reference field (created automatically if it doesn't exist)

#### Common Strategies

- **Content Type-Based**: Use `NodeClassID` to look up the content type and apply different strategies based on page type.
- **Path-Based**: Filter by `NodeAliasPath` to handle different sections of your site (e.g., archive pages, temporary content).
- **Site-Specific**: Use `source.SourceSite.SiteName` to apply different rules for different sites in multi-site scenarios.
- **Contextual**: Combine node properties with ancestor analysis to make intelligent decisions about reference placement.

#### Important Considerations

1. **Field creation**: When using `StoreReferenceInAncestor`, the content item reference field is created automatically if it doesn't exist.

2. **Allowed types**: If the reference field exists but doesn't allow the linked content's type, the type is automatically added to the allowed types.

3. **Content hub**: For the reference strategy to work properly, ensure that the linked content types are configured as reusable content in your `appsettings.json` under `ConvertClassesToContentHub`.

4. **Processing order**: Linked pages are processed using topological sorting to ensure that referenced content is migrated before the pages that reference it.

5. **Cross-site links**: Links between different sites are not supported and will be skipped with a warning.

#### Sample Implementation

You can see a comprehensive sample implementation in [SampleLinkedPageDirector.cs](./CommunityMigrations/SampleLinkedPageDirector.cs) that demonstrates various strategies for handling linked pages based on content type, path, and site context.

After implementing your linked page director, you need to [register the director](#registration) in the system.

#### Troubleshooting

**Common Issues:**
- **"Ancestor not found" error**: Check that the `parentLevel` value is correct (negative values for ancestors)
- **References not working**: Ensure the content type is in `ConvertClassesToContentHub` configuration  
- **Deferred processing**: Some linked pages may be processed in a second pass if their dependencies aren't ready

**Debugging Tips:**
- Use [logging](../Migration.Tool.CLI/README.md#logging) to track which strategy is applied to each linked page
- Verify that ancestor pages exist and have the expected structure

### Migrate Pages to Widgets

This migration allows you to migrate pages from the source instance as [widgets](https://docs.kentico.com/x/7gWiCQ) in the target instance. This migration can be used in the following ways:

- If you have a page with content stored in page fields, you can migrate the values of the fields into widget properties and display the content as a widget.
- If you have a page that serves as a listing and displays content from child pages, you can convert the child pages into widgets and as content items in the content hub, then link them from the widgets.

> :warning:
> The target page (with a [Page Builder editable area](https://docs.kentico.com/x/7AWiCQ)) and any [Page Builder components](https://docs.kentico.com/x/6QWiCQ) used in the migration need to be present in the system before you migrate content. The target page must be either the page itself or any ancestor of the page from which the content is migrated.

In `Migration.Tool.Extensions/CommunityMigrations`, create a new file with a class that inherits from the `ContentItemDirectorBase` class and override the `Direct(source, options)` method:

1. If the target page uses a [page template](https://docs.kentico.com/x/iInWCQ), ensure that the correct page template is applied.

    ```csharp
    // Store page uses a template and is the parent listing page
    if (source.SourceNode.SourceClassName == "Acme.Store")
    {
      // Ensures the page template is present in the system
      options.OverridePageTemplate("StorePageTemplate");
    }
    ```

2. Identify pages you want to migrate to widgets and use the `options.AsWidget()` action.

    ```csharp
    // Identifies pages by their content type
    else if (source.SourceNode.SourceClassName == "Acme.Coffee")
    {
        options.AsWidget("Acme.CoffeeSampleWidget", null, null, options =>
        {
            // Determines where to place the widget
            options.Location
                // Negative indexing is used - '-1' signifies direct parent node
                // Use the value of '0' if you want to target the page itself
                .OnAncestorPage(-1)
                .InEditableArea("main-area")
                .InSection("SingleColumnSection")
                .InFirstZone();

            // Specifies the widget's properties
            options.Properties.Fill(true, (itemProps, reusableItemGuid, childGuids) =>
            {
                // Simple way to achieve basic conversion of all properties, properties can be refined in the following steps
                var widgetProps = JObject.FromObject(itemProps);

                // The converted page is linked as a reusable content item into a single property of the widget.
                // NOTE: List the page class name app settings in ConvertClassesToContentHub to make it reusable!
                widgetProps["LinkedContent"] = LinkedItemPropertyValue(reusableItemGuid!.Value);

                // Link reusable content items created from page's original subnodes
                // NOTE: List the page class names in app settings in ConvertClassesToContentHub to make it reusable!
                widgetProps["LinkedChildren"] = LinkedItemsPropertyValue(childGuids);

                return widgetProps;
            });
        });
    }
    ```

After implementing the content item director, you need to [register the director](#registration) in the system.

> [!TIP]
> You can see a sample implementation in [SamplePageToWidgetDirector.cs](./CommunityMigrations/SamplePageToWidgetDirector.cs) or follow along with our complete practical example on [how to convert child pages to widgets](https://docs.kentico.com/x/convert_child_pages_to_widgets_guides) in the Kentico documentation.

### Custom Child Links

This feature allows you to link child pages as referenced content items of a page converted to reusable content item.

This feature is available by means of content item director.

You can apply a simple general rule to link child pages e.g. in `Children` field or you can apply more elaborate rules. You can see samples of both approaches in [SampleChildLinkDirector.cs](./CommunityMigrations/SampleChildLinkDirector.cs) or follow along with our [guide to transfer page hierarchy to the Content hub](https://docs.kentico.com/x/transfer_page_hierarchy_to_content_hub_guides).

After implementing the content item director, you need to [register the director](#registration) in the system.

### Customize Field Migrations

You can customize field migrations to control how fields are mapped for page types, modules, system objects, and forms. In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IFieldMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current field. Use properties of the `FieldMigrationContext` object passed as an argument to the method to evaluate the condition. For each field, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `SourceDataType` - A string property that specifies the [data type](https://docs.kentico.com/x/coJwCg) of the source field.
  - `SourceFormControl` - A string property that specifies the [form control](https://docs.kentico.com/x/lAyRBg) used by the source field.
  - `FieldName` - A string property that specifies the code name of the field.
  - `SourceObjectContext` - An interface that can be used to check the context of the source object (e.g. page vs. form).
- `MigrateFieldDefinition` - Migrate the definition of the field. Use the `XElement` attribute to transform the field.
- `MigrateValue` - Migrate the value of the field. Use the source value and the `FieldMigrationContext` with the same parameters as described in `ShallMigrate`.

You can see samples:

- [SampleTextMigration.cs](./CommunityMigrations/SampleTextMigration.cs)
  - A sample migration with further explanations
- [AssetMigration.cs](./DefaultMigrations/AssetMigration.cs)
  - An example of a usable migration

After implementing the migration, you need to [register the migration](#registration) in the system.

### Customize Widget Migrations

You can customize widget migration to change the widget to which source widgets are migrated in the target instance. In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IWidgetMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetMigrationContext` and `WidgetIdentifier` objects passed as an argument to the method to evaluate the condition. For each widget, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `WidgetMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetIdentifier.TypeIdentifier` - A string property that specifies the identifier of the widget.
- `MigrateWidget`- Migrate the widget data using the following properties:
  - `identifier` - A `WidgetIdentifier` object, see `ShallMigrate` to see properties.
  - `value` - A `JToken` object containing the deserialized value of the property.
  - `context` - A `WidgetMigrationContext` object, see `ShallMigrate` to see properties.

You can see a sample: [SampleWidgetMigration.cs](./CommunityMigrations/SampleWidgetMigration.cs)

After implementing the migration, you need to [register the migration](#registration) in the system.

> [!TIP]
> For a complete end-to-end example, see our guide on [how to migrate widget data as reusable content](https://docs.kentico.com/x/migrate_widget_data_as_reusable_content_guides) in the Kentico documentation.

### Customize Widget Property Migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IWidgetPropertyMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetPropertyMigrationContext` and `propertyName` objects passed as an argument to the method to evaluate the condition. For each widget property, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `WidgetPropertyMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetPropertyMigrationContext.EditingFormControlModel` - An object representing the [form control](https://docs.kentico.com/x/lAyRBg) of the property.
  - `propertyName` - A string property that specifies the identifier of the property.
- `MigrateWidgetProperty`- Migrate the widget property data using the following properties:
  - `key` - Name of the property.
  - `value` - A `JToken` object containing the deserialized value of the property.
  - `context` - A `WidgetPropertyMigrationContext`. See `ShallMigrate` method to see the properties.

You can see samples:

- [Path selector migration](./DefaultMigrations/WidgetPathSelectorMigration.cs)
- [Page selector migration](./DefaultMigrations/WidgetPageSelectorMigration.cs)
- [File selector migration](./DefaultMigrations/WidgetFileMigration.cs)

After implementing the migration, you need to [register the migration](#registration) in the system.

> [!TIP]
> For common widget property transformation scenarios, see [our technical deep-dive guide](https://docs.kentico.com/x/transform_widget_properties_guides) in the Kentico documentation.

## Registration

All custom migrations must be registered in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as dependencies in the service collection.

- Use `AddTransient` for field, widget, and director migrations
- Use `AddSingleton` for class mappings
- **After adding or modifying customizations, rebuild your migration tool solution before running the migration**

**Registration by migration type:**

- Field migrations - `services.AddTransient<IFieldMigration, MyFieldMigration>();`
- Linked page directors - `services.AddTransient<ContentItemDirectorBase, MyLinkedPageDirector>();`
- Widget migrations - `services.AddTransient<IWidgetMigration, MyWidgetMigration>();`
- Widget property migrations - `services.AddTransient<IWidgetPropertyMigration, MyWidgetPropertyMigration>();`
- Page to widget migrations - `services.AddTransient<ContentItemDirectorBase, MyPageToWidgetDirector>();`
- Class mappings - `services.AddSingleton<IClassMapping>(m);` (where `m` is your `MultiClassMapping` instance)
- Child link directors - `services.AddTransient<ContentItemDirectorBase, MyChildLinkDirector>();`

## Working with Source and Target APIs

When implementing custom migrations, you may need to query additional data from the source instance or create data in the target instance.

### Querying Source Instance Data

Use [`ModelFacade`](../KVA/Migration.Tool.Source/ModelFacade.cs) to query data from the source instance (K11/KX12/KX13). Inject it into your custom migration class and use methods like `SelectById<T>()`, `SelectWhere<T>()`, or `Select<T>()` to retrieve source data.

### Writing to Target Instance

Use `IImporter` with [Universal Migration Toolkit (UMT)](https://github.com/Kentico/xperience-by-kentico-universal-migration-tool) models to write data to the target Xperience by Kentico instance. Common UMT models include `ContentItemModel`, `DataClassModel`, `ContentItemLanguageMetadataModel`, and `WebPageItemModel`.

For advanced scenarios, you can also use native [Xperience database table APIs](https://docs.kentico.com/x/OoXWCQ) and the Info/Provider pattern.
