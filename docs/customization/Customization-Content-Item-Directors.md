# Customization: Content Item Directors

> **Audience:** Developers customizing page/content-item migration flow and relationships.

Content item directors (`ContentItemDirectorBase`) control migration behavior for individual content items.

## Typical Use Cases

- Customize linked page behavior (materialize, drop, or store as reference)
- Convert pages to widgets
- Link child pages as content item references
- Apply conditional migration logic based on content tree context

## Customize Linked Page Handling

When migrating from older Kentico versions, linked pages can be handled with custom strategies.

### Strategies

1. **Materialize** (default): create independent copy.
2. **Drop**: skip linked page migration.
3. **Store as reference**: add/maintain content item reference on ancestor.

### Implementation

Create a class in `Migration.Tool.Extensions/CommunityMigrations` inheriting from `ContentItemDirectorBase` and override `DirectLinkedNode(source, options)`.

`LinkedPageSource` includes:

- `source.SourceSite`
- `source.SourceNode`
- `source.LinkedNode`

Use available properties (`NodeClassID`, `NodeAliasPath`, `NodeName`, site name, etc.) to choose an action.

### Available actions

- `options.Drop()`
- `options.Materialize()`
- `options.StoreReferenceInAncestor(parentLevel, fieldName)`

`StoreReferenceInAncestor` parameters:

- `parentLevel`: relative ancestor level (`-1` = parent, `-2` = grandparent, ...)
- `fieldName`: content item reference field name (auto-created when missing)

### Considerations

1. Reference field is auto-created when needed.
2. Allowed types are auto-expanded when needed.
3. For reference strategy, configure linked content types as reusable (`ConvertClassesToContentHub`).
4. Linked pages are processed in topological order.
5. Cross-site links are skipped with warning.

Sample: [Migration.Tool.Extensions/CommunityMigrations/SampleLinkedPageDirector.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleLinkedPageDirector.cs)

## Migrate Pages to Widgets

You can migrate source pages as widgets in target pages.

Common patterns:

- Move page-field content into widget properties.
- Convert listing children into widgets and reusable content references.

> [!WARNING]
> Target pages, editable areas, and Page Builder components must exist before migration.

Example implementation in `Direct(source, options)`:

```csharp
// Store page uses a template and is the parent listing page
if (source.SourceNode.SourceClassName == "Acme.Store")
{
  // Ensures the page template is present in the system
  options.OverridePageTemplate("StorePageTemplate");
}
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

Sample: [Migration.Tool.Extensions/CommunityMigrations/SamplePageToWidgetDirector.cs](../../Migration.Tool.Extensions/CommunityMigrations/SamplePageToWidgetDirector.cs)

Kentico guide: [Convert child pages to widgets](https://docs.kentico.com/x/convert_child_pages_to_widgets_guides)

## Custom Child Links

Use content item directors to link child pages as referenced content items of pages converted to reusable content.

Samples:

- [Migration.Tool.Extensions/CommunityMigrations/SampleChildLinkDirector.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleChildLinkDirector.cs)
- Kentico guide: [Transfer page hierarchy to Content hub](https://docs.kentico.com/x/transfer_page_hierarchy_to_content_hub_guides)

## Registration

Register directors in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs) as `ContentItemDirectorBase` implementations.

Example:

```csharp
services.AddTransient<ContentItemDirectorBase, MyLinkedPageDirector>();
```

More details: [Targeted Code-Driven Customization](Customization-Targeted-Code.md#registration)
