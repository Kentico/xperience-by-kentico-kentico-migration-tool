using System.Diagnostics;
using CMS.Base;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Helpers;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Common.Services;
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
        ContentFolderService contentFolderService,
        IImporter importer,
        ILogger<AssetFacade> logger,
        IProtocol protocol,
        WorkspaceService workspaceService
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

        string? mediaFilePath = null;
        if (!toolConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false))
        {
            string? mediaLibraryAbsolutePath =
                GetMediaLibraryAbsolutePath(toolConfiguration, site, mediaLibrary, modelFacade);

            if (string.IsNullOrWhiteSpace(mediaLibraryAbsolutePath))
            {
                throw new InvalidOperationException(
                    $"Invalid media file path generated for {mediaFile} and {mediaLibrary} on {site}");
            }

            mediaFilePath = Path.Combine(mediaLibraryAbsolutePath, mediaFile.FilePath);
        }

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
        languageData.AddRange(contentLanguageNames.Select(contentLanguageName =>
        {
            var contentItemData = new Dictionary<string, object?>
            {
                [LegacyMediaFileTitleField.Column!] = mediaFile.FileTitle,
                [LegacyMediaFileDescriptionField.Column!] = mediaFile.FileDescription,
            };
            if (!toolConfiguration.MigrateOnlyMediaFileInfo.GetValueOrDefault(false))
            {
                contentItemData[LegacyMediaFileAssetField.Column!] = new AssetFileSource
                {
                    ContentItemGuid = translatedMediaGuid,
                    Identifier = GuidHelper.CreateAssetGuid(translatedMediaGuid, contentLanguageName),
                    Name =
                        Path.GetFileNameWithoutExtension(mediaFile.FileName) + mediaFile.FileExtension,
                    Extension = mediaFile.FileExtension,
                    Size = null,
                    LastModified = null,
                    FilePath = mediaFilePath
                };
            }

            return new ContentItemLanguageData
            {
                LanguageName = contentLanguageName,
                DisplayName = mediaFile.FileName.Truncate(FieldConstants.ContentItemLanguageMetadataDisplayNameColumnSize),
                UserGuid = createdByUser?.UserGUID,
                VersionStatus = VersionStatus.Published,
                ContentItemData = contentItemData
            };
        }));

        string mediaFolder = Path.Combine(mediaLibrary.LibraryFolder, Path.GetDirectoryName(mediaFile.FilePath)!);

        var folderGuid = await EnsureMediaFolder(mediaFolder, site);

        string? contentItemSafeName = await Service.Resolve<IContentItemCodeNameProvider>().Get($"{mediaFile.FileName}_{translatedMediaGuid}");
        var contentItem = new ContentItemSimplifiedModel
        {
            CustomProperties = [],
            ContentItemGUID = translatedMediaGuid,
            ContentItemContentFolderGUID = folderGuid,
            ContentItemWorkspaceGUID = workspaceService.FallbackWorkspace.Value.WorkspaceGUID,
            IsSecured = null,
            ContentTypeName = LegacyMediaFileContentType.ClassName,
            Name = contentItemSafeName,
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
        foreach (string contentLanguageName in contentLanguageNames)
        {
            string assetFileName = $"{Path.GetFileNameWithoutExtension(attachment.AttachmentName)}{attachment.AttachmentExtension}";
            var contentLanguageData = new ContentItemLanguageData
            {
                LanguageName = contentLanguageName,
                DisplayName = attachment.AttachmentName.Truncate(FieldConstants.ContentItemLanguageMetadataDisplayNameColumnSize),
                UserGuid = null,
                VersionStatus = VersionStatus.Published,
                ContentItemData = new Dictionary<string, object?>
                {
                    [LegacyAttachmentAssetField.Column!] = new AssetDataSource
                    {
                        ContentItemGuid = translatedAttachmentGuid,
                        Identifier = GuidHelper.CreateAssetGuid(translatedAttachmentGuid, contentLanguageName),
                        Name = assetFileName,
                        Extension = attachment.AttachmentExtension,
                        Size = null,
                        LastModified = attachment.AttachmentLastModified,
                        Data = attachment.AttachmentBinary,
                    }
                }
            };
            languageData.Add(contentLanguageData);
        }

        string mediaFolder = "__Attachments";
        if (referencedNode != null && string.IsNullOrWhiteSpace(referencedNode.NodeAliasPath))
        {
            mediaFolder += referencedNode.NodeAliasPath.StartsWith("/") ? referencedNode.NodeAliasPath : $"/{referencedNode.NodeAliasPath}";
        }

        var rootFolder = await EnsureMediaFolder(mediaFolder, site);

        string? contentItemSafeName = await Service.Resolve<IContentItemCodeNameProvider>().Get($"{attachment.AttachmentGUID}_{translatedAttachmentGuid}");
        var contentItem = new ContentItemSimplifiedModel
        {
            ContentItemGUID = translatedAttachmentGuid,
            ContentItemContentFolderGUID = rootFolder,
            ContentItemWorkspaceGUID = workspaceService.FallbackWorkspace.Value.WorkspaceGUID,
            IsSecured = null,
            ContentTypeName = LegacyAttachmentContentType.ClassName,
            Name = contentItemSafeName,
            IsReusable = true,
            LanguageData = languageData,
        };

        return contentItem;
    }

    private ContentLanguageInfo? defaultContentLanguage;

    private async Task<Guid?> EnsureMediaFolder(string sourceFolderFilesystemPath, ICmsSite site)
    {
        string folderSubPath = sourceFolderFilesystemPath.Replace(Path.DirectorySeparatorChar, '/');

        var pathTemplate = new List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)>();

        if (toolConfiguration.LegacyFlatAssetTree == true)
        {
            // Root folder
            var rootFolderGuid = GuidHelper.CreateFolderGuid($"{site.SiteName}|{site.SiteGUID}");
            var rootFolderName = $"{site.SiteName}-assets";
            pathTemplate.Add((rootFolderGuid, $"{rootFolderGuid}", $"{site.SiteDisplayName} assets", rootFolderName));

            // Subfolders. Legacy behavior is leaf folder directly under root
            string leafFolderName = folderSubPath.Split('/').Last();
            Guid guid = GuidHelper.CreateFolderGuid($"{rootFolderGuid}|/{rootFolderName}/{folderSubPath}");
            pathTemplate.Add((guid, $"{guid}", leafFolderName, folderSubPath));
        }
        else
        {
            string rootPath = (toolConfiguration?.AssetRootFolders?.TryGetValue(site.SiteName, out var userRootPath) ?? false)
                ? $"/{userRootPath.TrimStart('/')}" : $"/{site.SiteDisplayName} assets";

            string absolutePath = $"{rootPath.TrimEnd('/')}/{folderSubPath.TrimStart('/')}";

            pathTemplate.AddRange(ContentFolderService.StandardPathTemplate(site.SiteName, absolutePath));
        }

        return await contentFolderService.EnsureFolderStructure(pathTemplate);
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
        Column = "LegacyMediaFileAsset",
        ColumnType = "contentitemasset",
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        Guid = new Guid("DFC3D011-8F63-43F6-9ED8-4B444333A1D0"),
        Properties = new FormFieldProperties { FieldCaption = "Asset", },
        Settings = new FormFieldSettings { CustomProperties = new Dictionary<string, object?> { { "AllowedExtensions", "_INHERITED_" } }, ControlName = "Kentico.Administration.ContentItemAssetUploader" }
    };

    internal static readonly FormField LegacyMediaFileTitleField = new()
    {
        Column = "LegacyMediaFileTitle",
        ColumnType = "text",
        ColumnSize = 250,
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        Guid = new Guid("83650744-916B-4E19-A31F-B0250166D47D"),
        Properties = new FormFieldProperties { FieldCaption = "Title", },
        Settings = new FormFieldSettings { ControlName = TextInputComponent.IDENTIFIER }
    };

    internal static readonly FormField LegacyMediaFileDescriptionField = new()
    {
        Column = "LegacyMediaFileDescription",
        ColumnType = "longtext",
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        ColumnSize = FieldConstants.LongTextMaxColumnSize,
        Guid = new Guid("98F43915-B540-478D-80A4-E294E631C431"),
        Properties = new FormFieldProperties { FieldCaption = "Description", },
        Settings = new FormFieldSettings { ControlName = TextAreaComponent.IDENTIFIER }
    };

    public static readonly DataClassModel LegacyMediaFileContentType = new()
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
                LegacyMediaFileAssetField,
                LegacyMediaFileTitleField,
                LegacyMediaFileDescriptionField
            ]
    };

    public static readonly FormField LegacyMediaLinkUrlField = new()
    {
        Column = "LegacyMediaLinkURL",
        ColumnType = "text",
        ColumnSize = FieldConstants.TextUrlColumnSize,
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        Guid = new Guid("28C4198D-7ABA-47AE-9A54-EDA57D8DD362"),
        Properties = new FormFieldProperties { FieldCaption = "URL", },
        Settings = new FormFieldSettings { ControlName = TextInputComponent.IDENTIFIER }
    };

    public static readonly DataClassModel LegacyMediaLinkContentType = new()
    {
        ClassName = "Legacy.MediaLink",
        ClassType = ClassType.CONTENT_TYPE,
        ClassContentTypeType = ClassContentTypeType.REUSABLE,
        ClassGUID = new Guid("205639D0-5385-472A-BD1A-BA685CF3915D"),
        ClassDisplayName = "Legacy media link",
        ClassTableName = "Legacy_MediaLink",
        ClassLastModified = new DateTime(2024, 1, 1),
        ClassHasUnmanagedDbSchema = false,
        ClassWebPageHasUrl = false,
        Fields =
        [
            LegacyMediaLinkUrlField
        ]
    };

    internal static readonly FormField LegacyAttachmentAssetField = new()
    {
        Column = "LegacyAttachmentAsset",
        ColumnType = "contentitemasset",
        AllowEmpty = true,
        Visible = true,
        Enabled = true,
        Guid = new Guid("50C2BC4C-A8FF-46BA-95C2-0E74752D147F"),
        Properties = new FormFieldProperties { FieldCaption = "Asset", },
        Settings = new FormFieldSettings { CustomProperties = new Dictionary<string, object?> { { "AllowedExtensions", "_INHERITED_" } }, ControlName = "Kentico.Administration.ContentItemAssetUploader" }
    };

    public static readonly DataClassModel LegacyAttachmentContentType = new()
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

    private static readonly IUmtModel[] prerequisites =
    [
        LegacyMediaFileContentType,
        LegacyAttachmentContentType,
        LegacyMediaLinkContentType
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
