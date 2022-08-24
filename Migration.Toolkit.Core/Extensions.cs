namespace Migration.Toolkit.Core;

using CMS.Base;
using Newtonsoft.Json;

public static class Extensions
{
    public static void SetValueAsJson<TValue>(this ISimpleDataContainer container, string column, TValue value)
    {
        container.SetValue(column, !value?.Equals(default) ?? false ? JsonConvert.SerializeObject(value) : null);
    }

    public static void SetValueAsJson<TValue>(this ISimpleDataContainer container, string column, IEnumerable<TValue> values)
    {
        container.SetValue(column, values.Any() ? JsonConvert.SerializeObject(values) : null);
    }
}