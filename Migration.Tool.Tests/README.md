# Migration.Tool.Tests

> **Contributor-Only:** This is the test project. Users do not interact with this project. Only modify this if you are contributing to the migration tool itself.

## Purpose

Contains unit tests and integration tests for the migration tool. Ensures migration logic works correctly across all supported source versions (K11/KX12/KX13).

## Test Structure

- **Unit Tests** - Test individual mappers, handlers, and helpers in isolation
- **Integration Tests** - Test end-to-end migration scenarios with test databases
- **Test Fixtures** - Shared test data and mock objects

## Running Tests

```bash
dotnet test Migration.Tool.Tests
```

## Key Areas Tested

- Field migration logic and custom field transformations
- Widget migration and property mapping
- Entity mapping (sites, pages, users, forms, etc.)
- Primary key mapping and relationship preservation
- Error handling and validation
- Configuration parsing and validation

## Testing Philosophy

See [Architecture Deep Dive - Testing Philosophy](../docs/Architecture-Deep-Dive.md#testing-philosophy) for the migration tool's approach to testing, including:
- When to write tests vs. manual verification
- Testing custom migrations
- Integration test patterns
