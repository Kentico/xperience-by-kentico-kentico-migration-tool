using CMS.Base;
using CMS.ContentEngine.Internal;

namespace Migration.Tool.Core.KX12.Providers;

internal class UniqueContentItemNameProvider : UniqueStringValueProviderBase
{
    private readonly IContentItemNameValidator codeNameValidator;


    /// <summary>
    ///     Creates a new instance of <see cref="UniqueContentItemNameProvider" />.
    /// </summary>
    public UniqueContentItemNameProvider(IContentItemNameValidator codeNameValidator)
        : base(TypeHelper.GetMaxCodeNameLength(ContentItemInfo.TYPEINFO.MaxCodeNameLength)) => this.codeNameValidator = codeNameValidator;

    public override Task<string> GetUniqueValue(string inputValue) => base.GetUniqueValue(AddSuffix(inputValue));


    private string AddSuffix(string codeName)
    {
        string randomSuffix = GetRandomSuffix();
        string codeNameWithSuffix = codeName += randomSuffix;

        if (codeNameWithSuffix.Length > MaxLength)
        {
            int availableLength = MaxLength - randomSuffix.Length;

            codeNameWithSuffix = $"{codeName[..availableLength]}{randomSuffix}";
        }

        return codeNameWithSuffix;
    }


    /// <inheritdoc />
    protected override Task<bool> IsValueUnique(string value) => Task.FromResult(codeNameValidator.IsUnique(value));
}
