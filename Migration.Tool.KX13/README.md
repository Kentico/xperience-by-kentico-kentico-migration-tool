# Migration.Tool.KX13

> **Contributor-Only:** This is an internal database model project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Contains Entity Framework Core models and DbContext for the **Kentico Xperience 13 source database**. This project provides read-only access to KX13 source data.

## Key Components

- **Entity Models** - C# classes representing KX13 database tables
  - `CMS_Site`, `CMS_Document`, `CMS_Tree`, `CMS_User`, `CMS_Form`, etc.
  - Generated from KX13 database schema
  
- **DbContext** - `KX13Context` for querying the source database
  - Configured for read-only operations
  - Uses connection string from appsettings.json
  
- **Relationships** - Entity Framework navigation properties and foreign keys

## Usage

Referenced by **Migration.Tool.Core.KX13** handlers to query source data:

```csharp
// Example: Query KX13 documents
var documents = kx13Context.CmsSites
    .Include(s => s.CmsDocuments)
    .Where(s => s.SiteId == siteId)
    .ToList();
```

This project is a pure data access layer with no business logic. All transformation logic lives in Core.KX13.
