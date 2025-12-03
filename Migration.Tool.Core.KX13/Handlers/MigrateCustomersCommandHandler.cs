using CMS.Commerce;
using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.Core.KX13.Mappers;
using Migration.Tool.KX13.Context;
using Migration.Tool.KX13.Models;

namespace Migration.Tool.Core.KX13.Handlers;

public class MigrateCustomersCommandHandler(
    ILogger<MigrateCustomersCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IEntityMapper<CustomerInfoMapperSource, CustomerInfo> customerInfoMapper,
    IEntityMapper<CustomerAddressInfoMapperSource, CustomerAddressInfo> customerAddressInfoMapper,
    IProtocol protocol,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    ToolConfiguration toolConfiguration)
    : IRequestHandler<MigrateCustomersCommand, CommandResult>
{
    public async Task<CommandResult> Handle(MigrateCustomersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13FormsContext = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13CommerceSite = await GetCommerceSite(cancellationToken);

        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var kx13Customers = kx13Context.ComCustomers
            .Where(c => c.CustomerSiteId == kx13CommerceSite.SiteId || !c.CustomerSiteId.HasValue)
            .Include(c => c.ComAddresses)
            .Include(c => c.ComOrders)
            .Include(c => c.CustomerUser);

        foreach (var kx13Customer in kx13Customers)
        {
            protocol.FetchedSource(kx13Customer);
            logger.LogTrace("Migrating customer {CustomerId} with CustomerGuid {CustomerGuid}", kx13Customer.CustomerId, kx13Customer.CustomerGuid);

            // There has to be separate context for every collection
            await using var kx13UserContext = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

            var kx13User = kx13UserContext.CmsUsers.FirstOrDefault(u => u.UserId == kx13Customer.CustomerUserId);
            var xbkCustomerInfo = CustomerInfo.Provider.Get()
                .WhereEquals(nameof(CustomerInfo.CustomerGUID), kx13Customer.CustomerGuid).FirstOrDefault();

            MemberInfo? xbkMemberInfo = null;
            if (kx13User is not null)
            {
                xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(kx13User.UserGuid);
                protocol.FetchedTarget(xbkMemberInfo);
            }

            var mapped = customerInfoMapper.Map(new CustomerInfoMapperSource(kx13Customer, kx13CommerceSite.SiteName, xbkMemberInfo), xbkCustomerInfo);
            protocol.MappedTarget(mapped);

            SaveUserUsingKenticoApi(mapped!, kx13User);

            if (mapped is { Success: true, Item: CustomerInfo customerInfo })
            {
                foreach (var kx13CustomerAddress in kx13Customer.ComAddresses)
                {
                    var xbkCustomerAddress = CustomerAddressInfo.Provider.Get().WhereEquals(nameof(CustomerAddressInfo.CustomerAddressGUID), kx13CustomerAddress.AddressGuid).FirstOrDefault();
                    var mappedAddresses = customerAddressInfoMapper.Map(new CustomerAddressInfoMapperSource(kx13CustomerAddress, customerInfo), xbkCustomerAddress);
                    protocol.MappedTarget(mappedAddresses);
                    SaveCustomerAddressUsingKenticoApi(mappedAddresses!, kx13CustomerAddress);
                }
            }
        }

        return new GenericCommandResult();
    }


    private async Task<CmsSite> GetCommerceSite(CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);
        var commerceSiteName = toolConfiguration.CommerceConfiguration?.CommerceSiteName ?? string.Empty;
        var commerceSite = kx13Context.CmsSites.FirstOrDefault(s => s.SiteName == commerceSiteName);

        return commerceSite ?? throw new InvalidOperationException($"Commerce site {commerceSiteName} not found.");
    }


    private void SaveUserUsingKenticoApi(IModelMappingResult<CustomerInfo> mapped, CmsUser? kx13User)
    {
        if (mapped is { Success: true } result)
        {
            (var customerInfo, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(customerInfo);

            try
            {
                CustomerInfo.Provider.Set(customerInfo);

                protocol.Success(kx13User, customerInfo, mapped);
                logger.LogEntitySetAction(newInstance, customerInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, customerInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13User)
                    .WithData(kx13User is not null ? new { kx13User.UserName, kx13User.UserGuid, kx13User.UserId } : new { })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, customerInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(customerInfo)
                );
                return;
            }

            // left for OM_Activity
            if (kx13User is not null)
            {
                primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, kx13User.UserId, customerInfo.CustomerID);
            }
        }
    }


    private void SaveCustomerAddressUsingKenticoApi(IModelMappingResult<CustomerAddressInfo> mapped, ComAddress kx13CustomerAddress)
    {
        if (mapped is { Success: true } result)
        {
            (var customerAddressInfo, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(customerAddressInfo);

            try
            {
                CustomerAddressInfo.Provider.Set(customerAddressInfo);

                protocol.Success(kx13CustomerAddress, customerAddressInfo, mapped);
                logger.LogEntitySetAction(newInstance, customerAddressInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, customerAddressInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13CustomerAddress)
                    .WithData(new { kx13CustomerAddress.AddressId, kx13CustomerAddress.AddressGuid })
                    .WithMessage("Failed to migrate customer address, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, customerAddressInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<CustomerAddressInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(customerAddressInfo)
                );
                return;
            }

            // Map primary key for future reference
            primaryKeyMappingContext.SetMapping<ComAddress>(r => r.AddressId, kx13CustomerAddress.AddressId, customerAddressInfo.CustomerAddressID);
        }
    }
}
