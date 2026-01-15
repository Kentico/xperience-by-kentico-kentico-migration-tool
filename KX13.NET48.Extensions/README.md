# KX13.NET48.Extensions

> **Minimal User Interaction:** This project provides API controllers for .NET Framework 4.8 KX13 instances. Most users only need to copy the controller file to their source instance - no modification required.

## Purpose

Required when migrating **Page Builder widgets** from KX13 instances running on **.NET Framework 4.8** (not ASP.NET Core). Provides ASP.NET MVC API controllers that expose widget metadata (available properties, data types, default values) from your source instance. If your KX13 instance runs on ASP.NET Core, use the `KX13.Extensions` project instead.

## Setup

For complete setup instructions, see [Source instance API discovery](../Migration.Tool.CLI/README.md#api-discovery-setup) in the Migration.Tool.CLI README.

**Quick summary:**
1. Copy `ToolApiController.NET48.cs` to your KX13 source instance's `Controllers` folder
2. Register the controller route in `/App_Start/RouteConfig.cs`
3. Set a secret value in the controller
4. Configure the migration tool's `appsettings.json` with the matching secret

## Technical Details

- ASP.NET MVC (not Core) - for .NET Framework 4.8
- Uses `System.Web.Http` instead of `Microsoft.AspNetCore.Mvc`
- Returns JSON metadata consumed by the migration tool
- Requires matching secret between controller and migration tool configuration

Most users will not need to modify this code. Use this project only if your KX13 source instance runs on .NET Framework 4.8 instead of ASP.NET Core.
