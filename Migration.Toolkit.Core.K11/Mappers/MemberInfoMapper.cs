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
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;

namespace Migration.Toolkit.Core.K11.Mappers;

public record MemberInfoMapperSource(CmsUser User, CmsUserSetting UserSetting);

public class MemberInfoMapper(
    ILogger<MemberInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade,
    ToolkitConfiguration toolkitConfiguration,
    IDbContextFactory<K11Context> k11DbContextFactory)
    : EntityMapperBase<MemberInfoMapperSource, MemberInfo>(logger, primaryKeyMappingContext, protocol)
{
    public static IReadOnlyList<string> MigratedUserFields = new List<string>
    {
        nameof(CmsUser.UserGuid),
        nameof(CmsUser.UserName),
        nameof(CmsUser.Email),
        // nameof(KX12M.CmsUser.UserPassword),
        nameof(CmsUser.UserEnabled),
        nameof(CmsUser.UserCreated),
        nameof(CmsUser.UserSecurityStamp)
    };

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

                    using var conn = new SqlConnection(toolkitConfiguration.KxConnectionString);
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

                    using var conn = new SqlConnection(toolkitConfiguration.KxConnectionString);
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
