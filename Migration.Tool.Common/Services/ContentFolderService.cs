using CMS.ContentEngine;
using CMS.Helpers;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Services;
public class ContentFolderService(IImporter importer, ILogger<ContentFolderService> logger, WorkspaceService workspaceService)
{
    /// <summary>
    /// Folder tree path as key
    /// </summary>
    private readonly Dictionary<string, Guid> folderGuidCache = [];

    /// <summary>
    /// Iterates over the folder path. If a folder doesn't exist, it gets created
    /// </summary>
    /// <param name="folderPathTemplate">/FolderDisplayName1/FolderDisplayName2/...</param>
    /// <param name="workspaceGuid">Workspace in which to create the folder</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Guid?> EnsureFolderStructure(IEnumerable<(Guid Guid, string Name, string DisplayName, string PathSegmentName)> folderPathTemplate, Guid? workspaceGuid = null)
    {
        workspaceGuid ??= workspaceService.FallbackWorkspace.Value.WorkspaceGUID;
        Guid? parentGuid = GetWorkspaceRootFolder(workspaceGuid.Value);

        var currentPath = string.Empty;
        foreach (var folderTemplate in folderPathTemplate)
        {
            Guid newParentGuid;
            currentPath += $"/{folderTemplate.PathSegmentName}";
            if (folderGuidCache.TryGetValue(currentPath.ToString(), out var folderGuid))
            {
                newParentGuid = folderGuid;
            }
            else
            {
                var folderInfo = ContentFolderInfo.Provider.Get()
                    .WhereEquals(nameof(ContentFolderInfo.ContentFolderGUID), parentGuid)
                    .And().WhereEquals(nameof(ContentFolderInfo.ContentFolderDisplayName), folderTemplate.DisplayName)
                    .FirstOrDefault();

                if (folderInfo is null)
                {
                    var newFolderModel = new ContentFolderModel
                    {
                        ContentFolderGUID = folderTemplate.Guid,
                        ContentFolderName = UniqueNameHelper.MakeUnique(folderTemplate.Name, x => !ContentFolderInfo.Provider.Get().WhereEquals(nameof(ContentFolderInfo.ContentFolderName), x).Any()),
                        ContentFolderDisplayName = folderTemplate.DisplayName,
                        ContentFolderTreePath = currentPath,
                        ContentFolderParentFolderGUID = parentGuid,
                        ContentFolderWorkspaceGUID = workspaceGuid
                    };

                    switch (await importer.ImportAsync(newFolderModel))
                    {
                        case { Success: true }:
                        {
                            newParentGuid = folderGuidCache[currentPath] = folderTemplate.Guid;
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
                    newParentGuid = folderInfo.ContentFolderGUID;
                }
            }

            parentGuid = newParentGuid;
        }

        return parentGuid;
    }

    private readonly Dictionary<Guid, Guid> workspaceRootFolderCache = [];
    public Guid GetWorkspaceRootFolder(Guid workspaceGuid)
    {
        if (workspaceRootFolderCache.TryGetValue(workspaceGuid, out var folderGuid))
        {
            return folderGuid;
        }

        var workspace = workspaceService.GetWorkspace(workspaceGuid) ?? throw new Exception($"Required workspace(GUID={workspaceGuid}) not found");

        folderGuid = ContentFolderInfo.Provider.Get()
            .WhereEquals(nameof(ContentFolderInfo.ContentFolderWorkspaceID), workspace.WorkspaceID)
            .And().WhereEquals(nameof(ContentFolderInfo.ContentFolderParentFolderID), null)
            .FirstOrDefault()?.ContentFolderGUID
                     ?? throw new Exception($"Root folder for workspace(GUID={workspaceGuid}) not found");

        workspaceRootFolderCache[workspaceGuid] = folderGuid;

        return folderGuid;
    }

    private static string DisplayNamePathToTreePath(string displayNamePath) => string.Join("/", displayNamePath.Split('/').Select(x => ValidationHelper.GetCodeName(x, 0)));
    private static string FolderDisplayNameToName(string displayName) => ValidationHelper.GetCodeName(displayName, 0);

    /// <summary>
    /// Returns standard attributes of a new folder derived from its display name
    /// </summary>
    public static (Guid Guid, string Name, string DisplayName, string PathSegmentName) StandardFolderTemplate(string siteHash, string folderDisplayName, string absoluteDisplayNamePath)
        => (GuidHelper.CreateFolderGuid($"{siteHash}|{DisplayNamePathToTreePath(absoluteDisplayNamePath)}"), FolderDisplayNameToName(folderDisplayName), folderDisplayName, FolderDisplayNameToName(folderDisplayName));

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
    public static List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)> StandardPathTemplate(string siteHash, string absolutePath)
    {
        var pathTemplate = new List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)>();
        WalkFolderPath(absolutePath, (segmentDisplayName, path) => pathTemplate.Add(StandardFolderTemplate(siteHash, segmentDisplayName, path)));
        return pathTemplate;
    }

    /// <summary>
    /// Ensures that all folders in the folder path exist. The path segments represent display name 
    /// of the folders and the attributes of a folder that is to be created are derived from its display name 
    /// in a defined standard way
    /// </summary>
    public Task<Guid?> EnsureStandardFolderStructure(string siteHash, string absolutePath, Guid? workspaceGuid = null) => EnsureFolderStructure(StandardPathTemplate(siteHash, absolutePath), workspaceGuid);
}
