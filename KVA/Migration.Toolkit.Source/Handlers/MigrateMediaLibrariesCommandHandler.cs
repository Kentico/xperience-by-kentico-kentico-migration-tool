using MediatR;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Source.Services;

namespace Migration.Toolkit.Source.Handlers;

public class MigrateMediaLibrariesCommandHandler(
   IMediaFileMigrator mediaFileMigrator
    )
    : IRequestHandler<MigrateMediaLibrariesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken) => await mediaFileMigrator.Handle(request, cancellationToken);
}
