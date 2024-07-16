using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX13.Contexts;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Api.Enums;
using Migration.Toolkit.KXP.Models;

namespace Migration.Toolkit.Core.KX13.Handlers;

public class MigrateUsersCommandHandler(
    ILogger<MigrateUsersCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IEntityMapper<KX13M.CmsUser, UserInfo> userInfoMapper,
    IEntityMapper<KX13M.CmsRole, RoleInfo> roleMapper,
    IEntityMapper<KX13M.CmsUserRole, UserRoleInfo> userRoleMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : IRequestHandler<MigrateUsersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    public void Dispose()
    {
    }

    public async Task<CommandResult> Handle(MigrateUsersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsUsers = kx13Context.CmsUsers
                .Where(u => UserHelper.PrivilegeLevelsMigratedAsAdminUser.Contains(u.UserPrivilegeLevel))
            ;

        foreach (var kx13User in kx13CmsUsers)
        {
            protocol.FetchedSource(kx13User);
            logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", kx13User.UserName, kx13User.UserGuid);

            var xbkUserInfo = UserInfoProvider.ProviderObject.Get(kx13User.UserGuid);

            protocol.FetchedTarget(xbkUserInfo);

            if (kx13User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin && xbkUserInfo != null)
            {
                protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(xbkUserInfo));
                logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", kx13User.UserGuid);
                primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, xbkUserInfo.UserID);
                continue;
            }

            if (xbkUserInfo?.UserName == USER_PUBLIC || kx13User.UserName == USER_PUBLIC)
            {
                protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(xbkUserInfo));
                logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", xbkUserInfo?.UserGUID ?? kx13User.UserGuid);
                if (xbkUserInfo != null)
                {
                    primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, xbkUserInfo.UserID);
                }

                continue;
            }

            var mapped = userInfoMapper.Map(kx13User, xbkUserInfo);
            protocol.MappedTarget(mapped);

            await SaveUserUsingKenticoApi(mapped, kx13User);
        }

        await MigrateUserCmsRoles(kx13Context, cancellationToken);

        return new GenericCommandResult();
    }

    private Task SaveUserUsingKenticoApi(IModelMappingResult<UserInfo> mapped, KX13M.CmsUser kx13User)
    {
        if (mapped is { Success: true } result)
        {
            (var userInfo, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(userInfo);

            try
            {
                UserInfoProvider.ProviderObject.Set(userInfo);

                protocol.Success(kx13User, userInfo, mapped);
                logger.LogEntitySetAction(newInstance, userInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, userInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13User)
                    .WithData(new { kx13User.UserName, kx13User.UserGuid, kx13User.UserId })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, userInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(userInfo)
                );
                return Task.CompletedTask;
            }

            primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, userInfo.UserID);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private async Task MigrateUserCmsRoles(KX13Context kx13Context, CancellationToken cancellationToken)
    {
        var kx13CmsRoles = kx13Context.CmsRoles
            .Where(r =>
                r.CmsUserRoles.Any(ur => UserHelper.PrivilegeLevelsMigratedAsAdminUser.Contains(ur.User.UserPrivilegeLevel))
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var kx13CmsRole in kx13CmsRoles.WithCancellation(cancellationToken))
        {
            protocol.FetchedSource(kx13CmsRole);

            var xbkRoleInfo = RoleInfoProvider.ProviderObject.Get(kx13CmsRole.RoleGuid);
            protocol.FetchedTarget(xbkRoleInfo);
            var mapped = roleMapper.Map(kx13CmsRole, xbkRoleInfo);
            protocol.MappedTarget(mapped);

            if (mapped is not (var roleInfo, var newInstance) { Success: true })
            {
                continue;
            }

            ArgumentNullException.ThrowIfNull(roleInfo, nameof(roleInfo));
            try
            {
                RoleInfoProvider.ProviderObject.Set(roleInfo);

                protocol.Success(kx13CmsRole, roleInfo, mapped);
                logger.LogEntitySetAction(newInstance, roleInfo);

                primaryKeyMappingContext.SetMapping<KX13M.CmsRole>(
                    r => r.RoleId,
                    kx13CmsRole.RoleId,
                    roleInfo.RoleID
                );
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, roleInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<RoleInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(roleInfo)
                );
                continue;
            }

            await MigrateUserRole(kx13CmsRole.RoleId);
        }
    }

    private async Task MigrateUserRole(int kx13RoleId)
    {
        var kx13Context = await kx13ContextFactory.CreateDbContextAsync();
        var kx13UserRoles = kx13Context.CmsUserRoles
            .Where(ur =>
                ur.RoleId == kx13RoleId &&
                UserHelper.PrivilegeLevelsMigratedAsAdminUser.Contains(ur.User.UserPrivilegeLevel)
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var kx13UserRole in kx13UserRoles)
        {
            protocol.FetchedSource(kx13UserRole);
            if (!primaryKeyMappingContext.TryRequireMapFromSource<KX13M.CmsRole>(u => u.RoleId, kx13RoleId, out int xbkRoleId))
            {
                var handbookRef = HandbookReferences
                    .MissingRequiredDependency<CmsRole>(nameof(UserRoleInfo.RoleID), kx13UserRole.RoleId)
                    .NeedsManualAction();

                protocol.Append(handbookRef);
                logger.LogWarning("Unable to locate role in target instance with source RoleID '{RoleID}'", kx13UserRole.RoleId);
                continue;
            }

            if (!primaryKeyMappingContext.TryRequireMapFromSource<KX13M.CmsUser>(u => u.UserId, kx13UserRole.UserId, out int xbkUserId))
            {
                continue;
            }

            var xbkUserRole = UserRoleInfoProvider.ProviderObject.Get(xbkUserId, xbkRoleId);
            protocol.FetchedTarget(xbkUserRole);

            var mapped = userRoleMapper.Map(kx13UserRole, xbkUserRole);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true })
            {
                (var userRoleInfo, bool newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(userRoleInfo);

                try
                {
                    UserRoleInfoProvider.ProviderObject.Set(userRoleInfo);

                    protocol.Success(kx13UserRole, userRoleInfo, mapped);
                    logger.LogEntitySetAction(newInstance, userRoleInfo);
                }
                catch (Exception ex)
                {
                    logger.LogEntitySetError(ex, newInstance, userRoleInfo);
                    protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserRoleInfo>(ex)
                        .WithData(new { kx13UserRole.UserRoleId, kx13UserRole.UserId, kx13UserRole.RoleId })
                        .WithMessage("Failed to migrate user role")
                    );
                }
            }
        }
    }
}
