using Migration.Tool.Source.Helpers;

namespace Migration.Tool.Tests;

public class ContentItemAssetPathHelperTests
{
    private static readonly Guid ContentItemGuid = new("11111111-2222-3333-4444-555555555555");
    private static readonly Guid FieldGuid = new("DFC3D011-8F63-43F6-9ED8-4B444333A1D0");
    private static readonly Guid AssetGuid = new("66666666-7777-8888-9999-000000000000");

    [Theory]
    [InlineData(".jpg", "assets/contentitems/11/11111111-2222-3333-4444-555555555555/dfc3d011-8f63-43f6-9ed8-4b444333a1d0/66666666-7777-8888-9999-000000000000.jpg")]
    [InlineData("jpg", "assets/contentitems/11/11111111-2222-3333-4444-555555555555/dfc3d011-8f63-43f6-9ed8-4b444333a1d0/66666666-7777-8888-9999-000000000000.jpg")]
    public void GetRelativePath_BuildsExpectedPath(string extension, string expectedPath)
    {
        string path = ContentItemAssetPathHelper.GetRelativePath(ContentItemGuid, FieldGuid, AssetGuid, extension);

        Assert.Equal(expectedPath, path);
    }

    [Fact]
    public void GetRelativePath_UsesCustomCommonRootDirectoryName()
    {
        string path = ContentItemAssetPathHelper.GetRelativePath(ContentItemGuid, FieldGuid, AssetGuid, ".jpg", "customroot");

        Assert.StartsWith("customroot/contentitems/", path);
    }

    [Fact]
    public void GetRelativePath_OmitsRootSegment_WhenCommonRootDirectoryNameIsEmpty()
    {
        string path = ContentItemAssetPathHelper.GetRelativePath(ContentItemGuid, FieldGuid, AssetGuid, ".jpg", "");

        Assert.StartsWith("contentitems/", path);
    }
}
