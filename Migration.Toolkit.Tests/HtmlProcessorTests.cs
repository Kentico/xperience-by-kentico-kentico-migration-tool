using Migration.Toolkit.Common.Helpers;

namespace Migration.Toolkit.Tests;

public class HtmlProcessorTests
{
    private const string HtmlFragmentSample1 = """
                                               Media from other site<br />
                                               <img alt="" src="https://localhost:42157/Kentico13_2024_DG_CustomMedia/media/CustomDirLibWithDirContent/unnamed.jpg" /><br />
                                               <br />
                                               DirectPathMedia<br />
                                               <img alt="" src="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaWithDirectPath/84693449_B.png" /><br />
                                               <br />
                                               Fusion1<br />
                                               <br />
                                               <br />
                                               <br />
                                               <img alt="" src="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/Coffee-Cherries.jpg?width=200&amp;height=133" style="width: 200px; height: 133px;" /><br />
                                               <img alt="" src="/Kentico13_2024_DG_Extensibility/getattachment/7c0fa1fd-f099-4516-8818-538bf8128244/Annotation-2024-07-18-162944.jpg?lang=en-US" style="width: 200px; height: 68px;" /><br />
                                               Quick image insert:<br />
                                               <img alt="Annotation-2024-07-18-162944.jpg" src="/Kentico13_2024_DG_Extensibility/getattachment/7c0fa1fd-f099-4516-8818-538bf8128244/Annotation-2024-07-18-162944.jpg" style="width: 1688px; height: 575px;" title="Annotation-2024-07-18-162944.jpg" /><br />
                                               <br />
                                               &nbsp;
                                               """;

    [Fact]
    public void TestHtmlFragment1()
    {
        var mediaLinkService = new MediaLinkService(
            [
                (1, "Kentico13_2024_DG_CustomMedia", "https://localhost:42157/Kentico13_2024_DG_CustomMedia"),
                (2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")
            ],
            [
                // (null, null), not set globally
                (1, "CustomMediaFolder"),
                // (2, null), not set here
            ],
            [
                // (null, null) not set globally
                (1, "False"),
                (2, "True"),
            ],
            new Dictionary<int, HashSet<string>>
            {
                {1, new (["CustomDirLibWithDirContent"],StringComparer.InvariantCultureIgnoreCase)},
                {2, new (["MediaWithDirectPath"],StringComparer.InvariantCultureIgnoreCase)},
            }
        );

        var processor = new HtmlProcessor(HtmlFragmentSample1, mediaLinkService);

        var actual = processor.GetImages(2).ToArray();
        Assert.Collection(actual,
            r =>
            {
                Assert.True(r.Success);
                Assert.Equal(MediaLinkKind.DirectMediaPath, r.LinkKind);
                Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                Assert.Equal("CustomDirLibWithDirContent", r.LibraryDir);
                Assert.Null(r.MediaGuid);
            },
            r =>
            {
                Assert.True(r.Success);
                Assert.Equal(MediaLinkKind.DirectMediaPath, r.LinkKind);
                Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                Assert.Equal("MediaWithDirectPath", r.LibraryDir);
                Assert.Null(r.MediaGuid);
            },
            r =>
            {
                Assert.True(r.Success);
                Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                Assert.Null(r.LibraryDir);
                Assert.Equal(new Guid("f393c1be-9b65-47bb-a246-f00f1e8b1ce0"), r.MediaGuid);
            },
            r =>
            {
                Assert.True(r.Success);
                Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                Assert.Equal(MediaKind.Attachment, r.MediaKind);
                Assert.Null(r.LibraryDir);
                Assert.Equal(new Guid("7c0fa1fd-f099-4516-8818-538bf8128244"), r.MediaGuid);
            },
            r =>
            {
                Assert.True(r.Success);
                Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                Assert.Equal(MediaKind.Attachment, r.MediaKind);
                Assert.Null(r.LibraryDir);
                Assert.Equal(new Guid("7c0fa1fd-f099-4516-8818-538bf8128244"), r.MediaGuid);
            });
    }
}
