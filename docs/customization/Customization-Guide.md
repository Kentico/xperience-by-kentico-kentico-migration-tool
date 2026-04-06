# Customization Guide

> **Audience:** Developers and AI agents deciding how to customize migration behavior.

Use this page to choose the right customization approach quickly.

## Customization Scenarios

The migration tool handles standard data transformations by default. Customization is typically relevant when:

1. **Project-specific behavior is required** (custom field types, non-standard widgets, module/table specifics).
2. **Content model changes are required** (restructure content types, convert pages to reusable content, transform relationships).

## Decision Model

This model is intended as a decision aid rather than a strict process.

Customization choices are commonly evaluated across two dimensions:

- **Scope of change**
  - **Targeted**: single field/widget/type adjustments.
  - **Command-stage flow**: multi-step logic spanning command stages.
- **Implementation style**
  - **Configuration-driven** (`appsettings.json`): tune built-in behavior through options.
  - **Code-driven** (`Migration.Tool.Extensions`): add field/widget/director/class mapping logic.
  - **Hybrid**: combine both (most common).

Most projects combine approaches, with implementation scope determined by migration complexity.

## Approach Examples

- **Targeted + configuration-driven**: tune conversion/feature options in `appsettings.json`.
- **Targeted + code-driven**: map one custom form control value or rename a widget property in `Migration.Tool.Extensions`.
- **Command-stage flow + hybrid**: run behavior after `--sites` to prepare taxonomy data, then after `--pages` attach reusable schema fields and convert legacy values, while limiting scope via configuration.

## Recommended Customization Path

When customization is required, use this sequence:

1. Review [Repository Structure](../Repository-Structure.md) to confirm where customization code belongs.
2. Evaluate configuration-first options in [Migration CLI README](../../Migration.Tool.CLI/README.md).
3. If configuration is not sufficient, implement targeted extensions described in [Extensions for Data Transformation](Customization-Data-Transformation-Extensions.md).
4. For cross-command or stage-specific orchestration, implement MediatR pipeline behaviors (`IPipelineBehavior<TRequest, TResponse>`) as project-specific extensions using [Command Pipeline Architecture Guide](Customization-Pipeline-Behaviors.md).
