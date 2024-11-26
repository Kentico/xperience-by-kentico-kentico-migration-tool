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
- `MigrateFieldDefinition` - migrate definition of the field
- `MigrateValue` - migrate value of the field

You can see a sample in ...

## Customize widget migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new class that implements the `IWidgetMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than *100000*, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current widget. Use properties of the `WidgetMigrationContext` and `WidgetIdentifier` objects passed as an argument to the method to evaluate the condition:
  - `WidgetMigrationContext.SiteId` - An integer property that specifies the ID of the site on which the widget was used in the source instance.
  - `WidgetIdentifier.TypeIdentifier` - A string property that specifies the identifier of the widget.
- `MigrateWidget`- Migrate the widget data

You can see a sample in ...

## Customize widget property migrations

In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new class that implements the `IWidgetPropertiesMigration` interface. Implement the following properties and methods required by the interface:

You can see a sample in ...

## Register migrations

Register the migration in `Migration.Tool.Extensions/ServiceCollectionExtensions.cs` as a `Transient` dependency into the service collection:

- Field migrations - `services.AddTransient<IFieldMigration, MyFieldMigration>()`
- Widget property migrations - `services.AddTransient<IWidgetPropertyMigration, MyWidgetPropertyMigration>();`