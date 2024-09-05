using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;

namespace Migration.Toolkit.Source.Services;

public interface IMediaFileMigrator
{
    Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken);
}
