namespace Migration.Toolkit.Common.Helpers;

public static class GuidHelper
{
    public static readonly Guid GuidNsWebPageUrlPathInfo = new Guid("436E024E-BA61-435F-96A7-EC7E34160DCE");
    public static readonly Guid GuidNsReusableSchema = new("2702A9E7-D859-49F0-B620-FE4268A92596");
    public static readonly Guid GuidNsDocument = new("DCBADED0-54FC-4EEC-BB50-D6E7110E499D");

    public static Guid CreateWebPageUrlPathGuid(string hash) => GuidV5.NewNameBased(GuidNsWebPageUrlPathInfo, hash);
    public static Guid CreateReusableSchemaGuid(string name) => GuidV5.NewNameBased(GuidNsReusableSchema, name);
    public static Guid CreateDocumentGuid(string name) => GuidV5.NewNameBased(GuidNsDocument, name);
}