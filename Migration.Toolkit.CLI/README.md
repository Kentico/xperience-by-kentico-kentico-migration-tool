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
| `--page-types`              | Enables migration of [page types](https://docs.xperience.io/x/gYHWCQ) and [preset page templates](https://docs.xperience.io/x/KZnWCQ) (originally __custom page templates__ in Kentico Xperience 13). Required to migrate Pages.<br /><br />See: [Migration details for specific object types - Page types](#page-types)  | `--sites`              |
| `--pages`                   | Enables migration of [pages](https://docs.xperience.io/x/bxzfBw).<br /><br />The target instance must not contain pages other than those created by previous runs of the Migration toolkit. Requires the `--culture` parameter to be specified.<br /><br />See: [Migration details for specific object types - Pages](#pages) | `--sites`, `--users`, `--page-types` |
| `--attachments`             | Enables migration of page attachments to [media libraries](https://docs.xperience.io/x/agKiCQ) (page attachments are not supported in Xperience by Kentico).<br /><br />See: [Migration details for specific object types - Attachments](#attachments)   | `--sites`    |
| `--contact-management`      | Enables migration of [contacts](https://docs.xperience.io/x/nYPWCQ) and [activities](https://docs.xperience.io/x/oYPWCQ). The target instance must not contain any contacts or activities. May run for a long time depending on the number of contacts in the source database. |                        |
| `--data-protection`         | Enables migration of [consents](https://docs.xperience.io/x/zoB1CQ) and consent agreements. Requires the `--culture` parameter to be specified.   | `--sites`, `--users`, `--contact management`  |
| `--forms`                   | Enables migration of [forms](https://docs.xperience.io/x/WAKiCQ) and submitted form data.<br /><br />See: [Migration details for specific object types - Forms](#forms)  | `--sites`  |
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

#### Page types

**Troubleshooting**: The migration only includes page types that are assigned to the migrated site on the source instance. As a result, all page types that are used for pages in the content tree **must be assigned to the migrated site**. Otherwise, the migration will fail when transferring the given pages.

Xperience by Kentico currently does not support:
  * Macro expressions in page type field default values or other settings. Page type fields containing macros will not work correctly after the migration.
  * Page type inheritance. You cannot migrate page types that inherit fields from other types.
  * Categories for page type fields. Field categories are not migrated with page types.

The Migration toolkit attempts to map the _Data type_ and _Form control_ of page type fields to an appropriate equivalent in Xperience by Kentico. This is not always possible, and cannot be done for custom data types or form controls. We recommend that you check your page type fields after the migration and adjust them if necessary.
   
The following table describes how the Migration toolkit maps the data types and form controls/components of page type fields:

| KX13 Data type            | XbK Data type            | KX13 Form control             | XbK Form component    |
| ------------------------- | ------------------------ | ----------------------------- | --------------------- |
| Text                      | Text                     | Text box                      | Text input            |
| Text                      | Text                     | Drop-down list                | Dropdown selector     |
| Text                      | Text                     | Radio buttons                 | Radio button group    |
| Text                      | Text                     | Text area                     | Text area             |
| Text                      | Text                     | _other_                       | Text input            |
| Long text                 | Long text                | Rich text editor              | Rich text editor      |
| Long text                 | Long text                | Text box                      | Text input            |
| Long text                 | Long text                | Drop-down list                | Dropdown selector     |
| Long text                 | Long text                | Text area                     | Text area             |
| Long text                 | Long text                | _other_                       | Rich text editor      |
| Integer number            | Integer number           | _any_                         | Number input          |
| Long integer number       | Long integer number      | _any_                         | Number input          |
| Floating-point number     | Floating-point number    | _any_                         | Number input          |
| Decimal number            | Decimal number           | _any_                         | Decimal number input  |
| Date and time             | Date and time            | _any_                         | Datetime input        |
| Date                      | Date                     | _any_                         | Date input            |
| Time interval             | Time interval            | _any_                         | None (not supported)  |
| Boolean (Yes/No)          | Boolean (Yes/No)         | _any_                         | Checkbox              |
| Attachments               | Media files              | _any_ (Attachments)           | Media file selector<br />(the [attachments](#attachments) are converted to media files)    |
| File                      | Media files              | _any_ (Direct uploader)       | Media file selector<br />(the [attachments](#attachments) are converted to media files)    |
| Unique identifier (Guid)  | Unique identifier (Guid) | _any_                         | None (not supported)  |
| Pages                     | Pages                    | _any_ (Pages)                 | Page selector         |

Some [Form components](https://docs.xperience.io/x/5ASiCQ) used by page type fields in Xperience by Kentico store data differently than their equivalent Form control in Xperience 13. To ensure that content is displayed correctly on pages, you also need to manually adjust your website's implementation to match the new data format. See [Editing components in Xperience by Kentico](https://docs.xperience.io/x/wIfWCQ) to learn more about some of the most common components and selectors.

#### Pages

  * Xperience by Kentico currently does not support multilingual sites. The content of pages is migrated from the culture specified in the `migrate --culture` parameter.
  * Only pages that are **published** on the source instance are migrated. After the migration, all pages are in the published workflow step.
  * Migration includes the URL paths and Former URLs of pages, but not Alternative URLs, which are currently not supported in Xperience by Kentico.
  * Linked pages are currently not supported in Xperience by Kentico. The migration creates standard page copies for any linked pages on the source instance.
  * Page permissions (ACLs) are currently not supported in Xperience by Kentico, so are not migrated.
  
#### Page Builder content

By default, JSON data storing the Page Builder content of pages and custom page templates is migrated directly without modifications. On the target Xperience by Kentico instance, the migrated data can work in the Page Builder's legacy compatibility mode. However, we strongly recommend updating your codebase to the new Xperience by Kentico components. 

The Migration toolkit provides an advanced migration mode for Page Builder content that utilizes API discovery on the source instance. To learn more details and how to configure this feature, see [Source instance API discovery](#source-instance-api-discovery).

#### Media libraries

 * Media library permissions are currently not supported in Xperience by Kentico, so are not migrated.

#### Attachments

Page attachments are not supported in Xperience by Kentico. Attachment files are instead migrated into [media libraries](https://docs.xperience.io/x/agKiCQ).

 * Page attachments are migrated into a media library named: _"Attachments for site <sitename>"_ 
 * The media library contains folders matching the content tree structure for all pages with attachments (including empty folders for parent pages without attachments). The folders are named after the _node alias_ of the source pages.
   * Each page's folder directly contains all unsorted attachments (files added on the _Attachments_ tab in the Xperience 13 _Pages_ application).
   * Attachments stored in specific page fields are placed into subfolders, named in format: _"__fieldname"_. These subfolders can include multiple files for fields of the _Attachments_ type, or a single file for _File_ type fields.
 * Any "floating" attachments without an associated page are migrated into the media library root folder.
 * The migration does not include temporary attachments (created when a file upload is not finished correctly). If any are present on the source instance, a warning is logged in the [migration protocol](./MIGRATION_PROTOCOL_REFERENCE.md).

The following is an example of a media library created by the Migration toolkit for page attachments:

**Media library "Attachments for site DancingGoat"**
  * **Articles** (empty parent folder)
    * **Coffee-processing-techniques** (contains any unsorted attachments of the '/Articles/Coffee-processing-techniques' page)
      * **__Teaser** (contains attachments stored in the page's 'Teaser' field
    * **Which-brewing-fits-you**
      * **__Teaser**
	* ...

Additionally, any attachments placed into the content of migrated pages **will no longer work** in Xperience by Kentico. This includes images and file download links that use **/getattachment** and **/getimage** URLs. 

If you wish to continue using these legacy Kentico Xperience 13 attachment URLs, you need to add a custom handler to your Xperience by Kentico project. See [`Migration.Toolkit.KXP.Extensions/README.MD`](/Migration.Toolkit.KXP.Extensions/README.MD) for instructions.

#### Forms

The migration does not include the content of form autoresponder and notification emails. 

You can migrate form autoresponders to Xperience by Kentico manually by copying your HTML code and content into Email templates and Emails. See [Emails](https://docs.xperience.io/x/IaDWCQ).

#### Users

  * The 'administrator' user account is not updated by the migration, only transferred from the source if it does not exist on the target instance.
  * The 'public' system user is updated, and all bindings (e.g. the site binding) are mapped automatically on the target instance.
  * Site bindings are updated automatically for all migrated users.
  * Users in Xperience by Kentico must have an email address. Migration is only supported for users who have a **unique** email address value on the source instance.
  * The migration currently does not support custom user fields.
  * **Note**: Xperience by Kentico currently does not support registration and authentication of users on the live site. User accounts only control access to the administration interface.

#### Contacts

  * The migration currently does not support custom contact fields.
  * For performance reasons, contacts and related objects are migrated using bulk SQL queries. As a result, you always need to delete all Contacts, Activities and Consent agreements before running the migration (when using the `migrate --contact-management` parameter).
  

## Configuration

Before you run the migration, configure options in the `Migration.Toolkit.CLI/appsettings.json` file.

Add the options under the `Settings` section in the configuration file.

| Configuration                                            | Description                                                                                                          |
|----------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------|
| KxConnectionString                                       | The connection string to the source Kentico Xperience 13 database.                                                            |
| KxCmsDirPath                                             | The absolute file system path of the **CMS** folder in the source Kentico Xperience 13 administration project. Required to migrate media library files. |
| XbKConnectionString                                      | The connection string to the target Xperience by Kentico database.                                                               |
| XbKDirPath                                               | The absolute file system path of the root of the target Xperience by Kentico project. Required to migrate media library and page attachment files. |
| XbKApiSettings                                           | Configuration options set for the API when creating migrated objects in the target application.<br /><br />The `ConnectionStrings.CMSConnectionString`option is required - set the connection string to the target Xperience by Kentico database (the same value as `XbKConnectionString`).              |
| MigrationProtocolPath                                    | The absolute file system path of the location where the [migration protocol file](./MIGRATION_PROTOCOL_REFERENCE.md) is generated.<br /><br />For example: `"C:\\Logs\\Migration.Toolkit.Protocol.log"`               |
| MigrateOnlyMediaFileInfo                                 | If set to `true`, only the database representations of media files are migrated, without the files in the media folder in the project's file system. For example, enable this option if your media library files are mapped to a shared directory or Cloud storage.<br /><br />If `false`, media files are migrated based on the `KxCmsDirPath` location.  |
| UseOmActivityNodeRelationAutofix                         | Determines how the migration handles references from Contact management activities to non-existing pages.<br /><br />Possible options:<br />`DiscardData` - faulty references are removed,<br />`AttemptFix` - references are updated to the IDs of corresponding pages created by the migration,<br />`Error` - an error is reported and the reference can be translated or otherwise handled manually |
| UseOmActivitySiteRelationAutofix                         | Determines how the migration handles site references from Contact management activities.<br /><br />Possible options: `DiscardData`,`AttemptFix`,`Error` |
| EntityConfigurations                                           | Contains options that allow you to fine-tune the migration of specific object types.                 |
| EntityConfigurations.CMS_Site.ExplicitPrimaryKeyMapping.SiteID | **Required**. Maps the site ID (primary key) of the source site to the ID of the target site.        |
| EntityConfigurations.<object table name>.ExcludeCodeNames      | Excludes objects with the specified code names from the migration.                                   |
| OptInFeatures.QuerySourceInstanceApi.Enabled                   | If `true`, [source instance API discovery](#source-instance-api-discovery) is enabled to allow advanced migration of Page Builder content for pages and page templates. |
| OptInFeatures.QuerySourceInstanceApi.Connections               | To use [source instance API discovery](#source-instance-api-discovery), you need to add a connection JSON object containing the following values:<br />`SourceInstanceUri` - the base URI where the source instance's live site application is running.<br />`Secret` - the secret that you set in the _ToolkitApiController.cs_ file on the source instance.  |

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
    "KxConnectionString": "Data Source=myserver;Initial Catalog=Xperience13;Integrated Security=True;Persist Security Info=False;Connect Timeout=120;Encrypt=False;Current Language=English;",
    "KxCmsDirPath": "C:\\inetpub\\wwwroot\\Xperience13\\CMS",
    "XbKConnectionString": "Data Source=myserver;Initial Catalog=XperienceByKentico;Integrated Security=True;Persist Security Info=False;Connect Timeout=120;Encrypt=False;Current Language=English;",
    "XbKDirPath": "C:\\inetpub\\wwwroot\\XP_Target",    
    "XbKApiSettings": {
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
    },
    "OptInFeatures":{
        "QuerySourceInstanceApi": {
          "Enabled": true,
          "Connections": [
            { "SourceInstanceUri": "http://localhost:60527", "Secret": "__your secret string__" }
          ]
        }
    }
  }
}
```

## Source instance API discovery

By default, JSON data storing the Page Builder content of pages and custom page templates is migrated directly without modifications. Within this content, Page Builder components (widgets, sections, etc.) with properties have their configuration based on Kentico Xperience 13 form components, which are assigned to the properties on the source instance. On the target Xperience by Kentico instance, the migrated data can work in the Page Builder's legacy compatibility mode.

However, we strongly recommend updating your codebase to the new Xperience by Kentico components. See [Editing components in Xperience by Kentico](https://docs.xperience.io/x/wIfWCQ) to learn more.

To convert Page Builder data to a format suitable for the Xperience by Kentico components, the Migration toolkit provides an advanced migration mode that utilizes API discovery on the source instance. The advanced mode currently provides the following data conversion:
 * **Attachment selector** properties - converted to a format suitable for the Xperience by Kentico **Media selector** component, with `IEnumerable<AssetRelatedItem>` values.
 * **Page selector** properties - converted to a format suitable for the Xperience by Kentico Page selector component, with `IEnumerable<PageRelatedItem>` values.

### Prerequisites and Limitations

* To use source instance API discovery, the live site application of your source instance must be running and available during the migration.
* Using the advanced Page Builder data migration **prevents the data from being used in the Page Builder's legacy compatibility mode**. With this approach, you need to update all Page Builder component code files to the [Xperience by Kentico format](https://docs.xperience.io/x/wIfWCQ).
* The source instance API discovery feature only processes component properties defined using `[EditingComponent]` attribute notation. Other implementations, such as properties edited via custom view components in the cshtml view, are not supported.
```csharp
public class MyWidgetProperties : IWidgetProperties
{
    // Supported
    [EditingComponent(PageSelector.IDENTIFIER, Label = "Selected products", Order = 1)]    
    public IEnumerable<PageSelectorItem> SelectedProducts { get; set; } = new List<PageSelectorItem>();

    // NOT supported
    public IEnumerable<MediaFilesSelectorItem> Images { get; set; } = new List<MediaFilesSelectorItem>();
}
```

### API discovery setup

1. Copy the `ToolkitApiController.cs` file to the `Controllers` folder in the **live site project** of your Kentico Xperience 13 source instance. Get the file from the following location in the Migration toolkit repository:
	* For .NET Core projects: `KX13.Extensions\ToolkitApiController.cs`
	* For MVC 5 (.NET Framework 4.8) projects: `KX13.NET48.Extensions\ToolkitApiController.cs`
2. Register routes for the `ToolkitApi` controller's actions into the source instance's live site application.
 * For .NET Core projects, add endpoints in the project's `Startup.cs` or `Program.cs` file:
```csharp
app.UseEndpoints(endpoints =>
{	
    endpoints.MapControllerRoute(
        name: "ToolkitExtendedFeatures",
        pattern: "{controller}/{action}",
        constraints: new
        {
            controller = "ToolkitApi"
        }
    );	

    // other routes ...
});
```
 * For MVC 5 projects, map the routes in your application's `RouteCollection` (e.g., in the `/App_Start/RouteConfig.cs` file):
```csharp
public static void RegisterRoutes(RouteCollection routes)
{
    // Maps routes for Xperience handlers and enabled features
    routes.Kentico().MapRoutes()
	
    routes.MapRoute(
        name: "ToolkitExtendedFeatures",
        url: "{controller}/{action}",
        defaults: new { },
        constraints: new
        {   
            controller = "ToolkitApi"
        }
    );
	
    // other routes ...
}
```
3. Edit `ToolkitApiController.cs` and set a value for the `Secret` constant:
```csharp
private const string Secret = "__your secret string__";
```
4. Configure the `Settings.OptInFeatures.QuerySourceInstanceApi` [configuration options](#configuration) for the Migration toolkit:
```json
"OptInFeatures":{
  "QuerySourceInstanceApi": {
    "Enabled": true,
    "Connections": [
      { "SourceInstanceUri": "http://localhost:60527", "Secret": "__your secret string__" }
    ]
  }
},
```

You can test the source instance API discovery by making a POST request to `<source instance live site URI>/ToolkitApi/Test` with `{ "secret":"__your secret string__" }` in the body. If your setup is correct, the response should be: `{ "pong": true }`

When you now [Migrate data](#migrate-data), the toolkit performs API discovery of Page Builder component code on the source instance and advanced migration of Page Builder data.
