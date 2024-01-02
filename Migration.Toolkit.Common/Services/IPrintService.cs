namespace Migration.Toolkit.Common.Services;

public interface IPrintService
{
    string PrintKxpModelInfo<T>(T model);
    string GetEntityIdentityPrint<T>(T model, bool printType = true);
    string GetEntityIdentityPrints<T>(IEnumerable<T> models, string separator = "|");
    string PrintEnumValues<TEnum>(string separator) where TEnum : struct, Enum;
}