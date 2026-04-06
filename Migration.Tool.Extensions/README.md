# Migration.Tool.Extensions

> **User Interaction:** This project is where you implement custom migration logic. Modify this project to add field migrations, widget migrations, and custom class mappings that transform your data during migration.

## Purpose

**Migration.Tool.Extensions** is the primary project for targeted, code-driven customization of the Kentico Migration Tool. Use this project to implement custom logic that transforms your data during migration from Kentico Xperience 13, Kentico 12, or Kentico 11 to Xperience by Kentico.

The project enables you to:

- Customize migration behavior with custom transformations
- Handle project-specific data structures and field mappings
- Transform widget properties and Page Builder content
- Remodel content types and merge page types
- Control how linked pages and child relationships are migrated

## Extension Points

- `IClassMapping` – remodel content type structure and field definitions
- `ContentItemDirectorBase` – control per-item migration decisions and relationships
- `IFieldMigration` – transform field definitions and values
- `IWidgetMigration` – transform widget types/structures
- `IWidgetPropertyMigration` – transform widget property values

## Detailed Documentation

See [Data Transformation Extensions](../docs/customization/Customization-Data-Transformation-Extensions.md) for detailed explanation on how to define custom [Class Mappings](../docs/customization/Customization-Class-Mappings.md), [Content Item Directors](../docs/customization/Customization-Content-Item-Directors.md), [Field Migrations](../docs/customization/Customization-Field-Migrations.md), [Widget Migrations](../docs/customization/Customization-Widget-Migrations.md) and [Widget Property Migrations](../docs/customization/Customization-Widget-Property-Migrations.md).

For an overview of available Kentico Migration Tool customization options see the [Customization Guide](../docs/customization/Customization-Guide.md).
