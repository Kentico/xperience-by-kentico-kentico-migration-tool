using Microsoft.Extensions.Logging;
using Migration.Toolkit.Core.Contexts;

namespace Migration.Toolkit.Core.Mappers;

using System.Data;
using System.Text;
using CMS.Membership;
using Microsoft.Data.SqlClient;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;

public class UserInfoMapper : EntityMapperBase<KX13M.CmsUser, UserInfo>
{
    private readonly ILogger<UserInfoMapper> _logger;
    private readonly KxpClassFacade _kxpClassFacade;
    private readonly ToolkitConfiguration _toolkitConfiguration;

    public UserInfoMapper(
        ILogger<UserInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        IProtocol protocol,
        KxpClassFacade kxpClassFacade,
        ToolkitConfiguration toolkitConfiguration
    ) : base(logger, primaryKeyMappingContext, protocol)
    {
        _logger = logger;
        _kxpClassFacade = kxpClassFacade;
        _toolkitConfiguration = toolkitConfiguration;
    }

    protected override UserInfo CreateNewInstance(KX13.Models.CmsUser source, MappingHelper mappingHelper, AddFailure addFailure) => new();

    protected override UserInfo MapInternal(KX13.Models.CmsUser source, UserInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (!newInstance && source.UserGuid != target.UserGUID)
        {
            // assertion failed
            _logger.LogTrace("Assertion failed, entity key mismatch");
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

        // TODO tk: 2022-05-18 deduced - check
        target.UserAdministrationAccess = source.UserPrivilegeLevel == 3;
        // TODO tk: 2022-05-18 deduce info
        target.UserIsPendingRegistration = false;
        // TODO tk: 2022-05-18 deduce info
        // target.UserPasswordLastChanged = null;
        // TODO tk: 2022-05-18 deduce info
        target.UserRegistrationLinkExpiration = DateTime.Now.AddDays(365);

        var customizedFields = _kxpClassFacade.GetCustomizedFieldInfos(UserInfo.TYPEINFO.ObjectClassName).ToList();
        if (customizedFields.Count > 0)
        {
            try
            {
                var query =
                    $"SELECT {string.Join(", ", customizedFields.Select(x => x.FieldName))} FROM {UserInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {UserInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

                using var conn = new SqlConnection(_toolkitConfiguration.KxConnectionString);
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


        return target;
    }
}