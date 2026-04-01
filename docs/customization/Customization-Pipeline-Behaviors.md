# Command Pipeline Architecture Guide

> **Audience:** Developers implementing advanced command-pipeline scenarios that span command boundaries (for example, logic that must run after `--sites` or after `--pages`).

This guide explains command-pipeline architecture and customization using MediatR pipeline behaviors (`IPipelineBehavior<TRequest, TResponse>`).

`IPipelineBehavior<TRequest, TResponse>` is a MediatR abstraction (not specific to this repository). This page documents how the migration tool uses that abstraction and where project-specific behavior implementations are added.

Use this document as the canonical architecture and recipes reference for command-pipeline customizations.

The command pipeline architecture is based on the [Mediator pattern](https://www.geeksforgeeks.org/system-design/mediator-design-pattern/).

## What It Is

The migration tool executes commands (for example `MigrateSitesCommand`, `MigratePagesCommand`) through MediatR.
Each command passes through a chain of pipeline behaviors before the command handler runs.

In this repository, migration actions are modeled as MediatR commands implementing `IRequest<CommandResult>` (see [Migration.Tool.Common/Commands.cs](../../Migration.Tool.Common/Commands.cs)). Commands are dispatched via `IMediator.Send(...)` in the CLI startup flow (see [Migration.Tool.CLI/Program.cs](../../Migration.Tool.CLI/Program.cs)).

Conceptually:

`Request -> Behavior 1 -> Behavior 2 -> ... -> Handler -> Response`

Concrete built-in sequence (registration order):

`Request -> RequestHandlingBehavior -> CommandConstraintBehavior -> XbyKApiContextBehavior/XbKApiContextBehavior -> Handler -> Response`

This enables cross-cutting customization such as pre-checks, post-processing, command-flow control, and telemetry.

## Built-in Behaviors

The tool registers three standard behaviors:

- `RequestHandlingBehavior` - cross-cutting logging and protocol tracking around command execution. It starts a stopwatch, logs start/end timing, and reports command request/finish/error events to `IMigrationProtocol`.
- `CommandConstraintBehavior` - pre-flight validation gate. It performs critical source checks (for example supported source-version keys in KX12/KX13 and source-site validity) and short-circuits with `CommandCheckFailedResult` if checks fail.
- `XbyKApiContextBehavior` / `XbKApiContextBehavior` - target-environment bootstrap. It ensures target API initialization, verifies default admin existence, and executes downstream steps in a `CMSActionContext` under that admin.

Reference registrations:

- Shared source abstraction layer: [KVA/Migration.Tool.Source/KsCoreDiExtensions.cs](../../KVA/Migration.Tool.Source/KsCoreDiExtensions.cs)
- Version-specific cores:
  - [Migration.Tool.Core.K11/K11CoreDiExtensions.cs](../../Migration.Tool.Core.K11/K11CoreDiExtensions.cs)
  - [Migration.Tool.Core.KX12/KX12CoreDiExtensions.cs](../../Migration.Tool.Core.KX12/KX12CoreDiExtensions.cs)
  - [Migration.Tool.Core.KX13/DependencyInjectionExtensions.cs](../../Migration.Tool.Core.KX13/DependencyInjectionExtensions.cs)

Behavior implementation references:

- Shared source: [KVA/Migration.Tool.Source/Behaviors/RequestHandlingBehavior.cs](../../KVA/Migration.Tool.Source/Behaviors/RequestHandlingBehavior.cs)
- Shared source: [KVA/Migration.Tool.Source/Behaviors/CommandConstraintBehavior.cs](../../KVA/Migration.Tool.Source/Behaviors/CommandConstraintBehavior.cs)
- Shared source: [KVA/Migration.Tool.Source/Behaviors/XbyKApiContextBehavior.cs](../../KVA/Migration.Tool.Source/Behaviors/XbyKApiContextBehavior.cs)
- KX13: [Migration.Tool.Core.KX13/Behaviors/RequestHandlingBehavior.cs](../../Migration.Tool.Core.KX13/Behaviors/RequestHandlingBehavior.cs)
- KX13: [Migration.Tool.Core.KX13/Behaviors/CommandConstraintBehavior.cs](../../Migration.Tool.Core.KX13/Behaviors/CommandConstraintBehavior.cs)
- KX13: [Migration.Tool.Core.KX13/Behaviors/XbKApiContextBehavior.cs](../../Migration.Tool.Core.KX13/Behaviors/XbKApiContextBehavior.cs)

## When to Use Pipeline Behaviors

Prefer documented extension points first:

- `IFieldMigration` for single-field value transformation
- `IWidgetMigration` / `IWidgetPropertyMigration` for widget-level changes
- `ContentItemDirectorBase` for page/content-item migration decisions
- `IClassMapping` for content type/model remapping

Use `IPipelineBehavior` when you need command-flow customization around execution itself, for example:

- run custom logic immediately before/after `MigrateSitesCommand` or `MigratePagesCommand`
- coordinate multi-step flows that depend on command order
- perform command-level validation and fail early
- emit external audit/telemetry for command runs

## Decision Framework

Use this preference order when choosing customization approach:

1. **Default preference** - Existing extension points:
   - `IFieldMigration`
   - `IWidgetMigration` / `IWidgetPropertyMigration`
   - `ContentItemDirectorBase`
   - `IClassMapping`
2. **Escalation path** - `IPipelineBehavior` for command-stage flows that span multiple handlers.
3. **Rare / avoid** - Custom CLI commands, unless you are introducing a truly new top-level migration workflow.

This keeps the migration flow predictable while still allowing advanced command-pipeline customization when needed.

## Execution Anchor Model (Command Stages)

Anchor each behavior to the command stage that guarantees the data context you need.

| Anchor command               | Typical purpose                                                                     | Why this anchor                                     |
| ---------------------------- | ----------------------------------------------------------------------------------- | --------------------------------------------------- |
| `MigrateSitesCommand`        | Prepare shared prerequisites (languages, site-level metadata, taxonomy scaffolding) | Site and language context is available early        |
| `MigratePageTypesCommand`    | Content type model changes that should happen before page migration                 | Content type shape is established here              |
| `MigratePagesCommand`        | Post-page transformations, relationship backfills, schema attachment cleanup        | Page/content items and mappings are already created |
| `MigrateCustomTablesCommand` | Table-driven transformations when table import stage matters                        | Anchors logic to custom table data stage            |

Command ranks and dependencies are defined in [Migration.Tool.Common/Commands.cs](../../Migration.Tool.Common/Commands.cs), and command flags are parsed in [Migration.Tool.Common/Services/CommandParser.cs](../../Migration.Tool.Common/Services/CommandParser.cs).

When planning behavior placement, always verify command dependencies first.

## Why It Works Well for Advanced Scenarios

Pipeline behaviors can combine:

- **Command timing control** - anchor logic to a specific command stage
- **Source data access** - read source objects through `ModelFacade` without forcing default migration paths
- **Target API access** - use standard Xperience APIs in the same execution flow

This is useful for scenarios such as custom-table-to-taxonomy transformations, post-page schema updates, or staged remodel operations.

### ModelFacade

`ModelFacade` ([KVA/Migration.Tool.Source/ModelFacade.cs](../../KVA/Migration.Tool.Source/ModelFacade.cs)) provides read access to source instance data (K11/KX12/KX13) without triggering the default migration path. Inject it via DI:

```csharp
public class MyBehavior(ModelFacade modelFacade)
    : IPipelineBehavior<MigrateSitesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        MigrateSitesCommand request,
        RequestHandlerDelegate<CommandResult> next,
        CancellationToken cancellationToken)
    {
        // Read all records of a typed source model
        var sourceClasses = modelFacade.SelectAll<ICmsClass>().ToList();

        // Read rows from an arbitrary table as dictionaries (useful for custom tables)
        var rows = modelFacade.SelectAllAsDictionary("CustomTable_MyTags").ToList();

        return await next();
    }
}
```

### Target API — content type and field manipulation

Standard Xperience API is available within behaviors. Commonly used types for post-migration content model changes:

```csharp
// Attach a reusable field schema to a content type
var dataClass = DataClassInfoProvider.ProviderObject.Get("MyProject.MyPageType");
reusableSchemaService.AddReusableSchemaToDataClass(dataClass, "MySchemaName");
DataClassInfoProvider.SetDataClassInfo(dataClass);

// Remove a field from a content type
var formInfo = new FormInfo(dataClass.ClassFormDefinition);
var field = formInfo.GetFormField("OldFieldName");
if (field is not null)
{
    formInfo.RemoveFormField(field.Name);
}
dataClass.ClassFormDefinition = formInfo.GetXmlDefinition();
DataClassInfoProvider.SetDataClassInfo(dataClass);

// Add a new field to a content type
var newField = new FormFieldInfo
{
    Name = "MyNewField",
    DataType = FieldDataType.Text,
    AllowEmpty = true,
    Visible = true,
    Caption = "My New Field",
    Guid = Guid.NewGuid(),
};
formInfo.AddFormItem(newField);
dataClass.ClassFormDefinition = formInfo.GetXmlDefinition();
DataClassInfoProvider.SetDataClassInfo(dataClass);
```

`ReusableSchemaService` is registered in DI and can be injected directly into your behavior. See [KVA/Migration.Tool.Source/Services/ReusableSchemaService.cs](../../KVA/Migration.Tool.Source/Services/ReusableSchemaService.cs).

## Implementation Pattern

1. Create a behavior class implementing `IPipelineBehavior<TRequest, TResponse>`.
2. Add custom pre-processing.
3. Call `await next()` to continue (or short-circuit when required).
4. Add custom post-processing.
5. Register the behavior in DI.

### Command-specific behavior (recommended for most customizations)

Constrain the behavior to a single command by using a closed generic. This is the most common pattern for migration customizations — you know exactly which command stage your logic depends on:

```csharp
public class MyPostPagesBehavior(ILogger<MyPostPagesBehavior> logger)
    : IPipelineBehavior<MigratePagesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        MigratePagesCommand request,
        RequestHandlerDelegate<CommandResult> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Before pages migration");

        var response = await next();

        // Post-processing runs only after MigratePagesCommand completes
        logger.LogInformation("After pages migration");
        return response;
    }
}
```

Register with the closed generic:

```csharp
services.AddTransient(
    typeof(IPipelineBehavior<MigratePagesCommand, CommandResult>),
    typeof(MyPostPagesBehavior));
```

### Cross-cutting behavior (all commands)

To intercept every command, use the open generic form:

```csharp
public class MyGlobalBehavior<TRequest, TResponse>(
    ILogger<MyGlobalBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Before {Command}", typeof(TRequest).Name);
        var response = await next();
        logger.LogInformation("After {Command}", typeof(TRequest).Name);
        return response;
    }
}
```

Register with the open generic:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MyGlobalBehavior<,>));
```

> [!IMPORTANT]
> Registration order matters. Behaviors run in the order they are added to the service collection.

## Where to Register Project-Specific Behaviors

For migration customizations, register custom behaviors in:

- [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs)

`UseCustomizations()` is invoked during CLI startup in:

- [Migration.Tool.CLI/Program.cs](../../Migration.Tool.CLI/Program.cs)

This makes `Migration.Tool.Extensions` the standard place for project-level behavior customizations.

## Custom CLI Command vs Pipeline Behavior

It is possible to add custom commands, but this is usually unnecessary for project migrations.

In most cases, use a pipeline behavior attached to an existing command stage instead of introducing a new command moniker. This avoids command-rank/dependency complexity and keeps customization aligned with the standard migration flow.

## Worked Recipe: Custom Tables → Taxonomies/Tags + Post-Pages Schema Attachment

This recipe mirrors a common advanced scenario:

- Source uses multiple custom tables as taxonomy/tag sources.
- Page data stores selected values in a legacy format (for example pipe-separated IDs).
- Target requires taxonomy/tag fields in reusable schema fields.

### Flow

1. **After `MigrateSitesCommand`**
   - Read source custom table metadata and rows via `ModelFacade`.
   - Create taxonomies and tags in target.
   - Do not import source custom tables as final target structures if they are only intermediates.
2. **Normal migration stages**
   - Migrate page types and pages using standard flow.
3. **After `MigratePagesCommand`**
   - Attach reusable schema to selected content types.
   - Convert legacy field values (for example `"1|2|3"`) into target taxonomy JSON format.
   - Write converted values into schema fields.
   - Remove obsolete legacy fields.

### Why post-pages schema attachment can be preferable

Attaching schema fields before page import can increase processing overhead and produce repeated unmapped-field warnings for large page sets. Deferring schema attachment until pages are imported can reduce noise and improve throughput.

### Safety checklist

- Make behavior operations idempotent for repeated runs.
- Add guard clauses for missing content types, fields, or source rows.
- Log each structural change (`add schema`, `remove field`, `skip because missing`).
- Restrict scope to known class lists when validating the scenario.

## Practical Guidance

- Keep behaviors idempotent for repeated migration runs.
- Add explicit guard logging for null/missing source or target objects.
- For expensive operations, run them in the smallest viable command stage.
- If a behavior materially changes content model shape, document run order and rerun strategy in your project notes.
- If performance degrades, move schema/field structure mutations to a post-pages stage and keep import stages minimal.

## Related Documentation

- [Targeted Code-Driven Customization](Customization-Targeted-Code.md)
- [Migration.Tool.Extensions README](../../Migration.Tool.Extensions/README.md)
- [Migration CLI README](../../Migration.Tool.CLI/README.md)
- [Repository Structure](../Repository-Structure.md)
