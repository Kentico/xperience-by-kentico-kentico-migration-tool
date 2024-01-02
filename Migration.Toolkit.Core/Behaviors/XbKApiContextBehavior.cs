using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.Core.Behaviors;

using CMS.Base;
using CMS.Membership;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;

public class XbKApiContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : CommandResult
{
    private readonly ILogger<XbKApiContextBehavior<TRequest, TResponse>> _logger;
    private readonly IMigrationProtocol _protocol;
    private readonly KxpApiInitializer _initializer;

    public XbKApiContextBehavior(
        ILogger<XbKApiContextBehavior<TRequest, TResponse>> logger,
        IMigrationProtocol protocol,
        KxpApiInitializer initializer
    )
    {
        _logger = logger;
        _protocol = protocol;
        _initializer = initializer;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _initializer.EnsureApiIsInitialized();

        var defaultAdmin = UserInfoProvider.ProviderObject.Get(UserInfoProvider.DEFAULT_ADMIN_USERNAME);
        if (defaultAdmin == null)
        {
            _protocol.Append(HandbookReferences
                .MissingRequiredDependency<UserInfo>()
                .WithMessage($"Target XbK doesn't contain default administrator account ('{UserInfoProvider.DEFAULT_ADMIN_USERNAME}'). Default administrator account is required for migration.")
            );
            throw new InvalidOperationException($"Target XbK doesn't contain default administrator account ('{UserInfoProvider.DEFAULT_ADMIN_USERNAME}')");
        }

        using (new CMSActionContext(defaultAdmin) { User = defaultAdmin, UseGlobalAdminContext = true })
        {
            // TODO tk: 2022-11-25 revise in future
            // MembershipContext.AuthenticatedUser = defaultAdmin;

            _logger.LogInformation("Using CMSActionContext of user '{UserName}'", UserInfoProvider.DEFAULT_ADMIN_USERNAME);
            return await next();
        }
    }
}