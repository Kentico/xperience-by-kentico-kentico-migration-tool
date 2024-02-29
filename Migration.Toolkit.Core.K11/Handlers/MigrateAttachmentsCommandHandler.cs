namespace Migration.Toolkit.Core.K11.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Core.K11.Services;
using Migration.Toolkit.K11;

// ReSharper disable once UnusedMember.Global [implicit use]
public class MigrateAttachmentsCommandHandler(IDbContextFactory<K11Context> k11ContextFactory,
    AttachmentMigrator attachmentMigrator) : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var k11CmsAttachments = k11Context.CmsAttachments
            .Include(a => a.AttachmentSite);

        foreach (var k11CmsAttachment in k11CmsAttachments)
        {
            if (k11CmsAttachment.AttachmentIsUnsorted != true || k11CmsAttachment.AttachmentGroupGuid != null)
            {
                // those must be migrated with pages
                continue;
            }

            var (_, canContinue, _, _) = attachmentMigrator.MigrateAttachment(k11CmsAttachment);
            if (!canContinue)
                break;
        }

        return new GenericCommandResult();
    }
}