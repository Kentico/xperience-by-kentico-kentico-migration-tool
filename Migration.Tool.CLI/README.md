# Migration CLI

The [Xperience by Kentico: Kentico Migration Tool](/README.md) transfers content and other data from **Kentico Xperience
13**, **Kentico 12** or **Kentico 11** to **Xperience by Kentico**.

The migration is performed by running a command for the .NET CLI.

## Set up the source instance

The source instance must **not** use a [separated contact management database](https://docs.kentico.com/x/4giRBg), it is recommended that you [rejoin the contact management database](https://docs.kentico.com/x/5giRBg) before proceeding with the migration.

If you are migrating from Kentico Xperience 13, remember to [update your source instance to Refresh 5 or higher](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq#do-i-have-to-update-my-kx13-site-before-migration).

## Set up the target instance

The target of the migration must be an Xperience by Kentico instance that fulfills the following requirements:

- The instance's database and file system must be accessible from the environment where you run the migration.
- The target application _must not be running_ when you start the migration.
- The target instance must be empty except for data from the source instance created by previous runs of this tool.
- For performance optimization, the migration transfers certain objects using bulk SQL queries. As a result, you always
  need to delete all objects of the following types before running repeated migrations:
  - **Contacts**, including their **Activities** (when using the `migrate --contact-management` parameter)
  - **Consent agreements** (when using the `migrate --data-protection` parameter)
  - **Form submissions** (when using the `migrate --forms` parameter)
  - **Custom module class data** (when using the `--custom-modules` parameter)

To create a suitable target instance, [install a new Xperience by Kentico project](https://docs.xperience.io/x/DQKQC)
using the **Boilerplate** project template.

## Migrate data

To perform the migration:

1. Make sure the [target instance is set up correctly](#set-up-the-target-instance).
2. [Configure](#configuration) the options in the `Migration.Tool.CLI` project's `appsettings.json` file.
3. Compile the `Migration.Tool.CLI` project.
4. Open the command line prompt.
5. Navigate to the project's output directory.
6. Run the `Migration.Tool.CLI.exe migrate` command with parameters according to your requirements.
7. Observe the command line output and review the [migration protocol](./MIGRATION_PROTOCOL_REFERENCE.md), which
   provides information about the result of the migration, lists required manual steps, etc.
8. On SaaS projects, you need to manually move content item asset files. See [Content items](#content-items) for more information.

### Migrate command parameters

Command usage:

```powershell
Migration.Tool.CLI.exe migrate --sites --custom-modules --users --members --forms --media-libraries --page-types --pages --settings-keys --contact-management --data-protection
```

| Parameter                   | Description                                                                                                                                                                                                                                                                                                                                                                             | Dependencies                                   |
| --------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| `--sites`                   | Enables migration of sites to [website channels](https://docs.xperience.io/x/34HFC). The site's basic properties and settings are transferred to the target instance.                                                                                                                                                                                                                   |                                                |
| `--custom-modules`          | Enables migration of custom modules, [custom module classes and their data](https://docs.xperience.io/x/AKDWCQ), and [custom fields in supported system classes](https://docs.xperience.io/x/V6rWCQ).<br /><br />See: [Migration details for specific object types - Custom modules and classes](#custom-modules-and-classes)                                                           | `--sites`                                      |
| `--custom-tables`           | Enables migration of [custom tables](https://docs.kentico.com/x/eQ2RBg). Custom table data can be migrated to either [custom module classes](https://docs.kentico.com/x/AKDWCQ) (default behavior) or [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub.<br /><br />See: [Migration details for specific object types - Custom tables](#custom-tables)                                                                                                                                                                                                                  |                                                |
| `--users`                   | Enables migration of [users](https://docs.xperience.io/x/8ILWCQ) and [roles](https://docs.xperience.io/x/7IVwCg).<br /><br />See: [Migration details for specific object types - Users](#users)                                                                                                                                                                                         | `--sites`, `--custom-modules`                  |
| `--members`                 | Enables migration of live site user accounts to [members](https://docs.xperience.io/x/BIsuCw). <br /><br />See: [Migration details for specific object types - Members](#members)                                                                                                                                                                                                       | `--sites`, `--custom-modules`                  |
| `--settings-keys`           | Enables migration of values for [settings](https://docs.xperience.io/x/7YjFC) that are available in Xperience by Kentico.                                                                                                                                                                                                                                                               | `--sites`                                      |
| `--page-types`              | Enables migration of [content types](https://docs.xperience.io/x/gYHWCQ) (originally _page types_ in Kentico Xperience 13) and [preset page templates](https://docs.xperience.io/x/KZnWCQ) (originally _custom page templates_). Required to migrate Pages.<br /><br />See: [Migration details for specific object types - Content types](#content-types)                               | `--sites`                                      |
| `--pages`                   | Enables migration of [pages](https://docs.kentico.com/x/AQqRBg). Pages can be migrated either to [website channel pages](https://docs.kentico.com/x/JwKQC) (default behavior) or [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub. <br /><br />When migrating to website channel pages, the target instance must not contain pages other than those created by previous runs of the Kentico Migration Tool. <br /><br />See: [Migration details for specific object types - Pages](#pages)                                                                                                     | `--sites`, `--users`, `--page-types`           |
| `--type-restrictions`       | Enables migration of [content type restrictions](https://docs.kentico.com/x/bw2RBg).                                                                                                                                                                                           | `--sites`, `--pagetypes`, `--pages`            |
| `--categories`              | Enables migration of categories to taxonomies. Xperience by Kentico uses a different approach to categorization. Categories are migrated to [taxonomies](https://docs.kentico.com/x/taxonomies_xp) and selected categories for each page are assigned to pages in the target instance via a [reusable field schema](https://docs.kentico.com/x/D4_OD). See [`Categories`](#categories). | `--sites`, `--users`, `--pagetypes`, `--pages` |
| `--contact-management`      | Enables migration of [contacts](https://docs.xperience.io/x/nYPWCQ) and [activities](https://docs.xperience.io/x/oYPWCQ). The target instance must not contain any contacts or activities. May run for a long time depending on the number of contacts in the source database.                                                                                                          | `--users`, `--custom-modules`                  |
| `--data-protection`         | Enables migration of [consents](https://docs.xperience.io/x/zoB1CQ) and consent agreements.                                                                                                                                                                                                                                                                                             | `--sites`, `--users`, `--contact management`   |
| `--forms`                   | Enables migration of [forms](https://docs.xperience.io/x/WAKiCQ) and submitted form data.<br /><br />See: [Migration details for specific object types - Forms](#forms)                                                                                                                                                                                                                 | `--sites`, `--custom-modules`, `--users`       |
| `--media-libraries`         | Enables migration of [media libraries](https://docs.xperience.io/x/agKiCQ) to Content hub as [content item assets](https://docs.kentico.com/x/content_item_assets_xp). This behavior can be adjusted by `MigrateOnlyMediaFileInfo` and `MigrateMediaToMediaLibrary` [configuration options](#configuration).                                                                                            | `--sites`, `--custom-modules`, `--users`       |
| `--countries`               | Enables migration of countries and states. Xperience by Kentico currently uses countries and states to fill selectors when editing contacts and contact group conditions.                                                                                                                                                                                                               |                                                |
| `--bypass-dependency-check` | Skips the migrate command's dependency check. Use for repeated runs of the migration if you know that dependencies were already migrated successfully (for example `--page types` when migrating pages).                                                                                                                                                                                |                                                |

### Examples

- `Migration.Tool.CLI.exe migrate --sites --custom-modules --users --settings-keys --media-libraries --page-types --pages`
  - First migration that includes the site object, custom modules and classes, users, setting key values, media
    libraries, page types and pages
- `Migration.Tool.CLI.exe migrate --page-types --pages --bypass-dependency-check`
  - Repeated migration only for page types and pages, if you know that sites and users were already migrated
    successfully.
- `Migration.Tool.CLI.exe migrate --pages --bypass-dependency-check`
  - Repeated migration only for pages, if you know that page types, sites, and users were already migrated
    successfully.

> [!TIP]
> Refer to our [FAQ page](https://docs.kentico.com/guides/architecture/upgrade-from-kx13/upgrade-faq#can-i-run-the-kentico-migration-tool-against-my-project-multiple-times) for best practices of performing repeated (iterative) data migration.

### Migration details for specific object types

#### Content types

Content types are named **Page types** in earlier Kentico products.

Xperience by Kentico currently does not support:

- Macro expressions in page type field default values or other settings. Content type fields containing macros will not
  work correctly after the migration.
- Page type inheritance. You cannot migrate page types that inherit fields from other types.
- Categories for page type fields. Field categories are not migrated with page types.

The Kentico Migration Tool attempts to map the _Data type_ and _Form control_ of page type fields to an appropriate
equivalent in Xperience by Kentico. This is not always possible, and cannot be done for custom data types or form
controls. We recommend that you check your content type fields after the migration and adjust them if necessary.

The following table describes how the Kentico Migration Tool maps the data types and form controls/components of page
type fields:

| KX13/12/11 Data type     | XbyK Data type           | KX13/12/11 Form control | XbyK Form component                                                                     |
| ------------------------ | ------------------------ | ----------------------- | --------------------------------------------------------------------------------------- |
| Text                     | Text                     | Text box                | Text input                                                                              |
| Text                     | Text                     | Drop-down list          | Dropdown selector                                                                       |
| Text                     | Text                     | Radio buttons           | Radio button group                                                                      |
| Text                     | Text                     | Text area               | Text area                                                                               |
| Text                     | Text                     | _other_                 | Text input                                                                              |
| Long text                | Long text                | Rich text editor        | Rich text editor                                                                        |
| Long text                | Long text                | Text box                | Text input                                                                              |
| Long text                | Long text                | Drop-down list          | Dropdown selector                                                                       |
| Long text                | Long text                | Text area               | Text area                                                                               |
| Long text                | Long text                | _other_                 | Rich text editor                                                                        |
| Integer number           | Integer number           | _any_                   | Number input                                                                            |
| Long integer number      | Long integer number      | _any_                   | Number input                                                                            |
| Floating-point number    | Floating-point number    | _any_                   | Number input                                                                            |
| Decimal number           | Decimal number           | _any_                   | Decimal number input                                                                    |
| Date and time            | Date and time            | _any_                   | Datetime input                                                                          |
| Date                     | Date                     | _any_                   | Date input                                                                              |
| Time interval            | Time interval            | _any_                   | None (not supported)                                                                    |
| Boolean (Yes/No)         | Boolean (Yes/No)         | _any_                   | Check box                                                                               |
| Attachments              | Media files              | _any_ (Attachments)     | Media file selector<br />(the [attachments](#attachments) are converted to media files) |
| File                     | Media files              | _any_ (Direct uploader) | Media file selector<br />(the [attachments](#attachments) are converted to media files) |
| Unique identifier (Guid) | Unique identifier (Guid) | _any_                   | None (not supported)                                                                    |
| Pages                    | Pages                    | _any_ (Pages)           | Page selector                                                                           |

Additionally, you can enable the Conversion of text fields with media links (_Media selection_ form control) to content item assets or media
library files by setting
the `OptInFeatures.CustomMigration.FieldMigrations` [configuration option](#convert-text-fields-with-media-links). If you need additional control or want to customize the default mappings of data types, you can [customize the Migration Tool behavior](../Migration.Tool.Extensions/README.md).

Some [Form components](https://docs.xperience.io/x/5ASiCQ) used by content type fields in Xperience by Kentico store
data differently than their equivalent Form control in Xperience 13. To ensure that content is displayed correctly on
pages, you must manually adjust your website's implementation to match the new data format.
See [Editing components in Xperience by Kentico](https://docs.xperience.io/x/wIfWCQ) to learn more about some of the
most common components and selectors.

#### Reusable field schemas

You can create [reusable field schemas](https://docs.kentico.com/x/D4_OD) from page types from which other page types
inherit, by setting
the `Settings.CreateReusableFieldSchemaForClasses` configuration option. See [Convert page types to reusable field schemas](#convert-page-types-to-reusable-field-schemas) for detailed information.

Additionally, you can use the extensibility feature to implement [customizations](../Migration.Tool.Extensions/README.md) that allow you to inject reusable field schemas into content types or [extract fields from multiple page types into a shared schema](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides). Note that usage of `ReusableSchemaBuilder` in custom class mappings cannot be combined together with the `Settings.CreateReusableFieldSchemaForClasses` configuration option.

#### Content items

If the target instance is a [SaaS project](https://docs.kentico.com/x/saas_xp) ([installed](https://docs.kentico.com/x/DQKQC) with the `--cloud` option) you need to manually move any content item asset binary files from the default location (`~/assets`) to the location specified in the `StorageInitializationModule.cs` file, which is `~/$StorageAssets/default/assets` by default. This is necessary to enable the system to map the asset binary files to the [Azure Blob storage](https://docs.kentico.com/x/5IfWCQ).

#### Pages

Pages from older product versions can be migrated to either to [website channel pages](https://docs.kentico.com/x/JwKQC) (default behavior) or [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub. See [Convert pages or custom tables to Content hub](#convert-pages-or-custom-tables-to-content-hub) for detailed information.

- The migration includes the following versions of pages:
  - _Published_
  - _Latest draft version_ - for published pages, the version is migrated to the
    _Draft (New version)_ [status](https://docs.xperience.io/x/JwKQC); for pages that do not have a
    published version, the version is migrated to the _Draft (Initial)_ status.
  - _Archived_
- Page URLs are included only when migrating to [website channel pages](https://docs.kentico.com/x/JwKQC) (default behavior). URL migration depends on the source instance version:
  - For Kentico Xperience 13, the migration includes the URL paths of pages and Former URLs.
  - For Kentico 12 and Kentico 11, URL paths are not migrated. Instead, a default URL path is created from
    the `DocumentUrlPath` or `NodeAliasPath`.
  - For Kentico Xperience 13 and Kentico 12, [Alternative URLs](https://docs.kentico.com/13/managing-website-content/working-with-pages/managing-page-urls#alternative-urls) are migrated to [Vanity URLs](https://docs.kentico.com/documentation/business-users/website-content/manage-page-urls#manage-vanity-urls-of-pages).
- Linked pages are currently not supported in Xperience by Kentico. By default, the migration creates standard page copies for any
  linked pages on the source instance. This behavior can be changed by implementing [custom handling of linked pages](../Migration.Tool.Extensions/README.md#customize-linked-page-handling).
- Page permissions (ACLs) are currently not migrated into Xperience by Kentico.
- Migration of page builder content is only available for Kentico Xperience 13.

Additionally, you can define [custom migrations](../Migration.Tool.Extensions/README.md) to change the default behavior, for example to migrate page content to widgets in Xperience by Kentico.

#### Page builder content

> :warning: Page builder content migration is only available when migrating from Kentico Xperience 13.

By default, JSON data storing the page builder content of pages and custom page templates is migrated directly without
modifications. On the target Xperience by Kentico instance, the migrated data can work in the Page Builder's legacy
compatibility mode. However, we strongly recommend updating your codebase to the new Xperience by Kentico components.

The Kentico Migration Tool provides an advanced migration mode for page builder content that utilizes API discovery on
the source instance. To learn more details and how to configure this feature,
see [Source instance API discovery](#source-instance-api-discovery).

Additionally, you can define [custom migrations of widgets and widget properties](../Migration.Tool.Extensions/README.md) to change the default behavior and migrate widget content to Xperience by Kentico components.

#### Categories

Xperience by Kentico uses a different approach to categorization than older product versions. [Categories](https://docs.kentico.com/x/wA_RBg) were replaced by [taxonomies](https://docs.kentico.com/x/taxonomies_xp) and selected categories for each page are assigned to pages in the target instance via a [reusable field schema](https://docs.kentico.com/x/D4_OD). The key differences are:

- Categories in older versions can be added to any page via the _Properties -> Categories_ tab. Taxonomies can only be
  used for content items (pages, emails...) that have a field with the _Taxonomy_ data type.
- Categories can be global or site-specific. Taxonomies are always global, as there are no sites in Xperience by
  Kentico.
- Categories are assigned to pages regardless of their workflow step. Taxonomies are stored as a field and are covered
  by workflow. As a result, assigned tags can be different in each workflow step.
- [Categories stored as a field](https://docs.kentico.com/x/wA_RBg)
  and [personal categories](https://docs.kentico.com/x/IgqRBg) are not supported by the migration.

The migration process for categories performs the following steps:

1. A new [taxonomy](https://docs.kentico.com/x/taxonomies_x) named **Categories** (code
   name `categories`) is created to house all categories from the source instance.
2. A new [reusable field schema](https://docs.kentico.com/x/D4_OD) named **Categories container** (code
   name `categories_container`) is created to allow linking tags to pages.

   - The schema contains one field, **Categories_Legacy** (data type **Taxonomy**, configured to enable selection from the _Categories_ taxonomy).

3. On the target instance, the _Categories container_ reusable field schema is added to all content types where at least
   one page had a category assigned in the source instance.
4. Supported categories from the source instance are migrated as tags to the _Categories_ taxonomy in the target
   instance. The category hierarchy from the source instance is maintained in the target instance.
5. On the target instance, tags are assigned to pages according to the source instance.

- Each [language variant](https://docs.kentico.com/x/yRT_Cw) of a page is treated
  individually and receives its corresponding group of tags based on the source instance.
- Tags from the source page are added to all
  available [workflow steps](https://docs.kentico.com/x/workflows_xp) of the target page.

#### Custom modules and classes

The migration includes the following:

- Custom modules
  - Note: The `CMS.` prefix/namespace is reserved for system modules and not allowed in custom module code names. If
    present, this code name prefix is removed during the migration.
- All classes belonging under custom modules
- All data stored within custom module classes
- The following customizable system classes and their custom fields:
  - _Membership > User_
  - _Media libraries > Media file_
  - _Contact management > Contact management - Account_ (however, accounts are currently not supported in Xperience by
    Kentico)
  - _Contact management > Contact management - Contact_
- You can customize the default migration of fields using the [extensibility feature](../Migration.Tool.Extensions/README.md).

Module and class migration does NOT include:

- UI elements and all related user interface settings. The administration of Xperience by Kentico uses a different
  technology stack than Kentico Xperience 13 and is incompatible. To learn how to build the administration UI,
  see [Extend the administration interface](https://docs.xperience.io/x/GwKQC)
  and [Example - Offices management application](https://docs.xperience.io/x/hIFwCg).
- Alternative forms under classes and UI-related configuration of class fields (field labels, Form controls, etc.). You
  need to manually create appropriate [UI forms](https://docs.xperience.io/x/V6rWCQ) in Xperience by Kentico after the
  migration.
- Custom settings under modules, which are currently not supported in Xperience by Kentico
- Module permissions (permissions work differently in Xperience by Kentico,
  see [Role management](https://docs.xperience.io/x/7IVwCg)
  and [UI page permission checks](https://docs.xperience.io/x/8IKyCg))

Custom module classes and the stored data records are either migrated as module classes in Xperience by Kentico (default behavior) or [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub. See [Convert module classes to Content hub](#convert-module-classes-to-content-hub) for detailed information.

As with all object types, the Kentico Migration Tool does not transfer code files to the target project. You need to
manually move all code files generated for your custom classes (_Info_, _InfoProvider_, etc.).

To learn more about custom modules and classes in Xperience by Kentico, see
the [Object types](https://docs.xperience.io/x/AKDWCQ) documentation.

#### Custom tables

The migration includes the following:

- Basic information about custom tables (from the `CMS_Class` table) is migrated to the custom module
  table (`CMS_Resource`) as a special `customtables` resource.
- Content of individual custom tables is migrated to either [custom module classes](https://docs.kentico.com/x/AKDWCQ) (default behavior) or [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub. See [Convert pages or custom tables to Content hub](#convert-pages-or-custom-tables-to-content-hub) for detailed information.

Custom table migration does NOT include:

- Any other data related to custom tables (queries, alternative forms) are discarded by the migration.
- UI elements related to custom tables such as listings and filters are not migrated and need to be implemented. The
  administration of Xperience by Kentico uses a different technology stack than Kentico Xperience 13 and is
  incompatible. To learn how to build the administration UI,
  see [Extend the administration interface](https://docs.xperience.io/x/GwKQC)
  and [Example - Offices management application](https://docs.xperience.io/x/hIFwCg).

#### Media libraries

Media library files are migrated as [content item assets](https://docs.kentico.com/x/content_item_assets_xp) to Content hub into a content folder `<site_name>/<library_folder>`. All assets are created in the default language of the respective site. Migrated assets are created as content items of a _Legacy media file_ content type (code name `Legacy.Mediafile`) created by the tool.

If required, you can [configure the tool](#convert-attachments-and-media-library-files-to-media-libraries-instead-of-content-item-assets) to instead migrate media libraries as media libraries on the target instance.

#### Attachments

Attachment files are migrated as [content item assets](https://docs.kentico.com/x/content_item_assets_xp) to Content hub into a content folder `<site_name>/__Attachments`. Assets are created in the specified language if the language is available (e.g., attachments of pages). Migrated assets are created as content items of a _Legacy attachment_ content type (code name `Legacy.Attachment`) created by the tool.

If required, you can [configure the tool](#convert-attachments-and-media-library-files-to-media-libraries-instead-of-content-item-assets) to instead migrate attachments as media libraries on the target instance.

#### Forms

The migration does not include the content of form autoresponder and notification emails. You can customize the default migration of fields using the [extensibility feature](../Migration.Tool.Extensions/README.md).

You can migrate form autoresponders to Xperience by Kentico manually by copying your HTML code and content into Email
templates and Emails. See [Emails](https://docs.xperience.io/x/IaDWCQ).

#### Users

**Note**: Xperience by Kentico uses separate entities for users with access to the administration interface (_CMS_User_
table) and live site visitor accounts (_CMS_Member_ table). Consequently, only users whose _Privilege level_ is set to
_Editor_ and above are migrated (_Users_ -> edit a user -> _General_ tab) via the `--users` command. To migrate live
site accounts as well, use [`--members`](#migrate-command-parameters).

The command migrates all users with access to the administration interface. Note the following expected behavior:

- The 'administrator' user account is only transferred from the source if it does not exist on the target instance.
- The 'public' system user is updated, and all bindings (e.g., the site binding) are mapped automatically on the target
  instance.
- Site bindings are updated automatically for all migrated users.
- Users in Xperience by Kentico must have an email address. Migration is only supported for users with a **unique**
  email address on the source instance.
  - If you encounter issues related to email validation, you can change the default validation behavior via
    the `CMSEmailValidationRegex` [application key](https://docs.xperience.io/x/yA6RBg).
- Custom user fields can be migrated together with _module classes_.
  - You can customize the default migration of fields using the [extensibility feature](../Migration.Tool.Extensions/README.md).

Additionally, the command migrates all roles and user-role bindings for users whose _Privilege level_ is _Editor_ or
higher.

Because Xperience by Kentico uses a different [permission model](https://docs.xperience.io/x/7IVwCg), no existing role
permissions or UI personalization settings are migrated. After the migration, the permissions for each role must be
configured again.

#### Members

In Xperience by Kentico, live site users are represented using a separate **Member** entity and stored in the
_CMS_Member_ table.

The migration identifies live site users as those without access to the administration interface. That is, only those
accounts whose _Privilege level_ is set to _None_ (Users -> edit a user -> General tab) are migrated.

The migration includes:

- All system fields from the _CMS_User_ and _CMS_UserSettings_ tables. You can customize which fields are migrated via
  the `MemberIncludeUserSystemFields` configuration option. See [configuration](#configuration).
- All custom fields added to the _CMS_User_ and _CMS_UserSettings_ tables are migrated under `CMS_Member`. The columns specified in the `MemberIncludeSystemFields` option are appended to the `CMS_Member` table in the order in which they were specified.
  As an example, take the following `CMS_Member` columns

  ```text
  |MemberId|MemberEmail|...|MemberSecurityStamp|
  ```

  And the following `Migration.Tool.CLI/appsettings.json` configuration.

  ```json
  {
    "MemberIncludeUserSystemFields": "FirstName|LastName|UserPrivilegeLevel"
  }
  ```

  This will result in the following `CMS_Member` structure after migration.

  ```text
  |MemberId|MemberEmail|...|MemberSecurityStamp|FirstName|LastName|UserPrivilegeLevel|`
  ```

  > If you are migrating custom fields, the `--custom-modules` migration command must be run before the `--members`
  > command. For example:

  ```powershell
  Migration.Tool.CLI.exe migrate --sites --custom-modules  --users --members
  ```

- You can customize the default migration of fields using the [extensibility feature](../Migration.Tool.Extensions/README.md).

The migration **_DOES NOT_** include:

- External sign-in information associated with each account (e.g., Google or Facebook logins).
- User password hashes from the `CMS_User.UserPassword` column.

  After the migration, the corresponding `CMS_Member.MemberPassword` in the target Xperience by Kentico instance
  is `NULL`. This means that the migrated accounts **CANNOT** be used to sign in to the system under any circumstances.
  The account owners must first reset their password via ASP.NET Identity.

  See [Forms authentication](https://docs.xperience.io/x/t4ouCw) for a sample password reset process that can be adapted
  for this scenario. The general flow consists of these steps:

  1. Select the migrated member accounts.

     ```csharp
     // Selects members whose password is null and who don't use external providers to sign in
     var migratedMembers =
             MemberInfo.Provider
                     .Get()
                     .WhereNull("MemberPassword")
                     .WhereEquals("MemberIsExternal", 0);
     ```

  2. Generate password reset tokens for each account using `UserManager.GeneratePasswordResetTokenAsync(member)`.
  3. Send the password reset email to each account using `IEmailService`.

     ```csharp
     await emailService
             .SendEmail(new EmailMessage()
             {
                 Recipients = member.Email,
                 Subject = "Password reset request",
                 // {resetURL} targets a controller action with the password reset form
                 Body = $"To reset your account's password, click <a href=\"{resetUrl}\">here</a>."
             });
     ```

#### Contacts

- Custom contact fields can be migrated together with _module classes_.
- For performance reasons, contacts and related objects are migrated using bulk SQL queries. As a result, you always
  need to delete all Contacts, Activities and Consent agreements before running the migration (when using
  the `migrate --contact-management` parameter).

## Configuration

Before you run the migration, configure options in the `Migration.Tool.CLI/appsettings.json` file.

Add the options under the `Settings` section in the configuration file.

| Configuration                                                     | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
|-------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| KxConnectionString                                                | The connection string to the source Kentico Xperience 13, Kentico 12, or Kentico 11 database.                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| KxCmsDirPath                                                      | The absolute file system path of the **CMS** folder in the source Kentico Xperience 13, Kentico 12, or Kentico 11 administration project. Required to migrate media library files.                                                                                                                                                                                                                                                                                                                                                                         |
| XbyKDirPath                                                       | The absolute file system path of the root of the target Xperience by Kentico project. Required to migrate media library and page attachment files.                                                                                                                                                                                                                                                                                                                                                                                                         |
| XbyKApiSettings                                                   | Configuration options set for the API when creating migrated objects in the target application.<br /><br />The `ConnectionStrings.CMSConnectionString`option is required - set the connection string to the target Xperience by Kentico database (the same value as obsolete `XbKConnectionString`).                                                                                                                                                                                                                                                       |
| MigrationProtocolPath                                             | The absolute file system path of the location where the [migration protocol file](./MIGRATION_PROTOCOL_REFERENCE.md) is generated.<br /><br />For example: `"C:\\Logs\\Migration.Tool.Protocol.log"`                                                                                                                                                                                                                                                                                                                                                       |
| ConvertClassesToContentHub                                        | Specifies which page types, custom tables or custom module classes are migrated to [reusable content items](https://docs.kentico.com/x/content_items_xp) (instead of website channel pages or custom module classes for custom tables and classes). Enter page type code names, separated with either `;` or `,`. See [Convert pages or custom tables to Content hub](#convert-pages-or-custom-tables-to-content-hub) or [Convert module classes to Content hub](#convert-module-classes-to-content-hub) for detailed information.                         |
| CustomModuleClassDisplayNamePatterns                              | Specifies the format of content item names for items migrated from custom module classes. Add a dictionary with the class name as the key and the name pattern as the value. The name pattern can use placeholders that are replaced by values from a specific column in the source class. <br /><br />Example: `CustomModuleItem-{CustomClassGuid}`                                                                                                                                                                                                       |
| MigrateOnlyMediaFileInfo                                          | If set to `true`, only the database representations of media files are migrated, without the files in the media folder in the project's file system. For example, enable this option if your media library files are mapped to a shared directory or Cloud storage.<br /><br />If `false`, media files are migrated based on the `KxCmsDirPath` location.                                                                                                                                                                                                  |
| MigrateMediaToMediaLibrary                                        | Determines whether media library files and attachments from the source instance are migrated to the target instance as media libraries or as [content item assets](https://docs.kentico.com/x/content_item_assets_xp) in the content hub. The default value is `false` – media files and attachments are migrated as content item assets. <br /><br /> See [Convert attachments and media library files to media libraries instead of content item assets](#convert-attachments-and-media-library-files-to-media-libraries-instead-of-content-item-assets) |
| LegacyFlatAssetTree                                               | Use legacy behavior of versions up to 2.3.0. Content folders for asset content items will be created in a flat structure (all under root folder)                                                                                                                                                                                                                                                                                                                                                                                                           |
| AssetRootFolders                                                  | Dictionary defining the root folder for Asset content items per site: Key is site name (CMS_Site.SiteName). Value is in format _/FolderDisplayName1/FolderDisplayName2/..._                                                                                                                                                                                                                                                                                                                                                                                |
| TargetWorkspaceName                                               | The code name of the [workspace](https://docs.kentico.com/x/workspaces_xp) to which content items, content folders and other related entities are migrated. This workspace is used if another workspace is not specified explicitly, for example by the content item director API in [migration customizations](../Migration.Tool.Extensions/README.md). This configuration is not necessary if the target project only contains a single workspace.                                                                                                                                                         |
| MemberIncludeUserSystemFields                                     | Determines which system fields from the _CMS_User_ and _CMS_UserSettings_ tables are migrated to _CMS_Member_ in Xperience by Kentico. Fields that do not exist in _CMS_Member_ are automatically created. <br /><br />The sample `appsettings.json` file included with the tool by default includes all user fields that can be migrated from Kentico Xperience 13. Exclude specific fields from the migration by removing them from this configuration option.                                                                                           |
| IncludeExtendedMetadata                                           | Migrates DocumentPageTitle, DocumentPageDescription and DocumentPageKeywords if they are available in the source instance                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| UseOmActivityNodeRelationAutofix                                  | Determines how the migration handles references from Contact management activities to non-existing pages.<br /><br />Possible options:<br />`DiscardData` - faulty references are removed,<br />`AttemptFix` - references are updated to the IDs of corresponding pages created by the migration,<br />`Error` - an error is reported and the reference can be translated or otherwise handled manually                                                                                                                                                    |
| UseOmActivitySiteRelationAutofix                                  | Determines how the migration handles site references from Contact management activities.<br /><br />Possible options: `DiscardData`,`AttemptFix`,`Error`                                                                                                                                                                                                                                                                                                                                                                                                   |
| EntityConfigurations                                              | Contains options that allow you to fine-tune the migration of specific object types.                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| EntityConfigurations._&lt;object table name&gt;_.ExcludeCodeNames | Excludes objects with the specified code names from the migration.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| CreateReusableFieldSchemaForClasses                               | Specifies which page types are also converted to [reusable field schemas](#convert-page-types-to-reusable-field-schemas). This option cannot be combined with usage of `ReusableSchemaBuilder` in [custom class mappings](../Migration.Tool.Extensions/README.md#custom-class-mappings).                                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                                                                                               |
| OptInFeatures.QuerySourceInstanceApi.Enabled                      | If `true`, [source instance API discovery](#source-instance-api-discovery) is enabled to allow advanced migration of page builder content for pages and page templates.                                                                                                                                                                                                                                                                                                                                                                                    |
| OptInFeatures.QuerySourceInstanceApi.Connections                  | To use [source instance API discovery](#source-instance-api-discovery), you need to add a connection JSON object containing the following values:<br />`SourceInstanceUri` - the base URI where the source instance's live site application is running.<br />`Secret` - the secret that you set in the _ToolkitApiController.cs_ file on the source instance.                                                                                                                                                                                              |
| OptInFeatures.CustomMigration.FieldMigrations                     | Enables conversion of media selection text fields to content item assets or media library files. See [Convert text fields with media links](#convert-text-fields-with-media-links) for more information.                                                                                                                                                                                                                                                                                                                                                   |

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
    "XbyKDirPath": "C:\\inetpub\\wwwroot\\XP_Target",
    "XbyKApiSettings": {
      "ConnectionStrings": {
        "CMSConnectionString": "Data Source=myserver;Initial Catalog=XperienceByKentico;Integrated Security=True;Persist Security Info=False;Connect Timeout=120;Encrypt=False;Current Language=English;"
      }
    },
    "MigrationProtocolPath": "C:\\_Development\\xperience-migration-toolkit-master\\Migration.Tool.Protocol.log",
    "MemberIncludeUserSystemFields": "FirstName|MiddleName|LastName|FullName|UserPrivilegeLevel|UserIsExternal|LastLogon|UserLastModified|UserGender|UserDateOfBirth",
    "ConvertClassesToContentHub": "Acme.Article,Acme.Product,Acme.CustomClass",
    "CustomModuleClassDisplayNamePatterns": {
      "Acme.CustomClass": "Item-{CustomClassGuid}"
    },
    "MigrateOnlyMediaFileInfo": false,
    "MigrateMediaToMediaLibrary": false,
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
          "Acme.News",
          "Acme.Office",
          "CMS.Blog",
          "CMS.BlogPost"
        ]
      },
      "CMS_SettingsKey": {
        "ExcludeCodeNames": ["CMSHomePagePath"]
      }
    },
    "TargetWorkspaceName": "MainWorkspace",
    "OptInFeatures": {
      "QuerySourceInstanceApi": {
        "Enabled": true,
        "Connections": [
          {
            "SourceInstanceUri": "http://localhost:60527",
            "Secret": "__your secret string__"
          }
        ]
      },
      "FieldMigrations": {
        "SourceDataType": "text",
        "TargetDataType": "assets",
        "SourceFormControl": "MediaSelectionControl",
        "TargetFormComponent": "Kentico.Administration.AssetSelector",
        "Actions": ["convert to asset"],
        "FieldNameRegex": ".*"
      }
    }
  }
}
```

## Source instance API discovery

> :warning: **Warning** – source instance API discovery is only available when migrating from Kentico Xperience 13.

By default, JSON data storing the page builder content of pages and custom page templates is migrated directly without
modifications. Within this content, page builder components (widgets, sections, etc.) with properties have their
configuration based on Kentico Xperience 13 form components, which are assigned to the properties on the source
instance. On the target Xperience by Kentico instance, the migrated data can work in the Page Builder's legacy
compatibility mode.

However, we strongly recommend updating your codebase to the new Xperience by Kentico components.
See [Editing components in Xperience by Kentico](https://docs.xperience.io/x/wIfWCQ) to learn more.

To convert page builder data to a format suitable for the Xperience by Kentico components, the Kentico Migration Tool
provides an advanced migration mode that utilizes API discovery on the source instance. The advanced mode currently
provides the following data conversion:

- **Attachment selector** properties - converted to a format suitable for the Xperience by Kentico **Media selector**
  component, with `IEnumerable<AssetRelatedItem>` values.
- **Page selector** properties - converted to a format suitable for the Xperience by Kentico Page selector component,
  with `IEnumerable<WebPageRelatedItem>` values.

### Prerequisites and Limitations

- To use source instance API discovery, the live site application of your source instance must be running and available
  during the migration.
- Using the advanced page builder data migration **prevents the data from being used in the Page Builder's legacy
  compatibility mode**. With this approach, you need to update all page builder component code files to
  the [Xperience by Kentico format](https://docs.xperience.io/x/wIfWCQ).
- The source instance API discovery feature only processes component properties defined using `[EditingComponent]`
  attribute notation. Other implementations, such as properties edited via custom view components in the Razer view, are
  not supported.

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

1. Copy the `ToolApiController.cs` file to the `Controllers` folder in the **live site project** of your Kentico
   Xperience 13 source instance. Get the file from the following location in the Kentico Migration Tool repository:

   - For .NET Core projects: `KX13.Extensions\ToolApiController.cs`
   - For MVC 5 (.NET Framework 4.8) projects: `KX13.NET48.Extensions\ToolApiController.NET48.cs`

2. Register routes for the `ToolApi` controller's actions into the source instance's live site application.

   - For .NET Core projects, add endpoints in the project's `Startup.cs` or `Program.cs` file:

     ```csharp
     app.UseEndpoints(endpoints =>
     {
         endpoints.MapControllerRoute(
             name: "ToolExtendedFeatures",
             pattern: "{controller}/{action}",
             constraints: new
             {
                 controller = "ToolApi"
             }
         );

         // other routes ...
     });
     ```

   - For MVC 5 projects, map the routes in your application's `RouteCollection` (e.g., in
     the `/App_Start/RouteConfig.cs` file):

     ```csharp
     public static void RegisterRoutes(RouteCollection routes)
     {
         // Maps routes for Xperience handlers and enabled features
         routes.Kentico().MapRoutes()

         routes.MapRoute(
             name: "ToolExtendedFeatures",
             url: "{controller}/{action}",
             defaults: new { },
             constraints: new
             {
                 controller = "ToolApi"
             }
         );

         // other routes ...
     }
     ```

3. Edit `ToolApiController.cs` and set a value for the `Secret` constant:

   ```csharp
   private const string Secret = "__your secret string__";
   ```

4. Configure the `Settings.OptInFeatures.QuerySourceInstanceApi` [configuration options](#configuration) for the
   Kentico Migration Tool:

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

You can test the source instance API discovery by making a POST request
to `<source instance live site URI>/ToolApi/Test` with `{ "secret":"__your secret string__" }` in the body. If your
setup is correct, the response should be: `{ "pong": true }`

When you now [migrate data](#migrate-data), the tool performs API discovery of page builder component code on the source
instance and advanced migration of page builder data.

## Convert pages or custom tables to Content hub

You can migrate pages to [Content hub items](https://docs.kentico.com/x/content_items_xp) instead of website channel pages. Similarly, you can migrate the data of custom tables to [Content hub items](https://docs.kentico.com/x/content_items_xp) instead of module classes. This can be useful if you wish to transition your project's content model to utilize reusable content in Xperience by Kentico.

Specify a list of page type or custom table code names (separated with either `;` or `,`) in the `Settings.ConvertClassesToContentHub` [configuration option](#configuration).

```json
"Settings":{
  ...

  "ConvertClassesToContentHub": "Acme.Article,Acme.Product,Acme.CustomTable1",
},
```

The migration then converts the specified page types or custom tables to content types for reusable content, and all pages and custom table records of the given types to items in Content hub.

**Note**: Page URLs and other page-specific configuration and metadata is not migrated in this scenario.

For advanced scenarios, you can use the extensibility feature to implement [customizations](../Migration.Tool.Extensions/README.md#custom-class-mappings) that map specific page types, custom tables or their individual fields to reusable content types. For example, this allows you to migrate multiple page types to a single content type.

To preserve relationships between pages converted to reusable content items and its children, you can use [Custom child links](../Migration.Tool.Extensions/README.md#custom-child-links). This feature allows you to map children of the original page to a content item/page selector field of the target reusable content item.

## Convert module classes to Content hub

**Note**: This feature is only tested only when using Kentico Xperience 13 as the migration source.

You can migrate custom module classes and their data records to [Content hub items](https://docs.kentico.com/x/content_items_xp) instead of modules classes.

Specify a list of module class code names (separated with either `;` or `,`) in the `Settings.ConvertClassesToContentHub` [configuration option](#configuration). Optionally, you can specify the name pattern for the migrated content items using the `Settings.CustomModuleClassDisplayNamePatterns` option.

```json
"Settings":{
  ...

  "ConvertClassesToContentHub": "Acme.Article,Acme.Product,Acme.CustomClass",
  "CustomModuleClassDisplayNamePatterns": {
      "Acme.CustomClass": "Item-{CustomClassGuid}"
  },
```

For advanced scenarios, you can use the extensibility feature to implement [customizations](../Migration.Tool.Extensions/README.md#custom-class-mappings) that map specific module classes or their individual fields to reusable content types. For example, this allows you to migrate multiple classes to a single content type.

## Convert page types to reusable field schemas

It is not possible to migrate any page types that inherit fields from other page types. However, to make the manual
migration of such page types easier, you can create [reusable field schemas](https://docs.kentico.com/x/D4_OD) from
specified parent page types. Specify a list of page types to be converted to reusable field schemas (separated with
either `;` or `,`) in the `Settings.CreateReusableFieldSchemaForClasses` [configuration option](#configuration).

The following example specifies two page types from which reusable schemas are created:

```json
"Settings":{
  ...

  "CreateReusableFieldSchemaForClasses": "Acme.SeoFields;Acme.ArticleFields"
},
```

For advanced scenarios, you can use the extensibility feature to implement [customizations](../Migration.Tool.Extensions/README.md#custom-class-mappings) that allow you to specify the mapping of page types to reusable field schemas. For example, this allows you to [extract fields from multiple page types into a reusable field schema](https://docs.kentico.com/x/remodel_page_types_as_reusable_field_schemas_guides).

### :warning: Notes

- Conversion of page types to reusable field schemas works best when all field names of page types are unique (i.e., prefixed with the page type name). If multiple page types converted to reusable field schemas have fields with the same code name, the code name is prefixed with the content type name in the converted reusable field schemas.

- Page types specified by the `CreateReusableFieldSchemaForClasses` configuration option are also migrated as content types into to the target instance.

- Any usage of `ReusableSchemaBuilder` in [custom class mappings](../Migration.Tool.Extensions/README.md) cannot be combined together with usage of the `Settings.CreateReusableFieldSchemaForClasses` [configuration option](#configuration).

## Convert text fields with media links

By default, page type and module class fields with the _Text_ data type and the _Media
selection_ [form control](https://docs.xperience.io/x/0A_RBg) from the source instance are converted to plain _Text_
fields in the target instance. You can instead configure the Kentico Migration Tool to convert these fields to the
_Content items_ data type and use the _Content item selector_ form component, or _Media files_ data type and use the _Media file selector_ form component if you choose to [convert attachments and media library files to media libraries instead of content item assets](#convert-attachments-and-media-library-files-to-media-libraries-instead-of-content-item-assets).

### :warning: Notes

- Only media libraries using the **Permanent** [file URL format](https://docs.xperience.io/x/xQ_RBg) are supported. Content from media libraries with enabled **Use direct path for files in content** setting will not be converted.

- If you enable this feature, you also need to change retrieval and handling of affected files in your code, as the structure of the stored data changes from a text path to a _Media files_ data type.

  Source

  ```text
  ~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg?width=300&height=100
  ```

  Destination

  ```json
  [
    {
      "Identifier": "CCEAD0F0-E2BF-459B-814A-36699E5C773E",
      "Name": "somefile.jpeg",
      "Size": 11803,
      "Dimensions": { "Width": 300, "Height": 100 }
    }
  ]
  ```

- The value of the field now needs to be [retrieved as a media library file](https://docs.xperience.io/x/LA2RBg).

- If the target instance is a [SaaS project](https://docs.kentico.com/x/saas_xp), you need to manually move content item asset files. See [Content items](#content-items) for more information.

### Convert to content item assets

To enable this feature, configure the `OptInFeatures.CustomMigration.FieldMigrations` [options](#configuration) for this
tool. Use the values in the code snippet below:

```json
"OptInFeatures": {
  "CustomMigration": {
    "FieldMigrations": [
      {
        "SourceDataType": "text",
        "TargetDataType": "contentitemreference",
        "SourceFormControl": "MediaSelectionControl",
        "TargetFormComponent": "Kentico.Administration.ContentItemSelector",
        "Actions": [
          "convert to asset"
        ],
        "FieldNameRegex": ".*"
      }
    ]
  }
}
```

`FieldNameRegex` - a regular expression used to filter what fields are converted. Only fields with field names that
match the regular expressions are converted. Use `.*` to match all fields.

### Convert to media libraries

- Attachment links (containing a `getattachment` handler) are migrated as [attachments](#attachments) and changed to the
  _Media files_ data type.
- Media file links (containing a `getmedia` handler) are changed to the _Media files_ data type. It is expected that the
  media library containing the targeted file has been migrated.

To enable this feature, configure the `OptInFeatures.CustomMigration.FieldMigrations` [options](#configuration) for this
tool. Use the values in the code snippet below:

```json
"OptInFeatures":{
  "CustomMigration":{
    "FieldMigrations": [
      {
        "SourceDataType": "text",
        "TargetDataType": "assets",
        "SourceFormControl": "MediaSelectionControl",
        "TargetFormComponent": "Kentico.Administration.AssetSelector",
        "Actions": [ "convert to asset" ],
        "FieldNameRegex": ".*"
      }
    ]
  }
}
```

`FieldNameRegex` - a regular expression used to filter what fields are converted. Only fields with field names that
match the regular expressions are converted. Use `.*` to match all fields.

## Convert attachments and media library files to media libraries instead of content item assets

By default, media libraries and attachments are migrated as content item assets in the target instance, which is the recommended approach to ensure future-proofing of project and improve the [content model](https://docs.kentico.com/x/f4HWCQ). You can modify this behavior by configuring the value of the `MigrateMediaToMediaLibrary` setting to `true` and convert media library files and attachments to media libraries if you want to continue using media libraries. When set to `false`, media libraries and attachments are migrated as content item assets in the target instance.

### Media libraries

- In Xperience by Kentico, Media libraries are global instead of site-specific.
- The code name of each media library on the target instance is `{SiteName}_{LibraryCodeName}`.
- Media library permissions are currently not supported in Xperience by Kentico and are not migrated.

### Attachments

- Page attachments are migrated into a media library named: _"Attachments for site \<sitename\>"_
- The media library contains folders matching the content tree structure for all pages with attachments (including empty folders for parent pages without attachments). The folders are named after the _node alias_ of the source pages.
- Each page's folder directly contains all unsorted attachments (files added on the _Attachments_ tab in the source's _Pages_ application).
- Attachments stored in specific page fields are placed into subfolders, named in format: _"\_\_fieldname"_. These subfolders can include multiple files for fields of the _Attachments_ type, or a single file for _File_ type fields.
- The migration does not include temporary attachments (created when a file upload is not finished correctly). If any are present on the source instance, a warning is logged in the [migration protocol](./MIGRATION_PROTOCOL_REFERENCE.md).

The following is an example of a media library created by the Kentico Migration Tool for page attachments:

- **Articles** (empty parent folder)
  - **Coffee-processing-techniques** (contains any unsorted attachments of the '/Articles/Coffee-processing-techniques' page)
    - **\_\_Teaser** (contains attachments stored in the page's 'Teaser' field)
  - **Which-brewing-fits-you**
    - **\_\_Teaser**
  - ...

Additionally, any attachments placed into the content of migrated pages **will no longer work** in Xperience by Kentico.
This includes images and file download links that use **/getattachment** and **/getimage** URLs.

If you wish to continue using these legacy attachment URLs from earlier product versions, you need to add a custom
handler to your Xperience by Kentico project. See [`Migration.Tool.KXP.Extensions/README.MD`](/Migration.Tool.KXP.Extensions/README.MD) for instructions.

## Automatic database patching

Migration Tool has a database patching mechanism.

It's applied at each tool run, prior to executing the migration commands. Therefore if you wish to only apply the patches, use `migrate` argument without any additional commands.

Patches are applied in idempotent fashion, i.e. only if they haven't been applied before.


