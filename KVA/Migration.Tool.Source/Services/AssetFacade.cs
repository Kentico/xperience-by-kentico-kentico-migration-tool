using System.Diagnostics;
using CMS.Base;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Source.Auxiliary;
using Migration.Tool.Source.Helpers;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public interface IAssetFacade
{
    string DefaultContentLanguage { get; }

    /// <summary>
    /// Translates legacy media file to new preferred storage - content item 
    /// </summary>
    /// <param name="mediaFile">Media file to convert</param>
    /// <param name="mediaLibrary">Media library that owns media file</param>
    /// <param name="site">CmsSite that owns media file in source instance</param>
    /// <param name="contentLanguageNames">preferably only default language</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">occurs when media path cannot be determined</exception>
    Task<ContentItemSimplifiedModel> FromMediaFile(IMediaFile mediaFile, IMediaLibrary mediaLibrary, ICmsSite site, string[] contentLanguageNames);

    Task<ContentItemSimplifiedModel> FromAttachment(ICmsAttachment attachment, ICmsSite site, ICmsTree? referencedNode, string[] contentLanguageNames);

    /// <summary>
    /// translates identity of media file to newly created content item (and content item asset)
    /// </summary>
    /// <param name="mediaFile"></param>
    /// <param name="contentLanguageName"></param>
    /// <returns></returns>
    (Guid ownerContentItemGuid, Guid assetGuid) GetRef(IMediaFile mediaFile, string? contentLanguageName = null);

    /// <summary>
    /// translates identity of attachment to newly created content item (and content item asset)
    /// </summary>
    /// <param name="attachment"></param>
    /// <param name="contentLanguageName"></param>
    /// <returns></returns>
    (Guid ownerContentItemGuid, Guid assetGuid) GetRef(ICmsAttachment attachment, string? contentLanguageName);

    Task PreparePrerequisites();

    string GetAssetUri(IMediaFile mediaFile, string? contentLanguageName = null);
    string GetAssetUri(ICmsAttachment attachment, string? contentLanguageName);
}

public class AssetFacade(
        EntityIdentityFacade entityIdentityFacade,
        ToolConfiguration toolConfiguration,
        ModelFacade modelFacade,
        IImporter importer,
        ILogger<AssetFacade> logger,
        IProtocol protocol
        ) : IAssetFacade
{
    public string DefaultContentLanguage
    {
        get
        {
            if (defaultContentLanguage == null)
            {
                var contentLanguageRetriever = Service.Resolve<IContentLanguageRetriever>();
                defaultContentLanguage = contentLanguageRetriever.GetDefaultContentLanguage().GetAwaiter().GetResult();
            }
            return defaultContentLanguage.ContentLanguageName;
        }
    }

    /// <inheritdoc />
    public async Task<ContentItemSimplifiedModel> FromMediaFile(IMediaFile mediaFile, IMediaLibrary mediaLibrary, ICmsSite site, string[] contentLanguageNames)
    {
        Debug.Assert(mediaFile.FileLibraryID == mediaLibrary.LibraryID, "mediaFile.FileLibraryID == mediaLibrary.LibraryID");
        Debug.Assert(mediaLibrary.LibrarySiteID == site.SiteID, "mediaLibrary.LibrarySiteID == site.SiteID");

        string? mediaLibraryAbsolutePath = GetMediaLibraryAbsolutePath(toolConfiguration, site, mediaLibrary, modelFacade);
        if (toolConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false))
        {
            throw new InvalidOperationException($"Configuration 'Settings.MigrateOnlyMediaFileInfo' is set to to 'true', for migration of media files to content items this setting is required to be 'false'");
        }
        if (string.IsNullOrWhiteSpace(mediaLibraryAbsolutePath))
        {
            throw new InvalidOperationException($"Invalid media file path generated for {mediaFile} and {mediaLibrary} on {site}");
        }

        string mediaFilePath = Path.Combine(mediaLibraryAbsolutePath, mediaFile.FilePath);

        int? createdByUserId = AdminUserHelper.MapTargetAdminUser(
                mediaFile.FileCreatedByUserID,
                CMSActionContext.CurrentUser.UserID,
                () => protocol.Append(HandbookReferences
                        .MissingRequiredDependency<ICmsUser>(nameof(mediaFile.FileCreatedByUserID), mediaFile.FileCreatedByUserID)
                        .NeedsManualAction()
                )
        );
        var createdByUser = createdByUserId.HasValue ? modelFacade.SelectById<ICmsUser>(createdByUserId) : null;

        var (_, translatedMediaGuid) = entityIdentityFacade.Translate(mediaFile);

        List<ContentItemLanguageData> languageData = [];
        languageData.AddRange(contentLanguageNames.Select(contentLanguageName => new ContentItemLanguageData
        {
            LanguageName = contentLanguageName,
            DisplayName = $"{mediaFile.FileName}",
            UserGuid = createdByUser?.UserGUID,
            VersionStatus = VersionStatus.Published,
            ContentItemData = new Dictionary<string, object?>
            {
                ["Asset"] = new AssetFileSource
                {
                    ContentItemGuid = translatedMediaGuid,
                    Identifier = GuidHelper.CreateAssetGuid(translatedMediaGuid, contentLanguageName),
                    Name = Path.Combine(mediaFile.FileName, mediaFile.FileExtension),
                    Extension = mediaFile.FileExtension,
                    Size = null,
                    LastModified = null,
                    FilePath = mediaFilePath
                }
            }
        }));

        string mediaFolder = Path.Combine(mediaLibrary.LibraryFolder, Path.GetDirectoryName(mediaFile.FilePath)!);

        var folder = GetAssetFolder(site);

        var contentItem = new ContentItemSimplifiedModel
        {
            CustomProperties = [],
            ContentItemGUID = translatedMediaGuid,
            ContentItemContentFolderGUID = (await EnsureFolderStructure(mediaFolder, folder))?.ContentFolderGUID ?? folder.ContentFolderGUID,
            IsSecured = null,
            ContentTypeName = LegacyMediaFileContentType.ClassName,
            Name = $"{mediaFile.FileName}_{translatedMediaGuid}",
            IsReusable = true,
            LanguageData = languageData,
        };

        // TODO tomas.krch: 2024-09-02 append url to protocol
        // urlProtocol.AppendMediaFileUrlIfNeeded();

        return contentItem;
    }

    public async Task<ContentItemSimplifiedModel> FromAttachment(ICmsAttachment attachment, ICmsSite site, ICmsTree? referencedNode, string[] contentLanguageNames)
    {
        Debug.Assert(attachment.AttachmentSiteID == site.SiteID || attachment.AttachmentSiteID == 0, "attachment.AttachmentSiteID == site.SiteID || attachment.AttachmentSiteID == 0");

        var (_, translatedAttachmentGuid) = entityIdentityFacade.Translate(attachment);

        List<ContentItemLanguageData> languageData = [];
        languageData.AddRange(contentLanguageNames.Select(contentLanguageName => new ContentItemLanguageData
        {
            LanguageName = contentLanguageName,
            DisplayName = $"{attachment.AttachmentName}",
            UserGuid = null,
            VersionStatus = VersionStatus.Published,
            ContentItemData = new Dictionary<string, object?>
            {
                ["Asset"] = new AssetDataSource
                {
                    ContentItemGuid = translatedAttachmentGuid,
                    Identifier = GuidHelper.CreateAssetGuid(translatedAttachmentGuid, contentLanguageName),
                    Name = Path.Combine(attachment.AttachmentName, attachment.AttachmentExtension),
                    Extension = attachment.AttachmentExtension,
                    Size = null,
                    LastModified = attachment.AttachmentLastModified,
                    Data = attachment.AttachmentBinary,
                }
            }
        }));

        string mediaFolder = "__Attachments";
        if (referencedNode != null && string.IsNullOrWhiteSpace(referencedNode.NodeAliasPath))
        {
            mediaFolder += referencedNode.NodeAliasPath.StartsWith("/") ? referencedNode.NodeAliasPath : $"/{referencedNode.NodeAliasPath}";
        }

        var folder = GetAssetFolder(site);

        var contentItem = new ContentItemSimplifiedModel
        {
            ContentItemGUID = translatedAttachmentGuid,
            ContentItemContentFolderGUID = (await EnsureFolderStructure(mediaFolder, folder))?.ContentFolderGUID ?? folder.ContentFolderGUID,
            IsSecured = null,
            ContentTypeName = LegacyAttachmentContentType.ClassName,
            Name = $"{attachment.AttachmentGUID}_{translatedAttachmentGuid}",
            IsReusable = true,
            LanguageData = languageData,
        };

        // TODO tomas.krch: 2024-09-02 append url to protocol
        // urlProtocol.AppendMediaFileUrlIfNeeded();

        return contentItem;
    }

    private readonly Dictionary<string, ContentFolderModel> contentFolderModels = [];
    private ContentLanguageInfo? defaultContentLanguage;

    private async Task<ContentFolderModel?> EnsureFolderStructure(string folderPath, ContentFolderModel rootFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        string rootKey = $"root|{rootFolder.ContentFolderGUID}";
        if (!contentFolderModels.TryGetValue(rootKey, out _))
        {
            switch (await importer.ImportAsync(rootFolder))
            {
                case { Success: true }:
                {
                    contentFolderModels[rootKey] = rootFolder;
                    break;
                }
                case { Success: false, Exception: { } exception }:
                {
                    logger.LogError("Failed to import asset migration folder: {Error} {Prerequisite}", exception.ToString(), rootFolder.PrintMe());
                    break;
                }
                case { Success: false, ModelValidationResults: { } validation }:
                {
                    foreach (var validationResult in validation)
                    {
                        logger.LogError("Failed to import asset migration folder {Members}: {Error} - {Prerequisite}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage, rootFolder.PrintMe());
                    }

                    break;
                }
                default:
                {
                    throw new InvalidOperationException($"Asset migration cannot continue, cannot prepare prerequisite - unknown result");
                }
            }
            contentFolderModels[rootKey] = rootFolder;
        }

        string[] pathSplit = folderPath.Split(Path.DirectorySeparatorChar);
        ContentFolderModel? lastFolder = null;
        for (int i = 0; i < pathSplit.Length; i++)
        {
            string current = pathSplit[i];
            string currentPath = string.Join("/", rootFolder.ContentFolderTreePath!, string.Join("/", pathSplit[..(i + 1)]));
            var folderGuid = GuidHelper.CreateFolderGuid($"{rootFolder.ContentFolderGUID}|{currentPath}");
            string folderKey = $"{currentPath}";
            if (!contentFolderModels.TryGetValue(folderKey, out lastFolder))
            {
                lastFolder = new ContentFolderModel
                {
                    ContentFolderGUID = folderGuid,
                    ContentFolderName = $"{folderGuid}",
                    ContentFolderDisplayName = current,
                    ContentFolderTreePath = currentPath,
                    ContentFolderParentFolderGUID = lastFolder?.ContentFolderGUID ?? rootFolder.ContentFolderGUID
                };
                switch (await importer.ImportAsync(lastFolder))
                {
                    case { Success: true }:
                    {
                        contentFolderModels[folderKey] = lastFolder;
                        break;
                    }
                    case { Success: false, Exception: { } exception }:
                    {
                        logger.LogError("Failed to import asset migration folder: {Error} {Prerequisite}", exception.ToString(), lastFolder.PrintMe());
                        break;
                    }
                    case { Success: false, ModelValidationResults: { } validation }:
                    {
                        foreach (var validationResult in validation)
                        {
                            logger.LogError("Failed to import asset migration folder {Members}: {Error} - {Prerequisite}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage, lastFolder.PrintMe());
                        }

                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException($"Asset migration cannot continue, cannot prepare prerequisite - unknown result");
                    }
                }
            }
        }

        return lastFolder;
    }

    /// <inheritdoc />
    public (Guid ownerContentItemGuid, Guid assetGuid) GetRef(IMediaFile mediaFile, string? contentLanguageName = null)
    {
        var (_, translatedMediaGuid) = entityIdentityFacade.Translate(mediaFile);
        return (translatedMediaGuid, GuidHelper.CreateAssetGuid(translatedMediaGuid, contentLanguageName ?? DefaultContentLanguage));
    }

    /// <inheritdoc />
    public (Guid ownerContentItemGuid, Guid assetGuid) GetRef(ICmsAttachment attachment, string? contentLanguageName)
    {
        var (_, translatedAttachmentGuid) = entityIdentityFacade.Translate(attachment);
        return (translatedAttachmentGuid, GuidHelper.CreateAssetGuid(translatedAttachmentGuid, contentLanguageName ?? DefaultContentLanguage));
    }

    #region Asset facade DB prerequisites

    public async Task PreparePrerequisites()
    {
        foreach (var umtModel in prerequisites)
        {
            AssertSuccess(await importer.ImportAsync(umtModel), umtModel);
        }
    }

    public string GetAssetUri(IMediaFile mediaFile, string? contentLanguageName = null)
    {
        string contentLanguageSafe = contentLanguageName ?? DefaultContentLanguage;
        var (ownerContentItemGuid, _) = GetRef(mediaFile, contentLanguageName);
        return $"/getContentAsset/{ownerContentItemGuid}/{LegacyMediaFileAssetField.Guid}/{mediaFile.FileName}?language={contentLanguageSafe}";
    }

    public string GetAssetUri(ICmsAttachment attachment, string? contentLanguageName)
    {
        var (ownerContentItemGuid, _) = GetRef(attachment, contentLanguageName ?? DefaultContentLanguage);
        return $"/getContentAsset/{ownerContentItemGuid}/{LegacyAttachmentAssetField.Guid}/{attachment.AttachmentName}?language={contentLanguageName ?? DefaultContentLanguage}";
    }

    private void AssertSuccess(IImportResult importResult, IUmtModel model)
    {
        switch (importResult)
        {
            case { Success: true }:
            {
                logger.LogInformation("Asset migration prerequisite created {Prerequisite}", model.PrintMe());
                break;
            }
            case { Success: false, Exception: { } exception }:
            {
                logger.LogError("Failed to import asset migration prerequisite: {Error} {Prerequisite}", exception.ToString(), model.PrintMe());
                throw new InvalidOperationException($"Asset migration cannot continue, cannot prepare prerequisite");
            }
            case { Success: false, ModelValidationResults: { } validation }:
            {
                foreach (var validationResult in validation)
                {
                    logger.LogError("Failed to import asset migration prerequisite {Members}: {Error} - {Prerequisite}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage, model.PrintMe());
                }
                throw new InvalidOperationException($"Asset migration cannot continue, cannot prepare prerequisite");
            }
            default:
            {
                throw new InvalidOperationException($"Asset migration cannot continue, cannot prepare prerequisite - unknown result");
            }
        }
    }

    internal static readonly FormField LegacyMediaFileAssetField = new()
    {
        Column = "Asset",
        ColumnType = "contentitemasset",
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        Guid = new Guid("DFC3D011-8F63-43F6-9ED8-4B444333A1D0"),
        Properties = new FormFieldProperties { FieldCaption = "Asset", },
        Settings = new FormFieldSettings { CustomProperties = new Dictionary<string, object?> { { "AllowedExtensions", "_INHERITED_" } }, ControlName = "Kentico.Administration.ContentItemAssetUploader" }
    };
    internal static readonly DataClassModel LegacyMediaFileContentType = new()
    {
        ClassName = "Legacy.MediaFile",
        ClassType = ClassType.CONTENT_TYPE,
        ClassContentTypeType = ClassContentTypeType.REUSABLE,
        ClassGUID = new Guid("BB17604C-C134-4FAA-A401-2727CAD93707"),
        ClassDisplayName = "Legacy media file",
        ClassTableName = "Legacy_MediaFile",
        ClassLastModified = new DateTime(2024, 1, 1),
        ClassHasUnmanagedDbSchema = false,
        ClassWebPageHasUrl = false,
        Fields =
            [
                    LegacyMediaFileAssetField
            ]
    };

    internal static readonly FormField LegacyAttachmentAssetField = new()
    {
        Column = "Asset",
        ColumnType = "contentitemasset",
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        Guid = new Guid("50C2BC4C-A8FF-46BA-95C2-0E74752D147F"),
        Properties = new FormFieldProperties { FieldCaption = "Asset", },
        Settings = new FormFieldSettings { CustomProperties = new Dictionary<string, object?> { { "AllowedExtensions", "_INHERITED_" } }, ControlName = "Kentico.Administration.ContentItemAssetUploader" }
    };
    internal static readonly DataClassModel LegacyAttachmentContentType = new()
    {
        ClassName = "Legacy.Attachment",
        ClassType = ClassType.CONTENT_TYPE,
        ClassContentTypeType = ClassContentTypeType.REUSABLE,
        ClassGUID = new Guid("4E214DF1-EDD5-4441-A0B7-53849526B1D3"),
        ClassDisplayName = "Legacy attachment",
        ClassTableName = "Legacy_Attachment",
        ClassLastModified = new DateTime(2024, 1, 1),
        ClassHasUnmanagedDbSchema = false,
        ClassWebPageHasUrl = false,
        Fields =
            [
                    LegacyAttachmentAssetField
            ]
    };

    internal static ContentFolderModel GetAssetFolder(ICmsSite site)
    {
        var folderGuid = GuidHelper.CreateFolderGuid($"{site.SiteName}|{site.SiteGUID}");
        return new ContentFolderModel
        {
            ContentFolderGUID = folderGuid,
            ContentFolderName = $"{folderGuid}",
            ContentFolderDisplayName = $"{site.SiteDisplayName} assets",
            ContentFolderTreePath = $"/{site.SiteName}-assets",
            ContentFolderParentFolderGUID = null // root
        };
    }

    private static readonly IUmtModel[] prerequisites = [
            LegacyMediaFileContentType,
        LegacyAttachmentContentType
    ];


    #endregion

    private const string DirMedia = "media";
    public static string? GetMediaLibraryAbsolutePath(ToolConfiguration toolConfiguration, ICmsSite ksSite, IMediaLibrary ksMediaLibrary, ModelFacade modelFacade)
    {
        string? cmsMediaLibrariesFolder = KenticoHelper.GetSettingsKey(modelFacade, ksSite.SiteID, "CMSMediaLibrariesFolder");
        bool cmsUseMediaLibrariesSiteFolder = !"false".Equals(KenticoHelper.GetSettingsKey(modelFacade, ksSite.SiteID, "CMSUseMediaLibrariesSiteFolder"), StringComparison.InvariantCultureIgnoreCase);

        string? sourceMediaLibraryPath = null;
        if (!toolConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false) &&
                !string.IsNullOrWhiteSpace(toolConfiguration.KxCmsDirPath))
        {
            var pathParts = new List<string>();
            if (cmsMediaLibrariesFolder != null)
            {
                if (Path.IsPathRooted(cmsMediaLibrariesFolder))
                {
                    pathParts.Add(cmsMediaLibrariesFolder);
                    if (cmsUseMediaLibrariesSiteFolder)
                    {
                        pathParts.Add(ksSite.SiteName);
                    }
                    pathParts.Add(ksMediaLibrary.LibraryFolder);
                }
                else
                {
                    if (cmsMediaLibrariesFolder.StartsWith("~/"))
                    {
                        string cleared = $"{cmsMediaLibrariesFolder[2..]}".Replace("/", "\\");
                        pathParts.Add(toolConfiguration.KxCmsDirPath);
                        pathParts.Add(cleared);
                        if (cmsUseMediaLibrariesSiteFolder)
                        {
                            pathParts.Add(ksSite.SiteName);
                        }
                        pathParts.Add(ksMediaLibrary.LibraryFolder);
                    }
                    else
                    {
                        pathParts.Add(toolConfiguration.KxCmsDirPath);
                        pathParts.Add(cmsMediaLibrariesFolder);
                        if (cmsUseMediaLibrariesSiteFolder)
                        {
                            pathParts.Add(ksSite.SiteName);
                        }
                        pathParts.Add(ksMediaLibrary.LibraryFolder);
                    }
                }
            }
            else
            {
                pathParts.Add(toolConfiguration.KxCmsDirPath);
                if (cmsUseMediaLibrariesSiteFolder)
                {
                    pathParts.Add(ksSite.SiteName);
                }
                pathParts.Add(DirMedia);
                pathParts.Add(ksMediaLibrary.LibraryFolder);
            }

            sourceMediaLibraryPath = Path.Combine(pathParts.ToArray());
        }

        return sourceMediaLibraryPath;
    }
}
