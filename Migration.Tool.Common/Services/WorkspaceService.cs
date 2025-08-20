using CMS.Workspaces;
using Kentico.Xperience.UMT.Model;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common.ContentItemOptions;
using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Common.Services;
public class WorkspaceService(IImporter importer, ILogger<WorkspaceService> logger, ToolConfiguration toolConfiguration)
{
    public Lazy<WorkspaceInfo> FallbackWorkspace { get; } = new(() =>
        {
            var workspaces = WorkspaceInfo.Provider.Get().ToArray();

            if (!string.IsNullOrEmpty(toolConfiguration.TargetWorkspaceName))
            {
                return workspaces.FirstOrDefault(x => string.Equals(x.WorkspaceName,
                        toolConfiguration.TargetWorkspaceName, StringComparison.InvariantCultureIgnoreCase))
                    ?? throw new Exception($"Target workspace '{toolConfiguration.TargetWorkspaceName}' is specified by {nameof(toolConfiguration.TargetWorkspaceName)} " +
                                           "appsettings key, but it was not found in target instance");
            }

            return workspaces.Length switch
            {
                0 => throw new Exception(
                    "No workspace found in target instance. Option 1: Create exactly one workspace. " +
                    "Option 2: Create multiple workspaces and specify target workspace by " +
                    $"{nameof(toolConfiguration.TargetWorkspaceName)} appsettings key"),
                1 => workspaces[0],
                _ => throw new Exception("Multiple workspaces found in target instance, but no target workspace " +
                                         $"is specified in appsettings. Use {nameof(toolConfiguration.TargetWorkspaceName)} appsettings key.")
            };
        });

    /// <summary>
    /// Workspace name as key
    /// </summary>
    private readonly Dictionary<string, Guid> workspaceGuidCache = [];

    /// <summary>
    /// If workspace with passed <paramref name="name"/> exists, its GUID is returned and display name is untouched.
    /// Otherwise new workspace with passed name and display name is created and its GUID is returned.
    /// </summary>
    /// <param name="name">Name of workspace</param>
    /// <param name="displayName">Display name of workspace</param>
    /// <returns>GUID of existing or created workspace</returns>
    public async Task<Guid?> EnsureWorkspace(string name, string displayName)
    {
        if (workspaceGuidCache.TryGetValue(displayName, out var cachedGuid))
        {
            return cachedGuid;
        }

        var existingWorkspaceInfo = WorkspaceInfo.Provider.Get()
            .WhereEquals(nameof(WorkspaceInfo.WorkspaceName), name)
            .FirstOrDefault();

        if (existingWorkspaceInfo is null)
        {
            var model = new WorkspaceModel
            {
                WorkspaceGUID = GuidHelper.CreateWorkspaceGuid(name),
                WorkspaceName = name,
                WorkspaceDisplayName = displayName
            };

            switch (await importer.ImportAsync(model))
            {
                case { Success: true }:
                {
                    workspaceGuidCache[name] = model.WorkspaceGUID.Value;
                    return model.WorkspaceGUID;
                }
                case { Success: false, Exception: { } exception }:
                {
                    logger.LogError("Failed to import workspace: {Error} {Prerequisite}", exception.ToString(), model.PrintMe());
                    return null;
                }
                case { Success: false, ModelValidationResults: { } validation }:
                {
                    foreach (var validationResult in validation)
                    {
                        logger.LogError("Failed to import workspace {Members}: {Error} - {Prerequisite}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage, model.PrintMe());
                    }
                    return null;
                }
                default:
                {
                    throw new InvalidOperationException($"Migration cannot continue, cannot prepare prerequisite - unknown result");
                }
            }
        }

        workspaceGuidCache[name] = existingWorkspaceInfo.WorkspaceGUID;

        return existingWorkspaceInfo.WorkspaceGUID;
    }

    public WorkspaceInfo? GetWorkspace(Guid workspaceGuid) => WorkspaceInfo.Provider.Get()
        .WhereEquals(nameof(WorkspaceInfo.WorkspaceGUID), workspaceGuid)
        .FirstOrDefault();

    public Guid? EnsureWorkspace(WorkspaceOptions? options) => options switch
    {
        null => FallbackWorkspace.Value.WorkspaceGUID,
        { Guid: { } guid } => guid,
        { Name: { } name, DisplayName: { } displayName } => EnsureWorkspace(name, displayName).GetAwaiter().GetResult(),
        _ => throw new InvalidOperationException($"{nameof(WorkspaceOptions)} has neither {nameof(WorkspaceOptions.Guid)} nor [{nameof(WorkspaceOptions.Name)} and {nameof(WorkspaceOptions.DisplayName)}] specified")
    };
}
