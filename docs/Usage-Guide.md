# Usage Guide

Below you can find a list of prerequisites for the versions of Kentico supported by this migration tool.

For a full list of content and data that is supported by the migration tool, please see [Supported Data](/docs/Supported-Data.md).

## Source

The migration currently supports the Kentico Xperience 13, Kentico 12 or Kentico 11 as the source instance. See the following sections for compatibility information and limitations of respective versions.

### Kentico Xperience 13

- The source of the migration data must be a Kentico Xperience 13 instance, with [Refresh 5](https://docs.kentico.com/13/release-notes-xperience-13#ReleasenotesXperience13-Ref5), [hotfix 13.0.64](https://devnet.kentico.com/download/hotfixes) or newer applied.
- The development model (Core or MVC 5) does not affect the migration - both are supported.
- The source instance's database and file system must be accessible from the environment where you run the Kentico Migration Tool.
- All features described in this repository are available for migration from Kentico Xperience 13.

[![Kentico Xperience 13 upgrade paths](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-xperience-13-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-xperience-13-embedded.jpg)

### Kentico 12 MVC

- The source of the migration data can be any hotfix version of the Kentico 12.
  - If you encounter any issues, it is recommended to update to the latest hotfix.
- Only MVC development model is supported by this tool. Any Portal Engine project that you wish to migrate to Xperience by Kentico needs to be [migrated to MVC](https://www.youtube.com/watch?v=g2oeHU0h1e0) first.
- The source instance's database and file system must be accessible from the environment where you run the this tool.
- This repository describes the migration of the Kentico Xperience 13 feature set, however only features relevant to Kentico 12 MVC are migrated for this version.

[![Kentico Xperience 12 MVC upgrade paths](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-12-mvc-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-12-mvc-embedded.jpg)

### Kentico 11 and 12 Portal Engine

- The source of the migration data can be any hotfix version of the Kentico 12 or Kentico 11.
  - If you encounter any issues, it is recommended to update to the latest hotfix.
- The source instance's database and file system must be accessible from the environment where you run the this tool.
- Migration of Page Builder content is not supported. Only structured data of pages is migrated.
  - As a result, [source instance API discovery](/Migration.Toolkit.CLI/README.md#source-instance-api-discovery) is also not available.
- This repository describes the migration of the Kentico Xperience 13 feature set, however only features relevant to Kentico 11 and 12 Portal Engine are migrated for this version.

[![Kentico Xperience Portal Engine upgrade paths](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-10-12-portal-engine-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-10-12-portal-engine-embedded.jpg)

## Target

- The Kentico Migration Tool is periodically updated to support migration to the **latest version** of Xperience by Kentico. However, there may be delays between Xperience by Kentico releases and tool updates.
  - See the [README](/README.md#library-version-matrix) for supported releases of Xperience by Kentico.
- The target instance's database and file system must be accessible from the environment where you run this tool.
- The target instance's database must be empty except for data from the source instance created by previous runs of this tool to avoid conflicts and inconsistencies.

## Upgrade paths

The full set of upgrade paths to Xperience by Kentico can be seen below.

[![Full Kentico upgrade paths to Xperience by Kentico](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-full-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-full-embedded.jpg)
