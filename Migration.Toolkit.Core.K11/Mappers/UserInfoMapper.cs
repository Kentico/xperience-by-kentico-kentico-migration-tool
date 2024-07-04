namespace Migration.Toolkit.Core.K11.Mappers;

using System.Data;
using CMS.Membership;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;

public class UserInfoMapper(ILogger<UserInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        KxpClassFacade kxpClassFacade,
        ToolkitConfiguration toolkitConfiguration)
    : EntityMapperBase<CmsUser, UserInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override UserInfo CreateNewInstance(CmsUser source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override UserInfo MapInternal(CmsUser source, UserInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
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

        var customizedFields = kxpClassFacade.GetCustomizedFieldInfos(UserInfo.TYPEINFO.ObjectClassName).ToList();
        if (customizedFields.Count > 0)
        {
            try
            {
                var query =
                    $"SELECT {string.Join(", ", customizedFields.Select(x => x.FieldName))} FROM {UserInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {UserInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

                using var conn = new SqlConnection(toolkitConfiguration.KxConnectionString);
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