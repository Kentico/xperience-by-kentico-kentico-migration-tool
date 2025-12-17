# Complete Upgrade Guide Resources

The Kentico Migration Tool is one part of a complete upgrade from Kentico Xperience 13 to Xperience by Kentico. This page connects you to all available resources.

---

## Quick Start

### New to migration?
**→** [Understanding the Migration Process](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-from-kx13-overview)  
Learn the high-level upgrade process, what's involved, and whether it's right for your project.

### Ready to migrate?
**→** [Step-by-Step Upgrade Walkthrough](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-walkthrough)  
Follow a hands-on tutorial migrating the Dancing Goat sample site with video guides.

### Need to customize?
**→** [Customization Guide](../Migration.Tool.Extensions/README.md)  
Create custom field migrations, widget migrations, and class mappings.

---

## All Documentation Resources

### Architecture & Planning Guides (docs.kentico.com)

**For decision-makers and architects** planning the upgrade:

| Guide | What It Covers | When to Use |
|-------|----------------|-------------|
| [**Upgrade Overview**](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-from-kx13-overview) | Complete 4-phase upgrade process | **Read this first** |
| [**Plan Your Migration Strategy**](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/plan-your-strategy-for-migrating-features) | Effort level (low/medium/high) per feature | Estimating resources |
| [**Prep for Upgrade & Transfer Data**](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/prep-for-upgrade-and-transfer-data) | Environment setup, iterative migration approach | Before first migration |
| [**Adjust Code & Adapt**](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/adjust-your-code-and-adapt) | Post-migration code changes required | After data migration |
| [**Upgrade FAQ**](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq) | Common questions & troubleshooting | When stuck or planning |

### Hands-On Tutorial (docs.kentico.com)

**For developers** learning by doing:

| Guide | What It Covers | Time Required |
|-------|----------------|---------------|
| [**Upgrade Walkthrough**](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-walkthrough) | Complete Dancing Goat migration with videos | 2-4 hours |

**Includes video tutorials** showing actual migration tool execution, configuration, and code changes step-by-step.

### Developer Deep-Dive Guides (docs.kentico.com)

**For developers** implementing advanced customizations:

#### Content Modeling & Structure
- [**Remodel Page Types as Reusable Field Schemas**](https://docs.kentico.com/guides/development/upgrade-deep-dives/remodel-page-types-as-reusable-field-schemas)  
  Convert KX13 page types to modern, reusable XbyK content types
  
- [**Speed Up Remodeling with AI**](https://docs.kentico.com/guides/development/upgrade-deep-dives/speed-up-remodeling-with-ai)  
  Use AI tools to accelerate content type conversion
  
- [**Transfer Page Hierarchy to Content Hub**](https://docs.kentico.com/guides/development/upgrade-deep-dives/transfer-page-hierarchy-to-content-hub)  
  Migrate pages as reusable content items instead of website pages

#### Widget Migration
- [**Widget Migration Introduction**](https://docs.kentico.com/guides/development/upgrade-deep-dives/upgrade-widgets-introduction)  
  **Start here** - Overview of widget migration approaches (legacy mode vs. API discovery vs. custom)
  
- [**Migrate Widget Data to Content Hub**](https://docs.kentico.com/guides/development/upgrade-deep-dives/migrate-widget-data-to-content-hub)  
  Convert widget inline content to reusable content items
  
- [**Transform Widget Properties**](https://docs.kentico.com/guides/development/upgrade-deep-dives/transform-widget-properties)  
  Customize how widget properties migrate (IWidgetPropertyMigration examples)
  
- [**Convert Child Pages to Widgets**](https://docs.kentico.com/guides/development/upgrade-deep-dives/convert-child-pages-to-widgets)  
  Turn page tree nodes into Page Builder widgets

#### Performance & Optimization
- [**Optimize Images During Upgrade**](https://docs.kentico.com/guides/development/upgrade-deep-dives/optimize-images-during-upgrade)  
  Improve performance by optimizing media during migration
  
- [**Upgrade Content Retrieval Code**](https://docs.kentico.com/guides/development/upgrade-deep-dives/upgrade-content-retrieval)  
  Migrate from DocumentQuery API to XbyK Content Item API

### Migration Tool Documentation (This GitHub Repo)

**Technical reference** for configuring and running the tool:

| Document | What It Covers |
|----------|----------------|
| [**CLI README**](../Migration.Tool.CLI/README.md) | Commands, parameters, configuration options |
| [**Usage Guide**](Usage-Guide.md) | Supported versions, prerequisites, upgrade paths |
| [**Supported Data**](Supported-Data.md) | What can/cannot be migrated |
| [**Extensions README**](../Migration.Tool.Extensions/README.md) | Custom migrations (field, widget, class mappings) |
| [**Migration Protocol Reference**](../Migration.Tool.CLI/MIGRATION_PROTOCOL_REFERENCE.md) | Understanding output reports |
| [**Contributing Setup**](Contributing-Setup.md) | For contributors to the tool |

---

## Recommended Learning Paths

### Path 1: For Project Managers / Decision Makers

**Goal:** Understand what's involved and estimate effort

1. [Upgrade Overview](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-from-kx13-overview)
2. [Plan Your Migration Strategy](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/plan-your-strategy-for-migrating-features)
3. [Upgrade FAQ](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq)
4. [Supported Data](Supported-Data.md) (this repo)

### Path 2: For Developers (First Migration)

**Goal:** Successfully run your first data migration

1. [Prep for Upgrade & Transfer Data](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/prep-for-upgrade-and-transfer-data)
2. [Upgrade Walkthrough](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-walkthrough) - **Hands-on with videos**
3. [CLI README](../Migration.Tool.CLI/README.md) (this repo)
4. [Usage Guide](Usage-Guide.md) (this repo)
5. [Upgrade FAQ](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq) - when stuck

### Path 3: For Developers (Customizing Migration)

**Goal:** Implement custom field or widget migrations

1. [Widget Migration Introduction](https://docs.kentico.com/guides/development/upgrade-deep-dives/upgrade-widgets-introduction)
2. [Extensions README](../Migration.Tool.Extensions/README.md) (this repo) - **Technical implementation**
3. Choose relevant deep-dive guides based on your needs:
   - Custom widgets? → [Transform Widget Properties](https://docs.kentico.com/guides/development/upgrade-deep-dives/transform-widget-properties)
   - Pages to widgets? → [Convert Child Pages to Widgets](https://docs.kentico.com/guides/development/upgrade-deep-dives/convert-child-pages-to-widgets)
   - Content Hub migration? → [Transfer to Content Hub](https://docs.kentico.com/guides/development/upgrade-deep-dives/transfer-page-hierarchy-to-content-hub)
4. [Adjust Code & Adapt](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/adjust-your-code-and-adapt)

---

## Find What You Need

### By Task

- **"I need to understand the big picture"**  
  → [Upgrade Overview](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-from-kx13-overview)

- **"I want to see it done step-by-step"**  
  → [Upgrade Walkthrough](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-walkthrough)

- **"I need to configure the tool"**  
  → [CLI README](../Migration.Tool.CLI/README.md)

- **"I have custom widgets to migrate"**  
  → [Widget Migration Introduction](https://docs.kentico.com/guides/development/upgrade-deep-dives/upgrade-widgets-introduction)

- **"Something went wrong"**  
  → [Upgrade FAQ](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq)

- **"I need to customize field migration"**  
  → [Extensions README](../Migration.Tool.Extensions/README.md#customize-field-migrations)

- **"I want to migrate pages to Content Hub"**  
  → [Transfer Page Hierarchy to Content Hub](https://docs.kentico.com/guides/development/upgrade-deep-dives/transfer-page-hierarchy-to-content-hub)

- **"How do I update my content retrieval code?"**  
  → [Upgrade Content Retrieval](https://docs.kentico.com/guides/development/upgrade-deep-dives/upgrade-content-retrieval)

### By Content Type

- **Conceptual Overviews** → docs.kentico.com Architecture guides
- **Hands-On Tutorials** → docs.kentico.com Walkthrough (with videos)
- **Technical Reference** → This repo's README files
- **Advanced Techniques** → docs.kentico.com Deep-dive guides
- **Troubleshooting** → [Upgrade FAQ](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq)

---

## Still Have Questions?

- **General upgrade questions** → [Upgrade FAQ](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq)
- **Tool-specific issues** → [GitHub Issues](https://github.com/Kentico/xperience-by-kentico-kentico-migration-tool/issues)
- **Community discussion** → [Kentico Community](https://community.kentico.com/)
- **Enterprise support** → [Kentico Consulting](https://www.kentico.com/services/consulting)

---

## Understanding the Documentation Ecosystem

**docs.kentico.com** = WHY, WHEN, and strategic guidance  
**This GitHub repo** = HOW, configuration, and technical reference

Both are essential for a successful upgrade. Use them together for the best results.
