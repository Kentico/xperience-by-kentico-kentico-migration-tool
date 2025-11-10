using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CMS.DataEngine;
using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;

namespace Migration.Tool.KXP.Api;

public class KxpMediaFileFacade
{
    private readonly ILogger<KxpMediaFileFacade> logger;
    private readonly ToolConfiguration toolConfiguration;

    public KxpMediaFileFacade(ILogger<KxpMediaFileFacade> logger, KxpApiInitializer kxpApiInitializer, ToolConfiguration toolConfiguration)
    {
        this.logger = logger;
        this.toolConfiguration = toolConfiguration;
        kxpApiInitializer.EnsureApiIsInitialized();
    }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS0618 // Type or member is obsolete
    public void SetMediaFile(MediaFileInfo mfi, bool newInstance)
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CA1822 // Mark members as static
    {
        Debug.Assert((newInstance && mfi.FileID == 0) || (!newInstance && mfi.FileID != 0), "newInstance && mfi.FileID == 0");

        if (newInstance)
        {
            mfi.SaveFileToDisk(true);
            mfi.Insert();
        }
        else
        {
            mfi.Update();
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public MediaFileInfo? GetMediaFile(Guid mediaFileGuid) => MediaFileInfoProvider.GetMediaFiles("").Where(nameof(MediaFileInfo.FileGUID), QueryOperator.Equals, mediaFileGuid).SingleOrDefault();
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
    public MediaLibraryInfo GetMediaLibraryInfo(Guid mediaLibraryGuid) => MediaLibraryInfoProvider.ProviderObject.Get(mediaLibraryGuid);
#pragma warning restore CS0618 // Type or member is obsolete

    public static bool IsDirectoryNameAllowed(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        if (!name.All(x => char.IsLetterOrDigit(x) || new char[] { '-', '_', System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }.Contains(x)))
        {
            return false;
        }

        // Reserved device names (Windows only)
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string[] reserved = {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            string upper = name.ToUpperInvariant();
            if (reserved.Contains(upper) || reserved.Any(r => upper.StartsWith(r + ".", StringComparison.Ordinal)))
            {
                return false;
            }
        }

        return true;
    }

    private readonly HashSet<string> fixedFolderPaths = [];

#pragma warning disable CS0618 // Type or member is obsolete
    public void EnsureMediaFilePathExistsInLibrary(MediaFileInfo mfi, int libraryId)
#pragma warning restore CS0618 // Type or member is obsolete
    {
        string? librarySubDir = System.IO.Path.GetDirectoryName(mfi.FilePath);
        if (toolConfiguration.LegacyFlatAssetTree != true)
        {
            if (!string.IsNullOrEmpty(librarySubDir) && !IsDirectoryNameAllowed(librarySubDir))
            {
                // Try an automatic correction - replacing stretches of whitespaces with underscore.
                // If that succeeds, inform user by a warning. Otherwise raise error and defer fixing the issue to user.
                string fixedLibrarySubdir = Regex.Replace(librarySubDir.Trim(), @"\s+", "-");

                string bypassMessage = $"If you want to bypass this, use {nameof(toolConfiguration.LegacyPermissiveMediaLibrarySubfolders)} appsettings option. Please refer to README for potential sideeffects.";

                if (IsDirectoryNameAllowed(fixedLibrarySubdir))
                {
                    librarySubDir = fixedLibrarySubdir;
                    mfi.FilePath = $"{librarySubDir}/{System.IO.Path.GetFileName(mfi.FilePath)}";   // Don't use Path.Combine, because it doesn't work correctly with subsequent internal CMS logic

                    if (!fixedFolderPaths.Contains(librarySubDir))
                    {
                        fixedFolderPaths.Add(librarySubDir);
                        logger.LogWarning($"Media library subfolder '{{MediaLibrarySubfolder}}' was transformed to XbyK allowed format '{{FixedMediaLibrarySubfolder}}'. {bypassMessage}", librarySubDir, fixedLibrarySubdir);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Media library subfolder name {librarySubDir} is not in XbyK allowed format. It must contain only alphanumeric characters, underscores ('_'), hyphens ('-'), and cannot contain file names reserved by the operating system. {bypassMessage}");
                }
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        MediaLibraryInfoProvider.CreateMediaLibraryFolder(libraryId, $"{librarySubDir}");
#pragma warning restore CS0618 // Type or member is obsolete
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public MediaLibraryInfo CreateMediaLibrary(int siteId, string libraryFolder, string libraryDescription, string libraryName, string libraryDisplayName)
#pragma warning restore CS0618 // Type or member is obsolete
    {
        // Creates a new media library object
#pragma warning disable CS0618 // Type or member is obsolete
        var newLibrary = new MediaLibraryInfo
        {
            // Sets the library properties
            LibraryDisplayName = libraryDisplayName,
            LibraryName = libraryName,
            LibraryDescription = libraryDescription,
            LibraryFolder = libraryFolder
        };
#pragma warning restore CS0618 // Type or member is obsolete

        // Saves the new media library to the database
#pragma warning disable CS0618 // Type or member is obsolete
        MediaLibraryInfo.Provider.Set(newLibrary);
#pragma warning restore CS0618 // Type or member is obsolete

        logger.LogTrace("Emit library {Info}", new
        {
            siteId,
            libraryFolder,
            libraryDescription,
            libraryName,
            libraryDisplayName
        });

        return newLibrary;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public MediaLibraryInfo SetMediaLibrary(MediaLibraryInfo mfi)
#pragma warning restore CS0618 // Type or member is obsolete
    {
#pragma warning disable CS0618 // Type or member is obsolete
        MediaLibraryInfo.Provider.Set(mfi);
#pragma warning restore CS0618 // Type or member is obsolete

        Debug.Assert(mfi.LibraryID != 0, "mfi.LibraryID != 0");

        return mfi;
    }
}
