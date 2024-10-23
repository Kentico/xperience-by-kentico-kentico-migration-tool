using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Tests;

public class MediaHelperTest
{
    [Theory]
    // relative permanent link media file
    [InlineData("~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg", null, "CCEAD0F0-E2BF-459B-814A-36699E5C773E", true, MediaKind.MediaFile, MediaLinkKind.Guid)]
    // relative permanent link media file with query
    [InlineData("~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg?width=300&height=100", null, "CCEAD0F0-E2BF-459B-814A-36699E5C773E", true, MediaKind.MediaFile, MediaLinkKind.Guid)]
    // absolute permanent link media file with query (http)
    [InlineData("http://somedomain.com/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg?width=300&height=100", null, "CCEAD0F0-E2BF-459B-814A-36699E5C773E", true, MediaKind.MediaFile, MediaLinkKind.Guid)]
    // absolute permanent link media file with query (https)
    [InlineData("https://somedomain.com/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg?width=300&height=100", null, "CCEAD0F0-E2BF-459B-814A-36699E5C773E", true, MediaKind.MediaFile, MediaLinkKind.Guid)]
    // absolute permanent link media file with query (//)
    [InlineData("//somedomain.com/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg?width=300&height=100", null, "CCEAD0F0-E2BF-459B-814A-36699E5C773E", true, MediaKind.MediaFile, MediaLinkKind.Guid)]
    // relative permanent link attachment file with query
    [InlineData("~/getattachment/CCEAD0F0-E2BF-459B-814A-36699E5C773E/somefile.jpeg?width=300&height=100", null, "CCEAD0F0-E2BF-459B-814A-36699E5C773E", true, MediaKind.Attachment, MediaLinkKind.Guid)]
    // relative permanent link attachment file with query and owner NodeAliasPath
    [InlineData("~/getattachment/NicePath/To/my/somefile.jpeg?width=300&height=100", "/NicePath/To/my/somefile.jpeg", null, true, MediaKind.Attachment, MediaLinkKind.Path)]
    // absolute permanent link attachment file with query and owner NodeAliasPath
    [InlineData("www.hello.local/getattachment/NicePath/To/my/somefile.jpeg?width=300&height=100", "/NicePath/To/my/somefile.jpeg", null, true, MediaKind.Attachment, MediaLinkKind.Path)]
    // absolute permanent link attachment file with query and owner NodeAliasPath, missing start /
    [InlineData("getattachment/NicePath/To/my/somefile.jpeg?width=300&height=100", "/NicePath/To/my/somefile.jpeg", null, true, MediaKind.Attachment, MediaLinkKind.Path)]
    // not supported format in portal engine, false positive - TODO
    [InlineData("~/getmedia/hello/somefile.jpeg?width=300&height=100", "/hello/somefile.jpeg", null, true, MediaKind.MediaFile, MediaLinkKind.Path)]
    // not supported format in portal engine
    [InlineData("~/get-media/hello/somefile.jpeg?width=300&height=100", null, null, false, MediaKind.None, MediaLinkKind.None)]
    // invalid format
    [InlineData("~/getmedia/", null, null, false, MediaKind.None, MediaLinkKind.None)]
    // invalid format
    [InlineData("https://some.local/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpeg", null, null, false, MediaKind.None, MediaLinkKind.None)]
    // invalid format
    [InlineData("/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpeg", null, null, false, MediaKind.None, MediaLinkKind.None)]
    // invalid format
    [InlineData(null, null, null, false, MediaKind.None, MediaLinkKind.None)]
    public void ParseMediaLink(string? mediaUri, string? expectedPath, string? expectedGuid, bool expectSuccess, MediaKind expectedMediaKind, MediaLinkKind expectedMediaLinkKind)
    {
        Guid? expectedMediaId = Guid.TryParse(expectedGuid ?? "", out var eg) ? eg : null;

        (bool success, var mediaLinkKind, var mediaKind, string? path, var mediaGuid, int? _, string? _) = new MediaLinkService([], [], [], []).MatchMediaLink(mediaUri, 1);

        Assert.Equal(expectedMediaKind, mediaKind);
        Assert.Equal(expectedMediaLinkKind, mediaLinkKind);

        Assert.Equal(expectedMediaId, mediaGuid);
        Assert.Equal(expectedPath, path);
        Assert.Equal(expectSuccess, success);
    }

    [Fact]
    public void ParseMediaLinkDirectPath()
    {
        var mediaLinkService = new MediaLinkService(
            [
                (1, "Site1", "http://localhost:5001"), // site with custom global dir
                (2, "Site2", "http://localhost:5002/SiteSubPath"), // site with custom global dir & subpath
                (3, "Site3", "http://localhost:5003"), // site without custom global media library dir
                (4, "Site4", "http://localhost:5004"), // site with custom global media library dir & without media sites folder
                // add site with live site url "localhost" - that was valid in K11
            ],
            [
                // (null, null), not set globally
                (1, "Site1MediaFolder"),
                (2, "Site2MediaFolder"),
                // (3, null) not set for site 3
                (4, "Site4MediaFolder"),
            ],
            [
                // (null, null) not set globally
                (1, "False"),
                (2, "False"),
                (3, "True"),
                (4, "True"),
            ],
            new Dictionary<int, HashSet<string>>
            {
                {1, new (["MediaLibraryS1"],StringComparer.InvariantCultureIgnoreCase)},
                {2, new (["MediaLibraryS2"],StringComparer.InvariantCultureIgnoreCase)},
                {3, new (["MediaLibraryS3"],StringComparer.InvariantCultureIgnoreCase)},
                {4, new (["MediaLibraryS4"],StringComparer.InvariantCultureIgnoreCase)}
            }
        );


        // * Site 1: ~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 1);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(1, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }

        // {sitepath}/{CMSMediaLibrariesFolder}/{LibraryFolder}/{LibrarySubFolder}/{FileName}
        // * Site 1: ~/Site1MediaFolder/MediaLibraryS1/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/Site1MediaFolder/MediaLibraryS1/myfile.jpg", 1);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS1/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(1, a.LinkSiteId);
            Assert.Equal("MediaLibraryS1", a.LibraryDir);
        }
        // {sitepath}/{CMSMediaLibrariesFolder}/{LibraryFolder}/{LibrarySubFolder}/{FileName}
        // * Site 1: http://localhost:5001/Site1MediaFolder/MediaLibraryS1/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5001/Site1MediaFolder/MediaLibraryS1/myfile.jpg", 1);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS1/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(1, a.LinkSiteId);
            Assert.Equal("MediaLibraryS1", a.LibraryDir);
        }
        // * Site 1: http://localhost:5001/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5001/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 1);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(1, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }

        // * Site 2: ~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 2);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(2, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }
        // {sitepath}/{CMSMediaLibrariesFolder}/{LibraryFolder}/{LibrarySubFolder}/{FileName}
        // * Site 2: ~/SiteSubPath/Site2MediaFolder/MediaLibraryS2/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/SiteSubPath/Site2MediaFolder/MediaLibraryS2/myfile.jpg", 2);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS2/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(2, a.LinkSiteId);
            Assert.Equal("MediaLibraryS2", a.LibraryDir);
        }
        // * Site 2: http://localhost:5002/SiteSubPath/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5002/SiteSubPath/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 2);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(2, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }
        // {sitepath}/{CMSMediaLibrariesFolder}/{LibraryFolder}/{LibrarySubFolder}/{FileName}
        // * Site 2: http://localhost:5002/SiteSubPath/Site2MediaFolder/MediaLibraryS2/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5002/SiteSubPath/Site2MediaFolder/MediaLibraryS2/myfile.jpg", 2);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS2/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(2, a.LinkSiteId);
            Assert.Equal("MediaLibraryS2", a.LibraryDir);
        }

        // * Site 3: ~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 3);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(3, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }
        // * Site 3: ~/Site3/media/MediaLibraryS3/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/Site3/media/MediaLibraryS3/myfile.jpg", 3);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS3/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(3, a.LinkSiteId);
            Assert.Equal("MediaLibraryS3", a.LibraryDir);
        }
        // * Site 3: http://localhost:5003/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5003/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 3);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(3, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }
        // * Site 3: http://localhost:5003/Site3/media/MediaLibraryS3/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5003/Site3/media/MediaLibraryS3/myfile.jpg", 3);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS3/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(3, a.LinkSiteId);
            Assert.Equal("MediaLibraryS3", a.LibraryDir);
        }

        // * Site 4: ~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 4);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(4, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }
        // * Site 4: ~/Site4MediaFolder/Site4/media/MediaLibraryS4/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("~/Site4MediaFolder/Site4/media/MediaLibraryS4/myfile.jpg", 4);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS4/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(4, a.LinkSiteId);
            Assert.Equal("MediaLibraryS4", a.LibraryDir);
        }
        // * Site 4: http://localhost:5004/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5004/getmedia/CCEAD0F0-E2BF-459B-814A-36699E5C773E/myfile.jpg", 4);
            Assert.True(a.Success);
            Assert.Null(a.Path);
            Assert.Equal(new Guid("CCEAD0F0-E2BF-459B-814A-36699E5C773E"), a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.Guid, a.LinkKind);
            Assert.Equal(4, a.LinkSiteId);
            Assert.Null(a.LibraryDir);
        }
        // * Site 4: http://localhost:5004/Site4MediaFolder/Site4/media/MediaLibraryS4/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5004/Site4MediaFolder/Site4/media/MediaLibraryS4/myfile.jpg", 4);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS4/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(4, a.LinkSiteId);
            Assert.Equal("MediaLibraryS4", a.LibraryDir);
        }

        // refs from other site
        // * Site 2: http://localhost:5004/Site4MediaFolder/Site4/media/MediaLibraryS4/myfile.jpg
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5004/Site4MediaFolder/Site4/media/MediaLibraryS4/myfile.jpg", 2);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS4/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(4, a.LinkSiteId);
            Assert.Equal("MediaLibraryS4", a.LibraryDir);
        }

        // {sitepath}/{SiteName}/media/{LibraryFolder}/{LibrarySubFolder}/{FileName}
        {
            var a = mediaLinkService.MatchMediaLink("http://localhost:5003/Site3/media/MediaLibraryS3/myfile.jpg", 3);
            Assert.True(a.Success);
            Assert.Equal("/MediaLibraryS3/myfile.jpg", a.Path);
            Assert.Null(a.MediaGuid);
            Assert.Equal(MediaKind.MediaFile, a.MediaKind);
            Assert.Equal(MediaLinkKind.DirectMediaPath, a.LinkKind);
            Assert.Equal(3, a.LinkSiteId);
            Assert.Equal("MediaLibraryS3", a.LibraryDir);
        }
    }
}
