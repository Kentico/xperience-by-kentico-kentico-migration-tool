namespace Migration.Toolkit.Core.KX12.Mappers;

using System.Data;
using CMS.FormEngine;
using CMS.Membership;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.Enumerations;
using Migration.Toolkit.Common.Helpers;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.KX12.Contexts;
using Migration.Toolkit.KX12.Context;
using Migration.Toolkit.KXP.Api;

public record MemberInfoMapperSource(KX12M.CmsUser User, KX12M.CmsUserSetting UserSetting);

public class MemberInfoMapper : EntityMapperBase<MemberInfoMapperSource, MemberInfo>
{
    private readonly ILogger<MemberInfoMapper> _logger;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly IDbContextFactory<KX12Context> _k12DbContextFactory;

    public MemberInfoMapper(
        ILogger<MemberInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        KxpClassFacade kxpClassFacade,
        ToolkitConfiguration toolkitConfiguration,
        IDbContextFactory<KX12Context> k12DbContextFactory
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _kxpClassFacade = kxpClassFacade;
        _toolkitConfiguration = toolkitConfiguration;
        _k12DbContextFactory = k12DbContextFactory;
    }

    protected override MemberInfo CreateNewInstance(MemberInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    public static IReadOnlyList<string> MigratedUserFields = new List<string>
    {
        nameof(KX12M.CmsUser.UserGuid),
        nameof(KX12M.CmsUser.UserName),
        nameof(KX12M.CmsUser.Email),
        // nameof(KX12M.CmsUser.UserPassword),
        nameof(KX12M.CmsUser.UserEnabled),
        nameof(KX12M.CmsUser.UserCreated),
        nameof(KX12M.CmsUser.UserSecurityStamp),
    };

    protected override MemberInfo MapInternal(MemberInfoMapperSource source, MemberInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (user, userSetting) = source;

        if (!newInstance && user.UserGuid != target.MemberGuid)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        // target.UserName = source.UserName;
        target.MemberName = user.UserName;

        // target.FirstName = source.FirstName; // TODO tomas.krch: 2023-04-11 configurable autocreate
        // target.LastName = source.LastName; // TODO tomas.krch: 2023-04-11 configurable autocreate

        // target.Email = source.Email;
        target.MemberEmail = user.Email;

        // target.SetValue("UserPassword", source.UserPassword);
        target.MemberPassword = null; // source.UserPassword; // not migrated

        // target.UserEnabled = source.UserEnabled;
        target.MemberEnabled = user.UserEnabled;

        target.SetValue("UserCreated", user.UserCreated);
        target.MemberCreated = user.UserCreated.GetValueOrDefault();

        // target.SetValue("LastLogon", source.LastLogon); // TODO tomas.krch: 2023-04-11 configurable autocreate

        // target.UserGUID = source.UserGuid;
        target.MemberGuid = user.UserGuid;

        // target.UserLastModified = source.UserLastModified; // TODO tomas.krch: 2023-04-11 configurable autocreate
        target.MemberSecurityStamp = user.UserSecurityStamp; // TODO tomas.krch: 2023-04-11 still relevant?

        // OBSOLETE: target.UserAdministrationAccess = source.UserPrivilegeLevel == 3;
        // OBSOLETE: target.UserIsPendingRegistration = false;
        // OBSOLETE: target.UserPasswordLastChanged = null;
        // OBSOLETE: target.UserRegistrationLinkExpiration = DateTime.Now.AddDays(365);

        // TODO tomas.krch: 2023-04-11 migrate customized fields
        var customized = _kxpClassFacade.GetCustomizedFieldInfosAll(MemberInfo.TYPEINFO.ObjectClassName);
        foreach (var customizedFieldInfo in customized)
        {
            var fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<KX12M.CmsUser>.TryGetPropertyValue(user, fieldName, StringComparison.InvariantCultureIgnoreCase, out var value) ||
                ReflectionHelper<KX12M.CmsUserSetting>.TryGetPropertyValue(userSetting, fieldName, StringComparison.InvariantCultureIgnoreCase, out value))
            {
                target.SetValue(fieldName, value);
            }
        }

        var userCustomizedFields = _kxpClassFacade.GetCustomizedFieldInfos(K12SystemClass.cms_user).ToList();
        if (userCustomizedFields.Count > 0)
        {
            try
            {
                var query =
                    $"SELECT {string.Join(", ", userCustomizedFields.Select(x => x.FieldName))} FROM {UserInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {UserInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

                using var conn = new SqlConnection(_toolkitConfiguration.KxConnectionString);
                using var cmd = conn.CreateCommand();

                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 3;
                cmd.Parameters.AddWithValue("id", source.User.UserId);

                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    foreach (var customizedFieldInfo in userCustomizedFields)
                    {
                        _logger.LogDebug("Map customized field '{FieldName}'", customizedFieldInfo.FieldName);
                        target.SetValue(customizedFieldInfo.FieldName, reader.GetValue(customizedFieldInfo.FieldName));
                    }
                }
                else
                {
                    // failed!
                    _logger.LogError("Failed to load UserInfo custom data from source database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load UserInfo custom data from source database");
            }
        }


        using var kx12Context = _k12DbContextFactory.CreateDbContext();
        var usDci = kx12Context.CmsClasses.Select(x => new { x.ClassFormDefinition, x.ClassName, x.ClassTableName }).FirstOrDefault(x => x.ClassName == K12SystemClass.cms_usersettings);

        if (usDci != null)
        {
            var userSettingsCustomizedFields = _kxpClassFacade.GetCustomizedFieldInfos(new FormInfo(usDci?.ClassFormDefinition)).ToList();
            if (userSettingsCustomizedFields.Count > 0)
            {
                try
                {
                    var query =
                        $"SELECT {string.Join(", ", userSettingsCustomizedFields.Select(x => x.FieldName))} FROM {usDci.ClassTableName} WHERE UserSettingsID = @id";

                    using var conn = new SqlConnection(_toolkitConfiguration.KxConnectionString);
                    using var cmd = conn.CreateCommand();

                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 3;
                    cmd.Parameters.AddWithValue("id", source.UserSetting.UserSettingsId);

                    conn.Open();

                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        foreach (var customizedFieldInfo in userSettingsCustomizedFields)
                        {
                            _logger.LogDebug("Map customized field '{FieldName}'", customizedFieldInfo.FieldName);
                            target.SetValue(customizedFieldInfo.FieldName, reader.GetValue(customizedFieldInfo.FieldName));
                        }
                    }
                    else
                    {
                        // failed!
                        _logger.LogError("Failed to load UserSettingsInfo custom data from source database");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load UserSettingsInfo custom data from source database");
                }
            }
        }


        return target;
    }
}