using CMS.ContentEngine;
using CMS.Helpers;
using CMS.Workspaces;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Services;
public class ContentFolderService
{
    public ContentFolderService(IImporter importer, ILogger<ContentFolderService> logger)
    {
        this.importer = importer;
        this.logger = logger;
        defaultWorkspaceGuid = WorkspaceInfo.Provider.Get().FirstOrDefault()?.WorkspaceGUID ?? throw new Exception("No workspace found in target instance. At least Default workspace is expected");
    }
    /// <summary>
    /// Folder tree path as key
    /// </summary>
    private readonly Dictionary<string, Guid> folderGuidCache = [];
    private readonly Guid defaultWorkspaceGuid;
    private readonly IImporter importer;
    private readonly ILogger<ContentFolderService> logger;

    /// <summary>
    /// Iterates over the folder path. If a folder doesn't exist, it gets created
    /// </summary>
    /// <param name="folderPathTemplate"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Guid?> EnsureFolderStructure(IEnumerable<(Guid Guid, string Name, string DisplayName, string PathSegmentName)> folderPathTemplate)
    {
        Guid? parentGuid = null;

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
                        ContentFolderName = CodeNameHelper.MakeUnique(folderTemplate.Name),
                        ContentFolderDisplayName = folderTemplate.DisplayName,
                        ContentFolderTreePath = currentPath,
                        ContentFolderParentFolderGUID = parentGuid,
                        ContentFolderWorkspaceGUID = defaultWorkspaceGuid
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
                            logger.LogError("Failed to import asset migration folder: {Error} {Prerequisite}", exception.ToString(), newFolderModel.PrintMe());
                            return null;
                        }
                        case { Success: false, ModelValidationResults: { } validation }:
                        {
                            foreach (var validationResult in validation)
                            {
                                logger.LogError("Failed to import asset migration folder {Members}: {Error} - {Prerequisite}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage, newFolderModel.PrintMe());
                            }
                            return null;
                        }
                        default:
                        {
                            throw new InvalidOperationException($"Asset migration cannot continue, cannot prepare prerequisite - unknown result");
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

    private static string DisplayNamePathToTreePath(string displayNamePath) => string.Join("/", displayNamePath.Split('/').Select(x => ValidationHelper.GetCodeName(x, 0)));
    private static string FolderDisplayNameToName(string displayName) => ValidationHelper.GetCodeName(displayName, 0);

    public static (Guid Guid, string Name, string DisplayName, string PathSegmentName) StandardFolderTemplate(string siteHash, string folderDisplayName, string absoluteDisplayNamePath)
        => (GuidHelper.CreateFolderGuid($"{siteHash}|{DisplayNamePathToTreePath(absoluteDisplayNamePath)}"), FolderDisplayNameToName(folderDisplayName), folderDisplayName, FolderDisplayNameToName(folderDisplayName));

    public delegate void FolderPathSegmentCallback(string segmentDisplayName, string path);
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

    public static List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)> StandardPathTemplate(string siteHash, string absolutePath)
    {
        var pathTemplate = new List<(Guid Guid, string Name, string DisplayName, string PathSegmentName)>();
        WalkFolderPath(absolutePath, (segmentDisplayName, path) => pathTemplate.Add(StandardFolderTemplate(siteHash, segmentDisplayName, path)));
        return pathTemplate;
    }

    public Task<Guid?> EnsureStandardFolderStructure(string siteHash, string absolutePath) => EnsureFolderStructure(StandardPathTemplate(siteHash, absolutePath));
}
