namespace Migration.Toolkit.Core.K11.Handlers;

using System.Diagnostics;
using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Mappers;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api.Auxiliary;
using Migration.Toolkit.KXP.Api.Enums;

public class MigrateMembersCommandHandler(ILogger<MigrateMembersCommandHandler> logger,
        IDbContextFactory<K11Context> k11ContextFactory,
        IEntityMapper<MemberInfoMapperSource, MemberInfo> memberInfoMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol)
    : IRequestHandler<MigrateMembersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    public async Task<CommandResult> Handle(MigrateMembersCommand request, CancellationToken cancellationToken)
    {
        await using var k11Context = await k11ContextFactory.CreateDbContextAsync(cancellationToken);

        var k11CmsUsers = k11Context.CmsUsers
                .Include(u => u.CmsUserSettingUserSettingsUserNavigation)
                .Where(u => UserHelper.PrivilegeLevelsMigratedAsMemberUser.Contains(u.UserPrivilegeLevel))
            ;

        foreach (var k11User in k11CmsUsers)
        {
            protocol.FetchedSource(k11User);
            logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid} to member", k11User.UserName, k11User.UserGuid);

            var xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(k11User.UserGuid);

            protocol.FetchedTarget(xbkMemberInfo);


            if (xbkMemberInfo?.MemberName == USER_PUBLIC || k11User.UserName == USER_PUBLIC)
            {
                continue;
            }

            var mapped = memberInfoMapper.Map(new MemberInfoMapperSource(k11User, k11User.CmsUserSettingUserSettingsUserNavigation), xbkMemberInfo);
            protocol.MappedTarget(mapped);

            SaveUserUsingKenticoApi(mapped, k11User);
        }

        return new GenericCommandResult();
    }

    private void SaveUserUsingKenticoApi(IModelMappingResult<MemberInfo> mapped, CmsUser k11User)
    {
        if (mapped is { Success: true } result)
        {
            var (memberInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(memberInfo);

            try
            {
                MemberInfoProvider.ProviderObject.Set(memberInfo);

                protocol.Success(k11User, memberInfo, mapped);
                logger.LogEntitySetAction(newInstance, memberInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                logger.LogEntitySetError(sqlException, newInstance, memberInfo);
                protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, k11User)
                    .WithData(new { k11User.UserName, k11User.UserGuid, k11User.UserId, })
                    .WithMessage("Failed to migrate user, target database broken.")
                );
            }
            catch (Exception ex)
            {
                logger.LogEntitySetError(ex, newInstance, memberInfo);
                protocol.Append(HandbookReferences
                    .ErrorCreatingTargetInstance<UserInfo>(ex)
                    .NeedsManualAction()
                    .WithIdentityPrint(memberInfo)
                );
            }

            // left for OM_Activity
            primaryKeyMappingContext.SetMapping<CmsUser>(r => r.UserId, k11User.UserId, memberInfo.MemberID);
        }
    }

    public void Dispose()
    {

    }
}