-- Remedies GUIDs of multiple entities migrated to XbyK by Migration Tool(prior and including v3.12.0)
-- This fixes content synchronization between two XbyK instances, but should be applied even if that features
-- is not in use.
-- 
-- Usage: Run this script on database of all XbyK instances that contain migrated data.
--
-- Note that the script changes GUID of all items in several internal XbyK tables, which may be a breaking change 
-- for custom external tools. For official XbyK functionality this is a non-breaking change.

UPDATE CMS_ContentItemLanguageMetadata
SET ContentItemLanguageMetadataGUID = CAST(
    HASHBYTES('MD5', CAST(ContentItemGUID AS VARBINARY(16)) + CONVERT(VARBINARY(MAX), 'CMS_ContentItemLanguageMetadata'))
AS UNIQUEIDENTIFIER)
FROM CMS_ContentItemLanguageMetadata JOIN CMS_ContentItem ON ContentItemLanguageMetadataContentItemID = ContentItemID;


UPDATE CMS_WebPageItem
SET WebPageItemGUID = CAST(
    HASHBYTES('MD5', CAST(ContentItemGUID AS VARBINARY(16)) + CONVERT(VARBINARY(MAX), 'CMS_WebPageItem'))
AS UNIQUEIDENTIFIER)
FROM CMS_WebPageItem JOIN CMS_ContentItem ON WebPageItemContentItemID = ContentItemID;


UPDATE CMS_WebsiteChannel
SET WebsiteChannelGUID = CAST(
    HASHBYTES('MD5', CAST(ChannelGUID AS VARBINARY(16)) + CONVERT(VARBINARY(MAX), 'CMS_WebsiteChannel'))
AS UNIQUEIDENTIFIER)
FROM CMS_WebsiteChannel JOIN CMS_Channel ON WebsiteChannelChannelID = ChannelID;



SET NOCOUNT ON;

DECLARE 
    @schema sysname,
    @name   sysname,
    @fullQuoted nvarchar(512),
    @sql    nvarchar(max);

DECLARE c CURSOR FAST_FORWARD FOR
SELECT 
    -- If ClassTableName contains schema (e.g. dbo.MyTable), respect it; else default to dbo
    ISNULL(NULLIF(PARSENAME(ClassTableName, 2), N''), N'dbo')  AS schema_name,
    PARSENAME(ClassTableName, 1)                               AS table_name
FROM CMS_Class
WHERE ClassType = 'content';

OPEN c;
FETCH NEXT FROM c INTO @schema, @name;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @fullQuoted = QUOTENAME(@schema) + N'.' + QUOTENAME(@name);

    SET @sql = N'
UPDATE T
SET ContentItemDataGUID =
    CAST(
        HASHBYTES(
            ''MD5'',
            CAST(C.ContentItemCommonDataGUID AS varbinary(16))
            + CONVERT(varbinary(max), @TableNameForHash)
        ) AS uniqueidentifier
    )
FROM ' + @fullQuoted + N' AS T
JOIN CMS_ContentItemCommonData AS C
  ON T.ContentItemDataCommonDataID = C.ContentItemCommonDataID
WHERE C.ContentItemCommonDataGUID IS NOT NULL;';

    BEGIN TRY
        EXEC sp_executesql
            @sql,
            N'@TableNameForHash sysname',
            @TableNameForHash = @name;
        PRINT N'Updated: ' + @fullQuoted;
    END TRY
    BEGIN CATCH
        PRINT N'FAILED: ' + @fullQuoted + N' -> ' + ERROR_MESSAGE();
    END CATCH;

    FETCH NEXT FROM c INTO @schema, @name;
END

CLOSE c;
DEALLOCATE c;
