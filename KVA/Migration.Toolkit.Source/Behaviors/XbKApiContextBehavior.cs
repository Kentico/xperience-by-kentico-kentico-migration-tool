using CMS.Base;
using CMS.Membership;

using MediatR;

using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;

namespace Migration.Toolkit.Source.Behaviors;

public class XbKApiContextBehavior<TRequest, TResponse>(
    ILogger<XbKApiContextBehavior<TRequest, TResponse>> logger,
    IMigrationProtocol protocol,
    KxpApiInitializer initializer)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        initializer.EnsureApiIsInitialized();

        var defaultAdmin = UserInfoProvider.ProviderObject.Get(UserInfoProvider.DEFAULT_ADMIN_USERNAME);
        if (defaultAdmin == null)
        {
            protocol.Append(HandbookReferences
                .MissingRequiredDependency<UserInfo>()
                .WithMessage($"Target XbK doesn't contain default administrator account ('{UserInfoProvider.DEFAULT_ADMIN_USERNAME}'). Default administrator account is required for migration.")
            );
            throw new InvalidOperationException($"Target XbK doesn't contain default administrator account ('{UserInfoProvider.DEFAULT_ADMIN_USERNAME}')");
        }

        using (new CMSActionContext(defaultAdmin) { User = defaultAdmin, UseGlobalAdminContext = true })
        {
            logger.LogInformation("Using CMSActionContext of user '{UserName}'", UserInfoProvider.DEFAULT_ADMIN_USERNAME);
            return await next();
        }
    }
}
