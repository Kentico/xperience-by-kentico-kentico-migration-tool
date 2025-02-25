using CMS.ContentEngine;

namespace Migration.Tool.Common.Helpers;
public static class CodeNameHelper
{
    public static string MakeUnique(string codeName)
    {
        string uniqueCodeName = codeName;
        while (ContentFolderInfo.Provider.Get()
            .WhereEquals(nameof(ContentFolderInfo.ContentFolderName), uniqueCodeName)
            .Any()) // While conflict with existing folder, try new GUID suffix to make unique
        {
            uniqueCodeName = $"{codeName}-{Guid.NewGuid().ToString()[^4..]}";
        }
        return uniqueCodeName;
    }
}
