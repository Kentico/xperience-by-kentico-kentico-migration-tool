# Migration.Tool.KXP.Api

> **Contributor-Only:** This is an internal API wrapper project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Provides typed API wrappers and helpers for interacting with the Xperience by Kentico target instance:

- **Target Database Context** - Entity Framework Core DbContext for XbyK database
- **API Wrappers** - Strongly-typed interfaces for XbyK Info/Provider APIs
- **UMT Integration** - Universal Migration Toolkit adapter implementations
- **Data Validation** - Target-side validation rules and constraints
- **Bulk Operations** - Optimized bulk insert/update helpers for large datasets

## Key Components

- Target database models and DbContext for Xperience by Kentico
- Service interfaces for XbyK Managers and InfoProviders
- UMT (Universal Migration Toolkit) integration services
- Validation logic for target entity constraints
- Helper methods for common XbyK API operations

## Dependencies

- References the Xperience by Kentico assemblies
- Used by all version-specific Core projects (Core.K11, Core.KX12, Core.KX13)
- Used by Migration.Tool.Common for target instance operations

This project encapsulates all target-instance-specific logic, keeping version-specific Core projects focused on source data transformation.
