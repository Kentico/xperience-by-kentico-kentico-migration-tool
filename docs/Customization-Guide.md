# Customization Guide

> **Audience:** Developers and AI agents deciding how to customize migration behavior.

Use this page to choose the right customization approach quickly.

## Customization Scenarios

The migration tool handles standard data transformations by default. Customization is typically relevant when:

1. **You need to migrate project-specific behavior** (for example, custom field types, non-standard widgets, or module/table-specific handling).
2. **You want to evolve your content model** (for example, restructuring content types, converting pages to reusable content, or transforming relationships to better utilize Xperience by Kentico features).

## Available Customization Options

See below the full supported set of customization options available in the tool, including when each one fits best:

- **Configuration** (`appsettings.json`)
  - Use built-in settings to tune migration behavior without writing custom code.
  - Typical fit: scoped adjustments to built-in behavior.
  - Example: set `MigrateOnlyMediaFileInfo` to migrate media records without files, configure `ConvertClassesToContentHub` for selected page types/custom tables, or use `CreateReusableFieldSchemaForClasses` for specific page types.
  - See all available configuration option details in the [Migration CLI README](../Migration.Tool.CLI/README.md).

- **Data transformation extensions** (`Migration.Tool.Extensions` project)
  - Use extension points for targeted field, widget, content type, and content item migration logic.
  - These extensions are often used together with configuration options (`appsettings.json`) as part of a hybrid approach.
  - Typical fit: targeted, project-specific transformations that configuration alone doesn't cover.
  - Example: map one custom form control value, transform widget properties, migrate/reshape widget structures, customize linked page handling, convert pages to widgets, link child pages as content item references, or merge/remodel selected content types.
  - See [Data Transformation Extensions](customization/Customization-Data-Transformation-Extensions.md) for an overview

- **Command pipeline customization** (`IPipelineBehavior<TRequest, TResponse>`)
  - Use pipeline behaviors for command-stage orchestration before/after specific migration commands.
  - See [Command Pipeline Architecture Guide](customization/Customization-Pipeline-Behaviors.md) for details.
  - Typical fit: multi-step logic that must run at specific command stages.
  - Example: run preparation logic after `--sites` and post-processing after `--pages` in one coordinated flow.

Most projects use a hybrid approach: start with configuration, add targeted data transformation extensions when needed, and use pipeline behaviors only for command-stage orchestration.

## Before diving into customizations

1. Review [Repository Structure](../Repository-Structure.md) to confirm where customization code belongs.
2. Start with the smallest option that can solve your scenario (typically configuration first), then move to broader customization only if needed.
3. Use the linked guides above as your implementation references for the option you choose.
