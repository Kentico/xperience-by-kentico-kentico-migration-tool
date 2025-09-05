using CMS.ContentEngine;
using CMS.Helpers;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.ContentItemOptions;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Services;
public class ContentFolderService(IImporter importer, ILogger<ContentFolderService> logger, WorkspaceService workspaceService)
{
    /// <summary>
    /// Folder tree path as key
    /// </summary>
    private readonly Dictionary<string, ContentFolderInfo> folderCache = [];

    /// <summary>
    /// Iterates over the folder path. If a folder doesn't exist, it gets created
    /// </summary>
    /// <param name="folderPathTemplate">/FolderDisplayName1/FolderDisplayName2/...</param>
    /// <param name="workspaceGuid">Workspace in which to create the folder</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Guid?> EnsureFolderStructure(IEnumerable<(Guid Guid, string Name, string DisplayName, string PathSegmentName)> folderPathTemplate, Guid? workspaceGuid = null)
    {
        var workspaceInfo = workspaceGuid is not null
            ? (workspaceService.GetWorkspace(workspaceGuid!.Value) ?? workspaceService.FallbackWorkspace.Value)
            : workspaceService.FallbackWorkspace.Value;

        ContentFolderInfo? parentFolderInfo = GetWorkspaceRootFolder(workspaceInfo.WorkspaceGUID);

        string currentPath = string.Empty;
        foreach (var folderTemplate in folderPathTemplate)
        {
            ContentFolderInfo newParentFolder;
            currentPath += $"/{folderTemplate.PathSegmentName}";
            var folderCacheKey = $"{workspaceInfo.WorkspaceGUID}|{currentPath}";
            if (folderCache.TryGetValue(folderCacheKey, out var folderGuid))
            {
                newParentFolder = folderGuid;
            }
            else
            {
                var folderInfo = ContentFolderInfo.Provider.Get()
                    .And().WhereEquals(nameof(ContentFolderInfo.ContentFolderTreePath), folderTemplate.DisplayName)
                    .And().WhereEquals(nameof(ContentFolderInfo.ContentFolderWorkspaceID), workspaceInfo.WorkspaceID)
                    .FirstOrDefault();

                if (folderInfo is null)
                {
                    var newFolderModel = new ContentFolderModel
                    {
                        ContentFolderGUID = folderTemplate.Guid,
                        ContentFolderName = UniqueNameHelper.MakeUnique(folderTemplate.Name, x => !ContentFolderInfo.Provider.Get().WhereEquals(nameof(ContentFolderInfo.ContentFolderName), x).Any()),
                        ContentFolderDisplayName = folderTemplate.DisplayName,
                        ContentFolderTreePath = currentPath,
                        ContentFolderParentFolderGUID = parentFolderInfo.ContentFolderGUID,
                        ContentFolderWorkspaceGUID = workspaceInfo.WorkspaceGUID
                    };

                    switch (await importer.ImportAsync(newFolderModel))
                    {
                        case { Success: true, Imported: ContentFolderInfo importedInfo }:
                        {
                            newParentFolder = folderCache[folderCacheKey] = importedInfo;
                            break;
                        }
                        case { Success: false, Exception: { } exception }:
                        {
                            logger.LogError("Failed to import folder: {Error} {Prerequisite}", exception.ToString(), newFolderModel.PrintMe());
                            return null;
                        }
                        case { Success: false, ModelValidationResults: { } validation }:
                        {
                            foreach (var validationResult in validation)
                            {
                                logger.LogError("Failed to import folder {Members}: {Error} - {Prerequisite}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage, newFolderModel.PrintMe());
                            }
                            return null;
                        }
                        default:
                        {
                            throw new InvalidOperationException($"Migration cannot continue, cannot prepare prerequisite - unknown result");
                        }
                    }
                }
                else
                {
                    // The following inconsistency may exist in database due to migrations by previous versions of MT. If so, we patch it.
                    if (folderInfo.ContentFolderParentFolderID != parentFolderInfo.ContentFolderID)
                    {
                        folderInfo.ContentFolderParentFolderID = parentFolderInfo.ContentFolderID;
                        ContentFolderInfo.Provider.Set(folderInfo);
                    }

                    newParentFolder = folderInfo;
                }
            }

            parentFolderInfo = newParentFolder;
        }

        return parentFolderInfo.ContentFolderGUID;
    }

    private readonly Dictionary<Guid, ContentFolderInfo> workspaceRootFolderCache = [];
    public ContentFolderInfo GetWorkspaceRootFolder(Guid workspaceGuid)
    {
        if (workspaceRootFolderCache.TryGetValue(workspaceGuid, out var info))
        {
            return info;
        }

        var workspace = workspaceService.GetWorkspace(workspaceGuid) ?? throw new Exception($"Required workspace(GUID={workspaceGuid}) not found");

        info = ContentFolderInfo.Provider.Get()
            .WhereEquals(nameof(ContentFolderInfo.ContentFolderWorkspaceID), workspace.WorkspaceID)
            .And().WhereEquals(nameof(ContentFolderInfo.ContentFolderParentFolderID), null)
            .FirstOrDefault()
            ?? throw new Exception($"Root folder for workspace(GUID={workspaceGuid}) not found");

        workspaceRootFolderCache[workspaceGuid] = info;

        return info;
    }

    private static string DisplayNamePathToTreePath(string displayNamePath) => string.Join("/", displayNamePath.Split('/').Select(x => ValidationHelper.GetCodeName(x, 0)));
    private static string FolderDisplayNameToName(string displayName) => ValidationHelper.GetCodeName(displayName, 0);

    /// <summary>
    /// Returns standard attributes of a new folder derived from its display name
    /// </summary>
    public static (Guid Guid, string Name, string DisplayName, string PathSegmentName) StandardFolderTemplate(string siteHash, string folderDisplayName, string absoluteDisplayNamePath, Guid workspaceGuid)
        => (GuidHelper.CreateFolderGuid($"{workspaceGuid}|{siteHash}|{DisplayNamePathToTreePath(absoluteDisplayNamePath)}"), FolderDisplayNameToName(folderDisplayName), folderDisplayName, FolderDisplayNameToName(folderDisplayName));

    public delegate void FolderPathSegmentCallback(string segmentDisplayName, string path);

    /// <summary>
    /// Walks folder path folder by folder and for each of them invokes callback with both segment name and absolute path
    /// </summary>
    public static void WalkFolderPath(string path, FolderPathSegmentCallback segmentCallback)
    {
        var segmentDisplayNames = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < segmentDisplayNames.Length; i++)
        {
            string segmentDisplayName = segmentDisplayNames[i];
            string treePath = string.Join("/", segmentDisplayNames[..(i + 1)]);
            segmentCallback?.Invoke(segmentDisplayName, treePath);
        }
    }

    /// <summary>
    /// Accepts path where segments represent folder display names and returns path template, 
    /// i.e. sequence of folder templates, composed of standard folder templates derived from the display names
    /// </summary>
    public static List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)> StandardPathTemplate(string siteHash, string absolutePath, Guid workspaceGuid)
    {
        var pathTemplate = new List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)>();
        WalkFolderPath(absolutePath, (segmentDisplayName, path) => pathTemplate.Add(StandardFolderTemplate(siteHash, segmentDisplayName, path, workspaceGuid)));
        return pathTemplate;
    }

    /// <summary>
    /// Ensures that all folders in the folder path exist. The path segments represent display name 
    /// of the folders and the attributes of a folder that is to be created are derived from its display name 
    /// in a defined standard way
    /// </summary>
    public Task<Guid?> EnsureStandardFolderStructure(string siteHash, string absolutePath, Guid? workspaceGuid = null) => EnsureFolderStructure(StandardPathTemplate(siteHash, absolutePath, workspaceGuid!.Value), workspaceGuid);

    public Guid? EnsureFolder(ContentFolderOptions? options, bool isReusableItem, Guid? workspaceGuid = null) =>
        isReusableItem
            ? options switch
            {
                null => GetWorkspaceRootFolder(workspaceService.FallbackWorkspace.Value.WorkspaceGUID).ContentFolderGUID,
                { Guid: { } guid } => guid,
                { DisplayNamePath: { } displayNamePath } => EnsureStandardFolderStructure("customtables", displayNamePath, workspaceGuid).GetAwaiter().GetResult(),
                _ => throw new InvalidOperationException($"{nameof(ContentFolderOptions)} has neither {nameof(ContentFolderOptions.Guid)} nor {nameof(ContentFolderOptions.DisplayNamePath)} specified")
            }
            : null;
}
