using System.Diagnostics;
using CMS.DataEngine;
using CMS.MediaLibrary;
using Microsoft.Extensions.Logging;

namespace Migration.Toolkit.KXP.Api;

public class KxpMediaFileFacade
{
    private readonly ILogger<KxpMediaFileFacade> logger;

    public KxpMediaFileFacade(ILogger<KxpMediaFileFacade> logger, KxpApiInitializer kxpApiInitializer)
    {
        this.logger = logger;
        kxpApiInitializer.EnsureApiIsInitialized();
    }

    public void SetMediaFile(MediaFileInfo mfi, bool newInstance)
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

    public MediaFileInfo? GetMediaFile(Guid mediaFileGuid) => MediaFileInfoProvider.GetMediaFiles("").Where(nameof(MediaFileInfo.FileGUID), QueryOperator.Equals, mediaFileGuid).SingleOrDefault();

    public MediaLibraryInfo GetMediaLibraryInfo(Guid mediaLibraryGuid) => MediaLibraryInfoProvider.ProviderObject.Get(mediaLibraryGuid);

    public void EnsureMediaFilePathExistsInLibrary(MediaFileInfo mfi, int libraryId)
    {
        string? librarySubDir = System.IO.Path.GetDirectoryName(mfi.FilePath);
        MediaLibraryInfoProvider.CreateMediaLibraryFolder(libraryId, $"{librarySubDir}");
    }

    public MediaLibraryInfo CreateMediaLibrary(int siteId, string libraryFolder, string libraryDescription, string libraryName, string libraryDisplayName)
    {
        // Creates a new media library object
        var newLibrary = new MediaLibraryInfo
        {
            // Sets the library properties
            LibraryDisplayName = libraryDisplayName,
            LibraryName = libraryName,
            LibraryDescription = libraryDescription,
            LibraryFolder = libraryFolder
        };

        // Saves the new media library to the database
        MediaLibraryInfo.Provider.Set(newLibrary);

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

    public MediaLibraryInfo SetMediaLibrary(MediaLibraryInfo mfi)
    {
        MediaLibraryInfo.Provider.Set(mfi);

        Debug.Assert(mfi.LibraryID != 0, "mfi.LibraryID != 0");

        return mfi;
    }
}
