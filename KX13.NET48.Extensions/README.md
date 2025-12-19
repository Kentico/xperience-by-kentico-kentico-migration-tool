# KX13.NET48.Extensions

> **Minimal User Interaction:** This project provides API controllers for .NET Framework 4.8 KX13 instances. Most users only need to copy this project to their source instance - no modification required.

## Purpose

Provides ASP.NET MVC API controllers that expose widget metadata from your Kentico Xperience 13 source instance running on **.NET Framework 4.8**. This is the .NET Framework equivalent of the `KX13.Extensions` project.

## When to Use

Required when migrating **Page Builder widgets** from KX13 **on .NET Framework 4.8** (not ASP.NET Core). If your KX13 instance runs on ASP.NET Core, use the `KX13.Extensions` project instead.

## Setup

1. **Copy** this project to your KX13 .NET Framework web application
2. **Reference** it in your KX13 web project's .csproj
3. **Restore NuGet packages** (uses older Kentico packages compatible with .NET 4.8)
4. **Run** your KX13 application
5. Configure the migration tool's appsettings.json with the API endpoint URL

## API Endpoints

Same as KX13.Extensions:
- `/api/migration-tool/widgets` - Returns all registered Page Builder widgets
- `/api/migration-tool/widget-properties/{identifier}` - Returns properties for a specific widget

## Technical Details

- ASP.NET MVC (not Core) - for .NET Framework 4.8
- Uses `System.Web.Http` instead of `Microsoft.AspNetCore.Mvc`
- Compatible with KX13 Refresh 1+ on .NET Framework
- Returns JSON metadata consumed by the migration tool

Most users will not need to modify this code. Use this project only if your KX13 source instance runs on .NET Framework 4.8 instead of ASP.NET Core.
