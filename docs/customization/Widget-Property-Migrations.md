# Customization: Widget Property Migrations

> **Audience:** Developers implementing widget-property transformations using `IWidgetPropertyMigration`.

## Purpose

Widget property migrations customize transformations of individual widget properties during data migration.

**Use cases:**

- Update content references in widget properties
- Convert property value formats
- Transform paths or URLs in widget data

## Create Custom Widget Property Migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IWidgetPropertyMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than _100000_, as that is the value for system migrations.
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

Register widget property migrations in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs) as `IWidgetPropertyMigration` implementations.

Example registration:

```csharp
services.AddTransient<IWidgetPropertyMigration, MyWidgetPropertyMigration>();
```

> [!WARNING]
> After adding or updating widget property migrations, rebuild the migration tool solution before running migration.

For general registration guidance across all customization types, see [Data Transformation Extensions](Data-Transformation-Extensions.md#registration).
