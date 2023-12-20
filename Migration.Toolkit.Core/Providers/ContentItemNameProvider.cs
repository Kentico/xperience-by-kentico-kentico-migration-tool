namespace Migration.Toolkit.Core.Providers;

using System;
using System.Threading.Tasks;
using CMS.Base;
using CMS.ContentEngine.Internal;
using CMS.Helpers;

internal class ContentItemNameProvider
{
    private readonly IContentItemNameValidator codeNameValidator;


    /// <summary>
    /// Creates a new instance of <see cref="ContentItemNameProvider"/>.
    /// </summary>
    public ContentItemNameProvider(IContentItemNameValidator codeNameValidator)
    {
        this.codeNameValidator = codeNameValidator;
    }

    public Task<string> Get(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        async Task<string> Get(string name)
        {
            var codeName = ValidationHelper.GetCodeName(name, useUnicode: false);

            var isCodeNameValid = ValidationHelper.IsCodeName(codeName);

            if (string.IsNullOrEmpty(codeName) || !isCodeNameValid)
            {
                codeName = TypeHelper.GetNiceName(ContentItemInfo.OBJECT_TYPE);
            }

            var uniqueCodeNameProvider = new UniqueContentItemNameProvider(codeNameValidator);

            return await uniqueCodeNameProvider.GetUniqueValue(codeName);
        }

        return Get(name);
    }
}