[//]: # "[![Contributors][contributors-shield]][contributors-url]"
[//]: # "[![Forks][forks-shield]][forks-url]"
[//]: # "[![Stargazers][stars-shield]][stars-url]"
[//]: # "[![Issues][issues-shield]][issues-url]"
[//]: # "[![MIT License][license-shield]][license-url]"
[//]: # "[![Discord][discussion-shield]][discussion-url]"

<!-- ABOUT THE PROJECT -->
# Migration toolkit for Xperience by Kentico

The Migration toolkit transfers content and other data from **Kentico Xperience 13** to **Xperience by Kentico**.

## Prerequisites & Compatibility

### Source

The migration currently supports the Kentico Xperience 13 or Kentico 12 as the source instance. See the following sections for compatibility information and limitations of respective versions.

#### Kentico Xperience 13

* The source of the migration data must be a Kentico Xperience 13 instance, with **Refresh 5** ([hotfix](https://devnet.kentico.com/download/hotfixes) 13.0.64) or newer applied.
* The development model (Core or MVC 5) does not affect the migration - both are supported.
* The source instance's database and file system must be accessible from the environment where you run the Migration toolkit.
* All features described in this repository are available for migration from Kentico Xperience 13.

#### Kentico 12

* The source of the migration data can be any hotfix version of the Kentico 12.
* Only MVC development model is supported by the migration tool. Any Portal Engine project that you wish to migrate to Xperience by Kentico needs to be [migrated to MVC](https://www.youtube.com/watch?v=g2oeHU0h1e0) first.
* The source instance's database and file system must be accessible from the environment where you run the Migration toolkit.
* Migration of Page Builder content is not supported. Only structured data of pages is migrated.
  * As a result, [source instance API discovery](/Migration.Toolkit.CLI/README.md#source-instance-api-discovery) is also not available.
* This repository describes the migration of Kentico Xperience 13 feature set. Only features relevant to Kentico 12 are migrated for this version.

### Target

* The migration toolkit is periodically updated to support migration to the **latest version** of Xperience by Kentico. However, there may be delays between Xperience by Kentico releases and toolkit updates.
  * Currently, Xperience by Kentico **28.0.1** is tested and supported.
* The target instance's database and file system must be accessible from the environment where you run the Migration toolkit.
* The target instance's database must be empty except for data from the source instance created by previous runs of the Migration toolkit to avoid conflicts and inconsistencies.

## Supported data and limitations

The Migration toolkit does not transfer all data available in the source instance. Xperience by Kentico currently provides a smaller, more focused set of features. As a result, some objects are discarded or migrated to a suitable alternative.

The Migration toolkit only supports content and objects **stored in the database** and related binary data on the file system, such as media library files. Code, customizations, and any other types of content need to be migrated manually to the target project and adjusted for Xperience by Kentico.

Currently, the Migration toolkit supports the following types of data:

* **Sites**
  * The toolkit migrates each site on the source to a [website channel](https://docs.xperience.io/x/34HFC) object in Xperience by Kentico.
* **Cultures**
  * The set of cultures used across all sites in the source gets mapped to a [language](https://docs.xperience.io/x/OxT_Cw) in the _Languages_ application.
* **Content types** (_Page types_ in earlier Kentico versions)
  * The Migration toolkit attempts to map the _Data type_ and _Form control_ of page type fields to an appropriate equivalent in Xperience by Kentico. This mapping is not always possible and does not work for custom data types or form controls. We recommend checking your content type fields after the migration and adjusting them if necessary.
  * The migration includes only page types assigned to at least one site on the source instance.
  * Xperience by Kentico currently does not support:
    * Macro expressions in page type field default values or other settings. Content type fields containing macros will not work correctly after the migration.
    * Page type inheritance. You cannot migrate page types that inherit fields from other page types.
    * Categories for page type fields. Field categories are not migrated with page types.
  * All migrated Content types have the **Page** feature enabled (the migration never creates non-page content items).
* **Pages**
  * The migration includes the following versions of pages:
    * _Published_
    * _Latest draft version_ - for published pages, the version is migrated to the _Draft_ [workflow step](https://docs.xperience.io/x/JwKQC#Pages-Pageworkflow); for pages that do not have a published version, the version is migrated to the _Draft (initial)_ workflow step.
    * _Archived_
  * Each page gets assigned under its corresponding website channel.
  * Linked pages are currently not supported in Xperience by Kentico. The migration creates standard page copies for any linked pages on the source instance.
  * Page permissions (ACLs) are currently not supported in Xperience by Kentico and are not migrated.
  * Migration of Page Builder content is only available for Kentico Xperience 13.
* **Page attachments**
  * Page attachments are not supported in Xperience by Kentico. Attachments are migrated into media libraries. See [`Migration.Toolkit.CLI/README.md - Attachments`](./Migration.Toolkit.CLI/README.md#Attachments) for detailed information about the conversion process.
* **Preset page templates** (_Custom page templates_ in Kentico Xperience 13)
  * Migration of custom page templates is only available for Kentico Xperience 13.
* **Media libraries and media files**
  * Media library permissions are currently not supported in Xperience by Kentico and are not migrated.
* **Forms**
  * The migration does not include the content of form autoresponder and notification emails. You can migrate form autoresponders to Xperience by Kentico manually by copying your HTML code and content into Email templates and Emails. See [Emails](https://docs.xperience.io/x/IaDWCQ).
* **Users**
  * Xperience by Kentico uses separate entities for users with access to the administration interface (_CMS\_User_ table) and live site visitor accounts (_CMS\_Member_ table). Consequently, only users whose _Privilege level_ is _Editor_ or higher are migrated (_Users_ -> edit a user -> _General_ tab).
  * Users in Xperience by Kentico must have an email address. Migration is only supported for users who have a unique email address value on the source instance.
  * Custom user fields are an optional part of _module class_ migration.
  * Live site users are represented using a separate **Member** entity and stored in the _CMS_Member_ table. The migration identifies live site users as those without access to the administration interface - accounts with _Privilege level_ set to _None_ (Users -> edit a user -> General tab).
* **Roles**
  * Only roles that have at least one user whose _Privilege level_ is set to _Editor_ and above are migrated.
  * Because Xperience by Kentico uses a different [permission model](https://docs.xperience.io/x/7IVwCg), no existing role permissions or UI personalization settings are migrated. After the migration, the permissions for each role must be configured again.
* **Contacts**
  * The target instance's _OMContact_ and _OMActivity_ database tables must be empty for performance reasons.
  * Custom contact fields are an optional part of _module class_ migration.
* **Activities**
* **Consents and consent agreements**
* **Modules and classes**
  * The migration includes the following:
    * Custom modules
    * All classes associated with custom modules
    * All data stored within custom module classes
    * The following customizable system classes and their custom fields: _User_, _Media file_, _Contact management - Account_ (however, accounts are currently not supported in Xperience by Kentico), _Contact management - Contact_
  * Module and class migration does NOT include:
    * UI elements and all related user interface settings. The administration of Xperience by Kentico uses a different technology stack than Kentico Xperience 13 and is incompatible. To learn how to build the administration UI, see [Extend the administration interface](https://docs.xperience.io/x/GwKQC) and [Example - Offices management application](https://docs.xperience.io/x/hIFwCg).
    * Alternative forms under classes and UI-related configuration of class fields (field labels, Form controls, etc.). After the migration, you must manually create the appropriate [UI forms](https://docs.xperience.io/x/V6rWCQ) in Xperience by Kentico.
    * Custom settings under modules, which are currently not supported in Xperience by Kentico
    * Module permissions (permissions work differently in Xperience by Kentico - see [Role management](https://docs.xperience.io/x/7IVwCg) and [UI page permission checks](https://docs.xperience.io/x/8IKyCg))
    * As with all object types, the migration toolkit does not transfer code files to the target project. You must manually move all code files generated for your custom classes (_Info_, _InfoProvider_, etc.).
* **Setting values**
  * The migration only transfers the settings that exist in Xperience by Kentico.
  * The migration excludes site-specific settings that do not have a corresponding website channel-specific alternative in Xperience by Kentico.

* **Countries and states**

### Unsupported data

The following types of data exist in Xperience by Kentico but are currently **not supported** by the Migration toolkit:

* **Contact groups**
  * Static contact groups are currently not supported in Xperience by Kentico.
  * The condition format for dynamic contact groups is not compatible. To migrate contact groups:
    1. Migrate your contacts using the toolkit.
    2. Create the [contact groups](https://docs.xperience.io/x/o4PWCQ) manually in Xperience by Kentico.
    3. Build equivalent conditions.
    4. Recalculate the contact groups.
* **License keys**
  * Xperience by Kentico uses a different license key format.

 Additionally, object values or other content with **Macros** will not work correctly after the migration. Macro expressions are currently not supported for most data in Xperience by Kentico.

<!-- GETTING STARTED -->
## Get started

Follow the steps below to run the Migration toolkit:

1. Clone or download the Migration.Toolkit source code from this repository.
2. Open the `Migration.Toolkit.sln` solution in Visual Studio.
3. Configure the options in the `Migration.Toolkit.CLI/appsettings.json` configuration file. See [`Migration.Toolkit.CLI/README.md - Configuration`](./Migration.Toolkit.CLI/README.md#Configuration) for details.
4. Rebuild the solution and restore all required NuGet packages.
5. Open the command line prompt.
6. Navigate to the output directory of the `Migration.Toolkit.CLI` project.
7. Run the `Migration.Toolkit.CLI.exe migrate` command.
    * The following example shows the command with all parameters for complete migration:

        ```powershell
        Migration.Toolkit.CLI.exe  migrate --sites --users --settings-keys --page-types --pages --attachments --contact-management --forms --media-libraries --data-protection --countries
        ```

8. Observe the command line output. The command output is also stored in a log file (`logs\log-<date>.txt` under the output directory by default), which you can review later.
9. Review the migration protocol, which provides information about the result of the migration, lists required manual steps, etc.

    * You can find the protocol in the location specified by the `MigrationProtocolPath` key in the `appsettings.json` configuration file.
    * For more information, see [`Migration.Toolkit.CLI/MIGRATION_PROTOCOL_REFERENCE.md`](./Migration.Toolkit.CLI/MIGRATION_PROTOCOL_REFERENCE.md).

The data is now migrated to the target Xperience by Kentico instance according to your configuration. See [`Migration.Toolkit.CLI/README.md`](./Migration.Toolkit.CLI/README.md) for detailed information about the migration CLI, configuration options, instructions related to individual object types, and manual migration steps.

<!-- CONTRIBUTING -->
## Contributing

See [`CONTRIBUTING.md`](./CONTRIBUTING.md) to learn how to file issues, start discussions, and submit contributions.

Please provide all available information about the problem or error when submitting issues. If possible, include the command line output log file and migration protocol generated by the `Migration.Toolkit.CLI.exe migrate` command.

<!-- LICENSE -->
## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Questions & Support

See the [Kentico home repository](https://github.com/Kentico/Home/blob/master/README.md) for more information about the products and general advice on submitting questions.
