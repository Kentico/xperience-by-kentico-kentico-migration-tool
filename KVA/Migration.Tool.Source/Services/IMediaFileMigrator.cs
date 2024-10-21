using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;

namespace Migration.Tool.Source.Services;

public interface IMediaFileMigrator
{
    Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken);
}
