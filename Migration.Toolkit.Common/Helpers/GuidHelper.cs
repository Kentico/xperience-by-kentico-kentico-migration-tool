namespace Migration.Toolkit.Common.Helpers;

public static class GuidHelper
{
    public static readonly Guid GuidNsWebPageUrlPathInfo = new Guid("436E024E-BA61-435F-96A7-EC7E34160DCE");

    public static Guid CreateWebPageUrlPathGuid(string hash) => GuidV5.NewNameBased(GuidNsWebPageUrlPathInfo, hash);
}