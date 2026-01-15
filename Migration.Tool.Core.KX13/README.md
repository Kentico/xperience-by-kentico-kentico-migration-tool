# Migration.Tool.Core.KX13

> **Contributor-Only:** This is an internal handler implementation project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Implements Kentico Xperience 13-specific migration handlers, mappers, and behaviors. This project contains all the logic for transforming KX13 source data into Xperience by Kentico format.

## Key Components

- **Handlers/** - MediatR command handlers for each data type (sites, pages, users, forms, etc.)
  - Each handler orchestrates: query source → transform → import to target
  - Example: `MigratePagesCommandHandler` migrates CMS_Document + CMS_Tree to ContentItemSimplifiedDataModel
  
- **Mappers/** - Entity transformation logic using the "MapInternal" pattern
  - Converts KX13 entities to XbyK models
  - Applies field migrations and widget migrations
  - Handles content item variations, workflow, and versioning
  
- **Behaviors/** - MediatR pipeline behaviors (logging, error handling, validation)

- **Helpers/** - KX13-specific utilities for:
  - Page type conversion to reusable content types
  - Widget configuration transformation
  - Attachment and media file handling
  - Custom table mapping

## Dependencies

- **Migration.Tool.KX13** - Source database models (Entity Framework Core)
- **Migration.Tool.Source** - Abstraction layer interfaces
- **Migration.Tool.Common** - Command pipeline and configuration
- **Migration.Tool.KXP.Api** - Target instance API

## Version Specifics

This project handles KX13-specific features like:
- Page Builder widget migration (Xperience 13 → XbyK)
- Form Builder form migration
- MVC route configuration migration
- Content item variations (language/culture handling)
