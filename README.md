[//]: # ([![Contributors][contributors-shield]][contributors-url])

[//]: # ([![Forks][forks-shield]][forks-url])

[//]: # ([![Stargazers][stars-shield]][stars-url])

[//]: # ([![Issues][issues-shield]][issues-url])

[//]: # ([![MIT License][license-shield]][license-url])

[//]: # ([![Discord][discussion-shield]][discussion-url])


<!-- ABOUT THE PROJECT -->
## About The Project

The aim of Migration.Toolkit is to provide tools for move data from Kentico Xperience 13 to future Kentico Xperience, codenamed Odyssey.

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

1) clone source code locally
2) open solution `Migration.Toolkit.sln` in favourite IDE
3) restore nuget packages
4) copy file `appsettings.json` as `appsettings.local.json` to same directory in project `Migration.Toolkit.CLI.csproj`
5) configure basic parameters in json configuration file `appsettings.local.json` (for more information go to **/Migration.Toolkit.CLI/README.md**)
    * `SourceConnectionString` - sql connection string to **source instance of Kentico Xperience 13**
    * `SourceCmsDirPath` - absolute path to filesystem root of **source instance**
    * `TargetConnectionString` - sql connection string to target **instance of Kentico Xperience Odyssey**
    * `TargetCmsDirPath` - absolute path to filesystem root of **target instance**
    * `MigrateOnlyMediaFileInfo` - if media files are not stored locally on filesystem set to true (for cloud storage), otherwise tool will try to find mediafile in `SourceCmsDirPath`
    * `TargetKxoApiSettings.ConnectionStrings.CMSConnectionString` - set same value as `TargetConnectionString`
    * `UseOmActivityNodeRelationAutofix` - for **Contact management** migration decide how will tool resolve reference to no longer existing page, options:
        * `DiscardData` - faulty data are thrown away
        * `Error` - error is reported by tool for later change to resolve problem
    * `UseOmActivitySiteRelationAutofix` - for **Contact management** migration decide how will tool resolve reference to no longer existing site, options:
        * `DiscardData` - faulty data are thrown away
        * `Error` - error is reported by tool for later change to resolve problem
6) **target instance** must have empty site as target for migration, by that no other data than from source KX13 source instance
    * especially no pages, only page of type `CMS.Root` is allowed
    * some commands cannot be run multiple time against same instance (reasons are optimization issues and it might be temporary measure)
        * `migrate --contact-management`
        * `migrate --forms`
7) configure project profile for running `Migration.Toolkit.CLI` (or alternatively build project and navigate to output directory with cmd, run command manually)
    * set command line arguments for profile
        * `migrate --siteId <siteId>` - where `<siteId>` is `SiteID` of source instance for example *4* (can be loaded from source instace using database query `SELECT * FROM dbo.CMS_Site`, value from `SiteID` column)
        * `--culture <cultureCode>` - where `<cultureCode>` is `CultureCode` of source instance for example *en-US* (currently multi-language migration is not supported)
        * `--sites` - tool will migrate Site object
        * `--users` - for migration of User object and object closely related to user (Roles)
        * `--settings-keys` - for migration of currently supported Setting objects
        * `--page-types` - for migration of Page Type object - required for migration of pages
        * `--pages` - for migration of Page object - target instance must not contain *other Pages than those copied from source instance to avoid Url conflicts*
        * `--attachments` - for migration of no longer supported Attachment objects to Media Libraries - name of target library is set with setting `TargetAttachmentMediaLibraryName`
        * `--contact-management` - Contact management migration, depends on how many contact you have in database this can take time
        * `--forms` - migration of form object and related data
        * `--media-libraries` - migration of Media Libraries and related Media Files (binary file information will be migrated if setting `MigrateOnlyMediaFileInfo` is set to value *false*)
        * `--data-protection` - Data protection object migration
        * `--bypass-dependency-check` - to skip command dependency check, if you know you migrated `--sites` already, then you can skip it using this override

8) all set - command line argument should look like this for complete migration `migrate --siteId 4 --culture en-US --sites --users --settings-keys --page-types --pages --attachments --contact-management --forms --media-libraries --data-protection`
9) run launch profile an observe cmd output (should be also stored in `$ProjectOut\logs\log*.txt` with default settings)

### Prerequisites

* installation of nuget packages
* reachable source instance database and filesystem locally
* reachable target instance database and filesystem locally

### Installation

TODO section

<!-- USAGE EXAMPLES -->
## Usage


* `Migration.Toolkit.CLI.exe migrate --siteId 1 --sites --users --settings-keys --media-libraries --page-types --pages --culture en-US`
* `Migration.Toolkit.CLI.exe migrate --siteId 1 --pages --culture en-US --bypass-dependency-check` - if you want to retry only pages migration while knowing `--page-types`, `--sites`, `--users` migration were already run completely

_For more examples, please refer to the **/Migration.Toolkit.CLI/README.md**_

<!-- CONTRIBUTING -->
## Contributing

TODO section
For Contributing please see  <a href="./CONTRIBUTING.md">`CONTRIBUTING.md`</a> for more information.

<!-- LICENSE -->
## License

TODO section
Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.