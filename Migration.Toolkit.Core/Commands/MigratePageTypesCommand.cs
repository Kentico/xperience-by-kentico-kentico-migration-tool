using MediatR;
using Migration.Toolkit.Core.Abstractions;

namespace Migration.Toolkit.Core.Commands;

public record MigratePageTypesCommand(): IRequest<MigratePageTypesResult>
{
    public static string Moniker => "page-types";
}