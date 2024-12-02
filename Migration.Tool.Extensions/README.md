To create custom migrations:

1. Create the custom migration class:
  - [Field migrations](#Customize field migrations)
  - [Widget migrations](#Customize widget migrations)
  - [Widget property migrations](#Customize widget property migrations)
2. [Register the migration](#Register migrations)

## Customize field migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new class that implements the `IFieldMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current field. Use properties of the `FieldMigrationContext` object passed as an argument to the method to evaluate the condition:
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

After implementing the migration, you need to [register the migration](#Register migrations) in the system.

## Customize widget migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new class that implements the `IWidgetMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetMigrationContext` and `WidgetIdentifier` objects passed as an argument to the method to evaluate the condition:
  - `WidgetMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetIdentifier.TypeIdentifier` - A string property that specifies the identifier of the widget.
- `MigrateWidget`- Migrate the widget data using the following properties:
  - `identifier` - A `WidgetIdentifier` object, see `ShallMigrate` to see properties.
  - `value` - A `JToken` object containing the deserialized value of the property.
  - `context` - A `WidgetMigrationContext` object, see `ShallMigrate` to see properties.

You can see a sample: [SampleWidgetMigration.cs](./CommunityMigrations/SampleWidgetMigration.cs)

After implementing the migration, you need to [register the migration](#Register migrations) in the system.

## Customize widget property migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new class that implements the `IWidgetPropertyMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetPropertyMigrationContext` and `propertyName` objects passed as an argument to the method to evaluate the condition:
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

After implementing the migration, you need to [register the migration](#Register migrations) in the system.

## Register migrations

Register the migration in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as a `Transient` dependency into the service collection:

- Field migrations - `services.AddTransient<IFieldMigration, MyFieldMigration>();`
- Widget migrations - `services.AddTransient<IWidgetMigration, MyWidgetMigration>();`
- Widget property migrations - `services.AddTransient<IWidgetPropertyMigration, MyWidgetPropertyMigration>();`
