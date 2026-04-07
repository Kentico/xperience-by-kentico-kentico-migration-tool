# Data Transformation Extensions

> **Audience:** Developers implementing project-specific transformation logic in `Migration.Tool.Extensions`.

Use this page as the docs-first entry point for data transformation extensions.

Use this page when configuration alone is not enough and you need custom transformation logic for specific fields, widgets, content types, or page relationships.

> For conceptual understanding and migration strategies, see the [Upgrade Deep Dive guides](https://docs.kentico.com/x/upgrade_deep_dives_guides). If you are a developer new to upgrades and data migration, take the [developer training on upgrades](https://docs.kentico.com/paths/xbyk-upgrade-developer) in our Learn portal.

> [!TIP]
> When working with KX13 source APIs, AI coding assistants can use the [Kentico Xperience 13 library on Context7](https://context7.com/websites/kentico_13) for accurate KX13 documentation context. Note that Context7 is a third-party service not maintained or supported by Kentico.

## Available Customization Types

Migration presents an opportunity to remodel your content structure. The following customization types allow you to transform data during migration.

Each customization type is implemented by creating a class that implements a specific interface (e.g., `IFieldMigration`, `IWidgetMigration`) or inherits from a base class (e.g., `ContentItemDirectorBase`). These interfaces and classes are provided by the Migration Tool and define the methods your custom logic must implement.

> [!NOTE]
> You can register **multiple implementations** of each customization type. For example, you can have multiple Directors, multiple class mappings, or multiple field migrations—each handling different content types or scenarios. The migration tool evaluates all registered implementations and applies those that match the current context.

> [!IMPORTANT]
> After implementing any custom migration, you must register it in the dependency injection container. See [Registration](#registration) for complete registration instructions.

### Custom Class Mappings (`IClassMapping`)

Transforms content type structure and field definitions between source and target instances.

You can create multiple class mappings, each handling different source content types. For example, one mapping can merge blog-related page types while another handles product page types.

See [Class Mappings (`IClassMapping`)](Class-Mappings.md) for details and implementation.

**Use cases:**

- Merge multiple page types (e.g., `Article.BlogPost`, `Article.NewsArticle`) into a single content type
- Remodel page types as reusable field schemas by extracting common fields
- Change field names, data types, or form controls (e.g., rename `OldName` to `NewName` and convert from `text` to `longtext` or custom type)
- Split one page type into multiple content types
- Convert custom tables or module classes to content types

### Content Item Directors (`ContentItemDirectorBase`)

Controls migration behavior and relationships of individual content items during data migration.

You can create multiple Directors, each targeting different content types or scenarios. For example, one Director can handle linked pages for `Article` types while another handles `Product` types differently.

See [Content Item Directors (`ContentItemDirectorBase`)](Content-Item-Directors.md) for details and implementation.

**Use cases:**

- Handle linked pages by materializing, dropping, or storing as references (e.g., drop links in `/Archive/`, materialize others)
- Migrate pages as widgets (e.g., convert `NewsItem` pages to `NewsWidget` widgets on their parent pages)
- Link child pages as content item references (e.g., link child `Book` pages in a `Books` field when migrating `Author` pages to reusable content)
- Apply conditional logic based on content structure or hierarchy

### Widget Migrations (`IWidgetMigration`)

Changes widget types or restructures widget data.

See [Widget Migrations (`IWidgetMigration`)](Widget-Migrations.md) for details and implementation.

**Use cases:**

- Handle renamed widget types
- Consolidate multiple widgets into one
- Change widget structure

### Widget Property Migrations (`IWidgetPropertyMigration`)

Transforms individual widget property values.

See [Widget Property Migrations (`IWidgetPropertyMigration`)](Widget-Property-Migrations.md) for details and implementation.

**Use cases:**

- Update content references in widget properties
- Convert property value formats
- Transform paths or URLs in widget data

### Field Migrations (`IFieldMigration`)

Transforms individual field values during data migration.

See [Field Migrations (`IFieldMigration`)](Field-Migrations.md) for details an implementation.

**Use cases:**

- Handle custom form controls
- Convert data formats (date formats, URL structures)
- Transform content (HTML cleanup, path updates)

## When Custom Migrations Execute

When registered, customizations execute based on CLI migration parameters. The order depends on parameter dependencies (e.g., `--pages` requires `--page-types` to run first):

```powershell
.\Migration.Tool.CLI.exe migrate --page-types --custom-modules --forms --pages
```

For detailed information about all available CLI parameters, their dependencies, and execution order, see [Migrate command parameters](../../Migration.Tool.CLI/README.md#migrate-command-parameters) in the Migration.Tool.CLI README.

| Migration Type                 | CLI Parameter                         |
| ------------------------------ | ------------------------------------- |
| **Custom Class Mappings**      | `--page-types` `--custom-modules`     |
| **Content Item Directors**     | `--pages`                             |
| **Field Migrations**           | `--pages` `--forms` `--custom-tables` |
| **Widget Migrations**          | `--pages`                             |
| **Widget Property Migrations** | `--pages`                             |

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

> [!TIP]
> Use `Rank < 100000` in custom migrations when you want to override default system migrations.

## Registration

Register custom implementations in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs).

- `AddSingleton` for class mappings
- `AddTransient` for field/widget/director implementations

Rebuild the solution after changing registrations or custom migration classes.

For more details by migration type, visit the detailed customization pages in the [Available Customization Types](#available-customization-types) section above.

## Working with Source and Target APIs

- Query source data using [ModelFacade](../../KVA/Migration.Tool.Source/ModelFacade.cs); for usage patterns, see [ModelFacade in Pipeline Behaviors](Pipeline-Behaviors.md#modelfacade).
- Write to target using `IImporter` with UMT models (`ContentItemModel`, `DataClassModel`, `ContentItemLanguageMetadataModel`, `WebPageItemModel`).
- For advanced scenarios, native [Xperience table APIs](https://docs.kentico.com/documentation/developers-and-admins/api/database-table-api) are also available.

## Related Documentation

- [Customization Guide](../Customization-Guide.md) - Overview of available Kentico Migration Tool customization options and recommended decision path.
- [Pipeline Behaviors](Pipeline-Behaviors.md) - If your scenario is not covered by data transformation extensions and you need logic in the pipeline of a specific migration command (for example `--sites` or `--pages`), use pipeline behaviors.
- [Repository Structure](../Repository-Structure.md) - Project/component map showing where customization code belongs.
- [Migration.Tool.Extensions README](../../Migration.Tool.Extensions/README.md) - Scope and extension-point summary for the `Migration.Tool.Extensions` project.
