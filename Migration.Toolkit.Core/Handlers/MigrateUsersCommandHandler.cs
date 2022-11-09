namespace Migration.Toolkit.Core.Handlers;

using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Context;
using Migration.Toolkit.KXP.Models;

public class MigrateUsersCommandHandler: IRequestHandler<MigrateUsersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    private readonly ILogger<MigrateUsersCommandHandler> _logger;
    private readonly IDbContextFactory<KxpContext> _kxpContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<KX13.Models.CmsUser, UserInfo> _userInfoMapper;
    private readonly IEntityMapper<KX13M.CmsRole, CmsRole> _roleMapper;
    private readonly IEntityMapper<KX13M.CmsUserRole, UserRoleInfo> _userRoleMapper;
    private readonly IEntityMapper<KX13M.CmsUserSite, UserSiteInfo> _userSiteMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    public MigrateUsersCommandHandler(
        ILogger<MigrateUsersCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<KX13M.CmsUser, UserInfo> userInfoMapper,
        IEntityMapper<KX13M.CmsRole, CmsRole> roleMapper,
        IEntityMapper<KX13M.CmsUserRole, UserRoleInfo> userRoleMapper,
        IEntityMapper<KX13M.CmsUserSite, UserSiteInfo> userSiteMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _userInfoMapper = userInfoMapper;
        _roleMapper = roleMapper;
        _userRoleMapper = userRoleMapper;
        _userSiteMapper = userSiteMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }

    public async Task<CommandResult> Handle(MigrateUsersCommand request, CancellationToken cancellationToken)
    {
        var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId).Keys.ToList();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        // await MigrateCmsRoles(kx13Context, cancellationToken, migratedSiteIds);

        var kx13CmsUsers = kx13Context.CmsUsers
                .Include(u => u.CmsUserRoles.Where(x => migratedSiteIds.Contains(x.Role.SiteId!.Value) || x.Role.SiteId == null))
                .ThenInclude(ur => ur.Role)
            ;

        foreach (var kx13User in kx13CmsUsers)
        {
            _protocol.FetchedSource(kx13User);
            _logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", kx13User.UserName, kx13User.UserGuid);

            // var kxoUser = await _kxpContext.CmsUsers
            //         .Include(u => u.CmsUserRoles)
            //         .ThenInclude(ur => ur.Role)
            //         .FirstOrDefaultAsync(u => u.UserGuid == kx13User.UserGuid, cancellationToken)
            //     ;

            var xbkUserInfo = UserInfoProvider.ProviderObject.Get(kx13User.UserGuid);

            _protocol.FetchedTarget(xbkUserInfo);

            if (kx13User.UserPrivilegeLevel == 3 && xbkUserInfo != null)
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

             var userSaveSuccess = await SaveUserUsingKenticoApi(cancellationToken, mapped, kx13User);
             if (userSaveSuccess)
             {
                 var xbkUserId = _primaryKeyMappingContext.RequireMapFromSource<KX13M.CmsUser>(u => u.UserId, kx13User.UserId);

                 await MigrateUserSites(kx13User.UserId, xbkUserId, migratedSiteIds, cancellationToken);
                 // await MigrateUserRoles(kx13User.UserId, xbkUserId, cancellationToken);
             }
        }

        return new GenericCommandResult();
    }

    private async Task MigrateUserRoles(int kx13UserUserId, int xbkUserId, CancellationToken cancellationToken)
    {
        await using var kx13DbContext = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13UserRoles = kx13DbContext.CmsUserRoles.Where(x => x.UserId == kx13UserUserId);

        foreach (var kx13UserRole in kx13UserRoles)
        {
            _protocol.FetchedSource(kx13UserRole);
            if (!_primaryKeyMappingContext.TryRequireMapFromSource<KX13M.CmsRole>(u => u.RoleId, kx13UserRole.RoleId, out var xbkRoleId))
            {
                var handbookRef = HandbookReferences
                    .MissingRequiredDependency<KXP.Models.CmsRole>(nameof(UserRoleInfo.RoleID), kx13UserRole.RoleId)
                    .NeedsManualAction();

                _protocol.Append(handbookRef);
                _logger.LogWarning("Unable to locate role in arget instance with source RoleID '{RoleID}'", kx13UserRole.RoleId);
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

    private async Task MigrateUserSites(int kx13UserUserId, int xbkUserId, List<int> migratedSiteIds, CancellationToken cancellationToken)
    {
        await using var kx13DbContext = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13UserSites = kx13DbContext.CmsUserSites.Where(x => x.UserId == kx13UserUserId);

        foreach (var kx13UserSite in kx13UserSites)
        {
            _protocol.FetchedSource(kx13UserSite);
            if (!migratedSiteIds.Contains(kx13UserSite.SiteId)) continue;

            var xbkSiteId = _primaryKeyMappingContext.RequireMapFromSource<KX13M.CmsSite>(u => u.SiteId, kx13UserSite.SiteId);

            var xbkUserSiteInfo = UserSiteInfoProvider.ProviderObject.Get(xbkUserId, xbkSiteId);
            _protocol.FetchedTarget(xbkUserSiteInfo);

            var mapped = _userSiteMapper.Map(kx13UserSite, xbkUserSiteInfo);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true })
            {
                var (userSiteInfo, newInstance) = mapped;
                ArgumentNullException.ThrowIfNull(userSiteInfo);

                try
                {
                    UserSiteInfoProvider.ProviderObject.Set(userSiteInfo);

                    _protocol.Success(kx13UserSite, userSiteInfo, mapped);
                    _logger.LogEntitySetAction(newInstance, userSiteInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogEntitySetError(ex, newInstance, userSiteInfo);
                    _protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserSiteInfo>(ex)
                        .WithData(new { kx13UserSite.UserSiteId, kx13UserSite.UserId, kx13UserSite.SiteId, })
                        .WithMessage("Failed to migrate user role")
                    );
                }
            }
        }
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

                await _kxpContext.DisposeAsync();
                _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);
                return false;
            }

            _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, userInfo.UserID);
            return true;
        }

        return false;
    }

    private async Task MigrateCmsRoles(KX13Context kx13Context, CancellationToken cancellationToken, List<int> migratedSiteIds)
    {
        var kx13CmsRoles = kx13Context.CmsRoles
            .Where(x => migratedSiteIds.Contains(x.SiteId!.Value) || x.SiteId == null);

        foreach (var kx13CmsRole in kx13CmsRoles)
        {
            _protocol.FetchedSource(kx13CmsRole);

            var kxoCmsRole = await _kxpContext.CmsRoles
                .FirstOrDefaultAsync(x => x.RoleGuid == kx13CmsRole.RoleGuid, cancellationToken: cancellationToken);

            _protocol.FetchedTarget(kxoCmsRole);

            var mapped = _roleMapper.Map(kx13CmsRole, kxoCmsRole);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (cmsRole, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsRole, nameof(cmsRole));

                if (newInstance)
                {
                    _kxpContext.CmsRoles.Add(cmsRole);
                }
                else
                {
                    _kxpContext.CmsRoles.Update(cmsRole);
                }

                await _kxpContext.SaveChangesAsync(cancellationToken);

                _protocol.Success(kx13CmsRole, cmsRole, mapped);
                _logger.LogEntitySetAction(newInstance, cmsRole);

                _primaryKeyMappingContext.SetMapping<KX13M.CmsRole>(
                    r => r.RoleId,
                    kx13CmsRole.RoleId,
                    cmsRole.RoleId
                );
            }
            // TODO tk: 2022-07-15 handle error
        }
    }

    public void Dispose()
    {
        _kxpContext.Dispose();
    }
}