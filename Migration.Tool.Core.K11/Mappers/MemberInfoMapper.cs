using System.Data;

using CMS.FormEngine;
using CMS.Membership;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.Enumerations;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.K11.Contexts;
using Migration.Tool.K11;
using Migration.Tool.K11.Models;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.K11.Mappers;

public record MemberInfoMapperSource(CmsUser User, CmsUserSetting UserSetting);

public class MemberInfoMapper(
    ILogger<MemberInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade,
    ToolConfiguration toolConfiguration,
    IDbContextFactory<K11Context> k11DbContextFactory)
    : EntityMapperBase<MemberInfoMapperSource, MemberInfo>(logger, primaryKeyMappingContext, protocol)
{
    public static IReadOnlyList<string> MigratedUserFields =
    [
        nameof(CmsUser.UserGuid),
        nameof(CmsUser.UserName),
        nameof(CmsUser.Email),
        // nameof(KX12M.CmsUser.UserPassword),
        nameof(CmsUser.UserEnabled),
        nameof(CmsUser.UserCreated),
        nameof(CmsUser.UserSecurityStamp)
    ];

    protected override MemberInfo CreateNewInstance(MemberInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override MemberInfo MapInternal(MemberInfoMapperSource source, MemberInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (user, userSetting) = source;

        if (!newInstance && user.UserGuid != target.MemberGuid)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        // target.UserName = source.UserName;
        target.MemberName = user.UserName;

        // target.Email = source.Email;
        target.MemberEmail = user.Email;

        // target.SetValue("UserPassword", source.UserPassword);
        target.MemberPassword = null; // source.UserPassword; // not migrated

        // target.UserEnabled = source.UserEnabled;
        target.MemberEnabled = user.UserEnabled;

        target.SetValue("UserCreated", user.UserCreated);
        target.MemberCreated = user.UserCreated.GetValueOrDefault();

        // target.UserGUID = source.UserGuid;
        target.MemberGuid = user.UserGuid;

        target.MemberSecurityStamp = user.UserSecurityStamp;

        // OBSOLETE: target.UserAdministrationAccess = source.UserPrivilegeLevel == 3;
        // OBSOLETE: target.UserIsPendingRegistration = false;
        // OBSOLETE: target.UserPasswordLastChanged = null;
        // OBSOLETE: target.UserRegistrationLinkExpiration = DateTime.Now.AddDays(365);

        var customized = kxpClassFacade.GetCustomizedFieldInfosAll(MemberInfo.TYPEINFO.ObjectClassName);
        foreach (var customizedFieldInfo in customized)
        {
            string fieldName = customizedFieldInfo.FieldName;

            if (ReflectionHelper<CmsUser>.TryGetPropertyValue(user, fieldName, StringComparison.InvariantCultureIgnoreCase, out object? value) ||
                ReflectionHelper<CmsUserSetting>.TryGetPropertyValue(userSetting, fieldName, StringComparison.InvariantCultureIgnoreCase, out value))
            {
                target.SetValue(fieldName, value);
            }
        }

        using var k11Context = k11DbContextFactory.CreateDbContext();
        var uDci = k11Context.CmsClasses.Select(x => new { x.ClassFormDefinition, x.ClassName, x.ClassTableName }).FirstOrDefault(x => x.ClassName == Kx13SystemClass.cms_user);
        if (uDci != null)
        {
            var userCustomizedFields = kxpClassFacade.GetCustomizedFieldInfos(new FormInfo(uDci?.ClassFormDefinition)).ToList();
            if (userCustomizedFields.Count > 0)
            {
                try
                {
                    string query =
                        $"SELECT {string.Join(", ", userCustomizedFields.Select(x => x.FieldName))} FROM {UserInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {UserInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

                    using var conn = new SqlConnection(toolConfiguration.KxConnectionString);
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
                            logger.LogDebug("Map customized field '{FieldName}'", customizedFieldInfo.FieldName);
                            target.SetValue(customizedFieldInfo.FieldName, reader.GetValue(customizedFieldInfo.FieldName));
                        }
                    }
                    else
                    {
                        // failed!
                        logger.LogError("Failed to load UserInfo custom data from source database");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to load UserInfo custom data from source database");
                }
            }
        }

        var usDci = k11Context.CmsClasses.Select(x => new { x.ClassFormDefinition, x.ClassName, x.ClassTableName }).FirstOrDefault(x => x.ClassName == K12SystemClass.cms_usersettings);
        if (usDci != null)
        {
            var userSettingsCustomizedFields = kxpClassFacade.GetCustomizedFieldInfos(new FormInfo(usDci?.ClassFormDefinition)).ToList();
            if (userSettingsCustomizedFields.Count > 0)
            {
                try
                {
                    string query =
                        $"SELECT {string.Join(", ", userSettingsCustomizedFields.Select(x => x.FieldName))} FROM {usDci.ClassTableName} WHERE UserSettingsID = @id";

                    using var conn = new SqlConnection(toolConfiguration.KxConnectionString);
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
                            logger.LogDebug("Map customized field '{FieldName}'", customizedFieldInfo.FieldName);
                            target.SetValue(customizedFieldInfo.FieldName, reader.GetValue(customizedFieldInfo.FieldName));
                        }
                    }
                    else
                    {
                        // failed!
                        logger.LogError("Failed to load UserSettingsInfo custom data from source database");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to load UserSettingsInfo custom data from source database");
                }
            }
        }


        return target;
    }
}
