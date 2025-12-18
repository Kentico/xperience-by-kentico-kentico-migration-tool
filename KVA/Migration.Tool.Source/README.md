# Migration.Tool.Source (KVA/)

> **Contributor-Only:** This is an internal abstraction layer. Users do not interact with this project directly. Only modify this if you are contributing to the migration tool itself.

## Purpose

Provides version-agnostic abstractions and shared interfaces used by all version-specific Core projects (K11/KX12/KX13). This acts as a bridge between the core infrastructure (Common) and version-specific implementations.

Located in the `KVA/` solution folder to indicate it's part of the Kentico Version Abstraction layer.

## Key Components

- **Source Entity Abstractions** - Interfaces defining common properties across K11/KX12/KX13 entities
- **Repository Patterns** - Shared patterns for querying source databases
- **Source API Contracts** - Common interfaces for source instance API calls (e.g., widget metadata discovery)
- **Mapping Helpers** - Utility functions used across all version-specific implementations
- **Source Configuration** - Source instance connection and configuration models

## Architecture Role

```
Migration.Tool.Common
        ↓
Migration.Tool.Source  ← Abstraction layer (this project)
        ↓
Core.K11 / Core.KX12 / Core.KX13  ← Version-specific implementations
        ↓
K11 / KX12 / KX13  ← Source database models
```

This allows Core projects to share common logic while implementing version-specific behavior where source CMS versions differ.
