# KX13.Extensions

> **Minimal User Interaction:** This project provides API controllers that you deploy to your KX13 source instance for widget metadata discovery. Most users only need to copy the controller file to their source instance - no modification required.

## Purpose

Required when migrating **Page Builder widgets** from KX13. Provides ASP.NET Core API controllers that expose widget metadata (available properties, data types, default values) from your source instance. The migration tool calls these APIs to retrieve Page Builder widget configurations that are not stored in the database.

## Setup

For complete setup instructions, see [Source instance API discovery](../Migration.Tool.CLI/README.md#api-discovery-setup) in the Migration.Tool.CLI README.

**Quick summary:**
1. Copy `ToolApiController.cs` to your KX13 source instance's `Controllers` folder
2. Register the controller route in `Startup.cs` or `Program.cs`
3. Set a secret value in the controller
4. Configure the migration tool's `appsettings.json` with the matching secret

## Technical Details

- ASP.NET Core 3.1+ compatible (use `ToolApiController.NET48.cs` for .NET Framework 4.8 projects)
- Reads widget registrations from the Kentico API
- Returns JSON metadata consumed by the migration tool
- Requires matching secret between controller and migration tool configuration

Most users will not need to modify this code. Contributors may extend it to expose additional widget metadata or handle custom widget registration patterns.
