## Migration CLI

The [Migration toolkit](/README.md) transfers content and other data from **Kentico Xperience 13** to **Xperience by Kentico**.

The migration is performed by running a command for the .NET CLI.

## Set up the target instance

The target of the migration must be an Xperience by Kentico instance that fulfills the following requirements:
  * The instance's database and file system must be accessible from the environment where you run the migration.
  * The target application *must not be running* when you start the migration.
  * The instance must not contain any data except for an empty site and/or data from the source site created by previous runs of the Migration toolkit.
	* When running the migration for the first time, the content tree must be empty except for the root page (of the `CMS.Root` type).
    * For performance optimization, the migration transfers certain objects using bulk SQL queries. As a result, you always need to delete all objects of the following types before running repeated migrations:
	  * **Contacts**, including their **Activities** and **Consent agreements** (when using the `migrate --contact-management` parameter)
	  * **Form submissions** (when using the `migrate --forms` parameter)

To create a suitable target instance, [install a new Xperience by Kentico project](https://docs.xperience.io/x/DQKQC) using the **Boilerplate** project template.


## Migrate data

To perform the migration:

  1. Make sure the [target instance is set up correctly](#set-up-the-target-instance).
  2. [Configure](#configuration) the options in the `Migration.Toolkit.CLI` project's `appsettings.json` file.
  3. Compile the `Migration.Toolkit.CLI` project.
  4. Open the command line prompt.
  5. Navigate to the project's output directory.
  6. Run the `Migration.Toolkit.CLI.exe migrate` command with parameters according to your requirements.
  7. Observe the command line output and review the [migration protocol](./MIGRATION_PROTOCOL_REFERENCE.md), which provides information about the result of the migration, lists required manual steps, etc.

### Migrate command parameters

Command usage:
```
Migration.Toolkit.CLI.exe migrate --siteId 1 --culture en-US --sites --users
````

| Parameter                   | Description                                              |  Dependencies                         |
|-----------------------------|----------------------------------------------------------|---------------------------------------|
| `--siteId <siteId>`         | **Required**. Specifies the ID of the site on the source instance. You can find the ID in the source database by running the `SELECT * FROM dbo.CMS_Site` query and checking the value of the `SiteID` column. |                                           |
| `--culture <cultureCode>`   | Specifies the culture code from which content is migrated, for example _en-US_. Multilingual migration is currently not supported.  |    |
| `--sites`                   | Enables migration of the [site](https://docs.xperience.io/x/34HFC). The site's basic properties are transferred to the target instance. Requires the `siteId` parameter to be specified. |                                  |
| `--users`                   | Enables migration of [users](https://docs.xperience.io/x/8ILWCQ).<br /><br />See: [Migration details for specific object types - Users](#users) | `--sites`                             |
| `--settings-keys`           | Enables migration of values for [settings](https://docs.xperience.io/x/7YjFC) that are available in Xperience by Kentico. | `--sites`                             |
| `--page-types`              | Enables migration of [page types](https://docs.xperience.io/x/gYHWCQ). Required to migrate Pages.  | `--sites`              |
| `--pages`                   | Enables migration of [pages](https://docs.xperience.io/x/bxzfBw).<br /><br />The target instance must not contain pages other than those created by previous runs of the Migration toolkit. Requires the `--culture` parameter to be specified.<br /><br />See: [Migration details for specific object types - Page types and Pages](#page-types-and-pages) | `--sites`, `--users`, `--page-types` |
| `--attachments`             | Enables migration of page attachments to [media libraries](https://docs.xperience.io/x/agKiCQ) (page attachments are not supported in Xperience by Kentico).<br /><br />See: [Migration details for specific object types - Attachments](#attachments)   | `--sites`    |
| `--contact-management`      | Enables migration of [contacts](https://docs.xperience.io/x/nYPWCQ) and [activities](https://docs.xperience.io/x/oYPWCQ). The target instance must not contain any contacts or activities. May run for a long time depending on the number of contacts in the source database. |                        |
| `--data-protection`         | Enables migration of [consents](https://docs.xperience.io/x/zoB1CQ) and consent agreements. Requires the `--culture` parameter to be specified.   | `--sites`, `--users`, `--contact management`  |
| `--forms`                   | Enables migration of [forms](https://docs.xperience.io/x/WAKiCQ) and submitted form data.  | `--sites`                            |
| `--media-libraries`         | Enables migration of [media libraries](https://docs.xperience.io/x/agKiCQ) and contained media files. The actual binary files are only migrated between file systems if the `MigrateOnlyMediaFileInfo` [configuration option](#configuration) is set to _false_). | `--sites`, `--users` |
| `--countries`               | Enables migration of countries and states. Xperience by Kentico currently uses countries and states to fill selectors when editing contacts and contact group conditions. |   |
| `--bypass-dependency-check` | Skips the migrate command's dependency check. Use for repeated runs of the migration if you know that dependencies were already migrated successfully (for example `--page types` when migrating pages).  |       | 


### Examples

  * `Migration.Toolkit.CLI.exe migrate --siteId 1 --culture en-US --sites --users --settings-keys --media-libraries --page-types --pages`
    * Migration including the site object, users, setting key values, media libraries, page types and pages
  * `Migration.Toolkit.CLI.exe migrate --siteId 1 --culture en-US --page-types --pages  --bypass-dependency-check`
    * Repeated migration only for page types and pages, if you know that sites and users were already migrated successfully.
  * `Migration.Toolkit.CLI.exe migrate --siteId 1 --culture en-US --pages --bypass-dependency-check`
    * Repeated migration only for pages, if you know that page types, sites and users were already migrated successfully.

### Migration details for specific object types

#### Page types and Pages

  * Xperience by Kentico currently does not support multilingual sites. The content of pages is migrated from the culture specified in the `migrate --culture` parameter.
  * Only pages that are **published** on the source instance are migrated. After the migration, all pages are in the published workflow step.
  * Migration includes the URL paths and Former URLs of pages, but not Alternative URLs, which are currently not supported in Xperience by Kentico.

Writing in Progress

#### Attachments

Page attachments are not supported in Xperience by Kentico. Attachments are migrated into media libraries.

Writing in Progress

#### Users

  * The 'administrator' user account is not updated by the migration, only transferred from the source if it does not exist on the target instance.
  * The 'public' system user is updated, and all bindings (e.g. the site binding) are mapped automatically on the target instance.
  * Site bindings are updated automatically for all migrated users.
  * **Note**: Xperience by Kentico currently does not support registration and authentication of users on the live site. User accounts only control access to the administration interface.

## Configuration

Before you run the migration, configure options in the `Migration.Toolkit.CLI/appsettings.json` file.

Add the options under the `Settings` section in the configuration file.

| Configuration                                                           | Description                                                                                                          |
|-----------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------|
| SourceConnectionString                                         | The connection string to the source Kentico Xperience 13 database.                                                            |
| SourceCmsDirPath                                               | The absolute file system path of the **CMS** folder in the source Kentico Xperience 13 administration project. Required to migrate media library files. |
| TargetConnectionString                                         | The connection string to the target Xperience by Kentico database.                                                               |
| TargetCmsDirPath                                               | The absolute file system path of the root of the target Xperience by Kentico project. Required to migrate media library and page attachment files. |
| TargetKxpApiSettings                                           | Configuration options set for the API when creating migrated objects in the target application.<br /><br />The `ConnectionStrings.CMSConnectionString`option is required - set the connection string to the target Xperience by Kentico database (the same value as `TargetConnectionString`).              |
| MigrationProtocolPath                                          | The absolute file system path of the location where the [migration protocol file](./MIGRATION_PROTOCOL_REFERENCE.md) is generated.<br /><br />For example: `"C:\\Logs\\Migration.Toolkit.Protocol.log"`                       |
| MigrateOnlyMediaFileInfo                                       | If set to `true`, only the database representations of media files are migrated, without the files in the media folder in the project's file system. For example, enable this option if your media library files are mapped to a shared directory or Cloud storage.<br /><br />If `false`, media files are migrated based on the `SourceCmsDirPath` location.  |
| UseOmActivityNodeRelationAutofix                               | Determines how the migration handles references from Contact management activities to non-existing pages.<br /><br />Possible options:<br />`DiscardData` - faulty references are removed,<br />`AttemptFix` - references are updated to the IDs of corresponding pages created by the migration,<br />`Error` - an error is reported and the reference can be translated or otherwise handled manually                                                             |
| UseOmActivitySiteRelationAutofix                               | Determines how the migration handles site references from Contact management activities.<br /><br />Possible options: `DiscardData`,`AttemptFix`,`Error` |
| EntityConfigurations                                           | Contains options that allow you to fine-tune the migration of specific object types.                                                   |
| EntityConfigurations.CMS_Site.ExplicitPrimaryKeyMapping.SiteID | **Required**. Maps the site ID (primary key) of the source site to the ID of the target site.                      |
| EntityConfigurations.<object table name>.ExcludeCodeNames      | Excludes objects with the specified code names from the migration.                                   |

### Example
	
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "System": "Warning",
      "Microsoft": "Warning"
    },
    "MinimumLevel": {
      "Default": "Information",
      "System": "Warning",
      "Microsoft": "Warning"
    },
    "pathFormat": "logs/log.txt"
  },
  "Settings": {
    "SourceConnectionString": "Data Source=myserver;Initial Catalog=Xperience13;Integrated Security=True;Persist Security Info=False;Connect Timeout=120;Encrypt=False;Current Language=English;",
    "SourceCmsDirPath": "C:\\inetpub\\wwwroot\\Xperience13\\CMS",
    "TargetConnectionString": "Data Source=myserver;Initial Catalog=XperienceByKentico;Integrated Security=True;Persist Security Info=False;Connect Timeout=120;Encrypt=False;Current Language=English;",
    "TargetCmsDirPath": "C:\\inetpub\\wwwroot\\XP_Target",    
    "TargetKxpApiSettings": {
      "ConnectionStrings": {
        "CMSConnectionString": "Data Source=myserver;Initial Catalog=XperienceByKentico;Integrated Security=True;Persist Security Info=False;Connect Timeout=120;Encrypt=False;Current Language=English;"
      }
    },
	"MigrationProtocolPath" : "C:\\_Development\\xperience-migration-toolkit-master\\Migration.Toolkit.Protocol.log",
	"MigrateOnlyMediaFileInfo": false,
    "UseOmActivityNodeRelationAutofix": "AttemptFix",
    "UseOmActivitySiteRelationAutofix": "AttemptFix",
    "EntityConfigurations": {
      "CMS_Site": {
        "ExplicitPrimaryKeyMapping": {
          "SiteID": {
            "1": 1
          }
        }
      },
      "CMS_Class": {
        "ExcludeCodeNames": [
          "CMS.File", 
          "CMS.MenuItem",
          "ACME.News",
          "ACME.Office",
          "CMS.Blog",
          "CMS.BlogPost"
        ]
      },
      "CMS_SettingsKey": {
        "ExcludeCodeNames": [
          "CMSHomePagePath"
        ]
      }
    }
  }
}
```
