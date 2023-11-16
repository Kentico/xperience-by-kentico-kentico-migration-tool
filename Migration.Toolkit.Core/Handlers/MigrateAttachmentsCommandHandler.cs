// namespace Migration.Toolkit.Core.Handlers;
//
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using Migration.Toolkit.Common;
// using Migration.Toolkit.Core.Abstractions;
// using Migration.Toolkit.Core.Services;
// using Migration.Toolkit.KX13.Context;
// using Migration.Toolkit.KX13.Models;
//
// public class MigrateAttachmentsCommandHandler : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
// {
//     private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
//     private readonly ToolkitConfiguration _toolkitConfiguration;
//     private readonly AttachmentMigrator _attachmentMigrator;
//
//     public MigrateAttachmentsCommandHandler(
//         IDbContextFactory<KX13Context> kx13ContextFactory,
//         ToolkitConfiguration toolkitConfiguration,
//         AttachmentMigrator attachmentMigrator
//     )
//     {
//         _kx13ContextFactory = kx13ContextFactory;
//         _toolkitConfiguration = toolkitConfiguration;
//         _attachmentMigrator = attachmentMigrator;
//     }
//
//     public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
//     {
//         var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<CmsSite>(s => s.SiteId).Keys.ToList();
//         await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
//
//         var kx13CmsAttachments = kx13Context.CmsAttachments
//             .Where(x => migratedSiteIds.Contains(x.AttachmentSiteId));
//
//         foreach (var kx13CmsAttachment in kx13CmsAttachments)
//         {
//             if (kx13CmsAttachment.AttachmentIsUnsorted != true || kx13CmsAttachment.AttachmentGroupGuid != null)
//             {
//                 // those must be migrated with pages
//                 continue;
//             }
//
//             var (_, canContinue, _, _) = _attachmentMigrator.MigrateAttachment(kx13CmsAttachment);
//             if (!canContinue)
//                 break;
//         }
//
//         return new GenericCommandResult();
//     }
// }