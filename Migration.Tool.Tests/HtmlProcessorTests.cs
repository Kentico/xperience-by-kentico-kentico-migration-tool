using Migration.Tool.Common.Helpers;

namespace Migration.Tool.Tests;

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
                                               <img alt="" src="/getattachment/7c0fa1fd-f099-4516-8818-538bf8128244/Annotation-2024-07-18-162944.jpg?lang=en-US" style="width: 200px; height: 68px;" /><br />
                                               Quick image insert:<br />
                                               <img alt="Annotation-2024-07-18-162944.jpg" src="/getattachment/7c0fa1fd-f099-4516-8818-538bf8128244/Annotation-2024-07-18-162944.jpg" style="width: 1688px; height: 575px;" title="Annotation-2024-07-18-162944.jpg" /><br />
                                               <br />
                                               &nbsp;
                                               """;

    private const string HtmlFragmentSample2 = """
                                               <p>Download files:</p>
                                               <a href="https://localhost:42157/Kentico13_2024_DG_CustomMedia/media/CustomDirLibWithDirContent/document.pdf">Custom Media File</a><br />
                                               <a href="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaWithDirectPath/manual.pdf">Direct Path Media</a><br />
                                               <a href="https://localhost:51157/getmedia/a1b2c3d4-5e6f-7890-abcd-ef1234567890/whitepaper.pdf">Media File by GUID</a><br />
                                               <a href="/getattachment/9a8b7c6d-5e4f-3210-fedc-ba0987654321/report.docx?lang=en-US">Attachment by GUID</a><br />
                                               <a href="https://external.com/file.pdf">External Link</a>
                                               """;

    private const string HtmlFragmentSample3 = """
                                               <p>Mixed content:</p>
                                               <img alt="" src="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/image.jpg" /><br />
                                               <a href="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/document.pdf">Download same file</a><br />
                                               <a href="/getattachment/7c0fa1fd-f099-4516-8818-538bf8128244/file.pdf">Download attachment</a><br />
                                               <img alt="" src="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaWithDirectPath/photo.png" /><br />
                                               <a href="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaWithDirectPath/data.csv">Download direct path</a>
                                               """;

    [Fact]
    public void TestHtmlFragment1_Images()
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

    [Fact]
    public void TestHtmlFragment2_Hyperlinks()
    {
        var mediaLinkService = new MediaLinkService(
            [
                (1, "Kentico13_2024_DG_CustomMedia", "https://localhost:42157/Kentico13_2024_DG_CustomMedia"),
                (2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")
            ],
            [
                (1, "CustomMediaFolder"),
            ],
            [
                (1, "False"),
                (2, "True"),
            ],
            new Dictionary<int, HashSet<string>>
            {
                {1, new (["CustomDirLibWithDirContent"],StringComparer.InvariantCultureIgnoreCase)},
                {2, new (["MediaWithDirectPath"],StringComparer.InvariantCultureIgnoreCase)},
            }
        );

        var processor = new HtmlProcessor(HtmlFragmentSample2, mediaLinkService);

        var actual = processor.GetHyperlinks(2).ToArray();
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
                Assert.Equal(new Guid("a1b2c3d4-5e6f-7890-abcd-ef1234567890"), r.MediaGuid);
            },
            r =>
            {
                Assert.True(r.Success);
                Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                Assert.Equal(MediaKind.Attachment, r.MediaKind);
                Assert.Null(r.LibraryDir);
                Assert.Equal(new Guid("9a8b7c6d-5e4f-3210-fedc-ba0987654321"), r.MediaGuid);
            },
            r =>
            {
                Assert.False(r.Success);
                Assert.Equal(MediaLinkKind.None, r.LinkKind);
                Assert.Equal(MediaKind.None, r.MediaKind);
            });
    }

    [Fact]
    public void TestHtmlFragment3_MixedImagesAndHyperlinks()
    {
        var mediaLinkService = new MediaLinkService(
            [
                (1, "Kentico13_2024_DG_CustomMedia", "https://localhost:42157/Kentico13_2024_DG_CustomMedia"),
                (2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")
            ],
            [
                (1, "CustomMediaFolder"),
            ],
            [
                (1, "False"),
                (2, "True"),
            ],
            new Dictionary<int, HashSet<string>>
            {
                {1, new (["CustomDirLibWithDirContent"],StringComparer.InvariantCultureIgnoreCase)},
                {2, new (["MediaWithDirectPath"],StringComparer.InvariantCultureIgnoreCase)},
            }
        );

        var processor = new HtmlProcessor(HtmlFragmentSample3, mediaLinkService);

        var images = processor.GetImages(2).ToArray();

        Assert.Multiple(() =>
        {
            Assert.Equal(2, images.Length);
            Assert.Collection(images,
                r =>
                {
                    Assert.True(r.Success);
                    Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                    Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                    Assert.Equal(new Guid("f393c1be-9b65-47bb-a246-f00f1e8b1ce0"), r.MediaGuid);
                },
                r =>
                {
                    Assert.True(r.Success);
                    Assert.Equal(MediaLinkKind.DirectMediaPath, r.LinkKind);
                    Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                    Assert.Equal("MediaWithDirectPath", r.LibraryDir);
                });

            var hyperlinks = processor.GetHyperlinks(2).ToArray();
            Assert.Equal(3, hyperlinks.Length);
            Assert.Collection(hyperlinks,
                r =>
                {
                    Assert.True(r.Success);
                    Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                    Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                    Assert.Equal(new Guid("f393c1be-9b65-47bb-a246-f00f1e8b1ce0"), r.MediaGuid);
                },
                r =>
                {
                    Assert.True(r.Success);
                    Assert.Equal(MediaLinkKind.Guid, r.LinkKind);
                    Assert.Equal(MediaKind.Attachment, r.MediaKind);
                    Assert.Equal(new Guid("7c0fa1fd-f099-4516-8818-538bf8128244"), r.MediaGuid);
                },
                r =>
                {
                    Assert.True(r.Success);
                    Assert.Equal(MediaLinkKind.DirectMediaPath, r.LinkKind);
                    Assert.Equal(MediaKind.MediaFile, r.MediaKind);
                    Assert.Equal("MediaWithDirectPath", r.LibraryDir);
                });
        });
    }

    [Fact]
    public void GetHyperlinks_WithNoHyperlinks_ReturnsEmpty()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var processor = new HtmlProcessor("<p>No links here</p>", mediaLinkService);
        var actual = processor.GetHyperlinks(2).ToArray();

        Assert.Empty(actual);
    }

    [Fact]
    public void GetImages_WithNoImages_ReturnsEmpty()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var processor = new HtmlProcessor("<p>No images here</p>", mediaLinkService);
        var actual = processor.GetImages(2).ToArray();

        Assert.Empty(actual);
    }

    [Fact]
    public async Task ProcessHtml_TransformsImageLinks()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var html = """
            <img alt="" src="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/Coffee.jpg" />
            <img alt="" src="/getattachment/7c0fa1fd-f099-4516-8818-538bf8128244/file.jpg" />
            """;

        var processor = new HtmlProcessor(html, mediaLinkService);

        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            if (match.MediaKind == MediaKind.MediaFile && match.LinkKind == MediaLinkKind.Guid)
            {
                return Task.FromResult($"/new-media/{match.MediaGuid}");
            }
            if (match.MediaKind == MediaKind.Attachment && match.LinkKind == MediaLinkKind.Guid)
            {
                return Task.FromResult($"/new-attachment/{match.MediaGuid}");
            }
            return Task.FromResult(originalUrl);
        });

        Assert.Multiple(() =>
        {
            Assert.Contains("/new-media/f393c1be-9b65-47bb-a246-f00f1e8b1ce0", result);
            Assert.Contains("/new-attachment/7c0fa1fd-f099-4516-8818-538bf8128244", result);
            Assert.DoesNotContain("getmedia", result);
            Assert.DoesNotContain("getattachment", result);
        });
    }

    [Fact]
    public async Task ProcessHtml_TransformsHyperlinkUrls()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var html = """
            <a href="https://localhost:51157/getmedia/a1b2c3d4-5e6f-7890-abcd-ef1234567890/document.pdf">Download</a>
            <a href="/getattachment/9a8b7c6d-5e4f-3210-fedc-ba0987654321/report.docx">Report</a>
            """;

        var processor = new HtmlProcessor(html, mediaLinkService);

        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            if (match.MediaKind == MediaKind.MediaFile && match.LinkKind == MediaLinkKind.Guid)
            {
                return Task.FromResult($"/transformed-media/{match.MediaGuid}.pdf");
            }
            if (match.MediaKind == MediaKind.Attachment && match.LinkKind == MediaLinkKind.Guid)
            {
                return Task.FromResult($"/transformed-attachment/{match.MediaGuid}.docx");
            }
            return Task.FromResult(originalUrl);
        });

        Assert.Multiple(() =>
        {
            Assert.Contains("/transformed-media/a1b2c3d4-5e6f-7890-abcd-ef1234567890.pdf", result);
            Assert.Contains("/transformed-attachment/9a8b7c6d-5e4f-3210-fedc-ba0987654321.docx", result);
            Assert.DoesNotContain("getmedia", result);
            Assert.DoesNotContain("getattachment", result);
        });
    }

    [Fact]
    public async Task ProcessHtml_TransformsBothImagesAndHyperlinks()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>
            {
                {2, new (["MediaWithDirectPath"],StringComparer.InvariantCultureIgnoreCase)}
            }
        );

        var html = """
            <p>Image: <img src="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/image.jpg" /></p>
            <p>Download: <a href="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/file.pdf">File</a></p>
            <p>Direct: <img src="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaWithDirectPath/photo.png" /></p>
            """;

        var processor = new HtmlProcessor(html, mediaLinkService);

        var transformedCount = 0;
        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            transformedCount++;
            return Task.FromResult($"/transformed/{transformedCount}");
        });

        Assert.Multiple(() =>
        {
            Assert.Equal(3, transformedCount);
            Assert.Contains("/transformed/1", result);
            Assert.Contains("/transformed/2", result);
            Assert.Contains("/transformed/3", result);
        });
    }

    [Fact]
    public async Task ProcessHtml_LeavesUnmatchedLinksUnchanged()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var html = """
            <img src="https://external.com/image.jpg" alt="External" />
            <a href="https://google.com">Google</a>
            <img src="https://localhost:51157/getattachment/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/match.jpg" />
            """;

        var processor = new HtmlProcessor(html, mediaLinkService);

        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            return Task.FromResult($"/transformed/{match.MediaGuid}");
        });

        Assert.Multiple(() =>
        {
            Assert.Contains("https://external.com/image.jpg", result);
            Assert.Contains("https://google.com", result);
            Assert.Contains("/transformed/f393c1be-9b65-47bb-a246-f00f1e8b1ce0", result);
        });
    }

    [Fact]
    public async Task ProcessHtml_ReturnsOriginalHtmlWhenNoChanges()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var html = "<p>Just some text with no links or images</p>";

        var processor = new HtmlProcessor(html, mediaLinkService);

        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            Assert.Fail("Transformer should not be called");
            return Task.FromResult(originalUrl);
        });

        Assert.Equal(html, result);
    }

    [Fact]
    public async Task ProcessHtml_PreservesHtmlStructureAndAttributes()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>()
        );

        var html = """
            <img alt="Test Image" src="https://localhost:51157/getmedia/f393c1be-9b65-47bb-a246-f00f1e8b1ce0/img.jpg" style="width: 200px;" title="My Title" />
            <a href="https://localhost:51157/getmedia/a1b2c3d4-5e6f-7890-abcd-ef1234567890/doc.pdf" class="download-link" target="_blank">Download</a>
            """;

        var processor = new HtmlProcessor(html, mediaLinkService);

        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            return Task.FromResult($"/new/{match.MediaGuid}");
        });

        Assert.Multiple(() =>
        {
            Assert.Contains("alt=\"Test Image\"", result);
            Assert.Contains("style=\"width: 200px;\"", result);
            Assert.Contains("title=\"My Title\"", result);
            Assert.Contains("class=\"download-link\"", result);
            Assert.Contains("target=\"_blank\"", result);
            Assert.Contains("/new/f393c1be-9b65-47bb-a246-f00f1e8b1ce0", result);
            Assert.Contains("/new/a1b2c3d4-5e6f-7890-abcd-ef1234567890", result);
        });
    }

    [Fact]
    public async Task ProcessHtml_HandlesDirectMediaPath()
    {
        var mediaLinkService = new MediaLinkService(
            [(2, "MTExtensibilityTests", "https://localhost:51157/Kentico13_2024_DG_Extensibility")],
            [],
            [(2, "True")],
            new Dictionary<int, HashSet<string>>
            {
                {2, new (["MediaLibrary"],StringComparer.InvariantCultureIgnoreCase)}
            }
        );

        var html = """
            <img src="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaLibrary/photo.jpg" />
            <a href="/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaLibrary/document.pdf">Download</a>
            """;

        var processor = new HtmlProcessor(html, mediaLinkService);

        var result = await processor.ProcessHtml(2, (match, originalUrl) =>
        {
            if (match.LinkKind == MediaLinkKind.DirectMediaPath)
            {
                return Task.FromResult($"/direct/{match.LibraryDir}/file");
            }
            return Task.FromResult(originalUrl);
        });

        Assert.Multiple(() =>
        {
            Assert.Contains("/direct/MediaLibrary/file", result);
            Assert.DoesNotContain("/Kentico13_2024_DG_Extensibility/MTExtensibilityTests/media/MediaLibrary", result);
        });
    }
}
