using System.Collections.Concurrent;
using CMS.ContentEngine.Internal;
using CMS.Core;
using Kentico.Xperience.UMT.Services;
using Microsoft.Extensions.Logging;
using Migration.Tool.Common;
using Migration.Tool.Common.Abstractions;
using Migration.Tool.Source.Handlers;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Services;

public class MediaFileMigratorToContentItem(
    ILogger<MigrateMediaLibrariesCommandHandler> logger,
    ModelFacade modelFacade,
    IAssetFacade assetFacade,
    IImporter importer
    ) : IMediaFileMigrator
{
    public async Task<CommandResult> Handle(MigrateMediaLibrariesCommand request, CancellationToken cancellationToken)
    {
        await MigrateToAssets();
        return new GenericCommandResult();
    }

    private async Task MigrateToAssets()
    {
        var ksMediaFiles = modelFacade.SelectAll<IMediaFile>(" ORDER BY FileLibraryID");
        var ksMediaLibraries = new ConcurrentDictionary<int, IMediaLibrary?>();
        var ksSites = new ConcurrentDictionary<int, ICmsSite?>();
        var contentLanguageRetriever = Service.Resolve<IContentLanguageRetriever>();
        var defaultContentLanguage = await contentLanguageRetriever.GetDefaultContentLanguage();

        await assetFacade.PreparePrerequisites();

        foreach (var ksMediaFile in ksMediaFiles)
        {
            if (ksSites.GetOrAdd(ksMediaFile.FileSiteID, siteId => modelFacade.SelectById<ICmsSite>(siteId)) is not { } ksSite)
            {
                logger.LogError("Media file '{File}' site not found", ksMediaFile);
                continue;
            }
            if (ksMediaLibraries.GetOrAdd(ksMediaFile.FileLibraryID, libraryId => modelFacade.SelectById<IMediaLibrary>(libraryId)) is not { } ksMediaLibrary)
            {
                logger.LogError("Media file '{File}' library not found", ksMediaFile);
                continue;
            }

            var umtContentItem = await assetFacade.FromMediaFile(ksMediaFile, ksMediaLibrary, ksSite, [defaultContentLanguage.ContentLanguageName]);

            switch (await importer.ImportAsync(umtContentItem))
            {
                case { Success: true }:
                {
                    logger.LogInformation("Media file '{File}' imported", ksMediaFile.FileGUID);
                    break;
                }
                case { Success: false, Exception: { } exception }:
                {
                    logger.LogError("Media file '{File}' not migrated: {Error}", ksMediaFile.FileGUID, exception);
                    break;
                }
                case { Success: false, ModelValidationResults: { } validation }:
                {
                    foreach (var validationResult in validation)
                    {
                        logger.LogError("Media file '{File}' not migrated: {Members}: {Error}", ksMediaFile.FileGUID, string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
