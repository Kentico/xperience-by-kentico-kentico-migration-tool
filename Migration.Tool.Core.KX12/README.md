# Migration.Tool.Core.KX12

> **Contributor-Only:** This is an internal handler implementation project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Implements Kentico 12-specific migration handlers, mappers, and behaviors. This project contains all the logic for transforming KX12 source data into Xperience by Kentico format.

## Key Components

- **Handlers/** - MediatR command handlers for each data type (sites, pages, users, forms, etc.)
- **Mappers/** - Entity transformation logic using the "MapInternal" pattern
- **Behaviors/** - MediatR pipeline behaviors (logging, error handling, validation)
- **Helpers/** - KX12-specific utilities for data transformation

## Dependencies

- **Migration.Tool.KX12** - Source database models (Entity Framework Core)
- **Migration.Tool.Source** - Abstraction layer interfaces
- **Migration.Tool.Common** - Command pipeline and configuration
- **Migration.Tool.KXP.Api** - Target instance API

## Version Specifics

KX12 differs from KX13 primarily in:
- No Page Builder widgets (uses Portal Engine or MVC patterns)
- Different workflow and versioning model
- Simpler page type structure (no variations)
