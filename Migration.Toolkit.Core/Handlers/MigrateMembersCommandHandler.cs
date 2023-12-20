namespace Migration.Toolkit.Core.Handlers;

using System.Diagnostics;
using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.Mappers;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Api.Enums;

public class MigrateMembersCommandHandler : IRequestHandler<MigrateMembersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    private readonly ILogger<MigrateMembersCommandHandler> _logger;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<MemberInfoMapperSource, MemberInfo> _memberInfoMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private static int[] MigratedAdminUserPrivilegeLevels => new[] { (int)UserPrivilegeLevelEnum.None };

    public MigrateMembersCommandHandler(
        ILogger<MigrateMembersCommandHandler> logger,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<MemberInfoMapperSource, MemberInfo> memberInfoMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kx13ContextFactory = kx13ContextFactory;
        _memberInfoMapper = memberInfoMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
    }

    public async Task<CommandResult> Handle(MigrateMembersCommand request, CancellationToken cancellationToken)
    {
        // var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId).Keys.ToList();

        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        // TODO tomas.krch: 2023-04-11 query only users from particular sites !!
        var kx13CmsUsers = kx13Context.CmsUsers
                .Include(u => u.CmsUserSettingUserSettingsUserNavigation)
                .Where(u => MigratedAdminUserPrivilegeLevels.Contains(u.UserPrivilegeLevel))
            ;

        // TODO tomas.krch: 2023-04-11 migrate custom values
        // TODO tomas.krch: 2023-04-11 migrate deprecated sys value if desired
        // TODO tomas.krch: 2023-04-11 migrate inside module migration???

        foreach (var kx13User in kx13CmsUsers)
        {
            _protocol.FetchedSource(kx13User);
            _logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid} to member", kx13User.UserName, kx13User.UserGuid);

            var xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(kx13User.UserGuid);

            _protocol.FetchedTarget(xbkMemberInfo);

            // no member shall be admin, editor
            Debug.Assert(kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin, "kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin");
            Debug.Assert(kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin, "kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin");
            Debug.Assert(kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor, "kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor");

            // if (kx13User.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.GlobalAdmin && xbkMemberInfo != null)
            // {
            //     _protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(xbkMemberInfo));
            //     _logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", kx13User.UserGuid);
            //     _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, xbkMemberInfo.MemberID);
            //     continue;
            // }

            // TODO tomas.krch: 2023-04-11 check public user
            if (xbkMemberInfo?.MemberName == USER_PUBLIC || kx13User.UserName == USER_PUBLIC)
            {
                var c = 1;
                // _protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(xbkMemberInfo));
                // _logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", xbkMemberInfo?.UserGUID ?? kx13User.UserGuid);
                // if (xbkMemberInfo != null)
                // {
                //     _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, xbkMemberInfo.UserID);
                // }

                continue;
            }

            var mapped = _memberInfoMapper.Map(new MemberInfoMapperSource(kx13User, kx13User.CmsUserSettingUserSettingsUserNavigation), xbkMemberInfo);
            _protocol.MappedTarget(mapped);

            await SaveUserUsingKenticoApi(cancellationToken, mapped, kx13User);
        }

        return new GenericCommandResult();
    }

    // OBSOLETE 26.0.0
    // private async Task MigrateUserSites(int kx13UserUserId, int xbkUserId, List<int> migratedSiteIds, CancellationToken cancellationToken)
    // {
    //     await using var kx13DbContext = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);
    //
    //     var kx13UserSites = kx13DbContext.CmsUserSites
    //         .Where(x => x.UserId == kx13UserUserId)
    //         .AsNoTracking()
    //         .AsAsyncEnumerable();
    //
    //     await foreach (var kx13UserSite in kx13UserSites.WithCancellation(cancellationToken))
    //     {
    //         _protocol.FetchedSource(kx13UserSite);
    //         if (!migratedSiteIds.Contains(kx13UserSite.SiteId)) continue;
    //
    //         var xbkSiteId = _primaryKeyMappingContext.RequireMapFromSource<KX13M.CmsSite>(u => u.SiteId, kx13UserSite.SiteId);
    //
    //         var xbkUserSiteInfo = UserSiteInfoProvider.ProviderObject.Get(xbkUserId, xbkSiteId);
    //         _protocol.FetchedTarget(xbkUserSiteInfo);
    //
    //         var mapped = _userSiteMapper.Map(kx13UserSite, xbkUserSiteInfo);
    //         _protocol.MappedTarget(mapped);
    //
    //         if (mapped is { Success : true })
    //         {
    //             var (userSiteInfo, newInstance) = mapped;
    //             ArgumentNullException.ThrowIfNull(userSiteInfo);
    //
    //             try
    //             {
    //                 UserSiteInfoProvider.ProviderObject.Set(userSiteInfo);
    //
    //                 _protocol.Success(kx13UserSite, userSiteInfo, mapped);
    //                 _logger.LogEntitySetAction(newInstance, userSiteInfo);
    //             }
    //             catch (Exception ex)
    //             {
    //                 _logger.LogEntitySetError(ex, newInstance, userSiteInfo);
    //                 _protocol.Append(HandbookReferences.ErrorSavingTargetInstance<UserSiteInfo>(ex)
    //                     .WithData(new { kx13UserSite.UserSiteId, kx13UserSite.UserId, kx13UserSite.SiteId, })
    //                     .WithMessage("Failed to migrate user role")
    //                 );
    //             }
    //         }
    //     }
    // }

    private async Task<bool> SaveUserUsingKenticoApi(CancellationToken cancellationToken, IModelMappingResult<MemberInfo> mapped, KX13.Models.CmsUser kx13User)
    {
        if (mapped is { Success : true } result)
        {
            var (memberInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(memberInfo);

            try
            {
                MemberInfoProvider.ProviderObject.Set(memberInfo);

                _protocol.Success(kx13User, memberInfo, mapped);
                _logger.LogEntitySetAction(newInstance, memberInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                _logger.LogEntitySetError(sqlException, newInstance, memberInfo);
                _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13User)
                    .WithData(new { kx13User.UserName, kx13User.UserGuid, kx13User.UserId, })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogEntitySetError(ex, newInstance, memberInfo);
                _protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(memberInfo)
                );
                return false;
            }

            // left for OM_Activity
            _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, memberInfo.MemberID);
            return true;
        }

        return false;
    }

    public void Dispose()
    {

    }
}