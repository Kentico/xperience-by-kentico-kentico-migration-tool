namespace Migration.Toolkit.Source.Handlers;

using MediatR;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Source.Model;
using Migration.Toolkit.Source.Services;

// ReSharper disable once UnusedMember.Global [implicit use]
public class MigrateAttachmentsCommandHandler(
    ModelFacade modelFacade,
    AttachmentMigrator attachmentMigrator
) : IRequestHandler<MigrateAttachmentsCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var kx13CmsAttachments = modelFacade.SelectAll<ICmsAttachment>();

        foreach (var kx13CmsAttachment in kx13CmsAttachments)
        {
            if (kx13CmsAttachment.AttachmentIsUnsorted != true || kx13CmsAttachment.AttachmentGroupGUID != null)
            {
                // those must be migrated with pages
                continue;
            }

            var (_, canContinue, _, _) = attachmentMigrator.MigrateAttachment(kx13CmsAttachment);
            if (!canContinue)
                break;
        }

        return new GenericCommandResult();
    }
}