# Supported Data

The Kentico Migration Tool does not transfer all data available in the source instance. Xperience by Kentico currently provides a smaller, more focused set of features. As a result, some objects are discarded or migrated to a suitable alternative.

This tool only supports content and objects **stored in the database** and related binary data on the file system, such as media library files. Code, customizations, and any other types of content need to be migrated manually to the target project and adjusted for Xperience by Kentico.

Currently, the Kentico Migration Tool supports the following types of data:

- **Sites**
  - The tool migrates each site on the source to a [website channel](https://docs.kentico.com/x/34HFC) object in Xperience by Kentico.
- **Cultures**
  - The set of cultures used across all sites in the source gets mapped to a [language](https://docs.kentico.com/x/OxT_Cw) in the _Languages_ application.
- **Content types** (_Page types_ in earlier Kentico versions)
  - The Kentico Migration Tool attempts to map the _Data type_ and _Form control_ of page type fields to an appropriate equivalent in Xperience by Kentico. This mapping is not always possible and does not work for custom data types or form controls. We recommend checking your content type fields after the migration and adjusting them if necessary.
  - The migration includes only page types assigned to at least one site on the source instance.
  - The migration supports [content type restrictions](https://docs.kentico.com/x/bw2RBg) (scopes and allowed child types).
  - Xperience by Kentico currently does not support:
    - Macro expressions in page type field default values or other settings. Content type fields containing macros will not work correctly after the migration.
    - Categories for page type fields. Field categories are not migrated with page types.
    - Page type inheritance. Page types that inherit fields are migrated including all inherited fields but the binding to the parent page type is not preserved.
      - However, you can create [reusable field schemas](../Migration.Tool.CLI/README.md#convert-page-types-to-reusable-field-schemas) for page types from which other page types inherit.
  - All migrated Content types have the **Include in routing** option enabled (the migration never creates pages without URL and routing).
- **Pages**
  - Pages can be migrated either to [website channel pages](https://docs.kentico.com/x/JwKQC) (default behavior) or [reusable content items](https://docs.kentico.com/x/barWCQ) in Content hub.
  - The migration includes the following versions of pages:
    - _Published_
    - _Latest draft version_ - for published pages, the version is migrated to the _Draft (New version)_ [status](https://docs.kentico.com/x/JwKQC); for pages that do not have a published version, the version is migrated to the _Draft (Initial)_ status.
    - _Archived_ pages are migrated to the _Unpublished_ status.
  - Page URLs are included only when migrating to [website channel pages](https://docs.kentico.com/x/JwKQC) (default behavior). URL migration depends on the source instance version:
  - For Kentico Xperience 13, the migration includes the URL paths of pages and Former URLs.
  - For Kentico 12 and Kentico 11, URL paths are not migrated. Instead, a default URL path is created from
    the `DocumentUrlPath` or `NodeAliasPath`.
  - For Kentico Xperience 13 and Kentico 12, [Alternative URLs](https://docs.kentico.com/13/managing-website-content/working-with-pages/managing-page-urls#alternative-urls) are migrated to [Vanity URLs](https://docs.kentico.com/documentation/business-users/website-content/manage-page-urls#manage-vanity-urls-of-pages).
  - Linked pages are currently not supported in Xperience by Kentico. By default, the migration creates standard page copies for any
  linked pages on the source instance. This behavior can be changed by implementing [custom handling of linked pages](../Migration.Tool.Extensions/README.md#customize-linked-page-handling).
  - Page permissions are currently not supported by the Kentico Migration Tool and are not migrated. If you need page ACLs to be migrated, please [open an issue and request the feature](https://github.com/Kentico/xperience-by-kentico-kentico-migration-tool/issues/new?assignees=&labels=&projects=&template=feature_request.md).
  - Migration of Page Builder content is only available for Kentico Xperience 13.
    - If you are [migrating a Kentico 12 MVC application](https://github.com/Kentico/xperience-by-kentico-kentico-migration-tool/blob/master/docs/Usage-Guide.md#kentico-12-mvc) you can upgrade it to Kentico Xperience 13 using the Kentico Installation Manager (KIM) and then upgrade to Xperience by Kentico with Page Builder content.
  - You can find examples of how to update your page retrieval code to work with Xperience by Kentico's _content items_ [in our guides](https://docs.kentico.com/x/upgrade_content_retrieval_code_guides).
- **Page attachments**
  - Page attachments are not supported in Xperience by Kentico. Attachments are migrated into media libraries. See [`Migration.Tool.CLI/README.md - Attachments`](../Migration.Tool.CLI/README.md#Attachments) for detailed information about the conversion process.
- **Preset page templates** (_Custom page templates_ in Kentico Xperience 13)
  - Migration of custom page templates is only available for Kentico Xperience 13.
- **Categories**
  - Xperience by Kentico uses a different approach to categorization. Categories are migrated to [taxonomies](https://docs.kentico.com/x/taxonomies_xp) and selected categories for each page are assigned to pages in the target instance via a [reusable field schema](https://docs.kentico.com/x/D4_OD). See [`Migration.Tool.CLI/README.md - Categories`](../Migration.Tool.CLI/README.md#categories).
  - [Categories stored as a field of pages](https://docs.kentico.com/x/wA_RBg) and [personal categories](https://docs.kentico.com/x/IgqRBg) are not supported.
- **Media libraries and media files**
  - Media library permissions are currently not supported in Xperience by Kentico and are not migrated.
- **Forms**
  - The migration does not include the content of form autoresponder and notification emails. You can migrate form autoresponders to Xperience by Kentico manually by copying your HTML code and content into Email templates and Emails. See [Emails](https://docs.kentico.com/x/IaDWCQ).
- **Users**
  - Xperience by Kentico uses separate entities for users with access to the administration interface (_CMS_User_ table) and live site visitor accounts (_CMS_Member_ table). Consequently, only users whose _Privilege level_ is _Editor_ or higher are migrated (_Users_ -> edit a user -> _General_ tab).
  - Users in Xperience by Kentico must have an email address. Migration is only supported for users who have a unique email address value on the source instance.
  - Custom user fields are an optional part of _module class_ migration.
  - Live site users are represented using a separate **Member** entity and stored in the _CMS_Member_ table. The migration identifies live site users as those without access to the administration interface - accounts with _Privilege level_ set to _None_ (Users -> edit a user -> General tab).
- **Roles**
  - Only roles that have at least one user whose _Privilege level_ is set to _Editor_ and above are migrated.
  - Because Xperience by Kentico uses a different [permission model](https://docs.kentico.com/x/7IVwCg), no existing role permissions or UI personalization settings are migrated. After the migration, the permissions for each role must be configured again.
- **Contacts**
  - The target instance's _OMContact_ and _OMActivity_ database tables must be empty for performance reasons.
  - Custom contact fields are an optional part of _module class_ migration.
- **Activities**
- **Consents and consent agreements**
- **Modules and classes**
  - The migration includes the following:
    - Custom modules
    - All classes associated with custom modules
    - All data stored within custom module classes
    - The following customizable system classes and their custom fields: _User_, _Media file_, _Contact management - Account_ (however, accounts are currently not supported in Xperience by Kentico), _Contact management - Contact_
  - Module and class migration does NOT include:
    - UI elements and all related user interface settings. The administration of Xperience by Kentico uses a different technology stack than earlier Kentico versions and is incompatible. To learn how to build the administration UI, see [Extend the administration interface](https://docs.kentico.com/x/GwKQC) and [Example - Offices management application](https://docs.kentico.com/x/hIFwCg).
    - Alternative forms under classes and UI-related configuration of class fields (field labels, Form controls, etc.). After the migration, you must manually create the appropriate [UI forms](https://docs.kentico.com/x/V6rWCQ) in Xperience by Kentico.
    - Custom settings under modules, which are currently not supported in Xperience by Kentico
    - Module permissions (permissions work differently in Xperience by Kentico - see [Role management](https://docs.kentico.com/x/7IVwCg) and [UI page permission checks](https://docs.kentico.com/x/8IKyCg))
    - As with all object types, the Kentico Migration Tool does not transfer code files to the target project. You must manually move all code files generated for your custom classes (_Info_, _InfoProvider_, etc.).
  - Module classes and the stored data can optionally be migrated as [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub.
- **Custom tables**
  - Custom tables are not supported in Xperience by Kentico. Data from custom tables can be migrated to the target instance either as [custom module classes](https://docs.kentico.com/x/AKDWCQ) (default behavior) or as [reusable content items](https://docs.kentico.com/x/content_items_xp) in Content hub.
  - Any user interface, listings, filters, and other functionality related to migrated custom tables needs to be implemented in the target instance.
- **Setting values**

  - The migration only transfers the settings that exist in Xperience by Kentico.
  - The migration excludes site-specific settings that do not have a corresponding website channel-specific alternative in Xperience by Kentico.

- **Customers**
  - Migration of customers is only available for Kentico Xperience 13.
  - The migration includes customer records and customer addresses.
  - All custom fields are migrated. Optionally, you can also migrate selected system fields.
  - A `SiteOriginName` custom field is created to preserve the source site information.

- **Orders**
  - Migration of orders is only available for Kentico Xperience 13.
  - The migration includes order records, order items, and order addresses.
  - All custom fields are migrated. Optionally, you can also migrate selected system fields.
  - Orders are migrated with shipping and payment method **display names only** - the methods themselves are not migrated.
  - Custom fields `CurrencyCode` and `SiteOriginName` are created to preserve currency and source site information.
  - New fields `OrderItemTotalTax` and `OrderItemTaxRate` are set to 0 and need to be recalculated manually.
  - Orders can be filtered by date range or status during migration.
  - [Order statuses](https://docs.kentico.com/x/commerce_order_statuses_xp) are **not migrated** - they must be created manually on the target instance and mapped via configuration.

- **Countries and states**

## Unsupported data

The following types of data exist in Xperience by Kentico but are currently **not supported** by the Kentico Migration Tool:

- **Contact groups**
  - Static contact groups are currently not supported in Xperience by Kentico.
  - The condition format for dynamic contact groups is not compatible. To migrate contact groups:
    1. Migrate your contacts using the tool.
    2. Create the [contact groups](https://docs.kentico.com/x/o4PWCQ) manually in Xperience by Kentico.
    3. Build equivalent conditions.
    4. Recalculate the contact groups.
- **License keys**
  - Xperience by Kentico uses a different license key format.
- **Marketing automation**
  - Migration of Marketing automation is currently not supported

Additionally, object values or other content with **Macros** will not work correctly after the migration. Macro expressions are currently not supported for most data in Xperience by Kentico.
