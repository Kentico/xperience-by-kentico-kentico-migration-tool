using System.Diagnostics;
using CMS.DataEngine;
using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;

namespace Migration.Tool.KXP.Api;

public class KxpMediaFileFacade
{
    private readonly ILogger<KxpMediaFileFacade> logger;

    public KxpMediaFileFacade(ILogger<KxpMediaFileFacade> logger, KxpApiInitializer kxpApiInitializer)
    {
        this.logger = logger;
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

#pragma warning disable CS0618 // Type or member is obsolete
    public void EnsureMediaFilePathExistsInLibrary(MediaFileInfo mfi, int libraryId)
#pragma warning restore CS0618 // Type or member is obsolete
    {
        string? librarySubDir = System.IO.Path.GetDirectoryName(mfi.FilePath);
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
