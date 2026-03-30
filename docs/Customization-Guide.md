# Customization Guide

> **Audience:** Developers and AI agents deciding how to customize migration behavior.

Use this guide to choose the right customization approach quickly.

## When You Need Customization

The migration tool handles standard data transformations automatically. You usually customize when:

1. **Project-specific behavior is required** (custom field types, non-standard widgets, module/table specifics).
2. **Content model changes are required** (restructure content types, convert pages to reusable content, transform relationships).

## Decision Model

Use this as a **decision aid**, not a strict process.

Think across two dimensions:

- **Scope of change**
  - **Targeted**: single field/widget/type adjustments.
  - **Command-stage flow**: multi-step logic spanning command stages.
- **Implementation style**
  - **Configuration-driven** (`appsettings.json`): tune built-in behavior through options.
  - **Code-driven** (`Migration.Tool.Extensions`): add field/widget/director/class mapping logic.
  - **Hybrid**: combine both (most common).

Most projects use a mix. Start with the smallest change that works, then escalate only when needed.

## Quick Examples

- **Targeted + configuration-driven**: tune conversion/feature options in `appsettings.json`.
- **Targeted + code-driven**: map one custom form control value or rename a widget property in `Migration.Tool.Extensions`.
- **Command-stage flow + hybrid**: run behavior after `--sites` to prepare taxonomy data, then after `--pages` attach reusable schema fields and convert legacy values, while limiting scope via configuration.

## Where to Go Next

1. Review [Repository Structure](Repository-Structure.md) to understand components and boundaries.
2. For configuration-driven changes, start with [Migration CLI README](../Migration.Tool.CLI/README.md).
3. For targeted code-driven customizations, use [Extensions README](../Migration.Tool.Extensions/README.md).
4. For command-stage pipeline customization, use [Command Pipeline Architecture Guide](Customization-Pipeline-Behaviors.md).
