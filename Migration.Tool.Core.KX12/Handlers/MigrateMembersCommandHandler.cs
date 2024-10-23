using System.Diagnostics;

using CMS.Membership;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX12.Contexts;
using Migration.Tool.Core.KX12.Mappers;
using Migration.Tool.KX12.Context;
using Migration.Tool.KXP.Api.Enums;

namespace Migration.Tool.Core.KX12.Handlers;

public class MigrateMembersCommandHandler(
    ILogger<MigrateMembersCommandHandler> logger,
    IDbContextFactory<KX12Context> kx12ContextFactory,
    IEntityMapper<MemberInfoMapperSource, MemberInfo> memberInfoMapper,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol)
    : IRequestHandler<MigrateMembersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    public void Dispose()
    {
    }

    public async Task<CommandResult> Handle(MigrateMembersCommand request, CancellationToken cancellationToken)
    {
        await using var kx12Context = await kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        var k12CmsUsers = kx12Context.CmsUsers
                .Include(u => u.CmsUserSettingUserSettingsUserNavigation)
                .Where(u => u.UserPrivilegeLevel == (int)UserPrivilegeLevelEnum.None)
            ;

        foreach (var k12User in k12CmsUsers)
        {
            protocol.FetchedSource(k12User);
            logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid} to member", k12User.UserName, k12User.UserGuid);

            var xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(k12User.UserGuid);

            protocol.FetchedTarget(xbkMemberInfo);

            // no member shall be admin, editor
            Debug.Assert(k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin, "k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin");
            Debug.Assert(k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin, "k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin");
            Debug.Assert(k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor, "k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor");

            if (xbkMemberInfo?.MemberName == USER_PUBLIC || k12User.UserName == USER_PUBLIC)
            {
                continue;
            }

            var mapped = memberInfoMapper.Map(new MemberInfoMapperSource(k12User, k12User.CmsUserSettingUserSettingsUserNavigation), xbkMemberInfo);
            protocol.MappedTarget(mapped);

            SaveUserUsingKenticoApi(mapped, k12User);
        }

        return new GenericCommandResult();
    }

    private void SaveUserUsingKenticoApi(IModelMappingResult<MemberInfo> mapped, KX12M.CmsUser k12User)
    {
        if (mapped is { Success: true } result)
        {
            (var memberInfo, bool newInstance) = result;
            ArgumentNullException.ThrowIfNull(memberInfo);

            try
            {
                MemberInfoProvider.ProviderObject.Set(memberInfo);

                protocol.Success(k12User, memberInfo, mapped);
                logger.LogEntitySetAction(newInstance, memberInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, memberInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, k12User)
                    .WithData(new { k12User.UserName, k12User.UserGuid, k12User.UserId })
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
            primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, memberInfo.MemberID);
        }
    }
}
