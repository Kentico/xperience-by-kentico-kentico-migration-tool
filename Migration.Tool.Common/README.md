# Migration.Tool.Common

> **Contributor-Only:** This is an internal infrastructure project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Provides shared infrastructure used by all other migration projects:

- **Configuration Management** - `ToolConfiguration.cs` defines all appsettings.json configuration options with validation
- **Command Pipeline** - MediatR command handlers and behaviors for orchestrating migrations
- **Migration Protocol** - Tracks migration progress, errors, and warnings in structured format
- **Logging Infrastructure** - Centralized logging with progress reporting
- **Extension Points** - Abstract interfaces for field migrations, widget migrations, and custom mappers
- **Primary Key Mapping** - Tracks relationships between source and target entity IDs across migration runs

## Key Components

- `Commands.cs` - All MediatR commands (MigrateSitesCommand, MigratePagesCommand, etc.)
- `ToolConfiguration.cs` - Configuration model with validation
- `IPrinter.cs` / `IPrimaryKeyMappingContext.cs` - Core service abstractions
- `MigrationProtocol/` - Structured migration result reporting
- `Abstractions/` - Interfaces for custom migrations and mappers
- `Services/` - Command line interface parsing and execution logic

This project has no direct dependencies on source or target database models, making it version-agnostic.
