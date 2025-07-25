namespace Migration.Tool.Common.Helpers;

public static class GuidHelper
{
    public static readonly Guid GuidNsWebPageUrlPathInfo = new("436E024E-BA61-435F-96A7-EC7E34160DCE");
    public static readonly Guid GuidNsWebPageFormerUrlPathInfo = new("7ECD3157-4BD0-4ABF-AFA5-4B6FF901AF25");
    public static readonly Guid GuidNsReusableSchema = new("2702A9E7-D859-49F0-B620-FE4268A92596");
    public static readonly Guid GuidNsDocument = new("DCBADED0-54FC-4EEC-BB50-D6E7110E499D");
    public static readonly Guid GuidNsNode = new("8691FEE4-FFFF-4642-8605-1B20B9D05360");
    public static readonly Guid GuidNsTaxonomy = new("7F23EF23-F9AE-4DB8-914B-96964E6E78E6");
    public static readonly Guid GuidNsField = new("8935FCE5-1BDC-4677-A4CA-6DFD32F65A0F");
    public static readonly Guid GuidNsAsset = new("9CC6DE90-8993-42D8-B4C1-1429B2F780A2");
    public static readonly Guid GuidNsFolder = new("E21255AC-70F3-4A95-881A-E4AD908AF27C");
    public static readonly Guid GuidNsWorkspace = new("F198C350-EB93-45CF-90E0-FA336DA2846C");
    public static readonly Guid GuidNsDataClass = new("E21255AC-70F3-4A95-881A-E4AD908AF27C");
    public static readonly Guid GuidNsContentItem = new("EEBBD8D5-BA56-492F-969E-58E77EE90055");
    public static readonly Guid GuidNsContentItemCommonData = new("31BA319C-843F-482A-9841-87BC62062DC2");
    public static readonly Guid GuidNsContentItemLanguageMetadata = new("AAC0C3A9-3DE7-436E-AFAB-49C1E29D5DE2");
    public static readonly Guid GuidNsContentItemReference = new("9FEDEA1C-C677-4026-B5E8-1A83EC501D06");

    public static Guid CreateWebPageUrlPathGuid(string hash) => GuidV5.NewNameBased(GuidNsWebPageUrlPathInfo, hash);
    public static Guid CreateWebPageFormerUrlPathGuid(string hash) => GuidV5.NewNameBased(GuidNsWebPageFormerUrlPathInfo, hash);
    public static Guid CreateReusableSchemaGuid(string name) => GuidV5.NewNameBased(GuidNsReusableSchema, name);
    public static Guid CreateDocumentGuid(string name) => GuidV5.NewNameBased(GuidNsDocument, name);
    public static Guid CreateNodeGuid(string name) => GuidV5.NewNameBased(GuidNsNode, name);
    public static Guid CreateTaxonomyGuid(string name) => GuidV5.NewNameBased(GuidNsTaxonomy, name);
    public static Guid CreateFieldGuid(string name) => GuidV5.NewNameBased(GuidNsField, name);
    public static Guid CreateAssetGuid(Guid newMediaFileGuid, string contentLanguageCode) => GuidV5.NewNameBased(GuidNsAsset, $"{newMediaFileGuid}|{contentLanguageCode}");
    public static Guid CreateFolderGuid(string path) => GuidV5.NewNameBased(GuidNsFolder, path);
    public static Guid CreateWorkspaceGuid(string name) => GuidV5.NewNameBased(GuidNsWorkspace, name);
    public static Guid CreateDataClassGuid(string key) => GuidV5.NewNameBased(GuidNsDataClass, key);
    public static Guid CreateContentItemGuid(string key) => GuidV5.NewNameBased(GuidNsContentItem, key);
    public static Guid CreateContentItemReferenceGuid(string key) => GuidV5.NewNameBased(GuidNsContentItemReference, key);
    public static Guid CreateContentItemCommonDataGuid(string key) => GuidV5.NewNameBased(GuidNsContentItemCommonData, key);
    public static Guid CreateContentItemLanguageMetadataGuid(string key) => GuidV5.NewNameBased(GuidNsContentItemLanguageMetadata, key);



    public static readonly Guid GuidNsLibraryFallback = new("8935FCE5-1BDC-4677-A4CA-6DFD32F65A0F");
    public static Guid CreateGuidFromLibraryAndSiteID(string libraryName, int siteId) => GuidV5.NewNameBased(GuidNsLibraryFallback, $"{libraryName}|{siteId}");
}
