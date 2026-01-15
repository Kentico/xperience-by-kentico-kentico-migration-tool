# Migration.Tool.KXP.Api

> **Contributor-Only:** This is an internal API wrapper project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Provides API wrappers and services for interacting with the Xperience by Kentico target instance using the native Kentico Info/Provider pattern:

- **KxpClassFacade** - CRUD operations for DataClassInfo (content types, forms, custom classes)
- **KxpMediaFileFacade** - Media library and media file management operations
- **Migration Services** - Field and widget migration customization services
- **API Initialization** - Ensures Kentico API is properly initialized before operations
- **Auxiliary Helpers** - Form components, user helpers, and supporting utilities

## Key Components

- **KxpClassFacade** - Wrapper for DataClassInfoProvider operations (SetClass, GetClass, customized field enumeration)
- **KxpMediaFileFacade** - Wrapper for MediaFileInfoProvider and MediaLibraryInfoProvider operations
- **FieldMigrationService** - Service for registering and executing custom field migrations
- **WidgetMigrationService** - Service for registering and executing custom widget migrations
- **KxpApiInitializer** - Initializes Xperience by Kentico API environment
- **Auxiliary** - DummyFormFile, FormComponents, UserHelper for supporting operations

## Dependencies

- References the Xperience by Kentico assemblies
- Used by all version-specific Core projects (Core.K11, Core.KX12, Core.KX13)
- Used by Migration.Tool.Common for target instance operations

This project encapsulates all target-instance-specific logic, keeping version-specific Core projects focused on source data transformation.
