namespace Migration.Toolkit.Core.Handlers;

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
    private readonly IEntityMapper<KX13M.CmsUser, CmsUser> _userMapper;
    private readonly IEntityMapper<KX13M.CmsRole, CmsRole> _roleMapper;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IProtocol _protocol;

    private KxpContext _kxpContext;

    public MigrateUsersCommandHandler(
        ILogger<MigrateUsersCommandHandler> logger,
        IDbContextFactory<KxpContext> kxpContextFactory,
        IDbContextFactory<KX13Context> kx13ContextFactory,
        IEntityMapper<KX13M.CmsUser, CmsUser> userMapper,
        IEntityMapper<KX13M.CmsRole, CmsRole> roleMapper,
        ToolkitConfiguration toolkitConfiguration,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol
    )
    {
        _logger = logger;
        _kxpContextFactory = kxpContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _userMapper = userMapper;
        _roleMapper = roleMapper;
        _toolkitConfiguration = toolkitConfiguration;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _protocol = protocol;
        _kxpContext = _kxpContextFactory.CreateDbContext();
    }
    
    public async Task<CommandResult> Handle(MigrateUsersCommand request, CancellationToken cancellationToken)
    {
        var migratedSiteIds = _toolkitConfiguration.RequireExplicitMapping<KX13M.CmsSite>(s => s.SiteId).Keys.ToList();
        
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        await RequireMigratedCmsRoles(kx13Context, cancellationToken, migratedSiteIds);

        var kx13CmsUsers = kx13Context.CmsUsers
                .Include(u => u.CmsUserRoles.Where(x => migratedSiteIds.Contains(x.Role.SiteId!.Value) || x.Role.SiteId == null))
                .ThenInclude(ur => ur.Role)
            ;

        foreach (var kx13User in kx13CmsUsers)
        {
            _protocol.FetchedSource(kx13User);
            _logger.LogTrace("Migrating user {UserName} with UserGuid {UserGuid}", kx13User.UserName, kx13User.UserGuid);

            var kxoUser = await _kxpContext.CmsUsers
                    .Include(u => u.CmsUserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserGuid == kx13User.UserGuid, cancellationToken)
                ;
            
            _protocol.FetchedTarget(kxoUser);

            if (kx13User.UserPrivilegeLevel == 3 && kxoUser != null)
            {
                _protocol.Append(HandbookReferences.CmsUserAdminUserSkip.WithIdentityPrint(kxoUser));
                _logger.LogInformation("User with guid {UserGuid} is administrator, you need to update administrators manually => skipping", kx13User.UserGuid);
                _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, kxoUser.UserId);
                continue;
            }
            
            if (kxoUser?.UserName == USER_PUBLIC || kx13User.UserName == USER_PUBLIC)
            {
                _protocol.Append(HandbookReferences.CmsUserPublicUserSkip.WithIdentityPrint(kxoUser));
                _logger.LogInformation("User with guid {UserGuid} is public user, special case that can't be migrated => skipping", kxoUser?.UserGuid ?? kx13User.UserGuid);
                if (kxoUser != null)
                {
                    _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, kxoUser.UserId);    
                }
                continue;
            }
            
            var mapped = _userMapper.Map(kx13User, kxoUser);
            _protocol.MappedTarget(mapped);

            if (mapped is { Success : true } result)
            {
                var (cmsUser, newInstance) = result;
                ArgumentNullException.ThrowIfNull(cmsUser);

                if (newInstance)
                {
                    _kxpContext.CmsUsers.Add(cmsUser);
                }
                else
                {
                    _kxpContext.CmsUsers.Update(cmsUser);
                }

                try
                {
                    await _kxpContext.SaveChangesAsync(cancellationToken);

                    _protocol.Success(kx13User, cmsUser, mapped);
                    _logger.LogEntitySetAction(newInstance, cmsUser);
                }
                /*Violation in unique index or Violation in unique constraint */ 
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is SqlException { Number: 2601 or 2627 } sqlException)
                {
                    _logger.LogEntitySetError(sqlException, newInstance, cmsUser);
                    _protocol.Append(HandbookReferences.DbConstraintBroken(sqlException, kx13User)
                        .WithData(new
                        {
                            kx13User.UserName,
                            kx13User.UserGuid,
                            kx13User.UserId,
                        })
                        .WithMessage("Failed to migrate user, target database broken.")
                    );
                    
                    await _kxpContext.DisposeAsync();
                    _kxpContext = await _kxpContextFactory.CreateDbContextAsync(cancellationToken);
                }

                _primaryKeyMappingContext.SetMapping<KX13M.CmsUser>(r => r.UserId, kx13User.UserId, cmsUser.UserId);
            }
        }

        return new GenericCommandResult();
    }

    private async Task RequireMigratedCmsRoles(KX13Context kx13Context, CancellationToken cancellationToken, List<int> migratedSiteIds)
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