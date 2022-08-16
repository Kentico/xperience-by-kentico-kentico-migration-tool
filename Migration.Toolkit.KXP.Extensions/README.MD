# Attachment handler url legacy handler 

## Motivation

**Xperience by Kentico** no longer supports page attachments and therefore there are no handlers for serving those attachments from server. Migration toolkit can migrate those attachments into media libraries, but there is still issue with old urls in content. 

## Attachment url in content - elimination of possible 404

To mitigate issue with old urls in format:
* `/getimage/[AttachmentGUID]/image.jpg`
  * example: `/getimage/454BD7D7-200F-4976-AD21-8B5D70CBE7FD/[AttachmentName].[AttachmentExtension]`
* `/getattachment[NodeAliasPath]/[AttachmentName].[AttachmentExtension]` 
  * example: `/getattachment/news1/image.jpg`
* `/getattachment/[AttachmentGUID]/[AttachmentName].[AttachmentExtension]`
  * example: `/getattachment/454BD7D7-200F-4976-AD21-8B5D70CBE7FD/image.jpg`

Legacy handler is supplied in file `LegacyAttachmentHandler.cs`.

## Installation

1) Copy file `[Migration toolkit repository]\Migration.Toolkit.KXP.Extensions\LegacyAttachmentHandler.cs` to project with **Xperience by Kentico**, for example to `Modules\AttachmentHandler.cs`
2) Change default namespace `Migration.Toolkit.KXP.Extensions` accordingly to fit your project
3) Compile & run project