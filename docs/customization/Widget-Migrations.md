# Customization: Widget Migrations

> **Audience:** Developers implementing widget-level transformations using `IWidgetMigration`.

## Purpose

Widget migrations customize widget types or restructures widget data during migration.

**Use cases:**

- Handle renamed widget types
- Consolidate multiple widgets into one
- Change widget structure

## Create Custom Widget Migrations

You can customize widget migration to change the widget to which source widgets are migrated in the target instance. In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IWidgetMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than _100000_, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetMigrationContext` and `WidgetIdentifier` objects passed as an argument to the method to evaluate the condition. For each widget, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `WidgetMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetIdentifier.TypeIdentifier` - A string property that specifies the identifier of the widget.
- `MigrateWidget`- Migrate the widget data using the following properties:
  - `identifier` - A `WidgetIdentifier` object, see `ShallMigrate` to see properties.
  - `value` - A `JToken` object containing the deserialized value of the property.
  - `context` - A `WidgetMigrationContext` object, see `ShallMigrate` to see properties.

You can see a sample: [SampleWidgetMigration.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleWidgetMigration.cs)

After implementing the migration, you need to [register the migration](#registration) in the system.

> [!TIP]
> For a complete end-to-end example, see our guide on [how to migrate widget data as reusable content](https://docs.kentico.com/x/migrate_widget_data_as_reusable_content_guides) in the Kentico documentation.

## Registration

Register widget migrations in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs) as `IWidgetMigration` implementations.

Example registration:

```csharp
services.AddTransient<IWidgetMigration, MyWidgetMigration>();
```

> [!WARNING]
> After adding or updating widget migrations, rebuild the migration tool solution before running migration.

For general registration guidance across all customization types, see [Data Transformation Extensions](Data-Transformation-Extensions.md#registration).
