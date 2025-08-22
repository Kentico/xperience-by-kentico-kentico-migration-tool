using System.Text.RegularExpressions;
using CMS.MediaLibrary;

using Microsoft.Extensions.Logging;

using Migration.Tool.Common.Abstractions;
using Migration.Tool.Common.MigrationProtocol;
using Migration.Tool.Source.Contexts;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;

public record MediaLibraryInfoMapperSource(IMediaLibrary MediaLibrary, ICmsSite Site, Guid SafeLibraryGuid, string SafeLibraryName);

public class MediaLibraryInfoMapper(ILogger<MediaLibraryInfoMapper> logger, PrimaryKeyMappingContext primaryKeyMappingContext, IProtocol protocol)
#pragma warning disable CS0618 // Type or member is obsolete
    : EntityMapperBase<MediaLibraryInfoMapperSource, MediaLibraryInfo>(logger, primaryKeyMappingContext, protocol)
#pragma warning restore CS0618 // Type or member is obsolete
{
#pragma warning disable CS0618 // Type or member is obsolete
    protected override MediaLibraryInfo? CreateNewInstance(MediaLibraryInfoMapperSource source, MappingHelper mappingHelper, AddFailure addFailure) =>
        MediaLibraryInfo.New();
#pragma warning restore CS0618 // Type or member is obsolete

    private static readonly Regex allowedCharactersForLibraryName = new(@"[^a-zA-Z0-9_]", RegexOptions.Compiled | RegexOptions.Singleline);
#pragma warning disable CS0618 // Type or member is obsolete
    protected override MediaLibraryInfo MapInternal(MediaLibraryInfoMapperSource s, MediaLibraryInfo target, bool newInstance, MappingHelper mappingHelper, AddFailure addFailure)
#pragma warning restore CS0618 // Type or member is obsolete
    {
        var (ksLibrary, ksSite, safeLibraryGuid, safeLibraryName) = s;
        string ksSiteNameSafe = allowedCharactersForLibraryName.Replace(ksSite.SiteName, "_");
        // Sets the library properties
        target.LibraryDisplayName = ksLibrary.LibraryDisplayName;
        target.LibraryName = safeLibraryName;
        target.LibraryDescription = ksLibrary.LibraryDescription;
        target.LibraryFolder = ksLibrary.LibraryFolder;
        target.LibraryGUID = safeLibraryGuid;
        target.LibraryDisplayName = ksLibrary.LibraryDisplayName;
        target.LibraryDescription = ksLibrary.LibraryDescription;

        if (!target.LibraryFolder.StartsWith($"{ksSiteNameSafe}_", StringComparison.InvariantCultureIgnoreCase))
        {
            target.LibraryFolder = $"{ksSiteNameSafe}_{ksLibrary.LibraryFolder}";
        }

        target.LibraryLastModified = mappingHelper.Require(ksLibrary.LibraryLastModified, nameof(ksLibrary.LibraryLastModified));

        return target;
    }
}
