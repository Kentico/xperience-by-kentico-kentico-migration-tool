using MediatR;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

namespace Migration.Toolkit.Source.Handlers;

// ReSharper disable once UnusedMember.Global [implicit use]
public class MigrateAttachmentsCommandHandler(
    ModelFacade modelFacade,
    AttachmentMigrator attachmentMigrator
) : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
{
    public Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var ksCmsAttachments = modelFacade.SelectAll<ICmsAttachment>();

        foreach (var ksCmsAttachment in ksCmsAttachments)
        {
            if (ksCmsAttachment.AttachmentIsUnsorted != true || ksCmsAttachment.AttachmentGroupGUID != null)
            {
                // those must be migrated with pages
                continue;
            }

            (_, bool canContinue, _, _) = attachmentMigrator.MigrateAttachment(ksCmsAttachment);
            if (!canContinue)
            {
                break;
            }
        }

        return Task.FromResult<CommandResult>(new GenericCommandResult());
    }
}
