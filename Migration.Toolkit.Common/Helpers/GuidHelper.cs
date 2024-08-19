namespace Migration.Toolkit.Common.Helpers;

public static class GuidHelper
{
    public static readonly Guid GuidNsWebPageUrlPathInfo = new("436E024E-BA61-435F-96A7-EC7E34160DCE");
    public static readonly Guid GuidNsReusableSchema = new("2702A9E7-D859-49F0-B620-FE4268A92596");
    public static readonly Guid GuidNsDocument = new("DCBADED0-54FC-4EEC-BB50-D6E7110E499D");
    public static readonly Guid GuidNsNode = new("8691FEE4-FFFF-4642-8605-1B20B9D05360");
    public static readonly Guid GuidNsTaxonomy = new("7F23EF23-F9AE-4DB8-914B-96964E6E78E6");
    public static readonly Guid GuidNsDocumentNameField = new("8935FCE5-1BDC-4677-A4CA-6DFD32F65A0F");

    public static Guid CreateWebPageUrlPathGuid(string hash) => GuidV5.NewNameBased(GuidNsWebPageUrlPathInfo, hash);
    public static Guid CreateReusableSchemaGuid(string name) => GuidV5.NewNameBased(GuidNsReusableSchema, name);
    public static Guid CreateDocumentGuid(string name) => GuidV5.NewNameBased(GuidNsDocument, name);
    public static Guid CreateNodeGuid(string name) => GuidV5.NewNameBased(GuidNsNode, name);
    public static Guid CreateTaxonomyGuid(string name) => GuidV5.NewNameBased(GuidNsTaxonomy, name);
    public static Guid CreateDocumentNameFieldGuid(string name) => GuidV5.NewNameBased(GuidNsDocumentNameField, name);


    public static readonly Guid GuidNsLibraryFallback = new("8935FCE5-1BDC-4677-A4CA-6DFD32F65A0F");
    public static Guid CreateGuidFromLibraryAndSiteID(string libraryName, int siteId) => GuidV5.NewNameBased(GuidNsLibraryFallback, $"{libraryName}|{siteId}");
}
