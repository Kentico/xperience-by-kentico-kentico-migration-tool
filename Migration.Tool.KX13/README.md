# Migration.Tool.KX13

> **Contributor-Only:** This is an internal database model project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Contains Entity Framework Core models and DbContext for the **Kentico Xperience 13 source database**. This project provides read-only access to KX13 source data.

## Key Components

- **Entity Models** - C# classes representing KX13 database tables
- **DbContext** - `KX13Context` for querying the source database
- **Relationships** - Entity Framework navigation properties and foreign keys

## Usage

Referenced by **Migration.Tool.Core.KX13** handlers to query source data. This is a pure data access layer with no business logic.
