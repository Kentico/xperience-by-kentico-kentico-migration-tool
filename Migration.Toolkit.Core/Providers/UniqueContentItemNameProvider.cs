namespace Migration.Toolkit.Core.Providers;

using System.Threading.Tasks;
using CMS.Base;
using CMS.ContentEngine.Internal;

internal class UniqueContentItemNameProvider : UniqueStringValueProviderBase
{
    private readonly IContentItemNameValidator codeNameValidator;


    /// <summary>
    /// Creates a new instance of <see cref="UniqueContentItemNameProvider"/>.
    /// </summary>
    public UniqueContentItemNameProvider(IContentItemNameValidator codeNameValidator)
        : base(TypeHelper.GetMaxCodeNameLength(ContentItemInfo.TYPEINFO.MaxCodeNameLength))
    {
        this.codeNameValidator = codeNameValidator;
    }

    public override Task<string> GetUniqueValue(string inputValue)
    {
        return base.GetUniqueValue(AddSuffix(inputValue));
    }


    private string AddSuffix(string codeName)
    {
        var randomSuffix = GetRandomSuffix();
        var codeNameWithSuffix = codeName += randomSuffix;

        if (codeNameWithSuffix.Length > MaxLength)
        {
            var availableLength = MaxLength - randomSuffix.Length;

            codeNameWithSuffix = $"{codeName[..availableLength]}{randomSuffix}";
        }

        return codeNameWithSuffix;
    }


    ///<inheritdoc/>
    protected override Task<bool> IsValueUnique(string value)
    {
        return Task.FromResult(codeNameValidator.IsUnique(value));
    }
}