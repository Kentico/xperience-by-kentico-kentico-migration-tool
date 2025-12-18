# Migration Tool Architecture

> **Audience:** Developers who need to understand how the Migration Tool works under the hood and how the repository is organized.

The repository contains multiple projects that work together to perform migrations.

## How It Works

The Migration Tool uses a **handler-based architecture**:

1. **Load Source Data** - Retrieves from source database (K11/KX12/KX13), source filesystem (media/attachments), and source API (widget info for KX13)
2. **Transform** - Applies built-in mappers and custom migrations
3. **Write to Target** - Saves using Universal Migration Toolkit (UMT), XbyK API (Managers/InfoProviders), or Bulk SQL copy

### Key Concepts

**Handlers:** Each data type (sites, users, pages, etc.) has a dedicated handler that orchestrates the migration.

**Migrations:** Customize how fields and widgets transform between source and target:
- **Field Migrations** - Transform content type field data
- **Widget Property Migrations** - Transform Page Builder widget properties


## Repository Structure

### Core Migration Engine

| Project | Purpose |
|---------|---------|
| **Migration.Tool.CLI** | Command-line interface - the executable you run to perform migrations |
| **Migration.Tool.Common** | Shared infrastructure (configuration, logging, commands, protocols) |
| **Migration.Tool.KXP.Api** | Target instance API - interacts with Xperience by Kentico database and APIs |

## Version-Specific Migration Logic

Each source version has dedicated projects for migration logic:

| Project | Purpose |
|---------|---------|
| **Migration.Tool.Core.K11** | Kentico 11-specific migration handlers and mappers |
| **Migration.Tool.Core.KX12** | Kentico 12-specific migration handlers and mappers |
| **Migration.Tool.Core.KX13** | Kentico Xperience 13-specific migration handlers and mappers |
| **Migration.Tool.K11** | Kentico 11 source database models and context |
| **Migration.Tool.KX12** | Kentico 12 source database models and context |
| **Migration.Tool.KX13** | Kentico Xperience 13 source database models and context |

### Customization and Extensions

| Project | Purpose |
|---------|---------|
| **Migration.Tool.Extensions** | Custom migrations (field migrations, widget migrations, class mappings) |
| **Migration.Tool.KXP.Extensions** | Sample target instance customizations (optional) |
| **KX13.Extensions** | ASP.NET Core controller for widget API discovery on KX13 source instances |
| **KX13.NET48.Extensions** | .NET Framework 4.8 controller for widget API discovery on KX13 source instances |

## Project Dependencies

![Project Dependencies Diagram](../images/diagrams/project-dependencies.png)

The project dependencies flow primarily from left to right with some vertical connections:

1. **Migration.Tool.CLI** - Entry point that reads appsettings.json and executes commands via MediatR
2. **Migration.Tool.Common** - Shared infrastructure providing command pipeline, configuration validation, migration protocol, and logging
3. **Source Version Core** - Version-specific (K11/KX12/KX13) handlers, mappers, and behaviors
4. **Source DB Models** - EF Core DbContext and entity models for the source database
5. **Migration.Tool.Extensions** - Custom migrations connected via dashed line from Source DB Models (optional)
6. **Migration.Tool.KXP.Api** - Target instance API positioned below the main flow, receiving connections from both Common and Core layers for target database context, XbyK API wrappers, and data validation

The horizontal flow represents the primary dependency chain (CLI → Common → Core → DB Models → Extensions), while KXP.Api sits below as a shared target service accessed by both Common and Core components.

**Key Insight:** Direct interaction is only with **Migration.Tool.CLI** (run migrations) and **Migration.Tool.Extensions** (add customizations). Other projects provide version-specific migration logic that runs automatically based on source instance version.

## Project Dependencies Explained

### CLI → Common → Core
The CLI project depends on Common for shared infrastructure, which depends on the appropriate Core project (K11/KX12/KX13) based on source version.

### Core → Source DB Models
Each Core project (e.g., Migration.Tool.Core.KX13) depends on its corresponding source database model project (e.g., Migration.Tool.KX13) for database access.

### Core → KXP.Api
All Core projects depend on Migration.Tool.KXP.Api to interact with the target Xperience by Kentico instance.

### Extensions (Optional)
The Migration.Tool.Extensions project contains custom migration logic. It compiles as part of the solution and is automatically discovered at runtime. Consult the [Extensions Guide](../Migration.Tool.Extensions/README.md) for detailed instructions on creating field migrations, widget migrations, and custom class mappings.

## Project Modification Guide

| Scenario | Projects to Modify |
|----------|-------------------|
| **Running migrations** | None - configure appsettings.json only |
| **Custom field transformation** | `Migration.Tool.Extensions` |
| **Custom widget migration** | `Migration.Tool.Extensions` |
| **Custom table mapping** | `Migration.Tool.Extensions` |
| **Contributing bug fixes** | Relevant `Core.KX##` project |
| **Adding new data type support** | `Core.KX##` + `Common` + `CLI` |

## Related Documentation

- **[Architecture Deep Dive](Architecture-Deep-Dive.md)** - Detailed internal architecture, handler patterns, testing philosophy
- **[Extensions README](../Migration.Tool.Extensions/README.md)** - How to create custom migrations
- **[Contributing Setup](Contributing-Setup.md)** - For contributors to the tool