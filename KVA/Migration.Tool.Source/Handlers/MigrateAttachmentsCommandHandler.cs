using MediatR;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Source.Model;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Source.Handlers;

// ReSharper disable once UnusedMember.Global [implicit use]
public class MigrateAttachmentsCommandHandler(
    ModelFacade modelFacade,
    IAttachmentMigrator attachmentMigrator
) : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var ksCmsAttachments = modelFacade.SelectAll<ICmsAttachment>();

        foreach (var ksCmsAttachment in ksCmsAttachments)
        {
            if (ksCmsAttachment.AttachmentIsUnsorted != true || ksCmsAttachment.AttachmentGroupGUID != null)
            {
                // those must be migrated with pages
                continue;
            }

            (_, bool canContinue) = await attachmentMigrator.MigrateAttachment(ksCmsAttachment);
            if (!canContinue)
            {
                break;
            }
        }

        return new GenericCommandResult();
    }
}
