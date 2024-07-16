
using System.Data;

using CMS.Base;
using CMS.MediaLibrary;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.KXP.Api;
using Migration.Toolkit.Source.Contexts;
using Migration.Toolkit.Source.Helpers;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Mappers;
public record MediaFileInfoMapperSource(IMediaFile MediaFile, int TargetLibraryId, IUploadedFile? File, string? LibrarySubFolder,
    bool MigrateOnlyMediaFileInfo);

public class MediaFileInfoMapper(
    ILogger<MediaFileInfoMapper> logger,
    PrimaryKeyMappingContext primaryKeyMappingContext,
    KxpClassFacade classFacade,
    IProtocol protocol,
    ToolkitConfiguration toolkitConfiguration,
    ModelFacade modelFacade
)
    : EntityMapperBase<MediaFileInfoMapperSource, MediaFileInfo>(logger, primaryKeyMappingContext, protocol)
{
    protected override MediaFileInfo? CreateNewInstance(MediaFileInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure)
    {
        if (source.File != null)
        {
            var mf = new MediaFileInfo(source.File, source.TargetLibraryId, source.LibrarySubFolder ?? "", 0, 0, 0);
            mf.SaveFileToDisk(true);
            return mf;
        }

        return new MediaFileInfo();
    }

    protected override MediaFileInfo MapInternal(MediaFileInfoMapperSource args, MediaFileInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
    {
        var (mediaFile, targetLibraryId, file, _, migrateOnlyMediaFileInfo) = args;

        target.FileName = mediaFile.FileName;
        target.FileTitle = mediaFile.FileTitle;
        target.FileDescription = mediaFile.FileDescription;
        target.FileExtension = mediaFile.FileExtension;
        target.FileMimeType = mediaFile.FileMimeType;
        target.FileSize = mediaFile.FileSize;
        target.FileImageWidth = mediaFile.FileImageWidth ?? 0;
        target.FileImageHeight = mediaFile.FileImageHeight ?? 0;
        target.FileGUID = mediaFile.FileGUID;
        target.FileCreatedWhen = mediaFile.FileCreatedWhen;
        target.FileModifiedWhen = mediaFile.FileModifiedWhen;
        KenticoHelper.CopyCustomData(target.FileCustomData, mediaFile.FileCustomData);

        MigrateCustomizedFields(target, mediaFile);

        target.FileLibraryID = targetLibraryId;

        int? createdByUserId = AdminUserHelper.MapTargetAdminUser(
            mediaFile.FileCreatedByUserID,
            CMSActionContext.CurrentUser.UserID,
            () => addFailure(HandbookReferences
                .MissingRequiredDependency<ICmsUser>(nameof(mediaFile.FileCreatedByUserID), mediaFile.FileCreatedByUserID)
                .NeedsManualAction()
                .AsFailure<MediaFileInfo>()
            )
        );
        target.SetValue(nameof(target.FileCreatedByUserID), createdByUserId);

        int? modifiedByUserId = AdminUserHelper.MapTargetAdminUser(
            mediaFile.FileModifiedByUserID,
            CMSActionContext.CurrentUser.UserID,
            () => addFailure(HandbookReferences
                .MissingRequiredDependency<ICmsUser>(nameof(mediaFile.FileModifiedByUserID), mediaFile.FileModifiedByUserID)
                .NeedsManualAction()
                .AsFailure<MediaFileInfo>()
            )
        );
        target.SetValue(nameof(target.FileModifiedByUserID), modifiedByUserId);

        if (string.IsNullOrWhiteSpace(target.FilePath))
        {
            target.FilePath = mediaFile.FilePath;
        }

        if (file == null && !migrateOnlyMediaFileInfo)
        {
            addFailure(HandbookReferences.MediaFileIsMissingOnSourceFilesystem
                .WithId(nameof(mediaFile.FileID), mediaFile.FileID)
                .WithData(new
                {
                    mediaFile.FilePath,
                    mediaFile.FileGUID,
                    mediaFile.FileLibraryID,
                    mediaFile.FileSiteID
                })
                .AsFailure<MediaFileInfo>()
            );
        }

        return target;
    }

    private void MigrateCustomizedFields(MediaFileInfo target, IMediaFile sourceMediaFile)
    {
        var customizedFields = classFacade.GetCustomizedFieldInfos(MediaFileInfo.TYPEINFO.ObjectClassName).ToList();
        if (customizedFields.Count <= 0)
        {
            return;
        }

        try
        {
            string query =
                $"SELECT {string.Join(", ", customizedFields.Select(x => x.FieldName))} FROM {MediaFileInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {MediaFileInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

            using var conn = new SqlConnection(toolkitConfiguration.KxConnectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 3;
            cmd.Parameters.AddWithValue("id", sourceMediaFile.FileID);

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
                logger.LogError("Failed to load MediaFileInfo custom data from source database");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load MediaFileInfo custom data from source database");
        }
    }
}
