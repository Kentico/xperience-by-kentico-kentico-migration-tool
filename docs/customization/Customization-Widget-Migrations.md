# Customization: Widget Migrations

> **Audience:** Developers implementing widget-level transformations using `IWidgetMigration`.

## Purpose

Widget migrations customize widget type/structure migration behavior for page content.

## Extension Point

Implement `IWidgetMigration` in `Migration.Tool.Extensions/CommunityMigrations`.

Required members:

- `Rank`
- `ShallMigrate(WidgetMigrationContext context, WidgetIdentifier identifier)`
- `MigrateWidget(WidgetIdentifier identifier, JToken value, WidgetMigrationContext context)`

Useful context information:

- `WidgetMigrationContext.SiteId`
- `WidgetIdentifier.TypeIdentifier`

## Sample

- [Migration.Tool.Extensions/CommunityMigrations/SampleWidgetMigration.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleWidgetMigration.cs)

Kentico guide: [Migrate widget data as reusable content](https://docs.kentico.com/x/migrate_widget_data_as_reusable_content_guides)

## Registration

Register in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddTransient<IWidgetMigration, MyWidgetMigration>();
```

See also: [Data Transformation Extensions](Customization-Data-Transformation-Extensions.md#registration)
