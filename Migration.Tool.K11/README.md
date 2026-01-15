# Migration.Tool.K11

> **Contributor-Only:** This is an internal database model project. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Contains Entity Framework Core models and DbContext for the **Kentico 11 source database**. This project provides read-only access to K11 source data.

## Key Components

- **Entity Models** - C# classes representing K11 database tables
- **DbContext** - `K11Context` for querying the source database
- **Relationships** - Entity Framework navigation properties

## Usage

Referenced by **Migration.Tool.Core.K11** handlers to query source data. This is a pure data access layer with no business logic.
