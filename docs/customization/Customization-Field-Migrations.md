# Customization: Field Migrations

> **Audience:** Developers implementing field-level transformations using `IFieldMigration`.

## Purpose

Field migrations customize source field definitions and values during migration when built-in behavior is not sufficient.

## Extension Point

Implement `IFieldMigration` in `Migration.Tool.Extensions/CommunityMigrations`.

Required members:

- `Rank` – lower rank wins when multiple migrations match. Use a value below `100000` to override system migrations.
- `ShallMigrate(...)` – determines applicability.
- `MigrateFieldDefinition(...)` – transforms field definition (`XElement`).
- `MigrateValue(...)` – transforms field value.

Useful `FieldMigrationContext` properties:

- `SourceDataType`
- `SourceFormControl`
- `FieldName`
- `SourceObjectContext` (object context such as page/form)

## Samples

- [Migration.Tool.Extensions/CommunityMigrations/SampleTextMigration.cs](../../Migration.Tool.Extensions/CommunityMigrations/SampleTextMigration.cs)
- [Migration.Tool.Extensions/DefaultMigrations/AssetMigration.cs](../../Migration.Tool.Extensions/DefaultMigrations/AssetMigration.cs)

## Registration

Register in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddTransient<IFieldMigration, MyFieldMigration>();
```

See also: [Data Transformation Extensions](Customization-Data-Transformation-Extensions.md#registration)
