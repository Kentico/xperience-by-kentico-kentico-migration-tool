namespace Migration.Toolkit.Core.KX12.Handlers;

using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KXP.Api.Enums;

public class MigrateUsersCommandHandler : IRequestHandler<MigrateUsersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    private readonly ILogger<MigrateUsersCommandHandler> _logger;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly IEntityMapper<KX12M.CmsUser, UserInfo> _userInfoMapper;
    private readonly IEntityMapper<KX12M.CmsRole, RoleInfo> _roleMapper;
    private readonly IEntityMapper<KX12M.CmsUserRole, UserRoleInfo> _userRoleMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private static int[] MigratedAdminUserPrivilegeLevels => new[] { (int)UserPrivilegeLevelEnum.Editor, (int)UserPrivilegeLevelEnum.Admin, (int)UserPrivilegeLevelEnum.GlobalAdmin };

    public MigrateUsersCommandHandler(
        ILogger<MigrateUsersCommandHandler> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        IEntityMapper<KX12M.CmsUser, UserInfo> userInfoMapper,
        IEntityMapper<KX12M.CmsRole, RoleInfo> roleMapper,
        IEntityMapper<KX12M.CmsUserRole, UserRoleInfo> userRoleMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kx12ContextFactory = kx12ContextFactory;
        _userInfoMapper = userInfoMapper;
        _roleMapper = roleMapper;
        _userRoleMapper = userRoleMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
    }

    public async Task<CommandResult> Handle(MigrateUsersCommand request, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12CmsUsers = kx12Context.CmsUsers
                .Where(u => MigratedAdminUserPrivilegeLevels.Contains(u.UserPrivilegeLevel))
            ;

        foreach (var k12User in k12CmsUsers)
        {
            _protocol.FetchedSource(k12User);
            _logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", k12User.UserName, k12User.UserGuid);

            var xbkUserInfo = UserInfoProvider.ProviderObject.Get(k12User.UserGuid);

            _protocol.FetchedTarget(xbkUserInfo);

            if (k12User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin && xbkUserInfo != null)
            {
                _protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(xbkUserInfo));
                _logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", k12User.UserGuid);
                _primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, xbkUserInfo.UserID);
                continue;
            }

            if (xbkUserInfo?.UserName == USER_PUBLIC || k12User.UserName == USER_PUBLIC)
            {
                _protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(xbkUserInfo));
                _logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", xbkUserInfo?.UserGUID ?? k12User.UserGuid);
                if (xbkUserInfo != null)
                {
                    _primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, xbkUserInfo.UserID);
                }

                continue;
            }

            var mapped = _userInfoMapper.Map(k12User, xbkUserInfo);
            _protocol.MappedTarget(mapped);

            await SaveUserUsingKenticoApi(cancellationToken, mapped, k12User);
        }

        await MigrateUserCmsRoles(kx12Context, cancellationToken);

        return new GenericCommandResult();
    }

    private async Task<bool> SaveUserUsingKenticoApi(CancellationToken cancellationToken, IModelMappingResult<UserInfo> mapped, KX12M.CmsUser k12User)
    {
        if (mapped is { Success: true } result)
        {
            var (userInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(userInfo);

            try
            {
                UserInfoProvider.ProviderObject.Set(userInfo);

                _protocol.Success(k12User, userInfo, mapped);
                _logger.LogEntitySetAction(newInstance, userInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                _logger.LogEntitySetError(sqlException, newInstance, userInfo);
                _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, k12User)
                    .WithData(new { k12User.UserName, k12User.UserGuid, k12User.UserId, })
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

            _primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, userInfo.UserID);
            return true;
        }

        return false;
    }

    private async Task MigrateUserCmsRoles(KX12Context KX12Context, CancellationToken cancellationToken)
    {
        var k12CmsRoles = KX12Context.CmsRoles
            .Where(r =>
                r.CmsUserRoles.Any(ur => MigratedAdminUserPrivilegeLevels.Contains(ur.User.UserPrivilegeLevel))
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var k12CmsRole in k12CmsRoles.WithCancellation(cancellationToken))
        {
            _protocol.FetchedSource(k12CmsRole);

            var xbkRoleInfo = RoleInfoProvider.ProviderObject.Get(k12CmsRole.RoleGuid);
            _protocol.FetchedTarget(xbkRoleInfo);
            var mapped = _roleMapper.Map(k12CmsRole, xbkRoleInfo);
            _protocol.MappedTarget(mapped);

            if (mapped is not (var roleInfo, var newInstance) { Success: true })
            {
                continue;
            }

            ArgumentNullException.ThrowIfNull(roleInfo, nameof(roleInfo));
            try
            {
                RoleInfoProvider.ProviderObject.Set(roleInfo);

                _protocol.Success(k12CmsRole, roleInfo, mapped);
                _logger.LogEntitySetAction(newInstance, roleInfo);

                _primaryKeyMappingContext.SetMapping<KX12M.CmsRole>(
                    r => r.RoleId,
                    k12CmsRole.RoleId,
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

            await MigrateUserRole(k12CmsRole.RoleId);
        }
    }

    private async Task MigrateUserRole(int k12RoleId)
    {
        var kx12Context = await _kx12ContextFactory.CreateDbContextAsync();
        var k12UserRoles = kx12Context.CmsUserRoles
            .Where(ur =>
                ur.RoleId == k12RoleId &&
                MigratedAdminUserPrivilegeLevels.Contains(ur.User.UserPrivilegeLevel)
            )
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var k12UserRole in k12UserRoles)
        {
            _protocol.FetchedSource(k12UserRole);
            if (!_primaryKeyMappingContext.TryRequireMapFromSource<KX12M.CmsRole>(u => u.RoleId, k12RoleId, out var xbkRoleId))
            {
                var handbookRef = HandbookReferences
                    .MissingRequiredDependency<KXP.Models.CmsRole>(nameof(UserRoleInfo.RoleID), k12UserRole.RoleId)
                    .NeedsManualAction();

                _protocol.Append(handbookRef);
                _logger.LogWarning("Unable to locate role in target instance with source RoleID '{RoleID}'", k12UserRole.RoleId);
                continue;
            }

            if (!_primaryKeyMappingContext.TryRequireMapFromSource<KX12M.CmsUser>(u => u.UserId, k12UserRole.UserId, out var xbkUserId))
            {
                continue;
            }

            var xbkUserRole = UserRoleInfoProvider.ProviderObject.Get(xbkUserId, xbkRoleId);
            _protocol.FetchedTarget(xbkUserRole);

            var mapped = _userRoleMapper.Map(k12UserRole, xbkUserRole);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success: true })
            {
                var (userRoleInfo, newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(userRoleInfo);

                try
                {
                    UserRoleInfoProvider.ProviderObject.Set(userRoleInfo);

                    _protocol.Success(k12UserRole, userRoleInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, userRoleInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogEntitySetError(ex, newInstance, userRoleInfo);
                    _protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserRoleInfo>(ex)
                        .WithData(new { k12UserRole.UserRoleId, k12UserRole.UserId, k12UserRole.RoleId, })
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