## Handler for legacy attachment URLs

Page attachments are not supported in Xperience by Kentico. Instead, the [Migration toolkit](/README.md) transfers attachment files into [media libraries](https://docs.xperience.io/x/agKiCQ).

Any attachment files in the content of pages migrated from Kentico Xperience 13 still use the old attachment handler URLs and will no longer work (return 404 errors). For example, this can include images and file download links. If you wish to continue using these legacy attachment URLs, you need to [add a custom handler](#add-the-handler-to-your-project) to your Xperience by Kentico project.

### Prerequisite

To ensure that the handler provided in the `LegacyAttachmentHandler.cs` file works correctly, you need to [migrate](/Migration.Toolkit.CLI/README.md) your page attachments into a media library, and **keep the default folder structure and file names**.

### Attachment URLs in content

The `LegacyAttachmentHandler.cs` handler can serve the following legacy attachment URLs:

* `/getimage/[AttachmentGUID]/[AttachmentName].[AttachmentExtension]`
  * Example: `/getimage/454BD7D7-200F-4976-AD21-8B5D70CBE7FD/image.jpg`
* `/getattachment/[NodeAliasPath]/[AttachmentName].[AttachmentExtension]` 
  * Example: `/getattachment/news/article1/image.jpg`
* `/getattachment/[AttachmentGUID]/[AttachmentName].[AttachmentExtension]`
  * Example: `/getattachment/454BD7D7-200F-4976-AD21-8B5D70CBE7FD/image.jpg`

### Add the handler to your project

 1. Open your Xperience by Kentico solution in Visual Studio.
 2. Add a custom assembly (_Class Library_ project) to your solution or re-use an existing one. See the [Integrate custom code](https://docs.xperience.io/x/QoXWCQ) documentation for details.
 3. Copy the `[Migration toolkit repository]\Migration.Toolkit.KXP.Extensions\LegacyAttachmentHandler.cs` file into your custom class library.
 4. Change the default `Migration.Toolkit.KXP.Extensions` namespace in the class's code to fit your project and company.
 5. Rebuild your Xperience by Kentico solution.
 
The handler now processes legacy attachment URLs in your website's migrated content and returns matching media library files.
