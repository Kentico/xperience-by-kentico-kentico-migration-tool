using MediatR;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Source.Services;

namespace Migration.Tool.Source.Handlers;

public class MigrateMediaLibrariesCommandHandler(
   IMediaFileMigrator mediaFileMigrator
    )
    : IRequestHandler<MigrateMediaLibrariesCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken) => await mediaFileMigrator.Handle(request, cancellationToken);
}
