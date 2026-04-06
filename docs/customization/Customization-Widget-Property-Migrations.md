# Customization: Widget Property Migrations

> **Audience:** Developers implementing widget-property transformations using `IWidgetPropertyMigration`.

## Purpose

Widget property migrations customize transformations of individual widget properties.

## Extension Point

Implement `IWidgetPropertyMigration` in `Migration.Tool.Extensions/CommunityMigrations`.

Required members:

- `Rank`
- `ShallMigrate(WidgetPropertyMigrationContext context, string propertyName)`
- `MigrateWidgetProperty(string key, JToken value, WidgetPropertyMigrationContext context)`

Useful context information:

- `WidgetPropertyMigrationContext.SiteId`
- `WidgetPropertyMigrationContext.EditingFormControlModel`
- `propertyName`

## Samples

- [Path selector migration](../../Migration.Tool.Extensions/DefaultMigrations/WidgetPathSelectorMigration.cs)
- [Page selector migration](../../Migration.Tool.Extensions/DefaultMigrations/WidgetPageSelectorMigration.cs)
- [File selector migration](../../Migration.Tool.Extensions/DefaultMigrations/WidgetFileMigration.cs)

Kentico guide: [Transform widget properties](https://docs.kentico.com/x/transform_widget_properties_guides)

## Registration

Register in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddTransient<IWidgetPropertyMigration, MyWidgetPropertyMigration>();
```

See also: [Data Transformation Extensions](Customization-Data-Transformation-Extensions.md#registration)
