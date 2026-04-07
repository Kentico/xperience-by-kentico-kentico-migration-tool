# Customization: Content Item Directors

> **Audience:** Developers customizing page/content-item migration flow and relationships.

Content item directors (`ContentItemDirectorBase`) control migration behavior and relationships of individual content items during data migration.

You can create multiple Directors, each targeting different content types or scenarios. For example, one Director can handle linked pages for `Article` types while another handles `Product` types differently.

**Use cases:**

- Handle linked pages by materializing, dropping, or storing as references (e.g., drop links in `/Archive/`, materialize others)
- Migrate pages as widgets (e.g., convert `NewsItem` pages to `NewsWidget` widgets on their parent pages)
- Link child pages as content item references (e.g., link child `Book` pages in a `Books` field when migrating `Author` pages to reusable content)
- Apply conditional logic based on content structure or hierarchy

## Customize Linked Page Handling

When migrating from Kentico versions that support [linked pages](https://docs.kentico.com/13/managing-website-content/working-with-pages/copying-and-moving-pages-creating-linked-pages#creating-linked-pages) (pages that reference content from other pages in the content tree), you need to decide how to handle them since Xperience by Kentico doesn't support linked pages in the same way.

The linked pages director feature provides a flexible solution to customize how linked pages are handled during migration. You can choose to materialize linked content, drop it entirely, or store references in ancestor pages.

### Understanding Linked Pages

In older Kentico versions, linked pages allowed you to create pages that displayed content from other pages without duplicating the actual content. This was useful for:

- Sharing content across multiple sections of a website
- Creating content references without data duplication
- Building complex content hierarchies

### Migration Strategies

The linked pages director offers three main strategies for handling linked pages:

### 1. Materialize (Default)

Creates a full copy of the linked content as an independent page.

- **Use when**: You want to preserve the content structure but can accept content duplication
- **Result**: Each linked page becomes a separate content item with its own copy of the data

### 2. Drop

Completely skips migration of the linked page.

- **Use when**: The linked content is no longer needed or should be handled manually
- **Result**: The linked page will not be migrated to the target instance

### 3. Store as Reference

Creates a content item reference field in an ancestor page that points to the original linked content.

- **Use when**: You want to preserve the relationship without duplicating content
- **Result**: The linked content is referenced through a content item selector field

### Implementation

In `Migration.Tool.Extensions/CommunityMigrations`, create a new file with a class that inherits from the `ContentItemDirectorBase` class and override the `DirectLinkedNode(source, options)` method.

The `LinkedPageSource` provides access to:

- `source.SourceSite` - The site where the linked page exists
- `source.SourceNode` - The node that contains the link
- `source.LinkedNode` - The target node being linked to

Implement your decision logic based on available node properties (`NodeClassID`, `NodeAliasPath`, `NodeName`, etc.) and call the appropriate action method.

### Available Actions

### `options.Drop()`

Skips migration of the linked page entirely. Use for temporary content, archived pages, or content that should be handled manually.

### `options.Materialize()`

Creates an independent copy of the linked content (default behavior). This preserves the content structure but results in content duplication.

### `options.StoreReferenceInAncestor(parentLevel, fieldName)`

Creates a content item reference field in an ancestor page that points to the original linked content.

**Parameters:**

- `parentLevel`: Relative level of the ancestor page (-1 = direct parent, -2 = grandparent, etc.)
- `fieldName`: Name of the content item reference field (created automatically if it doesn't exist)

### Common Strategies

- **Content Type-Based**: Use `NodeClassID` to look up the content type and apply different strategies based on page type.
- **Path-Based**: Filter by `NodeAliasPath` to handle different sections of your site (e.g., archive pages, temporary content).
- **Site-Specific**: Use `source.SourceSite.SiteName` to apply different rules for different sites in multi-site scenarios.
- **Contextual**: Combine node properties with ancestor analysis to make intelligent decisions about reference placement.

### Important Considerations

1. **Field creation**: When using `StoreReferenceInAncestor`, the content item reference field is created automatically if it doesn't exist.

2. **Allowed types**: If the reference field exists but doesn't allow the linked content's type, the type is automatically added to the allowed types.

3. **Content hub**: For the reference strategy to work properly, ensure that the linked content types are configured as reusable content in your `appsettings.json` under `ConvertClassesToContentHub`.

4. **Processing order**: Linked pages are processed using topological sorting to ensure that referenced content is migrated before the pages that reference it.

5. **Cross-site links**: Links between different sites are not supported and will be skipped with a warning.

### Sample Implementation

You can see a comprehensive sample implementation in [SampleLinkedPageDirector.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleLinkedPageDirector.cs) that demonstrates various strategies for handling linked pages based on content type, path, and site context.

After implementing your linked page director, you need to [register the director](#registration) in the system.

### Troubleshooting

**Common Issues:**

- **"Ancestor not found" error**: Check that the `parentLevel` value is correct (negative values for ancestors)
- **References not working**: Ensure the content type is in `ConvertClassesToContentHub` configuration
- **Deferred processing**: Some linked pages may be processed in a second pass if their dependencies aren't ready

**Debugging Tips:**

- Use [logging](../../Migration.Tool.CLI/README.md#logging) to track which strategy is applied to each linked page
- Verify that ancestor pages exist and have the expected structure

## Migrate Pages to Widgets

This migration allows you to migrate pages from the source instance as [widgets](https://docs.kentico.com/x/7gWiCQ) in the target instance. This migration can be used in the following ways:

- If you have a page with content stored in page fields, you can migrate the values of the fields into widget properties and display the content as a widget.
- If you have a page that serves as a listing and displays content from child pages, you can convert the child pages into widgets and as content items in the content hub, then link them from the widgets.

> [!WARNING]
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
> You can see a sample implementation in [SamplePageToWidgetDirector.cs](../../Migration.Tool.Extensions/CommunityMigrations/SamplePageToWidgetDirector.cs) or follow along with our complete practical example on [how to convert child pages to widgets](https://docs.kentico.com/x/convert_child_pages_to_widgets_guides) in the Kentico documentation.

## Custom Child Links

This feature allows you to link child pages as referenced content items of a page converted to reusable content item.

This feature is available through a content item director.

You can apply a simple general rule to link child pages e.g. in `Children` field or you can apply more elaborate rules.

See samples of both approaches in [SampleChildLinkDirector.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleChildLinkDirector.cs) or follow along with our [guide to transfer page hierarchy to the Content hub](https://docs.kentico.com/x/transfer_page_hierarchy_to_content_hub_guides).

After implementing the content item director, you need to [register the director](#registration) in the system.

## Registration

Register directors in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs) as `ContentItemDirectorBase` implementations.

Example:

```csharp
services.AddTransient<ContentItemDirectorBase, MyLinkedPageDirector>();
```

> [!WARNING]
> After adding or updating content item directors, rebuild the migration tool solution before running migration.

For general registration guidance across all customization types, see [Data Transformation Extensions](Data-Transformation-Extensions.md#registration).
