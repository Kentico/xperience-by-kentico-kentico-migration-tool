namespace Migration.Tool.Common.Helpers;

public static class GuidHelper
{
    public static readonly Guid GuidNsWebPageUrlPathInfo = new("436E024E-BA61-435F-96A7-EC7E34160DCE");
    public static readonly Guid GuidNsReusableSchema = new("2702A9E7-D859-49F0-B620-FE4268A92596");
    public static readonly Guid GuidNsDocument = new("DCBADED0-54FC-4EEC-BB50-D6E7110E499D");
    public static readonly Guid GuidNsNode = new("8691FEE4-FFFF-4642-8605-1B20B9D05360");
    public static readonly Guid GuidNsTaxonomy = new("7F23EF23-F9AE-4DB8-914B-96964E6E78E6");
    public static readonly Guid GuidNsDocumentNameField = new("8935FCE5-1BDC-4677-A4CA-6DFD32F65A0F");
    public static readonly Guid GuidNsAsset = new("9CC6DE90-8993-42D8-B4C1-1429B2F780A2");
    public static readonly Guid GuidNsFolder = new("E21255AC-70F3-4A95-881A-E4AD908AF27C");
    public static readonly Guid GuidNsDataClass = new("E21255AC-70F3-4A95-881A-E4AD908AF27C");

    public static Guid CreateWebPageUrlPathGuid(string hash) => GuidV5.NewNameBased(GuidNsWebPageUrlPathInfo, hash);
    public static Guid CreateReusableSchemaGuid(string name) => GuidV5.NewNameBased(GuidNsReusableSchema, name);
    public static Guid CreateDocumentGuid(string name) => GuidV5.NewNameBased(GuidNsDocument, name);
    public static Guid CreateNodeGuid(string name) => GuidV5.NewNameBased(GuidNsNode, name);
    public static Guid CreateTaxonomyGuid(string name) => GuidV5.NewNameBased(GuidNsTaxonomy, name);
    public static Guid CreateDocumentNameFieldGuid(string name) => GuidV5.NewNameBased(GuidNsDocumentNameField, name);
    public static Guid CreateAssetGuid(Guid newMediaFileGuid, string contentLanguageCode) => GuidV5.NewNameBased(GuidNsAsset, $"{newMediaFileGuid}|{contentLanguageCode}");
    public static Guid CreateFolderGuid(string path) => GuidV5.NewNameBased(GuidNsFolder, path);
    public static Guid CreateDataClassGuid(string key) => GuidV5.NewNameBased(GuidNsDataClass, key);


    public static readonly Guid GuidNsLibraryFallback = new("8935FCE5-1BDC-4677-A4CA-6DFD32F65A0F");
    public static Guid CreateGuidFromLibraryAndSiteID(string libraryName, int siteId) => GuidV5.NewNameBased(GuidNsLibraryFallback, $"{libraryName}|{siteId}");
}
