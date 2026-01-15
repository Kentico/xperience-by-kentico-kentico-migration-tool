using CMS;
using CMS.Core;
using CMS.Helpers.Internal;

using Migration.Tool.KXP.Api;

[assembly: RegisterImplementation(typeof(IAdministrationDomainProvider), typeof(FakeAdministrationDomainProvider), Lifestyle = Lifestyle.Singleton, Priority = RegistrationPriority.Default)]

namespace Migration.Tool.KXP.Api;

/// <summary>
/// Provides a fake implementation of the IAdministrationDomainProvider interface. 
/// </summary>
/// <remarks>
/// This is done because Migration tool reference Kentico.Xperience.Admin nuget package.
/// When Order update in MigrateOrdersCommandHandler is perform the event in Kentico is triggered.
/// The service in this event is resolved and has dependency for HttpContextAccessor, which is not available,
/// therfore it can't be resolved and Exception is thrown.
/// NOTE: when the Kentico.Xperience.Admin is removed from the project this service can be deleted.
/// </remarks>
internal class FakeAdministrationDomainProvider : IAdministrationDomainProvider
{
    /// <inheritdoc />
    public string Get() => string.Empty;
}
