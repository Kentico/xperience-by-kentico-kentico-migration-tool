[//]: # "[![Contributors][contributors-shield]][contributors-url]"
[//]: # "[![Forks][forks-shield]][forks-url]"
[//]: # "[![Stargazers][stars-shield]][stars-url]"
[//]: # "[![Issues][issues-shield]][issues-url]"
[//]: # "[![MIT License][license-shield]][license-url]"
[//]: # "[![Discord][discussion-shield]][discussion-url]"


<!-- ABOUT THE PROJECT -->
## Migration toolkit for Xperience

The Migration toolkit transfers content and other data from **Kentico Xperience 13** to **Xperience by Kentico**.

## Prerequisites & Compatibility

### Source

  * The source of the migration data must be a Kentico Xperience 13 instance, with **Refresh 5** ([hotfix](https://devnet.kentico.com/download/hotfixes) 13.0.64) or newer applied. 
  * The development model (Core or MVC 5) does not affect the migration and both are supported.
  * The source instance's database and file system must be accessible from the environment where you run the Migration toolkit.

### Target

  * The toolkit is periodically updated to support migration to the **latest version** of Xperience by Kentico. However, there may be time gaps between Xperience by Kentico releases and Migration toolkit updates.
	* Currently, Xperience by Kentico **23.0.3** is tested and supported.
  * The target instance's database and file system must be accessible from the environment where you run the Migration toolkit.
  * To avoid conflicts and inconsistencies, the target instance must not contain any data apart from an empty site and/or data from the source site created by previous runs of the Migration toolkit.

## Supported data and limitations

The Migration toolkit does not transfer all data available in the Kentico Xperience 13 source. Xperience by Kentico currently provides a smaller, more focused, set of features, so many objects are not supported for migration. Certain types of data are migrated to a suitable alternative or discarded.

The Migration toolkit only supports content and objects **stored in the database**, except for related binary data on the file system, such as media library files. Code, customizations, and any other types of content need to be migrated manually to the target project and adjusted for Xperience by Kentico.

Currently, the Migration toolkit supports the following types of data:
  * **Sites**
  * **Content types** (_Page types_ in Kentico Xperience 13)
    * The Migration toolkit attempts to map the _Data type_ and _Form control_ of page type fields to an appropriate equivalent in Xperience by Kentico. This is not always possible, and cannot be done for custom data types or form controls. We recommend that you check your content type fields after the migration and adjust them if necessary.
	* Only page types assigned to the migrated site on the source instance are included.
	* Xperience by Kentico currently does not support:
      * Macro expressions in page type field default values or other settings. Content type fields containing macros will not work correctly after the migration.
	  * Page type inheritance. You cannot migrate page types that inherit fields from other types.
	  * Categories for page type fields. Field categories are not migrated with page types.
	* All migrated Content types have the **Page** feature enabled (the migration never creates non-page content items).
  * **Pages**
	* Xperience by Kentico currently does not support multilingual sites. You need to select one culture from which the content of pages is migrated.
	* Only pages that are **published** on the source instance are migrated.
	* Includes the **Former URLs** of pages, but not Alternative URLs, which are currently not supported in Xperience by Kentico.
	* Linked pages are currently not supported in Xperience by Kentico. The migration creates standard page copies for any linked pages on the source instance.
	* Page permissions (ACLs) are currently not supported in Xperience by Kentico, so are not migrated.
  * **Page attachments**
	* Page attachments are not supported in Xperience by Kentico. Attachments are migrated into media libraries. See [`Migration.Toolkit.CLI/README.md - Attachments`](./Migration.Toolkit.CLI/README.md#Attachments) for detailed information about the conversion process.
  * **Preset page templates** (_Custom page templates_ in Kentico Xperience 13)
  * **Media libraries and media files**
	* Media library permissions are currently not supported in Xperience by Kentico, so are not migrated.
  * **Forms**
    * The migration does not include the content of form autoresponder and notification emails. You can migrate form autoresponders to Xperience by Kentico manually by copying your HTML code and content into Email templates and Emails. See [Emails](https://docs.xperience.io/x/IaDWCQ).
  * **Users**
	* Xperience by Kentico currently does not support registration and authentication of users on the live site. User accounts only control access to the administration interface. Consequently, only users whose _Privilege level_ is set to _Editor_ and above are migrated (_Users_ -> edit a user -> _General_ tab).
	* Users in Xperience by Kentico must have an email address. Migration is only supported for users who have a unique email address value on the source instance.
	* Custom user fields can be migrated together with _modules classes_.
  * **Roles**
    * Only roles that have at least one user whose _Privilege level_ is set to _Editor_ and above are migrated.
    * Because Xperience by Kentico uses a different [permission model](https://docs.xperience.io/x/7IVwCg), no existing role permissions or UI personalization settings are migrated. After the migration, the permissions for each role must be configured again.
  * **Contacts**
    * Custom contact fields can be migrated together with _modules classes_.
  * **Activities**  
  * **Consents and consent agreements**
	* Only one culture version of consent texts is migrated, according to the culture selected during the migration.
  * **Modules and classes**
    * The migration includes the following:
	  * Custom modules
	  * All classes belonging under custom modules
	  * All data stored within custom module classes
	  * The following customizable system classes and their custom fields: _User_, _Media file_, _Contact management - Account_ (however, accounts are currently not supported in Xperience by Kentico), _Contact management - Contact_
	* Module and class migration does NOT include:
	  * UI elements and all related user interface settings. The administration of Xperience by Kentico uses a different technology stack than Kentico Xperience 13, and is incompatible. To learn how to build the administration UI, see [Extend the administration interface](https://docs.xperience.io/x/GwKQC) and [Example - Offices management application](https://docs.xperience.io/x/hIFwCg).
	  * Alternative forms under classes and UI-related configuration of class fields (field labels, Form controls, etc.). You need to manually create appropriate [UI forms](https://docs.xperience.io/x/V6rWCQ) in Xperience by Kentico after the migration.
	  * Custom settings under modules, which are are currently not supported in Xperience by Kentico
	  * Module permissions (permissions work differently in Xperience by Kentico, see [Role management](https://docs.xperience.io/x/7IVwCg) and [UI page permission checks](https://docs.xperience.io/x/8IKyCg))
	  * As with all object types, the migration toolkit does not transfer code files to the target project. You need to manually move all code files generated for your custom classes (_Info_, _InfoProvider_, etc.).
  * **Setting values**
    * Xperience by Kentico uses a sub-set of the settings available in Kentico Xperience 13. The migration only transfers the values of settings that exist in Xperience by Kentico.
  * **Countries and states**

### Unsupported data

The following types of data exist in Xperience by Kentico, but are currently **not supported** by the Migration toolkit: 

  * **Contact groups**
	* Static contact groups are currently not supported in Xperience by Kentico.
	* The condition format for dynamic contact groups is not compatible. To migrate contact groups: 
		1. Migrate your contacts using the toolkit.
		2. Create the [contact groups](https://docs.xperience.io/x/o4PWCQ) manually in Xperience by Kentico.
		3. Build equivalent conditions.
		4. Recalculate the contact groups.
  * **License keys**
    * Unnecessary, since Xperience by Kentico uses a new license key format.
 
 Additionally, object values or other content containing **Macros** will not work correctly after the migration. Macros in general are currently not supported for most data in Xperience by Kentico.
	

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
	```
	Migration.Toolkit.CLI.exe  migrate --siteId 1 --culture en-US --sites --users --settings-keys --page-types --pages --attachments --contact-management --forms --media-libraries --data-protection --countries
	```
8. Observe the command line output. The command output is also stored into a log file (`logs\log-<date>.txt` under the output directory by default), which you can review later.
9. Review the migration protocol, which provides information about the result of the migration, lists required manual steps, etc. 
	* You can find the protocol in the location specified by the `MigrationProtocolPath` key in the `appsettings.json` configuration file.
	* For more information, see [`Migration.Toolkit.CLI/MIGRATION_PROTOCOL_REFERENCE.md`](./Migration.Toolkit.CLI/MIGRATION_PROTOCOL_REFERENCE.md).

Data is now migrated to the target Xperience by Kentico instance according to your configuration. See [`Migration.Toolkit.CLI/README.md`](./Migration.Toolkit.CLI/README.md) for detailed information about the migration CLI, configuration options, instructions related to individual object types, and manual migration steps.

<!-- CONTRIBUTING -->
## Contributing

See [`CONTRIBUTING.md`](./CONTRIBUTING.md) to learn how to file issues, start discussions, and begin contributing.

When submitting issues, please provide all available information about the problem or error. If possible, include the command line output log file and migration protocol generated for your `Migration.Toolkit.CLI.exe migrate` command.

<!-- LICENSE -->
## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Questions & Support

See the [Kentico home repository](https://github.com/Kentico/Home/blob/master/README.md) for more information about the products and general advice on submitting questions.
