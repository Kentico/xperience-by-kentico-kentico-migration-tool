using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.KX12.Context;
using Migration.Tool.KXP.Api.Enums;
using Migration.Tool.KXP.Models;

namespace Migration.Tool.Core.KX12.Handlers;

public class MigrateUsersCommandHandler(
    ILogger<MigrateUsersCommandHandler> logger,
    IDbContextFactory<KX12Context> kx12ContextFactory,
    IEntityMapper<KX12M.CmsUser, UserInfo> userInfoMapper,
    IEntityMapper<KX12M.CmsRole, RoleInfo> roleMapper,
    IEntityMapper<KX12M.CmsUserRole, UserRoleInfo> userRoleMapper,
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
        await using var kx12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12CmsUsers = kx12Context.CmsUsers
                .Where(u => u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin)
            ;

        foreach (var k12User in k12CmsUsers)
        {
            protocol.FetchedSource(k12User);
            logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", k12User.UserName, k12User.UserGuid);

            var xbkUserInfo = UserInfoProvider.ProviderObject.Get(k12User.UserGuid);

            protocol.FetchedTarget(xbkUserInfo);

            if (k12User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin && xbkUserInfo != null)
            {
                protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(xbkUserInfo));
                logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", k12User.UserGuid);
                primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, xbkUserInfo.UserID);
                continue;
            }

            if (xbkUserInfo?.UserName == USER_PUBLIC || k12User.UserName == USER_PUBLIC)
            {
                protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(xbkUserInfo));
                logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", xbkUserInfo?.UserGUID ?? k12User.UserGuid);
                if (xbkUserInfo != null)
                {
                    primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, xbkUserInfo.UserID);
                }

                continue;
            }

            var mapped = userInfoMapper.Map(k12User, xbkUserInfo);
            protocol.MappedTarget(mapped);

            SaveUserUsingKenticoApi(mapped, k12User);
        }

        await MigrateUserCmsRoles(kx12Context, cancellationToken);

        return new GenericCommandResult();
    }

    private void SaveUserUsingKenticoApi(IModelMappingResult<UserInfo> mapped, KX12M.CmsUser k12User)
    {
        if (mapped is { Success: true } result)
        {
            (var userInfo, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(userInfo);

            try
            {
                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    logger.LogError($"User {userInfo.UserName} does not have an email set. Email is required. You can set it via admin web interface of your source instance or directly in CMS_User database table.");
                }
                UserInfoProvider.ProviderObject.Set(userInfo);

                protocol.Success(k12User, userInfo, mapped);
                logger.LogEntitySetAction(newInstance, userInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, userInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, k12User)
                    .WithData(new { k12User.UserName, k12User.UserGuid, k12User.UserId })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, userInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(userInfo)
                );
                return;
            }

            primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, userInfo.UserID);
        }
    }

    private async Task MigrateUserCmsRoles(KX12Context kx12Context, CancellationToken cancellationToken)
    {
        var k12CmsRoles = kx12Context.CmsRoles
            .Where(r =>
                r.CmsUserRoles.Any(ur => ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin)
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var k12CmsRole in k12CmsRoles.WithCancellation(cancellationToken))
        {
            protocol.FetchedSource(k12CmsRole);

            var xbkRoleInfo = RoleInfoProvider.ProviderObject.Get(k12CmsRole.RoleGuid);
            protocol.FetchedTarget(xbkRoleInfo);
            var mapped = roleMapper.Map(k12CmsRole, xbkRoleInfo);
            protocol.MappedTarget(mapped);

            if (mapped is not (var roleInfo, var newInstance) { Success: true })
            {
                continue;
            }

            ArgumentNullException.ThrowIfNull(roleInfo, nameof(roleInfo));
            try
            {
                RoleInfoProvider.ProviderObject.Set(roleInfo);

                protocol.Success(k12CmsRole, roleInfo, mapped);
                logger.LogEntitySetAction(newInstance, roleInfo);

                primaryKeyMappingContext.SetMapping<KX12M.CmsRole>(
                    r => r.RoleId,
                    k12CmsRole.RoleId,
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

            await MigrateUserRole(k12CmsRole.RoleId);
        }
    }

    private async Task MigrateUserRole(int k12RoleId)
    {
        var kx12Context = await kx12ContextFactory.CreateDbContextAsync();
        var k12UserRoles = kx12Context.CmsUserRoles
            .Where(ur =>
                ur.RoleId == k12RoleId && (
                    ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin
                )
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var k12UserRole in k12UserRoles)
        {
            protocol.FetchedSource(k12UserRole);
            if (!primaryKeyMappingContext.TryRequireMapFromSource<KX12M.CmsRole>(u => u.RoleId, k12RoleId, out int xbkRoleId))
            {
                var handbookRef = HandbookReferences
                    .MissingRequiredDependency<CmsRole>(nameof(UserRoleInfo.RoleID), k12UserRole.RoleId)
                    .NeedsManualAction();

                protocol.Append(handbookRef);
                logger.LogWarning("Unable to locate role in target instance with source RoleID '{RoleID}'", k12UserRole.RoleId);
                continue;
            }

            if (!primaryKeyMappingContext.TryRequireMapFromSource<KX12M.CmsUser>(u => u.UserId, k12UserRole.UserId, out int xbkUserId))
            {
                continue;
            }

            var xbkUserRole = UserRoleInfoProvider.ProviderObject.Get(xbkUserId, xbkRoleId);
            protocol.FetchedTarget(xbkUserRole);

            var mapped = userRoleMapper.Map(k12UserRole, xbkUserRole);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true })
            {
                (var userRoleInfo, bool newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(userRoleInfo);

                try
                {
                    UserRoleInfoProvider.ProviderObject.Set(userRoleInfo);

                    protocol.Success(k12UserRole, userRoleInfo, mapped);
                    logger.LogEntitySetAction(newInstance, userRoleInfo);
                }
                catch (Exception ex)
                {
                    logger.LogEntitySetError(ex, newInstance, userRoleInfo);
                    protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserRoleInfo>(ex)
                        .WithData(new { k12UserRole.UserRoleId, k12UserRole.UserId, k12UserRole.RoleId })
                        .WithMessage("Failed to migrate user role")
                    );
                }
            }
        }
    }
}
