using System.Data;
using CMS.Modules;
using Microsoft.Data.SqlClient;
using Migration.Tool.Common;

namespace Migration.Tool.KXP.Api.Services.CmsResource;

/// <summary>
/// Replaces ResourceInfoProvider that was used in former versions of Kentico Xperience, but doesn't have a public API anymore.
/// </summary>
public sealed class SqlResourceProvider(ToolConfiguration toolConfiguration)
{
    public ResourceInfo? Get(Guid guid)
    {
        const string sql = @"
SELECT TOP 1
    ResourceID,
    ResourceName,
    ResourceDisplayName,
    ResourceDescription,
    ResourceGUID,
    ResourceIsInDevelopment,
    ResourceLastModified
FROM CMS_Resource
WHERE ResourceGUID = @Guid";

        using var conn = new SqlConnection(toolConfiguration.XbKConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = guid;

        conn.Open();
        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new ResourceInfo
        {
            ResourceID = reader.GetInt32(reader.GetOrdinal("ResourceID")),
            ResourceName = reader.GetString(reader.GetOrdinal("ResourceName")),
            ResourceDisplayName = reader.GetString(reader.GetOrdinal("ResourceDisplayName")),
            ResourceDescription = reader.IsDBNull(reader.GetOrdinal("ResourceDescription"))
                ? null
                : reader.GetString(reader.GetOrdinal("ResourceDescription")),
            ResourceGUID = reader.GetGuid(reader.GetOrdinal("ResourceGUID")),
            ResourceIsInDevelopment = !reader.IsDBNull(reader.GetOrdinal("ResourceIsInDevelopment")) && reader.GetBoolean(reader.GetOrdinal("ResourceIsInDevelopment")),
            ResourceLastModified = reader.GetDateTime(reader.GetOrdinal("ResourceLastModified"))
        };
    }

    public void Set(ResourceInfo info)
    {
        const string sql = @"
DECLARE @ExistingID INT;

SELECT @ExistingID = ResourceID
FROM CMS_Resource
WHERE ResourceGUID = @ResourceGUID;

IF @ExistingID IS NOT NULL
BEGIN
    UPDATE CMS_Resource
    SET
        ResourceName = @ResourceName,
        ResourceDisplayName = @ResourceDisplayName,
        ResourceDescription = @ResourceDescription,
        ResourceIsInDevelopment = @ResourceIsInDevelopment,
        ResourceLastModified = @Now
    WHERE ResourceID = @ExistingID;

    SELECT @ExistingID AS ResourceID;
END
ELSE
BEGIN
    INSERT INTO CMS_Resource
    (
        ResourceName,
        ResourceDisplayName,
        ResourceDescription,
        ResourceGUID,
        ResourceIsInDevelopment,
        ResourceLastModified
    )
    VALUES
    (
        @ResourceName,
        @ResourceDisplayName,
        @ResourceDescription,
        @ResourceGUID,
        @ResourceIsInDevelopment,
        @Now
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS ResourceID;
END
";

        using var conn = new SqlConnection(toolConfiguration.XbKConnectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ResourceName", info.ResourceName);
        cmd.Parameters.AddWithValue("@ResourceDisplayName", info.ResourceDisplayName);
        cmd.Parameters.AddWithValue("@ResourceDescription",
            (object?)info.ResourceDescription ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@ResourceGUID", info.ResourceGUID);
        cmd.Parameters.AddWithValue("@ResourceIsInDevelopment", info.ResourceIsInDevelopment);
        cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);

        conn.Open();

        var result = cmd.ExecuteScalar();
        info.ResourceID = Convert.ToInt32(result);
    }

}
