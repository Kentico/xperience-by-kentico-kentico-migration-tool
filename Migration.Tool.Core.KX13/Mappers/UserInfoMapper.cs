using System.Data;

using CMS.Membership;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Core.KX13.Contexts;
using Migration.Tool.KXP.Api;

namespace Migration.Tool.Core.KX13.Mappers;

public class UserInfoMapper(
    ILogger<UserInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    IProtocol protocol,
    KxpClassFacade kxpClassFacade,
    ToolConfiguration toolConfiguration)
    : EntityMapperBase<KX13M.CmsUser, UserInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override UserInfo CreateNewInstance(KX13M.CmsUser source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override UserInfo MapInternal(KX13M.CmsUser source, UserInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (!newInstance && source.UserGuid != target.UserGUID)
        {
            // assertion failed
            logger.LogTrace("Assertion failed, entity key mismatch");
            throw new InvalidOperationException("Assertion failed, entity key mismatch.");
        }

        target.UserName = source.UserName;
        target.FirstName = source.FirstName;
        target.LastName = source.LastName;
        target.Email = source.Email;
        // target.UserPassword = source.UserPassword;
        target.SetValue("UserPassword", source.UserPassword);
        target.UserEnabled = source.UserEnabled;
        target.SetValue("UserCreated", source.UserCreated);
        // target.UserCreated = source.UserCreated;
        target.SetValue("LastLogon", source.LastLogon);
        // target.LastLogon = source.LastLogon;
        target.UserGUID = source.UserGuid;
        target.UserLastModified = source.UserLastModified;
        target.UserSecurityStamp = source.UserSecurityStamp;

        target.UserAdministrationAccess = source.UserPrivilegeLevel == 3;
        target.UserIsPendingRegistration = false;
        target.UserRegistrationLinkExpiration = DateTime.Now.AddDays(365);

        target.UserPreferredLanguage = string.IsNullOrEmpty(source.PreferredUicultureCode) ? null : source.PreferredUicultureCode;

        var customizedFields = kxpClassFacade.GetCustomizedFieldInfos(UserInfo.TYPEINFO.ObjectClassName).ToList();
        if (customizedFields.Count > 0)
        {
            try
            {
                string query =
                    $"SELECT {string.Join(", ", customizedFields.Select(x => x.FieldName))} FROM {UserInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {UserInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

                using var conn = new SqlConnection(toolConfiguration.KxConnectionString);
                using var cmd = conn.CreateCommand();

                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 3;
                cmd.Parameters.AddWithValue("id", source.UserId);

                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    foreach (var customizedFieldInfo in customizedFields)
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


        return target;
    }
}
