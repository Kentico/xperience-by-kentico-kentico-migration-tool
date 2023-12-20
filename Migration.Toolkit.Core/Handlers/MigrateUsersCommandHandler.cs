namespace Migration.Toolkit.Core.Handlers;

using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Api.Enums;

public class MigrateUsersCommandHandler : IRequestHandler<MigrateUsersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    private readonly ILogger<MigrateUsersCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<KX13.Models.CmsUser, UserInfo> _userInfoMapper;
    private readonly IEntityMapper<KX13M.CmsRole, RoleInfo> _roleMapper;
    private readonly IEntityMapper<KX13M.CmsUserRole, UserRoleInfo> _userRoleMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private static int[] MigratedAdminUserPrivilegeLevels => new[] { (int)UserPrivilegeLevelEnum.Editor, (int)UserPrivilegeLevelEnum.Admin, (int)UserPrivilegeLevelEnum.GlobalAdmin };

    public MigrateUsersCommandHandler(
        ILogger<MigrateUsersCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<KX13M.CmsUser, UserInfo> userInfoMapper,
        IEntityMapper<KX13M.CmsRole, RoleInfo> roleMapper,
        IEntityMapper<KX13M.CmsUserRole, UserRoleInfo> userRoleMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _userInfoMapper = userInfoMapper;
        _roleMapper = roleMapper;
        _userRoleMapper = userRoleMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
    }

    public async Task<CommandResult> Handle(MigrateUsersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsUsers = kx13Context.CmsUsers
                .Where(u => MigratedAdminUserPrivilegeLevels.Contains(u.UserPrivilegeLevel))
            ;

        foreach (var kx13User in kx13CmsUsers)
        {
            _protocol.FetchedSource(kx13User);
            _logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", kx13User.UserName, kx13User.UserGuid);

            var xbkUserInfo = UserInfoProvider.ProviderObject.Get(kx13User.UserGuid);

            _protocol.FetchedTarget(xbkUserInfo);

            if (kx13User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin && xbkUserInfo != null)
            {
                _protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(xbkUserInfo));
                _logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", kx13User.UserGuid);
                _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, xbkUserInfo.UserID);
                continue;
            }

            if (xbkUserInfo?.UserName == USER_PUBLIC || kx13User.UserName == USER_PUBLIC)
            {
                _protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(xbkUserInfo));
                _logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", xbkUserInfo?.UserGUID ?? kx13User.UserGuid);
                if (xbkUserInfo != null)
                {
                    _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, xbkUserInfo.UserID);
                }

                continue;
            }

            var mapped = _userInfoMapper.Map(kx13User, xbkUserInfo);
            _protocol.MappedTarget(mapped);

            await SaveUserUsingKenticoApi(cancellationToken, mapped, kx13User);
        }

        await MigrateUserCmsRoles(kx13Context, cancellationToken);

        return new GenericCommandResult();
    }

    private async Task<bool> SaveUserUsingKenticoApi(CancellationToken cancellationToken, IModelMappingResult<UserInfo> mapped, KX13.Models.CmsUser kx13User)
    {
        if (mapped is { Success : true } result)
        {
            var (userInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(userInfo);

            try
            {
                UserInfoProvider.ProviderObject.Set(userInfo);

                _protocol.Success(kx13User, userInfo, mapped);
                _logger.LogEntitySetAction(newInstance, userInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                _logger.LogEntitySetError(sqlException, newInstance, userInfo);
                _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13User)
                    .WithData(new { kx13User.UserName, kx13User.UserGuid, kx13User.UserId, })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogEntitySetError(ex, newInstance, userInfo);
                _protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(userInfo)
                );
                return false;
            }

            _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, userInfo.UserID);
            return true;
        }

        return false;
    }

    private async Task MigrateUserCmsRoles(KX13Context kx13Context, CancellationToken cancellationToken)
    {
        var kx13CmsRoles = kx13Context.CmsRoles
            .Where(r =>
                r.CmsUserRoles.Any(ur => MigratedAdminUserPrivilegeLevels.Contains(ur.User.UserPrivilegeLevel))
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var kx13CmsRole in kx13CmsRoles.WithCancellation(cancellationToken))
        {
            _protocol.FetchedSource(kx13CmsRole);

            var xbkRoleInfo = RoleInfoProvider.ProviderObject.Get(kx13CmsRole.RoleGuid);
            _protocol.FetchedTarget(xbkRoleInfo);
            var mapped = _roleMapper.Map(kx13CmsRole, xbkRoleInfo);
            _protocol.MappedTarget(mapped);

            if (mapped is not (var roleInfo, var newInstance) { Success : true })
            {
                continue;
            }

            ArgumentNullException.ThrowIfNull(roleInfo, nameof(roleInfo));
            try
            {
                RoleInfoProvider.ProviderObject.Set(roleInfo);

                _protocol.Success(kx13CmsRole, roleInfo, mapped);
                _logger.LogEntitySetAction(newInstance, roleInfo);

                _primaryKeyMappingContext.SetMapping<KX13M.CmsRole>(
                    r => r.RoleId,
                    kx13CmsRole.RoleId,
                    roleInfo.RoleID
                );
            }
            catch (Exception ex)
            {
                _logger.LogEntitySetError(ex, newInstance, roleInfo);
                _protocol.Append(HandbookReferences
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
        var kx13Context = await _kx13ContextFactory.CreateDbContextAsync();
        var kx13UserRoles = kx13Context.CmsUserRoles
            .Where(ur =>
                ur.RoleId == kx13RoleId &&
                MigratedAdminUserPrivilegeLevels.Contains(ur.User.UserPrivilegeLevel)
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var kx13UserRole in kx13UserRoles)
        {
            _protocol.FetchedSource(kx13UserRole);
            if (!_primaryKeyMappingContext.TryRequireMapFromSource<KX13M.CmsRole>(u => u.RoleId, kx13RoleId, out var xbkRoleId))
            {
                var handbookRef = HandbookReferences
                    .MissingRequiredDependency<KXP.Models.CmsRole>(nameof(UserRoleInfo.RoleID), kx13UserRole.RoleId)
                    .NeedsManualAction();

                _protocol.Append(handbookRef);
                _logger.LogWarning("Unable to locate role in target instance with source RoleID '{RoleID}'", kx13UserRole.RoleId);
                continue;
            }

            if (!_primaryKeyMappingContext.TryRequireMapFromSource<KX13M.CmsUser>(u => u.UserId, kx13UserRole.UserId, out var xbkUserId))
            {
                continue;
            }

            var xbkUserRole = UserRoleInfoProvider.ProviderObject.Get(xbkUserId, xbkRoleId);
            _protocol.FetchedTarget(xbkUserRole);

            var mapped = _userRoleMapper.Map(kx13UserRole, xbkUserRole);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true })
            {
                var (userRoleInfo, newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(userRoleInfo);

                try
                {
                    UserRoleInfoProvider.ProviderObject.Set(userRoleInfo);

                    _protocol.Success(kx13UserRole, userRoleInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, userRoleInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogEntitySetError(ex, newInstance, userRoleInfo);
                    _protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserRoleInfo>(ex)
                        .WithData(new { kx13UserRole.UserRoleId, kx13UserRole.UserId, kx13UserRole.RoleId, })
                        .WithMessage("Failed to migrate user role")
                    );
                }
            }
        }
    }

    public void Dispose()
    {

    }
}