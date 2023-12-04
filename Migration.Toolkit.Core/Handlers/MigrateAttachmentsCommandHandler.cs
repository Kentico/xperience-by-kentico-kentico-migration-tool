namespace Migration.Toolkit.Core.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Core.Services;
using Migration.Toolkit.KX13.Context;

// ReSharper disable once UnusedMember.Global [implicit use]
public class MigrateAttachmentsCommandHandler : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
{
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly AttachmentMigrator _attachmentMigrator;

    public MigrateAttachmentsCommandHandler(
        IDbContextFactory<KX13Context> kx13ContextFactory,
        AttachmentMigrator attachmentMigrator
    )
    {
        _kx13ContextFactory = kx13ContextFactory;
        _attachmentMigrator = attachmentMigrator;
    }

    public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsAttachments = kx13Context.CmsAttachments
            .Include(a => a.AttachmentSite);

        foreach (var kx13CmsAttachment in kx13CmsAttachments)
        {
            if (kx13CmsAttachment.AttachmentIsUnsorted != true || kx13CmsAttachment.AttachmentGroupGuid != null)
            {
                // those must be migrated with pages
                continue;
            }

            var (_, canContinue, _, _) = _attachmentMigrator.MigrateAttachment(kx13CmsAttachment);
            if (!canContinue)
                break;
        }

        return new GenericCommandResult();
    }
}