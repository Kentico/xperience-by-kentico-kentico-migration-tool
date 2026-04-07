# Customization: Field Migrations

> **Audience:** Developers implementing field-level transformations using `IFieldMigration`.

## Purpose

Field migrations customize source field definitions and values during migration when built-in behavior is not sufficient.

**Use cases:**

- Handle custom form controls
- Convert data formats (date formats, URL structures)
- Transform content (HTML cleanup, path updates)

## Create Custom Field Migrations

You can customize field migrations to control how fields are mapped for page types, modules, system objects, and forms. In the `Migration.Tool.Extensions/CommunityMigrations` folder, create a new file with a class that implements the `IFieldMigration` interface. Implement the following properties and methods required by the interface:

- `Rank` - An integer property that determines the order in which migrations are applied. Use a value lower than _100000_, as that is the value for system migrations.
- `ShallMigrate` - A boolean method that specifies whether the migration shall be applied for the current field. Use properties of the `FieldMigrationContext` object passed as an argument to the method to evaluate the condition. For each field, the migration with the lowest rank that returns `true` from the `ShallMigrate` method is used.
  - `SourceDataType` - A string property that specifies the [data type](https://docs.kentico.com/x/coJwCg) of the source field.
  - `SourceFormControl` - A string property that specifies the [form control](https://docs.kentico.com/x/lAyRBg) used by the source field.
  - `FieldName` - A string property that specifies the code name of the field.
  - `SourceObjectContext` - An interface that can be used to check the context of the source object (e.g. page vs. form).
- `MigrateFieldDefinition` - Migrate the definition of the field. Use the `XElement` attribute to transform the field.
- `MigrateValue` - Migrate the value of the field. Use the source value and the `FieldMigrationContext` with the same parameters as described in `ShallMigrate`.

You can see samples:

- [SampleTextMigration.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleTextMigration.cs)
  - A sample migration with further explanations
- [AssetMigration.cs](../../Migration.Tool.Extensions/DefaultMigrations/AssetMigration.cs)
  - An example of a usable migration

After implementing the migration, you need to [register the migration](#registration) in the system.

## Registration

Register field migrations in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs) as `IFieldMigration` implementations.

Example registration:

```csharp
services.AddTransient<IFieldMigration, MyFieldMigration>();
```

> [!WARNING]
> After adding or updating field migrations, rebuild the migration tool solution before running migration.

For general registration guidance across all customization types, see [Data Transformation Extensions](Data-Transformation-Extensions.md#registration).
