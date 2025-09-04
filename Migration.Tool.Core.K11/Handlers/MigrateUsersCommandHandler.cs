using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11;
using Migration.Tool.K11.Models;
using Migration.Tool.KXP.Api.Enums;

namespace Migration.Tool.Core.K11.Handlers;

public class MigrateUsersCommandHandler(
    ILogger<MigrateUsersCommandHandler> logger,
    IDbContextFactory<K11Context> k11ContextFactory,
    IEntityMapper<CmsUser, UserInfo> userInfoMapper,
    IEntityMapper<CmsRole, RoleInfo> roleMapper,
    IEntityMapper<CmsUserRole, UserRoleInfo> userRoleMapper,
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
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var k11CmsUsers = k11Context.CmsUsers
                .Where(u => u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin)
            ;

        foreach (var k11User in k11CmsUsers)
        {
            protocol.FetchedSource(k11User);
            logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", k11User.UserName, k11User.UserGuid);

            var xbkUserInfo = UserInfoProvider.ProviderObject.Get(k11User.UserGuid);

            protocol.FetchedTarget(xbkUserInfo);

            if (k11User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin && xbkUserInfo != null)
            {
                protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(xbkUserInfo));
                logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", k11User.UserGuid);
                primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, k11User.UserId, xbkUserInfo.UserID);
                continue;
            }

            if (xbkUserInfo?.UserName == USER_PUBLIC || k11User.UserName == USER_PUBLIC)
            {
                protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(xbkUserInfo));
                logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", xbkUserInfo?.UserGUID ?? k11User.UserGuid);
                if (xbkUserInfo != null)
                {
                    primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, k11User.UserId, xbkUserInfo.UserID);
                }

                continue;
            }

            var mapped = userInfoMapper.Map(k11User, xbkUserInfo);
            protocol.MappedTarget(mapped);

            SaveUserUsingKenticoApi(mapped!, k11User);
        }

        await MigrateUserCmsRoles(k11Context, cancellationToken);

        return new GenericCommandResult();
    }

    private bool SaveUserUsingKenticoApi(IModelMappingResult<UserInfo> mapped, CmsUser k11User)
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

                protocol.Success(k11User, userInfo, mapped);
                logger.LogEntitySetAction(newInstance, userInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, userInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, k11User)
                    .WithData(new { k11User.UserName, k11User.UserGuid, k11User.UserId })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return false;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, userInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(userInfo)
                );
                return false;
            }

            primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, k11User.UserId, userInfo.UserID);
            return true;
        }

        return false;
    }

    private async Task MigrateUserCmsRoles(K11Context k11Context, CancellationToken cancellationToken)
    {
        var groupedRoles = k11Context.CmsRoles
            .Where(r =>
                r.CmsUserRoles.Any(ur => ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin)
            )
            .GroupBy(x => x.RoleName)
            .AsNoTracking();

        var skippedRoles = new HashSet<Guid>();
        foreach (var groupedRole in groupedRoles)
        {
            if (groupedRole.Count() <= 1)
            {
                continue;
            }

            logger.LogError("""
                            Roles with RoleGuid ({RoleGuids}) have same RoleName '{RoleName}', due to removal of sites and role globalization it is required to set unique RoleName
                            """, string.Join(",", groupedRole.Select(l => l.RoleGuid)), groupedRole.Key);

            foreach (var r in groupedRole)
            {
                skippedRoles.Add(r.RoleGuid);
                protocol.Append(HandbookReferences.NotCurrentlySupportedSkip()
                    .WithMessage($"Role '{r.RoleName}' with RoleGUID '{r.RoleGuid}' doesn't satisfy unique RoleName condition for migration")
                    .WithIdentityPrint(r)
                    .WithData(new { r.RoleGuid, r.RoleName, r.SiteId })
                );
            }
        }

        var k11CmsRoles = k11Context.CmsRoles
            .Where(r =>
                r.CmsUserRoles.Any(ur => ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin)
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var k11CmsRole in k11CmsRoles.WithCancellation(cancellationToken))
        {
            if (skippedRoles.Contains(k11CmsRole.RoleGuid))
            {
                continue;
            }

            protocol.FetchedSource(k11CmsRole);

            var xbkRoleInfo = RoleInfoProvider.ProviderObject.Get(k11CmsRole.RoleGuid);
            protocol.FetchedTarget(xbkRoleInfo);
            var mapped = roleMapper.Map(k11CmsRole, xbkRoleInfo);
            protocol.MappedTarget(mapped);

            if (mapped is not (var roleInfo, var newInstance) { Success: true })
            {
                continue;
            }

            ArgumentNullException.ThrowIfNull(roleInfo, nameof(roleInfo));
            try
            {
                RoleInfoProvider.ProviderObject.Set(roleInfo);

                protocol.Success(k11CmsRole, roleInfo, mapped);
                logger.LogEntitySetAction(newInstance, roleInfo);

                primaryKeyMappingContext.SetMapping<CmsRole>(
                    r => r.RoleId,
                    k11CmsRole.RoleId,
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

            await MigrateUserRole(k11CmsRole.RoleId);
        }
    }

    private async Task MigrateUserRole(int k11RoleId)
    {
        var k11Context = await k11ContextFactory.CreateDbContextAsync();
        var k11UserRoles = k11Context.CmsUserRoles
            .Where(ur =>
                ur.RoleId == k11RoleId && (
                    ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Editor || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.Admin || ur.User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin
                )
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var k11UserRole in k11UserRoles)
        {
            protocol.FetchedSource(k11UserRole);
            if (!primaryKeyMappingContext.TryRequireMapFromSource<CmsRole>(u => u.RoleId, k11RoleId, out int xbkRoleId))
            {
                var handbookRef = HandbookReferences
                    .MissingRequiredDependency<RoleInfo>(nameof(UserRoleInfo.RoleID), k11UserRole.RoleId)
                    .NeedsManualAction();

                protocol.Append(handbookRef);
                logger.LogWarning("Unable to locate role in target instance with source RoleID '{RoleID}'", k11UserRole.RoleId);
                continue;
            }

            if (!primaryKeyMappingContext.TryRequireMapFromSource<CmsUser>(u => u.UserId, k11UserRole.UserId, out int xbkUserId))
            {
                continue;
            }

            var xbkUserRole = UserRoleInfoProvider.ProviderObject.Get(xbkUserId, xbkRoleId);
            protocol.FetchedTarget(xbkUserRole);

            var mapped = userRoleMapper.Map(k11UserRole, xbkUserRole);
            protocol.MappedTarget(mapped);

            if (mapped is { Success: true })
            {
                (var userRoleInfo, bool newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(userRoleInfo);

                try
                {
                    UserRoleInfoProvider.ProviderObject.Set(userRoleInfo);

                    protocol.Success(k11UserRole, userRoleInfo, mapped);
                    logger.LogEntitySetAction(newInstance, userRoleInfo);
                }
                catch (Exception ex)
                {
                    logger.LogEntitySetError(ex, newInstance, userRoleInfo);
                    protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserRoleInfo>(ex)
                        .WithData(new { k11UserRole.UserRoleId, k11UserRole.UserId, k11UserRole.RoleId })
                        .WithMessage("Failed to migrate user role")
                    );
                }
            }
        }
    }
}
