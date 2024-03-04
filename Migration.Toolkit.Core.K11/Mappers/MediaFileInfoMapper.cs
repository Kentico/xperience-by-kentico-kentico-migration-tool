namespace Migration.Toolkit.Core.K11.Mappers;

using System.Data;
using CMS.Base;
using CMS.MediaLibrary;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Migration.Toolkit.Common;
using Migration.Toolkit.Common.Abstractions;
using Migration.Toolkit.Common.MigrationProtocol;
using Migration.Toolkit.Core.K11.Contexts;
using Migration.Toolkit.Core.K11.Helpers;
using Migration.Toolkit.K11.Models;
using Migration.Toolkit.KXP.Api;

public record MediaFileInfoMapperSource(MediaFile MediaFile, int TargetLibraryId, IUploadedFile? File, string? LibrarySubFolder,
    bool MigrateOnlyMediaFileInfo);

public class MediaFileInfoMapper(ILogger<MediaFileInfoMapper> logger,
        PrimaryKeyMappingContext primaryKeyMappingContext,
        KxpClassFacade classFacade,
        IProtocol protocol,
        ToolkitConfiguration toolkitConfiguration,
        KeyMappingContext keyMappingContext)
    : EntityMapperBase<MediaFileInfoMapperSource, MediaFileInfo>(logger, primaryKeyMappingContext, protocol)
{
    private readonly IProtocol _protocol = protocol;


    protected override MediaFileInfo? CreateNewInstance(MediaFileInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) {
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
        target.FileGUID = mediaFile.FileGuid;
        target.FileCreatedWhen = mediaFile.FileCreatedWhen;
        target.FileModifiedWhen = mediaFile.FileModifiedWhen;
        KenticoHelper.CopyCustomData(target.FileCustomData, mediaFile.FileCustomData);

        MigrateCustomizedFields(target, mediaFile);

        target.FileLibraryID = targetLibraryId;

        var targetCreatedMemberId = keyMappingContext.MapSourceKey<CmsUser, KXP.Models.CmsMember, int?>(
            s => s.UserId,
            s => s.UserGuid,
            mediaFile.FileCreatedByUserId,
            t => t.MemberId,
            t => t.MemberGuid
        );
        if (targetCreatedMemberId.Success)
        {
            // user was migrated to MEMBER => setting user would break foreign key
            target.SetValue(nameof(target.FileCreatedByUserID), CMSActionContext.CurrentUser.UserID);
        }
        else if (mappingHelper.TranslateIdAllowNulls<CmsUser>(c => c.UserId, mediaFile.FileCreatedByUserId, out var createdByUserId))
        {
            target.SetValue(nameof(target.FileCreatedByUserID), createdByUserId);
        }

        var targetModifiedMemberId = keyMappingContext.MapSourceKey<CmsUser, KXP.Models.CmsMember, int?>(
            s => s.UserId,
            s => s.UserGuid,
            mediaFile.FileModifiedByUserId,
            t => t.MemberId,
            t => t.MemberGuid
        );
        if (targetModifiedMemberId.Success)
        {
            // user was migrated to MEMBER => setting user would break foreign key
            target.SetValue(nameof(target.FileModifiedByUserID), CMSActionContext.CurrentUser.UserID);
        }
        else if (mappingHelper.TranslateIdAllowNulls<CmsUser>(c => c.UserId, mediaFile.FileModifiedByUserId, out var modifiedByUserId))
        {
            target.SetValue(nameof(target.FileModifiedByUserID), modifiedByUserId);
        }

        if (string.IsNullOrWhiteSpace(target.FilePath))
        {
            target.FilePath = mediaFile.FilePath;
        }

        if (file == null && !migrateOnlyMediaFileInfo)
        {
            addFailure(HandbookReferences.MediaFileIsMissingOnSourceFilesystem
                .WithId(nameof(mediaFile.FileId), mediaFile.FileId)
                .WithData(new
                {
                    mediaFile.FilePath,
                    mediaFile.FileGuid,
                    mediaFile.FileLibraryId,
                    mediaFile.FileSiteId
                })
                .AsFailure<MediaFileInfo>()
            );
        }

        return target;
    }

    private void MigrateCustomizedFields(MediaFileInfo target, MediaFile mediaFile)
    {
        var customizedFields = classFacade.GetCustomizedFieldInfos(MediaFileInfo.TYPEINFO.ObjectClassName).ToList();
        if (customizedFields.Count <= 0) return;

        try
        {
            var query =
                $"SELECT {string.Join(", ", customizedFields.Select(x => x.FieldName))} FROM {MediaFileInfo.TYPEINFO.ClassStructureInfo.TableName} WHERE {MediaFileInfo.TYPEINFO.ClassStructureInfo.IDColumn} = @id";

            using var conn = new SqlConnection(toolkitConfiguration.KxConnectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 3;
            cmd.Parameters.AddWithValue("id", mediaFile.FileId);

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