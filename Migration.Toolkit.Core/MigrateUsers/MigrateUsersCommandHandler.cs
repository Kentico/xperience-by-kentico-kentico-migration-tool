using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Abstractions;
using Migration.Toolkit.Core.Contexts;
using Migration.Toolkit.Core.MigrationProtocol;
using Migration.Toolkit.KX13.Context;
using Migration.Toolkit.KXO.Context;

namespace Migration.Toolkit.Core.MigrateUsers;

public class MigrateUsersCommandHandler: IRequestHandler<MigrateUsersCommand, GenericCommandResult>, IDisposable
{
    private readonly ILogger<MigrateUsersCommandHandler> _logger;
    private readonly IDbContextFactory<KxoContext> _kxoContextFactory;
    private readonly IDbContextFactory<KX13Context> _kx13ContextFactory;
    private readonly IEntityMapper<KX13.Models.CmsUser, KXO.Models.CmsUser> _userMapper;
    private readonly PrimaryKeyMappingContext _primaryKeyMappingContext;
    private readonly IMigrationProtocol _migrationProtocol;

    private KxoContext _kxoContext;

    public MigrateUsersCommandHandler(
        ILogger<MigrateUsersCommandHandler> logger,
        IDbContextFactory<KXO.Context.KxoContext> kxoContextFactory,
        IDbContextFactory<KX13.Context.KX13Context> kx13ContextFactory,
        IEntityMapper<KX13.Models.CmsUser, KXO.Models.CmsUser> userMapper,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IMigrationProtocol migrationProtocol
    )
    {
        _logger = logger;
        _kxoContextFactory = kxoContextFactory;
        _kx13ContextFactory = kx13ContextFactory;
        _userMapper = userMapper;
        _primaryKeyMappingContext = primaryKeyMappingContext;
        _migrationProtocol = migrationProtocol;
        _kxoContext = _kxoContextFactory.CreateDbContext();
    }
    
    public async Task<GenericCommandResult> Handle(MigrateUsersCommand request, CancellationToken cancellationToken)
    {
        using var protocolScope = _migrationProtocol.CreateScope<MigrateUsersCommandHandler>();  
        
        await using var kx13Context = await _kx13ContextFactory.CreateDbContextAsync(cancellationToken);

        foreach (var kx13User in kx13Context.CmsUsers)
        {
            _migrationProtocol.FetchedSource(kx13User);
            _logger.LogTrace("Migrating user {userName} with UserGuid {userGuid}", kx13User.UserName, kx13User.UserGuid);
            
            var kxoUser = await _kxoContext.CmsUsers.FirstOrDefaultAsync(u => u.UserGuid == kx13User.UserGuid, cancellationToken);
            _migrationProtocol.FetchedTarget(kxoUser);

            if (kxoUser?.UserAdministrationAccess ?? false || kx13User.UserPrivilegeLevel == 3)
            {
                _logger.LogInformation("User with guid {guid} is administrator, you need to set administrators manually => skipping.", kxoUser?.UserGuid ?? kx13User.UserGuid);
                continue;
            }
            
            if (kxoUser?.UserName == "public" || kx13User.UserName == "public")
            {
                _logger.LogInformation("User with guid {guid} is public user, special case that can't be migrated => skipping.", kxoUser?.UserGuid ?? kx13User.UserGuid);
                continue;
            }
            
            var mapped = _userMapper.Map(kx13User, kxoUser);
            
            mapped.LogResult(_logger);
            _migrationProtocol.MappedTarget(mapped);

            switch (mapped)
            {
                case ModelMappingSuccess<KXO.Models.CmsUser>(var cmsUser, var newInstance):
                    ArgumentNullException.ThrowIfNull(cmsUser, nameof(cmsUser));

                    if (newInstance)
                    {
                        _kxoContext.CmsUsers.Add(cmsUser);
                    }
                    else
                    {
                        _kxoContext.CmsUsers.Update(cmsUser);
                    }

                    try
                    {
                        await _kxoContext.SaveChangesAsync(cancellationToken);

                        _migrationProtocol.Success<KX13.Models.CmsUser, KXO.Models.CmsUser>(kx13User, cmsUser, mapped);
                        _logger.LogInformation(newInstance
                            ? $"CmsUser: {cmsUser.UserName} with UserGuid '{cmsUser.UserGuid}' was inserted."
                            : $"CmsUser: {cmsUser.UserName} with UserGuid '{cmsUser.UserGuid}' was updated.");
                    }
                    catch (DbUpdateException dbUpdateException) when (
                        dbUpdateException.InnerException is SqlException sqlException &&
                        sqlException.Message.Contains("Cannot insert duplicate key row in object") &&
                        sqlException.Message.Contains("IX_CMS_User_Email") &&
                        sqlException.Message.Contains("CMS_User")
                    )
                    {
                        await _kxoContext.DisposeAsync();
                        // TODO tk: 2022-05-18 protocol - request manual migration
                        _logger.LogError("Failed to migrate user, possibly due to duplicated email - user guid: {userGuid}. Use needs manual migration. Email: {email}", kx13User.UserGuid, kx13User.Email);
                        _kxoContext = await _kxoContextFactory.CreateDbContextAsync(cancellationToken);

                        _migrationProtocol.NeedsManualAction(
                            HandbookReferences.CmsUserEmailConstraintBroken,
                            $"Failed to migrate user, possibly due to duplicated email - user guid: {kx13User.UserGuid}. Use needs manual migration. Email: {kx13User.Email}",
                            kx13User, 
                            cmsUser, 
                            mapped
                        );
                        continue;
                    }

                    _primaryKeyMappingContext.SetMapping<KX13.Models.CmsUser>(r => r.UserId, kx13User.UserId, cmsUser.UserId);

                    break;
                default:
                    break;
            }
        }

        return new GenericCommandResult();
    }

    public void Dispose()
    {
        _kxoContext.Dispose();
    }
}