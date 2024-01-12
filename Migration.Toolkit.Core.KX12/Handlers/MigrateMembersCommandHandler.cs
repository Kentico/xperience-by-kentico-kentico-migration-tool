namespace Migration.Toolkit.Core.KX12.Handlers;

using System.Diagnostics;
using CMS.Membership;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.Core.KX12.Mappers;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KXP.Api.Enums;

public class MigrateMembersCommandHandler : IRequestHandler<MigrateMembersCommand, CommandResult>, IDisposable
{
    private const string USER_PUBLIC = "public";

    private readonly ILogger<MigrateMembersCommandHandler> _logger;
    private readonly IDbContextFactory<KX12Context> _kx12ContextFactory;
    private readonly IEntityMapper<MemberInfoMapperSource, MemberInfo> _memberInfoMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private static int[] MigratedAdminUserPrivilegeLevels => new[] { (int)UserPrivilegeLevelEnum.None };

    public MigrateMembersCommandHandler(
        ILogger<MigrateMembersCommandHandler> logger,
        IDbContextFactory<KX12Context> kx12ContextFactory,
        IEntityMapper<MemberInfoMapperSource, MemberInfo> memberInfoMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kx12ContextFactory = kx12ContextFactory;
        _memberInfoMapper = memberInfoMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
    }

    public async Task<CommandResult> Handle(MigrateMembersCommand request, CancellationToken cancellationToken)
    {
        await using var kx12Context = await _kx12ContextFactory.CreateDbContextAsync(cancellationToken);

        // TODO tomas.krch: 2023-04-11 query only users from particular sites !!
        var k12CmsUsers = kx12Context.CmsUsers
                .Include(u => u.CmsUserSettingUserSettingsUserNavigation)
                .Where(u => MigratedAdminUserPrivilegeLevels.Contains(u.UserPrivilegeLevel))
            ;

        foreach (var k12User in k12CmsUsers)
        {
            _protocol.FetchedSource(k12User);
            _logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid} to member", k12User.UserName, k12User.UserGuid);

            var xbkMemberInfo = MemberInfoProvider.ProviderObject.Get(k12User.UserGuid);

            _protocol.FetchedTarget(xbkMemberInfo);

            // no member shall be admin, editor
            Debug.Assert(k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin, "k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.GlobalAdmin");
            Debug.Assert(k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin, "k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Admin");
            Debug.Assert(k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor, "k12User.UserPrivilegeLevel != (int)UserPrivilegeLevelEnum.Editor");

            if (xbkMemberInfo?.MemberName == USER_PUBLIC || k12User.UserName == USER_PUBLIC)
            {
                continue;
            }

            var mapped = _memberInfoMapper.Map(new MemberInfoMapperSource(k12User, k12User.CmsUserSettingUserSettingsUserNavigation), xbkMemberInfo);
            _protocol.MappedTarget(mapped);

            await SaveUserUsingKenticoApi(cancellationToken, mapped, k12User);
        }

        return new GenericCommandResult();
    }

    private async Task<bool> SaveUserUsingKenticoApi(CancellationToken cancellationToken, IModelMappingResult<MemberInfo> mapped, KX12M.CmsUser k12User)
    {
        if (mapped is { Success : true } result)
        {
            var (memberInfo, newInstance) = result;
            ArgumentNullException.ThrowIfNull(memberInfo);

            try
            {
                MemberInfoProvider.ProviderObject.Set(memberInfo);

                _protocol.Success(k12User, memberInfo, mapped);
                _logger.LogEntitySetAction(newInstance, memberInfo);
            }
            /*Violation in unique index or Violation in unique constraint */
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
            {
                _logger.LogEntitySetError(sqlException, newInstance, memberInfo);
                _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, k12User)
                    .WithData(new { k12User.UserName, k12User.UserGuid, k12User.UserId, })
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
            _primaryKeyMappingContext.SetMapping<KX12M.CmsUser>(r => r.UserId, k12User.UserId, memberInfo.MemberID);
            return true;
        }

        return false;
    }

    public void Dispose()
    {

    }
}