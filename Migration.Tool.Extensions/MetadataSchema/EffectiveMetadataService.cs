using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Tool.Common;

namespace Migration.Tool.Extensions.MetadataSchema;

/// <summary>
/// Reads the pre-populated <c>Migration_EffectiveMetadata</c> auxiliary table
/// from the source (KX13) database and caches its rows for use during migration.
/// </summary>
public class EffectiveMetadataService(
    ILogger<EffectiveMetadataService> logger,
    ToolConfiguration toolConfiguration)
{
    private List<EffectiveMetadataRecord>? cache;

    private const string SqlReadTable = $"""
        SELECT
            DocumentID,
            NodeID,
            NodeSiteID,
            DocumentGUID,
            ClassName,
            EffectiveTitle,
            TitleInherited,
            EffectiveKeywords,
            KeywordsInherited,
            EffectiveDescription,
            DescriptionInherited
        FROM dbo.{SeoMetadataConstants.AuxTableName}
        ORDER BY DocumentID;
        """;

    /// <summary>
    /// Returns all rows from the <c>Migration_EffectiveMetadata</c> table.
    /// The result is cached after the first call within the same process run.
    /// </summary>
    public IReadOnlyList<EffectiveMetadataRecord> GetAll()
    {
        if (cache is not null)
        {
            return cache;
        }

        logger.LogInformation(
            "Loading effective metadata from auxiliary table '{TableName}' ...",
            SeoMetadataConstants.AuxTableName);

        using var conn = new SqlConnection(toolConfiguration.KxConnectionString);
        conn.Open();

        using var cmd = new SqlCommand(SqlReadTable, conn);
        using var reader = cmd.ExecuteReader();

        var rows = new List<EffectiveMetadataRecord>();
        while (reader.Read())
        {
            rows.Add(new EffectiveMetadataRecord(
                DocumentID: reader.GetInt32("DocumentID"),
                NodeID: reader.GetInt32("NodeID"),
                NodeSiteID: reader.GetInt32("NodeSiteID"),
                DocumentGUID: reader.GetGuid("DocumentGUID"),
                ClassName: reader.GetString("ClassName"),
                EffectiveTitle: reader.IsDBNull("EffectiveTitle") ? null : reader.GetString("EffectiveTitle"),
                TitleInherited: reader.GetBoolean("TitleInherited"),
                EffectiveKeywords: reader.IsDBNull("EffectiveKeywords") ? null : reader.GetString("EffectiveKeywords"),
                KeywordsInherited: reader.GetBoolean("KeywordsInherited"),
                EffectiveDescription: reader.IsDBNull("EffectiveDescription") ? null : reader.GetString("EffectiveDescription"),
                DescriptionInherited: reader.GetBoolean("DescriptionInherited")
            ));
        }

        cache = rows;
        logger.LogInformation(
            "Loaded {Count} effective-metadata row(s) from '{TableName}'.",
            rows.Count, SeoMetadataConstants.AuxTableName);

        return cache;
    }

    /// <summary>
    /// Returns all distinct source class names present in the auxiliary table.
    /// These are the class names of page types that have
    /// <c>ClassHasMetadata = 1</c> in the source instance.
    /// </summary>
    public IReadOnlyList<string> GetDistinctClassNames()
        => GetAll().Select(r => r.ClassName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
}
