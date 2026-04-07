# Customization: Command Pipeline Architecture

> **Audience:** Developers implementing advanced command-pipeline scenarios that span command boundaries (for example, logic that must run after `--sites` or after `--pages`).

This page explains command-pipeline architecture and customization using MediatR pipeline behaviors (`IPipelineBehavior<TRequest, TResponse>`).

`IPipelineBehavior<TRequest, TResponse>` is a MediatR abstraction (not specific to this repository). This page documents how the migration tool uses that abstraction and where project-specific behavior implementations are added.

Use this document as the main reference for command-pipeline customizations.

## Command Pipeline Architecture

The command pipeline architecture is based on the [Mediator pattern](https://www.geeksforgeeks.org/system-design/mediator-design-pattern/).

The migration tool executes commands (for example `MigrateSitesCommand`, `MigratePagesCommand`) through the MediatR library.
Each command passes through a chain of pipeline behaviors before the command handler runs.

In this repository, migration actions are modeled as MediatR commands implementing `IRequest<CommandResult>` (see [Migration.Tool.Common/Commands.cs](../../Migration.Tool.Common/Commands.cs)). Commands are dispatched via `IMediator.Send(...)` in the CLI startup flow (see [Migration.Tool.CLI/Program.cs](../../Migration.Tool.CLI/Program.cs)).

### Built-in Behaviors

The tool uses three standard behavior types:

- **`RequestHandlingBehavior`**
  - Cross-cutting logging and protocol tracking around command execution. It starts a stopwatch, logs start/end timing, and reports command request/finish/error events to `IMigrationProtocol`. See [KVA/Migration.Tool.Source/Behaviors/RequestHandlingBehavior.cs](../../KVA/Migration.Tool.Source/Behaviors/RequestHandlingBehavior.cs).
- **`CommandConstraintBehavior`**
  - Pre-flight validation gate. It performs critical source checks and short-circuits with `CommandCheckFailedResult` if checks fail (site validity in all cores, and source-version key checks in KX12/KX13). See [KVA/Migration.Tool.Source/Behaviors/CommandConstraintBehavior.cs](../../KVA/Migration.Tool.Source/Behaviors/CommandConstraintBehavior.cs).
- **`XbyKApiContextBehavior` / `XbKApiContextBehavior`**
  - Target-environment bootstrap. It ensures target API initialization, verifies default admin existence, and executes downstream steps in a `CMSActionContext` under that admin. See [KVA/Migration.Tool.Source/Behaviors/XbyKApiContextBehavior.cs](../../KVA/Migration.Tool.Source/Behaviors/XbyKApiContextBehavior.cs).

The same three behavior categories are also implemented in the version-specific source cores for Kentico 11, Kentico Xperience 12, and Kentico Xperience 13 (`Migration.Tool.Core.K11`, `Migration.Tool.Core.KX12`, `Migration.Tool.Core.KX13`). In those projects, the API-context behavior is named `XbKApiContextBehavior`.

Behavior registrations are configured in [KVA/Migration.Tool.Source/KsCoreDiExtensions.cs](../../KVA/Migration.Tool.Source/KsCoreDiExtensions.cs), [Migration.Tool.Core.K11/K11CoreDiExtensions.cs](../../Migration.Tool.Core.K11/K11CoreDiExtensions.cs), [Migration.Tool.Core.KX12/KX12CoreDiExtensions.cs](../../Migration.Tool.Core.KX12/KX12CoreDiExtensions.cs), and [Migration.Tool.Core.KX13/DependencyInjectionExtensions.cs](../../Migration.Tool.Core.KX13/DependencyInjectionExtensions.cs).

Built-in pipeline order:

1. `RequestHandlingBehavior`
1. `CommandConstraintBehavior`
1. `XbyKApiContextBehavior` (or `XbKApiContextBehavior`, depending on source/core)
1. Command `Handler`

## Custom Behaviors

Project customization is done by adding your own `IPipelineBehavior<TRequest, TResponse>` implementations.

### Where custom behaviors run in the chain

For a given command, the execution order is:

1. `RequestHandlingBehavior`
2. `CommandConstraintBehavior`
3. `XbyKApiContextBehavior` / `XbKApiContextBehavior`
4. Your custom behavior (if registered for that exact command type)
5. Command handler logic executes

Then control returns back up the chain in reverse order.

This means code before `await next()` in your custom behavior runs before the handler, and code after `await next()` runs after the handler.

For example, when migration runs with the `--sites` switch:

1. CLI triggers `IMediator.Send(new MigrateSitesCommand())`
2. `RequestHandlingBehavior` runs
3. `CommandConstraintBehavior` runs
4. `XbyKApiContextBehavior`/`XbKApiContextBehavior` runs
5. Custom behavior for `MigrateSitesCommand` (if registered) starts, runs pre-handler logic, then calls `await next()`
6. `MigrateSitesCommand` handler runs
7. Control returns to the custom behavior for post-handler logic (code after `await next()`), then returns up the remaining chain

> [!NOTE] “Post-command” means logic that runs after `await next()` returns.

This lets you add common command-level logic, for example checks before a command runs, logic after it finishes, run-order control, and logging/monitoring.

For migration projects, the most practical pattern is **command-scoped behaviors** (closed generic registration), for example one behavior after `MigrateSitesCommand` and one after `MigratePagesCommand`.

Register custom behaviors in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddTransient(
    typeof(IPipelineBehavior<MigrateSitesCommand, CommandResult>),
    typeof(CreateTaxonomiesFromCustomTablesBehavior));

services.AddTransient(
    typeof(IPipelineBehavior<MigratePagesCommand, CommandResult>),
    typeof(AttachGlobalTagsSchemaAfterPageTypesBehavior));
```

### Steps to use a custom pipeline behavior

1. Pick the command stage where your data is available (for example `MigrateSitesCommand` or `MigratePagesCommand`).
   - `MigrateSitesCommand` - site-level setup
   - `MigratePageTypesCommand` - content type structure
   - `MigratePagesCommand` - post-page data updates
   - `MigrateCustomTablesCommand` - custom-table processing
   - Other commands are valid for domain-specific scenarios (for example orders, customers, forms, media libraries). If unsure, confirm order in [Migration.Tool.Common/Commands.cs](../../Migration.Tool.Common/Commands.cs).
2. Implement `IPipelineBehavior<TCommand, CommandResult>` for that command.
3. In `Handle(...)`, call `await next()` once, then run post-processing logic.
4. Register the behavior in DI with a closed generic mapping in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs).

For a full end-to-end example, see [Example: Global Tags Taxonomy Migration (KX13)](#example-global-tags-taxonomy-migration-kx13).

> [!IMPORTANT]
> If you have multiple custom processes for the same command stage (for example multiple post-`MigratePagesCommand` concerns), use **one consolidated custom behavior** for that command and delegate to processor services. In end-to-end runs with many customizations, this pattern is more predictable and makes it easier to verify that all steps run.

```csharp
// Processor services
services.AddTransient<GlobalTagsSchemaPostProcessor>();
services.AddTransient<HomepageSlideSmartFolderPostProcessor>();
services.AddTransient<VerticalTabContentItemReferencePostProcessor>();

// Single consolidated behavior for post-pages stage
services.AddTransient(
    typeof(IPipelineBehavior<MigratePagesCommand, CommandResult>),
    typeof(PostPageImportBehavior));
```

## When to Use Custom Pipeline Behaviors

When possible, prefer data transformation extension points first (for example field, widget, content-item-director, and class-mapping customizations); see [Data Transformation Extensions](Data-Transformation-Extensions.md).

Use `IPipelineBehavior` when you need command-flow customization around execution and explicit control over when logic runs.

**Example use cases:**

- **Create tags from source custom tables (`MigrateSitesCommand`)** - Read source lookup tables and create tags/taxonomies in the target. Do this when those tables are only helper data and should not be migrated as final objects.
- **Fix page data after page import (`MigratePagesCommand`)** - After pages are migrated, add reusable schemas, convert old values (for example `"1|2|3"`) to a new taxonomy JSON format, and remove old fields.
- **Run commerce commands in a safe order (`MigrateCustomersCommand` / `MigrateOrdersCommand`)** - Add checks so order migration runs only after customer/member data is ready and mapped.
- **Run many post-page steps in one place** - Use one behavior for `MigratePagesCommand` and call small processor services from it, in a fixed order, with separate error logging for each step.

## Accessing Source and Target Data

### ModelFacade

`ModelFacade` ([KVA/Migration.Tool.Source/ModelFacade.cs](../../KVA/Migration.Tool.Source/ModelFacade.cs)) is your read-only window into source instance data (K11/KX12/KX13) without triggering default object migration.

In practice, this means a command-scoped `IPipelineBehavior` can read source data at a precise stage (for example after `MigrateSitesCommand`) and apply custom logic before or after `await next()`.

This is especially useful when your behavior depends on source table data, but you do not want to migrate those tables as final target structures.

Inject it via DI and query the source directly:

```csharp
public class MyBehavior(ModelFacade modelFacade)
    : IPipelineBehavior<MigrateSitesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        MigrateSitesCommand request,
        RequestHandlerDelegate<CommandResult> next,
        CancellationToken cancellationToken)
    {
        // Get source class definitions
        var sourceClasses = modelFacade.SelectAll<ICmsClass>().ToList();

        // Get rows for a specific source table as dictionaries
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

## Example: Global Tags Taxonomy Migration (KX13)

This example reflects a common migration pattern and shows why command-scoped behaviors are chosen.

### Scenario summary

- Source (KX13) has 6 custom tables functioning as taxonomy/tag sources.
- Pages store selected tag IDs in pipe-separated values (for example `"1|2|3|4|5"`).
- Target (XbyK) needs taxonomy/tag fields and reusable schema fields.
- Goal: use source table data, but do not migrate those tables as final structures in the target instance.

### Stage mapping used in practice

0. **Pre-run setup (one-time, outside behaviors)**
   - Create the reusable field schema definition once (for example with a startup customization step or admin/API setup).
1. **After `MigrateSitesCommand`**
   - Read source custom-table metadata/data through `ModelFacade`.
   - Create taxonomies and tags in target.
2. **Normal migration flow**
   - Migrate page types and pages.
3. **After `MigratePagesCommand`**
   - Attach reusable schema to target content types.
   - Convert legacy pipe-separated values to target taxonomy JSON format.
   - Populate schema fields and remove old source fields.

### Why the post-pages step matters

In this case, attaching schema before page import causes heavy slowdown and repeated unmapped-field warnings. Running schema attachment and value conversion after pages are imported is more reliable.

### Consolidated behavior pattern

When you have multiple post-pages operations, use one consolidated behavior and call processor services from it:

```csharp
public sealed class PostPageImportBehavior(
    ILogger<PostPageImportBehavior> logger,
    GlobalTagsSchemaPostProcessor globalTagsProcessor,
    HomepageSlideSmartFolderPostProcessor homepageSlideProcessor,
    VerticalTabContentItemReferencePostProcessor verticalTabProcessor)
    : IPipelineBehavior<MigratePagesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        MigratePagesCommand request,
        RequestHandlerDelegate<CommandResult> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        try
        {
            logger.LogInformation("Starting Global Tags schema post-processing...");
            await globalTagsProcessor.ExecuteAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed during Global Tags schema post-processing.");
        }

        try
        {
            logger.LogInformation("Starting HomepageSlide smart folder post-processing...");
            await homepageSlideProcessor.ExecuteAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed during HomepageSlide smart folder post-processing.");
        }

        try
        {
            logger.LogInformation("Starting VerticalTab content item reference post-processing...");
            verticalTabProcessor.Execute();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed during VerticalTab content item reference post-processing.");
        }

        return result;
    }
}
```

Processor shape:

```csharp
public sealed class VerticalTabContentItemReferencePostProcessor(
    ILogger<VerticalTabContentItemReferencePostProcessor> logger,
    ModelFacade modelFacade,
    ISpoiledGuidContext spoiledGuidContext)
{
    public void Execute()
    {
        AddContentItemReferenceField();
        PopulateContentItemReferences();
    }

    private void AddContentItemReferenceField() { /* ... */ }
    private void PopulateContentItemReferences() { /* ... */ }
}
```

### Registration

Register custom behaviors in [Migration.Tool.Extensions/ServiceCollectionExtensions.cs](../../Migration.Tool.Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddTransient(
    typeof(IPipelineBehavior<MigratePagesCommand, CommandResult>),
    typeof(AttachGlobalTagsSchemaAfterPageTypesBehavior));

services.AddTransient<GlobalTagsSchemaPostProcessor>();
services.AddTransient<HomepageSlideSmartFolderPostProcessor>();
services.AddTransient<VerticalTabContentItemReferencePostProcessor>();
services.AddTransient(
    typeof(IPipelineBehavior<MigratePagesCommand, CommandResult>),
    typeof(PostPageImportBehavior));
```

`UseCustomizations()` is wired during CLI startup in [Migration.Tool.CLI/Program.cs](../../Migration.Tool.CLI/Program.cs).

> [!IMPORTANT]
> **Safety checklist**
>
> - Make behavior operations idempotent for repeated runs.
> - Add guard clauses for missing content types, fields, or source rows.
> - Log each structural change (`add schema`, `remove field`, `skip because missing`).
> - Restrict scope to known class lists when validating the scenario.
> - For expensive operations, run them in the smallest viable command stage.
> - If a behavior materially changes content model shape, document run order and rerun strategy in your project notes.
> - If performance degrades, move schema/field structure mutations to a post-pages stage and keep import stages minimal.

## Related Documentation

- [Customization Guide](../Customization-Guide.md) - Overview of available Kentico Migration Tool customization options and recommended decision path.
- [Data Transformation Extensions](Data-Transformation-Extensions.md) - Use these extension points first for object/field/widget transformation scenarios; use pipeline behaviors when you need command-stage flow control.
- [Repository Structure](../Repository-Structure.md) - Project/component map showing where customization code belongs.
- [Migration CLI README](../../Migration.Tool.CLI/README.md) - CLI command options and execution flow details.
