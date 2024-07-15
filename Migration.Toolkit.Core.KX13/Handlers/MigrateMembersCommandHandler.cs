namespace Migration.Toolkit.Core.KX13.Handlers;

using System.Diagnostics;
using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX13.Contexts;
using Migration.Toolkit.Core.KX13.Mappers;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Api.Enums;

public class MigrateMembersCommandHandler(
    ILogger<MigrateMembersCommandHandler> logger,
    IDbContextFactory<KX13Context> kx13ContextFactory,
    IEntityMapper<MemberInfoMapperSource, MemberInfo> memberInfoMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : IRequestHandler<MigrateMembersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    public async Task<CommandResult> Handle(MigrateMembersCommand request, CancellationToken cancellationToken)
    {
        await using var kx13Context = await kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        var kx13CmsUsers = kx13Context.CmsUsers
                .Include(u => u.CmsUserSettingUserSettingsUserNavigation)
                .Where(u => UserHelper.PrivilegeLevelsMigratedAsMemberUser.Contains(u.UserPrivilegeLevel))
            ;

        foreach (var kx13User in kx13CmsUsers)
        {
            protocol.FetchedSource(kx13User);
            logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid} to member", kx13User.UserName, kx13User.UserGuid);

            var xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(kx13User.UserGuid);

            protocol.FetchedTarget(xbkMemberInfo);

            // no member shall be admin, editor
            Debug.Assert(kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin, "kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin");
            Debug.Assert(kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin, "kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin");
            Debug.Assert(kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor, "kx13User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor");

            if (xbkMemberInfo?.MemberName == USER_PUBLIC || kx13User.UserName == USER_PUBLIC)
            {
                continue;
            }

            var mapped = memberInfoMapper.Map(new MemberInfoMapperSource(kx13User, kx13User.CmsUserSettingUserSettingsUserNavigation), xbkMemberInfo);
            protocol.MappedTarget(mapped);

            SaveUserUsingKenticoApi(mapped, kx13User);
        }

        return new GenericCommandResult();
    }

    private void SaveUserUsingKenticoApi(IModelMappingResult<MemberInfo> mapped, KX13M.CmsUser kx13User)
    {
        if (mapped is { Success: true } result)
        {
            var (memberInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(memberInfo);

            try
            {
                MemberInfoProvider.ProviderObject.Set(memberInfo);

                protocol.Success(kx13User, memberInfo, mapped);
                logger.LogEntitySetAction(newInstance, memberInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, memberInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13User)
                    .WithData(new { kx13User.UserName, kx13User.UserGuid, kx13User.UserId, })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
                return;
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, memberInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(memberInfo)
                );
                return;
            }

            // left for OM_Activity
            primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, memberInfo.MemberID);
            return;
        }
    }

    public void Dispose()
    {

    }
}