namespace Migration.Toolkit.Tests;

using Migration.Toolkit.Common.Helpers;

public class UnitTest1
{
    [Theory()]
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

    /*
     * Direct file path – a direct path to the media file on the file system. For example: ~/MediaLibraryFolder/sample_image.jpg. These URLs change whenever the file is renamed or moved to a different media library (directory on the file system).
     * also depends on storage settings - Settings -> Content -> Media -> Media libraries folder !!!
     */
    // to successfully implement next 2 instances, context will be needed - SiteName, Custom library path setting value
    //           ~/[SITENAME] /media/[LibraryName]   /[LibDir]/[LibDir]   /[MediaFileName].[ext]?[query]
    [InlineData("~/mysitename/media/mylibraryname/sub-dir/sub-sub-dir/somefile.png?ext=.png", "/mysitename/media/mylibraryname/sub-dir/sub-sub-dir/somefile.png", null, true, MediaKind.MediaFile, MediaLinkKind.DirectMediaPath)]
    //           ~/[CustomDir]/media/[LibraryName]   /[LibDir]/[LibDir]   /[MediaFileName].[ext]?[query]
    [InlineData("~/custom_library_global_dir/mysite/sub-dir/sub-sub-dir/somefile.png?ext=.png", "/custom_library_global_dir/mysite/sub-dir/sub-sub-dir/somefile.png", null, true, MediaKind.MediaFile, MediaLinkKind.DirectMediaPath)]
    // direct media path is not supported for absolute physical paths eg. C:\MyLibraries
    // direct media path is not supported for absolute network share paths eg. \\myserver\MyLibraries
    public void ParseMediaLink(string mediaUri, string? expectedPath, string? expectedGuid, bool expectSuccess, MediaKind expectedMediaKind, MediaLinkKind expectedMediaLinkKind) {
        Guid? expectedMediaId = Guid.TryParse(expectedGuid ?? "", out var eg) ? eg : null;

        var (success, mediaLinkKind, mediaKind, path, mediaGuid) = MediaHelper.MatchMediaLink(mediaUri);

        Assert.Equal(expectedMediaKind, mediaKind);
        Assert.Equal(expectedMediaLinkKind, mediaLinkKind);

        Assert.Equal(expectedMediaId, mediaGuid);
        Assert.Equal(expectedPath, path);
        Assert.Equal(expectSuccess, success);
    }
}