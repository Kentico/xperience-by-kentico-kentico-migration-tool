namespace Migration.Toolkit.Core.KX12.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Core.KX12.Services;
using Migration.Toolkit.KX12.Context;

// ReSharper disable once UnusedMember.Global [implicit use]
public class MigrateAttachmentsCommandHandler : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
{
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly AttachmentMigrator _attachmentMigrator;

    public MigrateAttachmentsCommandHandler(
        IDbContextFactory<KX12Context> kx12ContextFactory,
        AttachmentMigrator attachmentMigrator
    )
    {
        _kx12ContextFactory = kx12ContextFactory;
        _attachmentMigrator = attachmentMigrator;
    }

    public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12CmsAttachments = kx12Context.CmsAttachments
            .Include(a => a.AttachmentSite);

        foreach (var k12CmsAttachment in k12CmsAttachments)
        {
            if (k12CmsAttachment.AttachmentIsUnsorted != true || k12CmsAttachment.AttachmentGroupGuid != null)
            {
                // those must be migrated with pages
                continue;
            }

            var (_, canContinue, _, _) = _attachmentMigrator.MigrateAttachment(k12CmsAttachment);
            if (!canContinue)
                break;
        }

        return new GenericCommandResult();
    }
}