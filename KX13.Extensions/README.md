# KX13.Extensions

> **Minimal User Interaction:** This project provides API controllers that you deploy to your KX13 source instance for widget metadata discovery. Most users only need to copy this project to their source instance - no modification required.

## Purpose

Required when migrating **Page Builder widgets** from KX13. Provides ASP.NET Core API controllers that expose widget metadata (available properties, data types, default values) from your source instance. The migration tool calls these APIs to retrieve Page Builder widget configurations that are not stored in the database.

## Setup

1. **Copy** this project to your KX13 source web application
2. **Reference** it in your KX13 web project's .csproj
3. **Run** your KX13 application (e.g., `https://localhost:5001`)
4. **Configure** the migration tool's `appsettings.json` with the API endpoint URL:

```json
"OptInFeatures": {
  "QuerySourceInstanceApi": {
    "Enabled": true,
    "Connections": [
      {
        "SourceInstanceUri": "https://localhost:5001",
        "Secret": "your-secret-key"
      }
    ]
  }
}
```

See [Migration.Tool.CLI/appsettings.json](../Migration.Tool.CLI/appsettings.json) for the full configuration reference.

## API Endpoints

- `/api/migration-tool/widgets` - Returns all registered Page Builder widgets
- `/api/migration-tool/widget-properties/{identifier}` - Returns properties for a specific widget

## Technical Details

- ASP.NET Core 3.1+ compatible
- Reads widget registrations from the Kentico API
- Returns JSON metadata consumed by the migration tool
- No authentication required (intended for local/internal use during migration)

Most users will not need to modify this code. Contributors may extend it to expose additional widget metadata or handle custom widget registration patterns.
