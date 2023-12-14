namespace Migration.Toolkit.Core.Providers;

using CMS.ContentEngine.Internal;

internal class ContentItemNameValidator : IContentItemNameValidator
{
    ///<inheritdoc/>
    public bool IsUnique(string name)
    {
        return IsUnique(0, name);
    }


    ///<inheritdoc/>
    public bool IsUnique(int id, string name)
    {
        var contentItemInfo = new ContentItemInfo()
        {
            ContentItemID = id,
            ContentItemName = name,
        };

        return contentItemInfo.CheckUniqueCodeName();
    }
}